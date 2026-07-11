// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>A control that displays formatted text using Inlines.</summary>
public class FormattedTextPresenter : TextBlock
{
    /// <summary>Property for <see cref="Document"/>.</summary>
    public static readonly StyledProperty<FlowDocument?> DocumentProperty =
        AvaloniaProperty.Register<FormattedTextPresenter, FlowDocument?>(nameof(Document));

    /// <summary>Property for <see cref="DefaultForeground"/>.</summary>
    public static readonly StyledProperty<IBrush?> DefaultForegroundProperty =
        AvaloniaProperty.Register<FormattedTextPresenter, IBrush?>(nameof(DefaultForeground));

    /// <summary>Property for <see cref="DefaultFontSize"/>.</summary>
    public static readonly StyledProperty<double> DefaultFontSizeProperty =
        AvaloniaProperty.Register<FormattedTextPresenter, double>(nameof(DefaultFontSize), 14);

    /// <summary>Provides the InlineObjectBoundarySentinel member.</summary>
    private const string InlineObjectBoundarySentinel = "\u200B";

    /// <summary>Provides the DefaultImageMaxWidth member.</summary>
    private const double DefaultImageMaxWidth = 640;

    /// <summary>Provides the DefaultImageMaxHeight member.</summary>
    private const double DefaultImageMaxHeight = 480;

    /// <summary>Gets or sets a value indicating whether HTTP/HTTPS image sources may be resolved by <see cref="RemoteImageLoader"/>.</summary>
    public bool IsRemoteImageLoadingEnabled { get; set; }

    /// <summary>Gets or sets the opt-in remote image loader used for HTTP/HTTPS image sources.</summary>
    public Func<Uri, IImage?>? RemoteImageLoader { get; set; }

    /// <summary>Gets or sets the document to display.</summary>
    public FlowDocument? Document
    {
        get => GetValue(DocumentProperty);
        set => SetValue(DocumentProperty, value);
    }

    /// <summary>Gets or sets the default foreground brush.</summary>
    public IBrush? DefaultForeground
    {
        get => GetValue(DefaultForegroundProperty);
        set => SetValue(DefaultForegroundProperty, value);
    }

    /// <summary>Gets or sets the default font size.</summary>
    public double DefaultFontSize
    {
        get => GetValue(DefaultFontSizeProperty);
        set => SetValue(DefaultFontSizeProperty, value);
    }

    /// <summary>Updates the inline collection from the document.</summary>
    public void UpdateInlines()
    {
        Inlines?.Clear();

        if (Document is null || Document.Segments.Count == 0)
        {
            return;
        }

        foreach (var segment in Document.Segments)
        {
            AppendSegment(segment);
        }
    }

    /// <summary>Creates a sized image element for a document image segment.</summary>
    /// <param name="segment">The image segment.</param>
    /// <returns>The image element, or <see langword="null"/> when the source cannot be loaded.</returns>
    internal Image? CreateImageElement(TextSegment segment)
    {
        if (string.IsNullOrWhiteSpace(segment.ImageSource) ||
            !TryLoadImage(segment.ImageSource, out var bitmap) ||
            bitmap is null)
        {
            return null;
        }

        var image = new Image
        {
            Source = bitmap,
            MaxWidth = DefaultImageMaxWidth,
            MaxHeight = DefaultImageMaxHeight,
            Stretch = Stretch.Uniform,
            StretchDirection = StretchDirection.DownOnly,
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

        if (!segment.ImageWidth.HasValue && !segment.ImageHeight.HasValue && bitmap.Size is { Width: > 0, Height: > 0 } naturalSize)
        {
            var scale = Math.Min(1, Math.Min(DefaultImageMaxWidth / naturalSize.Width, DefaultImageMaxHeight / naturalSize.Height));
            image.Width = naturalSize.Width * scale;
            image.Height = naturalSize.Height * scale;
        }

        return image;
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change is null)
        {
            return;
        }

        if (change.Property != DocumentProperty &&
            change.Property != DefaultForegroundProperty &&
            change.Property != DefaultFontSizeProperty &&
            change.Property != ForegroundProperty &&
            change.Property != FontSizeProperty &&
            change.Property != FontFamilyProperty)
        {
            return;
        }

        UpdateInlines();
    }

    /// <summary>Provides the TryLoadDataUri member.</summary>
    /// <param name="source">The image source.</param>
    /// <param name="bitmap">The loaded bitmap.</param>
    /// <returns><see langword="true"/> when the data URI was loaded.</returns>
    private static bool TryLoadDataUri(string source, out IImage? bitmap)
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

