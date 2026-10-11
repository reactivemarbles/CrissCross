// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using RibbonButton = Avalonia.Controls.Button;
using RibbonTextBox = Avalonia.Controls.TextBox;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>A reusable ribbon composed around a separate rich text editor.</summary>
public sealed class RichTextEditorRibbon : UserControl
{
    /// <summary>Identifies the <see cref="Editor"/> property.</summary>
    public static readonly StyledProperty<RichTextBox?> EditorProperty =
        AvaloniaProperty.Register<RichTextEditorRibbon, RichTextBox?>(nameof(Editor));

    /// <summary>The spacing between command groups.</summary>
    private const double GroupSpacing = 6;

    /// <summary>The space surrounding ribbon controls.</summary>
    private const double ButtonMargin = 2;

    /// <summary>The hyperlink address input width.</summary>
    private const double AddressWidth = 220;

    /// <summary>The default table row and column count.</summary>
    private const int DefaultTableDimension = 3;

    /// <summary>The body font size choice.</summary>
    private const double BodyFontSize = 12;

    /// <summary>The normal font size choice.</summary>
    private const double NormalFontSize = 16;

    /// <summary>The heading font size choice.</summary>
    private const double HeadingFontSize = 20;

    /// <summary>The title font size choice.</summary>
    private const double TitleFontSize = 28;

    /// <summary>The display font size choice.</summary>
    private const double DisplayFontSize = 36;

    /// <summary>The controls that insert document elements.</summary>
    private readonly List<Control> _insertionControls = [];

    /// <summary>The controls that require an editable selection.</summary>
    private readonly List<Control> _formattingControls = [];

    /// <summary>Reports insertion failures without interrupting the document.</summary>
    private global::Avalonia.Controls.TextBlock _status = new();

    /// <summary>The currently subscribed editor.</summary>
    private RichTextBox? _observedEditor;

    /// <summary>Tracks whether the ribbon is attached to a visual tree.</summary>
    private bool _attached;

    /// <summary>Initializes a new instance of the <see cref="RichTextEditorRibbon"/> class.</summary>
    public RichTextEditorRibbon() => BuildRibbon();

    /// <summary>Gets or sets the editor controlled by this ribbon.</summary>
    public RichTextBox? Editor
    {
        get => GetValue(EditorProperty);
        set => SetValue(EditorProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property != EditorProperty)
        {
            return;
        }

        ObserveEditor();
        BuildRibbon();
    }

