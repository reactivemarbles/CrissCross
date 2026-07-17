// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace CrissCross.WPF.UI.Controls.BBCode;

/// <summary>Creates block-like WPF elements for BBCode nodes.</summary>
internal sealed partial class BbCodeRenderer
{
    /// <summary>The CrissCross control-fill resource key.</summary>
    private const string ControlFillBrushKey = "ControlFillColorDefaultBrush";

    /// <summary>The compact layout spacing.</summary>
    private const double CompactSpacing = 4D;

    /// <summary>The standard layout spacing.</summary>
    private const double StandardSpacing = 8D;

    /// <summary>The wide layout spacing.</summary>
    private const double WideSpacing = 12D;

    /// <summary>The code and media horizontal padding.</summary>
    private const double ContentPadding = 10D;

    /// <summary>The maximum gallery image preview width.</summary>
    private const double MaximumImagePreviewWidth = 640D;

    /// <summary>The width of the quote accent stroke.</summary>
    private const double QuoteStrokeWidth = 3D;

    /// <summary>The blur effect radius.</summary>
    private const double TextBlurRadius = 4D;

    /// <summary>The indentation for list content.</summary>
    private const double ListIndent = 16D;

    /// <summary>The spacing after a list marker.</summary>
    private const double ListMarkerSpacing = 6D;

    /// <summary>The minimum width for aligned block content.</summary>
    private const double MinimumAlignedBlockWidth = 320D;

    /// <summary>The largest heading font size.</summary>
    private const double HeadingBaseFontSize = 32D;

    /// <summary>The font-size reduction for each heading level.</summary>
    private const double HeadingLevelStep = 3D;

    /// <summary>The default heading level.</summary>
    private const int DefaultHeadingLevel = 2;

    /// <summary>The largest heading level.</summary>
    private const int MaximumHeadingLevel = 6;

    /// <summary>The default maximum rating.</summary>
    private const int DefaultMaximumRating = 5;

    /// <summary>The largest supported rating maximum.</summary>
    private const int MaximumRating = 10;

    /// <summary>The minimum horizontal-rule width.</summary>
    private const double MinimumSeparatorWidth = 240D;

    /// <summary>Renders an image node.</summary>
    /// <param name="node">The image node.</param>
    /// <returns>The image container.</returns>
    private InlineUIContainer CreateImage(BbCodeNode node)
    {
        var source = node.Attributes.TryGetValue("src", out var src) ? src : node.GetText().Trim();
        var legacyParts = (node.Value ?? string.Empty).Split(',');
        if (legacyParts.Length > 0 && BbCodeRenderHelpers.LooksLikeAddress(legacyParts[0]))
        {
            source = legacyParts[0];
            BbCodeRenderHelpers.AddLegacyImageAttributes(node, legacyParts.Skip(1));
        }

        Image image = new() { Stretch = Stretch.Uniform, MaxWidth = MaximumImagePreviewWidth };
        image.SetResourceReference(FrameworkElement.ToolTipProperty, "BBCodeBlockImageUnavailableText");
        if (BbCodeRenderHelpers.TryCreateBitmap(source, out var bitmap))
        {
            image.Source = bitmap;
        }

        BbCodeRenderHelpers.ApplyImageSize(image, node);
        StackPanel panel = new() { Orientation = Orientation.Vertical };
        _ = panel.Children.Add(image);
        var hasCaption =
            node.Attributes.TryGetValue("alt", out var alt) || node.Attributes.TryGetValue("title", out alt);
        if (!hasCaption && BbCodeRenderHelpers.LooksLikeAddress(legacyParts[0]))
        {
            alt = node.GetText().Trim();
            hasCaption = alt.Length > 0;
        }

        if (hasCaption)
        {
            var caption = CreateThemedTextBlock();
            caption.Text = alt;
            caption.TextAlignment = TextAlignment.Center;
            _ = panel.Children.Add(caption);
        }

        return new(panel) { BaselineAlignment = BaselineAlignment.Center };
    }

    /// <summary>Renders a quotation block.</summary>
    /// <param name="node">The quote node.</param>
    /// <returns>The quote container.</returns>
    private InlineUIContainer CreateQuote(BbCodeNode node)
    {
        StackPanel content = new();
        if (!string.IsNullOrWhiteSpace(node.Value))
        {
            var author = CreateThemedTextBlock();
            author.Text = node.Value + " wrote:";
            author.FontWeight = FontWeights.SemiBold;
            author.Margin = new(0D, 0D, 0D, CompactSpacing);
            _ = content.Children.Add(author);
        }

        _ = content.Children.Add(CreateTextBlock(node.Children));
        var border = CreateThemedBorder("CardBackgroundFillColorDefaultBrush");
        border.BorderThickness = new(QuoteStrokeWidth, 0D, 0D, 0D);
        border.Padding = new(WideSpacing, StandardSpacing, WideSpacing, StandardSpacing);
        border.Margin = new(0D, CompactSpacing, 0D, CompactSpacing);
        border.Child = content;
        return new(border) { BaselineAlignment = BaselineAlignment.Center };
    }

