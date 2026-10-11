// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Xml.Linq;
using AngleSharp.Dom;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides shared WordprocessingML writer formatting helpers.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>The maximum page dimension in CSS pixels.</summary>
    private const int MaximumPagePixels = 100_000;

    /// <summary>The maximum font size in points.</summary>
    private const int MaximumFontSizePoints = 10_000;

    /// <summary>The minimum numeric bold font weight.</summary>
    private const int MinimumBoldWeight = 600;

    /// <summary>The number of twips per CSS pixel.</summary>
    private const int TwipsPerPixel = 15;

    /// <summary>The number of half-points per point.</summary>
    private const int HalfPointsPerPoint = 2;

    /// <summary>The number of characters in a CSS unit suffix.</summary>
    private const int CssUnitSuffixLength = 2;

    /// <summary>The Word run vertical-alignment property name.</summary>
    private const string VerticalAlignmentProperty = "vertAlign";

    /// <summary>The CSS foreground color property name.</summary>
    private const string ColorElementName = "color";

    /// <summary>The number of half-points per CSS pixel.</summary>
    private const double HalfPointsPerPixel = 1.5;

    /// <summary>Creates the package content type declarations.</summary>
    /// <returns>The content types element.</returns>
    private static XElement CreateContentTypes() =>
        new(
            ContentTypes + "Types",
            new XElement(
                ContentTypes + "Default",
                new XAttribute("Extension", "rels"),
                new XAttribute("ContentType", "application/vnd.openxmlformats-package.relationships+xml")),
            new XElement(
                ContentTypes + "Default",
                new XAttribute("Extension", "xml"),
                new XAttribute("ContentType", "application/xml")));

    /// <summary>Reads an original properties element or creates an empty one.</summary>
    /// <param name="element">The source HTML.</param>
    /// <param name="name">The required Word element name.</param>
    /// <returns>The properties element.</returns>
    private static XElement Properties(IElement element, string name)
    {
        var result = element.GetAttribute("data-docx-properties") is { } encoded
            ? DecodeXml(encoded)
            : new XElement(Word + name);
        if (result.Name != Word + name)
        {
            throw new InvalidDataException($"DOCX metadata must contain '{name}' properties.");
        }

        return result;
    }

    /// <summary>Reads inline CSS declarations.</summary>
    /// <param name="element">The HTML element.</param>
    /// <returns>The CSS declarations.</returns>
    private static Dictionary<string, string> Styles(IElement element)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var declarations = (element.GetAttribute("style") ?? string.Empty)
            .Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var declaration in declarations)
        {
            var colon = declaration.IndexOf(':');
            if (colon > 0)
            {
                result[declaration[..colon].Trim()] = declaration[(colon + 1)..].Trim();
            }
        }

        return result;
    }

    /// <summary>Sets a numbering paragraph property.</summary>
    /// <param name="properties">The paragraph properties.</param>
    /// <param name="number">The numbering identifier.</param>
    /// <param name="level">The level.</param>
    private static void SetNumber(XElement properties, string number, string level) =>
        Replace(properties, new(
            Word + "numPr",
            new XElement(Word + "ilvl", new XAttribute(Word + "val", level)),
            new XElement(Word + "numId", new XAttribute(Word + "val", number))));

    /// <summary>Sets a Word value property.</summary>
    /// <param name="properties">The properties.</param>
    /// <param name="name">The property name.</param>
    /// <param name="value">The value.</param>
    private static void Set(XElement properties, string name, string value) =>
        Replace(properties, new(
            Word + name,
            new XAttribute(Word + "val", value)));

    /// <summary>Replaces a property while preserving its original position.</summary>
    /// <param name="properties">The properties element.</param>
    /// <param name="property">The replacement property.</param>
    private static void Replace(XElement properties, XElement property)
    {
        var existing = properties.Element(property.Name);
        if (existing is null)
        {
            properties.Add(property);
            return;
        }

        existing.ReplaceWith(property);
    }

    /// <summary>Applies semantic tags and CSS declarations to run properties.</summary>
    /// <param name="element">The HTML element.</param>
    /// <param name="properties">The run properties.</param>
    private static void ApplyRunStyles(IElement element, XElement properties)
    {
        var styles = Styles(element);
        ApplySemanticRunStyles(element, properties);
        ApplyTypographyStyles(styles, properties);
        ApplyColorStyles(styles, properties);
        ApplyVerticalAlignment(styles, properties);
    }

    /// <summary>Applies formatting implied by semantic HTML tags.</summary>
    /// <param name="element">The HTML element.</param>
    /// <param name="properties">The run properties.</param>
    private static void ApplySemanticRunStyles(IElement element, XElement properties)
    {
        var formatting = element.LocalName switch
        {
            "b" or "strong" => (Property: "b", Value: "1"),
            "i" or "em" => (Property: "i", Value: "1"),
            "u" => (Property: "u", Value: "single"),
            "s" or "strike" or "del" => (Property: "strike", Value: "1"),
            "sup" => (Property: VerticalAlignmentProperty, Value: "superscript"),
            "sub" => (Property: VerticalAlignmentProperty, Value: "subscript"),
            _ => (Property: string.Empty, Value: string.Empty),
        };
        if (formatting.Property.Length == 0)
        {
            return;
        }

        Set(properties, formatting.Property, formatting.Value);
    }

    /// <summary>Applies font family, weight, style, decoration, and size.</summary>
    /// <param name="styles">The CSS declarations.</param>
    /// <param name="properties">The run properties.</param>
    private static void ApplyTypographyStyles(Dictionary<string, string> styles, XElement properties)
    {
        if (styles.TryGetValue("font-weight", out var weight))
        {
            var isBold = weight == "bold"
                || (int.TryParse(weight, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numeric)
                    && numeric >= MinimumBoldWeight);
            Set(properties, "b", isBold ? "1" : "0");
        }

        if (styles.TryGetValue("font-style", out var fontStyle))
        {
            Set(properties, "i", fontStyle is "italic" or "oblique" ? "1" : "0");
        }

        ApplyTextDecoration(styles, properties);
        ApplyFontFamily(styles, properties);
        ApplyFontSize(styles, properties);
    }

    /// <summary>Applies text decoration declarations.</summary>
    /// <param name="styles">The CSS declarations.</param>
    /// <param name="properties">The run properties.</param>
    private static void ApplyTextDecoration(Dictionary<string, string> styles, XElement properties)
    {
        if (!styles.TryGetValue("text-decoration", out var decoration)
            && !styles.TryGetValue("text-decoration-line", out decoration))
        {
            return;
        }

        Set(properties, "u", decoration.Contains("underline", StringComparison.Ordinal) ? "single" : "none");
        Set(properties, "strike", decoration.Contains("line-through", StringComparison.Ordinal) ? "1" : "0");
    }

    /// <summary>Applies a CSS font family.</summary>
    /// <param name="styles">The CSS declarations.</param>
    /// <param name="properties">The run properties.</param>
    private static void ApplyFontFamily(Dictionary<string, string> styles, XElement properties)
    {
        if (!styles.TryGetValue("font-family", out var font))
        {
            return;
        }

        font = font.Split(',')[0].Trim().Trim('"', '\'');
        Replace(properties, new(
            Word + "rFonts",
            new XAttribute(Word + "ascii", font),
            new XAttribute(Word + "hAnsi", font)));
    }

    /// <summary>Applies a CSS font size converted to Word half-points.</summary>
    /// <param name="styles">The CSS declarations.</param>
    /// <param name="properties">The run properties.</param>
    private static void ApplyFontSize(Dictionary<string, string> styles, XElement properties)
    {
        if (!styles.TryGetValue("font-size", out var size))
        {
            return;
        }

        var pixels = size.EndsWith("px", StringComparison.OrdinalIgnoreCase);
        var hasUnit = pixels || size.EndsWith("pt", StringComparison.OrdinalIgnoreCase);
        var numeric = hasUnit ? size[..^CssUnitSuffixLength] : size;
        if (!double.TryParse(numeric, NumberStyles.Float, CultureInfo.InvariantCulture, out var points)
            || !double.IsFinite(points) || points <= 0 || points > MaximumFontSizePoints)
        {
            throw new NotSupportedException("DOCX export requires finite font sizes in points or pixels.");
        }

        var halfPoints = Math.Round(points * (pixels ? HalfPointsPerPixel : HalfPointsPerPoint));
        Set(properties, "sz", halfPoints.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>Applies foreground and background colors.</summary>
    /// <param name="styles">The CSS declarations.</param>
    /// <param name="properties">The run properties.</param>
    private static void ApplyColorStyles(Dictionary<string, string> styles, XElement properties)
    {
        var foreground = default(Color);
        var hasForeground = styles.TryGetValue(ColorElementName, out var foregroundColor)
            && Color.TryParse(foregroundColor, out foreground);
        if (hasForeground)
        {
            Set(properties, "color", $"{foreground.R:X2}{foreground.G:X2}{foreground.B:X2}");
        }

        if (!styles.TryGetValue("background-color", out var background)
            && !styles.TryGetValue("background", out background))
        {
            return;
        }

        if (!Color.TryParse(background, out var fill))
        {
            return;
        }

        Replace(properties, new(
            Word + "shd",
            new XAttribute(Word + "val", "clear"),
            new XAttribute(Word + "fill", $"{fill.R:X2}{fill.G:X2}{fill.B:X2}")));
    }

    /// <summary>Applies CSS vertical alignment.</summary>
    /// <param name="styles">The CSS declarations.</param>
    /// <param name="properties">The run properties.</param>
    private static void ApplyVerticalAlignment(Dictionary<string, string> styles, XElement properties)
    {
        if (!styles.TryGetValue("vertical-align", out var vertical))
        {
            return;
        }

        var alignment = vertical switch
        {
            "super" => "superscript",
            "sub" => "subscript",
            _ => "baseline",
        };
        Set(properties, VerticalAlignmentProperty, alignment);
    }

    /// <summary>Returns the next numeric identifier for a Word element type.</summary>
    /// <param name="root">The collection root.</param>
    /// <param name="tag">The Word element name.</param>
    /// <param name="attribute">The identifier attribute name.</param>
    /// <returns>The next available identifier.</returns>
    private static string NextNumericId(XElement root, string tag, string attribute)
    {
        var maximum = 0;
        foreach (var element in root.Elements(Word + tag))
        {
            if (int.TryParse(Value(element, attribute), NumberStyles.Integer, CultureInfo.InvariantCulture, out var id)
                && id > maximum)
            {
                maximum = id;
            }
        }

        return (maximum + 1).ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>Converts a pixel page dimension to twips.</summary>
    /// <param name="value">The pixel value.</param>
    /// <returns>The corresponding twip value.</returns>
    private static long PixelsTwips(string value)
    {
        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var pixels)
            || !double.IsFinite(pixels) || pixels < 0 || pixels > MaximumPagePixels)
        {
            throw new InvalidDataException("Invalid DOCX page dimension.");
        }

        return (long)Math.Round(pixels * TwipsPerPixel);
    }
}
