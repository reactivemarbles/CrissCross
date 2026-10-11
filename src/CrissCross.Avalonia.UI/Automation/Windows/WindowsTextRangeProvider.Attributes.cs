// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Exposes effective rich character attributes.</summary>
internal sealed partial class WindowsTextRangeProvider
{
    /// <summary>Stores the BackgroundColorAttribute value.</summary>
    private const int BackgroundColorAttribute = 40_001;

    /// <summary>Stores the FontNameAttribute value.</summary>
    private const int FontNameAttribute = 40_005;

    /// <summary>Stores the FontSizeAttribute value.</summary>
    private const int FontSizeAttribute = 40_006;

    /// <summary>Stores the FontWeightAttribute value.</summary>
    private const int FontWeightAttribute = 40_007;

    /// <summary>Stores the ForegroundColorAttribute value.</summary>
    private const int ForegroundColorAttribute = 40_008;

    /// <summary>Stores the IsHiddenAttribute value.</summary>
    private const int IsHiddenAttribute = 40_013;

    /// <summary>Stores the IsItalicAttribute value.</summary>
    private const int IsItalicAttribute = 40_014;

    /// <summary>Stores the IsReadOnlyAttribute value.</summary>
    private const int IsReadOnlyAttribute = 40_015;

    /// <summary>Stores the StrikethroughStyleAttribute value.</summary>
    private const int StrikethroughStyleAttribute = 40_026;

    /// <summary>Stores the UnderlineStyleAttribute value.</summary>
    private const int UnderlineStyleAttribute = 40_030;

    /// <summary>Stores the PointsPerDip value.</summary>
    private const double PointsPerDip = 0.75;

    /// <inheritdoc/>
    public object GetAttributeValue(int attributeId) => Invoke(() => GetAttributeValueCore(attributeId));

    /// <inheritdoc/>
    public IWindowsTextRangeProvider? FindAttribute(int attributeId, object value, bool backward) => Invoke(() => FindAttributeCore(attributeId, value, backward));

    /// <summary>Resolves nested table-cell formatting at a document position.</summary>
    /// <param name="segments">The positioned segments.</param>
    /// <param name="position">The collection-local insertion position.</param>
    /// <returns>The innermost segment.</returns>
    private static TextSegment? FindSegment(IReadOnlyList<TextSegment> segments, int position)
    {
        foreach (var segment in segments)
        {
            if (position < segment.StartIndex || position >= segment.EndIndex)
            {
                continue;
            }

            if (segment.Table is { } table)
            {
                foreach (var cell in table.Cells)
                {
                    var start = segment.StartIndex + cell.StartIndex;
                    if (position >= start && position < start + cell.Length)
                    {
                        return FindSegment(cell.Segments, position - start) ?? segment;
                    }
                }
            }

            return segment;
        }

        return null;
    }

    /// <summary>Encodes a solid brush as a Windows COLORREF.</summary>
    /// <param name="brush">The effective brush.</param>
    /// <returns>The COLORREF, or null for non-solid brushes.</returns>
    private static object? GetColor(IBrush? brush) => brush is ISolidColorBrush solid
        ? solid.Color.R | (solid.Color.G << 8) | (solid.Color.B << 16)
        : null;

    /// <summary>Gets a segment's text decoration attribute.</summary>
    /// <param name="attributeId">The native attribute identifier.</param>
    /// <param name="segment">The effective segment.</param>
    /// <returns>The decoration style, or null for other attributes.</returns>
    private static object? GetDecorationAttribute(int attributeId, TextSegment? segment) => attributeId switch
    {
        UnderlineStyleAttribute => segment?.IsUnderline == true ? 1 : 0,
        StrikethroughStyleAttribute => segment?.IsStrikethrough == true ? 1 : 0,
        _ => null,
    };

    /// <summary>Gets an attribute shared by the entire range, or the native mixed sentinel.</summary>
    /// <param name="attributeId">The native attribute identifier.</param>
    /// <returns>The attribute or reserved value.</returns>
    private object GetAttributeValueCore(int attributeId)
    {
        var initial = GetAttributeAt(attributeId, Start);
        if (initial is null)
        {
            return WindowsAutomationNative.GetReservedNotSupportedValue();
        }

