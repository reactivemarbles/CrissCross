// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DocumentsInline = Avalonia.Controls.Documents.Inline;
using DocumentsInlineUIContainer = Avalonia.Controls.Documents.InlineUIContainer;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// A control that displays formatted text using Inlines.
/// </summary>
public class FormattedTextPresenter : TextBlock
{
    /// <summary>
    /// Property for <see cref="Document"/>.
    /// </summary>
    public static readonly StyledProperty<FlowDocument?> DocumentProperty =
        AvaloniaProperty.Register<FormattedTextPresenter, FlowDocument?>(nameof(Document));

    /// <summary>
    /// Property for <see cref="DefaultForeground"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush?> DefaultForegroundProperty =
        AvaloniaProperty.Register<FormattedTextPresenter, IBrush?>(nameof(DefaultForeground));

    /// <summary>
    /// Property for <see cref="DefaultFontSize"/>.
    /// </summary>
    public static readonly StyledProperty<double> DefaultFontSizeProperty =
        AvaloniaProperty.Register<FormattedTextPresenter, double>(nameof(DefaultFontSize), 14);

    private const string InlineObjectBoundarySentinel = "\u200B";

    /// <summary>
    /// Initializes a new instance of the <see cref="FormattedTextPresenter"/> class.
    /// </summary>
    public FormattedTextPresenter()
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether HTTP/HTTPS image sources may be resolved by <see cref="RemoteImageLoader"/>.
    /// </summary>
    public bool IsRemoteImageLoadingEnabled { get; set; }

    /// <summary>
    /// Gets or sets the opt-in remote image loader used for HTTP/HTTPS image sources.
    /// </summary>
    public Func<Uri, IImage?>? RemoteImageLoader { get; set; }

    /// <summary>
    /// Gets or sets the document to display.
    /// </summary>
    public FlowDocument? Document
    {
        get => GetValue(DocumentProperty);
        set => SetValue(DocumentProperty, value);
    }

    /// <summary>
    /// Gets or sets the default foreground brush.
    /// </summary>
    public IBrush? DefaultForeground
    {
        get => GetValue(DefaultForegroundProperty);
        set => SetValue(DefaultForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the default font size.
    /// </summary>
    public double DefaultFontSize
    {
        get => GetValue(DefaultFontSizeProperty);
        set => SetValue(DefaultFontSizeProperty, value);
    }

    /// <summary>
    /// Updates the inline collection from the document.
    /// </summary>
    public void UpdateInlines()
    {
        Inlines?.Clear();

        if (Document is null || Document.Segments.Count == 0)
        {
            return;
        }

        foreach (var segment in Document.Segments)
        {
            if (segment.IsParagraphBreak)
            {
                Inlines!.Add(new LineBreak());
                Inlines.Add(new LineBreak());
                continue;
            }

            if (segment.IsLineBreak)
            {
                Inlines!.Add(new LineBreak());
                continue;
            }

            if (segment.IsImage)
            {
                // Keep the preceding shaped text run splittable when Avalonia wraps before an embedded inline.
                AppendInlineObjectBoundarySentinel();
                var imageInline = CreateImageInline(segment);
                if (imageInline is not null)
                {
                    Inlines!.Add(imageInline);
                }

                continue;
            }

            if (!segment.HasRenderableText)
            {
                continue;
            }

            var run = new Run(segment.Text)
            {
                FontWeight = segment.FontWeight,
                FontStyle = segment.FontStyle,
                FontSize = segment.FontSize ?? DefaultFontSize,
                FontFamily = segment.FontFamily ?? FontFamily,
            };

            if (segment.TextDecorations is { } decorations)
            {
                run.TextDecorations = decorations;
            }

            run.Foreground = segment.Foreground ?? DefaultForeground ?? Foreground;
            if (segment.Background is not null)
            {
                run.Background = segment.Background;
            }

            Inlines!.Add(run);
        }
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change is null)
        {
            return;
        }

        if (change.Property == DocumentProperty ||
            change.Property == DefaultForegroundProperty ||
            change.Property == DefaultFontSizeProperty ||
            change.Property == ForegroundProperty ||
            change.Property == FontSizeProperty ||
            change.Property == FontFamilyProperty)
        {
            UpdateInlines();
        }
    }

    private DocumentsInline? CreateImageInline(TextSegment segment)
    {
        if (string.IsNullOrWhiteSpace(segment.ImageSource))
        {
            return null;
        }

        if (!TryLoadImage(segment.ImageSource, out var bitmap))
        {
            return null;
        }

        var image = new Image
        {
            Source = bitmap,
            Stretch = Stretch.Uniform,
            HorizontalAlignment = segment.ImageAlignment,
        };

        if (segment.ImageWidth.HasValue)
        {
            image.Width = segment.ImageWidth.Value;
        }

        if (segment.ImageHeight.HasValue)
        {
            image.Height = segment.ImageHeight.Value;
        }

        return new DocumentsInlineUIContainer
        {
            Child = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Child = image,
            }
        };
    }

