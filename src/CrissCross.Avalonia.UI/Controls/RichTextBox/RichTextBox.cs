// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;

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
        CreateContextMenu();
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
            return Text.Substring(start, Math.Min(length, Text.Length - start));
        }
    }

    /// <summary>
    /// Returns the text position nearest the provided point.
    /// </summary>
    /// <param name="point">Point in control coordinates.</param>
    /// <param name="snapToText">When false and point is outside content bounds, returns null.</param>
    /// <returns>The nearest document position.</returns>
    public TextPointer? GetPositionFromPoint(in Point point, bool snapToText)
    {
        if (_editingTextBox is null)
        {
            return snapToText ? CaretPosition : null;
        }

        var bounds = _editingTextBox.Bounds;
        if (!snapToText && !bounds.Contains(point))
        {
            return null;
        }

        return CaretPosition;
    }

    /// <summary>
    /// Determines whether the document should be serialized.
    /// </summary>
    /// <returns><see langword="true"/> when the current document has content.</returns>
    public bool ShouldSerializeDocument() => !string.IsNullOrWhiteSpace(Document.GetText());

    /// <summary>
    /// Applies bold formatting to the selection.
    /// </summary>
    public void ToggleBold() => ApplyFormattingToSelection(TextFormatType.Bold);

    /// <summary>
    /// Applies italic formatting to the selection.
    /// </summary>
    public void ToggleItalic() => ApplyFormattingToSelection(TextFormatType.Italic);

    /// <summary>
    /// Applies underline formatting to the selection.
    /// </summary>
    public void ToggleUnderline() => ApplyFormattingToSelection(TextFormatType.Underline);

    /// <summary>
    /// Applies strikethrough formatting to the selection.
    /// </summary>
    public void ToggleStrikethrough() => ApplyFormattingToSelection(TextFormatType.Strikethrough);

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
    public void SelectAll()
    {
        SelectionStart = 0;
        SelectionEnd = Document.Length;
        CaretIndex = SelectionEnd;
        SynchronizeSelectionFromIndexes(raiseEvent: true);
        _editingTextBox?.SelectAll();
    }

    /// <summary>
    /// Clears all text.
    /// </summary>
    public void Clear()
    {
        SetHtml(string.Empty);
        UpdateFormattedPresenter();
    }

    /// <summary>
    /// Clears all formatting.
    /// </summary>
    public void ClearFormatting()
    {
        Document.ClearFormatting();
        SyncTextFromDocument();
        UpdateFormattedPresenter();
    }

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
    public void SetHtml(string? html)
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
    }

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

        var start = Math.Min(SelectionStart, SelectionEnd);
        var length = Math.Abs(SelectionEnd - SelectionStart);
        Document.Replace(start, length, html);
        SyncTextFromDocument();

        var newCaret = Math.Clamp(start + html.Length, 0, Document.Length);
        SelectionStart = newCaret;
        SelectionEnd = newCaret;
        CaretIndex = newCaret;
        ApplySelectionToTextBox();
        SynchronizeSelectionFromIndexes(raiseEvent: true);
    }

    /// <summary>
    /// Copies the selection to the clipboard.
    /// </summary>
    public void Copy() => _editingTextBox?.Copy();

    /// <summary>
    /// Cuts the selection to the clipboard.
    /// </summary>
    public void Cut() => _editingTextBox?.Cut();

    /// <summary>
    /// Pastes from the clipboard.
    /// </summary>
    public void Paste() => _editingTextBox?.Paste();

    /// <summary>
    /// Undoes the last action.
    /// </summary>
    public void Undo() => _editingTextBox?.Undo();

    /// <summary>
    /// Redoes the last undone action.
    /// </summary>
    public void Redo() => _editingTextBox?.Redo();

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
    /// Loads content from a stream.
    /// </summary>
    /// <param name="stream">The stream to load from.</param>
    /// <param name="encoding">The encoding to use.</param>
    public void Load(Stream stream, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        encoding ??= Encoding.UTF8;
        using var reader = new StreamReader(stream, encoding);
        Text = reader.ReadToEnd();
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

        // Setup editing text box
        if (_editingTextBox is not null)
        {
            _editingTextBox.AcceptsReturn = AcceptsReturn;
            _editingTextBox.AcceptsTab = AcceptsTab;
            _editingTextBox.IsReadOnly = IsReadOnly;
            _editingTextBox.TextWrapping = TextWrapping.Wrap;
            _editingTextBox.Text = Text;

            _editingTextBox.TextChanged += OnTextBoxTextChanged;
            _editingTextBox.PropertyChanged += OnTextBoxPropertyChanged;
            _editingTextBox.GotFocus += OnTextBoxGotFocus;
            _editingTextBox.LostFocus += OnTextBoxLostFocus;
            _editingTextBox.ContextMenu = _contextMenu;
            DragDrop.SetAllowDrop(_editingTextBox, IsDragDropEnabled && !IsReadOnly);
        }

        if (_formattedPresenter is not null)
        {
            _formattedPresenter.ContextMenu = _contextMenu;
            DragDrop.SetAllowDrop(_formattedPresenter, IsDragDropEnabled && !IsReadOnly);
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
        else if (change.Property == IsReadOnlyProperty && _editingTextBox is not null)
        {
            _editingTextBox.IsReadOnly = IsReadOnly;
            DragDrop.SetAllowDrop(this, IsDragDropEnabled && !IsReadOnly);
            if (_formattedPresenter is not null)
            {
                DragDrop.SetAllowDrop(_formattedPresenter, IsDragDropEnabled && !IsReadOnly);
            }

            DragDrop.SetAllowDrop(_editingTextBox, IsDragDropEnabled && !IsReadOnly);
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
            DragDrop.SetAllowDrop(this, IsDragDropEnabled && !IsReadOnly);
            if (_editingTextBox is not null)
            {
                DragDrop.SetAllowDrop(_editingTextBox, IsDragDropEnabled && !IsReadOnly);
            }

            if (_formattedPresenter is not null)
            {
                DragDrop.SetAllowDrop(_formattedPresenter, IsDragDropEnabled && !IsReadOnly);
            }
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

        // Handle formatting shortcuts
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control) && IsFormattingEnabled && !IsReadOnly)
        {
            switch (e.Key)
            {
                case Key.B:
                    ToggleBold();
                    e.Handled = true;
                    return;
                case Key.I:
                    ToggleItalic();
                    e.Handled = true;
                    return;
                case Key.U:
                    ToggleUnderline();
                    e.Handled = true;
                    return;
                case Key.A:
                    SelectAll();
                    e.Handled = true;
                    return;
                case Key.S when e.KeyModifiers.HasFlag(KeyModifiers.Shift):
                    ToggleStrikethrough();
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
    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);

        if (_editingTextBox is not null)
        {
            _editingTextBox.IsVisible = true;
            _editingTextBox.IsHitTestVisible = true;
            _editingTextBox.Opacity = 1;
            _editingTextBox.Focus();
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
        var newText = _editingTextBox?.Text;

        // Update document to reflect changes
        if (oldText != newText)
        {
            // For simplicity, we update the document entirely
            // A more sophisticated implementation would track incremental changes
            Document.SetText(newText);
            Text = newText;
            UpdateFormattedPresenter();
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

    private void OnTextBoxGotFocus(object? sender, GotFocusEventArgs e)
    {
        UpdateDisplayMode();
        SynchronizeSelectionFromIndexes(raiseEvent: false);
    }

    private void OnTextBoxLostFocus(object? sender, RoutedEventArgs e)
    {
        UpdateFormattedPresenter();
        UpdateDisplayMode();
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
        if (!IsFormattingEnabled || IsReadOnly || !HasSelection)
        {
            return;
        }

        var start = Math.Min(SelectionStart, SelectionEnd);
        var length = Math.Abs(SelectionEnd - SelectionStart);

        Document.ToggleFormatting(start, length, formatType);
        SyncTextFromDocument();
        UpdateFormattedPresenter();

        RaiseEvent(new FormattingEventArgs(FormattingAppliedEvent, this, formatType, SelectedText));
    }

    private void ApplySpanStyleToSelection(string style)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(style);

        if (!IsFormattingEnabled || IsReadOnly || !HasSelection)
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

        var showEditing = _editingTextBox.IsFocused || IsFocused;
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

        if (!IsDragDropEnabled || IsReadOnly)
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
            return;
        }

        e.DragEffects = DragDropEffects.Copy;
        e.Handled = true;
    }

    private async void OnDrop(object? sender, DragEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        static bool IsSupportedImagePath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            var extension = Path.GetExtension(path);
            return extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".gif", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".webp", StringComparison.OrdinalIgnoreCase);
        }

        static string GetMimeType(string path)
        {
            var extension = Path.GetExtension(path);
            return extension.ToLowerInvariant() switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }

        static async Task<string?> TryCreateDataUriAsync(IStorageFile file)
        {
            var localPath = file.TryGetLocalPath() ?? file.Path.AbsolutePath;
            if (!IsSupportedImagePath(localPath))
            {
                return null;
            }

            await using var stream = await file.OpenReadAsync();
            using var buffer = new MemoryStream();
            await stream.CopyToAsync(buffer);
            var bytes = buffer.ToArray();
            if (bytes.Length == 0)
            {
                return null;
            }

            var mimeType = GetMimeType(localPath);
            return $"data:{mimeType};base64,{Convert.ToBase64String(bytes)}";
        }

        static async Task<object?> InvokeTransferMemberAsync(IDataTransfer dataTransfer, string memberName)
        {
            MethodInfo? targetMethod = null;
            foreach (var method in dataTransfer.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (string.Equals(method.Name, memberName, StringComparison.Ordinal))
                {
                    targetMethod = method;
                    break;
                }
            }

            if (targetMethod is null)
            {
                return null;
            }

            object? invocationResult;
            var parameters = targetMethod.GetParameters();
            if (parameters.Length == 0)
            {
                invocationResult = targetMethod.Invoke(dataTransfer, null);
            }
            else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(CancellationToken))
            {
                invocationResult = targetMethod.Invoke(dataTransfer, [CancellationToken.None]);
            }
            else
            {
                return null;
            }

            if (invocationResult is null)
            {
                return null;
            }

            if (invocationResult is Task task)
            {
                await task;
                return task.GetType().GetProperty("Result", BindingFlags.Public | BindingFlags.Instance)?.GetValue(task);
            }

            var asTaskMethod = invocationResult.GetType().GetMethod("AsTask", BindingFlags.Public | BindingFlags.Instance, []);
            if (asTaskMethod?.Invoke(invocationResult, null) is Task valueTaskAsTask)
            {
                await valueTaskAsTask;
                return valueTaskAsTask.GetType().GetProperty("Result", BindingFlags.Public | BindingFlags.Instance)?.GetValue(valueTaskAsTask);
            }

            return invocationResult;
        }

        static IReadOnlyList<IStorageItem>? ToStorageItemList(object? value)
        {
            if (value is IReadOnlyList<IStorageItem> readOnlyList)
            {
                return readOnlyList;
            }

            if (value is IEnumerable<IStorageItem> typedEnumerable)
            {
                return [.. typedEnumerable];
            }

            if (value is IEnumerable<object> objectEnumerable)
            {
                return [.. objectEnumerable.OfType<IStorageItem>()];
            }

            return null;
        }

        if (!IsDragDropEnabled || IsReadOnly)
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
            return;
        }

        var filesResult = await InvokeTransferMemberAsync(e.DataTransfer, "GetFilesAsync") ??
                          await InvokeTransferMemberAsync(e.DataTransfer, "GetFiles");
        var files = ToStorageItemList(filesResult);
        if (files is null || files.Count == 0)
        {
            var legacyFiles = e.Data.GetFiles();
            if (legacyFiles is not null)
            {
                files = [.. legacyFiles];
            }
        }

        if (files is { Count: > 0 })
        {
            var imageTags = new List<string>();
            foreach (var file in files)
            {
                if (file is IStorageFile storageFile)
                {
                    var dataUri = await TryCreateDataUriAsync(storageFile);
                    if (!string.IsNullOrWhiteSpace(dataUri))
                    {
                        imageTags.Add($"<img src=\"{dataUri}\" />");
                        continue;
                    }
                }

                var path = file.Path.AbsolutePath;
                if (IsSupportedImagePath(path))
                {
                    imageTags.Add($"<img src=\"{file.Path.AbsoluteUri}\" />");
                }
                else if (file is IStorageFile textFile)
                {
                    await using var readStream = await textFile.OpenReadAsync();
                    using var reader = new StreamReader(readStream, Encoding.UTF8, true);
                    var text = await reader.ReadToEndAsync();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        ReplaceSelection(text);
                    }
                }
            }

            if (imageTags.Count > 0)
            {
                var htmlBuilder = new StringBuilder();
                foreach (var tag in imageTags)
                {
                    htmlBuilder.Append(tag);
                }

                ReplaceSelectionWithHtml(htmlBuilder.ToString());
            }

            e.Handled = true;
            return;
        }

        var textPayload = (await InvokeTransferMemberAsync(e.DataTransfer, "GetTextAsync") as string) ??
                          (await InvokeTransferMemberAsync(e.DataTransfer, "GetText") as string);
        if (string.IsNullOrWhiteSpace(textPayload))
        {
            textPayload = e.Data.GetText();
        }

        if (!string.IsNullOrWhiteSpace(textPayload))
        {
            var looksLikeHtml = textPayload.Contains('<', StringComparison.Ordinal) && textPayload.Contains('>', StringComparison.Ordinal);
            if (looksLikeHtml)
            {
                ReplaceSelectionWithHtml(textPayload);
            }
            else
            {
                ReplaceSelection(textPayload);
            }
        }

        e.Handled = true;
    }

    private void CreateContextMenu()
    {
        _contextMenu = new ContextMenu();

        var cutItem = new MenuItem { Header = "Cut", InputGesture = new KeyGesture(Key.X, KeyModifiers.Control) };
        cutItem.Click += (_, _) => Cut();

        var copyItem = new MenuItem { Header = "Copy", InputGesture = new KeyGesture(Key.C, KeyModifiers.Control) };
        copyItem.Click += (_, _) => Copy();

        var pasteItem = new MenuItem { Header = "Paste", InputGesture = new KeyGesture(Key.V, KeyModifiers.Control) };
        pasteItem.Click += (_, _) => Paste();

        var selectAllItem = new MenuItem { Header = "Select All", InputGesture = new KeyGesture(Key.A, KeyModifiers.Control) };
        selectAllItem.Click += (_, _) => SelectAll();

        var undoItem = new MenuItem { Header = "Undo", InputGesture = new KeyGesture(Key.Z, KeyModifiers.Control) };
        undoItem.Click += (_, _) => Undo();

        var redoItem = new MenuItem { Header = "Redo", InputGesture = new KeyGesture(Key.Y, KeyModifiers.Control) };
        redoItem.Click += (_, _) => Redo();

        var boldItem = new MenuItem { Header = "Bold", InputGesture = new KeyGesture(Key.B, KeyModifiers.Control) };
        boldItem.Click += (_, _) => ToggleBold();

        var italicItem = new MenuItem { Header = "Italic", InputGesture = new KeyGesture(Key.I, KeyModifiers.Control) };
        italicItem.Click += (_, _) => ToggleItalic();

        var underlineItem = new MenuItem { Header = "Underline", InputGesture = new KeyGesture(Key.U, KeyModifiers.Control) };
        underlineItem.Click += (_, _) => ToggleUnderline();

        var strikethroughItem = new MenuItem { Header = "Strikethrough" };
        strikethroughItem.Click += (_, _) => ToggleStrikethrough();

        var fontItem = new MenuItem { Header = "Font" };
        var fontConsolas = new MenuItem { Header = "Consolas" };
        fontConsolas.Click += (_, _) => SetSelectionFontFamily("Consolas");
        var fontSegoe = new MenuItem { Header = "Segoe UI" };
        fontSegoe.Click += (_, _) => SetSelectionFontFamily("Segoe UI");
        var fontTimes = new MenuItem { Header = "Times New Roman" };
        fontTimes.Click += (_, _) => SetSelectionFontFamily("Times New Roman");
        fontItem.ItemsSource = new object[] { fontConsolas, fontSegoe, fontTimes };

        var fontSizeItem = new MenuItem { Header = "Font Size" };
        var size12 = new MenuItem { Header = "12" };
        size12.Click += (_, _) => SetSelectionFontSize(12);
        var size16 = new MenuItem { Header = "16" };
        size16.Click += (_, _) => SetSelectionFontSize(16);
        var size20 = new MenuItem { Header = "20" };
        size20.Click += (_, _) => SetSelectionFontSize(20);
        fontSizeItem.ItemsSource = new object[] { size12, size16, size20 };

        var foregroundItem = new MenuItem { Header = "Foreground" };
        var fgWhite = new MenuItem { Header = "White" };
        fgWhite.Click += (_, _) => SetSelectionForeground(Colors.White);
        var fgBlue = new MenuItem { Header = "DeepSkyBlue" };
        fgBlue.Click += (_, _) => SetSelectionForeground(Colors.DeepSkyBlue);
        var fgOrange = new MenuItem { Header = "Orange" };
        fgOrange.Click += (_, _) => SetSelectionForeground(Colors.Orange);
        foregroundItem.ItemsSource = new object[] { fgWhite, fgBlue, fgOrange };

        var highlightItem = new MenuItem { Header = "Highlight" };
        var hlTransparent = new MenuItem { Header = "Transparent" };
        hlTransparent.Click += (_, _) => SetSelectionHighlight(Colors.Transparent);
        var hlYellow = new MenuItem { Header = "Yellow" };
        hlYellow.Click += (_, _) => SetSelectionHighlight(Colors.Yellow);
        var hlGreen = new MenuItem { Header = "LightGreen" };
        hlGreen.Click += (_, _) => SetSelectionHighlight(Colors.LightGreen);
        highlightItem.ItemsSource = new object[] { hlTransparent, hlYellow, hlGreen };

        var clearFormattingItem = new MenuItem { Header = "Clear Formatting" };
        clearFormattingItem.Click += (_, _) => ClearFormatting();

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
    }

    private void UpdateContextMenuState()
    {
        if (_contextMenu?.ItemsSource is not object[] items)
        {
            return;
        }

        var hasSelection = HasSelection;
        var canEdit = !IsReadOnly;

        foreach (var item in items)
        {
            if (item is MenuItem menuItem)
            {
                var header = menuItem.Header?.ToString();
                menuItem.IsEnabled = header switch
                {
                    "Cut" => hasSelection && canEdit,
                    "Copy" => hasSelection,
                    "Paste" => canEdit,
                    "Undo" => canEdit,
                    "Redo" => canEdit,
                    "Bold" or "Italic" or "Underline" or "Strikethrough" or "Font" or "Font Size" or "Foreground" or "Highlight" => hasSelection && canEdit && IsFormattingEnabled,
                    "Clear Formatting" => canEdit && IsFormattingEnabled,
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
}
