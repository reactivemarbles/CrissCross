// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Net;
using System.Text;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides formatting for text entered after a collapsed selection.</summary>
public partial class RichTextBox
{
    /// <summary>Underline decoration token.</summary>
    private const string UnderlineDecoration = "underline";

    /// <summary>Strike decoration token.</summary>
    private const string StrikeDecoration = "line-through";

    /// <summary>Explicit character styles for the current insertion position.</summary>
    private readonly Dictionary<string, string> _typingStyles = [];

    /// <summary>The insertion position associated with the pending styles.</summary>
    private int _typingStylePosition = -1;

    /// <summary>Resets insertion formatting to the editor's default typography.</summary>
    private void ClearTypingFormatting()
    {
        _typingStyles.Clear();
        _typingStylePosition = CaretIndex;
        SetTypingStyle($"font-weight:normal;font-style:normal;text-decoration:none;font-family:{FontFamily.Name};"
            + $"font-size:{FontSize.ToString(CultureInfo.InvariantCulture)}px;color:{Foreground};background-color:transparent;");
        NotifyCommandStateChanged();
    }

    /// <summary>Updates character styles at the insertion position.</summary>
    /// <param name="style">CSS declarations to apply to newly entered text.</param>
    private void SetTypingStyle(string style)
    {
        EnsureTypingPosition(CaretIndex);
        foreach (var declaration in style.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = declaration.IndexOf(':');
            if (separator > 0)
            {
                _typingStyles[declaration[..separator].Trim()] = declaration[(separator + 1)..].Trim();
            }
        }
    }

    /// <summary>Toggles a character style at the insertion position.</summary>
    /// <param name="format">The character format to toggle.</param>
    private void ToggleTypingFormat(TextFormatType format)
    {
        EnsureTypingPosition(CaretIndex);
        var segment = GetTypingSegment();
        var property = format switch
        {
            TextFormatType.Bold => "font-weight",
            TextFormatType.Italic => "font-style",
            _ => "text-decoration",
        };
        var enabledValue = format switch
        {
            TextFormatType.Bold => "bold",
            TextFormatType.Italic => "italic",
            TextFormatType.Underline => UnderlineDecoration,
            _ => StrikeDecoration,
        };
        if (property == "text-decoration")
        {
            ToggleTypingDecoration(format, segment);
            return;
        }

        var active = format == TextFormatType.Bold ? segment?.IsBold == true : segment?.IsItalic == true;
        if (_typingStyles.TryGetValue(property, out var pending))
        {
            active = pending == enabledValue;
        }

        _typingStyles[property] = active ? "normal" : enabledValue;
    }

    /// <summary>Toggles one decoration while retaining the other.</summary>
    /// <param name="format">The selected decoration.</param>
    /// <param name="segment">Inherited character styles.</param>
    private void ToggleTypingDecoration(TextFormatType format, TextSegment? segment)
    {
        const string property = "text-decoration";
        var underline = segment?.IsUnderline == true;
        var strike = segment?.IsStrikethrough == true;
        if (_typingStyles.TryGetValue(property, out var pending))
        {
            underline = pending.Contains(UnderlineDecoration, StringComparison.Ordinal);
            strike = pending.Contains(StrikeDecoration, StringComparison.Ordinal);
        }

        if (format == TextFormatType.Underline)
        {
            underline = !underline;
        }
        else
        {
            strike = !strike;
        }

        var value = underline ? UnderlineDecoration : string.Empty;
        if (strike)
        {
            value = $"{value} {StrikeDecoration}";
        }

        _typingStyles[property] = value.Length == 0 ? "none" : value.Trim();
    }

    /// <summary>Encodes entered text with the pending character styles.</summary>
    /// <param name="text">The entered text.</param>
    /// <param name="position">The insertion position.</param>
    /// <returns>A portable HTML fragment.</returns>
    private string EncodeTypedText(string text, int position)
    {
        EnsureTypingPosition(position);
        var encoded = HtmlClipboardUtilities.EncodePlainText(text);
        if (text.Length == 0 || _typingStyles.Count == 0)
        {
            return encoded;
        }

        var style = new StringBuilder();
        foreach (var declaration in _typingStyles)
        {
            _ = style.Append(declaration.Key).Append(':').Append(declaration.Value).Append(';');
        }

        _typingStylePosition = position + text.Length;
        return $"<span style=\"{WebUtility.HtmlEncode(style.ToString())}\">{encoded}</span>";
    }

    /// <summary>Clears insertion styles after the caret moves to another position.</summary>
    /// <param name="position">The current insertion position.</param>
    private void EnsureTypingPosition(int position)
    {
        if (_typingStylePosition == position)
        {
            return;
        }

        _typingStyles.Clear();
        _typingStylePosition = position;
    }

    /// <summary>Finds the character style inherited at the insertion position.</summary>
    /// <returns>The adjacent text segment.</returns>
    private TextSegment? GetTypingSegment()
    {
        TextSegment? previous = null;
        foreach (var segment in Document.Segments)
        {
            if (segment.StartIndex > CaretIndex)
            {
                break;
            }

            if (segment.Length > 0 && !segment.IsImage && segment.Table is null)
            {
                previous = segment;
            }
        }

        return previous;
    }
}