    private bool TryLoadImage(string source, out IImage? bitmap)
    {
        bitmap = null;

        try
        {
            if (TryLoadDataUri(source, out bitmap))
            {
                return true;
            }

            if (Uri.TryCreate(source, UriKind.Absolute, out var absolute))
            {
                if (absolute.IsFile)
                {
                    var localPath = absolute.LocalPath;
                    if (File.Exists(localPath))
                    {
                        using var fileStream = File.OpenRead(localPath);
                        bitmap = CreateBitmapFromStream(fileStream);
                        return true;
                    }

                    return false;
                }

                if (absolute.Scheme is "http" or "https")
                {
                    return TryLoadRemoteImage(absolute, out bitmap);
                }

                using var assetStream = AssetLoader.Open(absolute);
                bitmap = CreateBitmapFromStream(assetStream);
                return true;
            }

            if (File.Exists(source))
            {
                using var fallbackFile = File.OpenRead(source);
                bitmap = CreateBitmapFromStream(fallbackFile);
                return true;
            }
        }
        catch
        {
            bitmap = null;
        }

        return false;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Kept as an instance helper to satisfy StyleCop member ordering for this control.")]
    private bool TryLoadDataUri(string source, out IImage? bitmap)
    {
        bitmap = null;
        if (!source.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var commaIndex = source.IndexOf(',');
        if (commaIndex < 0)
        {
            return false;
        }

        var metadata = source[..commaIndex];
        if (!metadata.Contains(";base64", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var payload = source[(commaIndex + 1)..].Trim();
        if (payload.Length == 0)
        {
            return false;
        }

        try
        {
            var normalizedPayload = Uri.UnescapeDataString(payload).Replace(" ", "+", StringComparison.Ordinal);
            var bytes = Convert.FromBase64String(normalizedPayload);
            if (bytes.Length == 0)
            {
                return false;
            }

            using var memory = new MemoryStream(bytes);
            bitmap = new Bitmap(memory);
            return true;
        }
        catch
        {
            bitmap = null;
            return false;
        }
    }

    private bool TryLoadRemoteImage(Uri uri, out IImage? bitmap)
    {
        bitmap = null;
        if (!IsRemoteImageLoadingEnabled || RemoteImageLoader is null)
        {
            return false;
        }

        try
        {
            bitmap = RemoteImageLoader(uri);
            return bitmap is not null;
        }
        catch
        {
            bitmap = null;
            return false;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Kept as an instance helper to satisfy StyleCop member ordering for this control.")]
    private Bitmap CreateBitmapFromStream(Stream stream)
    {
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        buffer.Position = 0;
        return new Bitmap(buffer);
    }

    private void AppendInlineObjectBoundarySentinel()
    {
        if (Inlines is null)
        {
            return;
        }

        if (Inlines.Count > 0 && Inlines[^1] is Run run)
        {
            if (run.Text?.EndsWith(InlineObjectBoundarySentinel, StringComparison.Ordinal) != true)
            {
                run.Text += InlineObjectBoundarySentinel;
            }

            return;
        }

        Inlines.Add(new Run(InlineObjectBoundarySentinel)
        {
            Foreground = Brushes.Transparent,
            FontSize = 1,
        });
    }
}
