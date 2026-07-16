// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Provides the Input members for <see cref="RichTextBox"/>.</summary>
public partial class RichTextBox
{
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
            Key.S
                when e.KeyModifiers.HasFlag(KeyModifiers.Shift)
                    && IsFormattingEnabled
                    && !IsReadOnlyInternal => ToggleStrikethroughCommand,
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
        if ((e.Property == global::Avalonia.Controls.TextBox.SelectionStartProperty
                || e.Property == global::Avalonia.Controls.TextBox.SelectionEndProperty)
            && _editingTextBox is not null)
        {
            var max = Document.Length;
            SelectionStart = Math.Clamp(_editingTextBox.SelectionStart, 0, max);
            SelectionEnd = Math.Clamp(_editingTextBox.SelectionEnd, 0, max);
            CaretIndex = Math.Clamp(_editingTextBox.CaretIndex, 0, max);
            SynchronizeSelectionFromIndexes(raiseEvent: true);
            UpdateContextMenuState();
        }
        else if (e.Property == global::Avalonia.Controls.TextBox.CaretIndexProperty
            && _editingTextBox is not null)
        {
            CaretIndex = Math.Clamp(_editingTextBox.CaretIndex, 0, Document.Length);
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
    private Dictionary<AvaloniaProperty, Action> CreatePropertyChangedHandlers() =>
        new()
        {
            [TextProperty] = OnTextPropertyChanged,
            [AcceptsReturnProperty] = () =>
                UpdateEditingTextBox(textBox => textBox.AcceptsReturn = AcceptsReturn),
            [AcceptsTabProperty] = () =>
                UpdateEditingTextBox(textBox => textBox.AcceptsTab = AcceptsTab),
            [TextWrappingProperty] = OnTextWrappingPropertyChanged,
            [HorizontalScrollBarVisibilityProperty] = () =>
                UpdateEditingTextBox(textBox =>
                    ScrollViewer.SetHorizontalScrollBarVisibility(
                        textBox,
                        HorizontalScrollBarVisibility)),
            [VerticalScrollBarVisibilityProperty] = () =>
                UpdateEditingTextBox(textBox =>
                    ScrollViewer.SetVerticalScrollBarVisibility(
                        textBox,
                        VerticalScrollBarVisibility)),
            [CaretBrushProperty] = () =>
                UpdateEditingTextBox(textBox => textBox.CaretBrush = CaretBrush),
            [SelectionBrushProperty] = () =>
                UpdateEditingTextBox(textBox => textBox.SelectionBrush = SelectionBrush),
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
    private void OnDragDropEnabledPropertyChanged() =>
        SetAllowDropOnSurfaces(IsDragDropEnabled && !IsReadOnlyInternal);

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
    private IRichTextClipboardAdapter GetClipboardAdapter() =>
        ClipboardAdapter ?? _defaultClipboardAdapter;

    /// <summary>Gets the clipboard associated with the current top-level control.</summary>
    /// <returns>The platform clipboard when the control is attached to a top level.</returns>
    private global::Avalonia.Input.Platform.IClipboard? GetSystemClipboard() =>
        TopLevel.GetTopLevel(this)?.Clipboard;

    /// <summary>Determines whether the clipboard has content that can be pasted.</summary>
    /// <param name="clipboard">The clipboard adapter.</param>
    /// <returns><see langword="true"/> when pasteable content is available.</returns>
    private bool HasPasteContent(IRichTextClipboardAdapter clipboard) =>
        (IsRichClipboardEnabled && clipboard.ContainsHtml)
        || clipboard.ContainsPlainText
        || (
            IsImagePasteEnabled
            && clipboard.ContainsImage
            && RichTextHelpers.IsSupportedImageSource(clipboard.ImageSource));
}
