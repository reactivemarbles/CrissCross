// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.BBCode;
#else
namespace CrissCross.WPF.UI.Controls.BBCode;
#endif

/// <summary>Provides pure conversion helpers used by the BBCode renderer.</summary>
internal static class BbCodeRenderHelpers
{
    /// <summary>The image width attribute name.</summary>
    private const string WidthAttribute = "width";

    /// <summary>The image height attribute name.</summary>
    private const string HeightAttribute = "height";

    /// <summary>The number of components in a name-value pair.</summary>
    private const int NameValuePartCount = 2;

    /// <summary>The number of letters in the English alphabet.</summary>
    private const int AlphabetLength = 26;

    /// <summary>The maximum rendered image dimension.</summary>
    private const double MaximumImageDimension = 4096D;

    /// <summary>The small named font size.</summary>
    private const double SmallFontSize = 11D;

    /// <summary>The large named font size.</summary>
    private const double LargeFontSize = 22D;

    /// <summary>The extra-large named font size.</summary>
    private const double ExtraLargeFontSize = 28D;

    /// <summary>The maximum author-selected font size.</summary>
    private const double MaximumFontSize = 200D;

    /// <summary>The reference font size for percentages.</summary>
    private const double PercentageFontBase = 14D;

    /// <summary>The divisor used to convert percentages.</summary>
    private const double PercentageDivisor = 100D;

    /// <summary>The Roman numeral conversion map.</summary>
    private static readonly (int Value, string Text)[] RomanMap = [(10, "X"), (9, "IX"), (5, "V"), (4, "IV"), (1, "I")];

    /// <summary>Adds compatibility attributes from the legacy comma-delimited image form.</summary>
    /// <param name="node">The image node.</param>
    /// <param name="attributes">The legacy attributes.</param>
    internal static void AddLegacyImageAttributes(BbCodeNode node, IEnumerable<string> attributes)
    {
        foreach (var attribute in attributes)
        {
            var parts = attribute.Split(['='], NameValuePartCount);
            if (parts.Length != NameValuePartCount)
            {
                continue;
            }

            node.Attributes[parts[0].Trim()] = parts[1].Trim();
        }
    }

    /// <summary>Applies an author-selected foreground color to a span.</summary>
    /// <param name="span">The target span.</param>
    /// <param name="value">The color value.</param>
    internal static void ApplyColor(Span span, string? value)
    {
        if (!TryCreateBrush(value, out var brush))
        {
            return;
        }

        span.Foreground = brush;
    }

    /// <summary>Applies an author-selected font family to a span.</summary>
    /// <param name="span">The target span.</param>
    /// <param name="value">The font-family value.</param>
    internal static void ApplyFont(Span span, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        span.FontFamily = new(value);
    }

    /// <summary>Applies an author-selected foreground to a text block.</summary>
    /// <param name="textBlock">The target text block.</param>
    /// <param name="value">The color value.</param>
    internal static void ApplyForeground(TextBlock textBlock, string? value)
    {
        if (!TryCreateBrush(value, out var brush))
        {
            return;
        }

        textBlock.Foreground = brush;
    }

    /// <summary>Applies standard and complex image dimensions.</summary>
    /// <param name="image">The target image.</param>
    /// <param name="node">The image node.</param>
    internal static void ApplyImageSize(Image image, BbCodeNode node)
    {
        AddShorthandDimensions(node);
        ApplyImageDimension(node, WidthAttribute, value => image.Width = value);
        ApplyImageDimension(node, HeightAttribute, value => image.Height = value);
    }

    /// <summary>Applies named style attributes to a span.</summary>
    /// <param name="span">The target span.</param>
    /// <param name="node">The style node.</param>
    internal static void ApplyStyle(Span span, BbCodeNode node)
    {
        _ = node.Attributes.TryGetValue("size", out var size);
        _ = node.Attributes.TryGetValue("color", out var color);
        _ = node.Attributes.TryGetValue("font", out var font);
        ApplySize(span, size);
        ApplyColor(span, color);
        ApplyFont(span, font);
    }

