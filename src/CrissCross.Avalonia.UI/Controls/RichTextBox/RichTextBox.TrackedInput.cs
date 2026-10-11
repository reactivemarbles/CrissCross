// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Routes composed plain-text editor changes through revision tracking.</summary>
public partial class RichTextBox
{
    /// <summary>Applies one tracked mutation from the continuous editing surface.</summary>
    /// <param name="newText">The newly rendered plain text.</param>
    /// <returns>Whether revision tracking handled the change.</returns>
    private bool TryApplyTrackedTextChange(string newText)
    {
        if (!TrackChanges)
        {
            return false;
        }

        if (Document.PlainText == newText || IsReadOnlyInternal)
        {
            return true;
        }

        var replacement = RichTextEditingShim.CalculateReplacement(Document.PlainText, newText);
        Select(replacement.Start, replacement.Length);
        ReplaceSelection(replacement.Text);
        return true;
    }
}
