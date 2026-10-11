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

/// <summary>Exports editable table CSS while preserving unedited native formatting.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>The original visible CSS stored alongside native properties.</summary>
    private const string TableCssSnapshotAttribute = "data-docx-table-css";

    /// <summary>The CSS padding property.</summary>
    private const string TableCssPadding = "padding";

    /// <summary>The bottom side name.</summary>
    private const string TableCssBottom = "bottom";

    /// <summary>The native solid border pattern.</summary>
    private const string TableBorderSingle = "single";

    /// <summary>The smallest visible native border measurement.</summary>
    private const int MinimumTableBorderSize = 2;

    /// <summary>The largest visible native border measurement.</summary>
    private const int MaximumTableBorderSize = 96;

    /// <summary>The number of components in a paired shorthand.</summary>
    private const int TableTwoSides = 2;

    /// <summary>The number of components in a three-side shorthand.</summary>
    private const int TableThreeSides = 3;

    /// <summary>The index of the left side.</summary>
    private const int TableLastSideIndex = TableThreeSides;

    /// <summary>The number of outer sides.</summary>
    private const int TableFourSides = 4;

    /// <summary>The native fully opaque alpha channel.</summary>
    private const byte TableOpaqueAlpha = 255;

    /// <summary>Outer side names in CSS shorthand order.</summary>
    private static readonly string[] TableCssSides = ["top", "right", TableCssBottom, "left"];

    /// <summary>Applies changed table or cell CSS declarations.</summary>
    /// <param name="source">The HTML element.</param>
    /// <param name="properties">The native property container.</param>
    /// <param name="cell">Whether the element is a cell.</param>
    private static void ApplyTableStyles(IElement source, XElement properties, bool cell) =>
        ApplyTableStyles(source, Styles(source), properties, cell);

    /// <summary>Reconciles the import snapshot before applying changed declarations.</summary>
    /// <param name="source">The HTML element.</param>
    /// <param name="styles">The current declarations.</param>
    /// <param name="properties">The native property container.</param>
    /// <param name="cell">Whether this is a cell.</param>
    private static void ApplyTableStyles(
        IElement source,
        Dictionary<string, string> styles,
        XElement properties,
        bool cell)
    {
        ApplyTableSnapshotChanges(source, styles);
        ApplyTableStyles(styles, properties, cell);
    }

    /// <summary>Applies changed declarations to native properties.</summary>
    /// <param name="styles">The changed declarations.</param>
    /// <param name="properties">The native property container.</param>
    /// <param name="cell">Whether this is a cell.</param>
    private static void ApplyTableStyles(Dictionary<string, string> styles, XElement properties, bool cell)
    {
        ApplyTableFill(styles, properties);
        ApplyTableWidth(styles, properties, cell);
        ApplyTablePadding(styles, properties, cell);
        ApplyTableBorders(styles, properties, cell);
        ApplyTableVerticalAlignment(styles, properties, cell);
    }

    /// <summary>Applies a supported vertical cell alignment.</summary>
    /// <param name="styles">The changed declarations.</param>
    /// <param name="properties">The cell properties.</param>
    /// <param name="cell">Whether this is a cell.</param>
    private static void ApplyTableVerticalAlignment(Dictionary<string, string> styles, XElement properties, bool cell)
    {
        if (!cell || !styles.TryGetValue("vertical-align", out var alignment))
        {
            return;
        }

        var native = alignment == "middle" ? "center" : alignment;
        if (native is not ("top" or "center" or TableCssBottom))
        {
            return;
        }

        Set(properties, "vAlign", native);
    }

    /// <summary>Retains native values for unchanged CSS and resets removed declarations.</summary>
    /// <param name="source">The imported element.</param>
    /// <param name="styles">The current declarations.</param>
    private static void ApplyTableSnapshotChanges(IElement source, Dictionary<string, string> styles)
    {
        foreach (var declaration in ParseTableSnapshot(source.GetAttribute(TableCssSnapshotAttribute)))
        {
            if (styles.TryGetValue(declaration.Key, out var value))
            {
                if (value == declaration.Value)
                {
                    _ = styles.Remove(declaration.Key);
                }
            }
            else if (!TableCssReplaced(styles, declaration.Key))
            {
                styles[declaration.Key] = TableCssReset(declaration.Key);
            }
        }
    }

    /// <summary>Checks whether a replacement shorthand supersedes a deleted declaration.</summary>
    /// <param name="styles">The current CSS declarations.</param>
    /// <param name="name">The deleted declaration.</param>
    /// <returns>Whether a replacement already covers the declaration.</returns>
    private static bool TableCssReplaced(Dictionary<string, string> styles, string name) =>
        (name.StartsWith("border-", StringComparison.Ordinal) && styles.ContainsKey("border"))
        || (name.StartsWith("padding-", StringComparison.Ordinal) && styles.ContainsKey(TableCssPadding));

    /// <summary>Resets deleted visible declarations to their CSS defaults.</summary>
    /// <param name="name">The deleted CSS property.</param>
    /// <returns>The replacement CSS value.</returns>
    private static string TableCssReset(string name) =>
        name.StartsWith(TableCssPadding, StringComparison.Ordinal) || name.StartsWith("border-width", StringComparison.Ordinal)
            ? "0"
            : TableCssResetValue(name);

    /// <summary>Returns the default visible value of a removed property.</summary>
    /// <param name="name">The CSS property.</param>
    /// <returns>The reset declaration.</returns>
    private static string TableCssResetValue(string name) => name switch
    {
        "width" => "auto",
        "vertical-align" => "top",
        "background-color" or "background" => "transparent",
        _ => "none",
    };

    /// <summary>Reads declarations from the imported CSS snapshot.</summary>
    /// <param name="snapshot">The original declarations.</param>
    /// <returns>The declarations keyed by property name.</returns>
    private static Dictionary<string, string> ParseTableSnapshot(string? snapshot)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var declaration in (snapshot ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var colon = declaration.IndexOf(':');
            if (colon <= 0)
            {
                continue;
            }

            result[declaration[..colon].Trim().ToLowerInvariant()] = declaration[(colon + 1)..].Trim();
        }

        return result;
    }

    /// <summary>Applies a table or cell background fill.</summary>
    /// <param name="styles">The changed declarations.</param>
    /// <param name="properties">The native properties.</param>
    private static void ApplyTableFill(Dictionary<string, string> styles, XElement properties)
    {
        if (!styles.TryGetValue("background-color", out var fill) && !styles.TryGetValue("background", out fill))
        {
            return;
        }

        if (TableCssColor(fill) is not { } color)
        {
            return;
        }

        var shading = properties.Element(Word + "shd") ?? new(Word + "shd");
        shading.SetAttributeValue(Word + "val", "clear");
        shading.SetAttributeValue(Word + "fill", color);
        Replace(properties, shading);
    }

    /// <summary>Applies a preferred table or cell width.</summary>
    /// <param name="styles">The changed declarations.</param>
    /// <param name="properties">The native properties.</param>
    /// <param name="cell">Whether the properties belong to a cell.</param>
    private static void ApplyTableWidth(Dictionary<string, string> styles, XElement properties, bool cell)
    {
        if (!styles.TryGetValue("width", out var text))
        {
            return;
        }

        if (text == "auto")
        {
            Replace(properties, new(
                Word + (cell ? "tcW" : "tblW"),
                new XAttribute(Word + "type", "auto"),
                new XAttribute(Word + "w", "0")));
            return;
        }

        var percent = text.EndsWith('%');
        var size = percent ? TablePercentage(text) : TableCssLength(text);
        if (size is not { } width)
        {
            return;
        }

        Replace(properties, new(
            Word + (cell ? "tcW" : "tblW"),
            new XAttribute(Word + "type", percent ? "pct" : "dxa"),
            new XAttribute(Word + "w", TableInteger(width * (percent ? TableUnitsPerPercent : TwipsPerPixel)))));
    }

    /// <summary>Parses a percentage without applying font relative units.</summary>
    /// <param name="text">The percentage string.</param>
    /// <returns>The finite nonnegative percentage.</returns>
    private static double? TablePercentage(string text) =>
        double.TryParse(text[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var number)
        && double.IsFinite(number) && number >= 0 ? number : null;

    /// <summary>Parses a nonnegative CSS length.</summary>
    /// <param name="text">The CSS length.</param>
    /// <returns>The pixel length.</returns>
    private static double? TableCssLength(string text) =>
        text is "0" or "0px" or "0pt" ? 0 : CssDocumentValues.ParseLength(text, null);

    /// <summary>Rounds a native measurement to an invariant integer.</summary>
    /// <param name="number">The native measurement.</param>
    /// <returns>The rounded integer string.</returns>
    private static string TableInteger(double number) =>
        Math.Round(Math.Clamp(number, 0, int.MaxValue)).ToString(CultureInfo.InvariantCulture);

    /// <summary>Parses a CSS color into the native RGB representation.</summary>
    /// <param name="text">The CSS color.</param>
    /// <returns>The native color, or null for unsupported values.</returns>
    private static string? TableCssColor(string text) =>
        CssDocumentValues.TryParseColor(text, out var color) ? TableNativeColor(color) : null;

    /// <summary>Maps transparent or opaque CSS colors to native RGB.</summary>
    /// <param name="color">The parsed color.</param>
    /// <returns>The native color.</returns>
    private static string? TableNativeColor(Color color) => color.A switch
    {
        0 => "auto",
        TableOpaqueAlpha => $"{color.R:X2}{color.G:X2}{color.B:X2}",
        _ => null,
    };

    /// <summary>Expands one through four CSS shorthand components.</summary>
    /// <param name="text">The shorthand declaration.</param>
    /// <param name="index">The side index in CSS order.</param>
    /// <returns>The applicable component.</returns>
    private static string? TableSideValue(string? text, int index)
    {
        var parts = text?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts?.Length switch
        {
            1 => parts[0],
            TableTwoSides => parts[index % TableTwoSides],
            TableThreeSides => parts[index == TableLastSideIndex ? 1 : index],
            TableFourSides => parts[index],
            _ => null,
        };
    }

    /// <summary>Writes cell margins in native schema side order.</summary>
    /// <param name="styles">The changed declarations.</param>
    /// <param name="properties">The native properties.</param>
    /// <param name="cell">Whether these are cell properties.</param>
    private static void ApplyTablePadding(Dictionary<string, string> styles, XElement properties, bool cell)
    {
        var name = cell ? "tcMar" : "tblCellMar";
        var margins = properties.Element(Word + name) ?? new(Word + name);
        _ = styles.TryGetValue(TableCssPadding, out var shorthand);
        for (var index = 0; index < TableCssSides.Length; index++)
        {
            var side = TableCssSides[index];
            var text = styles.TryGetValue($"padding-{side}", out var individual) ? individual : TableSideValue(shorthand, index);
            if (text is null || TableCssLength(text) is not { } pixels)
            {
                continue;
            }

            Replace(margins, new(
                Word + side,
                new XAttribute(Word + "w", TableInteger(pixels * TwipsPerPixel)),
                new XAttribute(Word + "type", "dxa")));
        }

        if (!margins.HasElements)
        {
            return;
        }

        OrderWordPropertyChildren(margins, ["top", "start", "left", TableCssBottom, "end", "right"]);
        Replace(properties, margins);
    }

    /// <summary>Writes outer borders and retains untouched interior borders.</summary>
    /// <param name="styles">The changed declarations.</param>
    /// <param name="properties">The native properties.</param>
    /// <param name="cell">Whether these are cell properties.</param>
    private static void ApplyTableBorders(Dictionary<string, string> styles, XElement properties, bool cell)
    {
        var name = cell ? "tcBorders" : "tblBorders";
        var borders = properties.Element(Word + name) ?? new(Word + name);
        for (var index = 0; index < TableCssSides.Length; index++)
        {
            ApplyTableSideBorder(styles, borders, index);
        }

        if (!borders.HasElements)
        {
            return;
        }

        OrderWordPropertyChildren(borders, ["top", "start", "left", TableCssBottom, "end", "right", "insideH", "insideV", "tl2br", "tr2bl"]);
        Replace(properties, borders);
    }

    /// <summary>Applies a changed outer border with component overrides.</summary>
    /// <param name="styles">The changed CSS declarations.</param>
    /// <param name="borders">The native border container.</param>
    /// <param name="index">The side index.</param>
    private static void ApplyTableSideBorder(Dictionary<string, string> styles, XElement borders, int index)
    {
        var side = TableCssSides[index];
        var border = borders.Element(Word + side) ?? new(Word + side);
        _ = styles.TryGetValue("border", out var common);
        var shorthand = styles.TryGetValue($"border-{side}", out var individual) ? individual : common;
        if (shorthand is not null)
        {
            ApplyTableBorderShorthand(border, shorthand);
        }

        ApplyTableBorderComponent(styles, border, "width", index);
        ApplyTableBorderComponent(styles, border, "style", index);
        ApplyTableBorderComponent(styles, border, ColorElementName, index);
        if (border.Attribute(Word + "val") is null)
        {
            return;
        }

        Replace(borders, border);
    }

    /// <summary>Applies the components of a border shorthand.</summary>
    /// <param name="border">The native border.</param>
    /// <param name="text">The CSS shorthand.</param>
    private static void ApplyTableBorderShorthand(XElement border, string text)
    {
        border.SetAttributeValue(Word + "val", TableBorderSingle);
        border.SetAttributeValue(Word + "sz", TableBorderUnitsPerPixel);
        border.SetAttributeValue(Word + ColorElementName, "auto");
        foreach (var component in TableBorderTokens(text))
        {
            ApplyTableBorderValue(border, component);
        }
    }

    /// <summary>Splits border shorthand tokens while retaining functional colors.</summary>
    /// <param name="text">The CSS shorthand.</param>
    /// <returns>The individual border components.</returns>
    private static List<string> TableBorderTokens(string text)
    {
        var tokens = new List<string>();
        var start = 0;
        var depth = 0;
        for (var index = 0; index <= text.Length; index++)
        {
            if (index == text.Length || (char.IsWhiteSpace(text[index]) && depth == 0))
            {
                if (index > start)
                {
                    tokens.Add(text[start..index]);
                }

                start = index + 1;
            }
            else if (text[index] == '(')
            {
                depth++;
            }
            else if (text[index] == ')')
            {
                depth--;
            }
        }

        return tokens;
    }

    /// <summary>Applies a side-specific or expanded common border component.</summary>
    /// <param name="styles">The changed CSS declarations.</param>
    /// <param name="border">The native border.</param>
    /// <param name="component">The component name.</param>
    /// <param name="index">The side index.</param>
    private static void ApplyTableBorderComponent(Dictionary<string, string> styles, XElement border, string component, int index)
    {
        _ = styles.TryGetValue($"border-{component}", out var common);
        var text = TableBorderComponentValue(styles, component, index, common);
        if (text is null)
        {
            return;
        }

        if (border.Attribute(Word + "val") is null)
        {
            border.SetAttributeValue(Word + "val", TableBorderSingle);
        }

        ApplyTableBorderValue(border, text);
    }

    /// <summary>Resolves the applicable individual or common border component.</summary>
    /// <param name="styles">The changed declarations.</param>
    /// <param name="component">The component name.</param>
    /// <param name="index">The side index.</param>
    /// <param name="common">The common value.</param>
    /// <returns>The applicable component.</returns>
    private static string? TableBorderComponentValue(
        Dictionary<string, string> styles,
        string component,
        int index,
        string? common)
    {
        if (styles.TryGetValue($"border-{TableCssSides[index]}-{component}", out var individual))
        {
            return individual;
        }

        return component == ColorElementName ? common : TableSideValue(common, index);
    }

    /// <summary>Applies one border width, pattern or color token.</summary>
    /// <param name="border">The native border.</param>
    /// <param name="text">The CSS component.</param>
    private static void ApplyTableBorderValue(XElement border, string text)
    {
        var pattern = TableBorderPattern(text);
        if (pattern is not null)
        {
            border.SetAttributeValue(Word + "val", pattern);
        }
        else
        {
            ApplyTableBorderMeasurement(border, text);
        }
    }

    /// <summary>Maps a CSS border style to its native pattern.</summary>
    /// <param name="text">The CSS style.</param>
    /// <returns>The native pattern, or null.</returns>
    private static string? TableBorderPattern(string text) => text.ToLowerInvariant() switch
    {
        "none" or "hidden" => "nil",
        "solid" => TableBorderSingle,
        "double" => "double",
        "dashed" => "dashed",
        "dotted" => "dotted",
        "inset" or "groove" => "inset",
        "outset" or "ridge" => "outset",
        _ => null,
    };

    /// <summary>Applies a border width or color component.</summary>
    /// <param name="border">The native border.</param>
    /// <param name="text">The CSS component.</param>
    private static void ApplyTableBorderMeasurement(XElement border, string text)
    {
        if (TableCssLength(text) is { } pixels)
        {
            var size = Math.Clamp(pixels * TableBorderUnitsPerPixel, MinimumTableBorderSize, MaximumTableBorderSize);
            border.SetAttributeValue(Word + "sz", TableInteger(size));
            if (pixels == 0)
            {
                border.SetAttributeValue(Word + "val", "nil");
            }
        }
        else if (TableCssColor(text) is { } color)
        {
            border.SetAttributeValue(Word + ColorElementName, color);
        }
    }
}
