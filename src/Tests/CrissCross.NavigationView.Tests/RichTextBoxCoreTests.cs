// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Input;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Regression tests for RichTextBox document offsets and core editing behavior.</summary>
public sealed class RichTextBoxCoreTests
{
    /// <summary>Provides the FlowDocument_UsesRenderedOffsetsForFormattedHtml member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task FlowDocument_UsesRenderedOffsetsForFormattedHtml()
    {
        var document = new FlowDocument();

        document.SetText("<strong>Hello</strong> world");
        var selected = document.GetTextRange(document.GetTextPointer(0), document.GetTextPointer(5));

        await Assert.That(document.Length).IsEqualTo(11);
        await Assert.That(selected).IsEqualTo("Hello");
    }

    /// <summary>Provides the FlowDocument_Replace_UsesRenderedOffsetsWithoutCorruptingMarkup member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task FlowDocument_Replace_UsesRenderedOffsetsWithoutCorruptingMarkup()
    {
        var document = new FlowDocument();

        document.SetText("<strong>Hello</strong> world");
        document.Replace(6, 5, "Avalonia");

        await Assert.That(document.PlainText).IsEqualTo("Hello Avalonia");
        await Assert.That(document.Text).IsEqualTo("<strong>Hello</strong> Avalonia");
    }

    /// <summary>Provides the RichTextBox_SelectedText_UsesRenderedOffsets member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_SelectedText_UsesRenderedOffsets()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetHtml("<strong>Hello</strong> world");
        richTextBox.Select(0, 5);