    /// <summary>Provides the CreateBitmapFromStream member.</summary>
    /// <param name="stream">The source stream.</param>
    /// <returns>The decoded bitmap.</returns>
    private static Bitmap CreateBitmapFromStream(Stream stream)
    {
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        buffer.Position = 0;
        return new Bitmap(buffer);
    }

    /// <summary>Loads an image from a local file.</summary>
    /// <param name="path">The local image path.</param>
    /// <returns>The loaded image, or <see langword="null"/> when the file does not exist.</returns>
    private static Bitmap? LoadImageFile(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        using var stream = File.OpenRead(path);
        return CreateBitmapFromStream(stream);
    }

    /// <summary>Appends one segment to the inline collection.</summary>
    /// <param name="segment">The segment to append.</param>
    private void AppendSegment(TextSegment segment)
    {
        if (segment.IsParagraphBreak)
        {
            AppendParagraphBreak();
            return;
        }

        if (segment.IsLineBreak)
        {
            AppendLineBreak();
            return;
        }

        if (segment.IsImage)
        {
            AppendImageSegment();
            return;
        }

        if (!segment.HasRenderableText)
        {
            return;
        }

        AppendRunSegment(segment);
    }

    /// <summary>Appends a paragraph break.</summary>
    private void AppendParagraphBreak()
    {
        Inlines!.Add(new LineBreak());
        Inlines.Add(new LineBreak());
    }

    /// <summary>Appends a line break.</summary>
    private void AppendLineBreak() => Inlines!.Add(new LineBreak());

    /// <summary>Appends an image segment.</summary>
    private void AppendImageSegment()
    {
        // The composed image overlay owns rendering; these inlines reserve a stable document boundary.
        AppendInlineObjectBoundarySentinel();
        Inlines!.Add(new LineBreak());
    }

    /// <summary>Appends a text run segment.</summary>
    /// <param name="segment">The text segment.</param>
    private void AppendRunSegment(TextSegment segment)
    {
        var run = new Run(segment.Text)
        {
            FontWeight = segment.FontWeight,
            FontStyle = segment.FontStyle,
            FontSize = segment.FontSize ?? DefaultFontSize,
            FontFamily = segment.FontFamily ?? FontFamily,
            Foreground = segment.Foreground ?? DefaultForeground ?? Foreground,
        };

        if (segment.TextDecorations is { } decorations)
        {
            run.TextDecorations = decorations;
        }

        if (segment.Background is not null)
        {
            run.Background = segment.Background;
        }

        Inlines!.Add(run);
    }

    /// <summary>Provides the TryLoadImage member.</summary>
    /// <param name="source">The source value.</param>
    /// <param name="bitmap">The bitmap value.</param>
    /// <returns>The result.</returns>
    private bool TryLoadImage(string source, out IImage? bitmap)
    {
        try
        {
            bitmap = LoadImage(source);
            return bitmap is not null;
        }
        catch (IOException)
        {
            bitmap = null;
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            bitmap = null;
            return false;
        }
        catch (InvalidOperationException)
        {
            bitmap = null;
            return false;
        }
        catch (NotSupportedException)
        {
            bitmap = null;
            return false;
        }
        catch (ArgumentException)
        {
            bitmap = null;
            return false;
        }
    }

    /// <summary>Loads one supported image source.</summary>
    /// <param name="source">The image source.</param>
    /// <returns>The loaded image, or <see langword="null"/> when no supported source exists.</returns>
    private IImage? LoadImage(string source)
    {
        if (TryLoadDataUri(source, out var dataImage))
        {
            return dataImage;
        }

        if (!Uri.TryCreate(source, UriKind.Absolute, out var absolute))
        {
            return LoadImageFile(source);
        }

        if (absolute.IsFile)
        {
            return LoadImageFile(absolute.LocalPath);
        }

        if (absolute.Scheme is "http" or "https")
        {
            return TryLoadRemoteImage(absolute, out var remoteImage) ? remoteImage : null;
        }

        using var assetStream = AssetLoader.Open(absolute);
        return CreateBitmapFromStream(assetStream);
    }

    /// <summary>Provides the TryLoadRemoteImage member.</summary>
    /// <param name="uri">The uri value.</param>
    /// <param name="bitmap">The bitmap value.</param>
    /// <returns>The result.</returns>
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

    /// <summary>Provides the AppendInlineObjectBoundarySentinel member.</summary>
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