        foreach (var boundary in WindowsTextUnits.GetBoundaries(Editor, WindowsTextUnit.Format))
        {
            if (boundary > Start && boundary < End && !Equals(initial, GetAttributeAt(attributeId, boundary)))
            {
                return WindowsAutomationNative.GetReservedMixedAttributeValue();
            }
        }

        return initial;
    }

    /// <summary>Searches maximal contiguous runs with the requested attribute.</summary>
    /// <param name="attributeId">The attribute identifier.</param>
    /// <param name="value">The requested value.</param>
    /// <param name="backward">Whether to find the last run.</param>
    /// <returns>The matching range, or null.</returns>
    private IWindowsTextRangeProvider? FindAttributeCore(int attributeId, object value, bool backward)
    {
        ArgumentNullException.ThrowIfNull(value);
        var boundaries = WindowsTextUnits.GetBoundaries(Editor, WindowsTextUnit.Format);
        var runStart = -1;
        var runEnd = 0;
        IWindowsTextRangeProvider? result = null;
        for (var index = 0; index + 1 < boundaries.Count; index++)
        {
            var first = Math.Max(Start, boundaries[index]);
            var last = Math.Min(End, boundaries[index + 1]);
            if (last <= first)
            {
                continue;
            }

            if (Equals(GetAttributeAt(attributeId, first), value))
            {
                runStart = runStart < 0 ? first : runStart;
                runEnd = last;
            }
            else if (runStart >= 0)
            {
                result = new WindowsTextRangeProvider(_node, runStart, runEnd);
                if (!backward)
                {
                    return result;
                }

                runStart = -1;
            }
        }

        return runStart >= 0 ? new WindowsTextRangeProvider(_node, runStart, runEnd) : result;
    }

    /// <summary>Gets the effective attribute at one insertion position.</summary>
    /// <param name="attributeId">The native attribute identifier.</param>
    /// <param name="offset">The insertion position.</param>
    /// <returns>The effective attribute, or null for unsupported attributes.</returns>
    private object? GetAttributeAt(int attributeId, int offset)
    {
        var position = Math.Min(offset, Math.Max(0, Editor.Document.Length - 1));
        var segment = FindSegment(Editor.Document.Segments, position);
        return GetFontAttribute(attributeId, segment)
            ?? GetDecorationAttribute(attributeId, segment)
            ?? GetGeneralAttribute(attributeId, segment);
    }

    /// <summary>Gets an effective font attribute.</summary>
    /// <param name="attributeId">The native attribute identifier.</param>
    /// <param name="segment">The effective segment.</param>
    /// <returns>The font attribute, or null for other attributes.</returns>
    private object? GetFontAttribute(int attributeId, TextSegment? segment) => attributeId switch
    {
        FontNameAttribute => (segment?.FontFamily ?? Editor.FontFamily).Name,
        FontSizeAttribute => (segment?.FontSize ?? Editor.FontSize) * PointsPerDip,
        FontWeightAttribute => segment is null ? (int)Editor.FontWeight : (int)segment.FontWeight,
        IsItalicAttribute => segment?.IsItalic ?? (Editor.FontStyle == FontStyle.Italic),
        _ => null,
    };

    /// <summary>Gets an effective state or color attribute.</summary>
    /// <param name="attributeId">The native attribute identifier.</param>
    /// <param name="segment">The effective segment.</param>
    /// <returns>The general attribute, or null for unsupported attributes.</returns>
    private object? GetGeneralAttribute(int attributeId, TextSegment? segment) => attributeId switch
    {
        IsHiddenAttribute => false,
        IsReadOnlyAttribute => Editor.IsAutomationReadOnly,
        ForegroundColorAttribute => GetColor(segment?.Foreground ?? Editor.Foreground),
        BackgroundColorAttribute => GetColor(segment?.Background ?? Editor.Background),
        _ => null,
    };
}
