// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Layout;
using Avalonia.Media;
using CrissCross.Avalonia.UI.Controls;
using AvaloniaContextMenu = Avalonia.Controls.ContextMenu;
using AvaloniaMenuItem = Avalonia.Controls.MenuItem;

namespace CrissCross.NavigationView.Tests;

/// <summary>
/// Comprehensive feature coverage for the Avalonia RichTextBox control surface.
/// </summary>
public sealed class RichTextBoxFeatureTests
{
    [Test]
    public async Task FlowDocument_ParsesInlineFormattingImagesAndParagraphAlignment()
    {
        var document = new FlowDocument();

        document.SetText("<p style=\"text-align:center\"><strong>Bold</strong><em>Italic</em><u>Under</u><s>Strike</s><span style=\"font-family:Consolas;font-size:18px;color:#FFFF0000;background-color:#FFFFFF00;font-weight:bold;font-style:italic;text-decoration:underline\">Styled</span><img src=\"file:///tmp/photo.png\" width=\"40\" height=\"30\" align=\"Right\" /></p>");

        var bold = SingleTextSegment(document, "Bold");
        var italic = SingleTextSegment(document, "Italic");
        var underline = SingleTextSegment(document, "Under");
        var strike = SingleTextSegment(document, "Strike");
        var styled = SingleTextSegment(document, "Styled");
        var image = document.Segments.Single(segment => segment.IsImage);
        var paragraph = (FlowDocument.Paragraph)document.Blocks[0];

        await Assert.That(bold.IsBold).IsTrue();
        await Assert.That(italic.IsItalic).IsTrue();
        await Assert.That(underline.IsUnderline).IsTrue();
        await Assert.That(strike.IsStrikethrough).IsTrue();
        await Assert.That(styled.IsBold).IsTrue();
        await Assert.That(styled.IsItalic).IsTrue();
        await Assert.That(styled.IsUnderline).IsTrue();
        await Assert.That(styled.FontFamily?.Name).IsEqualTo("Consolas");
        await Assert.That(styled.FontSize).IsEqualTo(18);
        await Assert.That(GetSolidColor(styled.Foreground)).IsEqualTo(Colors.Red);
        await Assert.That(GetSolidColor(styled.Background)).IsEqualTo(Colors.Yellow);
        await Assert.That(image.ImageSource).IsEqualTo("file:///tmp/photo.png");
        await Assert.That(image.ImageWidth).IsEqualTo(40);
        await Assert.That(image.ImageHeight).IsEqualTo(30);
        await Assert.That(image.ImageAlignment).IsEqualTo(HorizontalAlignment.Right);
        await Assert.That(paragraph.TextAlignment).IsEqualTo(TextAlignment.Center);
    }

    [Test]
    public async Task RichTextBox_InlineFormattingCommands_ApplyExpectedMarkupToSelection()
    {
        await AssertFormattingAsync(box => box.ToggleBold(), "Hello <strong>world</strong>");
        await AssertFormattingAsync(box => box.ToggleItalic(), "Hello <em>world</em>");
        await AssertFormattingAsync(box => box.ToggleUnderline(), "Hello <u>world</u>");
        await AssertFormattingAsync(box => box.ToggleStrikethrough(), "Hello <s>world</s>");
    }

    [Test]
    public async Task RichTextBox_StyleFormattingCommands_ApplyFontAndBrushStylesToSelection()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetPlainText("Hello world");
        richTextBox.Select(6, 5);
        richTextBox.SetSelectionFontFamily("Consolas");
        richTextBox.Select(6, 5);
        richTextBox.SetSelectionFontSize(20);
        richTextBox.Select(6, 5);
        richTextBox.SetSelectionForeground(Colors.DeepSkyBlue);
        richTextBox.Select(6, 5);
        richTextBox.SetSelectionHighlight(Colors.LightGreen);

