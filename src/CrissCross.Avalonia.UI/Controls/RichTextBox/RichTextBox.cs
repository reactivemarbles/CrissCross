// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
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
    /// <summary>Property for <see cref="Text"/>.</summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<RichTextBox, string?>(nameof(Text), defaultBindingMode: BindingMode.TwoWay);

    /// <summary>Property for <see cref="AcceptsReturn"/>.</summary>
    public static readonly StyledProperty<bool> AcceptsReturnProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(AcceptsReturn), true);

    /// <summary>Property for <see cref="AcceptsTab"/>.</summary>
    public static readonly StyledProperty<bool> AcceptsTabProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(AcceptsTab), true);

    /// <summary>Property for <see cref="IsReadOnly"/>.</summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsReadOnly), false);

    /// <summary>Property for <see cref="IsTextSelectionEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsTextSelectionEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsTextSelectionEnabled), false);

    /// <summary>Property for <see cref="TextWrapping"/>.</summary>
    public static readonly StyledProperty<TextWrapping> TextWrappingProperty =
        AvaloniaProperty.Register<RichTextBox, TextWrapping>(nameof(TextWrapping), TextWrapping.Wrap);

    /// <summary>Property for <see cref="HorizontalScrollBarVisibility"/>.</summary>
    public static readonly StyledProperty<ScrollBarVisibility> HorizontalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<RichTextBox, ScrollBarVisibility>(nameof(HorizontalScrollBarVisibility), ScrollBarVisibility.Disabled);

    /// <summary>Property for <see cref="VerticalScrollBarVisibility"/>.</summary>
    public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<RichTextBox, ScrollBarVisibility>(nameof(VerticalScrollBarVisibility), ScrollBarVisibility.Auto);

    /// <summary>Property for <see cref="CaretBrush"/>.</summary>
    public static readonly StyledProperty<IBrush?> CaretBrushProperty =
        AvaloniaProperty.Register<RichTextBox, IBrush?>(nameof(CaretBrush), Brushes.White);

    /// <summary>Property for <see cref="SelectionBrush"/>.</summary>
    public static readonly StyledProperty<IBrush?> SelectionBrushProperty =
        AvaloniaProperty.Register<RichTextBox, IBrush?>(nameof(SelectionBrush), Brush.Parse("#400078D4"));

    /// <summary>Property for <see cref="Watermark"/>.</summary>
    public static readonly StyledProperty<string?> WatermarkProperty =
        AvaloniaProperty.Register<RichTextBox, string?>(nameof(Watermark));

    /// <summary>Property for <see cref="IsFormattingEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsFormattingEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsFormattingEnabled), true);

    /// <summary>Property for <see cref="IsDocumentEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsDocumentEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsDocumentEnabled), false);

    /// <summary>Property for <see cref="IsDragDropEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsDragDropEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsDragDropEnabled), true);

    /// <summary>Property for <see cref="EditMode"/>.</summary>
    public static readonly StyledProperty<RichTextEditMode> EditModeProperty =
        AvaloniaProperty.Register<RichTextBox, RichTextEditMode>(nameof(EditMode), RichTextEditMode.EditOnFocus);

    /// <summary>Property for <see cref="IsRichClipboardEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsRichClipboardEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsRichClipboardEnabled), true);

    /// <summary>Property for <see cref="IsImagePasteEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsImagePasteEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsImagePasteEnabled), true);

    /// <summary>Property for <see cref="IsImageDropEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsImageDropEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsImageDropEnabled), true);

    /// <summary>Property for <see cref="MaxDroppedTextFileBytes"/>.</summary>
    public static readonly StyledProperty<long> MaxDroppedTextFileBytesProperty =
        AvaloniaProperty.Register<RichTextBox, long>(nameof(MaxDroppedTextFileBytes), 1_048_576);

    /// <summary>Property for <see cref="SelectionStart"/>.</summary>
    public static readonly StyledProperty<int> SelectionStartProperty =
        AvaloniaProperty.Register<RichTextBox, int>(nameof(SelectionStart));

    /// <summary>Property for <see cref="SelectionEnd"/>.</summary>
    public static readonly StyledProperty<int> SelectionEndProperty =
        AvaloniaProperty.Register<RichTextBox, int>(nameof(SelectionEnd));

    /// <summary>Property for <see cref="CaretIndex"/>.</summary>
    public static readonly StyledProperty<int> CaretIndexProperty =
        AvaloniaProperty.Register<RichTextBox, int>(nameof(CaretIndex));

    /// <summary>Defines the <see cref="TextChanged"/> event.</summary>
    public static readonly RoutedEvent<TextChangedEventArgs> TextChangedEvent =
        RoutedEvent.Register<RichTextBox, TextChangedEventArgs>(nameof(TextChanged), RoutingStrategies.Bubble);

    /// <summary>Defines the <see cref="SelectionChanged"/> event.</summary>
    public static readonly RoutedEvent<RoutedEventArgs> SelectionChangedEvent =
        RoutedEvent.Register<RichTextBox, RoutedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);

    /// <summary>Defines the <see cref="FormattingApplied"/> event.</summary>
    public static readonly RoutedEvent<FormattingEventArgs> FormattingAppliedEvent =
        RoutedEvent.Register<RichTextBox, FormattingEventArgs>(nameof(FormattingApplied), RoutingStrategies.Bubble);

    /// <summary>Provides the ParagraphBreakLineCount member.</summary>
    private const int ParagraphBreakLineCount = 2;

    /// <summary>Provides the DefaultImageOverlayHeight member.</summary>
    private const double DefaultImageOverlayHeight = 100d;

    /// <summary>Provides the CharacterWidthFontScale member.</summary>
    private const double CharacterWidthFontScale = 0.6d;

    /// <summary>Provides the DefaultCharacterWidth member.</summary>
    private const double DefaultCharacterWidth = 8.4d;

    /// <summary>Provides the LineHeightFontScale member.</summary>
    private const double LineHeightFontScale = 1.4d;

    /// <summary>Provides the DefaultLineHeight member.</summary>
    private const double DefaultLineHeight = 19.6d;

    /// <summary>Provides the SmallFontSize member.</summary>
    private const double SmallFontSize = 12d;

    /// <summary>Provides the MediumFontSize member.</summary>
    private const double MediumFontSize = 16d;

    /// <summary>Provides the LargeFontSize member.</summary>
    private const double LargeFontSize = 20d;

    /// <summary>Provides the WebPHeaderLength member.</summary>
    private const int WebPHeaderLength = 12;

    /// <summary>Provides the RiffSignatureLength member.</summary>
    private const int RiffSignatureLength = 4;

    /// <summary>Provides the WebPSignatureOffset member.</summary>
    private const int WebPSignatureOffset = 8;

    /// <summary>Provides the _propertyChangedHandlers member.</summary>
    private readonly Dictionary<AvaloniaProperty, Action> _propertyChangedHandlers;

    /// <summary>Provides the _undoStack member.</summary>
    private readonly Stack<RichTextHistoryEntry> _undoStack = new();

    /// <summary>Provides the _redoStack member.</summary>
    private readonly Stack<RichTextHistoryEntry> _redoStack = new();

    /// <summary>Provides the _defaultClipboardAdapter member.</summary>
    private readonly IRichTextClipboardAdapter _defaultClipboardAdapter = new RichTextMemoryClipboardAdapter();

    /// <summary>Provides platform clipboard access for runtime interactions.</summary>
    private readonly RichTextSystemClipboard _systemClipboard;

    /// <summary>Provides the _editingTextBox member.</summary>
    private global::Avalonia.Controls.TextBox? _editingTextBox;

    /// <summary>Provides the _formattedPresenter member.</summary>
    private FormattedTextPresenter? _formattedPresenter;

    /// <summary>Scroll host for formatted display and image overlays.</summary>
    private global::Avalonia.Controls.ScrollViewer? _displayScrollViewer;

    /// <summary>Composed image layer used because text inlines do not render child visuals reliably.</summary>
    private Canvas? _imageOverlay;

    /// <summary>Provides the _contextMenu member.</summary>
    private ContextMenu? _contextMenu;

    /// <summary>Provides the _isUpdating member.</summary>
    private bool _isUpdating;

    /// <summary>Initializes a new instance of the <see cref="RichTextBox"/> class.</summary>
    public RichTextBox()
    {
        _propertyChangedHandlers = CreatePropertyChangedHandlers();
        _systemClipboard = new(() => TopLevel.GetTopLevel(this)?.Clipboard);
        Document = new();
        Selection = new(Document);
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
        AddHandler(KeyDownEvent, OnPreviewKeyDown, RoutingStrategies.Tunnel, true);
        DragDrop.SetAllowDrop(this, true);

        Text = string.Empty;
        Document.SetText(Text);
    }

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
    public bool CanPaste => !IsReadOnlyInternal && (_systemClipboard.IsAvailable || HasPasteContent(GetClipboardAdapter()));

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
                throw new ArgumentException("Pointer does not belong to this document.", nameof(value));
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
    public int SelectionLength => string.IsNullOrEmpty(Text) ? 0 : Math.Abs(SelectionEnd - SelectionStart);

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
            return Document.GetTextRange(Document.GetTextPointer(start), Document.GetTextPointer(start + length));
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
    private bool IsReadOnlyInternal => IsReadOnly || IsTextSelectionEnabled || EditMode == RichTextEditMode.Display;

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

        return TryGetRenderedDocumentOffsetFromPoint(point, out var renderedOffset) ? Document.GetTextPointer(renderedOffset) : Document.GetTextPointer(EstimateDocumentOffsetFromPoint(point, textBounds));
    }

    /// <summary>Determines whether the document should be serialized.</summary>
    /// <returns><see langword="true"/> when the current document has content.</returns>
    public bool ShouldSerializeDocument() => !string.IsNullOrWhiteSpace(Document.PlainText);

    /// <summary>Applies bold formatting to the selection.</summary>
    public void ToggleBold() => ToggleBoldCommand.Execute(null);

    /// <summary>Applies italic formatting to the selection.</summary>
    public void ToggleItalic() => ToggleItalicCommand.Execute(null);

    /// <summary>Applies underline formatting to the selection.</summary>
    public void ToggleUnderline() => ToggleUnderlineCommand.Execute(null);

    /// <summary>Applies strikethrough formatting to the selection.</summary>
    public void ToggleStrikethrough() => ToggleStrikethroughCommand.Execute(null);

    /// <summary>Applies a font family to the selected content.</summary>
    /// <param name="fontFamily">The font family name.</param>
    public void SetSelectionFontFamily(string fontFamily)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fontFamily);
        ApplySpanStyleToSelection($"font-family:{fontFamily};");
    }

    /// <summary>Applies a font size to the selected content.</summary>
    /// <param name="fontSize">The font size in pixels.</param>
    public void SetSelectionFontSize(double fontSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(fontSize);

        ApplySpanStyleToSelection($"font-size:{fontSize}px;");
    }

    /// <summary>Applies a foreground color to the selected content.</summary>
    /// <param name="color">The text color.</param>
    public void SetSelectionForeground(in Color color) => ApplySpanStyleToSelection($"color:{color};");

    /// <summary>Applies a highlight color to the selected content.</summary>
    /// <param name="color">The highlight color.</param>
    public void SetSelectionHighlight(in Color color) => ApplySpanStyleToSelection($"background-color:{color};");

    /// <summary>Selects all text.</summary>
    public void SelectAll() => SelectAllCommand.Execute(null);

    /// <summary>Selects a range by offset and length.</summary>
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

    /// <summary>Selects a range using flow document text pointers.</summary>
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

    /// <summary>Clears all text.</summary>
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

    /// <summary>Clears all formatting.</summary>
    public void ClearFormatting() => ClearFormattingCommand.Execute(null);

    /// <summary>Appends plain text to the end of the document.</summary>
    /// <param name="text">The text to append.</param>
    public void AppendText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var encoded = HtmlClipboardUtilities.EncodePlainText(text);
        Document.AppendText(encoded);
        SyncTextFromDocument();
    }

    /// <summary>Appends HTML content to the end of the document.</summary>
    /// <param name="html">The html fragment to append.</param>
    public void AppendHtml(string html)
    {
        ArgumentNullException.ThrowIfNull(html);
        Document.AppendText(html);
        SyncTextFromDocument();
    }

    /// <summary>Replaces the current content with HTML.</summary>
    /// <param name="html">The html content.</param>
    public void SetHtml(string? html) => SetHtmlCore(html, resetUndo: true);

    /// <summary>Replaces the current content with plain text.</summary>
    /// <param name="text">The plain text content.</param>
    public void SetPlainText(string? text) => SetHtml(HtmlClipboardUtilities.EncodePlainText(text));

    /// <summary>Replaces the current content with markdown converted to HTML.</summary>
    /// <param name="markdown">The markdown content.</param>
    public void SetMarkdown(string? markdown) => SetHtml(MarkdownUtilities.ToHtml(markdown));

    /// <summary>Replaces the current selection with plain text.</summary>
    /// <param name="text">The replacement text.</param>
    public void ReplaceSelection(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        ReplaceSelectionWithHtml(HtmlClipboardUtilities.EncodePlainText(text));
    }

    /// <summary>Replaces the current selection with HTML.</summary>
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

    /// <summary>Copies the selection to the clipboard.</summary>
    public void Copy() => CopyCommand.Execute(null);

    /// <summary>Copies the selection to the configured or platform clipboard.</summary>
    /// <returns>A task that completes when the clipboard write has finished.</returns>
    public async Task CopyToClipboardAsync()
    {
        if (!CanCopy)
        {
            return;
        }

        if (ClipboardAdapter is not null || !_systemClipboard.IsAvailable)
        {
            CopyToAdapter(GetClipboardAdapter());
            return;
        }

        await WriteSelectionToSystemClipboardAsync().ConfigureAwait(true);
    }

    /// <summary>Cuts the selection to the clipboard.</summary>
    public void Cut() => CutCommand.Execute(null);

    /// <summary>Cuts the selection to the configured or platform clipboard.</summary>
    /// <returns>A task that completes when the clipboard write has finished.</returns>
    public async Task CutToClipboardAsync()
    {
        if (!CanCut)
        {
            return;
        }

        await CopyToClipboardAsync().ConfigureAwait(true);
        ReplaceSelectionWithHtml(string.Empty);
    }

    /// <summary>Pastes from the clipboard.</summary>
    public void Paste() => PasteCommand.Execute(null);

    /// <summary>Pastes from the configured or platform clipboard.</summary>
    /// <returns>A task that completes when clipboard content has been read and inserted.</returns>
    public async Task PasteFromClipboardAsync()
    {
        if (IsReadOnlyInternal)
        {
            return;
        }

        if (ClipboardAdapter is not null || !_systemClipboard.IsAvailable)
        {
            PasteFromAdapter(GetClipboardAdapter());
            return;
        }

        await PasteFromSystemClipboardAsync().ConfigureAwait(true);
    }

    /// <summary>Ensures that the RichTextBox context menu exists and returns it.</summary>
    /// <returns>The context menu used by the control.</returns>
    public ContextMenu EnsureContextMenu()
    {
        _contextMenu ??= CreateContextMenu();
        UpdateContextMenuState();
        return _contextMenu;
    }

    /// <summary>Refreshes command enablement for the context menu items.</summary>
    public void RefreshContextMenuState()
    {
        _contextMenu ??= CreateContextMenu();
        UpdateContextMenuState();
    }

    /// <summary>Inserts a dropped text payload when drag/drop and edit state allow it.</summary>
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

    /// <summary>Inserts a dropped image source when drag/drop, image policy, and edit state allow it.</summary>
    /// <param name="imageSource">The image file URI/path or data URI.</param>
    /// <returns><see langword="true"/> when an image was inserted.</returns>
    public bool TryDropImage(string? imageSource)
    {
        return imageSource is null ? false : TryInsertImage(imageSource, requireDragDrop: true);
    }

    /// <summary>Undoes the last action.</summary>
    public void Undo() => UndoCommand.Execute(null);

    /// <summary>Redoes the last undone action.</summary>
    public void Redo() => RedoCommand.Execute(null);

    /// <summary>Saves content to a stream.</summary>
    /// <param name="stream">The stream to save to.</param>
    /// <param name="encoding">The encoding to use.</param>
    public void Save(Stream stream, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        encoding ??= Encoding.UTF8;
        var bytes = encoding.GetBytes(Text ?? string.Empty);
        stream.Write(bytes, 0, bytes.Length);
    }

    /// <summary>Saves content to a stream in the requested format.</summary>
    /// <param name="stream">The stream to save to.</param>
    /// <param name="format">The data format to save.</param>
    /// <param name="encoding">The encoding to use.</param>
    public void Save(Stream stream, RichTextDataFormat format, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        encoding ??= Encoding.UTF8;
        var content = format switch
        {
            RichTextDataFormat.PlainText => PlainText,
            RichTextDataFormat.Html => Html,
            RichTextDataFormat.Markdown => Markdown,
            RichTextDataFormat.Rtf => string.Empty,
            _ => Html
        };
        var bytes = encoding.GetBytes(content);
        stream.Write(bytes, 0, bytes.Length);
    }

    /// <summary>Loads content from a stream.</summary>
    /// <param name="stream">The stream to load from.</param>
    /// <param name="encoding">The encoding to use.</param>
    public void Load(Stream stream, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        encoding ??= Encoding.UTF8;
        using var reader = new StreamReader(stream, encoding);
        SetHtml(reader.ReadToEnd());
    }

    /// <summary>Loads content from a stream in the requested format.</summary>
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

    /// <summary>Creates an embeddable image source from a dropped storage file.</summary>
    /// <param name="file">The dropped storage file.</param>
    /// <returns>The embedded data URI, or <see langword="null"/> when the file is not a supported image.</returns>
    internal static Task<string?> TryCreateDroppedImageSourceAsync(IStorageFile file) => RichTextHelpers.TryCreateImageSourceAsync(file);

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
        _displayScrollViewer = e.NameScope.Find<global::Avalonia.Controls.ScrollViewer>("PART_DisplayScrollViewer");
        _imageOverlay = e.NameScope.Find<Canvas>("PART_ImageOverlay");
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
            _editingTextBox.Text = Document.PlainText;

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

        if (!_propertyChangedHandlers.TryGetValue(change.Property, out var handler))
        {
            return;
        }

        handler();
    }

    /// <inheritdoc/>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e is null)
        {
            base.OnKeyDown(e!);
            return;
        }

        if (TryHandleShortcut(e))
        {
            return;
        }

        base.OnKeyDown(e);
    }

    /// <inheritdoc/>
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (e?.InitialPressMouseButton != MouseButton.Right)
        {
            return;
        }

        UpdateContextMenuState();
        _contextMenu?.Open(this);
        e.Handled = true;
    }

    /// <inheritdoc/>
    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);

        _ = _editingTextBox?.Focus();

        UpdateDisplayMode();
    }

    /// <summary>Handles command gestures before the composed text editor consumes them.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The key event.</param>
    private void OnPreviewKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        _ = TryHandleShortcut(e);
    }

    /// <summary>Routes WPF-compatible editing and formatting gestures.</summary>
    /// <param name="e">The key event.</param>
    /// <returns><see langword="true"/> when a command handled the gesture.</returns>
    private bool TryHandleShortcut(KeyEventArgs e)
    {
        if (!e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            return false;
        }

        var command = e.Key switch
        {
            Key.C => CopyCommand,
            Key.X => CutCommand,
            Key.V => PasteCommand,
            Key.Z when e.KeyModifiers.HasFlag(KeyModifiers.Shift) => RedoCommand,
            Key.Z => UndoCommand,
            Key.Y => RedoCommand,
            Key.B when IsFormattingEnabled && !IsReadOnlyInternal => ToggleBoldCommand,
            Key.I when IsFormattingEnabled && !IsReadOnlyInternal => ToggleItalicCommand,
            Key.U when IsFormattingEnabled && !IsReadOnlyInternal => ToggleUnderlineCommand,
            Key.A => SelectAllCommand,
            Key.S when e.KeyModifiers.HasFlag(KeyModifiers.Shift) && IsFormattingEnabled && !IsReadOnlyInternal => ToggleStrikethroughCommand,
            _ => null,
        };

        if (command is null)
        {
            return false;
        }

        command.Execute(null);
        e.Handled = true;
        return true;
    }

    /// <summary>Provides the OnTextBoxTextChanged member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_isUpdating)
        {
            return;
        }

        _isUpdating = true;

        var newText = _editingTextBox?.Text ?? string.Empty;
        var before = CaptureHistory();

        if (RichTextEditingShim.ApplyPlainTextChange(Document, newText))
        {
            Text = Document.Text;
            UpdateFormattedPresenter();
            CommitHistory(before);
            RaiseEvent(new TextChangedEventArgs(TextChangedEvent));
        }

        _isUpdating = false;
    }

    /// <summary>Provides the OnTextBoxPropertyChanged member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
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

    /// <summary>Provides the OnTextBoxGotFocus member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnTextBoxGotFocus(object? sender, FocusChangedEventArgs e)
    {
        UpdateDisplayMode();
        SynchronizeSelectionFromIndexes(raiseEvent: false);
    }

    /// <summary>Provides the OnTextBoxLostFocus member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnTextBoxLostFocus(object? sender, FocusChangedEventArgs e)
    {
        UpdateFormattedPresenter();
        UpdateDisplayMode();
    }

    /// <summary>Provides the SelectAllCore member.</summary>
    private void SelectAllCore()
    {
        SelectionStart = 0;
        SelectionEnd = Document.Length;
        CaretIndex = SelectionEnd;
        SynchronizeSelectionFromIndexes(raiseEvent: true);
        _editingTextBox?.SelectAll();
        NotifyCommandStateChanged();
    }

    /// <summary>Provides the ClearFormattingCore member.</summary>
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

    /// <summary>Provides the SetHtmlCore member.</summary>
    /// <param name="html">The html value.</param>
    /// <param name="resetUndo">The resetUndo value.</param>
    private void SetHtmlCore(string? html, bool resetUndo)
    {
        _isUpdating = true;
        Document.SetText(html);
        Text = html;
        if (_editingTextBox is not null)
        {
            _editingTextBox.Text = Document.PlainText;
            _editingTextBox.CaretIndex = Document.Length;
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

    /// <summary>Creates the property changed handler table.</summary>
    /// <returns>The property changed handler table.</returns>
    private Dictionary<AvaloniaProperty, Action> CreatePropertyChangedHandlers() => new()
    {
        [TextProperty] = OnTextPropertyChanged,
        [AcceptsReturnProperty] = () => UpdateEditingTextBox(textBox => textBox.AcceptsReturn = AcceptsReturn),
        [AcceptsTabProperty] = () => UpdateEditingTextBox(textBox => textBox.AcceptsTab = AcceptsTab),
        [TextWrappingProperty] = OnTextWrappingPropertyChanged,
        [HorizontalScrollBarVisibilityProperty] = () => UpdateEditingTextBox(textBox => ScrollViewer.SetHorizontalScrollBarVisibility(textBox, HorizontalScrollBarVisibility)),
        [VerticalScrollBarVisibilityProperty] = () => UpdateEditingTextBox(textBox => ScrollViewer.SetVerticalScrollBarVisibility(textBox, VerticalScrollBarVisibility)),
        [CaretBrushProperty] = () => UpdateEditingTextBox(textBox => textBox.CaretBrush = CaretBrush),
        [SelectionBrushProperty] = () => UpdateEditingTextBox(textBox => textBox.SelectionBrush = SelectionBrush),
        [IsReadOnlyProperty] = OnReadOnlyStatePropertyChanged,
        [IsTextSelectionEnabledProperty] = OnTextSelectionEnabledPropertyChanged,
        [SelectionStartProperty] = OnSelectionIndexPropertyChanged,
        [SelectionEndProperty] = OnSelectionIndexPropertyChanged,
        [CaretIndexProperty] = OnCaretIndexPropertyChanged,
        [IsDocumentEnabledProperty] = OnDocumentEnabledPropertyChanged,
        [IsDragDropEnabledProperty] = OnDragDropEnabledPropertyChanged,
        [EditModeProperty] = OnEditModePropertyChanged,
    };

    /// <summary>Updates the editing text box when it is available.</summary>
    /// <param name="update">The update action.</param>
    private void UpdateEditingTextBox(Action<global::Avalonia.Controls.TextBox> update)
    {
        if (_editingTextBox is null)
        {
            return;
        }

        update(_editingTextBox);
    }

    /// <summary>Handles text property updates.</summary>
    private void OnTextPropertyChanged()
    {
        _isUpdating = true;
        try
        {
            Document.SetText(Text);
            _editingTextBox?.Text = Document.PlainText;
            UpdateFormattedPresenter();
        }
        finally
        {
            _isUpdating = false;
        }

        ClampSelectionToDocument();
        RaiseEvent(new TextChangedEventArgs(TextChangedEvent));
    }

    /// <summary>Handles text wrapping property updates.</summary>
    private void OnTextWrappingPropertyChanged()
    {
        UpdateEditingTextBox(textBox => textBox.TextWrapping = TextWrapping);
        if (_formattedPresenter is null)
        {
            return;
        }

        _formattedPresenter.TextWrapping = TextWrapping;
    }

    /// <summary>Handles read-only state property updates.</summary>
    private void OnReadOnlyStatePropertyChanged()
    {
        var readOnly = IsReadOnly || IsTextSelectionEnabled;
        UpdateEditingTextBox(textBox => textBox.IsReadOnly = readOnly);
        SetAllowDropOnSurfaces(IsDragDropEnabled && !readOnly);
    }

    /// <summary>Handles text selection mode property updates.</summary>
    private void OnTextSelectionEnabledPropertyChanged()
    {
        OnReadOnlyStatePropertyChanged();
        UpdateDisplayMode();
    }

    /// <summary>Handles selection index property updates.</summary>
    private void OnSelectionIndexPropertyChanged()
    {
        ClampSelectionToDocument();
        ApplySelectionToTextBox();
        SynchronizeSelectionFromIndexes(raiseEvent: true);
    }

    /// <summary>Handles caret index property updates.</summary>
    private void OnCaretIndexPropertyChanged()
    {
        var boundedCaret = Math.Clamp(CaretIndex, 0, Document.Length);
        if (CaretIndex != boundedCaret)
        {
            CaretIndex = boundedCaret;
        }

        _editingTextBox?.CaretIndex = boundedCaret;
    }

    /// <summary>Handles document enabled property updates.</summary>
    private void OnDocumentEnabledPropertyChanged()
    {
        if (_formattedPresenter is null)
        {
            return;
        }

        _formattedPresenter.IsHitTestVisible = IsDocumentEnabled;
    }

    /// <summary>Handles drag and drop enabled property updates.</summary>
    private void OnDragDropEnabledPropertyChanged() => SetAllowDropOnSurfaces(IsDragDropEnabled && !IsReadOnlyInternal);

    /// <summary>Handles edit mode property updates.</summary>
    private void OnEditModePropertyChanged()
    {
        UpdateEditingTextBox(textBox => textBox.IsReadOnly = IsReadOnlyInternal);
        SetAllowDropOnSurfaces(IsDragDropEnabled && !IsReadOnlyInternal);
        UpdateDisplayMode();
        NotifyCommandStateChanged();
    }

    /// <summary>Sets drag and drop availability on all rich text surfaces.</summary>
    /// <param name="allowDrop">Whether drag and drop is allowed.</param>
    private void SetAllowDropOnSurfaces(bool allowDrop)
    {
        DragDrop.SetAllowDrop(this, allowDrop);
        RichTextHelpers.SetAllowDrop(_editingTextBox, allowDrop);
        RichTextHelpers.SetAllowDrop(_formattedPresenter, allowDrop);
    }

    /// <summary>Provides the GetClipboardAdapter member.</summary>
    /// <returns>The result.</returns>
    private IRichTextClipboardAdapter GetClipboardAdapter() => ClipboardAdapter ?? _defaultClipboardAdapter;

    /// <summary>Determines whether the clipboard has content that can be pasted.</summary>
    /// <param name="clipboard">The clipboard adapter.</param>
    /// <returns><see langword="true"/> when pasteable content is available.</returns>
    private bool HasPasteContent(IRichTextClipboardAdapter clipboard) =>
        (IsRichClipboardEnabled && clipboard.ContainsHtml) ||
        clipboard.ContainsPlainText ||
        (IsImagePasteEnabled && clipboard.ContainsImage && RichTextHelpers.IsSupportedImageSource(clipboard.ImageSource));

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
    private RichTextHistoryEntry CaptureHistory() => new(Document.Text, SelectionStart, SelectionEnd, CaretIndex);

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

        if (ClipboardAdapter is not null || !_systemClipboard.IsAvailable)
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
        clipboard.SetHtml(string.IsNullOrEmpty(selectedHtml) ? HtmlClipboardUtilities.EncodePlainText(selectedText) : selectedHtml);
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

        return _systemClipboard.WriteAsync(selectedText, selectedHtml);
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

        if (ClipboardAdapter is null && _systemClipboard.IsAvailable)
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
        var content = await _systemClipboard.ReadAsync().ConfigureAwait(true);
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
        if (!IsRichClipboardEnabled || !clipboard.ContainsHtml || string.IsNullOrEmpty(clipboard.HtmlText))
        {
            return false;
        }

        var fragment = HtmlClipboardUtilities.ExtractFragment(clipboard.HtmlText) ?? clipboard.HtmlText;
        ReplaceSelectionWithHtml(fragment ?? string.Empty);
        return true;
    }

    /// <summary>Tries to paste image clipboard content.</summary>
    /// <param name="clipboard">The clipboard adapter.</param>
    /// <returns><see langword="true"/> when an image was pasted.</returns>
    private bool TryPasteImage(IRichTextClipboardAdapter clipboard) =>
        IsImagePasteEnabled &&
        clipboard.ContainsImage &&
        TryInsertImage(clipboard.ImageSource, requireDragDrop: false);

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
        !IsReadOnlyInternal &&
        (requireDragDrop ? IsDragDropEnabled && IsImageDropEnabled : IsImagePasteEnabled);

    /// <summary>Provides the NotifyCommandStateChanged member.</summary>
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

    /// <summary>Provides the UpdateFormattedPresenter member.</summary>
    private void UpdateFormattedPresenter()
    {
        if (_formattedPresenter is null)
        {
            return;
        }

        Document.Refresh();
        _formattedPresenter.Document = Document;
        _formattedPresenter.UpdateInlines();
        UpdateImageOverlay();
        UpdateDisplayMode();
    }

    /// <summary>Updates the composed image layer from document image segments.</summary>
    private void UpdateImageOverlay()
    {
        if (_imageOverlay is null || _formattedPresenter is null)
        {
            return;
        }

        _imageOverlay.Children.Clear();
        var lineHeight = GetEstimatedLineHeight();
        var stackedImageOffset = 0d;
        var overlayHeight = 0d;

        foreach (var segment in Document.Segments.Where(segment => segment.IsImage))
        {
            var image = _formattedPresenter.CreateImageElement(segment);
            if (image is null)
            {
                continue;
            }

            var precedingLineCount = Document.Segments.Count(candidate =>
                candidate.StartIndex < segment.StartIndex && candidate.IsLineBreak) +
                (Document.Segments.Count(candidate => candidate.StartIndex < segment.StartIndex && candidate.IsParagraphBreak) * ParagraphBreakLineCount) +
                1;
            var top = (precedingLineCount * lineHeight) + stackedImageOffset;
            var imageHeight = double.IsNaN(image.Height) ? DefaultImageOverlayHeight : image.Height;

            Canvas.SetLeft(image, 0);
            Canvas.SetTop(image, top);
            _imageOverlay.Children.Add(image);

            stackedImageOffset += imageHeight + lineHeight;
            overlayHeight = Math.Max(overlayHeight, top + imageHeight);
        }

        _imageOverlay.Height = overlayHeight;
    }

    /// <summary>Provides the UpdateDisplayMode member.</summary>
    private void UpdateDisplayMode()
    {
        if (_editingTextBox is null || _formattedPresenter is null)
        {
            return;
        }

        var showEditing = ShouldShowEditingSurface();
        var showRichEditingSurface = showEditing && Document.Segments.Any(segment => segment.IsImage);
        UpdateEditingSurface(showEditing, showRichEditingSurface);
        UpdateFormattedSurface(showEditing, showRichEditingSurface);
    }

    /// <summary>Applies state to the editable text surface.</summary>
    /// <param name="showEditing">Whether editing is active.</param>
    /// <param name="showRichEditingSurface">Whether formatted content overlays the editor.</param>
    private void UpdateEditingSurface(bool showEditing, bool showRichEditingSurface)
    {
        if (_editingTextBox is null)
        {
            return;
        }

        _editingTextBox.IsVisible = showEditing;
        _editingTextBox.IsHitTestVisible = showEditing;
        _editingTextBox.Opacity = showEditing ? 1 : 0;
        _editingTextBox.Foreground = showRichEditingSurface ? Brushes.Transparent : Foreground;
    }

    /// <summary>Applies state to the formatted text and image surfaces.</summary>
    /// <param name="showEditing">Whether editing is active.</param>
    /// <param name="showRichEditingSurface">Whether formatted content overlays the editor.</param>
    private void UpdateFormattedSurface(bool showEditing, bool showRichEditingSurface)
    {
        if (_formattedPresenter is null)
        {
            return;
        }

        var showFormattedSurface = !showEditing || showRichEditingSurface;
        _formattedPresenter.IsVisible = showFormattedSurface;
        _formattedPresenter.IsHitTestVisible = !showEditing && IsDocumentEnabled;
        _imageOverlay?.SetCurrentValue(IsVisibleProperty, showFormattedSurface);
        _displayScrollViewer?.SetCurrentValue(IsHitTestVisibleProperty, !showEditing && IsDocumentEnabled);
    }

    /// <summary>Determines whether the composed text input surface should be active.</summary>
    /// <returns><see langword="true"/> when the editing surface should be shown.</returns>
    private bool ShouldShowEditingSurface() => EditMode switch
    {
        RichTextEditMode.Edit => true,
        RichTextEditMode.Display => false,
        _ => _editingTextBox?.IsFocused == true || IsFocused,
    };

    /// <summary>Provides the OnDragOver member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnDragOver(object? sender, DragEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        e.DragEffects = CanAcceptDropData(e.DataTransfer) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    /// <summary>Provides the OnDrop member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
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

    /// <summary>Provides the CanAcceptDropData member.</summary>
    /// <param name="dataTransfer">The dataTransfer value.</param>
    /// <returns>The result.</returns>
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
            var path = RichTextHelpers.GetStorageItemPath(file);
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

    /// <summary>Provides the TryInsertDropDataAsync member.</summary>
    /// <param name="dataTransfer">The dataTransfer value.</param>
    /// <returns>The result.</returns>
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
                else if (RichTextHelpers.IsSupportedImagePath(RichTextHelpers.GetStorageItemPath(file)) && TryDropImage(file.Path.AbsoluteUri))
                {
                    inserted = true;
                }
            }

            return inserted;
        }

        return TryDropText(dataTransfer.TryGetText());
    }

    /// <summary>Provides the TryDropStorageFileAsync member.</summary>
    /// <param name="file">The file value.</param>
    /// <returns>The result.</returns>
    private async Task<bool> TryDropStorageFileAsync(IStorageFile file)
    {
        var path = RichTextHelpers.GetStorageItemPath(file);
        if (RichTextHelpers.IsSupportedImagePath(path))
        {
            var imageSource = await RichTextHelpers.TryCreateImageSourceAsync(file).ConfigureAwait(true);
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
        return Encoding.UTF8.GetByteCount(text) > MaxDroppedTextFileBytes ? false : TryDropText(text);
    }

    /// <summary>Provides the MoveSelectionToDropPoint member.</summary>
    /// <param name="point">The point value.</param>
    private void MoveSelectionToDropPoint(Point point)
    {
        var position = GetPositionFromPoint(point, snapToText: true);
        if (position is null)
        {
            return;
        }

        Select(position.Offset, 0);
    }

    /// <summary>Provides the TryGetRenderedDocumentOffsetFromPoint member.</summary>
    /// <param name="point">The point value.</param>
    /// <param name="offset">The offset value.</param>
    /// <returns>The result.</returns>
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

    /// <summary>Provides the TryHitTestTextLayout member.</summary>
    /// <param name="visual">The visual value.</param>
    /// <param name="point">The point value.</param>
    /// <param name="offset">The offset value.</param>
    /// <returns>The result.</returns>
    private bool TryHitTestTextLayout(Visual? visual, Point point, out int offset)
    {
        offset = 0;
        if (visual is null)
        {
            return false;
        }

        var textLayout = visual switch
        {
            TextPresenter presenter => presenter.TextLayout,
            FormattedTextPresenter presenter => presenter.TextLayout,
            _ => null,
        };

        if (textLayout is null)
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

    /// <summary>Provides the GetTextHitTestBounds member.</summary>
    /// <returns>The result.</returns>
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

    /// <summary>Provides the EstimateDocumentOffsetFromPoint member.</summary>
    /// <param name="point">The point value.</param>
    /// <param name="bounds">The bounds value.</param>
    /// <returns>The result.</returns>
    private int EstimateDocumentOffsetFromPoint(Point point, Rect bounds)
    {
        var text = Document.PlainText;
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

    /// <summary>Provides the GetEstimatedCharacterWidth member.</summary>
    /// <returns>The result.</returns>
    private double GetEstimatedCharacterWidth() => Math.Max(1, FontSize > 0 ? FontSize * CharacterWidthFontScale : DefaultCharacterWidth);

    /// <summary>Provides the GetEstimatedLineHeight member.</summary>
    /// <returns>The result.</returns>
    private double GetEstimatedLineHeight() => Math.Max(1, FontSize > 0 ? FontSize * LineHeightFontScale : DefaultLineHeight);

    /// <summary>Provides the CreateContextMenu member.</summary>
    /// <returns>The result.</returns>
    private ContextMenu CreateContextMenu()
    {
        _contextMenu = new();
        _contextMenu.Opened += (_, _) => UpdateContextMenuState();

        var cutItem = new global::Avalonia.Controls.MenuItem { Header = "Cut", InputGesture = new(Key.X, KeyModifiers.Control), Command = CutCommand };

        var copyItem = new global::Avalonia.Controls.MenuItem { Header = "Copy", InputGesture = new(Key.C, KeyModifiers.Control), Command = CopyCommand };

        var pasteItem = new global::Avalonia.Controls.MenuItem { Header = "Paste", InputGesture = new(Key.V, KeyModifiers.Control), Command = PasteCommand };

        var selectAllItem = new global::Avalonia.Controls.MenuItem { Header = "Select All", InputGesture = new(Key.A, KeyModifiers.Control), Command = SelectAllCommand };

        var undoItem = new global::Avalonia.Controls.MenuItem { Header = "Undo", InputGesture = new(Key.Z, KeyModifiers.Control), Command = UndoCommand };

        var redoItem = new global::Avalonia.Controls.MenuItem { Header = "Redo", InputGesture = new(Key.Y, KeyModifiers.Control), Command = RedoCommand };

        var boldItem = new global::Avalonia.Controls.MenuItem { Header = "Bold", InputGesture = new(Key.B, KeyModifiers.Control), Command = ToggleBoldCommand };

        var italicItem = new global::Avalonia.Controls.MenuItem { Header = "Italic", InputGesture = new(Key.I, KeyModifiers.Control), Command = ToggleItalicCommand };

        var underlineItem = new global::Avalonia.Controls.MenuItem { Header = "Underline", InputGesture = new(Key.U, KeyModifiers.Control), Command = ToggleUnderlineCommand };

        var strikethroughItem = new global::Avalonia.Controls.MenuItem { Header = "Strikethrough", Command = ToggleStrikethroughCommand };

        var fontItem = new global::Avalonia.Controls.MenuItem { Header = "Font" };
        var fontConsolas = new global::Avalonia.Controls.MenuItem { Header = "Consolas", Command = SetFontFamilyCommand, CommandParameter = "Consolas" };
        var fontSegoe = new global::Avalonia.Controls.MenuItem { Header = "Segoe UI", Command = SetFontFamilyCommand, CommandParameter = "Segoe UI" };
        var fontTimes = new global::Avalonia.Controls.MenuItem { Header = "Times New Roman", Command = SetFontFamilyCommand, CommandParameter = "Times New Roman" };
        fontItem.ItemsSource = new object[] { fontConsolas, fontSegoe, fontTimes };

        var fontSizeItem = new global::Avalonia.Controls.MenuItem { Header = "Font Size" };
        var size12 = new global::Avalonia.Controls.MenuItem { Header = "12", Command = SetFontSizeCommand, CommandParameter = SmallFontSize };
        var size16 = new global::Avalonia.Controls.MenuItem { Header = "16", Command = SetFontSizeCommand, CommandParameter = MediumFontSize };
        var size20 = new global::Avalonia.Controls.MenuItem { Header = "20", Command = SetFontSizeCommand, CommandParameter = LargeFontSize };
        fontSizeItem.ItemsSource = new object[] { size12, size16, size20 };

        var foregroundItem = new global::Avalonia.Controls.MenuItem { Header = "Foreground" };
        var foregroundWhite = new global::Avalonia.Controls.MenuItem { Header = "White", Command = SetForegroundCommand, CommandParameter = Colors.White };
        var foregroundBlue = new global::Avalonia.Controls.MenuItem { Header = "DeepSkyBlue", Command = SetForegroundCommand, CommandParameter = Colors.DeepSkyBlue };
        var foregroundOrange = new global::Avalonia.Controls.MenuItem { Header = "Orange", Command = SetForegroundCommand, CommandParameter = Colors.Orange };
        foregroundItem.ItemsSource = new object[] { foregroundWhite, foregroundBlue, foregroundOrange };

        var highlightItem = new global::Avalonia.Controls.MenuItem { Header = "Highlight" };
        var highlightTransparent = new global::Avalonia.Controls.MenuItem { Header = "Transparent", Command = SetHighlightCommand, CommandParameter = Colors.Transparent };
        var highlightYellow = new global::Avalonia.Controls.MenuItem { Header = "Yellow", Command = SetHighlightCommand, CommandParameter = Colors.Yellow };
        var highlightGreen = new global::Avalonia.Controls.MenuItem { Header = "LightGreen", Command = SetHighlightCommand, CommandParameter = Colors.LightGreen };
        highlightItem.ItemsSource = new object[] { highlightTransparent, highlightYellow, highlightGreen };

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

    /// <summary>Provides the UpdateContextMenuState member.</summary>
    private void UpdateContextMenuState()
    {
        if (_contextMenu?.ItemsSource is not object[] items)
        {
            return;
        }

        var hasSelection = HasSelection;
        var canEdit = !IsReadOnlyInternal;

        foreach (var menuItem in items.OfType<global::Avalonia.Controls.MenuItem>())
        {
            menuItem.IsEnabled = IsContextMenuItemEnabled(menuItem, hasSelection, canEdit);
        }
    }

    /// <summary>Determines whether a context menu item should be enabled.</summary>
    /// <param name="menuItem">The menu item.</param>
    /// <param name="hasSelection">Whether the editor has selected text.</param>
    /// <param name="canEdit">Whether editing is currently allowed.</param>
    /// <returns><see langword="true"/> when the item should be enabled.</returns>
    private bool IsContextMenuItemEnabled(global::Avalonia.Controls.MenuItem menuItem, bool hasSelection, bool canEdit)
    {
        var header = menuItem.Header?.ToString();
        return RichTextHelpers.IsFormattingMenuHeader(header)
            ? hasSelection && canEdit && IsFormattingEnabled
            : header switch
            {
                "Cut" => CanCut,
                "Copy" => hasSelection,
                "Paste" => CanPaste,
                "Undo" => CanUndo,
                "Redo" => CanRedo,
                "Select All" => Document.Length > 0,
                "Clear Formatting" => ClearFormattingCommand.CanExecute(null),
                _ => true
            };
    }

    /// <summary>Provides the SyncTextFromDocument member.</summary>
    private void SyncTextFromDocument()
    {
        _isUpdating = true;
        Text = Document.Text;

        _editingTextBox?.Text = Document.PlainText;

        _isUpdating = false;
        ClampSelectionToDocument();
        UpdateFormattedPresenter();
    }

    /// <summary>Provides the ClampSelectionToDocument member.</summary>
    private void ClampSelectionToDocument()
    {
        var max = Document.Length;
        SelectionStart = Math.Clamp(SelectionStart, 0, max);
        SelectionEnd = Math.Clamp(SelectionEnd, 0, max);
        CaretIndex = Math.Clamp(CaretIndex, 0, max);
        SynchronizeSelectionFromIndexes(raiseEvent: false);
    }

    /// <summary>Provides the ApplySelectionToTextBox member.</summary>
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

    /// <summary>Provides the SynchronizeSelectionFromIndexes member.</summary>
    /// <param name="raiseEvent">The raiseEvent value.</param>
    private void SynchronizeSelectionFromIndexes(bool raiseEvent)
    {
        Selection.Select(SelectionStart, SelectionEnd);
        if (!raiseEvent)
        {
            return;
        }

        RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
    }

    /// <summary>Provides the RichTextHistoryEntry member.</summary>
    /// <param name="Html">The Html value.</param>
    /// <param name="SelectionStart">The SelectionStart value.</param>
    /// <param name="SelectionEnd">The SelectionEnd value.</param>
    /// <param name="CaretIndex">The CaretIndex value.</param>
    private readonly record struct RichTextHistoryEntry(string Html, int SelectionStart, int SelectionEnd, int CaretIndex);

    /// <summary>Provides the RichTextHelpers member.</summary>
    private static class RichTextHelpers
    {
        /// <summary>Provides the supported text file extensions.</summary>
        private static readonly string[] SupportedTextFileExtensions =
        [
            ".txt",
            ".md",
            ".markdown",
            ".csv",
            ".log",
            ".json",
            ".xml",
            ".html",
            ".htm",
            ".rtf"
        ];

        /// <summary>Sets drag and drop availability on a control when it exists.</summary>
        /// <param name="control">The target control.</param>
        /// <param name="allowDrop">Whether drag and drop is allowed.</param>
        public static void SetAllowDrop(Control? control, bool allowDrop)
        {
            if (control is null)
            {
                return;
            }

            DragDrop.SetAllowDrop(control, allowDrop);
        }

        /// <summary>Determines whether a context menu header controls formatting.</summary>
        /// <param name="header">The menu header.</param>
        /// <returns><see langword="true"/> when the header represents a formatting command.</returns>
        public static bool IsFormattingMenuHeader(string? header) =>
            header is "Bold" or "Italic" or "Underline" or "Strikethrough" or "Font" or "Font Size" or "Foreground" or "Highlight";

        /// <summary>Appends selected text from one segment as HTML.</summary>
        /// <param name="segment">The segment to inspect.</param>
        /// <param name="start">The selected range start.</param>
        /// <param name="end">The selected range end.</param>
        /// <param name="builder">The destination builder.</param>
        public static void AppendSelectedSegmentHtml(TextSegment segment, int start, int end, StringBuilder builder)
        {
            if (!segment.HasRenderableText || segment.EndIndex <= start || segment.StartIndex >= end)
            {
                return;
            }

            var segmentStart = Math.Max(start, segment.StartIndex);
            var segmentEnd = Math.Min(end, segment.EndIndex);
            var segmentLength = segmentEnd - segmentStart;
            if (segmentLength <= 0)
            {
                return;
            }

            var localStart = segmentStart - segment.StartIndex;
            var selectedText = segment.Text.Substring(localStart, segmentLength);
            _ = builder.Append(FormatSegmentAsHtml(segment, selectedText));
        }

        /// <summary>Determines whether one segment contains formatting.</summary>
        /// <param name="segment">The segment to inspect.</param>
        /// <returns><see langword="true"/> when formatting is present.</returns>
        public static bool HasFormatting(TextSegment segment) =>
            HasCharacterFormatting(segment) ||
            HasObjectFormatting(segment) ||
            segment.ParagraphAlignment.HasValue;

        /// <summary>Determines whether one segment contains character formatting.</summary>
        /// <param name="segment">The segment to inspect.</param>
        /// <returns><see langword="true"/> when character formatting is present.</returns>
        public static bool HasCharacterFormatting(TextSegment segment) =>
            segment.IsBold ||
            segment.IsItalic ||
            segment.IsUnderline ||
            segment.IsStrikethrough ||
            segment.Foreground is not null ||
            segment.Background is not null ||
            segment.FontSize.HasValue ||
            segment.FontFamily is not null;

        /// <summary>Determines whether one segment contains object formatting.</summary>
        /// <param name="segment">The segment to inspect.</param>
        /// <returns><see langword="true"/> when object formatting is present.</returns>
        public static bool HasObjectFormatting(TextSegment segment) => segment.IsImage;

        /// <summary>Provides the GetStorageItemPath member.</summary>
        /// <param name="file">The storage item.</param>
        /// <returns>The local storage item path when available.</returns>
        public static string? GetStorageItemPath(IStorageItem file)
        {
            var localPath = file.TryGetLocalPath();
            if (!string.IsNullOrWhiteSpace(localPath))
            {
                return localPath;
            }

            return file.Path.IsAbsoluteUri ? file.Path.AbsolutePath : file.Path.ToString();
        }

        /// <summary>Provides the TryCreateImageSourceAsync member.</summary>
        /// <param name="file">The file value.</param>
        /// <returns>The result.</returns>
        public static async Task<string?> TryCreateImageSourceAsync(IStorageFile file)
        {
            var path = GetStorageItemPath(file);
            if (!IsSupportedImagePath(path))
            {
                return null;
            }

            var dataUri = await TryCreateImageDataUriAsync(file, path).ConfigureAwait(true);
            if (!string.IsNullOrWhiteSpace(dataUri))
            {
                var localPath = file.TryGetLocalPath();
                return string.IsNullOrWhiteSpace(localPath)
                    ? dataUri
                    : new Uri(localPath, UriKind.Absolute).AbsoluteUri;
            }

            return file.Path.Scheme is "http" or "https" ? file.Path.AbsoluteUri : null;
        }

        /// <summary>Provides the FormatSegmentAsHtml member.</summary>
        /// <param name="segment">The segment value.</param>
        /// <param name="text">The text value.</param>
        /// <returns>The result.</returns>
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

        /// <summary>Provides the LooksLikeHtml member.</summary>
        /// <param name="text">The text value.</param>
        /// <returns>The result.</returns>
        public static bool LooksLikeHtml(string text) => text.Contains('<', StringComparison.Ordinal) && text.Contains('>', StringComparison.Ordinal);

        /// <summary>Provides the NormalizeClipboardText member.</summary>
        /// <param name="textPayload">The textPayload value.</param>
        /// <returns>The result.</returns>
        public static string NormalizeClipboardText(string? textPayload)
        {
            if (string.IsNullOrWhiteSpace(textPayload))
            {
                return string.Empty;
            }

            var fragment = HtmlClipboardUtilities.ExtractFragment(textPayload);
            return string.IsNullOrWhiteSpace(fragment) ? textPayload : fragment;
        }

        /// <summary>Provides the IsSupportedImageSource member.</summary>
        /// <param name="source">The source value.</param>
        /// <returns>The result.</returns>
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

        /// <summary>Provides the IsSupportedImagePath member.</summary>
        /// <param name="path">The path value.</param>
        /// <returns>The result.</returns>
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

        /// <summary>Provides the IsSupportedTextFilePath member.</summary>
        /// <param name="path">The path value.</param>
        /// <returns>The result.</returns>
        public static bool IsSupportedTextFilePath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            var extension = Path.GetExtension(path) ?? string.Empty;
            return SupportedTextFileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>Provides the GetImageMimeType member.</summary>
        /// <param name="path">The path value.</param>
        /// <returns>The result.</returns>
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

            return extension.Equals(".webp", StringComparison.OrdinalIgnoreCase) ? "image/webp" : "image/png";
        }

        /// <summary>Provides the CreateImageHtml member.</summary>
        /// <param name="imageSource">The imageSource value.</param>
        /// <returns>The result.</returns>
        public static string CreateImageHtml(string imageSource) => $"<img src=\"{imageSource.Replace("\"", "%22", StringComparison.Ordinal)}\" />";

        /// <summary>Provides the TryCreateImageDataUriAsync member.</summary>
        /// <param name="file">The file value.</param>
        /// <param name="path">The path value.</param>
        /// <returns>The result.</returns>
        private static async Task<string?> TryCreateImageDataUriAsync(IStorageFile file, string? path)
        {
            try
            {
                await using var stream = await file.OpenReadAsync().ConfigureAwait(true);
                await using var buffer = new MemoryStream();
                await stream.CopyToAsync(buffer).ConfigureAwait(true);
                var bytes = buffer.ToArray();
                if (!HasExpectedImageSignature(bytes, path))
                {
                    return null;
                }

                var mimeType = GetImageMimeType(path);
                return $"data:{mimeType};base64,{Convert.ToBase64String(bytes)}";
            }
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>Checks that file content starts with the signature expected for its image extension.</summary>
        /// <param name="bytes">The image file bytes.</param>
        /// <param name="path">The image file path.</param>
        /// <returns>The result.</returns>
        private static bool HasExpectedImageSignature(ReadOnlySpan<byte> bytes, string? path)
        {
            var extension = Path.GetExtension(path) ?? string.Empty;
            return extension.ToUpperInvariant() switch
            {
                ".PNG" => bytes.StartsWith(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
                ".JPG" or ".JPEG" => bytes.StartsWith(new byte[] { 0xFF, 0xD8, 0xFF }),
                ".GIF" => bytes.StartsWith("GIF87a"u8) || bytes.StartsWith("GIF89a"u8),
                ".BMP" => bytes.StartsWith("BM"u8),
                ".WEBP" => HasWebPImageSignature(bytes),
                _ => false,
            };
        }

        /// <summary>Checks for the RIFF and WebP markers in a WebP image header.</summary>
        /// <param name="bytes">The image file bytes.</param>
        /// <returns>The result.</returns>
        private static bool HasWebPImageSignature(ReadOnlySpan<byte> bytes) =>
            bytes.Length >= WebPHeaderLength &&
            bytes[..RiffSignatureLength].SequenceEqual("RIFF"u8) &&
            bytes[WebPSignatureOffset..WebPHeaderLength].SequenceEqual("WEBP"u8);
    }
}