    /// <summary>Renders an expandable spoiler.</summary>
    /// <param name="node">The spoiler node.</param>
    /// <returns>The spoiler container.</returns>
    private InlineUIContainer CreateSpoiler(BbCodeNode node)
    {
        Expander expander = new()
        {
            Header = string.IsNullOrWhiteSpace(node.Value) ? "Spoiler" : node.Value,
            IsExpanded = false,
            Content = CreateTextBlock(node.Children),
            Padding = new(StandardSpacing),
            Margin = new(0D, CompactSpacing, 0D, CompactSpacing),
        };
        expander.SetResourceReference(Control.BackgroundProperty, ControlFillBrushKey);
        expander.SetResourceReference(Control.ForegroundProperty, "TextFillColorPrimaryBrush");
        return new(expander) { BaselineAlignment = BaselineAlignment.Center };
    }

    /// <summary>Renders blurred content.</summary>
    /// <param name="node">The blur node.</param>
    /// <returns>The blur container.</returns>
    private InlineUIContainer CreateBlur(BbCodeNode node)
    {
        var textBlock = CreateTextBlock(node.Children);
        textBlock.Effect = new BlurEffect { Radius = TextBlurRadius };
        if (!string.IsNullOrWhiteSpace(node.Value))
        {
            BbCodeRenderHelpers.ApplyForeground(textBlock, node.Value);
        }

        textBlock.ToolTip = "Blurred text";
        return new(textBlock) { BaselineAlignment = BaselineAlignment.Center };
    }

    /// <summary>Renders literal code or preformatted text.</summary>
    /// <param name="node">The literal node.</param>
    /// <returns>The code container.</returns>
    private InlineUIContainer CreateCode(BbCodeNode node)
    {
        var content = CreateThemedTextBlock();
        content.FontFamily = new("Consolas");
        content.Text = node.GetText();
        content.TextWrapping = TextWrapping.Wrap;
        if (!string.IsNullOrWhiteSpace(node.Value))
        {
            content.ToolTip = "Language: " + node.Value;
        }

        var border = CreateThemedBorder(ControlFillBrushKey);
        border.Padding = new(ContentPadding, StandardSpacing, ContentPadding, StandardSpacing);
        border.Margin = new(0D, CompactSpacing, 0D, CompactSpacing);
        border.Child = content;
        return new(border) { BaselineAlignment = BaselineAlignment.Center };
    }

    /// <summary>Renders an ordered or unordered list.</summary>
    /// <param name="node">The list node.</param>
    /// <returns>The list container.</returns>
    private InlineUIContainer CreateList(BbCodeNode node)
    {
        StackPanel panel = new() { Margin = new(ListIndent, CompactSpacing, 0D, CompactSpacing) };
        var items = BbCodeRenderHelpers.ExtractListItems(node);
        var markerStyle = node.Name == "ol" ? "1" : node.Value ?? "disc";
        for (var index = 0; index < items.Count; index++)
        {
            Grid row = new();
            row.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
            row.ColumnDefinitions.Add(new() { Width = new(1D, GridUnitType.Star) });
            var marker = CreateThemedTextBlock();
            marker.Text = BbCodeRenderHelpers.CreateListMarker(markerStyle, index + 1) + " ";
            marker.Margin = new(0D, 0D, ListMarkerSpacing, 0D);
            var content = CreateTextBlock(items[index]);
            Grid.SetColumn(content, 1);
            _ = row.Children.Add(marker);
            _ = row.Children.Add(content);
            _ = panel.Children.Add(row);
        }

        return new(panel) { BaselineAlignment = BaselineAlignment.Center };
    }

    /// <summary>Renders a BBCode table.</summary>
    /// <param name="node">The table node.</param>
    /// <returns>The table container.</returns>
    private InlineUIContainer CreateTable(BbCodeNode node) =>
        new(CreateTableGrid(BbCodeRenderHelpers.GetRows(node))) { BaselineAlignment = BaselineAlignment.Center };

