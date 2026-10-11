// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Reconciles plain-text editor changes with the rich HTML document model.</summary>
internal static class RichTextEditingShim
{
    /// <summary>Applies the smallest plain-text change while retaining markup outside the edited range.</summary>
    /// <param name="document">The document to update.</param>
    /// <param name="newText">The new rendered text.</param>
    /// <param name="encodeText">Optional insertion formatting encoder.</param>
    /// <returns><see langword="true"/> when the document changed.</returns>
    internal static bool ApplyPlainTextChange(FlowDocument document, string newText, Func<string, int, string>? encodeText = null)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(newText);

        var oldText = document.PlainText;
        if (string.Equals(oldText, newText, StringComparison.Ordinal))
        {
            return false;
        }

        var replacement = CalculateReplacement(oldText, newText);
        var prefixLength = replacement.Start;
        var removedLength = replacement.Length;
        var insertedText = replacement.Text;

        if (prefixLength == 0 && removedLength == oldText.Length)
        {
            document.SetText(EncodeInsertion(insertedText, prefixLength, encodeText));
            return true;
        }

        for (var index = removedLength - 1; index >= 0;)
        {
            var previousLength = document.Length;
            document.Delete(prefixLength + index, 1);
            var projectedCharactersRemoved = Math.Max(1, previousLength - document.Length);
            index -= projectedCharactersRemoved;
        }

        if (insertedText.Length == 0)
        {
            return true;
        }

        document.Insert(prefixLength, EncodeInsertion(insertedText, prefixLength, encodeText));
        return true;
    }

    /// <summary>Finds the smallest changed range between two rendered text values.</summary>
    /// <param name="oldText">The committed text.</param>
    /// <param name="newText">The replacement text.</param>
    /// <returns>The changed range and inserted characters.</returns>
    internal static (int Start, int Length, string Text) CalculateReplacement(string oldText, string newText)
    {
        var prefix = GetCommonPrefixLength(oldText, newText);
        var suffix = GetCommonSuffixLength(oldText, newText, prefix);
        return (prefix, oldText.Length - prefix - suffix, newText[prefix..(newText.Length - suffix)]);
    }

    /// <summary>Encodes replacement characters using the editor's insertion styles.</summary>
    /// <param name="text">Inserted characters.</param>
    /// <param name="position">Document insertion position.</param>
    /// <param name="encoder">Optional custom encoder.</param>
    /// <returns>The replacement HTML.</returns>
    private static string EncodeInsertion(string text, int position, Func<string, int, string>? encoder) =>
        encoder?.Invoke(text, position) ?? HtmlClipboardUtilities.EncodePlainText(text);

    /// <summary>Gets the number of matching characters at the start of two strings.</summary>
    /// <param name="left">The first string.</param>
    /// <param name="right">The second string.</param>
    /// <returns>The common prefix length.</returns>
    private static int GetCommonPrefixLength(string left, string right)
    {
        var maximum = Math.Min(left.Length, right.Length);
        var index = 0;
        while (index < maximum && left[index] == right[index])
        {
            index++;
        }

        return index;
    }

    /// <summary>Gets the number of matching characters at the end of two strings.</summary>
    /// <param name="left">The first string.</param>
    /// <param name="right">The second string.</param>
    /// <param name="prefixLength">The already matched prefix length.</param>
    /// <returns>The common suffix length.</returns>
    private static int GetCommonSuffixLength(string left, string right, int prefixLength)
    {
        var maximum = Math.Min(left.Length, right.Length) - prefixLength;
        var length = 0;
        while (length < maximum && left[left.Length - length - 1] == right[right.Length - length - 1])
        {
            length++;
        }

        return length;
    }
}
