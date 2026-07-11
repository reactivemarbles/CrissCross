// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Reconciles plain-text editor changes with the rich HTML document model.</summary>
internal static class RichTextEditingShim
{
    /// <summary>Applies the smallest plain-text change while retaining markup outside the edited range.</summary>
    /// <param name="document">The document to update.</param>
    /// <param name="newText">The new rendered text.</param>
    /// <returns><see langword="true"/> when the document changed.</returns>
    public static bool ApplyPlainTextChange(FlowDocument document, string newText)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(newText);

        var oldText = document.PlainText;
        if (string.Equals(oldText, newText, StringComparison.Ordinal))
        {
            return false;
        }

        var prefixLength = GetCommonPrefixLength(oldText, newText);
        var suffixLength = GetCommonSuffixLength(oldText, newText, prefixLength);
        var removedLength = oldText.Length - prefixLength - suffixLength;
        var insertedLength = newText.Length - prefixLength - suffixLength;
        var insertedText = insertedLength == 0 ? string.Empty : newText[prefixLength..(prefixLength + insertedLength)];

        if (prefixLength == 0 && removedLength == oldText.Length)
        {
            document.SetText(HtmlClipboardUtilities.EncodePlainText(insertedText));
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

        document.Insert(prefixLength, HtmlClipboardUtilities.EncodePlainText(insertedText));
        return true;
    }

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
