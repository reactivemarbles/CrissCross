// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Threading;
using DemoRichTextBox = CrissCross.Avalonia.UI.Controls.RichTextBox;

namespace RichTextBoxParity.AvaloniaDemo;

/// <summary>Main window for the Avalonia RichTextBox parity demo.</summary>
public partial class MainWindow : Window
{
    /// <summary>State refresh interval in milliseconds.</summary>
    private const int StateRefreshMilliseconds = 500;

    /// <summary>Maximum selected-text preview length.</summary>
    private const int PreviewMaxLength = 48;

    /// <summary>Length retained before appending preview ellipsis.</summary>
    private const int PreviewTrimmedLength = 45;

    /// <summary>Refreshes observable editor and clipboard state.</summary>
    private readonly DispatcherTimer _stateTimer;

    /// <summary>The RichTextBox under test.</summary>
    private DemoRichTextBox _editor = null!;

    /// <summary>Displays document state.</summary>
    private TextBlock _textStateText = null!;

    /// <summary>Displays selection state.</summary>
    private TextBlock _selectionStateText = null!;

    /// <summary>Displays command state.</summary>
    private TextBlock _commandStateText = null!;

    /// <summary>Displays platform clipboard state.</summary>
    private TextBlock _clipboardStateText = null!;

    /// <summary>Displays drag-and-drop state.</summary>
    private TextBlock _dropStateText = null!;

    /// <summary>Displays the latest interaction.</summary>
    private TextBlock _lastActionText = null!;

    /// <summary>Displays the selected text.</summary>
    private TextBox _selectedTextPreview = null!;

    /// <summary>The latest drag-and-drop observation.</summary>
    private string _dropState = "Waiting for drag/drop input.";

    /// <summary>The latest user action.</summary>
    private string _lastAction = "Ready.";

    /// <summary>The latest platform clipboard observation.</summary>
    private string _clipboardState = "Not inspected.";

