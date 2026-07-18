// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides the History members for <see cref="RichTextBox"/>.</summary>
public partial class RichTextBox
{
    /// <summary>Creates the selected HTML fragment.</summary>
    /// <returns>The selected HTML fragment.</returns>
    private string CreateSelectedHtml()
    {
        if (!HasSelection || string.IsNullOrEmpty(Text))
        {
            return string.Empty;
        }

        var start = Math.Min(SelectionStart, SelectionEnd);
        var end = Math.Max(SelectionStart, SelectionEnd);
        var builder = new StringBuilder();
        foreach (var segment in Document.Segments)
        {
            RichTextHelpers.AppendSelectedSegmentHtml(segment, start, end, builder);
        }

        return builder.ToString();
    }

    /// <summary>Provides the CaptureHistory member.</summary>
    /// <returns>The result.</returns>
    private RichTextHistoryEntry CaptureHistory() =>
        new(Document.Text, SelectionStart, SelectionEnd, CaretIndex);

    /// <summary>Provides the CommitHistory member.</summary>
    /// <param name="before">The before value.</param>
    private void CommitHistory(RichTextHistoryEntry before)
    {
        if (string.Equals(before.Html, Document.Text, StringComparison.Ordinal))
        {
            NotifyCommandStateChanged();
            return;
        }

        _undoStack.Push(before);
        _redoStack.Clear();
        NotifyCommandStateChanged();
    }

    /// <summary>Provides the RestoreHistory member.</summary>
    /// <param name="entry">The entry value.</param>
    private void RestoreHistory(RichTextHistoryEntry entry)
    {
        _isUpdating = true;
        Document.SetText(entry.Html);
        Text = entry.Html;
        if (_editingTextBox is not null)
        {
            _editingTextBox.Text = Document.PlainText;
        }

        _isUpdating = false;
        SelectionStart = Math.Clamp(entry.SelectionStart, 0, Document.Length);
        SelectionEnd = Math.Clamp(entry.SelectionEnd, 0, Document.Length);
        CaretIndex = Math.Clamp(entry.CaretIndex, 0, Document.Length);
        ApplySelectionToTextBox();
        SynchronizeSelectionFromIndexes(raiseEvent: true);
        UpdateFormattedPresenter();
        RaiseEvent(new TextChangedEventArgs(TextChangedEvent));
        NotifyCommandStateChanged();
    }

    /// <summary>Provides the UndoCore member.</summary>
    private void UndoCore()
    {
        if (!CanUndo)
        {
            return;
        }

        var current = CaptureHistory();
        var previous = _undoStack.Pop();
        _redoStack.Push(current);
        RestoreHistory(previous);
    }

    /// <summary>Provides the RedoCore member.</summary>
    private void RedoCore()
    {
        if (!CanRedo)
        {
            return;
        }

        var current = CaptureHistory();
        var next = _redoStack.Pop();
        _undoStack.Push(current);
        RestoreHistory(next);
    }

    /// <summary>Provides the CopyCore member.</summary>
    private void CopyCore()
    {
        if (!CanCopy)
        {
            return;
        }

        if (ClipboardAdapter is not null || !RichTextSystemClipboard.IsAvailable(GetSystemClipboard()))
        {
            CopyToAdapter(GetClipboardAdapter());
        }
        else
        {
            _ = WriteSelectionToSystemClipboardAsync();
        }

        NotifyCommandStateChanged();
    }

    /// <summary>Copies the current selection into a deterministic clipboard adapter.</summary>
    /// <param name="clipboard">The target clipboard adapter.</param>
    private void CopyToAdapter(IRichTextClipboardAdapter clipboard)
    {
        var selectedText = SelectedText;
        clipboard.SetPlainText(selectedText);
        if (!IsRichClipboardEnabled)
        {
            return;
        }

        var selectedHtml = SelectedHtml;
        clipboard.SetHtml(
            string.IsNullOrEmpty(selectedHtml)
                ? HtmlClipboardUtilities.EncodePlainText(selectedText)
                : selectedHtml);
    }

    /// <summary>Copies the current selection into the platform clipboard.</summary>
    /// <returns>A task that completes when the clipboard write has finished.</returns>
    private Task WriteSelectionToSystemClipboardAsync()
    {
        var selectedText = SelectedText;
        var selectedHtml = IsRichClipboardEnabled ? SelectedHtml : null;
        if (IsRichClipboardEnabled && string.IsNullOrEmpty(selectedHtml))
        {
            selectedHtml = HtmlClipboardUtilities.EncodePlainText(selectedText);
        }

        return RichTextSystemClipboard.WriteAsync(GetSystemClipboard(), selectedText, selectedHtml);
    }

    /// <summary>Provides the CutCore member.</summary>
    private void CutCore()
    {
        if (!CanCut)
        {
            return;
        }

        CopyCore();
        ReplaceSelectionWithHtml(string.Empty);
    }

    /// <summary>Provides the PasteCore member.</summary>
    private void PasteCore()
    {
        if (IsReadOnlyInternal)
        {
            return;
        }

        if (ClipboardAdapter is null && RichTextSystemClipboard.IsAvailable(GetSystemClipboard()))
        {
            _ = PasteFromSystemClipboardAsync();
            return;
        }

        PasteFromAdapter(GetClipboardAdapter());
    }

