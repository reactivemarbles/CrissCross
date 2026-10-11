// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls.Templates;
using Avalonia.Threading;
using CrissCross.Avalonia.UI.Controls;
using EditingTextBox = Avalonia.Controls.TextBox;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies actual composed TextBox edits retain rich insertion and revision behavior.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextBoxContinuousEditingTests
{
    /// <summary>Character commands at the caret affect continuous typing.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ContinuousTyping_RetainsPendingBoldAndHistory()
    {
        var input = new EditingTextBox();
        var editor = CreateEditor(input);
        editor.ToggleBold();
        input.Text = "a";
        await Dispatcher.UIThread.InvokeAsync(static () => { }, DispatcherPriority.Background);
        await Assert.That(editor.PlainText).IsEqualTo("a");
        await Assert.That(editor.Document.Segments[0].IsBold).IsTrue();
        editor.Undo();
        await Assert.That(editor.PlainText).IsEmpty();
    }

    /// <summary>Native continuous typing creates one reviewable insertion and one undo entry.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ContinuousTyping_TracksInsertedText()
    {
        var input = new EditingTextBox();
        var editor = CreateEditor(input);
        editor.TrackChanges = true;
        editor.RevisionAuthor = "Editor";
        input.Text = "typed";
        await Dispatcher.UIThread.InvokeAsync(static () => { }, DispatcherPriority.Background);
        await Assert.That(editor.Html).Contains("<ins");
        await Assert.That(editor.Html).Contains("data-author=\"Editor\"");
        await Assert.That(editor.PlainText).IsEqualTo("typed");
        editor.Undo();
        await Assert.That(editor.PlainText).IsEmpty();
        await Assert.That(editor.CanUndo).IsFalse();
    }

    /// <summary>Applies the same composed editing part used by the production template.</summary>
    /// <param name="input">The native Avalonia input control.</param>
    /// <returns>The initialized rich editor.</returns>
    private static RichTextBox CreateEditor(EditingTextBox input)
    {
        var template = new FuncControlTemplate<RichTextBox>((_, scope) =>
            {
                scope.Register("PART_TextBox", input);
                return input;
            });
        var editor = new RichTextBox { Template = template };
        editor.ApplyTemplate();
        return editor;
    }
}
