// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Input;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies routed input edits the paged document.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextBoxPagedEditingTests
{
    /// <summary>The length of the replaced selection.</summary>
    private const int ReplacedSelectionLength = 5;

    /// <summary>The length of a three-unit supplementary Unicode character.</summary>
    private const int SupplementaryCharacterLength = 2;

    /// <summary>The expected second-line text length.</summary>
    private const int SecondLineLength = 6;

    /// <summary>The expected caret offset at the end of the ASCII prefix.</summary>
    private const int AsciiPrefixLength = 5;

    /// <summary>The expected caret offset after one supplementary character.</summary>
    private const int PrefixAndSupplementaryCharacterLength = 3;

    /// <summary>The selected range length in the ASCII fixture.</summary>
    private const int ThreeCharacterSelectionLength = 3;

    /// <summary>The start of a selected character after an ASCII prefix and separator.</summary>
    private const int FirstCharacterAfterPrefix = 1;

    /// <summary>The second paragraph's two-character caret offset.</summary>
    private const int SecondParagraphCaretOffset = 2;

    /// <summary>The insertion offset between the two page-break fixture words.</summary>
    private const int PageBreakInsertionOffset = 6;

    /// <summary>Verifies text input replaces rendered selections and supports history.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task TextInput_ReplacesFormattedSelectionAndSupportsUndoRedo()
    {
        var editor = new RichTextBox { IsPageLayoutEnabled = true };
        editor.SetHtml("<b>Hello</b> world");
        editor.Select(PageBreakInsertionOffset, ReplacedSelectionLength);
        var input = new TextInputEventArgs { RoutedEvent = InputElement.TextInputEvent, Text = "日本語 😀" };
        editor.RaiseEvent(input);
        await Assert.That(input.Handled).IsTrue();
        await Assert.That(editor.PlainText).IsEqualTo("Hello 日本語 😀");
        await Assert.That(editor.CaretIndex).IsEqualTo(editor.Document.Length);
        editor.Undo();
        await Assert.That(editor.Html).IsEqualTo("<b>Hello</b> world");
        editor.Redo();
        await Assert.That(editor.PlainText).IsEqualTo("Hello 日本語 😀");
    }

    /// <summary>Verifies read-only pages accept navigation without mutations.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ReadOnly_InputAndDeletionLeaveContentUnchanged()
    {
        var editor = new RichTextBox { IsPageLayoutEnabled = true, IsReadOnly = true, AcceptsReturn = true, AcceptsTab = true };
        editor.SetPlainText("Hello");
        editor.Select(FirstCharacterAfterPrefix, ThreeCharacterSelectionLength);
        editor.RaiseEvent(new TextInputEventArgs { RoutedEvent = InputElement.TextInputEvent, Text = "changed" });
        _ = Press(editor, Key.Back);
        _ = Press(editor, Key.Delete);
        _ = Press(editor, Key.Enter, KeyModifiers.Control);
        _ = Press(editor, Key.Tab);
        await Assert.That(editor.PlainText).IsEqualTo("Hello");
        _ = Press(editor, Key.End, KeyModifiers.Control);
        await Assert.That(editor.CaretIndex).IsEqualTo(AsciiPrefixLength);
    }

    /// <summary>Verifies shift movement maintains the selection anchor.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Navigation_ShiftExtendsAndReversesSelection()
    {
        var editor = new RichTextBox { IsPageLayoutEnabled = true };
        editor.SetPlainText("abcde");
        editor.Select(SupplementaryCharacterLength, 0);
        _ = Press(editor, Key.Right, KeyModifiers.Shift);
        _ = Press(editor, Key.Right, KeyModifiers.Shift);
        await Assert.That(editor.SelectedText).IsEqualTo("cd");
        _ = Press(editor, Key.Left, KeyModifiers.Shift);
        _ = Press(editor, Key.Left, KeyModifiers.Shift);
        _ = Press(editor, Key.Left, KeyModifiers.Shift);
        await Assert.That(editor.SelectedText).IsEqualTo("b");
        await Assert.That(editor.CaretIndex).IsEqualTo(FirstCharacterAfterPrefix);
    }

    /// <summary>Verifies Home and End find paragraph and document boundaries.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Navigation_HomeEndRespectParagraphAndControlModifier()
    {
        var editor = new RichTextBox { IsPageLayoutEnabled = true };
        editor.SetPlainText("first\nsecond\nthird");
        var secondStart = AsciiPrefixLength + Environment.NewLine.Length;
        editor.Select(secondStart + SecondParagraphCaretOffset, 0);
        _ = Press(editor, Key.Home);
        await Assert.That(editor.CaretIndex).IsEqualTo(secondStart);
        _ = Press(editor, Key.End);
        await Assert.That(editor.CaretIndex).IsEqualTo(secondStart + SecondLineLength);
        _ = Press(editor, Key.Home, KeyModifiers.Control);
        await Assert.That(editor.CaretIndex).IsEqualTo(0);
        _ = Press(editor, Key.End, KeyModifiers.Control);
        await Assert.That(editor.CaretIndex).IsEqualTo(editor.Document.Length);
    }

    /// <summary>Verifies deletion removes selected document content and remains undoable.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Delete_RemovesSelectionAndBackspaceRemovesPreviousCharacter()
    {
        var editor = new RichTextBox { IsPageLayoutEnabled = true };
        editor.SetPlainText("abcde");
        editor.Select(FirstCharacterAfterPrefix, ThreeCharacterSelectionLength);
        _ = Press(editor, Key.Delete);
        await Assert.That(editor.PlainText).IsEqualTo("ae");
        await Assert.That(editor.CaretIndex).IsEqualTo(FirstCharacterAfterPrefix);
        _ = Press(editor, Key.Back);
        await Assert.That(editor.PlainText).IsEqualTo("e");
        editor.Undo();
        await Assert.That(editor.PlainText).IsEqualTo("ae");
    }

    /// <summary>Verifies page breaks are inserted through the keyboard and history.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ControlEnter_InsertsPageBreakAndSupportsUndo()
    {
        var editor = new RichTextBox { IsPageLayoutEnabled = true, AcceptsReturn = true };
        editor.SetPlainText("beforeafter");
        editor.Select(PageBreakInsertionOffset, 0);
        var input = Press(editor, Key.Enter, KeyModifiers.Control);
        await Assert.That(input.Handled).IsTrue();
        await Assert.That(editor.Html).Contains("break-before:page");
        await Assert.That(editor.PlainText).Contains("before");
        await Assert.That(editor.PlainText).Contains("after");
        editor.Undo();
        await Assert.That(editor.PlainText).IsEqualTo("beforeafter");
    }

    /// <summary>Verifies navigation and deletion preserve supplementary Unicode characters.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task NavigationAndDeletion_DoNotSplitEmoji()
    {
        var editor = new RichTextBox { IsPageLayoutEnabled = true };
        editor.SetPlainText("a😀b");
        editor.Select(FirstCharacterAfterPrefix, 0);
        _ = Press(editor, Key.Right);
        await Assert.That(editor.CaretIndex).IsEqualTo(PrefixAndSupplementaryCharacterLength);
        _ = Press(editor, Key.Back);
        await Assert.That(editor.PlainText).IsEqualTo("ab");
        editor.Undo();
        editor.Select(FirstCharacterAfterPrefix, 0);
        _ = Press(editor, Key.Delete);
        await Assert.That(editor.PlainText).IsEqualTo("ab");
    }

    /// <summary>Raises a routed keyboard gesture.</summary>
    /// <param name="editor">The editor.</param>
    /// <param name="key">The key.</param>
    /// <param name="modifiers">The modifiers.</param>
    /// <returns>The routed arguments.</returns>
    private static KeyEventArgs Press(RichTextBox editor, Key key, KeyModifiers modifiers = KeyModifiers.None)
    {
        var args = new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = key, KeyModifiers = modifiers };
        editor.RaiseEvent(args);
        return args;
    }
}