    /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
    public MainWindow()
    {
        InitializeComponent();
        ResolveControls();
        ConfigureEditor();
        WireToolbar();

        _stateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(StateRefreshMilliseconds) };
        _stateTimer.Tick += StateTimerTick;
    }

    /// <inheritdoc/>
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        _stateTimer.Start();
        RefreshState();
        _ = RefreshClipboardStateAsync();
    }

    /// <summary>Counts logical line breaks in a plain-text value.</summary>
    /// <param name="text">The text to inspect.</param>
    /// <returns>The line count.</returns>
    private static int CountLines(string text)
    {
        if (text.Length == 0)
        {
            return 0;
        }

        var count = 1;
        foreach (var character in text)
        {
            if (character == '\n')
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>Normalizes and truncates a value for compact state display.</summary>
    /// <param name="text">The text to display.</param>
    /// <returns>A compact display string.</returns>
    private static string TrimForDisplay(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var normalized = text.Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);
        return normalized.Length <= PreviewMaxLength ? normalized : normalized[..PreviewTrimmedLength] + "...";
    }

    /// <summary>Resolves named controls from the loaded XAML tree.</summary>
    private void ResolveControls()
    {
        _editor = this.Get<DemoRichTextBox>(nameof(Editor));
        _textStateText = this.Get<TextBlock>(nameof(TextStateText));
        _selectionStateText = this.Get<TextBlock>(nameof(SelectionStateText));
        _commandStateText = this.Get<TextBlock>(nameof(CommandStateText));
        _clipboardStateText = this.Get<TextBlock>(nameof(ClipboardStateText));
        _dropStateText = this.Get<TextBlock>(nameof(DropStateText));
        _lastActionText = this.Get<TextBlock>(nameof(LastActionText));
        _selectedTextPreview = this.Get<TextBox>(nameof(SelectedTextPreview));
    }

    /// <summary>Configures the RichTextBox with sample content and event reporting.</summary>
    private void ConfigureEditor()
    {
        _editor.SetHtml(
            "<p><strong>RichTextBox parity baseline</strong></p>"
            + "<p>Select text, apply formatting, cut/copy/paste, undo/redo, or drop text and files here.</p>"
            + "<p>The editor remains in rendered-text coordinates while preserving formatting outside the changed "
            + "range.</p>");
        _editor.SelectionChanged += (_, _) => UpdateAction("Selection changed.");
        _editor.TextChanged += (_, _) => UpdateAction("Text changed.");
        _editor.FormattingApplied += (_, args) =>
            UpdateAction($"{args.FormatType} formatting applied to '{TrimForDisplay(args.AffectedText)}'.");
        _editor.AddHandler(
            DragDrop.DragOverEvent,
            OnDragOver,
            RoutingStrategies.Tunnel | RoutingStrategies.Bubble,
            true);
        _editor.AddHandler(DragDrop.DropEvent, OnDrop, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
    }

    /// <summary>Connects toolbar buttons to the RichTextBox command surface.</summary>
    private void WireToolbar()
    {
        Wire(nameof(BoldButton), () => Execute("Bold", _editor.ToggleBold));
        Wire(nameof(ItalicButton), () => Execute("Italic", _editor.ToggleItalic));
        Wire(nameof(UnderlineButton), () => Execute("Underline", _editor.ToggleUnderline));
        Wire(nameof(StrikeButton), () => Execute("Strikethrough", _editor.ToggleStrikethrough));
        Wire(nameof(ClearFormattingButton), () => Execute("Clear formatting", _editor.ClearFormatting));
        Wire(nameof(UndoButton), () => Execute("Undo", _editor.Undo));
        Wire(nameof(RedoButton), () => Execute("Redo", _editor.Redo));
        WireTask(nameof(CutButton), () => ExecuteAsync("Cut", _editor.CutToClipboardAsync));
        WireTask(nameof(CopyButton), () => ExecuteAsync("Copy", _editor.CopyToClipboardAsync));
        WireTask(nameof(PasteButton), () => ExecuteAsync("Paste", _editor.PasteFromClipboardAsync));
        Wire(nameof(SelectAllButton), () => Execute("Select all", _editor.SelectAll));
        Wire(nameof(ResetButton), ResetEditor);
        WireTask(nameof(SeedClipboardButton), SeedClipboardAsync);
        Wire(nameof(SimulateDropButton), SimulateTextDrop);
        Wire(nameof(InsertHtmlButton), InsertHtmlSample);
        this.Get<Border>(nameof(DragSource)).PointerPressed += DragSourcePointerPressed;
    }

    /// <summary>Connects a button click to a synchronous action.</summary>
    /// <param name="buttonName">The XAML button name.</param>
    /// <param name="action">The action to run.</param>
    private void Wire(string buttonName, Action action)
    {
        var button = this.Get<Button>(buttonName);
        button.Click += (_, _) => action();
    }

    /// <summary>Connects a button click to an asynchronous action.</summary>
    /// <param name="buttonName">The XAML button name.</param>
    /// <param name="action">The action to run.</param>
    private void WireTask(string buttonName, Func<Task> action)
    {
        var button = this.Get<Button>(buttonName);
        button.Click += async (_, _) => await action().ConfigureAwait(true);
    }

    /// <summary>Runs an editor command and records the visible action state.</summary>
    /// <param name="actionName">The command display name.</param>
    /// <param name="action">The command action.</param>
    private void Execute(string actionName, Action action)
    {
        action();
        UpdateAction($"{actionName} command invoked.");
    }

    /// <summary>Runs an asynchronous editor command and records the visible action state.</summary>
    /// <param name="actionName">The command display name.</param>
    /// <param name="action">The command action.</param>
    /// <returns>A task that completes after the command and state refresh.</returns>
    private async Task ExecuteAsync(string actionName, Func<Task> action)
    {
        await action().ConfigureAwait(true);
        UpdateAction($"{actionName} command completed.");
        await RefreshClipboardStateAsync().ConfigureAwait(true);
    }

    /// <summary>Resets the editor to the shared comparison text.</summary>
    private void ResetEditor()
    {
        _editor.SetHtml(
            "<p><strong>RichTextBox parity baseline</strong></p>"
            + "<p>Type, select, format, copy, paste, undo, and drop content here.</p>");
        UpdateAction("Editor reset.");
    }

    /// <summary>Seeds the actual platform clipboard.</summary>
    /// <returns>A task that completes when the platform clipboard has been updated.</returns>
    private async Task SeedClipboardAsync()
    {
        var clipboard = TopLevel.GetTopLevel(_editor)?.Clipboard;
        if (clipboard is null)
        {
            UpdateAction("Platform clipboard unavailable.");
            return;
        }

        await clipboard.SetTextAsync($"Seeded platform clipboard text at {DateTime.Now:T}.").ConfigureAwait(true);
        UpdateAction("Platform clipboard seeded with plain text.");
        await RefreshClipboardStateAsync().ConfigureAwait(true);
    }

    /// <summary>Exercises the RichTextBox drop insertion path.</summary>
    private void SimulateTextDrop()
    {
        var inserted = _editor.TryDropText($"Dropped text at {DateTime.Now:T}.");
        _dropState = inserted ? "Simulated text drop inserted content." : "Simulated text drop was rejected.";
        UpdateAction("Simulated drop probe invoked.");
    }

    /// <summary>Starts a platform text drag for comparison with the WPF harness.</summary>
    /// <param name="sender">The drag source.</param>
    /// <param name="e">The pointer press that begins the drag.</param>
    private async void DragSourcePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        const string dragText = "Dropped parity text from Avalonia";
        var item = new DataTransferItem();
        item.SetText(dragText);
        if (OperatingSystem.IsWindows())
        {
            item.Set(DataFormat.CreateStringPlatformFormat("UnicodeText"), dragText);
            item.Set(DataFormat.CreateStringPlatformFormat("Text"), dragText);
        }

        using var transfer = new DataTransfer();
        transfer.Add(item);

        var effect = await DragDrop.DoDragDropAsync(e, transfer, DragDropEffects.Copy).ConfigureAwait(true);
        UpdateAction($"Drag source completed with {effect}.");
    }

    /// <summary>Inserts a formatted HTML fragment at the current selection.</summary>
    private void InsertHtmlSample()
    {
        _editor.ReplaceSelectionWithHtml("<em>Avalonia HTML fragment</em>");
        UpdateAction("Inserted HTML sample at the current selection.");
    }

    /// <summary>Reports drag-over state for external drag/drop comparisons.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The drag event arguments.</param>
    private void OnDragOver(object? sender, DragEventArgs e)
    {
        var text = e.DataTransfer.TryGetText();
        _dropState = string.IsNullOrWhiteSpace(text)
            ? $"Drag over: effects={e.DragEffects}, text=no."
            : $"Drag over: effects={e.DragEffects}, text='{TrimForDisplay(text)}'.";
        RefreshState();
    }

    /// <summary>Reports drop state for external drag/drop comparisons.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The drop event arguments.</param>
    private void OnDrop(object? sender, DragEventArgs e)
    {
        var text = e.DataTransfer.TryGetText();
        _dropState = string.IsNullOrWhiteSpace(text)
            ? $"Drop observed: effects={e.DragEffects}, text=no."
            : $"Drop observed: effects={e.DragEffects}, text='{TrimForDisplay(text)}'.";
        UpdateAction("Drop interaction observed.");
    }

    /// <summary>Refreshes editor and clipboard state on the timer.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private async void StateTimerTick(object? sender, EventArgs e)
    {
        RefreshState();
        await RefreshClipboardStateAsync().ConfigureAwait(true);
    }

    /// <summary>Refreshes the platform clipboard summary.</summary>
    /// <returns>A task that completes after the clipboard has been inspected.</returns>
    private async Task RefreshClipboardStateAsync()
    {
        var clipboard = TopLevel.GetTopLevel(_editor)?.Clipboard;
        if (clipboard is null)
        {
            _clipboardState = "unavailable";
            RefreshState();
            return;
        }

        using var transfer = await clipboard.TryGetDataAsync().ConfigureAwait(true);
        _clipboardState = transfer is null
            ? "empty"
            : string.Join(", ", transfer.Formats.Select(format => format.Identifier));
        RefreshState();
    }

    /// <summary>Updates the last-action label and refreshes visible state.</summary>
    /// <param name="action">The action description.</param>
    private void UpdateAction(string action)
    {
        _lastAction = action;
        RefreshState();
    }

    /// <summary>Refreshes all visible state labels.</summary>
    private void RefreshState()
    {
        var text = _editor.PlainText;
        _textStateText.Text = $"length={text.Length}, caret={_editor.CaretIndex}, lines={CountLines(text)}";
        _selectionStateText.Text =
            $"start={_editor.SelectionStart}, end={_editor.SelectionEnd}, length={_editor.SelectionLength}, "
            + $"text='{TrimForDisplay(_editor.SelectedText)}'";
        _commandStateText.Text =
            $"copy={_editor.CanCopy}, cut={_editor.CanCut}, paste={_editor.CanPaste}, "
            + $"undo={_editor.CanUndo}, redo={_editor.CanRedo}";
        _clipboardStateText.Text = _clipboardState;
        _dropStateText.Text = _dropState;
        _lastActionText.Text = _lastAction;
        _selectedTextPreview.Text = _editor.SelectedText;
    }
}
