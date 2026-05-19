// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>
/// Regression tests for RichTextBox document offsets and core editing behavior.
/// </summary>
public sealed class RichTextBoxCoreTests
{
    [Test]
    public async Task FlowDocument_UsesRenderedOffsetsForFormattedHtml()
    {
        var document = new FlowDocument();

        document.SetText("<strong>Hello</strong> world");
        var selected = document.GetTextRange(document.GetTextPointer(0), document.GetTextPointer(5));

        await Assert.That(document.Length).IsEqualTo(11);
        await Assert.That(selected).IsEqualTo("Hello");
    }

    [Test]
    public async Task FlowDocument_Replace_UsesRenderedOffsetsWithoutCorruptingMarkup()
    {
        var document = new FlowDocument();

        document.SetText("<strong>Hello</strong> world");
        document.Replace(6, 5, "Avalonia");

        await Assert.That(document.GetPlainText()).IsEqualTo("Hello Avalonia");
        await Assert.That(document.GetText()).IsEqualTo("<strong>Hello</strong> Avalonia");
    }

    [Test]
    public async Task RichTextBox_SelectedText_UsesRenderedOffsets()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetHtml("<strong>Hello</strong> world");
        richTextBox.Select(0, 5);

        await Assert.That(richTextBox.SelectionLength).IsEqualTo(5);
        await Assert.That(richTextBox.SelectedText).IsEqualTo("Hello");
    }

    [Test]
    public async Task RichTextBox_ReplaceSelection_UsesRenderedOffsetsAndPlainTextCaret()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetHtml("<strong>Hello</strong> world");
        richTextBox.Select(6, 5);
        richTextBox.ReplaceSelection("Avalonia");

        await Assert.That(richTextBox.GetPlainText()).IsEqualTo("Hello Avalonia");
        await Assert.That(richTextBox.GetHtml()).IsEqualTo("<strong>Hello</strong> Avalonia");
        await Assert.That(richTextBox.CaretIndex).IsEqualTo(14);
    }
    [Test]
    public async Task RichTextBox_ToggleBold_AppliesFormattingToRenderedSelection()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetHtml("<strong>Hello</strong> world");
        richTextBox.Select(6, 5);
        richTextBox.ToggleBold();

        await Assert.That(richTextBox.GetHtml()).IsEqualTo("<strong>Hello</strong> <strong>world</strong>");
        await Assert.That(richTextBox.GetPlainText()).IsEqualTo("Hello world");
    }

    [Test]
    public async Task RichTextBox_ToggleBoldCommand_UsesSamePipelineAsPublicMethod()
    {
        var commandDriven = new RichTextBox();
        var methodDriven = new RichTextBox();

        commandDriven.SetHtml("Hello world");
        methodDriven.SetHtml("Hello world");
        commandDriven.Select(6, 5);
        methodDriven.Select(6, 5);

        await Assert.That(commandDriven.ToggleBoldCommand.CanExecute(null)).IsTrue();
        await Assert.That(commandDriven.CanApplyFormatting).IsTrue();

        commandDriven.ToggleBoldCommand.Execute(null);
        methodDriven.ToggleBold();

        await Assert.That(commandDriven.GetHtml()).IsEqualTo(methodDriven.GetHtml());
        await Assert.That(commandDriven.GetHtml()).IsEqualTo("Hello <strong>world</strong>");
    }

    [Test]
    public async Task RichTextBox_UndoRedo_RestoresReplacementAndFormattingTransactions()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetHtml("Hello world");
        richTextBox.Select(6, 5);
        richTextBox.ReplaceSelection("Avalonia");

        await Assert.That(richTextBox.CanUndo).IsTrue();
        await Assert.That(richTextBox.CanRedo).IsFalse();

        richTextBox.Undo();
        await Assert.That(richTextBox.GetHtml()).IsEqualTo("Hello world");
        await Assert.That(richTextBox.CanRedo).IsTrue();

        richTextBox.Redo();
        await Assert.That(richTextBox.GetHtml()).IsEqualTo("Hello Avalonia");

        richTextBox.Select(6, 8);
        richTextBox.ToggleItalic();
        await Assert.That(richTextBox.GetHtml()).IsEqualTo("Hello <em>Avalonia</em>");

        richTextBox.Undo();
        await Assert.That(richTextBox.GetHtml()).IsEqualTo("Hello Avalonia");
    }

    [Test]
    public async Task RichTextBox_ReadOnly_BlocksMutationCommandsButAllowsSelectionAndCopy()
    {
        var clipboard = new FakeRichTextClipboardAdapter();
        var richTextBox = new RichTextBox { ClipboardAdapter = clipboard };

        richTextBox.SetHtml("Hello world");
        richTextBox.Select(0, 5);
        richTextBox.IsReadOnly = true;

        await Assert.That(richTextBox.CanCopy).IsTrue();
        await Assert.That(richTextBox.CanCut).IsFalse();
        await Assert.That(richTextBox.CanPaste).IsFalse();
        await Assert.That(richTextBox.CanApplyFormatting).IsFalse();

        clipboard.PlainText = "paste";
        richTextBox.Copy();
        richTextBox.ToggleBold();
        richTextBox.Cut();
        richTextBox.Paste();

        await Assert.That(clipboard.PlainText).IsEqualTo("Hello");
        await Assert.That(richTextBox.GetHtml()).IsEqualTo("Hello world");
        await Assert.That(richTextBox.SelectedText).IsEqualTo("Hello");
    }

    [Test]
    public async Task RichTextBox_ClipboardAdapter_CutAndPasteUsePlainAndHtmlFallbacks()
    {
        var clipboard = new FakeRichTextClipboardAdapter();
        var richTextBox = new RichTextBox { ClipboardAdapter = clipboard };

        richTextBox.SetHtml("<strong>Hello</strong> world");
        richTextBox.Select(0, 5);
        richTextBox.Cut();

        await Assert.That(clipboard.PlainText).IsEqualTo("Hello");
        await Assert.That(clipboard.HtmlText).Contains("Hello");
        await Assert.That(richTextBox.GetHtml()).IsEqualTo(" world");

        clipboard.HtmlText = "<em>Avalonia</em>";
        clipboard.PlainText = "ignored";
        richTextBox.Select(richTextBox.Document.Length, 0);
        richTextBox.Paste();

        await Assert.That(richTextBox.GetHtml()).IsEqualTo(" world<em>Avalonia</em>");
        await Assert.That(richTextBox.GetPlainText()).IsEqualTo(" worldAvalonia");
    }

    private sealed class FakeRichTextClipboardAdapter : IRichTextClipboardAdapter
    {
        public string? PlainText { get; set; }

        public string? HtmlText { get; set; }

        public string? ImageSource { get; set; }

        public bool ContainsPlainText => !string.IsNullOrEmpty(PlainText);

        public bool ContainsHtml => !string.IsNullOrEmpty(HtmlText);

        public bool ContainsImage => !string.IsNullOrEmpty(ImageSource);

        public void SetPlainText(string? text) => PlainText = text;

        public void SetHtml(string? html) => HtmlText = html;

        public void SetImage(string? imageSource) => ImageSource = imageSource;
    }
}