    /// <summary>Renders pipe-delimited tabular text.</summary>
    /// <param name="node">The pipe-table node.</param>
    /// <returns>The table container.</returns>
    private InlineUIContainer CreatePipeTable(BbCodeNode node)
    {
        var rows = node.GetText()
            .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim().Trim('|').Split('|').Select(value => new BbCodeNode(value.Trim())).ToList())
            .Where(row => row.Count > 0)
            .ToList();
        return new(CreateTableGrid(rows)) { BaselineAlignment = BaselineAlignment.Center };
    }

    /// <summary>Renders an aligned text block.</summary>
    /// <param name="node">The alignment node.</param>
    /// <returns>The aligned container.</returns>
    private InlineUIContainer CreateAlignedBlock(BbCodeNode node)
    {
        var textBlock = CreateTextBlock(node.Children);
        var alignment = node.Name == "align" ? node.Value : node.Name;
        textBlock.TextAlignment = alignment?.ToLowerInvariant() switch
        {
            "center" => TextAlignment.Center,
            "right" => TextAlignment.Right,
            "justify" => TextAlignment.Justify,
            _ => TextAlignment.Left,
        };
        textBlock.Width = Math.Max(_source.ActualWidth, MinimumAlignedBlockWidth);
        return new(textBlock) { BaselineAlignment = BaselineAlignment.Center };
    }

    /// <summary>Renders a heading.</summary>
    /// <param name="node">The heading node.</param>
    /// <returns>The heading container.</returns>
    private InlineUIContainer CreateHeading(BbCodeNode node)
    {
        var levelText =
            node.Name.Length == DefaultHeadingLevel && node.Name[0] == 'h'
                ? node.Name[1..]
                : node.Value ?? DefaultHeadingLevel.ToString(CultureInfo.InvariantCulture);
        _ = int.TryParse(levelText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var level);
        level = Math.Max(1, Math.Min(level == 0 ? DefaultHeadingLevel : level, MaximumHeadingLevel));
        var heading = CreateTextBlock(node.Children);
        heading.FontWeight = FontWeights.SemiBold;
        heading.FontSize = HeadingBaseFontSize - ((level - 1) * HeadingLevelStep);
        heading.Margin = new(0D, StandardSpacing, 0D, CompactSpacing);
        return new(heading) { BaselineAlignment = BaselineAlignment.Center };
    }

    /// <summary>Renders a paragraph.</summary>
    /// <param name="node">The paragraph node.</param>
    /// <returns>The paragraph container.</returns>
    private InlineUIContainer CreateParagraph(BbCodeNode node)
    {
        var paragraph = CreateTextBlock(node.Children);
        paragraph.Margin = new(0D, CompactSpacing, 0D, StandardSpacing);
        return new(paragraph) { BaselineAlignment = BaselineAlignment.Center };
    }

    /// <summary>Renders an accessible star rating.</summary>
    /// <param name="node">The rating node.</param>
    /// <returns>The rating container.</returns>
    private InlineUIContainer CreateRating(BbCodeNode node)
    {
        var raw = node.Value ?? node.GetText();
        _ = double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var rating);
        var maximum =
            node.Attributes.TryGetValue("max", out var maxValue)
            && int.TryParse(maxValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedMaximum)
                ? parsedMaximum
                : DefaultMaximumRating;
        maximum = Math.Max(1, Math.Min(maximum, MaximumRating));
        rating = Math.Max(0D, Math.Min(rating, maximum));
        var fullStars = (int)Math.Round(rating, MidpointRounding.AwayFromZero);
        var text =
            new string('★', fullStars)
            + new string('☆', maximum - fullStars)
            + " "
            + rating.ToString("0.#", CultureInfo.InvariantCulture)
            + "/"
            + maximum.ToString(CultureInfo.InvariantCulture);
        var ratingBlock = CreateThemedTextBlock();
        ratingBlock.Text = text;
        ratingBlock.SetResourceReference(TextBlock.ForegroundProperty, "SystemAccentColorBrush");
        ratingBlock.ToolTip = "Rating " + rating.ToString("0.#", CultureInfo.InvariantCulture) + " out of " + maximum;
        return new(ratingBlock) { BaselineAlignment = BaselineAlignment.Center };
    }

    /// <summary>Renders a safe media-opening affordance.</summary>
    /// <param name="node">The media node.</param>
    /// <returns>The media container.</returns>
    private InlineUIContainer CreateMediaLink(BbCodeNode node)
    {
        var value = node.Value ?? node.GetText().Trim();
        var address = node.Name switch
        {
            "youtube" => "https://www.youtube.com/watch?v=" + value,
            "gvideo" => "https://video.google.com/videoplay?docid=" + value,
            _ => value,
        };
        var textBlock = CreateThemedTextBlock();
        if (BbCodeRenderHelpers.TryCreateAllowedUri(address, out var uri))
        {
            Hyperlink hyperlink = new(new Run("▶ Open " + node.Name + " media")) { NavigateUri = uri };
            hyperlink.SetResourceReference(TextElement.ForegroundProperty, "HyperlinkButtonForeground");
            textBlock.Inlines.Add(hyperlink);
        }
        else
        {
            textBlock.Text = value;
        }

        var border = CreateThemedBorder(ControlFillBrushKey);
        border.Padding = new(ContentPadding);
        border.Child = textBlock;
        return new(border) { BaselineAlignment = BaselineAlignment.Center };
    }

    /// <summary>Creates a themed horizontal separator.</summary>
    /// <returns>The separator container.</returns>
    private InlineUIContainer CreateSeparator()
    {
        Rectangle separator = new()
        {
            Height = 1D,
            MinWidth = Math.Max(_source.ActualWidth, MinimumSeparatorWidth),
            Margin = new(0D, StandardSpacing, 0D, StandardSpacing),
        };
        separator.SetResourceReference(Shape.FillProperty, "DividerStrokeColorDefaultBrush");
        return new(separator) { BaselineAlignment = BaselineAlignment.Center };
    }
}
