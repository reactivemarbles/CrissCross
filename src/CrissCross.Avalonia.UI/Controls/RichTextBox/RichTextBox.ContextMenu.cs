// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides the ContextMenu members for <see cref="RichTextBox"/>.</summary>
public partial class RichTextBox
{
    /// <summary>Provides the CreateContextMenu member.</summary>
    /// <returns>The result.</returns>
    private ContextMenu CreateContextMenu()
    {
        _contextMenu = new();
        _contextMenu.Opened += (_, _) => UpdateContextMenuState();
        _contextMenu.ItemsSource = CreateContextMenuItems();
        ContextMenu = _contextMenu;
        return _contextMenu;
    }

    /// <summary>Creates the context menu item hierarchy.</summary>
    /// <returns>The editor context menu items.</returns>
    private object[] CreateContextMenuItems() =>
        [
            RichTextMenuItemFactory.CreateGestureMenuItem("Cut", Key.X, CutCommand),
            RichTextMenuItemFactory.CreateGestureMenuItem("Copy", Key.C, CopyCommand),
            RichTextMenuItemFactory.CreateGestureMenuItem("Paste", Key.V, PasteCommand),
            new Separator(),
            RichTextMenuItemFactory.CreateGestureMenuItem("Select All", Key.A, SelectAllCommand),
            new Separator(),
            RichTextMenuItemFactory.CreateGestureMenuItem("Undo", Key.Z, UndoCommand),
            RichTextMenuItemFactory.CreateGestureMenuItem("Redo", Key.Y, RedoCommand),
            new Separator(),
            RichTextMenuItemFactory.CreateGestureMenuItem("Bold", Key.B, ToggleBoldCommand),
            RichTextMenuItemFactory.CreateGestureMenuItem("Italic", Key.I, ToggleItalicCommand),
            RichTextMenuItemFactory.CreateGestureMenuItem("Underline", Key.U, ToggleUnderlineCommand),
            RichTextMenuItemFactory.CreateCommandMenuItem("Strikethrough", ToggleStrikethroughCommand),
            RichTextMenuItemFactory.CreateChoiceMenuItem(
                "Font",
                SetFontFamilyCommand,
                [("Consolas", "Consolas"), ("Segoe UI", "Segoe UI"), ("Times New Roman", "Times New Roman")]),
            RichTextMenuItemFactory.CreateChoiceMenuItem(
                "Font Size",
                SetFontSizeCommand,
                [("12", SmallFontSize), ("16", MediumFontSize), ("20", LargeFontSize)]),
            RichTextMenuItemFactory.CreateChoiceMenuItem(
                "Foreground",
                SetForegroundCommand,
                [("White", Colors.White), ("DeepSkyBlue", Colors.DeepSkyBlue), ("Orange", Colors.Orange)]),
            RichTextMenuItemFactory.CreateChoiceMenuItem(
                "Highlight",
                SetHighlightCommand,
                [("Transparent", Colors.Transparent), ("Yellow", Colors.Yellow), ("LightGreen", Colors.LightGreen)]),
            new Separator(),
            RichTextMenuItemFactory.CreateCommandMenuItem("Clear Formatting", ClearFormattingCommand),
        ];

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
    private bool IsContextMenuItemEnabled(
        global::Avalonia.Controls.MenuItem menuItem,
        bool hasSelection,
        bool canEdit)
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
                _ => true,
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
}
