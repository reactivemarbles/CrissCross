// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// A control that displays formatted text using Inlines.
/// </summary>
public class FormattedTextPresenter : SelectableTextBlock
{
    /// <summary>
    /// Property for <see cref="Document"/>.
    /// </summary>
    public static readonly StyledProperty<RichTextDocument?> DocumentProperty =
        AvaloniaProperty.Register<FormattedTextPresenter, RichTextDocument?>(nameof(Document));

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

    /// <summary>
    /// Initializes a new instance of the <see cref="FormattedTextPresenter"/> class.
    /// </summary>
    public FormattedTextPresenter()
    {
        // Enable text selection
        SelectionBrush = new SolidColorBrush(Color.FromArgb(100, 0, 120, 212));
    }

    /// <summary>
    /// Gets or sets the document to display.
    /// </summary>
    public RichTextDocument? Document
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

        Inlines ??= [];

        foreach (var segment in Document.Segments)
        {
            var run = new Run(segment.Text)
            {
                FontWeight = segment.FontWeight,
                FontStyle = segment.FontStyle,
            };

            // Apply text decorations
            if (segment.TextDecorations is { } decorations)
            {
                run.TextDecorations = decorations;
            }

            // Apply custom foreground
            if (segment.Foreground is not null)
            {
                run.Foreground = segment.Foreground;
            }
            else if (DefaultForeground is not null)
            {
                run.Foreground = DefaultForeground;
            }

            // Apply custom font size
            if (segment.FontSize.HasValue)
            {
                run.FontSize = segment.FontSize.Value;
            }

            // Apply custom font family
            if (segment.FontFamily is not null)
            {
                run.FontFamily = segment.FontFamily;
            }

            Inlines.Add(run);
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
}