    /// <inheritdoc/>
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _attached = true;
        ObserveEditor();
    }

    /// <inheritdoc/>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _attached = false;
        ObserveEditor();
        base.OnDetachedFromVisualTree(e);
    }

    /// <summary>Adds a command button that preserves editor focus.</summary>
    /// <param name="panel">The command group.</param>
    /// <param name="label">The button label.</param>
    /// <param name="command">The editor command.</param>
    private static void AddCommand(Panel panel, string label, ICommand? command) => panel.Children.Add(new RibbonButton
    {
        Content = label,
        Command = command,
        IsEnabled = command is not null,
        Focusable = false,
        Margin = new(ButtonMargin),
    });

    /// <summary>Adds a selection menu using the editor command's availability.</summary>
    /// <param name="panel">The group.</param>
    /// <param name="label">The menu label.</param>
    /// <param name="values">The command values.</param>
    /// <param name="command">The editor command.</param>
    private void AddChoices(Panel panel, string label, object[] values, ICommand? command)
    {
        var button = new RibbonButton { Content = label, Focusable = false, Margin = new(ButtonMargin), IsEnabled = command is not null };
        var menu = new ContextMenu();
        foreach (var value in values)
        {
            _ = menu.Items.Add(new MenuItem { Header = value is Color color ? color.ToString() : value.ToString(), Command = command, CommandParameter = value });
        }

        button.Click += (_, _) => menu.Open(button);
        panel.Children.Add(button);
        _formattingControls.Add(button);
    }

    /// <summary>Updates the editor subscription when the target or attachment changes.</summary>
    private void ObserveEditor()
    {
        if (_observedEditor is not null)
        {
            _observedEditor.PropertyChanged -= OnEditorPropertyChanged;
        }

        _observedEditor = _attached ? Editor : null;
        if (_observedEditor is not null)
        {
            _observedEditor.PropertyChanged += OnEditorPropertyChanged;
        }

        RefreshInsertionState();
    }

    /// <summary>Updates insertion availability when the target changes mode.</summary>
    /// <param name="sender">The editor.</param>
    /// <param name="e">The property change.</param>
    private void OnEditorPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e) => RefreshInsertionState();

    /// <summary>Refreshes document insertion availability.</summary>
    private void RefreshInsertionState()
    {
        var enabled = Editor is { IsReadOnly: false, IsTextSelectionEnabled: false } editor && editor.EditMode != RichTextEditMode.Display;
        foreach (var control in _insertionControls)
        {
            control.IsEnabled = enabled;
        }

        foreach (var control in _formattingControls)
        {
            control.IsEnabled = Editor?.CanApplyFormatting == true;
        }
    }

    /// <summary>Builds command groups for the current editor.</summary>
    private void BuildRibbon()
    {
        _insertionControls.Clear();
        _formattingControls.Clear();
        _status = new();
        var panel = new global::Avalonia.Controls.StackPanel { Spacing = GroupSpacing };
        var commands = new WrapPanel();
        panel.Children.Add(commands);
        if (Editor is not { } editor)
        {
            Content = panel;
            return;
        }

        AddCommand(commands, "Undo", editor.UndoCommand);
        AddCommand(commands, "Redo", editor.RedoCommand);
        AddCommand(commands, "Cut", editor.CutCommand);
        AddCommand(commands, "Copy", editor.CopyCommand);
        AddCommand(commands, "Paste", editor.PasteCommand);
        AddCommand(commands, "Select all", editor.SelectAllCommand);
        AddCommand(commands, "Bold", editor.ToggleBoldCommand);
        AddCommand(commands, "Italic", editor.ToggleItalicCommand);
        AddCommand(commands, "Underline", editor.ToggleUnderlineCommand);
        AddCommand(commands, "Strike", editor.ToggleStrikethroughCommand);
        AddCommand(commands, "Clear", editor.ClearFormattingCommand);
        var formatting = new WrapPanel();
        panel.Children.Add(formatting);
        AddChoices(formatting, "Font", ["Arial", "Calibri", "Georgia", "Consolas"], editor.SetFontFamilyCommand);
        AddChoices(formatting, "Size", [BodyFontSize, NormalFontSize, HeadingFontSize, TitleFontSize, DisplayFontSize], editor.SetFontSizeCommand);
        AddChoices(formatting, "Text color", [Colors.Black, Colors.DarkBlue, Colors.DarkRed, Colors.DarkGreen], editor.SetForegroundCommand);
        AddChoices(formatting, "Highlight", [Colors.Transparent, Colors.Yellow, Colors.LightBlue, Colors.LightGreen], editor.SetHighlightCommand);
        panel.Children.Add(CreateParagraphGroup());
        var insert = new WrapPanel();
        panel.Children.Add(insert);
        var address = new RibbonTextBox { PlaceholderText = "https://example.com", Width = AddressWidth, Margin = new(ButtonMargin) };
        insert.Children.Add(address);
        _insertionControls.Add(address);
        AddInsertion(insert, "Link", () =>
        {
            if (Uri.TryCreate(address.Text, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeMailto))
            {
                Editor?.InsertHyperlink(uri);
                _status.Text = string.Empty;
            }
            else
            {
                _status.Text = "Enter an absolute HTTP, HTTPS, or mail address.";
            }
        });
        AddInsertion(insert, "Table 3 × 3", () => Editor?.InsertTable(DefaultTableDimension, DefaultTableDimension));
        AddInsertion(insert, "Page break", () => Editor?.InsertPageBreak());
        var image = new RibbonButton { Content = "Image…", Focusable = false, Margin = new(ButtonMargin) };
        image.Click += OnImageClick;
        insert.Children.Add(image);
        _insertionControls.Add(image);
        panel.Children.Add(_status);
        Content = panel;
        RefreshInsertionState();
    }

    /// <summary>Creates paragraph alignment and list controls.</summary>
    /// <returns>The paragraph command group.</returns>
    private WrapPanel CreateParagraphGroup()
    {
        var paragraphs = new WrapPanel();
        AddInsertion(paragraphs, "Align left", () => Editor?.SetSelectionParagraphAlignment(TextAlignment.Left));
        AddInsertion(paragraphs, "Center", () => Editor?.SetSelectionParagraphAlignment(TextAlignment.Center));
        AddInsertion(paragraphs, "Align right", () => Editor?.SetSelectionParagraphAlignment(TextAlignment.Right));
        AddInsertion(paragraphs, "Justify", () => Editor?.SetSelectionParagraphAlignment(TextAlignment.Justify));
        AddInsertion(paragraphs, "Bullets", () => Editor?.ToggleBulletedList());
        AddInsertion(paragraphs, "Numbering", () => Editor?.ToggleNumberedList());
        _formattingControls.AddRange(paragraphs.Children);

        return paragraphs;
    }

    /// <summary>Adds a document insertion action.</summary>
    /// <param name="panel">The group.</param>
    /// <param name="label">The button label.</param>
    /// <param name="action">The insertion action.</param>
    private void AddInsertion(Panel panel, string label, Action action)
    {
        var button = new RibbonButton { Content = label, Focusable = false, Margin = new(ButtonMargin) };
        button.Click += (_, _) =>
        {
            action();
            _ = Editor?.Focus();
        };
        panel.Children.Add(button);
        _insertionControls.Add(button);
    }

    /// <summary>Loads an image selected through the platform file picker.</summary>
    /// <param name="sender">The image button.</param>
    /// <param name="e">The click event.</param>
    private async void OnImageClick(object? sender, RoutedEventArgs e)
    {
        if (Editor is not { } editor || TopLevel.GetTopLevel(this)?.StorageProvider is not { CanOpen: true } storage)
        {
            return;
        }

        var start = editor.SelectionStart;
        var end = editor.SelectionEnd;
        var options = new FilePickerOpenOptions { Title = "Insert image", AllowMultiple = false, FileTypeFilter = [FilePickerFileTypes.ImageAll] };
        var files = await storage.OpenFilePickerAsync(options).ConfigureAwait(true);
        if (files.Count == 0 || !ReferenceEquals(Editor, editor))
        {
            return;
        }

        try
        {
            await using var stream = await files[0].OpenReadAsync().ConfigureAwait(true);
            await using var bytes = new MemoryStream();
            await stream.CopyToAsync(bytes).ConfigureAwait(true);
            var extension = Path.GetExtension(files[0].Name).ToLowerInvariant();
            var mime = extension switch { ".jpg" or ".jpeg" => "image/jpeg", ".gif" => "image/gif", ".webp" => "image/webp", ".bmp" => "image/bmp", _ => "image/png" };
            if (!ReferenceEquals(Editor, editor))
            {
                return;
            }

            _ = editor.Focus();
            editor.Select(start, end - start);
            editor.ReplaceSelectionWithHtml($"<img src=\"data:{mime};base64,{Convert.ToBase64String(bytes.ToArray())}\" />");
            _status.Text = string.Empty;
        }
        catch (IOException exception)
        {
            _status.Text = $"The image could not be read: {exception.Message}";
        }
    }
}
