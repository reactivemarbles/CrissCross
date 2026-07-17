// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides the Properties members for <see cref="RichTextBox"/>.</summary>
public partial class RichTextBox
{
    /// <summary>Occurs when text changes.</summary>
    public event EventHandler<TextChangedEventArgs>? TextChanged
    {
        add => AddHandler(TextChangedEvent, value);
        remove => RemoveHandler(TextChangedEvent, value);
    }

    /// <summary>Occurs when selection changes.</summary>
    public event EventHandler<RoutedEventArgs>? SelectionChanged
    {
        add => AddHandler(SelectionChangedEvent, value);
        remove => RemoveHandler(SelectionChangedEvent, value);
    }

    /// <summary>Occurs when formatting is applied.</summary>
    public event EventHandler<FormattingEventArgs>? FormattingApplied
    {
        add => AddHandler(FormattingAppliedEvent, value);
        remove => RemoveHandler(FormattingAppliedEvent, value);
    }

    /// <summary>Gets or sets the text content.</summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether Enter inserts a new line.</summary>
    public bool AcceptsReturn
    {
        get => GetValue(AcceptsReturnProperty);
        set => SetValue(AcceptsReturnProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether Tab inserts a tab character.</summary>
    public bool AcceptsTab
    {
        get => GetValue(AcceptsTabProperty);
        set => SetValue(AcceptsTabProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the control is read-only.</summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether text selection-only mode is enabled.</summary>
    public bool IsTextSelectionEnabled
    {
        get => GetValue(IsTextSelectionEnabledProperty);
        set => SetValue(IsTextSelectionEnabledProperty, value);
    }

    /// <summary>Gets or sets the wrapping mode.</summary>
    public TextWrapping TextWrapping
    {
        get => GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }

    /// <summary>Gets or sets the horizontal scroll bar visibility.</summary>
    public ScrollBarVisibility HorizontalScrollBarVisibility
    {
        get => GetValue(HorizontalScrollBarVisibilityProperty);
        set => SetValue(HorizontalScrollBarVisibilityProperty, value);
    }

    /// <summary>Gets or sets the vertical scroll bar visibility.</summary>
    public ScrollBarVisibility VerticalScrollBarVisibility
    {
        get => GetValue(VerticalScrollBarVisibilityProperty);
        set => SetValue(VerticalScrollBarVisibilityProperty, value);
    }

    /// <summary>Gets or sets the caret brush.</summary>
    public IBrush? CaretBrush
    {
        get => GetValue(CaretBrushProperty);
        set => SetValue(CaretBrushProperty, value);
    }

    /// <summary>Gets or sets the selection brush.</summary>
    public IBrush? SelectionBrush
    {
        get => GetValue(SelectionBrushProperty);
        set => SetValue(SelectionBrushProperty, value);
    }

    /// <summary>Gets or sets the watermark text.</summary>
    public string? Watermark
    {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether formatting is enabled.</summary>
    public bool IsFormattingEnabled
    {
        get => GetValue(IsFormattingEnabledProperty);
        set => SetValue(IsFormattingEnabledProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether content elements in the document are interactive.</summary>
    public bool IsDocumentEnabled
    {
        get => GetValue(IsDocumentEnabledProperty);
        set => SetValue(IsDocumentEnabledProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether drag and drop insertion is enabled.</summary>
    public bool IsDragDropEnabled
    {
        get => GetValue(IsDragDropEnabledProperty);
        set => SetValue(IsDragDropEnabledProperty, value);
    }

    /// <summary>Gets or sets the edit/display mode.</summary>
    public RichTextEditMode EditMode
    {
        get => GetValue(EditModeProperty);
        set => SetValue(EditModeProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether HTML clipboard payloads are copied and pasted.</summary>
    public bool IsRichClipboardEnabled
    {
        get => GetValue(IsRichClipboardEnabledProperty);
        set => SetValue(IsRichClipboardEnabledProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether image paste is enabled.</summary>
    public bool IsImagePasteEnabled
    {
        get => GetValue(IsImagePasteEnabledProperty);
        set => SetValue(IsImagePasteEnabledProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether image drop is enabled.</summary>
    public bool IsImageDropEnabled
    {
        get => GetValue(IsImageDropEnabledProperty);
        set => SetValue(IsImageDropEnabledProperty, value);
    }

    /// <summary>Gets or sets the maximum UTF-8 text file size accepted by runtime file drops.</summary>
    public long MaxDroppedTextFileBytes
    {
        get => GetValue(MaxDroppedTextFileBytesProperty);
        set => SetValue(MaxDroppedTextFileBytesProperty, Math.Max(0, value));
    }

    /// <summary>Gets or sets the clipboard adapter used by copy, cut, and paste operations.</summary>
    public IRichTextClipboardAdapter? ClipboardAdapter { get; set; }

    /// <summary>Gets a value indicating whether the current selection can be copied.</summary>
    public bool CanCopy => HasSelection;

    /// <summary>Gets a value indicating whether the current selection can be cut.</summary>
    public bool CanCut => HasSelection && !IsReadOnlyInternal;

    /// <summary>Gets a value indicating whether the clipboard can be pasted into the current document.</summary>
    public bool CanPaste =>
        !IsReadOnlyInternal
        && (RichTextSystemClipboard.IsAvailable(GetSystemClipboard()) || HasPasteContent(GetClipboardAdapter()));

    /// <summary>Gets a value indicating whether undo history is available.</summary>
    public bool CanUndo => !IsReadOnlyInternal && _undoStack.Count > 0;

    /// <summary>Gets a value indicating whether redo history is available.</summary>
    public bool CanRedo => !IsReadOnlyInternal && _redoStack.Count > 0;

    /// <summary>Gets a value indicating whether formatting can be applied to the current selection.</summary>
    public bool CanApplyFormatting => IsFormattingEnabled && !IsReadOnlyInternal && HasSelection;

    /// <summary>Gets the copy command.</summary>
    public ICommand CopyCommand { get; }

    /// <summary>Gets the cut command.</summary>
    public ICommand CutCommand { get; }

    /// <summary>Gets the paste command.</summary>
    public ICommand PasteCommand { get; }

    /// <summary>Gets the undo command.</summary>
    public ICommand UndoCommand { get; }

    /// <summary>Gets the redo command.</summary>
    public ICommand RedoCommand { get; }

    /// <summary>Gets the select all command.</summary>
    public ICommand SelectAllCommand { get; }

    /// <summary>Gets the bold formatting command.</summary>
    public ICommand ToggleBoldCommand { get; }

    /// <summary>Gets the italic formatting command.</summary>
    public ICommand ToggleItalicCommand { get; }

    /// <summary>Gets the underline formatting command.</summary>
    public ICommand ToggleUnderlineCommand { get; }

    /// <summary>Gets the strikethrough formatting command.</summary>
    public ICommand ToggleStrikethroughCommand { get; }

    /// <summary>Gets the clear formatting command.</summary>
    public ICommand ClearFormattingCommand { get; }

    /// <summary>Gets the font family formatting command.</summary>
    public ICommand SetFontFamilyCommand { get; }

    /// <summary>Gets the font size formatting command.</summary>
    public ICommand SetFontSizeCommand { get; }

    /// <summary>Gets the foreground color formatting command.</summary>
    public ICommand SetForegroundCommand { get; }

    /// <summary>Gets the highlight color formatting command.</summary>
    public ICommand SetHighlightCommand { get; }

    /// <summary>Gets or sets the selection start index.</summary>
    public int SelectionStart
    {
        get => GetValue(SelectionStartProperty);
        set => SetValue(SelectionStartProperty, value);
    }

    /// <summary>Gets or sets the selection end index.</summary>
    public int SelectionEnd
    {
        get => GetValue(SelectionEndProperty);
        set => SetValue(SelectionEndProperty, value);
    }

    /// <summary>Gets or sets the caret index.</summary>
    public int CaretIndex
    {
        get => GetValue(CaretIndexProperty);
        set => SetValue(CaretIndexProperty, value);
    }

    /// <summary>Gets the document model.</summary>
    public FlowDocument Document { get; }

    /// <summary>Gets the text selection.</summary>
    public TextSelection Selection { get; }

    /// <summary>Gets or sets the current caret position.</summary>
    public TextPointer CaretPosition
    {
        get => Document.GetTextPointer(Math.Clamp(CaretIndex, 0, Document.Length));
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (!ReferenceEquals(value.Document, Document))
            {
                throw new ArgumentException(
                    "Pointer does not belong to this document.",
                    nameof(value));
            }

            CaretIndex = value.Offset;
            SelectionStart = value.Offset;
            SelectionEnd = value.Offset;
            ApplySelectionToTextBox();
            SynchronizeSelectionFromIndexes(raiseEvent: true);
        }
    }

    /// <summary>Gets a value indicating whether text is selected.</summary>
    public bool HasSelection => SelectionStart != SelectionEnd;

    /// <summary>Gets the current selection length.</summary>
    public int SelectionLength =>
        string.IsNullOrEmpty(Text) ? 0 : Math.Abs(SelectionEnd - SelectionStart);

    /// <summary>Gets the selected text.</summary>
    public string SelectedText
    {
        get
        {
            if (!HasSelection || string.IsNullOrEmpty(Text))
            {
                return string.Empty;
            }

            var start = Math.Min(SelectionStart, SelectionEnd);
            var length = Math.Abs(SelectionEnd - SelectionStart);
            return Document.GetTextRange(
                Document.GetTextPointer(start),
                Document.GetTextPointer(start + length));
        }
    }

    /// <summary>Gets the selected content as an HTML fragment preserving inline formatting where possible.</summary>
    public string SelectedHtml => CreateSelectedHtml();

    /// <summary>Gets the current HTML/markup representation for the document.</summary>
    public string Html => Document.Text;

    /// <summary>Gets a Markdown-compatible text representation.</summary>
    public string Markdown => PlainText;

    /// <summary>Gets the rendered plain text for the current document.</summary>
    public string PlainText => Document.PlainText;

    /// <summary>Gets the IsReadOnlyInternal value.</summary>
    private bool IsReadOnlyInternal =>
        IsReadOnly || IsTextSelectionEnabled || EditMode == RichTextEditMode.Display;

    /// <summary>Returns the text position nearest the provided point.</summary>
    /// <param name="point">Point in control coordinates.</param>
    /// <param name="snapToText">When false and point is outside content bounds, returns null.</param>
    /// <returns>The nearest document position.</returns>
    public TextPointer? GetPositionFromPoint(in Point point, bool snapToText)
    {
        var textBounds = GetTextHitTestBounds();
        if (!snapToText && !textBounds.Contains(point))
        {
            return null;
        }

        if (Document.Length == 0)
        {
            return Document.GetTextPointer(0);
        }

        return TryGetRenderedDocumentOffsetFromPoint(point, out var renderedOffset)
            ? Document.GetTextPointer(renderedOffset)
            : Document.GetTextPointer(EstimateDocumentOffsetFromPoint(point, textBounds));
    }
}