    /// <summary>Pastes data from a deterministic clipboard adapter.</summary>
    /// <param name="clipboard">The source clipboard adapter.</param>
    private void PasteFromAdapter(IRichTextClipboardAdapter clipboard)
    {
        if (IsReadOnlyInternal || !HasPasteContent(clipboard))
        {
            return;
        }

        if (TryPasteHtml(clipboard))
        {
            return;
        }

        if (TryPasteImage(clipboard))
        {
            return;
        }

        if (!clipboard.ContainsPlainText)
        {
            return;
        }

        ReplaceSelection(clipboard.PlainText ?? string.Empty);
    }

    /// <summary>Reads and pastes content from the platform clipboard.</summary>
    /// <returns>A task that completes when the clipboard read has finished.</returns>
    private async Task PasteFromSystemClipboardAsync()
    {
        var content = await RichTextSystemClipboard.ReadAsync(GetSystemClipboard()).ConfigureAwait(true);
        var clipboard = new RichTextMemoryClipboardAdapter
        {
            PlainText = content.PlainText,
            HtmlText = content.HtmlText,
            ImageSource = content.ImageSource,
        };

        PasteFromAdapter(clipboard);
    }

    /// <summary>Tries to paste HTML clipboard content.</summary>
    /// <param name="clipboard">The clipboard adapter.</param>
    /// <returns><see langword="true"/> when HTML was pasted.</returns>
    private bool TryPasteHtml(IRichTextClipboardAdapter clipboard)
    {
        if (
            !IsRichClipboardEnabled
            || !clipboard.ContainsHtml
            || string.IsNullOrEmpty(clipboard.HtmlText))
        {
            return false;
        }

        var fragment =
            HtmlClipboardUtilities.ExtractFragment(clipboard.HtmlText) ?? clipboard.HtmlText;
        ReplaceSelectionWithHtml(fragment ?? string.Empty);
        return true;
    }

    /// <summary>Tries to paste image clipboard content.</summary>
    /// <param name="clipboard">The clipboard adapter.</param>
    /// <returns><see langword="true"/> when an image was pasted.</returns>
    private bool TryPasteImage(IRichTextClipboardAdapter clipboard) =>
        IsImagePasteEnabled
        && clipboard.ContainsImage
        && TryInsertImage(clipboard.ImageSource, requireDragDrop: false);

    /// <summary>Provides the TryInsertImage member.</summary>
    /// <param name="imageSource">The imageSource value.</param>
    /// <param name="requireDragDrop">The requireDragDrop value.</param>
    /// <returns>The result.</returns>
    private bool TryInsertImage(string? imageSource, bool requireDragDrop)
    {
        if (!CanInsertImage(requireDragDrop))
        {
            return false;
        }

        if (!RichTextHelpers.IsSupportedImageSource(imageSource))
        {
            return false;
        }

        ReplaceSelectionWithHtml(RichTextHelpers.CreateImageHtml(imageSource!.Trim()));
        return true;
    }

    /// <summary>Determines whether image insertion is available for the current mode.</summary>
    /// <param name="requireDragDrop">Whether drag and drop state is required.</param>
    /// <returns><see langword="true"/> when image insertion is enabled.</returns>
    private bool CanInsertImage(bool requireDragDrop) =>
        !IsReadOnlyInternal
        && (requireDragDrop ? IsDragDropEnabled && IsImageDropEnabled : IsImagePasteEnabled);

    /// <summary>Provides the NotifyCommandStateChanged member.</summary>
    private void NotifyCommandStateChanged()
    {
        foreach (
            var command in new[]
            {
                CopyCommand,
                CutCommand,
                PasteCommand,
                UndoCommand,
                RedoCommand,
                SelectAllCommand,
                ToggleBoldCommand,
                ToggleItalicCommand,
                ToggleUnderlineCommand,
                ToggleStrikethroughCommand,
                ClearFormattingCommand,
                SetFontFamilyCommand,
                SetFontSizeCommand,
                SetForegroundCommand,
                SetHighlightCommand,
            })
        {
            if (command is RichTextCommand richTextCommand)
            {
                richTextCommand.RaiseCanExecuteChanged();
            }
        }
    }

    /// <summary>Provides the HasAnyFormatting member.</summary>
    /// <returns>The result.</returns>
    private bool HasAnyFormatting() => Document.Segments.Any(RichTextHelpers.HasFormatting);

    /// <summary>Provides the ApplyFormattingToSelection member.</summary>
    /// <param name="formatType">The formatType value.</param>
    private void ApplyFormattingToSelection(TextFormatType formatType)
    {
        if (!IsFormattingEnabled || IsReadOnlyInternal || !HasSelection)
        {
            return;
        }

        var start = Math.Min(SelectionStart, SelectionEnd);
        var length = Math.Abs(SelectionEnd - SelectionStart);
        var before = CaptureHistory();

        _ = Document.ToggleFormatting(start, length, formatType);
        SyncTextFromDocument();
        UpdateFormattedPresenter();
        CommitHistory(before);

        RaiseEvent(new FormattingEventArgs(FormattingAppliedEvent, this, formatType, SelectedText));
    }

    /// <summary>Provides the ApplySpanStyleToSelection member.</summary>
    /// <param name="style">The style value.</param>
    private void ApplySpanStyleToSelection(string style)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(style);

        if (!IsFormattingEnabled || IsReadOnlyInternal || !HasSelection)
        {
            return;
        }

        var safeStyle = style.Replace("\"", "'", StringComparison.Ordinal);
        var selectionText = SelectedText;
        if (string.IsNullOrWhiteSpace(selectionText))
        {
            return;
        }

        ReplaceSelectionWithHtml($"<span style=\"{safeStyle}\">{selectionText}</span>");
    }
}
