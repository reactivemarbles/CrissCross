// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace RichTextBoxParity.WpfDemo;

/// <summary>Interaction window for the native WPF RichTextBox parity demo.</summary>
public partial class MainWindow : Window
{
    /// <summary>Milliseconds between clipboard availability polls.</summary>
    private const int ClipboardPollIntervalMilliseconds = 750;

    /// <summary>Maximum selected text preview length shown in state output.</summary>
    private const int SelectedTextPreviewMaxLength = 80;

    /// <summary>Maximum rendered width for an inline dropped image.</summary>
    private const int DroppedImageMaxWidth = 640;

    /// <summary>Maximum rendered height for an inline dropped image.</summary>
    private const int DroppedImageMaxHeight = 480;

    /// <summary>Uniform margin for inline dropped images.</summary>
    private const int DroppedImageMargin = 2;

    /// <summary>Suffix appended to truncated selected text previews.</summary>
    private const string PreviewEllipsis = "...";

    /// <summary>Image extensions that WPF can decode for inline file drops.</summary>
    private static readonly HashSet<string> SupportedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".bmp",
        ".gif",
        ".ico",
        ".jpg",
        ".jpeg",
        ".png",
        ".tif",
        ".tiff",
        ".wdp"
    };

    /// <summary>Timer used to refresh clipboard availability state.</summary>
    private readonly DispatcherTimer _clipboardTimer;

    /// <summary>Last editor command that used the clipboard.</summary>
    private string _lastClipboardAction = "none";

    /// <summary>Last observed drag/drop interaction state.</summary>
    private string _lastDropState = "none";

    /// <summary>Indicates that all XAML named elements are available.</summary>
    private bool _isReady;

    /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
    public MainWindow()
    {
        InitializeComponent();
        _isReady = true;

        _clipboardTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(ClipboardPollIntervalMilliseconds)
        };
        _clipboardTimer.Tick += ClipboardTimerTick;
        _clipboardTimer.Start();

        AddLog("Demo started");
        UpdateState("startup");
    }

    /// <summary>Returns whether a drag/drop payload contains data this demo can process.</summary>
    /// <param name="data">The drag/drop data object.</param>
    /// <returns><see langword="true"/> when supported drop data is present; otherwise, <see langword="false"/>.</returns>
    private static bool HasSupportedDropData(IDataObject data) =>
        data.GetDataPresent(DataFormats.UnicodeText) ||
        data.GetDataPresent(DataFormats.Text) ||
        data.GetDataPresent(DataFormats.FileDrop);

    /// <summary>Builds a display string containing the data formats on a drag/drop payload.</summary>
    /// <param name="data">The drag/drop data object.</param>
    /// <returns>A comma-separated format list.</returns>
    private static string DescribeDataObject(IDataObject data)
    {
        string[] formats = data.GetFormats(autoConvert: true);
        return formats.Length == 0 ? "no formats" : string.Join(", ", formats);
    }

    /// <summary>Builds a display string describing current clipboard formats.</summary>
    /// <returns>A compact clipboard format summary.</returns>
    private static string DescribeClipboard()
    {
        try
        {
            IDataObject? dataObject = Clipboard.GetDataObject();
            if (dataObject is null)
            {
                return "empty/unavailable";
            }

            bool hasText = dataObject.GetDataPresent(DataFormats.UnicodeText);
            bool hasRtf = dataObject.GetDataPresent(DataFormats.Rtf);
            bool hasXaml = dataObject.GetDataPresent(DataFormats.Xaml);
            bool hasHtml = dataObject.GetDataPresent(DataFormats.Html);
            bool hasFiles = dataObject.GetDataPresent(DataFormats.FileDrop);
            return $"text={hasText}; rtf={hasRtf}; xaml={hasXaml}; html={hasHtml}; files={hasFiles}";
        }
        catch (ExternalException ex)
        {
            return $"unavailable ({ex.GetType().Name})";
        }
        catch (InvalidOperationException ex)
        {
            return $"unavailable ({ex.GetType().Name})";
        }
    }

    /// <summary>Counts logical lines in a plain text snapshot.</summary>
    /// <param name="text">The plain text snapshot.</param>
    /// <returns>The logical line count.</returns>
    private static int CountLines(string text) => text.Length == 0 ? 0 : text.Split(Environment.NewLine, StringSplitOptions.None).Length;

    /// <summary>Returns a compact escaped preview for selected text.</summary>
    /// <param name="text">The selected text.</param>
    /// <returns>A display-safe selected text preview.</returns>
    private static string Preview(string text)
    {
        string compact = text.Replace(Environment.NewLine, "\\n", StringComparison.Ordinal);
        return compact.Length <= SelectedTextPreviewMaxLength
            ? compact
            : compact[..(SelectedTextPreviewMaxLength - PreviewEllipsis.Length)] + PreviewEllipsis;
    }

    /// <summary>Returns whether the file is a supported image file for inline rendering.</summary>
    /// <param name="file">The dropped file path.</param>
    /// <returns><see langword="true"/> when the file is a decodable image candidate; otherwise, <see langword="false"/>.</returns>
    private static bool IsSupportedImageFile(string file)
    {
        string extension = Path.GetExtension(file);
        return File.Exists(file) && SupportedImageExtensions.Contains(extension);
    }

    /// <summary>Creates a WPF image element for a dropped image file.</summary>
    /// <param name="file">The image file path.</param>
    /// <returns>An image element ready to insert into the flow document.</returns>
    private static Image CreateDroppedImage(string file)
    {
        BitmapImage bitmap = new();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = new(file, UriKind.Absolute);
        bitmap.EndInit();
        bitmap.Freeze();

        return new Image
        {
            Source = bitmap,
            MaxWidth = DroppedImageMaxWidth,
            MaxHeight = DroppedImageMaxHeight,
            Margin = new(DroppedImageMargin),
            Stretch = Stretch.Uniform,
            StretchDirection = StretchDirection.DownOnly,
            ToolTip = Path.GetFileName(file)
        };
    }

    /// <summary>Handles all toolbar buttons by command parameter.</summary>
    /// <param name="sender">The toolbar button.</param>
    /// <param name="e">The routed event arguments.</param>
    private void ToolbarButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { CommandParameter: string action })
        {
            AddLog("Unknown toolbar action");
            UpdateState("unknown toolbar action");
            return;
        }

        if (TryApplyDirectFormatting(action))
        {
            e.Handled = true;
            return;
        }

        (RoutedCommand? command, string commandName, bool reset) = action switch
        {
            "Bold" => (EditingCommands.ToggleBold, "bold", false),
            "Italic" => (EditingCommands.ToggleItalic, "italic", false),
            "Underline" => (EditingCommands.ToggleUnderline, "underline", false),
            "Undo" => (ApplicationCommands.Undo, "undo", false),
            "Redo" => (ApplicationCommands.Redo, "redo", false),
            "Cut" => (ApplicationCommands.Cut, "cut", false),
            "Copy" => (ApplicationCommands.Copy, "copy", false),
            "Paste" => (ApplicationCommands.Paste, "paste", false),
            "SelectAll" => (ApplicationCommands.SelectAll, "select all", false),
            "Reset" => (null, "reset", true),
            _ => (null, action, false)
        };

        if (reset)
        {
            ResetEditorText();
        }
        else if (command is not null)
        {
            ExecuteEditorCommand(command, commandName);
        }
        else
        {
            AddLog($"Unknown toolbar action: {commandName}");
            UpdateState("unknown toolbar action");
        }

        e.Handled = true;
    }

    /// <summary>Applies formatting actions that do not have a WPF editing command.</summary>
    /// <param name="action">The formatting action.</param>
    /// <returns><see langword="true"/> when the action was handled.</returns>
    private bool TryApplyDirectFormatting(string action)
    {
        if (action == "Strike")
        {
            Editor.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Strikethrough);
        }
        else if (action == "ClearFormatting")
        {
            Editor.Selection.ClearAllProperties();
        }
        else
        {
            return false;
        }

        AddLog(action);
        UpdateState(action);
        return true;
    }

    /// <summary>Starts a real OLE text drag for comparison with the Avalonia harness.</summary>
    /// <param name="sender">The drag source.</param>
    /// <param name="e">The mouse event.</param>
    private void DragSourceMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var data = new DataObject(DataFormats.UnicodeText, "Dropped parity text from WPF");
        _ = DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Copy);
        e.Handled = true;
    }

    /// <summary>Resets the editor text to a simple baseline paragraph.</summary>
    private void ResetEditorText()
    {
        Editor.Document.Blocks.Clear();
        Editor.Document.Blocks.Add(new Paragraph(new Run("Reset WPF RichTextBox content. Type or paste here.")));
        AddLog("Reset text");
        UpdateState("reset");
        _ = Editor.Focus();
    }

    /// <summary>Records keyboard focus entering or leaving the editor.</summary>
    /// <param name="sender">The editor raising the focus event.</param>
    /// <param name="e">The keyboard focus event arguments.</param>
    private void EditorFocusChanged(object sender, KeyboardFocusChangedEventArgs e)
    {
        string state = e.RoutedEvent == Keyboard.GotKeyboardFocusEvent ? "focus gained" : "focus lost";
        AddLog(state);
        UpdateState(state);
    }

    /// <summary>Refreshes state after the editor selection changes.</summary>
    /// <param name="sender">The editor raising the selection event.</param>
    /// <param name="e">The routed event arguments.</param>
    private void EditorSelectionChanged(object sender, RoutedEventArgs e) => UpdateState("selection changed");

    /// <summary>Refreshes state after the editor text changes.</summary>
    /// <param name="sender">The editor raising the text event.</param>
    /// <param name="e">The text change event arguments.</param>
    private void EditorTextChanged(object sender, TextChangedEventArgs e) => UpdateState("text changed");

    /// <summary>Records drag enter data formats.</summary>
    /// <param name="sender">The editor receiving dragged data.</param>
    /// <param name="e">The drag event arguments.</param>
    private void EditorDragEnter(object sender, DragEventArgs e)
    {
        _lastDropState = $"drag enter: {DescribeDataObject(e.Data)}";
        AddLog(_lastDropState);
        UpdateState("drag enter");
    }

    /// <summary>Records drag leave state.</summary>
    /// <param name="sender">The editor losing dragged data.</param>
    /// <param name="e">The drag event arguments.</param>
    private void EditorDragLeave(object sender, DragEventArgs e)
    {
        _lastDropState = "drag leave";
        AddLog(_lastDropState);
        UpdateState("drag leave");
    }

    /// <summary>Applies copy effects for supported drag data while reporting current formats.</summary>
    /// <param name="sender">The editor receiving dragged data.</param>
    /// <param name="e">The drag event arguments.</param>
    private void EditorPreviewDragOver(object sender, DragEventArgs e)
    {
        e.Effects = HasSupportedDropData(e.Data) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
        _lastDropState = $"drag over: {DescribeDataObject(e.Data)}; effect={e.Effects}";
        UpdateState("drag over");
    }

    /// <summary>Handles dropped text or files and reports the observed payload.</summary>
    /// <param name="sender">The editor receiving dropped data.</param>
    /// <param name="e">The drag event arguments.</param>
    private void EditorDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop) && e.Data.GetData(DataFormats.FileDrop) is string[] files)
        {
            int insertedImages = InsertDroppedImages(files, GetDropInsertionPosition(e));
            _lastDropState = insertedImages == 0
                ? $"dropped files: {files.Length}"
                : $"dropped files: {files.Length}; inserted images: {insertedImages}";
        }
        else if (e.Data.GetDataPresent(DataFormats.UnicodeText))
        {
            Editor.Selection.Text = (e.Data.GetData(DataFormats.UnicodeText) as string) ?? string.Empty;
            _lastDropState = $"dropped text: {Editor.Selection.Text.Length} chars";
        }
        else
        {
            _lastDropState = $"dropped unsupported data: {DescribeDataObject(e.Data)}";
        }

        e.Handled = true;
        DropMirrorText.Text = _lastDropState;
        AddLog(_lastDropState);
        UpdateState("drop");
    }

    /// <summary>Gets the document insertion position closest to a drop point.</summary>
    /// <param name="e">The drag event arguments.</param>
    /// <returns>The text pointer where dropped inline content should be inserted.</returns>
    private TextPointer GetDropInsertionPosition(DragEventArgs e)
    {
        TextPointer? position = Editor.GetPositionFromPoint(e.GetPosition(Editor), snapToText: true);
        return position?.GetInsertionPosition(LogicalDirection.Forward) ?? Editor.CaretPosition;
    }

    /// <summary>Inserts supported image files as inline document elements.</summary>
    /// <param name="files">The dropped file paths.</param>
    /// <param name="insertionPosition">The initial insertion position.</param>
    /// <returns>The number of images inserted.</returns>
    private int InsertDroppedImages(IEnumerable<string> files, TextPointer insertionPosition)
    {
        int insertedImages = 0;
        TextPointer currentPosition = insertionPosition;

        foreach (string file in files)
        {
            if (!IsSupportedImageFile(file))
            {
                continue;
            }

            try
            {
                InlineUIContainer container = new(CreateDroppedImage(file), currentPosition);
                currentPosition = container.ElementEnd.GetInsertionPosition(LogicalDirection.Forward) ?? container.ElementEnd;
                Editor.CaretPosition = currentPosition;
                insertedImages++;
            }
            catch (IOException ex)
            {
                AddLog($"Skipped image {Path.GetFileName(file)} ({ex.GetType().Name})");
            }
            catch (InvalidOperationException ex)
            {
                AddLog($"Skipped image {Path.GetFileName(file)} ({ex.GetType().Name})");
            }
            catch (NotSupportedException ex)
            {
                AddLog($"Skipped image {Path.GetFileName(file)} ({ex.GetType().Name})");
            }
        }

        return insertedImages;
    }

    /// <summary>Refreshes visible clipboard state on a timer.</summary>
    /// <param name="sender">The timer raising the tick event.</param>
    /// <param name="e">The event arguments.</param>
    private void ClipboardTimerTick(object? sender, EventArgs e) => UpdateState("clipboard poll");

    /// <summary>Executes an editor routed command and records command availability.</summary>
    /// <param name="command">The command to execute against the editor.</param>
    /// <param name="action">The display name for the action.</param>
    private void ExecuteEditorCommand(RoutedCommand command, string action)
    {
        _ = Editor.Focus();
        if (command.CanExecute(null, Editor))
        {
            command.Execute(null, Editor);
            if (action is "cut" or "copy" or "paste")
            {
                _lastClipboardAction = action;
            }

            AddLog(action);
        }
        else
        {
            AddLog($"{action} unavailable");
        }

        UpdateState(action);
    }

    /// <summary>Updates the visible parity state panel.</summary>
    /// <param name="reason">The interaction reason to display.</param>
    private void UpdateState(string reason)
    {
        if (!_isReady)
        {
            return;
        }

        TextRange documentRange = new(Editor.Document.ContentStart, Editor.Document.ContentEnd);
        string plainText = documentRange.Text;
        TextRange selectionRange = new(Editor.Selection.Start, Editor.Selection.End);
        string selectedText = selectionRange.Text;
        int caretOffset = new TextRange(Editor.Document.ContentStart, Editor.CaretPosition).Text.Length;
        int selectionStart = new TextRange(Editor.Document.ContentStart, Editor.Selection.Start).Text.Length;
        int selectionEnd = new TextRange(Editor.Document.ContentStart, Editor.Selection.End).Text.Length;

        FocusStateText.Text = $"Reason: {reason}; keyboard focus={Editor.IsKeyboardFocusWithin}";
        TextStateText.Text = $"Text: chars={plainText.Length}; lines={CountLines(plainText)}; blocks={Editor.Document.Blocks.Count}";
        SelectionStateText.Text = $"Selection: start={selectionStart}; end={selectionEnd}; length={selectedText.Length}; caret={caretOffset}; text=\"{Preview(selectedText)}\"";
        FormattingStateText.Text = $"Formatting: bold={IsSelectionPropertyActive(TextElement.FontWeightProperty, FontWeights.Bold)}; italic={IsSelectionPropertyActive(TextElement.FontStyleProperty, FontStyles.Italic)}; underline={IsUnderlineActive()}";
        UndoStateText.Text = $"Undo: canUndo={Editor.CanUndo}; canRedo={Editor.CanRedo}";
        ClipboardStateText.Text = $"Clipboard: {DescribeClipboard()}; lastAction={_lastClipboardAction}";
        DropStateText.Text = $"Drop: {_lastDropState}";
        DropMirrorText.Text = _lastDropState;
    }

    /// <summary>Returns whether the current selection has the expected dependency property value.</summary>
    /// <param name="property">The dependency property to inspect.</param>
    /// <param name="expectedValue">The expected property value.</param>
    /// <returns><see langword="true"/> when the selection property matches; otherwise, <see langword="false"/>.</returns>
    private bool IsSelectionPropertyActive(DependencyProperty property, object expectedValue)
    {
        object value = Editor.Selection.GetPropertyValue(property);
        return value != DependencyProperty.UnsetValue && value.Equals(expectedValue);
    }

    /// <summary>Returns whether underline formatting is active at the current selection.</summary>
    /// <returns><see langword="true"/> when underline formatting is active; otherwise, <see langword="false"/>.</returns>
    private bool IsUnderlineActive()
    {
        object value = Editor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
        return value is TextDecorationCollection decorations && decorations.Count > 0;
    }

    /// <summary>Adds a timestamped message to the interaction log.</summary>
    /// <param name="message">The message to add.</param>
    private void AddLog(string message)
    {
        string entry = $"{DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} {message}";
        InteractionLog.Items.Insert(0, entry);
        while (InteractionLog.Items.Count > 40)
        {
            InteractionLog.Items.RemoveAt(InteractionLog.Items.Count - 1);
        }
    }
}
