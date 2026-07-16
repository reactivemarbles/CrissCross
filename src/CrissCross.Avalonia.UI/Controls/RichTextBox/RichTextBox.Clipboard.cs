// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Platform.Storage;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Provides the Clipboard members for <see cref="RichTextBox"/>.</summary>
public partial class RichTextBox
{
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

        if (ClipboardAdapter is not null || !RichTextSystemClipboard.IsAvailable(GetSystemClipboard()))
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

        if (ClipboardAdapter is not null || !RichTextSystemClipboard.IsAvailable(GetSystemClipboard()))
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
    public void Save(Stream stream) => Save(stream, null);

    /// <summary>Saves content to a stream using the requested encoding.</summary>
    /// <param name="stream">The stream to save to.</param>
    /// <param name="encoding">The encoding to use.</param>
    public void Save(Stream stream, Encoding? encoding)
    {
        ArgumentNullException.ThrowIfNull(stream);
        encoding ??= Encoding.UTF8;
        var bytes = encoding.GetBytes(Text ?? string.Empty);
        stream.Write(bytes, 0, bytes.Length);
    }

    /// <summary>Saves content to a stream in the requested format.</summary>
    /// <param name="stream">The stream to save to.</param>
    /// <param name="format">The data format to save.</param>
    public void Save(Stream stream, RichTextDataFormat format) => Save(stream, format, null);

    /// <summary>Saves content to a stream in the requested format and encoding.</summary>
    /// <param name="stream">The stream to save to.</param>
    /// <param name="format">The data format to save.</param>
    /// <param name="encoding">The encoding to use.</param>
    public void Save(Stream stream, RichTextDataFormat format, Encoding? encoding)
    {
        ArgumentNullException.ThrowIfNull(stream);
        encoding ??= Encoding.UTF8;
        var content = format switch
        {
            RichTextDataFormat.PlainText => PlainText,
            RichTextDataFormat.Html => Html,
            RichTextDataFormat.Markdown => Markdown,
            RichTextDataFormat.Rtf => string.Empty,
            _ => Html,
        };
        var bytes = encoding.GetBytes(content);
        stream.Write(bytes, 0, bytes.Length);
    }

    /// <summary>Loads content from a stream.</summary>
    /// <param name="stream">The stream to load from.</param>
    public void Load(Stream stream) => Load(stream, null);

    /// <summary>Loads content from a stream using the requested encoding.</summary>
    /// <param name="stream">The stream to load from.</param>
    /// <param name="encoding">The encoding to use.</param>
    public void Load(Stream stream, Encoding? encoding)
    {
        ArgumentNullException.ThrowIfNull(stream);
        encoding ??= Encoding.UTF8;
        using var reader = new StreamReader(stream, encoding);
        SetHtml(reader.ReadToEnd());
    }

    /// <summary>Loads content from a stream in the requested format.</summary>
    /// <param name="stream">The stream to load from.</param>
    /// <param name="format">The data format to load.</param>
    public void Load(Stream stream, RichTextDataFormat format) => Load(stream, format, null);

    /// <summary>Loads content from a stream in the requested format and encoding.</summary>
    /// <param name="stream">The stream to load from.</param>
    /// <param name="format">The data format to load.</param>
    /// <param name="encoding">The encoding to use.</param>
    public void Load(Stream stream, RichTextDataFormat format, Encoding? encoding)
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
    internal static Task<string?> TryCreateDroppedImageSourceAsync(IStorageFile file) =>
        RichTextHelpers.TryCreateImageSourceAsync(file);

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
        _displayScrollViewer = e.NameScope.Find<global::Avalonia.Controls.ScrollViewer>(
            "PART_DisplayScrollViewer");
        _imageOverlay = e.NameScope.Find<Canvas>("PART_ImageOverlay");
        _contextMenu ??= CreateContextMenu();

        // Setup editing text box
        if (_editingTextBox is not null)
        {
            _editingTextBox.AcceptsReturn = AcceptsReturn;
            _editingTextBox.AcceptsTab = AcceptsTab;
            _editingTextBox.IsReadOnly = IsReadOnly || IsTextSelectionEnabled;
            _editingTextBox.TextWrapping = TextWrapping;
            ScrollViewer.SetHorizontalScrollBarVisibility(
                _editingTextBox,
                HorizontalScrollBarVisibility);
            ScrollViewer.SetVerticalScrollBarVisibility(
                _editingTextBox,
                VerticalScrollBarVisibility);
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
}
