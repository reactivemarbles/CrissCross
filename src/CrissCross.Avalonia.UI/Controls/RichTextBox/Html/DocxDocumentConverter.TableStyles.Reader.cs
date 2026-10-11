// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Text;
using System.Xml.Linq;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Projects Word table formatting into editable CSS.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>The number of border size units per CSS pixel.</summary>
    private const int TableBorderUnitsPerPixel = 6;

    /// <summary>The number of Word percentage units per percent.</summary>
    private const int TableUnitsPerPercent = 50;

    /// <summary>The number of outer table sides.</summary>
    private const int TableSideCount = 4;

    /// <summary>Emits visible formatting and its unchanged import snapshot.</summary>
    /// <param name="properties">The preserved Word properties.</param>
    /// <param name="cell">Whether these are cell properties.</param>
    /// <param name="output">The HTML destination.</param>
    private static void ReadTableStyleAttributes(
        XElement? properties,
        bool cell,
        StringBuilder output)
    {
        if (properties is null)
        {
            return;
        }

        var css = new StringBuilder();
        if (Value(properties.Element(Word + "shd"), "fill") is { } fill)
        {
            _ = css.Append("background-color:").Append(TableReaderColor(fill, "transparent")).Append(';');
        }

        ReadTableWidth(properties.Element(Word + (cell ? "tcW" : "tblW")), css);
        ReadTableMargins(properties.Element(Word + (cell ? "tcMar" : "tblCellMar")), css);
        ReadTableBorders(properties.Element(Word + (cell ? "tcBorders" : "tblBorders")), css);
        if (cell && Value(properties.Element(Word + "vAlign")) is { } alignment)
        {
            _ = css.Append("vertical-align:").Append(alignment == "center" ? "middle" : alignment).Append(';');
        }

        var style = css.Length == 0 ? null : css.ToString();
        Attribute(output, ReaderStyle, style);
        Attribute(output, TableCssSnapshotAttribute, style);
    }

    /// <summary>Reads a preferred table or cell width.</summary>
    /// <param name="width">The Word width.</param>
    /// <param name="css">The CSS destination.</param>
    private static void ReadTableWidth(XElement? width, StringBuilder css)
    {
        if (!double.TryParse(Value(width, "w"), NumberStyles.Float, CultureInfo.InvariantCulture, out var size))
        {
            return;
        }

        var type = Value(width, "type");
        if (type is not ("dxa" or "pct"))
        {
            return;
        }

        var divisor = type == "pct" ? TableUnitsPerPercent : ReaderTwipsPerPixel;
        var unit = type == "pct" ? "%" : "px";
        _ = css.Append("width:").Append((size / divisor).ToString(ReaderPixelFormat, CultureInfo.InvariantCulture)).Append(unit).Append(';');
    }

    /// <summary>Projects cell margins in CSS side order.</summary>
    /// <param name="margins">The Word margins.</param>
    /// <param name="css">The CSS destination.</param>
    private static void ReadTableMargins(XElement? margins, StringBuilder css)
    {
        if (margins is null)
        {
            return;
        }

        var padding = new string[TableSideCount];
        var index = 0;
        foreach (var side in new[] { "top", "right", "bottom", "left" })
        {
            padding[index] = $"{TwipPixels(Value(margins.Element(Word + side), "w")) ?? ReaderDisabledValue}px";
            index++;
        }

        _ = css.Append("padding:").AppendJoin(' ', padding).Append(';');
    }

    /// <summary>Projects outer borders, retaining uniform borders as a shorthand.</summary>
    /// <param name="borders">The Word borders.</param>
    /// <param name="css">The CSS destination.</param>
    private static void ReadTableBorders(XElement? borders, StringBuilder css)
    {
        if (borders is null)
        {
            return;
        }

        string[] sides = ["top", "right", "bottom", "left"];
        var values = new string?[TableSideCount];
        var widths = new string[TableSideCount];
        for (var index = 0; index < sides.Length; index++)
        {
            var border = borders.Element(Word + sides[index]);
            values[index] = ReadTableBorder(border);
            widths[index] = TableReaderBorderWidth(border);
        }

        if (!HasTableBorder(values))
        {
            return;
        }

        if (values[0] is { } uniform && values[1] == uniform && values[2] == uniform && values[3] == uniform)
        {
            _ = css.Append("border:").Append(uniform).Append(';');
            return;
        }

        _ = css.Append("border-width:").AppendJoin(' ', widths).Append(';');
        for (var index = 0; index < sides.Length; index++)
        {
            if (values[index] is { } value)
            {
                _ = css.Append("border-").Append(sides[index]).Append(':').Append(value).Append(';');
            }
        }
    }

    /// <summary>Checks whether any outer side has a border.</summary>
    /// <param name="values">The outer border values.</param>
    /// <returns>Whether a border is present.</returns>
    private static bool HasTableBorder(string?[] values)
    {
        foreach (var value in values)
        {
            if (value is not null)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Reads one outer border as a CSS shorthand.</summary>
    /// <param name="border">The Word border.</param>
    /// <returns>The visible border, or null when absent.</returns>
    private static string? ReadTableBorder(XElement? border)
    {
        if (border is null)
        {
            return null;
        }

        var value = Value(border);
        if (value is "nil" or "none")
        {
            return "none";
        }

        var style = TableReaderBorderStyle(value);
        var color = TableReaderColor(Value(border, "color") ?? ReaderAutomaticColor, "#000000");
        return $"{TableReaderBorderWidth(border)} {style} {color}";
    }

    /// <summary>Converts a Word border size to CSS pixels.</summary>
    /// <param name="border">The Word border.</param>
    /// <returns>The visible border width.</returns>
    private static string TableReaderBorderWidth(XElement? border)
    {
        if (border is null || Value(border) is "nil" or "none")
        {
            return "0px";
        }

        return double.TryParse(Value(border, "sz"), NumberStyles.Float, CultureInfo.InvariantCulture, out var size)
            ? $"{(size / TableBorderUnitsPerPixel).ToString(ReaderPixelFormat, CultureInfo.InvariantCulture)}px"
            : "1px";
    }

    /// <summary>Maps supported Word border patterns to CSS.</summary>
    /// <param name="value">The Word border pattern.</param>
    /// <returns>The closest CSS border pattern.</returns>
    private static string TableReaderBorderStyle(string? value) => value switch
    {
        "double" => "double",
        "dashed" or "dashSmallGap" or "dashDotStroked" or "dotDash" or "dotDotDash" => "dashed",
        "dotted" => "dotted",
        "inset" or "threeDEngrave" => "inset",
        "outset" or "threeDEmboss" => "outset",
        _ => "solid",
    };

    /// <summary>Maps automatic and explicit Word colors to CSS.</summary>
    /// <param name="value">The Word color.</param>
    /// <param name="automatic">The automatic CSS color.</param>
    /// <returns>The CSS color.</returns>
    private static string TableReaderColor(string value, string automatic) =>
        value == ReaderAutomaticColor ? automatic : $"#{value}";
}