        await Assert.That(richTextBox.SelectionLength).IsEqualTo(5);
        await Assert.That(richTextBox.SelectedText).IsEqualTo("Hello");
    }

    /// <summary>Provides the RichTextBox_ReplaceSelection_UsesRenderedOffsetsAndPlainTextCaret member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_ReplaceSelection_UsesRenderedOffsetsAndPlainTextCaret()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetHtml("<strong>Hello</strong> world");
        richTextBox.Select(6, 5);
        richTextBox.ReplaceSelection("Avalonia");

        await Assert.That(richTextBox.PlainText).IsEqualTo("Hello Avalonia");
        await Assert.That(richTextBox.Html).IsEqualTo("<strong>Hello</strong> Avalonia");
        await Assert.That(richTextBox.CaretIndex).IsEqualTo(14);
    }

    /// <summary>Provides the RichTextBox_ToggleBold_AppliesFormattingToRenderedSelection member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_ToggleBold_AppliesFormattingToRenderedSelection()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetHtml("<strong>Hello</strong> world");
        richTextBox.Select(6, 5);
        richTextBox.ToggleBold();

        await Assert.That(richTextBox.Html).IsEqualTo("<strong>Hello</strong> <strong>world</strong>");
        await Assert.That(richTextBox.PlainText).IsEqualTo("Hello world");
    }

    /// <summary>Provides the RichTextBox_ToggleBoldCommand_UsesSamePipelineAsPublicMethod member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

        await Assert.That(commandDriven.Html).IsEqualTo(methodDriven.Html);
        await Assert.That(commandDriven.Html).IsEqualTo("Hello <strong>world</strong>");
    }

    /// <summary>Provides the RichTextBox_UndoRedo_RestoresReplacementAndFormattingTransactions member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
        await Assert.That(richTextBox.Html).IsEqualTo("Hello world");
        await Assert.That(richTextBox.CanRedo).IsTrue();

        richTextBox.Redo();
        await Assert.That(richTextBox.Html).IsEqualTo("Hello Avalonia");

        richTextBox.Select(6, 8);
        richTextBox.ToggleItalic();
        await Assert.That(richTextBox.Html).IsEqualTo("Hello <em>Avalonia</em>");

        richTextBox.Undo();
        await Assert.That(richTextBox.Html).IsEqualTo("Hello Avalonia");
    }

    /// <summary>Provides the RichTextBox_ReadOnly_BlocksMutationCommandsButAllowsSelectionAndCopy member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
        await Assert.That(richTextBox.Html).IsEqualTo("Hello world");
        await Assert.That(richTextBox.SelectedText).IsEqualTo("Hello");
    }

    /// <summary>Provides the RichTextBox_ClipboardAdapter_CutAndPasteUsePlainAndHtmlFallbacks member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
        await Assert.That(richTextBox.Html).IsEqualTo(" world");

        clipboard.HtmlText = "<em>Avalonia</em>";
        clipboard.PlainText = "ignored";
        richTextBox.Select(richTextBox.Document.Length, 0);
        richTextBox.Paste();

        await Assert.That(richTextBox.Html).IsEqualTo(" world<em>Avalonia</em>");
        await Assert.That(richTextBox.PlainText).IsEqualTo(" worldAvalonia");
    }

    /// <summary>Provides the RichTextBox_KeyboardShortcuts_RouteEditingCommandsThroughCommandPipeline member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_KeyboardShortcuts_RouteEditingCommandsThroughCommandPipeline()
    {
        var clipboard = new FakeRichTextClipboardAdapter { PlainText = "paste" };
        var richTextBox = new TestableRichTextBox { ClipboardAdapter = clipboard };

        richTextBox.SetPlainText("Hello world");
        richTextBox.Select(6, 5);
        richTextBox.SendKey(Key.C);

        await Assert.That(clipboard.PlainText).IsEqualTo("world");

        richTextBox.SendKey(Key.X);
        await Assert.That(richTextBox.PlainText).IsEqualTo("Hello ");
        await Assert.That(clipboard.PlainText).IsEqualTo("world");

        clipboard.PlainText = "Avalonia";
        clipboard.HtmlText = null;
        richTextBox.Select(richTextBox.Document.Length, 0);
        richTextBox.SendKey(Key.V);
        await Assert.That(richTextBox.PlainText).IsEqualTo("Hello Avalonia");

        richTextBox.SendKey(Key.Z);
        await Assert.That(richTextBox.PlainText).IsEqualTo("Hello ");

        richTextBox.SendKey(Key.Y);
        await Assert.That(richTextBox.PlainText).IsEqualTo("Hello Avalonia");
    }

    /// <summary>Provides the TestableRichTextBox member.</summary>
    private sealed class TestableRichTextBox : RichTextBox
    {
        /// <summary>Provides the SendKey member.</summary>
        /// <param name="key">The key value.</param>
        public void SendKey(Key key)
        {
            var args = new KeyEventArgs
            {
                Key = key,
                KeyModifiers = KeyModifiers.Control
            };

            OnKeyDown(args);
        }
    }

    /// <summary>Provides the FakeRichTextClipboardAdapter member.</summary>
    private sealed class FakeRichTextClipboardAdapter : IRichTextClipboardAdapter
    {
        /// <summary>Gets or sets the plain text.</summary>
        public string? PlainText { get; set; }

        /// <summary>Gets or sets the HTML text.</summary>
        public string? HtmlText { get; set; }

        /// <summary>Gets or sets the image source.</summary>
        public string? ImageSource { get; set; }

        /// <summary>Gets a value indicating whether plain text is available.</summary>
        public bool ContainsPlainText => !string.IsNullOrEmpty(PlainText);

        /// <summary>Gets a value indicating whether HTML text is available.</summary>
        public bool ContainsHtml => !string.IsNullOrEmpty(HtmlText);

        /// <summary>Gets a value indicating whether an image is available.</summary>
        public bool ContainsImage => !string.IsNullOrEmpty(ImageSource);

        /// <summary>Provides the SetPlainText member.</summary>
        /// <param name="text">The text value.</param>
        public void SetPlainText(string? text) => PlainText = text;

        /// <summary>Provides the SetHtml member.</summary>
        /// <param name="html">The html value.</param>
        public void SetHtml(string? html) => HtmlText = html;

        /// <summary>Provides the SetImage member.</summary>
        /// <param name="imageSource">The imageSource value.</param>
        public void SetImage(string? imageSource) => ImageSource = imageSource;
    }
}
