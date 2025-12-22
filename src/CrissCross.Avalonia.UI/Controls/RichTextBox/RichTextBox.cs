// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

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

    private readonly FlowDocument _viewDocument;
    private global::Avalonia.Controls.TextBox? _editingTextBox;
    private FormattedTextPresenter? _formattedPresenter;
    private ContextMenu? _contextMenu;
    private bool _isUpdating;
    private bool _isEditing;

    /// <summary>
    /// Initializes a new instance of the <see cref="RichTextBox"/> class.
    /// </summary>
    public RichTextBox()
    {
        _viewDocument = new FlowDocument(Document);
        CreateContextMenu();
        Focusable = true;
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
    public RichTextDocument Document { get; } = new();

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
    /// Selects all text.
    /// </summary>
    public void SelectAll()
    {
        SelectionStart = 0;
        SelectionEnd = Text?.Length ?? 0;
        _editingTextBox?.SelectAll();
    }

    /// <summary>
    /// Clears all text.
    /// </summary>
    public void Clear()
    {
        Text = string.Empty;
        Document.SetText(string.Empty);
        UpdateFormattedPresenter();
    }

    /// <summary>
    /// Clears all formatting.
    /// </summary>
    public void ClearFormatting()
    {
        Document.ClearFormatting();
        UpdateFormattedPresenter();
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
        }

        // Initialize document
        Document.SetText(Text);
        UpdateFormattedPresenter();
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
        _editingTextBox?.Focus();
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
                SelectionStart = _editingTextBox.SelectionStart;
                SelectionEnd = _editingTextBox.SelectionEnd;
                RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
                UpdateContextMenuState();
            }
        }
        else if (e.Property == global::Avalonia.Controls.TextBox.CaretIndexProperty)
        {
            if (_editingTextBox is not null)
            {
                CaretIndex = _editingTextBox.CaretIndex;
            }
        }
    }

    private void OnTextBoxGotFocus(object? sender, GotFocusEventArgs e)
    {
        _isEditing = true;

        // Show editing TextBox, hide formatted presenter
        _editingTextBox?.Opacity = 1;

        _formattedPresenter?.IsVisible = false;
    }

    private void OnTextBoxLostFocus(object? sender, RoutedEventArgs e)
    {
        _isEditing = false;

        // Update formatted presenter and show it
        UpdateFormattedPresenter();

        // Show formatted presenter if we have formatting, otherwise keep text box visible
        if (_formattedPresenter is not null && HasAnyFormatting())
        {
            _formattedPresenter.IsVisible = true;

            _editingTextBox?.Opacity = 0;
        }
    }

    private bool HasAnyFormatting()
    {
        foreach (var segment in Document.Segments)
        {
            if (segment.IsBold || segment.IsItalic || segment.IsUnderline || segment.IsStrikethrough)
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
        UpdateFormattedPresenter();

        RaiseEvent(new FormattingEventArgs(FormattingAppliedEvent, this, formatType, SelectedText));
    }

    private void UpdateFormattedPresenter()
    {
        if (_formattedPresenter is not null)
        {
            _viewDocument.Refresh();
            _formattedPresenter.Document = _viewDocument;
            _formattedPresenter.UpdateInlines();
        }
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

        var boldItem = new MenuItem { Header = "Bold", InputGesture = new KeyGesture(Key.B, KeyModifiers.Control) };
        boldItem.Click += (_, _) => ToggleBold();

        var italicItem = new MenuItem { Header = "Italic", InputGesture = new KeyGesture(Key.I, KeyModifiers.Control) };
        italicItem.Click += (_, _) => ToggleItalic();

        var underlineItem = new MenuItem { Header = "Underline", InputGesture = new KeyGesture(Key.U, KeyModifiers.Control) };
        underlineItem.Click += (_, _) => ToggleUnderline();

        var strikethroughItem = new MenuItem { Header = "Strikethrough" };
        strikethroughItem.Click += (_, _) => ToggleStrikethrough();

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
            boldItem,
            italicItem,
            underlineItem,
            strikethroughItem,
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
                    "Bold" or "Italic" or "Underline" or "Strikethrough" => hasSelection && canEdit && IsFormattingEnabled,
                    "Clear Formatting" => canEdit && IsFormattingEnabled,
                    _ => true
                };
            }
        }
    }
}