    /// <summary>Applies a named, numeric, or percentage font size to a span.</summary>
    /// <param name="span">The target span.</param>
    /// <param name="value">The font-size value.</param>
    internal static void ApplySize(Span span, string? value)
    {
        if (!TryParseFontSize(value, out var size))
        {
            return;
        }

        span.FontSize = size;
    }

    /// <summary>Creates a marker for an ordered or unordered list item.</summary>
    /// <param name="style">The list style.</param>
    /// <param name="number">The one-based item number.</param>
    /// <returns>The marker text.</returns>
    internal static string CreateListMarker(string style, int number) =>
        style switch
        {
            "1" => $"{number.ToString(CultureInfo.InvariantCulture)}.",
            "a" => $"{((char)('a' + ((number - 1) % AlphabetLength))).ToString(CultureInfo.InvariantCulture)}.",
            "A" => $"{((char)('A' + ((number - 1) % AlphabetLength))).ToString(CultureInfo.InvariantCulture)}.",
            "i" => $"{ToRoman(number).ToLowerInvariant()}.",
            "I" => $"{ToRoman(number)}.",
            "circle" => "○",
            "square" => "▪",
            _ => "•",
        };

    /// <summary>Creates a bitmap without file-scheme access or propagated decode failures.</summary>
    /// <param name="value">The image URI text.</param>
    /// <param name="bitmap">The resulting bitmap.</param>
    /// <returns><see langword="true"/> when a supported bitmap URI was initialized.</returns>
    internal static bool TryCreateBitmap(string value, [NotNullWhen(true)] out BitmapImage? bitmap)
    {
        bitmap = null;
        if (
            !Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri)
            || (uri.IsAbsoluteUri && uri.Scheme is not ("http" or "https" or "pack")))
        {
            return false;
        }

