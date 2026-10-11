// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AngleSharp.Dom;
using Avalonia;
using Avalonia.Media;
using VerticalAlignment = Avalonia.Layout.VerticalAlignment;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Reads portable cell borders, padding and vertical content alignment.</summary>
internal sealed class RichTableCellStyle
{
    /// <summary>The number of sides in a CSS box.</summary>
    private const int SideCount = 4;

    /// <summary>The index of the CSS bottom side.</summary>
    private const int BottomIndex = 2;

    /// <summary>The index of the CSS left side.</summary>
    private const int LeftIndex = 3;

    /// <summary>Gets the optional cell border thickness.</summary>
    internal Thickness? BorderThickness { get; private set; }

    /// <summary>Gets the optional cell border color.</summary>
    internal IBrush? BorderBrush { get; private set; }

    /// <summary>Gets the optional content padding.</summary>
    internal Thickness? Padding { get; private set; }

    /// <summary>Gets the vertical content alignment.</summary>
    internal VerticalAlignment Alignment { get; private set; } = VerticalAlignment.Top;

    /// <summary>Reads cell styles in declaration order.</summary>
    /// <param name="cell">The source table cell.</param>
    /// <returns>The supported visual properties.</returns>
    internal static RichTableCellStyle Read(IElement cell)
    {
        var result = new RichTableCellStyle();
        foreach (var declaration in (cell.GetAttribute("style") ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var colon = declaration.IndexOf(':');
            if (colon > 0)
            {
                result.Apply(declaration[..colon].Trim().ToLowerInvariant(), declaration[(colon + 1)..].Trim());
            }
        }

        return result;
    }

    /// <summary>Reads a one-to-four-value CSS box declaration.</summary>
    /// <param name="value">The CSS lengths.</param>
    /// <returns>The supported nonnegative lengths.</returns>
    private static Thickness? ReadBox(string value)
    {
        var values = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (values.Length == 0 || values.Length > SideCount)
        {
            return null;
        }

        var top = ReadLength(values[0]);
        var right = values.Length > 1 ? ReadLength(values[1]) : top;
        var bottom = values.Length > BottomIndex ? ReadLength(values[BottomIndex]) : top;
        var left = values.Length > LeftIndex ? ReadLength(values[LeftIndex]) : right;
        return new(left, top, right, bottom);
    }

    /// <summary>Reads a CSS length including zero.</summary>
    /// <param name="value">The length.</param>
    /// <returns>The supported nonnegative length.</returns>
    private static double ReadLength(string value) => CssDocumentValues.ParseLength(value, null) ?? 0;

    /// <summary>Reads a vertical content alignment value.</summary>
    /// <param name="value">The CSS alignment.</param>
    /// <returns>The supported cell alignment.</returns>
    private static VerticalAlignment ReadAlignment(string value) => value.ToLowerInvariant() switch
    {
        "middle" => VerticalAlignment.Center,
        "bottom" => VerticalAlignment.Bottom,
        _ => VerticalAlignment.Top,
    };

    /// <summary>Applies one declaration.</summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The CSS value.</param>
    private void Apply(string name, string value)
    {
        if (name == "border")
        {
            ApplyBorder(value);
            return;
        }

        if (name.StartsWith("border-", StringComparison.Ordinal))
        {
            ApplyBorderProperty(name, value);
            return;
        }

        if (name == "padding")
        {
            Padding = ReadBox(value);
            return;
        }

        Alignment = name == "vertical-align" ? ReadAlignment(value) : Alignment;
    }

    /// <summary>Reads border width, color and visibility declarations.</summary>
    /// <param name="name">The border property.</param>
    /// <param name="value">The CSS value.</param>
    private void ApplyBorderProperty(string name, string value)
    {
        if (name is "border-left" or "border-top" or "border-right" or "border-bottom")
        {
            ApplyBorderSide(name, value);
            return;
        }

        BorderThickness = name switch
        {
            "border-width" => ReadBox(value),
            "border-style" when value is "none" or "hidden" => default(Thickness),
            _ => BorderThickness,
        };
        if (name != "border-color" || !CssDocumentValues.TryParseColor(value, out var color))
        {
            return;
        }

        BorderBrush = new SolidColorBrush(color);
    }

    /// <summary>Applies an independently authored border edge.</summary>
    /// <param name="name">The edge property.</param>
    /// <param name="value">The shorthand edge declaration.</param>
    private void ApplyBorderSide(string name, string value)
    {
        var edge = new RichTableCellStyle();
        edge.ApplyBorder(value);
        if (edge.BorderThickness is not { } parsed)
        {
            return;
        }

        var existing = BorderThickness ?? default;
        BorderThickness = name switch
        {
            "border-left" => new(parsed.Left, existing.Top, existing.Right, existing.Bottom),
            "border-top" => new(existing.Left, parsed.Top, existing.Right, existing.Bottom),
            "border-right" => new(existing.Left, existing.Top, parsed.Right, existing.Bottom),
            _ => new(existing.Left, existing.Top, existing.Right, parsed.Bottom),
        };
        BorderBrush = edge.BorderBrush ?? BorderBrush;
    }

    /// <summary>Reads the shorthand border's width and color.</summary>
    /// <param name="value">The shorthand declaration.</param>
    private void ApplyBorder(string value)
    {
        if (value is "none" or "hidden" or "0")
        {
            BorderThickness = default(Thickness);
            return;
        }

        foreach (var token in value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (CssDocumentValues.ParseLength(token, null) is { } width)
            {
                BorderThickness = new(width);
            }
            else if (CssDocumentValues.TryParseColor(token, out var color))
            {
                BorderBrush = new SolidColorBrush(color);
            }
        }
    }
}
