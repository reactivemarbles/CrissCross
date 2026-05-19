// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a rich editing control that provides multi-line text editing capabilities with visual formatting.
/// Supports bold, italic, underline, and strikethrough formatting via keyboard shortcuts and context menu.
/// </summary>
/// <remarks>
/// Key features matching WPF RichTextBox:
/// <list type="bullet">
/// <item><description>Multi-line text editing with word wrap.</description></item>
/// <item><description>Visual text formatting (Bold, Italic, Underline, Strikethrough).</description></item>
/// <item><description>Keyboard shortcuts (Ctrl+B, Ctrl+I, Ctrl+U).</description></item>
/// <item><description>Context menu with formatting options.</description></item>
/// <item><description>Undo/Redo support.</description></item>
/// <item><description>Save/Load content to/from files or streams.</description></item>
/// </list>
/// </remarks>
public class RichTextBox : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="Text"/>.
    /// </summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<RichTextBox, string?>(nameof(Text), defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Property for <see cref="AcceptsReturn"/>.
    /// </summary>
    public static readonly StyledProperty<bool> AcceptsReturnProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(AcceptsReturn), true);

    /// <summary>
    /// Property for <see cref="AcceptsTab"/>.
    /// </summary>
    public static readonly StyledProperty<bool> AcceptsTabProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(AcceptsTab), true);

    /// <summary>
    /// Property for <see cref="IsReadOnly"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsReadOnly), false);

    /// <summary>
    /// Property for <see cref="IsTextSelectionEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsTextSelectionEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsTextSelectionEnabled), false);

    /// <summary>
    /// Property for <see cref="TextWrapping"/>.
    /// </summary>
    public static readonly StyledProperty<TextWrapping> TextWrappingProperty =
        AvaloniaProperty.Register<RichTextBox, TextWrapping>(nameof(TextWrapping), TextWrapping.Wrap);

    /// <summary>
    /// Property for <see cref="HorizontalScrollBarVisibility"/>.
    /// </summary>
    public static readonly StyledProperty<ScrollBarVisibility> HorizontalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<RichTextBox, ScrollBarVisibility>(nameof(HorizontalScrollBarVisibility), ScrollBarVisibility.Disabled);

    /// <summary>
    /// Property for <see cref="VerticalScrollBarVisibility"/>.
    /// </summary>
    public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<RichTextBox, ScrollBarVisibility>(nameof(VerticalScrollBarVisibility), ScrollBarVisibility.Auto);

    /// <summary>
    /// Property for <see cref="CaretBrush"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush?> CaretBrushProperty =
        AvaloniaProperty.Register<RichTextBox, IBrush?>(nameof(CaretBrush), Brushes.White);

    /// <summary>
    /// Property for <see cref="SelectionBrush"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush?> SelectionBrushProperty =
        AvaloniaProperty.Register<RichTextBox, IBrush?>(nameof(SelectionBrush), Brush.Parse("#400078D4"));

    /// <summary>
    /// Property for <see cref="Watermark"/>.
    /// </summary>
    public static readonly StyledProperty<string?> WatermarkProperty =
        AvaloniaProperty.Register<RichTextBox, string?>(nameof(Watermark));

    /// <summary>
    /// Property for <see cref="IsFormattingEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsFormattingEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsFormattingEnabled), true);

    /// <summary>
    /// Property for <see cref="IsDocumentEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsDocumentEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsDocumentEnabled), false);

    /// <summary>
    /// Property for <see cref="IsDragDropEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsDragDropEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsDragDropEnabled), true);

    /// <summary>
    /// Property for <see cref="EditMode"/>.
    /// </summary>
    public static readonly StyledProperty<RichTextEditMode> EditModeProperty =
        AvaloniaProperty.Register<RichTextBox, RichTextEditMode>(nameof(EditMode), RichTextEditMode.EditOnFocus);

    /// <summary>
    /// Property for <see cref="IsRichClipboardEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsRichClipboardEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsRichClipboardEnabled), true);

    /// <summary>
    /// Property for <see cref="IsImagePasteEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsImagePasteEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsImagePasteEnabled), true);

    /// <summary>
    /// Property for <see cref="IsImageDropEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsImageDropEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsImageDropEnabled), true);

    /// <summary>
    /// Property for <see cref="MaxDroppedTextFileBytes"/>.
    /// </summary>
    public static readonly StyledProperty<long> MaxDroppedTextFileBytesProperty =
        AvaloniaProperty.Register<RichTextBox, long>(nameof(MaxDroppedTextFileBytes), 1_048_576);

    /// <summary>
    /// Property for <see cref="SelectionStart"/>.
    /// </summary>
    public static readonly StyledProperty<int> SelectionStartProperty =
        AvaloniaProperty.Register<RichTextBox, int>(nameof(SelectionStart));

    /// <summary>
    /// Property for <see cref="SelectionEnd"/>.
    /// </summary>
    public static readonly StyledProperty<int> SelectionEndProperty =
        AvaloniaProperty.Register<RichTextBox, int>(nameof(SelectionEnd));

    /// <summary>
    /// Property for <see cref="CaretIndex"/>.
    /// </summary>
    public static readonly StyledProperty<int> CaretIndexProperty =
        AvaloniaProperty.Register<RichTextBox, int>(nameof(CaretIndex));

    /// <summary>
    /// Defines the <see cref="TextChanged"/> event.
    /// </summary>
    public static readonly RoutedEvent<TextChangedEventArgs> TextChangedEvent =
        RoutedEvent.Register<RichTextBox, TextChangedEventArgs>(nameof(TextChanged), RoutingStrategies.Bubble);

    /// <summary>
    /// Defines the <see cref="SelectionChanged"/> event.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> SelectionChangedEvent =
        RoutedEvent.Register<RichTextBox, RoutedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);

    /// <summary>
    /// Defines the <see cref="FormattingApplied"/> event.
    /// </summary>
    public static readonly RoutedEvent<FormattingEventArgs> FormattingAppliedEvent =
        RoutedEvent.Register<RichTextBox, FormattingEventArgs>(nameof(FormattingApplied), RoutingStrategies.Bubble);

    private readonly Stack<RichTextHistoryEntry> _undoStack = new();
    private readonly Stack<RichTextHistoryEntry> _redoStack = new();
    private readonly IRichTextClipboardAdapter _defaultClipboardAdapter = new RichTextMemoryClipboardAdapter();
    private global::Avalonia.Controls.TextBox? _editingTextBox;
    private FormattedTextPresenter? _formattedPresenter;
    private ContextMenu? _contextMenu;
    private bool _isUpdating;

    /// <summary>
    /// Initializes a new instance of the <see cref="RichTextBox"/> class.
    /// </summary>
    public RichTextBox()
    {
        Document = new FlowDocument();
        Selection = new TextSelection(Document);
        CopyCommand = new RichTextCommand(_ => CopyCore(), _ => CanCopy);
        CutCommand = new RichTextCommand(_ => CutCore(), _ => CanCut);
        PasteCommand = new RichTextCommand(_ => PasteCore(), _ => CanPaste);
        UndoCommand = new RichTextCommand(_ => UndoCore(), _ => CanUndo);
        RedoCommand = new RichTextCommand(_ => RedoCore(), _ => CanRedo);
        SelectAllCommand = new RichTextCommand(_ => SelectAllCore(), _ => Document.Length > 0);
        ToggleBoldCommand = new RichTextCommand(_ => ApplyFormattingToSelection(TextFormatType.Bold), _ => CanApplyFormatting);
        ToggleItalicCommand = new RichTextCommand(_ => ApplyFormattingToSelection(TextFormatType.Italic), _ => CanApplyFormatting);
        ToggleUnderlineCommand = new RichTextCommand(_ => ApplyFormattingToSelection(TextFormatType.Underline), _ => CanApplyFormatting);
        ToggleStrikethroughCommand = new RichTextCommand(_ => ApplyFormattingToSelection(TextFormatType.Strikethrough), _ => CanApplyFormatting);
        ClearFormattingCommand = new RichTextCommand(_ => ClearFormattingCore(), _ => !IsReadOnlyInternal && IsFormattingEnabled && HasAnyFormatting());
        SetFontFamilyCommand = new RichTextCommand(parameter => SetSelectionFontFamily(parameter?.ToString() ?? string.Empty), parameter => CanApplyFormatting && !string.IsNullOrWhiteSpace(parameter?.ToString()));
        SetFontSizeCommand = new RichTextCommand(parameter => SetSelectionFontSize(Convert.ToDouble(parameter, CultureInfo.InvariantCulture)), parameter => CanApplyFormatting && parameter is not null);
        SetForegroundCommand = new RichTextCommand(parameter => SetSelectionForeground((Color)parameter!), parameter => CanApplyFormatting && parameter is Color);
        SetHighlightCommand = new RichTextCommand(parameter => SetSelectionHighlight((Color)parameter!), parameter => CanApplyFormatting && parameter is Color);
        Focusable = true;

        AddHandler(DragDrop.DragOverEvent, OnDragOver, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
        AddHandler(DragDrop.DropEvent, OnDrop, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
        DragDrop.SetAllowDrop(this, true);

        Text = string.Empty;
        Document.SetText(Text);
    }

    /// <summary>
    /// Occurs when text changes.
    /// </summary>
    public event EventHandler<TextChangedEventArgs>? TextChanged
    {
        add => AddHandler(TextChangedEvent, value);
        remove => RemoveHandler(TextChangedEvent, value);
    }

    /// <summary>
    /// Occurs when selection changes.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? SelectionChanged
    {
        add => AddHandler(SelectionChangedEvent, value);
        remove => RemoveHandler(SelectionChangedEvent, value);
    }

    /// <summary>
    /// Occurs when formatting is applied.
    /// </summary>
    public event EventHandler<FormattingEventArgs>? FormattingApplied
    {
        add => AddHandler(FormattingAppliedEvent, value);
        remove => RemoveHandler(FormattingAppliedEvent, value);
    }

    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Enter inserts a new line.
    /// </summary>
    public bool AcceptsReturn
    {
        get => GetValue(AcceptsReturnProperty);
        set => SetValue(AcceptsReturnProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Tab inserts a tab character.
    /// </summary>
    public bool AcceptsTab
    {
        get => GetValue(AcceptsTabProperty);
        set => SetValue(AcceptsTabProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control is read-only.
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether text selection-only mode is enabled.
    /// </summary>
    public bool IsTextSelectionEnabled
    {
        get => GetValue(IsTextSelectionEnabledProperty);
        set => SetValue(IsTextSelectionEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the wrapping mode.
    /// </summary>
    public TextWrapping TextWrapping
    {
        get => GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal scroll bar visibility.
    /// </summary>
    public ScrollBarVisibility HorizontalScrollBarVisibility
    {
        get => GetValue(HorizontalScrollBarVisibilityProperty);
        set => SetValue(HorizontalScrollBarVisibilityProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical scroll bar visibility.
    /// </summary>
    public ScrollBarVisibility VerticalScrollBarVisibility
    {
        get => GetValue(VerticalScrollBarVisibilityProperty);
        set => SetValue(VerticalScrollBarVisibilityProperty, value);
    }

    /// <summary>
    /// Gets or sets the caret brush.
    /// </summary>
    public IBrush? CaretBrush
    {
        get => GetValue(CaretBrushProperty);
        set => SetValue(CaretBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the selection brush.
    /// </summary>
    public IBrush? SelectionBrush
    {
        get => GetValue(SelectionBrushProperty);
        set => SetValue(SelectionBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the watermark text.
    /// </summary>
    public string? Watermark
    {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether formatting is enabled.
    /// </summary>
    public bool IsFormattingEnabled
    {
        get => GetValue(IsFormattingEnabledProperty);
        set => SetValue(IsFormattingEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether content elements in the document are interactive.
    /// </summary>
    public bool IsDocumentEnabled
    {
        get => GetValue(IsDocumentEnabledProperty);
        set => SetValue(IsDocumentEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether drag and drop insertion is enabled.
    /// </summary>
    public bool IsDragDropEnabled
    {
        get => GetValue(IsDragDropEnabledProperty);
        set => SetValue(IsDragDropEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the edit/display mode.
    /// </summary>
    public RichTextEditMode EditMode
    {
        get => GetValue(EditModeProperty);
        set => SetValue(EditModeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether HTML clipboard payloads are copied and pasted.
    /// </summary>
    public bool IsRichClipboardEnabled
    {
        get => GetValue(IsRichClipboardEnabledProperty);
        set => SetValue(IsRichClipboardEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether image paste is enabled.
    /// </summary>
    public bool IsImagePasteEnabled
    {
        get => GetValue(IsImagePasteEnabledProperty);
        set => SetValue(IsImagePasteEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether image drop is enabled.
    /// </summary>
    public bool IsImageDropEnabled
    {
        get => GetValue(IsImageDropEnabledProperty);
        set => SetValue(IsImageDropEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum UTF-8 text file size accepted by runtime file drops.
    /// </summary>
    public long MaxDroppedTextFileBytes
    {
        get => GetValue(MaxDroppedTextFileBytesProperty);
        set => SetValue(MaxDroppedTextFileBytesProperty, Math.Max(0, value));
    }

    /// <summary>
    /// Gets or sets the clipboard adapter used by copy, cut, and paste operations.
    /// </summary>
    public IRichTextClipboardAdapter? ClipboardAdapter { get; set; }

    /// <summary>
    /// Gets a value indicating whether the current selection can be copied.
    /// </summary>
    public bool CanCopy => HasSelection;

    /// <summary>
    /// Gets a value indicating whether the current selection can be cut.
    /// </summary>
    public bool CanCut => HasSelection && !IsReadOnlyInternal;

    /// <summary>
    /// Gets a value indicating whether the clipboard can be pasted into the current document.
    /// </summary>
    public bool CanPaste
    {
        get
        {
            var clipboard = GetClipboardAdapter();
            return !IsReadOnlyInternal &&
                   ((IsRichClipboardEnabled && clipboard.ContainsHtml) ||
                    clipboard.ContainsPlainText ||
                    (IsImagePasteEnabled && clipboard.ContainsImage && RichTextHelpers.IsSupportedImageSource(clipboard.ImageSource)));
        }
    }

    /// <summary>
    /// Gets a value indicating whether undo history is available.
    /// </summary>
    public bool CanUndo => !IsReadOnlyInternal && _undoStack.Count > 0;

    /// <summary>
    /// Gets a value indicating whether redo history is available.
    /// </summary>
    public bool CanRedo => !IsReadOnlyInternal && _redoStack.Count > 0;

    /// <summary>
    /// Gets a value indicating whether formatting can be applied to the current selection.
    /// </summary>
    public bool CanApplyFormatting => IsFormattingEnabled && !IsReadOnlyInternal && HasSelection;

    /// <summary>
    /// Gets the copy command.
    /// </summary>
    public ICommand CopyCommand { get; }

    /// <summary>
    /// Gets the cut command.
    /// </summary>
    public ICommand CutCommand { get; }

    /// <summary>
    /// Gets the paste command.
    /// </summary>
    public ICommand PasteCommand { get; }

    /// <summary>
    /// Gets the undo command.
    /// </summary>
    public ICommand UndoCommand { get; }

    /// <summary>
    /// Gets the redo command.
    /// </summary>
    public ICommand RedoCommand { get; }

    /// <summary>
    /// Gets the select all command.
    /// </summary>
    public ICommand SelectAllCommand { get; }

    /// <summary>
    /// Gets the bold formatting command.
    /// </summary>
    public ICommand ToggleBoldCommand { get; }

    /// <summary>
    /// Gets the italic formatting command.
    /// </summary>
    public ICommand ToggleItalicCommand { get; }

    /// <summary>
    /// Gets the underline formatting command.
    /// </summary>
    public ICommand ToggleUnderlineCommand { get; }

    /// <summary>
    /// Gets the strikethrough formatting command.
    /// </summary>
    public ICommand ToggleStrikethroughCommand { get; }

    /// <summary>
    /// Gets the clear formatting command.
    /// </summary>
    public ICommand ClearFormattingCommand { get; }

    /// <summary>
    /// Gets the font family formatting command.
    /// </summary>
    public ICommand SetFontFamilyCommand { get; }

    /// <summary>
    /// Gets the font size formatting command.
    /// </summary>
    public ICommand SetFontSizeCommand { get; }

    /// <summary>
    /// Gets the foreground color formatting command.
    /// </summary>
    public ICommand SetForegroundCommand { get; }

    /// <summary>
    /// Gets the highlight color formatting command.
    /// </summary>
    public ICommand SetHighlightCommand { get; }

    /// <summary>
    /// Gets or sets the selection start index.
    /// </summary>
    public int SelectionStart
    {
        get => GetValue(SelectionStartProperty);
        set => SetValue(SelectionStartProperty, value);
    }

    /// <summary>
    /// Gets or sets the selection end index.
    /// </summary>
    public int SelectionEnd
    {
        get => GetValue(SelectionEndProperty);
        set => SetValue(SelectionEndProperty, value);
    }

    /// <summary>
    /// Gets or sets the caret index.
    /// </summary>
    public int CaretIndex
    {
        get => GetValue(CaretIndexProperty);
        set => SetValue(CaretIndexProperty, value);
    }

    /// <summary>
    /// Gets the document model.
    /// </summary>
    public FlowDocument Document { get; }

    /// <summary>
    /// Gets the text selection.
    /// </summary>
    public TextSelection Selection { get; }

    /// <summary>
    /// Gets or sets the current caret position.
    /// </summary>
    public TextPointer CaretPosition
    {
        get => Document.GetTextPointer(Math.Clamp(CaretIndex, 0, Document.Length));
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (!ReferenceEquals(value.Document, Document))
            {
                throw new ArgumentException("Pointer does not belong to this document.", nameof(value));
            }

            CaretIndex = value.Offset;
            SelectionStart = value.Offset;
            SelectionEnd = value.Offset;
            ApplySelectionToTextBox();
            SynchronizeSelectionFromIndexes(raiseEvent: true);
        }
    }

    /// <summary>
    /// Gets a value indicating whether text is selected.
    /// </summary>
    public bool HasSelection => SelectionStart != SelectionEnd;

    /// <summary>
    /// Gets the current selection length.
    /// </summary>
    public int SelectionLength
    {
        get
        {
            if (string.IsNullOrEmpty(Text))
            {
                return 0;
            }

            return Math.Abs(SelectionEnd - SelectionStart);
        }
    }

    /// <summary>
    /// Gets the selected text.
    /// </summary>
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
            return Document.GetTextRange(Document.GetTextPointer(start), Document.GetTextPointer(start + length));
        }
    }

    /// <summary>
    /// Gets the selected content as an HTML fragment preserving inline formatting where possible.
    /// </summary>
    public string SelectedHtml
    {
        get
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
                if (!segment.HasRenderableText || segment.EndIndex <= start || segment.StartIndex >= end)
                {
                    continue;
                }

                var segmentStart = Math.Max(start, segment.StartIndex);
                var segmentEnd = Math.Min(end, segment.EndIndex);
                var segmentLength = segmentEnd - segmentStart;
                if (segmentLength <= 0)
                {
                    continue;
                }

                var localStart = segmentStart - segment.StartIndex;
                var selectedText = segment.Text.Substring(localStart, segmentLength);
                builder.Append(RichTextHelpers.FormatSegmentAsHtml(segment, selectedText));
            }

            return builder.ToString();
        }
    }

    private bool IsReadOnlyInternal => IsReadOnly || IsTextSelectionEnabled || EditMode == RichTextEditMode.Display;

    /// <summary>
    /// Returns the text position nearest the provided point.
    /// </summary>
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

        if (TryGetRenderedDocumentOffsetFromPoint(point, out var renderedOffset))
        {
            return Document.GetTextPointer(renderedOffset);
        }

        return Document.GetTextPointer(EstimateDocumentOffsetFromPoint(point, textBounds));
    }

    /// <summary>
    /// Determines whether the document should be serialized.
    /// </summary>
    /// <returns><see langword="true"/> when the current document has content.</returns>
    public bool ShouldSerializeDocument() => !string.IsNullOrWhiteSpace(Document.GetPlainText());

    /// <summary>
    /// Gets the rendered plain text for the current document.
    /// </summary>
    /// <returns>The plain text projection.</returns>
    public string GetPlainText() => Document.GetPlainText();

    /// <summary>
    /// Gets the current HTML/markup representation for the document.
    /// </summary>
    /// <returns>The HTML/markup representation.</returns>
    public string GetHtml() => Document.GetText();

    /// <summary>
    /// Applies bold formatting to the selection.
    /// </summary>
    public void ToggleBold() => ToggleBoldCommand.Execute(null);

    /// <summary>
    /// Applies italic formatting to the selection.
    /// </summary>
    public void ToggleItalic() => ToggleItalicCommand.Execute(null);

    /// <summary>
    /// Applies underline formatting to the selection.
    /// </summary>
    public void ToggleUnderline() => ToggleUnderlineCommand.Execute(null);

    /// <summary>
    /// Applies strikethrough formatting to the selection.
    /// </summary>
    public void ToggleStrikethrough() => ToggleStrikethroughCommand.Execute(null);

    /// <summary>
    /// Applies a font family to the selected content.
    /// </summary>
    /// <param name="fontFamily">The font family name.</param>
    public void SetSelectionFontFamily(string fontFamily)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fontFamily);
        ApplySpanStyleToSelection($"font-family:{fontFamily};");
    }

    /// <summary>
    /// Applies a font size to the selected content.
    /// </summary>
    /// <param name="fontSize">The font size in pixels.</param>
    public void SetSelectionFontSize(double fontSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(fontSize);

        ApplySpanStyleToSelection($"font-size:{fontSize}px;");
    }

    /// <summary>
    /// Applies a foreground color to the selected content.
    /// </summary>
    /// <param name="color">The text color.</param>
    public void SetSelectionForeground(in Color color) => ApplySpanStyleToSelection($"color:{color.ToString()};");

    /// <summary>
    /// Applies a highlight color to the selected content.
    /// </summary>
    /// <param name="color">The highlight color.</param>
    public void SetSelectionHighlight(in Color color) => ApplySpanStyleToSelection($"background-color:{color.ToString()};");

    /// <summary>
    /// Selects all text.
    /// </summary>
    public void SelectAll() => SelectAllCommand.Execute(null);

    /// <summary>
    /// Selects a range by offset and length.
    /// </summary>
    /// <param name="startIndex">The start index.</param>
    /// <param name="length">The length.</param>
    public void Select(int startIndex, int length)
    {
        var max = Document.Length;
        var start = Math.Clamp(startIndex, 0, max);
        var end = Math.Clamp(start + length, 0, max);
        SelectionStart = Math.Min(start, end);
        SelectionEnd = Math.Max(start, end);
        CaretIndex = SelectionEnd;
        ApplySelectionToTextBox();
        SynchronizeSelectionFromIndexes(raiseEvent: true);
        NotifyCommandStateChanged();
    }

    /// <summary>
    /// Selects a range using flow document text pointers.
    /// </summary>
    /// <param name="start">The selection start.</param>
    /// <param name="end">The selection end.</param>
    public void Select(TextPointer start, TextPointer end)
    {
        ArgumentNullException.ThrowIfNull(start);
        ArgumentNullException.ThrowIfNull(end);
        if (!ReferenceEquals(start.Document, Document) || !ReferenceEquals(end.Document, Document))
        {
            throw new InvalidOperationException("Selection pointers must belong to this document.");
        }

        var min = Math.Min(start.Offset, end.Offset);
        var max = Math.Max(start.Offset, end.Offset);
        SelectionStart = min;
        SelectionEnd = max;
        CaretIndex = max;
        ApplySelectionToTextBox();
        SynchronizeSelectionFromIndexes(raiseEvent: true);
        NotifyCommandStateChanged();
    }

    /// <summary>
    /// Clears all text.
    /// </summary>
    public void Clear()
    {
        if (IsReadOnlyInternal)
        {
            return;
        }

        var before = CaptureHistory();
        SetHtmlCore(string.Empty, resetUndo: false);
        CommitHistory(before);
    }

    /// <summary>
    /// Clears all formatting.
    /// </summary>
    public void ClearFormatting() => ClearFormattingCommand.Execute(null);

    /// <summary>
    /// Appends plain text to the end of the document.
    /// </summary>
    /// <param name="text">The text to append.</param>
    public void AppendText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var encoded = HtmlClipboardUtilities.EncodePlainText(text);
        Document.AppendText(encoded);
        SyncTextFromDocument();
    }

    /// <summary>
    /// Appends HTML content to the end of the document.
    /// </summary>
    /// <param name="html">The html fragment to append.</param>
    public void AppendHtml(string html)
    {
        ArgumentNullException.ThrowIfNull(html);
        Document.AppendText(html);
        SyncTextFromDocument();
    }

    /// <summary>
    /// Replaces the current content with HTML.
    /// </summary>
    /// <param name="html">The html content.</param>
    public void SetHtml(string? html) => SetHtmlCore(html, resetUndo: true);

    /// <summary>
    /// Replaces the current content with plain text.
    /// </summary>
    /// <param name="text">The plain text content.</param>
    public void SetPlainText(string? text) => SetHtml(HtmlClipboardUtilities.EncodePlainText(text));

    /// <summary>
    /// Replaces the current content with markdown converted to HTML.
    /// </summary>
    /// <param name="markdown">The markdown content.</param>
    public void SetMarkdown(string? markdown) => SetHtml(MarkdownUtilities.ToHtml(markdown));

    /// <summary>
    /// Replaces the current selection with plain text.
    /// </summary>
    /// <param name="text">The replacement text.</param>
    public void ReplaceSelection(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        ReplaceSelectionWithHtml(HtmlClipboardUtilities.EncodePlainText(text));
    }

    /// <summary>
    /// Replaces the current selection with HTML.
    /// </summary>
    /// <param name="html">The replacement html content.</param>
    public void ReplaceSelectionWithHtml(string html)
    {
        ArgumentNullException.ThrowIfNull(html);

        if (IsReadOnlyInternal)
        {
            return;
        }

        var before = CaptureHistory();
        var start = Math.Min(SelectionStart, SelectionEnd);
        var length = Math.Abs(SelectionEnd - SelectionStart);
        Document.Replace(start, length, html);
        SyncTextFromDocument();

        var insertedLength = HtmlTextProjection.Create(html).Length;
        var newCaret = Math.Clamp(start + insertedLength, 0, Document.Length);
        SelectionStart = newCaret;
        SelectionEnd = newCaret;
        CaretIndex = newCaret;
        ApplySelectionToTextBox();
        SynchronizeSelectionFromIndexes(raiseEvent: true);
        CommitHistory(before);
    }

    /// <summary>
    /// Copies the selection to the clipboard.
    /// </summary>
    public void Copy() => CopyCommand.Execute(null);

    /// <summary>
    /// Cuts the selection to the clipboard.
    /// </summary>
    public void Cut() => CutCommand.Execute(null);

    /// <summary>
    /// Pastes from the clipboard.
    /// </summary>
    public void Paste() => PasteCommand.Execute(null);

    /// <summary>
    /// Ensures that the RichTextBox context menu exists and returns it.
    /// </summary>
    /// <returns>The context menu used by the control.</returns>
    public ContextMenu EnsureContextMenu()
    {
        _contextMenu ??= CreateContextMenu();
        UpdateContextMenuState();
        return _contextMenu;
    }

    /// <summary>
    /// Refreshes command enablement for the context menu items.
    /// </summary>
    public void RefreshContextMenuState()
    {
        _contextMenu ??= CreateContextMenu();
        UpdateContextMenuState();
    }

    /// <summary>
    /// Inserts a dropped text payload when drag/drop and edit state allow it.
    /// </summary>
    /// <param name="text">The dropped text or HTML payload.</param>
    /// <returns><see langword="true"/> when content was inserted.</returns>
    public bool TryDropText(string? text)
    {
        if (!IsDragDropEnabled || IsReadOnlyInternal)
        {
            return false;
        }

        var textPayload = RichTextHelpers.NormalizeClipboardText(text);
        if (string.IsNullOrWhiteSpace(textPayload))
        {
            return false;
        }

        if (RichTextHelpers.LooksLikeHtml(textPayload))
        {
            ReplaceSelectionWithHtml(textPayload);
        }
        else
        {
            ReplaceSelection(textPayload);
        }

        return true;
    }

    /// <summary>
    /// Inserts a dropped image source when drag/drop, image policy, and edit state allow it.
    /// </summary>
    /// <param name="imageSource">The image file URI/path or data URI.</param>
    /// <returns><see langword="true"/> when an image was inserted.</returns>
    public bool TryDropImage(string? imageSource)
    {
        if (imageSource is null)
        {
            return false;
        }

        return TryInsertImage(imageSource, requireDragDrop: true);
    }

    /// <summary>
    /// Undoes the last action.
    /// </summary>
    public void Undo() => UndoCommand.Execute(null);

    /// <summary>
    /// Redoes the last undone action.
    /// </summary>
    public void Redo() => RedoCommand.Execute(null);

    /// <summary>
    /// Saves content to a stream.
    /// </summary>
    /// <param name="stream">The stream to save to.</param>
    /// <param name="encoding">The encoding to use.</param>
    public void Save(Stream stream, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        encoding ??= Encoding.UTF8;
        var bytes = encoding.GetBytes(Text ?? string.Empty);
        stream.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Saves content to a stream in the requested format.
    /// </summary>
    /// <param name="stream">The stream to save to.</param>
    /// <param name="format">The data format to save.</param>
    /// <param name="encoding">The encoding to use.</param>
    public void Save(Stream stream, RichTextDataFormat format, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        encoding ??= Encoding.UTF8;
        var content = format switch
        {
            RichTextDataFormat.PlainText => GetPlainText(),
            RichTextDataFormat.Html => GetHtml(),
            RichTextDataFormat.Markdown => GetMarkdown(),
            RichTextDataFormat.Rtf => string.Empty,
            _ => GetHtml()
        };
        var bytes = encoding.GetBytes(content);
        stream.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Gets a Markdown-compatible text representation.
    /// </summary>
    /// <returns>The Markdown-compatible representation.</returns>
    public string GetMarkdown() => GetPlainText();

    /// <summary>
    /// Loads content from a stream.
    /// </summary>
    /// <param name="stream">The stream to load from.</param>
    /// <param name="encoding">The encoding to use.</param>
    public void Load(Stream stream, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        encoding ??= Encoding.UTF8;
        using var reader = new StreamReader(stream, encoding);
        SetHtml(reader.ReadToEnd());
    }

    /// <summary>
    /// Loads content from a stream in the requested format.
    /// </summary>
    /// <param name="stream">The stream to load from.</param>
    /// <param name="format">The data format to load.</param>
    /// <param name="encoding">The encoding to use.</param>
    public void Load(Stream stream, RichTextDataFormat format, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        encoding ??= Encoding.UTF8;
        using var reader = new StreamReader(stream, encoding);
        var content = reader.ReadToEnd();
        if (format == RichTextDataFormat.PlainText)
        {
            SetPlainText(content);
        }
        else if (format == RichTextDataFormat.Markdown)
        {
            SetMarkdown(content);
        }
        else
        {
            SetHtml(content);
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (e is null)
        {
            return;
        }

        // Unsubscribe from old controls
        if (_editingTextBox is not null)
        {
            _editingTextBox.TextChanged -= OnTextBoxTextChanged;
            _editingTextBox.PropertyChanged -= OnTextBoxPropertyChanged;
            _editingTextBox.GotFocus -= OnTextBoxGotFocus;
            _editingTextBox.LostFocus -= OnTextBoxLostFocus;
        }

        // Find template parts
        _editingTextBox = e.NameScope.Find<global::Avalonia.Controls.TextBox>("PART_TextBox");
        _formattedPresenter = e.NameScope.Find<FormattedTextPresenter>("PART_FormattedPresenter");
        _contextMenu ??= CreateContextMenu();

        // Setup editing text box
        if (_editingTextBox is not null)
        {
            _editingTextBox.AcceptsReturn = AcceptsReturn;
            _editingTextBox.AcceptsTab = AcceptsTab;
            _editingTextBox.IsReadOnly = IsReadOnly || IsTextSelectionEnabled;
            _editingTextBox.TextWrapping = TextWrapping;
            ScrollViewer.SetHorizontalScrollBarVisibility(_editingTextBox, HorizontalScrollBarVisibility);
            ScrollViewer.SetVerticalScrollBarVisibility(_editingTextBox, VerticalScrollBarVisibility);
            _editingTextBox.CaretBrush = CaretBrush;
            _editingTextBox.SelectionBrush = SelectionBrush;
            _editingTextBox.Text = Text;

            _editingTextBox.TextChanged += OnTextBoxTextChanged;
            _editingTextBox.PropertyChanged += OnTextBoxPropertyChanged;
            _editingTextBox.GotFocus += OnTextBoxGotFocus;
            _editingTextBox.LostFocus += OnTextBoxLostFocus;
            _editingTextBox.ContextMenu = _contextMenu;
            DragDrop.SetAllowDrop(_editingTextBox, IsDragDropEnabled && !IsReadOnlyInternal);
        }

        if (_formattedPresenter is not null)
        {
            _formattedPresenter.TextWrapping = TextWrapping;
            _formattedPresenter.ContextMenu = _contextMenu;
            DragDrop.SetAllowDrop(_formattedPresenter, IsDragDropEnabled && !IsReadOnlyInternal);
        }

        // Initialize document
        Document.SetText(Text);
        ClampSelectionToDocument();
        UpdateFormattedPresenter();
        UpdateDisplayMode();
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change is null || _isUpdating)
        {
            return;
        }

        var readOnly = IsReadOnly || IsTextSelectionEnabled;

        if (change.Property == TextProperty)
        {
            _isUpdating = true;
            Document.SetText(Text);

            _editingTextBox?.Text = Text;

            UpdateFormattedPresenter();
            _isUpdating = false;
            ClampSelectionToDocument();

            RaiseEvent(new TextChangedEventArgs(TextChangedEvent));
        }
        else if (change.Property == AcceptsReturnProperty && _editingTextBox is not null)
        {
            _editingTextBox.AcceptsReturn = AcceptsReturn;
        }
        else if (change.Property == AcceptsTabProperty && _editingTextBox is not null)
        {
            _editingTextBox.AcceptsTab = AcceptsTab;
        }
        else if (change.Property == TextWrappingProperty && _editingTextBox is not null)
        {
            _editingTextBox.TextWrapping = TextWrapping;
            if (_formattedPresenter is not null)
            {
                _formattedPresenter.TextWrapping = TextWrapping;
            }
        }
        else if (change.Property == HorizontalScrollBarVisibilityProperty && _editingTextBox is not null)
        {
            ScrollViewer.SetHorizontalScrollBarVisibility(_editingTextBox, HorizontalScrollBarVisibility);
        }
        else if (change.Property == VerticalScrollBarVisibilityProperty && _editingTextBox is not null)
        {
            ScrollViewer.SetVerticalScrollBarVisibility(_editingTextBox, VerticalScrollBarVisibility);
        }
        else if (change.Property == CaretBrushProperty && _editingTextBox is not null)
        {
            _editingTextBox.CaretBrush = CaretBrush;
        }
        else if (change.Property == SelectionBrushProperty && _editingTextBox is not null)
        {
            _editingTextBox.SelectionBrush = SelectionBrush;
        }
        else if (change.Property == IsReadOnlyProperty && _editingTextBox is not null)
        {
            _editingTextBox.IsReadOnly = readOnly;
            DragDrop.SetAllowDrop(this, IsDragDropEnabled && !readOnly);
            if (_formattedPresenter is not null)
            {
                DragDrop.SetAllowDrop(_formattedPresenter, IsDragDropEnabled && !readOnly);
            }

            DragDrop.SetAllowDrop(_editingTextBox, IsDragDropEnabled && !readOnly);
        }
        else if (change.Property == IsTextSelectionEnabledProperty && _editingTextBox is not null)
        {
            _editingTextBox.IsReadOnly = readOnly;
            DragDrop.SetAllowDrop(this, IsDragDropEnabled && !readOnly);
            DragDrop.SetAllowDrop(_editingTextBox, IsDragDropEnabled && !readOnly);
            if (_formattedPresenter is not null)
            {
                DragDrop.SetAllowDrop(_formattedPresenter, IsDragDropEnabled && !readOnly);
            }

            UpdateDisplayMode();
        }
        else if (change.Property == SelectionStartProperty || change.Property == SelectionEndProperty)
        {
            ClampSelectionToDocument();
            ApplySelectionToTextBox();
            SynchronizeSelectionFromIndexes(raiseEvent: true);
        }
        else if (change.Property == CaretIndexProperty)
        {
            var boundedCaret = Math.Clamp(CaretIndex, 0, Document.Length);
            if (CaretIndex != boundedCaret)
            {
                CaretIndex = boundedCaret;
            }

            _editingTextBox?.CaretIndex = boundedCaret;
        }
        else if (change.Property == IsDocumentEnabledProperty && _formattedPresenter is not null)
        {
            _formattedPresenter.IsHitTestVisible = IsDocumentEnabled;
        }
        else if (change.Property == IsDragDropEnabledProperty)
        {
            DragDrop.SetAllowDrop(this, IsDragDropEnabled && !IsReadOnlyInternal);
            if (_editingTextBox is not null)
            {
                DragDrop.SetAllowDrop(_editingTextBox, IsDragDropEnabled && !IsReadOnlyInternal);
            }

            if (_formattedPresenter is not null)
            {
                DragDrop.SetAllowDrop(_formattedPresenter, IsDragDropEnabled && !IsReadOnlyInternal);
            }
        }
        else if (change.Property == EditModeProperty)
        {
            if (_editingTextBox is not null)
            {
                _editingTextBox.IsReadOnly = IsReadOnlyInternal;
                DragDrop.SetAllowDrop(_editingTextBox, IsDragDropEnabled && !IsReadOnlyInternal);
            }

            DragDrop.SetAllowDrop(this, IsDragDropEnabled && !IsReadOnlyInternal);
            if (_formattedPresenter is not null)
            {
                DragDrop.SetAllowDrop(_formattedPresenter, IsDragDropEnabled && !IsReadOnlyInternal);
            }

            UpdateDisplayMode();
            NotifyCommandStateChanged();
        }
    }

    /// <inheritdoc/>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e is null)
        {
            base.OnKeyDown(e!);
            return;
        }

        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            switch (e.Key)
            {
                case Key.C:
                    CopyCommand.Execute(null);
                    e.Handled = true;
                    return;
                case Key.X:
                    CutCommand.Execute(null);
                    e.Handled = true;
                    return;
                case Key.V:
                    PasteCommand.Execute(null);
                    e.Handled = true;
                    return;
                case Key.Z:
                    UndoCommand.Execute(null);
                    e.Handled = true;
                    return;
                case Key.Y:
                    RedoCommand.Execute(null);
                    e.Handled = true;
                    return;
                case Key.B when IsFormattingEnabled && !IsReadOnlyInternal:
                    ToggleBoldCommand.Execute(null);
                    e.Handled = true;
                    return;
                case Key.I when IsFormattingEnabled && !IsReadOnlyInternal:
                    ToggleItalicCommand.Execute(null);
                    e.Handled = true;
                    return;
                case Key.U when IsFormattingEnabled && !IsReadOnlyInternal:
                    ToggleUnderlineCommand.Execute(null);
                    e.Handled = true;
                    return;
                case Key.A:
                    SelectAllCommand.Execute(null);
                    e.Handled = true;
                    return;
                case Key.S when e.KeyModifiers.HasFlag(KeyModifiers.Shift) && IsFormattingEnabled && !IsReadOnlyInternal:
                    ToggleStrikethroughCommand.Execute(null);
                    e.Handled = true;
                    return;
            }
        }

        base.OnKeyDown(e);
    }

    /// <inheritdoc/>
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (e?.InitialPressMouseButton == MouseButton.Right)
        {
            UpdateContextMenuState();
            _contextMenu?.Open(this);
            e.Handled = true;
        }
    }

    /// <inheritdoc/>
    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);

        if (_editingTextBox is not null)
        {
            _editingTextBox.IsVisible = true;
            _editingTextBox.IsHitTestVisible = true;
            _editingTextBox.Opacity = 1;

            if (!_editingTextBox.IsFocused)
            {
                _editingTextBox.Focus();
            }
        }

        if (_formattedPresenter is not null)
        {
            _formattedPresenter.IsVisible = false;
        }
    }

    private void OnTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_isUpdating)
        {
            return;
        }

        _isUpdating = true;

        var oldText = Text;
        var newText = _editingTextBox?.Text ?? string.Empty;

        // Update document to reflect changes
        if (oldText != newText)
        {
            // For simplicity, we update the document entirely
            // A more sophisticated implementation would track incremental changes
            Document.SetText(newText);
            Text = newText;
            UpdateFormattedPresenter();
            RaiseEvent(new TextChangedEventArgs(TextChangedEvent));
        }

        _isUpdating = false;
    }

    private void OnTextBoxPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == global::Avalonia.Controls.TextBox.SelectionStartProperty ||
            e.Property == global::Avalonia.Controls.TextBox.SelectionEndProperty)
        {
            if (_editingTextBox is not null)
            {
                var max = Document.Length;
                SelectionStart = Math.Clamp(_editingTextBox.SelectionStart, 0, max);
                SelectionEnd = Math.Clamp(_editingTextBox.SelectionEnd, 0, max);
                CaretIndex = Math.Clamp(_editingTextBox.CaretIndex, 0, max);
                SynchronizeSelectionFromIndexes(raiseEvent: true);
                UpdateContextMenuState();
            }
        }
        else if (e.Property == global::Avalonia.Controls.TextBox.CaretIndexProperty)
        {
            if (_editingTextBox is not null)
            {
                CaretIndex = Math.Clamp(_editingTextBox.CaretIndex, 0, Document.Length);
            }
        }
    }

    private void OnTextBoxGotFocus(object? sender, FocusChangedEventArgs e)
    {
        UpdateDisplayMode();
        SynchronizeSelectionFromIndexes(raiseEvent: false);
    }

    private void OnTextBoxLostFocus(object? sender, FocusChangedEventArgs e)
    {
        UpdateFormattedPresenter();
        UpdateDisplayMode();
    }

    private void SelectAllCore()
    {
        SelectionStart = 0;
        SelectionEnd = Document.Length;
        CaretIndex = SelectionEnd;
        SynchronizeSelectionFromIndexes(raiseEvent: true);
        _editingTextBox?.SelectAll();
        NotifyCommandStateChanged();
    }

    private void ClearFormattingCore()
    {
        if (IsReadOnlyInternal)
        {
            return;
        }

        var before = CaptureHistory();
        Document.ClearFormatting();
        SyncTextFromDocument();
        UpdateFormattedPresenter();
        CommitHistory(before);
    }

    private void SetHtmlCore(string? html, bool resetUndo)
    {
        _isUpdating = true;
        Document.SetText(html);
        Text = html;
        if (_editingTextBox is not null)
        {
            _editingTextBox.Text = html;
            _editingTextBox.CaretIndex = _editingTextBox.Text?.Length ?? 0;
        }

        _isUpdating = false;
        ClampSelectionToDocument();
        UpdateFormattedPresenter();
        if (resetUndo)
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        NotifyCommandStateChanged();
    }

    private IRichTextClipboardAdapter GetClipboardAdapter() => ClipboardAdapter ?? _defaultClipboardAdapter;

    private RichTextHistoryEntry CaptureHistory() => new(Document.GetText(), SelectionStart, SelectionEnd, CaretIndex);

    private void CommitHistory(RichTextHistoryEntry before)
    {
        if (string.Equals(before.Html, Document.GetText(), StringComparison.Ordinal))
        {
            NotifyCommandStateChanged();
            return;
        }

        _undoStack.Push(before);
        _redoStack.Clear();
        NotifyCommandStateChanged();
    }

    private void RestoreHistory(RichTextHistoryEntry entry)
    {
        _isUpdating = true;
        Document.SetText(entry.Html);
        Text = entry.Html;
        if (_editingTextBox is not null)
        {
            _editingTextBox.Text = Text;
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

    private void CopyCore()
    {
        if (!CanCopy)
        {
            return;
        }

        var selectedText = SelectedText;
        var clipboard = GetClipboardAdapter();
        clipboard.SetPlainText(selectedText);
        if (IsRichClipboardEnabled)
        {
            var selectedHtml = SelectedHtml;
            clipboard.SetHtml(string.IsNullOrEmpty(selectedHtml) ? HtmlClipboardUtilities.EncodePlainText(selectedText) : selectedHtml);
        }

        NotifyCommandStateChanged();
    }

    private void CutCore()
    {
        if (!CanCut)
        {
            return;
        }

        CopyCore();
        ReplaceSelectionWithHtml(string.Empty);
    }

    private void PasteCore()
    {
        if (!CanPaste)
        {
            return;
        }

        var clipboard = GetClipboardAdapter();
        if (IsRichClipboardEnabled && clipboard.ContainsHtml && !string.IsNullOrEmpty(clipboard.HtmlText))
        {
            var fragment = HtmlClipboardUtilities.ExtractFragment(clipboard.HtmlText) ?? clipboard.HtmlText;
            ReplaceSelectionWithHtml(fragment ?? string.Empty);
            return;
        }

        if (IsImagePasteEnabled && clipboard.ContainsImage && TryInsertImage(clipboard.ImageSource, requireDragDrop: false))
        {
            return;
        }

        if (clipboard.ContainsPlainText)
        {
            ReplaceSelection(clipboard.PlainText ?? string.Empty);
        }
    }

    private bool TryInsertImage(string? imageSource, bool requireDragDrop)
    {
        if (IsReadOnlyInternal || (requireDragDrop && !IsDragDropEnabled))
        {
            return false;
        }

        if (requireDragDrop)
        {
            if (!IsImageDropEnabled)
            {
                return false;
            }
        }
        else if (!IsImagePasteEnabled)
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

    private void NotifyCommandStateChanged()
    {
        foreach (var command in new[]
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
            SetHighlightCommand
        })
        {
            if (command is RichTextCommand richTextCommand)
            {
                richTextCommand.RaiseCanExecuteChanged();
            }
        }
    }

    private bool HasAnyFormatting()
    {
        foreach (var segment in Document.Segments)
        {
            if (segment.IsBold ||
                segment.IsItalic ||
                segment.IsUnderline ||
                segment.IsStrikethrough ||
                segment.IsImage ||
                segment.Foreground is not null ||
                segment.Background is not null ||
                segment.FontSize.HasValue ||
                segment.FontFamily is not null ||
                segment.ParagraphAlignment.HasValue)
            {
                return true;
            }
        }

        return false;
    }

    private void ApplyFormattingToSelection(TextFormatType formatType)
    {
        if (!IsFormattingEnabled || IsReadOnlyInternal || !HasSelection)
        {
            return;
        }

        var start = Math.Min(SelectionStart, SelectionEnd);
        var length = Math.Abs(SelectionEnd - SelectionStart);
        var before = CaptureHistory();

        Document.ToggleFormatting(start, length, formatType);
        SyncTextFromDocument();
        UpdateFormattedPresenter();
        CommitHistory(before);

        RaiseEvent(new FormattingEventArgs(FormattingAppliedEvent, this, formatType, SelectedText));
    }

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

    private void UpdateFormattedPresenter()
    {
        if (_formattedPresenter is not null)
        {
            Document.Refresh();
            _formattedPresenter.Document = Document;
            _formattedPresenter.IsHitTestVisible = IsDocumentEnabled;
            _formattedPresenter.UpdateInlines();
        }
    }

    private void UpdateDisplayMode()
    {
        if (_editingTextBox is null || _formattedPresenter is null)
        {
            return;
        }

        var showEditing = EditMode switch
        {
            RichTextEditMode.Edit => true,
            RichTextEditMode.Display => false,
            _ => _editingTextBox.IsFocused || IsFocused,
        };
        _editingTextBox.IsVisible = showEditing;
        _editingTextBox.IsHitTestVisible = showEditing;
        _editingTextBox.Opacity = showEditing ? 1 : 0;

        _formattedPresenter.IsVisible = !showEditing;
        _formattedPresenter.IsHitTestVisible = !showEditing && IsDocumentEnabled;
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        e.DragEffects = CanAcceptDropData(e.DataTransfer) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private async void OnDrop(object? sender, DragEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (!CanAcceptDropData(e.DataTransfer))
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
            return;
        }

        MoveSelectionToDropPoint(e.GetPosition(this));
        var inserted = await TryInsertDropDataAsync(e.DataTransfer);
        e.DragEffects = inserted ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private bool CanAcceptDropData(IDataTransfer dataTransfer)
    {
        if (!IsDragDropEnabled || IsReadOnlyInternal)
        {
            return false;
        }

        var textPayload = RichTextHelpers.NormalizeClipboardText(dataTransfer.TryGetText());
        if (!string.IsNullOrWhiteSpace(textPayload))
        {
            return true;
        }

        var files = dataTransfer.TryGetFiles();
        if (files is null)
        {
            return false;
        }

        foreach (var file in files)
        {
            var path = GetStorageItemPath(file);
            if (RichTextHelpers.IsSupportedImagePath(path))
            {
                return IsImageDropEnabled;
            }

            if (file is IStorageFile && RichTextHelpers.IsSupportedTextFilePath(path))
            {
                return true;
            }
        }

        return false;
    }

    private async Task<bool> TryInsertDropDataAsync(IDataTransfer dataTransfer)
    {
        var files = dataTransfer.TryGetFiles();
        if (files is { Length: > 0 })
        {
            var inserted = false;
            foreach (var file in files)
            {
                if (file is IStorageFile storageFile)
                {
                    if (await TryDropStorageFileAsync(storageFile).ConfigureAwait(true))
                    {
                        inserted = true;
                    }
                }
                else if (RichTextHelpers.IsSupportedImagePath(GetStorageItemPath(file)) && TryDropImage(file.Path.AbsoluteUri))
                {
                    inserted = true;
                }
            }

            return inserted;
        }

        return TryDropText(dataTransfer.TryGetText());
    }

    private async Task<bool> TryDropStorageFileAsync(IStorageFile file)
    {
        var path = GetStorageItemPath(file);
        if (RichTextHelpers.IsSupportedImagePath(path))
        {
            var imageSource = await TryCreateImageSourceAsync(file).ConfigureAwait(true);
            return !string.IsNullOrWhiteSpace(imageSource) && TryDropImage(imageSource);
        }

        if (!RichTextHelpers.IsSupportedTextFilePath(path))
        {
            return false;
        }

        var properties = await file.GetBasicPropertiesAsync().ConfigureAwait(true);
        if (properties.Size is { } size && size > (ulong)MaxDroppedTextFileBytes)
        {
            return false;
        }

        await using var readStream = await file.OpenReadAsync().ConfigureAwait(true);
        if (readStream.CanSeek && readStream.Length > MaxDroppedTextFileBytes)
        {
            return false;
        }

        using var reader = new StreamReader(readStream, Encoding.UTF8, true);
        var text = await reader.ReadToEndAsync().ConfigureAwait(true);
        if (Encoding.UTF8.GetByteCount(text) > MaxDroppedTextFileBytes)
        {
            return false;
        }

        return TryDropText(text);
    }

    private async Task<string?> TryCreateImageSourceAsync(IStorageFile file)
    {
        var localPath = file.TryGetLocalPath();
        if (RichTextHelpers.IsSupportedImagePath(localPath))
        {
            return new Uri(localPath!, UriKind.Absolute).AbsoluteUri;
        }

        var path = GetStorageItemPath(file);
        if (!RichTextHelpers.IsSupportedImagePath(path))
        {
            return null;
        }

        if (file.Path.IsFile || file.Path.Scheme is "http" or "https")
        {
            return file.Path.AbsoluteUri;
        }

        await using var stream = await file.OpenReadAsync().ConfigureAwait(true);
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer).ConfigureAwait(true);
        var bytes = buffer.ToArray();
        if (bytes.Length == 0)
        {
            return null;
        }

        var mimeType = RichTextHelpers.GetImageMimeType(path);
        return $"data:{mimeType};base64,{Convert.ToBase64String(bytes)}";
    }

    private void MoveSelectionToDropPoint(Point point)
    {
        var position = GetPositionFromPoint(point, snapToText: true);
        if (position is null)
        {
            return;
        }

        Select(position.Offset, 0);
    }

    private bool TryGetRenderedDocumentOffsetFromPoint(Point point, out int offset)
    {
        offset = 0;

        if (_editingTextBox is not null)
        {
            var textPresenter = _editingTextBox.GetVisualDescendants().OfType<TextPresenter>().FirstOrDefault();
            if (TryHitTestTextLayout(textPresenter, point, out offset))
            {
                return true;
            }
        }

        return TryHitTestTextLayout(_formattedPresenter, point, out offset);
    }

    private bool TryHitTestTextLayout(Visual? visual, Point point, out int offset)
    {
        offset = 0;
        var textLayout = visual switch
        {
            TextPresenter presenter => presenter.TextLayout,
            FormattedTextPresenter presenter => presenter.TextLayout,
            _ => null,
        };

        if (visual is null || textLayout is null)
        {
            return false;
        }

        var translatedPoint = this.TranslatePoint(point, visual);
        if (!translatedPoint.HasValue)
        {
            return false;
        }

        var hitTest = textLayout.HitTestPoint(translatedPoint.Value);
        offset = Math.Clamp(hitTest.TextPosition, 0, Document.Length);
        return hitTest.IsInside || new Rect(visual.Bounds.Size).Contains(translatedPoint.Value);
    }

    private Rect GetTextHitTestBounds()
    {
        if (_editingTextBox is not null)
        {
            return _editingTextBox.Bounds;
        }

        if (_formattedPresenter is not null)
        {
            return _formattedPresenter.Bounds;
        }

        var width = Bounds.Width > 0 ? Bounds.Width : Math.Max(Document.Length * GetEstimatedCharacterWidth(), GetEstimatedCharacterWidth());
        var height = Bounds.Height > 0 ? Bounds.Height : GetEstimatedLineHeight();
        return new Rect(0, 0, width, height);
    }

    private int EstimateDocumentOffsetFromPoint(Point point, Rect bounds)
    {
        var text = Document.GetPlainText();
        if (text.Length == 0)
        {
            return 0;
        }

        var characterWidth = GetEstimatedCharacterWidth();
        var lineHeight = GetEstimatedLineHeight();
        var relativeX = Math.Max(0, point.X - bounds.X);
        var relativeY = Math.Max(0, point.Y - bounds.Y);
        var targetLine = Math.Max(0, (int)Math.Floor(relativeY / lineHeight));
        var targetColumn = Math.Max(0, (int)Math.Round(relativeX / characterWidth, MidpointRounding.AwayFromZero));
        var currentLine = 0;
        var currentColumn = 0;

        for (var offset = 0; offset < text.Length; offset++)
        {
            if (currentLine == targetLine && currentColumn >= targetColumn)
            {
                return Math.Clamp(offset, 0, Document.Length);
            }

            if (text[offset] == '\n')
            {
                if (currentLine == targetLine)
                {
                    return offset;
                }

                currentLine++;
                currentColumn = 0;
                continue;
            }

            currentColumn++;
        }

        return Document.Length;
    }

    private double GetEstimatedCharacterWidth() => Math.Max(1, FontSize > 0 ? FontSize * 0.6 : 8.4);

    private double GetEstimatedLineHeight() => Math.Max(1, FontSize > 0 ? FontSize * 1.4 : 19.6);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Kept as an instance helper to satisfy StyleCop member ordering for this control.")]
    private string? GetStorageItemPath(IStorageItem file)
    {
        var localPath = file.TryGetLocalPath();
        if (!string.IsNullOrWhiteSpace(localPath))
        {
            return localPath;
        }

        return file.Path.IsAbsoluteUri ? file.Path.AbsolutePath : file.Path.ToString();
    }

    private ContextMenu CreateContextMenu()
    {
        _contextMenu = new ContextMenu();
        _contextMenu.Opened += (_, _) => UpdateContextMenuState();

        var cutItem = new global::Avalonia.Controls.MenuItem { Header = "Cut", InputGesture = new KeyGesture(Key.X, KeyModifiers.Control), Command = CutCommand };

        var copyItem = new global::Avalonia.Controls.MenuItem { Header = "Copy", InputGesture = new KeyGesture(Key.C, KeyModifiers.Control), Command = CopyCommand };

        var pasteItem = new global::Avalonia.Controls.MenuItem { Header = "Paste", InputGesture = new KeyGesture(Key.V, KeyModifiers.Control), Command = PasteCommand };

        var selectAllItem = new global::Avalonia.Controls.MenuItem { Header = "Select All", InputGesture = new KeyGesture(Key.A, KeyModifiers.Control), Command = SelectAllCommand };

        var undoItem = new global::Avalonia.Controls.MenuItem { Header = "Undo", InputGesture = new KeyGesture(Key.Z, KeyModifiers.Control), Command = UndoCommand };

        var redoItem = new global::Avalonia.Controls.MenuItem { Header = "Redo", InputGesture = new KeyGesture(Key.Y, KeyModifiers.Control), Command = RedoCommand };

        var boldItem = new global::Avalonia.Controls.MenuItem { Header = "Bold", InputGesture = new KeyGesture(Key.B, KeyModifiers.Control), Command = ToggleBoldCommand };

        var italicItem = new global::Avalonia.Controls.MenuItem { Header = "Italic", InputGesture = new KeyGesture(Key.I, KeyModifiers.Control), Command = ToggleItalicCommand };

        var underlineItem = new global::Avalonia.Controls.MenuItem { Header = "Underline", InputGesture = new KeyGesture(Key.U, KeyModifiers.Control), Command = ToggleUnderlineCommand };

        var strikethroughItem = new global::Avalonia.Controls.MenuItem { Header = "Strikethrough", Command = ToggleStrikethroughCommand };

        var fontItem = new global::Avalonia.Controls.MenuItem { Header = "Font" };
        var fontConsolas = new global::Avalonia.Controls.MenuItem { Header = "Consolas", Command = SetFontFamilyCommand, CommandParameter = "Consolas" };
        var fontSegoe = new global::Avalonia.Controls.MenuItem { Header = "Segoe UI", Command = SetFontFamilyCommand, CommandParameter = "Segoe UI" };
        var fontTimes = new global::Avalonia.Controls.MenuItem { Header = "Times New Roman", Command = SetFontFamilyCommand, CommandParameter = "Times New Roman" };
        fontItem.ItemsSource = new object[] { fontConsolas, fontSegoe, fontTimes };

        var fontSizeItem = new global::Avalonia.Controls.MenuItem { Header = "Font Size" };
        var size12 = new global::Avalonia.Controls.MenuItem { Header = "12", Command = SetFontSizeCommand, CommandParameter = 12d };
        var size16 = new global::Avalonia.Controls.MenuItem { Header = "16", Command = SetFontSizeCommand, CommandParameter = 16d };
        var size20 = new global::Avalonia.Controls.MenuItem { Header = "20", Command = SetFontSizeCommand, CommandParameter = 20d };
        fontSizeItem.ItemsSource = new object[] { size12, size16, size20 };

        var foregroundItem = new global::Avalonia.Controls.MenuItem { Header = "Foreground" };
        var fgWhite = new global::Avalonia.Controls.MenuItem { Header = "White", Command = SetForegroundCommand, CommandParameter = Colors.White };
        var fgBlue = new global::Avalonia.Controls.MenuItem { Header = "DeepSkyBlue", Command = SetForegroundCommand, CommandParameter = Colors.DeepSkyBlue };
        var fgOrange = new global::Avalonia.Controls.MenuItem { Header = "Orange", Command = SetForegroundCommand, CommandParameter = Colors.Orange };
        foregroundItem.ItemsSource = new object[] { fgWhite, fgBlue, fgOrange };

        var highlightItem = new global::Avalonia.Controls.MenuItem { Header = "Highlight" };
        var hlTransparent = new global::Avalonia.Controls.MenuItem { Header = "Transparent", Command = SetHighlightCommand, CommandParameter = Colors.Transparent };
        var hlYellow = new global::Avalonia.Controls.MenuItem { Header = "Yellow", Command = SetHighlightCommand, CommandParameter = Colors.Yellow };
        var hlGreen = new global::Avalonia.Controls.MenuItem { Header = "LightGreen", Command = SetHighlightCommand, CommandParameter = Colors.LightGreen };
        highlightItem.ItemsSource = new object[] { hlTransparent, hlYellow, hlGreen };

        var clearFormattingItem = new global::Avalonia.Controls.MenuItem { Header = "Clear Formatting", Command = ClearFormattingCommand };

        _contextMenu.ItemsSource = new object[]
        {
            cutItem,
            copyItem,
            pasteItem,
            new Separator(),
            selectAllItem,
            new Separator(),
            undoItem,
            redoItem,
            new Separator(),
            boldItem,
            italicItem,
            underlineItem,
            strikethroughItem,
            fontItem,
            fontSizeItem,
            foregroundItem,
            highlightItem,
            new Separator(),
            clearFormattingItem
        };

        ContextMenu = _contextMenu;
        return _contextMenu;
    }

    private void UpdateContextMenuState()
    {
        if (_contextMenu?.ItemsSource is not object[] items)
        {
            return;
        }

        var hasSelection = HasSelection;
        var canEdit = !IsReadOnlyInternal;

        foreach (var item in items)
        {
            if (item is global::Avalonia.Controls.MenuItem menuItem)
            {
                var header = menuItem.Header?.ToString();
                menuItem.IsEnabled = header switch
                {
                    "Cut" => CanCut,
                    "Copy" => hasSelection,
                    "Paste" => CanPaste,
                    "Undo" => CanUndo,
                    "Redo" => CanRedo,
                    "Select All" => Document.Length > 0,
                    "Bold" or "Italic" or "Underline" or "Strikethrough" or "Font" or "Font Size" or "Foreground" or "Highlight" => hasSelection && canEdit && IsFormattingEnabled,
                    "Clear Formatting" => ClearFormattingCommand.CanExecute(null),
                    _ => true
                };
            }
        }
    }

    private void SyncTextFromDocument()
    {
        _isUpdating = true;
        Text = Document.GetText();

        _editingTextBox?.Text = Text;

        _isUpdating = false;
        ClampSelectionToDocument();
        UpdateFormattedPresenter();
    }

    private void ClampSelectionToDocument()
    {
        var max = Document.Length;
        SelectionStart = Math.Clamp(SelectionStart, 0, max);
        SelectionEnd = Math.Clamp(SelectionEnd, 0, max);
        CaretIndex = Math.Clamp(CaretIndex, 0, max);
        SynchronizeSelectionFromIndexes(raiseEvent: false);
    }

    private void ApplySelectionToTextBox()
    {
        if (_editingTextBox is null)
        {
            return;
        }

        var start = Math.Clamp(SelectionStart, 0, _editingTextBox.Text?.Length ?? 0);
        var end = Math.Clamp(SelectionEnd, 0, _editingTextBox.Text?.Length ?? 0);
        _editingTextBox.SelectionStart = start;
        _editingTextBox.SelectionEnd = end;
        _editingTextBox.CaretIndex = Math.Clamp(CaretIndex, 0, _editingTextBox.Text?.Length ?? 0);
    }

    private void SynchronizeSelectionFromIndexes(bool raiseEvent)
    {
        Selection.Select(SelectionStart, SelectionEnd);
        if (raiseEvent)
        {
            RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
        }
    }

    private readonly record struct RichTextHistoryEntry(string Html, int SelectionStart, int SelectionEnd, int CaretIndex);

    private static class RichTextHelpers
    {
        public static string FormatSegmentAsHtml(TextSegment segment, string text)
        {
            var result = HtmlClipboardUtilities.EncodePlainText(text);
            var styles = new List<string>();
            if (segment.FontFamily is not null)
            {
                styles.Add($"font-family:{segment.FontFamily.Name}");
            }

            if (segment.FontSize.HasValue)
            {
                styles.Add(FormattableString.Invariant($"font-size:{segment.FontSize.Value}px"));
            }

            if (segment.Foreground is SolidColorBrush foreground)
            {
                styles.Add($"color:{foreground.Color}");
            }

            if (segment.Background is SolidColorBrush background)
            {
                styles.Add($"background-color:{background.Color}");
            }

            if (styles.Count > 0)
            {
                result = $"<span style=\"{string.Join(';', styles)}\">{result}</span>";
            }

            if (segment.IsStrikethrough)
            {
                result = $"<s>{result}</s>";
            }

            if (segment.IsUnderline)
            {
                result = $"<u>{result}</u>";
            }

            if (segment.IsItalic)
            {
                result = $"<em>{result}</em>";
            }

            if (segment.IsBold)
            {
                result = $"<strong>{result}</strong>";
            }

            return result;
        }

        public static bool LooksLikeHtml(string text) => text.Contains('<', StringComparison.Ordinal) && text.Contains('>', StringComparison.Ordinal);

        public static string NormalizeClipboardText(string? textPayload)
        {
            if (string.IsNullOrWhiteSpace(textPayload))
            {
                return string.Empty;
            }

            var fragment = HtmlClipboardUtilities.ExtractFragment(textPayload);
            return string.IsNullOrWhiteSpace(fragment) ? textPayload : fragment;
        }

        public static bool IsSupportedImageSource(string? source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return false;
            }

            var value = source.Trim();
            if (value.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
            {
                return value.Contains(";base64,", StringComparison.OrdinalIgnoreCase) || value.Contains(',', StringComparison.Ordinal);
            }

            if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                return IsSupportedImagePath(uri.IsFile ? uri.LocalPath : uri.AbsolutePath);
            }

            return IsSupportedImagePath(value);
        }

        public static bool IsSupportedImagePath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            var extension = Path.GetExtension(path) ?? string.Empty;
            return extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".gif", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".webp", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsSupportedTextFilePath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            var extension = Path.GetExtension(path) ?? string.Empty;
            return extension.Equals(".txt", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".md", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".markdown", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".csv", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".log", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".json", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".xml", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".html", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".htm", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".rtf", StringComparison.OrdinalIgnoreCase);
        }

        public static string GetImageMimeType(string? path)
        {
            var extension = Path.GetExtension(path) ?? string.Empty;
            if (extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return "image/jpeg";
            }

            if (extension.Equals(".gif", StringComparison.OrdinalIgnoreCase))
            {
                return "image/gif";
            }

            if (extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
            {
                return "image/bmp";
            }

            if (extension.Equals(".webp", StringComparison.OrdinalIgnoreCase))
            {
                return "image/webp";
            }

            return "image/png";
        }

        public static string CreateImageHtml(string imageSource) => $"<img src=\"{imageSource.Replace("\"", "%22", StringComparison.Ordinal)}\" />";
    }
}