        try
        {
            bitmap = new(uri);
            return true;
        }
        catch (Exception exception)
            when (exception is IOException or InvalidOperationException or NotSupportedException or UriFormatException)
        {
            bitmap = null;
            return false;
        }
    }

    /// <summary>Splits list children into logical items.</summary>
    /// <param name="list">The list node.</param>
    /// <returns>The list items.</returns>
    internal static List<IReadOnlyList<BbCodeNode>> ExtractListItems(BbCodeNode list)
    {
        var items = new List<IReadOnlyList<BbCodeNode>>();
        var current = new List<BbCodeNode>();
        foreach (var child in list.Children)
        {
            if (child.Name is not ("li" or "*"))
            {
                current.Add(child);
                continue;
            }

            FlushListItem(items, current);
            if (child.Name == "li")
            {
                items.Add(new List<BbCodeNode>(child.Children));
            }
        }

        FlushListItem(items, current);
        return items;
    }

    /// <summary>Gets table rows from canonical and alias nodes.</summary>
    /// <param name="table">The table node.</param>
    /// <returns>The table rows.</returns>
    internal static List<IReadOnlyList<BbCodeNode>> GetRows(BbCodeNode table)
    {
        var rows = new List<IReadOnlyList<BbCodeNode>>();
        foreach (var row in table.Children)
        {
            if (row.Name is not ("tr" or "row"))
            {
                continue;
            }

            List<BbCodeNode> cells = [];
            foreach (var child in row.Children)
            {
                if (child.Name is "th" or "td" or "cell")
                {
                    cells.Add(child);
                }
            }

            rows.Add(cells);
        }

        return rows;
    }

    /// <summary>Gets a value indicating whether a value resembles an image address.</summary>
    /// <param name="value">The candidate value.</param>
    /// <returns><see langword="true"/> when the value is address-like.</returns>
    internal static bool LooksLikeAddress(string value) =>
        value.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
        || value.StartsWith("https:", StringComparison.OrdinalIgnoreCase)
        || value.StartsWith("pack:", StringComparison.OrdinalIgnoreCase)
        || value.StartsWith('/');

    /// <summary>Creates a URI only for supported navigation schemes.</summary>
    /// <param name="value">The URI text.</param>
    /// <param name="uri">The resulting URI.</param>
    /// <returns><see langword="true"/> when the URI is supported.</returns>
    internal static bool TryCreateAllowedUri(string value, out Uri? uri)
    {
        uri = null;
        if (
            !Uri.TryCreate(value, UriKind.Absolute, out var candidate)
            || candidate.Scheme is not ("http" or "https" or "mailto" or "cmd"))
        {
            return false;
        }

        uri = candidate;
        return true;
    }

    /// <summary>Adds dimensions from shorthand image syntax.</summary>
    /// <param name="node">The image node.</param>
    private static void AddShorthandDimensions(BbCodeNode node)
    {
        var dimensions = node.Value ?? string.Empty;
        var parts = dimensions.Split('x');
        if (parts.Length == 2)
        {
            node.Attributes[WidthAttribute] = parts[0];
            node.Attributes[HeightAttribute] = parts[1];
            return;
        }

        if (!double.TryParse(dimensions, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
        {
            return;
        }

        node.Attributes[WidthAttribute] = dimensions;
    }

    /// <summary>Applies one positive, bounded image dimension.</summary>
    /// <param name="node">The image node.</param>
    /// <param name="name">The dimension name.</param>
    /// <param name="apply">The dimension setter.</param>
    private static void ApplyImageDimension(BbCodeNode node, string name, Action<double> apply)
    {
        if (
            !node.Attributes.TryGetValue(name, out var text)
            || !double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            || value <= 0D)
        {
            return;
        }

        apply(Math.Min(value, MaximumImageDimension));
    }

    /// <summary>Files a completed implicit list item.</summary>
    /// <param name="items">The completed items.</param>
    /// <param name="current">The current item.</param>
    private static void FlushListItem(List<IReadOnlyList<BbCodeNode>> items, List<BbCodeNode> current)
    {
        if (current.Count == 0)
        {
            return;
        }

        items.Add(new List<BbCodeNode>(current));
        current.Clear();
    }

    /// <summary>Converts a color value into a brush.</summary>
    /// <param name="value">The color value.</param>
    /// <param name="brush">The converted brush.</param>
    /// <returns><see langword="true"/> when conversion succeeds.</returns>
    private static bool TryCreateBrush(string? value, out Brush? brush)
    {
        brush = null;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            BrushConverter converter = new();
            brush = (Brush?)converter.ConvertFromInvariantString(value);
            return brush is not null;
        }
        catch (FormatException)
        {
            return false;
        }
        catch (NotSupportedException)
        {
            return false;
        }
    }

    /// <summary>Parses a named, numeric, or percentage font size.</summary>
    /// <param name="value">The font-size value.</param>
    /// <param name="size">The parsed size.</param>
    /// <returns><see langword="true"/> when the size is valid.</returns>
    private static bool TryParseFontSize(string? value, out double size)
    {
        size = double.NaN;
        if (value is null)
        {
            return false;
        }

        var normalized = value.Trim().ToLowerInvariant();
        if (normalized.Length == 0)
        {
            return false;
        }

        size = normalized switch
        {
            "small" => SmallFontSize,
            "large" => LargeFontSize,
            "x-large" => ExtraLargeFontSize,
            _ => double.NaN,
        };
        var parsed = !double.IsNaN(size) || TryParseNumericSize(normalized, out size);
        return parsed && size is > 0D and <= MaximumFontSize;
    }

    /// <summary>Parses a numeric or percentage font size.</summary>
    /// <param name="value">The normalized size.</param>
    /// <param name="size">The parsed size.</param>
    /// <returns><see langword="true"/> when parsing succeeds.</returns>
    private static bool TryParseNumericSize(string value, out double size)
    {
        var isPercentage = value.EndsWith('%');
        var numeric = value.TrimEnd('%');
        if (!double.TryParse(numeric, NumberStyles.Float, CultureInfo.InvariantCulture, out size))
        {
            return false;
        }

        size = isPercentage ? PercentageFontBase * size / PercentageDivisor : size;
        return true;
    }

    /// <summary>Converts a small positive integer to a Roman numeral.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The Roman numeral.</returns>
    private static string ToRoman(int value)
    {
        StringBuilder result = new();
        foreach (var pair in RomanMap)
        {
            while (value >= pair.Value)
            {
                _ = result.Append(pair.Text);
                value -= pair.Value;
            }
        }

        return result.ToString();
    }
}