        await Assert.That(richTextBox.GetPlainText()).IsEqualTo("Hello world");
        await Assert.That(richTextBox.GetHtml()).Contains("font-family:Consolas");
        await Assert.That(richTextBox.GetHtml()).Contains("font-size:20px");
        await Assert.That(richTextBox.GetHtml()).Contains("color:DeepSkyBlue");
        await Assert.That(richTextBox.GetHtml()).Contains("background-color:LightGreen");
    }

    [Test]
    public async Task RichTextBox_ClearFormatting_RemovesInlineStylesWhilePreservingText()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetHtml("<strong>Hello</strong> <span style=\"font-size:18px;color:#FFFF0000\">world</span>");
        richTextBox.SelectAll();
        richTextBox.ClearFormatting();

        await Assert.That(richTextBox.GetPlainText()).IsEqualTo("Hello world");
        await Assert.That(richTextBox.GetHtml()).IsEqualTo("Hello world");
        await Assert.That(richTextBox.ClearFormattingCommand.CanExecute(null)).IsFalse();
    }

    [Test]
    public async Task RichTextBox_MixedSelectionFormatting_AppliesFormattingAcrossFormattedAndPlainRuns()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetHtml("Hello <strong>bold</strong> plain");
        richTextBox.Select(6, 10);
        richTextBox.ToggleItalic();

        await Assert.That(richTextBox.GetPlainText()).IsEqualTo("Hello bold plain");
        await Assert.That(richTextBox.Document.Segments.Where(segment => segment.Text is "bold" or " plain").All(segment => segment.IsItalic)).IsTrue();
    }

    [Test]
    public async Task RichTextBox_ContextMenu_ContainsExpectedCommandsAndReflectsCommandEnablement()
    {
        var clipboard = new FakeRichTextClipboardAdapter { PlainText = "paste" };
        var richTextBox = new RichTextBox { ClipboardAdapter = clipboard };

        richTextBox.SetPlainText("Hello world");
        richTextBox.Select(0, 5);
        var menu = richTextBox.EnsureContextMenu();
        richTextBox.RefreshContextMenuState();

        foreach (var expectedHeader in new[] { "Cut", "Copy", "Paste", "Select All", "Undo", "Redo", "Bold", "Italic", "Underline", "Strikethrough", "Font", "Font Size", "Foreground", "Highlight", "Clear Formatting" })
        {
            await Assert.That(FindMenuItem(menu, expectedHeader) is not null).IsTrue();
        }

        await Assert.That(FindMenuItem(menu, "Cut")!.IsEnabled).IsTrue();
        await Assert.That(FindMenuItem(menu, "Copy")!.IsEnabled).IsTrue();
        await Assert.That(FindMenuItem(menu, "Paste")!.IsEnabled).IsTrue();
        await Assert.That(FindMenuItem(menu, "Bold")!.IsEnabled).IsTrue();

        richTextBox.IsReadOnly = true;
        richTextBox.RefreshContextMenuState();

        await Assert.That(FindMenuItem(menu, "Cut")!.IsEnabled).IsFalse();
        await Assert.That(FindMenuItem(menu, "Copy")!.IsEnabled).IsTrue();
        await Assert.That(FindMenuItem(menu, "Paste")!.IsEnabled).IsFalse();
        await Assert.That(FindMenuItem(menu, "Bold")!.IsEnabled).IsFalse();
    }

    [Test]
    public async Task RichTextBox_CommandSurface_ExecutesEditingAndFormattingOperations()
    {
        var clipboard = new FakeRichTextClipboardAdapter { PlainText = "Avalonia" };
        var richTextBox = new RichTextBox { ClipboardAdapter = clipboard };

        richTextBox.SetPlainText("Hello world");
        richTextBox.Select(6, 5);
        richTextBox.ToggleBoldCommand.Execute(null);

        await Assert.That(richTextBox.GetHtml()).IsEqualTo("Hello <strong>world</strong>");

        richTextBox.Select(6, 5);
        richTextBox.CopyCommand.Execute(null);
        await Assert.That(clipboard.PlainText).IsEqualTo("world");
        await Assert.That(clipboard.HtmlText).IsEqualTo("<strong>world</strong>");

        richTextBox.Select(6, 5);
        richTextBox.PasteCommand.Execute(null);
        await Assert.That(richTextBox.GetPlainText()).IsEqualTo("Hello world");
    }

    [Test]
    public async Task RichTextBox_DropText_AcceptsRawAndHtmlTextAndRejectsInvalidOrReadOnlyDrops()
    {
        var richTextBox = new RichTextBox();

        await Assert.That(richTextBox.TryDropText("plain text")).IsTrue();
        await Assert.That(richTextBox.GetPlainText()).IsEqualTo("plain text");

        richTextBox.Select(richTextBox.Document.Length, 0);
        await Assert.That(richTextBox.TryDropText("<em> rich</em>")).IsTrue();
        await Assert.That(richTextBox.GetHtml()).IsEqualTo("plain text<em> rich</em>");

        await Assert.That(richTextBox.TryDropText("   ")).IsFalse();
        richTextBox.IsReadOnly = true;
        await Assert.That(richTextBox.TryDropText("blocked")).IsFalse();
        await Assert.That(richTextBox.GetPlainText()).IsEqualTo("plain text rich");
    }

    [Test]
    public async Task RichTextBox_DropImage_AcceptsImagePathsAndDataUrisButRejectsUnsupportedImages()
    {
        var richTextBox = new RichTextBox();

        await Assert.That(richTextBox.TryDropImage("file:///tmp/photo.png")).IsTrue();
        await Assert.That(richTextBox.Document.Segments.Any(segment => segment.IsImage && segment.ImageSource == "file:///tmp/photo.png")).IsTrue();

        await Assert.That(richTextBox.TryDropImage("data:image/png;base64,AAAA")).IsTrue();
        await Assert.That(richTextBox.Document.Segments.Any(segment => segment.IsImage && segment.ImageSource == "data:image/png;base64,AAAA")).IsTrue();

        var before = richTextBox.GetHtml();
        await Assert.That(richTextBox.TryDropImage("file:///tmp/readme.txt")).IsFalse();
        richTextBox.IsImageDropEnabled = false;
        await Assert.That(richTextBox.TryDropImage("file:///tmp/blocked.png")).IsFalse();
        await Assert.That(richTextBox.GetHtml()).IsEqualTo(before);
    }

    [Test]
    public async Task RichTextBox_DropImage_ConsumesSingleRenderedPositionAndKeepsFollowingDropsOrdered()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetPlainText("abcdef");
        richTextBox.Select(richTextBox.Document.Length, 0);
        await Assert.That(richTextBox.TryDropImage("file:///tmp/photo.png")).IsTrue();
        richTextBox.ReplaceSelection("Z");

        await Assert.That(richTextBox.Document.Length).IsEqualTo(8);
        await Assert.That(richTextBox.CaretIndex).IsEqualTo(8);
        await Assert.That(richTextBox.GetHtml()).IsEqualTo("abcdef<img src=\"file:///tmp/photo.png\" />Z");
    }

    [Test]
    public async Task FormattedTextPresenter_ImageInline_KeepsPreviousTextRunSplitSafe()
    {
        var document = new FlowDocument();
        const string imageSource = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+/p9sAAAAASUVORK5CYII=";
        var presenter = new FormattedTextPresenter();

        document.SetText($"abcdef<img src=\"{imageSource}\" width=\"48\" height=\"48\" />");
        presenter.Document = document;
        presenter.UpdateInlines();

        var firstRun = presenter.Inlines?.OfType<global::Avalonia.Controls.Documents.Run>().FirstOrDefault();
        await Assert.That(firstRun?.Text).IsEqualTo("abcdef\u200B");
    }

    [Test]
    public async Task RichTextBox_Clipboard_PreservesRichSelectionAndFallsBackToPlainTextPaste()
    {
        var clipboard = new FakeRichTextClipboardAdapter();
        var richTextBox = new RichTextBox { ClipboardAdapter = clipboard };

        richTextBox.SetHtml("<strong>Hello</strong> <em>world</em>");
        richTextBox.Select(6, 5);
        richTextBox.Copy();

        await Assert.That(clipboard.PlainText).IsEqualTo("world");
        await Assert.That(clipboard.HtmlText).IsEqualTo("<em>world</em>");

        clipboard.HtmlText = null;
        clipboard.PlainText = "Avalonia";
        richTextBox.Select(0, 5);
        richTextBox.Paste();

        await Assert.That(richTextBox.GetPlainText()).IsEqualTo("Avalonia world");
    }

    [Test]
    public async Task RichTextBox_Clipboard_CutAndPasteImagesHonorImagePolicyAndReadOnlyMode()
    {
        var clipboard = new FakeRichTextClipboardAdapter { ImageSource = "data:image/png;base64,AAAA" };
        var richTextBox = new RichTextBox { ClipboardAdapter = clipboard };

        await Assert.That(richTextBox.CanPaste).IsTrue();
        richTextBox.Paste();

        await Assert.That(richTextBox.Document.Segments.Any(segment => segment.IsImage && segment.ImageSource == "data:image/png;base64,AAAA")).IsTrue();

        var before = richTextBox.GetHtml();
        clipboard.ImageSource = "data:image/png;base64,BBBB";
        richTextBox.IsImagePasteEnabled = false;
        richTextBox.Paste();
        await Assert.That(richTextBox.GetHtml()).IsEqualTo(before);

        richTextBox.IsImagePasteEnabled = true;
        richTextBox.IsReadOnly = true;
        richTextBox.Paste();
        await Assert.That(richTextBox.GetHtml()).IsEqualTo(before);
    }

    [Test]
    public async Task RichTextBox_DisplayMode_DisablesMutationsButPreservesContentAndCopySelection()
    {
        var clipboard = new FakeRichTextClipboardAdapter { PlainText = "blocked" };
        var richTextBox = new RichTextBox { ClipboardAdapter = clipboard };

        richTextBox.SetPlainText("Hello world");
        richTextBox.Select(0, 5);
        richTextBox.EditMode = RichTextEditMode.Display;

        await Assert.That(richTextBox.CanCopy).IsTrue();
        await Assert.That(richTextBox.CanCut).IsFalse();
        await Assert.That(richTextBox.CanPaste).IsFalse();
        await Assert.That(richTextBox.CanApplyFormatting).IsFalse();

        richTextBox.Copy();
        richTextBox.Cut();
        richTextBox.Paste();
        richTextBox.ToggleBold();

        await Assert.That(clipboard.PlainText).IsEqualTo("Hello");
        await Assert.That(richTextBox.GetPlainText()).IsEqualTo("Hello world");

        richTextBox.EditMode = RichTextEditMode.Edit;
        richTextBox.Select(6, 5);
        richTextBox.Paste();
        await Assert.That(richTextBox.GetPlainText()).IsEqualTo("Hello Hello");
    }

    private static async Task AssertFormattingAsync(Action<RichTextBox> apply, string expectedHtml)
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetPlainText("Hello world");
        richTextBox.Select(6, 5);
        apply(richTextBox);

        await Assert.That(richTextBox.GetHtml()).IsEqualTo(expectedHtml);
        await Assert.That(richTextBox.GetPlainText()).IsEqualTo("Hello world");
    }

    private static TextSegment SingleTextSegment(FlowDocument document, string text) => document.Segments.Single(segment => segment.Text == text);

    private static Color? GetSolidColor(IBrush? brush) => brush is SolidColorBrush solidColorBrush ? solidColorBrush.Color : null;

    private static AvaloniaMenuItem? FindMenuItem(AvaloniaContextMenu menu, string header)
    {
        foreach (var item in FlattenMenuItems(menu.ItemsSource))
        {
            if (string.Equals(item.Header?.ToString(), header, StringComparison.Ordinal))
            {
                return item;
            }
        }

        return null;
    }

    private static IEnumerable<AvaloniaMenuItem> FlattenMenuItems(object? source)
    {
        if (source is not IEnumerable<object> items)
        {
            yield break;
        }

        foreach (var item in items)
        {
            if (item is not AvaloniaMenuItem menuItem)
            {
                continue;
            }

            yield return menuItem;
            foreach (var child in FlattenMenuItems(menuItem.ItemsSource))
            {
                yield return child;
            }
        }
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
