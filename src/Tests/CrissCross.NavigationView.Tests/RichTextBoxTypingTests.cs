// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Media;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies character formatting at a collapsed insertion position.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextBoxTypingTests
{
    /// <summary>Typing retains chosen formatting and participates in history.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task PendingBold_AppliesToSuccessiveInputAndUndo()
    {
        var editor = new RichTextBox();
        editor.ToggleBold();
        editor.ReplaceSelection("a");
        editor.ReplaceSelection("b");
        await Assert.That(editor.PlainText).IsEqualTo("ab");
        await Assert.That(editor.Document.Segments[0].IsBold).IsTrue();
        editor.Undo();
        await Assert.That(editor.PlainText).IsEqualTo("a");
        editor.Redo();
        await Assert.That(editor.PlainText).IsEqualTo("ab");
    }

    /// <summary>Independent decorations can be toggled without losing each other.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task PendingDecorations_ToggleIndependently()
    {
        var editor = new RichTextBox();
        editor.ToggleUnderline();
        editor.ToggleStrikethrough();
        editor.ToggleUnderline();
        editor.ReplaceSelection("text");
        await Assert.That(editor.Document.Segments[0].IsUnderline).IsFalse();
        await Assert.That(editor.Document.Segments[0].IsStrikethrough).IsTrue();
    }

    /// <summary>A collapsed toggle can override inherited bold without altering existing text.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task PendingToggle_CanDisableInheritedFormatting()
    {
        var editor = new RichTextBox();
        editor.SetHtml("<strong>a</strong>");
        editor.Select(editor.Document.Length, 0);
        editor.ToggleBold();
        editor.ReplaceSelection("b");
        await Assert.That(editor.Document.Segments[0].IsBold).IsTrue();
        await Assert.That(editor.Document.Segments[^1].IsBold).IsFalse();
        await Assert.That(editor.PlainText).IsEqualTo("ab");
    }

    /// <summary>Clearing at the caret affects subsequent input while preserving existing formatting.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ClearFormatting_AtCaretPreservesExistingDocument()
    {
        var editor = new RichTextBox();
        editor.SetHtml("<strong>a</strong>");
        editor.Select(editor.Document.Length, 0);
        editor.ClearFormatting();
        editor.ReplaceSelection("b");
        await Assert.That(editor.Document.Segments[0].IsBold).IsTrue();
        await Assert.That(editor.Document.Segments[^1].IsBold).IsFalse();
    }

    /// <summary>Font, size, color and highlight apply to newly inserted text.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task PendingTypography_PreservesExplicitCharacterStyles()
    {
        const double chosenFontSize = 22;
        var editor = new RichTextBox();
        editor.SetSelectionFontFamily("Consolas");
        editor.SetSelectionFontSize(chosenFontSize);
        editor.SetSelectionForeground(Colors.Red);
        editor.SetSelectionHighlight(Colors.Yellow);
        editor.ReplaceSelection("日本語");
        var segment = editor.Document.Segments[0];
        await Assert.That(segment.FontFamily?.Name).IsEqualTo("Consolas");
        await Assert.That(segment.FontSize).IsEqualTo(chosenFontSize);
        await Assert.That((segment.Foreground as ISolidColorBrush)?.Color).IsEqualTo(Colors.Red);
        await Assert.That((segment.Background as ISolidColorBrush)?.Color).IsEqualTo(Colors.Yellow);
    }

    /// <summary>Moving the insertion position drops explicit pending styles.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task PendingColor_CaretMoveResetsStyleAndReadOnlyRejectsChanges()
    {
        var editor = new RichTextBox();
        editor.SetPlainText("ab");
        editor.Select(0, 0);
        editor.SetSelectionForeground(Colors.Red);
        editor.Select(1, 0);
        editor.ReplaceSelection("x");
        await Assert.That(editor.Html.Contains("#FFFF0000", StringComparison.Ordinal)).IsFalse();
        editor.IsReadOnly = true;
        editor.ToggleBold();
        editor.ReplaceSelection("ignored");
        await Assert.That(editor.PlainText).IsEqualTo("axb");
    }
}
