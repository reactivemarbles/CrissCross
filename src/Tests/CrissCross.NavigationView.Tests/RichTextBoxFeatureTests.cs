// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Layout;
using Avalonia.Media;
using CrissCross.Avalonia.UI.Controls;
using AvaloniaContextMenu = Avalonia.Controls.ContextMenu;

namespace CrissCross.NavigationView.Tests;

/// <summary>Comprehensive feature coverage for the Avalonia RichTextBox control surface.</summary>
public sealed partial class RichTextBoxFeatureTests
{
    /// <summary>The image file URI used by drop tests.</summary>
    private const string TestImageFileUri = "file:///tmp/photo.png";

    /// <summary>The image data URI used by drop and clipboard tests.</summary>
    private const string TestImageDataUri = "data:image/png;base64,AAAA";

    /// <summary>The plain Hello world fixture.</summary>
    private const string HelloWorldText = "Hello world";

    /// <summary>The paste menu header.</summary>
    private const string PasteMenuHeader = "Paste";

    /// <summary>The alphabet fixture used by position and image tests.</summary>
    private const string AlphabetText = "abcdef";

    /// <summary>The rendered start offset for the word Hello.</summary>
    private const int HelloStartOffset = 0;

    /// <summary>The rendered length of the word Hello.</summary>
    private const int HelloLength = 5;

    /// <summary>The rendered start offset for the word world.</summary>
    private const int WorldStartOffset = 6;

    /// <summary>The rendered length of the word world.</summary>
    private const int WorldLength = 5;

    /// <summary>The styled font size encoded in the parsed HTML fixture.</summary>
    private const int ParsedStyledFontSize = 18;

    /// <summary>The styled image width encoded in the parsed HTML fixture.</summary>
    private const int ParsedImageWidth = 40;

    /// <summary>The styled image height encoded in the parsed HTML fixture.</summary>
    private const int ParsedImageHeight = 30;

    /// <summary>The font size applied by the style command test.</summary>
    private const int StyleCommandFontSize = 20;

    /// <summary>The rendered length of the mixed bold and plain text selection.</summary>
    private const int MixedSelectionLength = 10;

    /// <summary>The rendered document length after inserting an image and trailing text.</summary>
    private const int ImageDropDocumentLength = 8;

    /// <summary>The test control width used for hit testing.</summary>
    private const int HitTestWidth = 240;

    /// <summary>The test control height used for hit testing.</summary>
    private const int HitTestHeight = 40;

    /// <summary>The far right X coordinate used to snap to the document end.</summary>
    private const int FarRightPointX = 1_000;

    /// <summary>The X coordinate outside the control bounds.</summary>
    private const int OutsideLeftPointX = -10;

    /// <summary>The caret offset after dropping the XX text payload.</summary>
    private const int DroppedTextLength = 2;

    /// <summary>The delay used to allow asynchronous drop handlers to complete.</summary>
    private const int DropProcessingDelayMilliseconds = 25;

    /// <summary>The byte limit used to reject an oversized dropped text file.</summary>
    private const int DroppedTextFileByteLimit = 4;

    /// <summary>The natural width of the remote image test double.</summary>
    private const int RemoteImageNaturalWidth = 1200;

    /// <summary>The natural height of the remote image test double.</summary>
    private const int RemoteImageNaturalHeight = 600;

    /// <summary>The expected rendered width after fitting a remote image.</summary>
    private const int RemoteImageRenderedWidth = 640;

    /// <summary>The expected rendered height after fitting a remote image.</summary>
    private const int RemoteImageRenderedHeight = 320;

    /// <summary>The explicit image width applied to an inline image segment.</summary>
    private const int ExplicitImageWidth = 48;

    /// <summary>The explicit image height applied to an inline image segment.</summary>
    private const int ExplicitImageHeight = 32;

    /// <summary>Provides the FlowDocument_ParsesInlineFormattingImagesAndParagraphAlignment member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task FlowDocument_ParsesInlineFormattingImagesAndParagraphAlignment()
    {
        var document = new FlowDocument();

        const string ImageMarkup = "<img src=\"file:///tmp/photo.png\" width=\"40\" height=\"30\" align=\"Right\" />";
        var markup = new System.Text.StringBuilder("<p style=\"text-align:center\"><strong>Bold</strong><em>Italic</em><u>Under</u><s>Strike</s>")
            .Append("<span style=\"font-family:Consolas;font-size:18px;color:#FFFF0000;background-color:#FFFFFF00;")
            .Append("font-weight:bold;font-style:italic;text-decoration:underline\">Styled</span>")
            .Append(ImageMarkup)
            .Append("</p>")
            .ToString();
        document.SetText(markup);

        var bold = SingleTextSegment(document, "Bold");
        var italic = SingleTextSegment(document, "Italic");
        var underline = SingleTextSegment(document, "Under");
        var strike = SingleTextSegment(document, "Strike");
        var styled = SingleTextSegment(document, "Styled");
        TextSegment? image = null;
        foreach (var segment in document.Segments)
        {
            if (segment.IsImage)
            {
                image = segment;
                break;
            }
        }

        ArgumentNullException.ThrowIfNull(image);
        var paragraph = (FlowDocument.Paragraph)document.Blocks[0];

        await Assert.That(bold.IsBold).IsTrue();
        await Assert.That(italic.IsItalic).IsTrue();
        await Assert.That(underline.IsUnderline).IsTrue();
        await Assert.That(strike.IsStrikethrough).IsTrue();
        await Assert.That(styled.IsBold).IsTrue();
        await Assert.That(styled.IsItalic).IsTrue();
        await Assert.That(styled.IsUnderline).IsTrue();
        await Assert.That(styled.FontFamily?.Name).IsEqualTo("Consolas");
        await Assert.That(styled.FontSize).IsEqualTo(ParsedStyledFontSize);
        await Assert.That(GetSolidColor(styled.Foreground)).IsEqualTo(Colors.Red);
        await Assert.That(GetSolidColor(styled.Background)).IsEqualTo(Colors.Yellow);
        await Assert.That(image.ImageSource).IsEqualTo(TestImageFileUri);
        await Assert.That(image.ImageWidth).IsEqualTo(ParsedImageWidth);
        await Assert.That(image.ImageHeight).IsEqualTo(ParsedImageHeight);
        await Assert.That(image.ImageAlignment).IsEqualTo(HorizontalAlignment.Right);
        await Assert.That(paragraph.TextAlignment).IsEqualTo(TextAlignment.Center);
    }

    /// <summary>Provides the RichTextBox_InlineFormattingCommands_ApplyExpectedMarkupToSelection member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_InlineFormattingCommands_ApplyExpectedMarkupToSelection()
    {
        await AssertFormattingAsync(static box => box.ToggleBold(), "Hello <strong>world</strong>");
        await AssertFormattingAsync(static box => box.ToggleItalic(), "Hello <em>world</em>");
        await AssertFormattingAsync(static box => box.ToggleUnderline(), "Hello <u>world</u>");
        await AssertFormattingAsync(static box => box.ToggleStrikethrough(), "Hello <s>world</s>");
    }

    /// <summary>Provides the RichTextBox_StyleFormattingCommands_ApplyFontAndBrushStylesToSelection member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_StyleFormattingCommands_ApplyFontAndBrushStylesToSelection()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetPlainText(HelloWorldText);
        richTextBox.Select(WorldStartOffset, WorldLength);
        richTextBox.SetSelectionFontFamily("Consolas");
        richTextBox.Select(WorldStartOffset, WorldLength);
        richTextBox.SetSelectionFontSize(StyleCommandFontSize);
        richTextBox.Select(WorldStartOffset, WorldLength);
        richTextBox.SetSelectionForeground(Colors.DeepSkyBlue);
        richTextBox.Select(WorldStartOffset, WorldLength);
        richTextBox.SetSelectionHighlight(Colors.LightGreen);

        await Assert.That(richTextBox.PlainText).IsEqualTo(HelloWorldText);
        await Assert.That(richTextBox.Html).Contains("font-family:Consolas");
        await Assert.That(richTextBox.Html).Contains("font-size:20px");
        await Assert.That(richTextBox.Html).Contains("color:DeepSkyBlue");
        await Assert.That(richTextBox.Html).Contains("background-color:LightGreen");
    }

    /// <summary>Provides the RichTextBox_ClearFormatting_RemovesInlineStylesWhilePreservingText member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_ClearFormatting_RemovesInlineStylesWhilePreservingText()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetHtml("<strong>Hello</strong> <span style=\"font-size:18px;color:#FFFF0000\">world</span>");
        richTextBox.SelectAll();
        richTextBox.ClearFormatting();

        await Assert.That(richTextBox.PlainText).IsEqualTo(HelloWorldText);
        await Assert.That(richTextBox.Html).IsEqualTo(HelloWorldText);
        await Assert.That(richTextBox.ClearFormattingCommand.CanExecute(null)).IsFalse();
    }

    /// <summary>Verifies the documented behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_MixedSelectionFormatting_AppliesFormattingAcrossFormattedAndPlainRuns()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetHtml("Hello <strong>bold</strong> plain");
        richTextBox.Select(WorldStartOffset, MixedSelectionLength);
        richTextBox.ToggleItalic();

        await Assert.That(richTextBox.PlainText).IsEqualTo("Hello bold plain");
        var matchingSegmentsAreItalic = true;
        foreach (var segment in richTextBox.Document.Segments)
        {
            if (segment.Text is "bold" or " plain" && !segment.IsItalic)
            {
                matchingSegmentsAreItalic = false;
                break;
            }
        }

        await Assert.That(matchingSegmentsAreItalic).IsTrue();
    }

    /// <summary>Verifies the documented behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_ContextMenu_ContainsExpectedCommandsAndReflectsCommandEnablement()
    {
        var clipboard = new FakeRichTextClipboardAdapter { PlainText = "paste" };
        var richTextBox = new RichTextBox { ClipboardAdapter = clipboard };

        richTextBox.SetPlainText(HelloWorldText);
        richTextBox.Select(HelloStartOffset, HelloLength);
        var menu = richTextBox.EnsureContextMenu();
        richTextBox.RefreshContextMenuState();

        foreach (
            var expectedHeader in new[]
            {
                "Cut",
                "Copy",
                PasteMenuHeader,
                "Select All",
                "Undo",
                "Redo",
                "Bold",
                "Italic",
                "Underline",
                "Strikethrough",
                "Font",
                "Font Size",
                "Foreground",
                "Highlight",
                "Clear Formatting",
            })
        {
            await Assert.That(FindMenuItem(menu, expectedHeader) is not null).IsTrue();
        }

        await Assert.That(FindMenuItem(menu, "Cut")!.IsEnabled).IsTrue();
        await Assert.That(FindMenuItem(menu, "Copy")!.IsEnabled).IsTrue();
        await Assert.That(FindMenuItem(menu, PasteMenuHeader)!.IsEnabled).IsTrue();
        await Assert.That(FindMenuItem(menu, "Bold")!.IsEnabled).IsTrue();

        InvokeMenuItem(FindMenuItem(menu, "Bold")!);
        await Assert.That(richTextBox.Html).IsEqualTo("<strong>Hello</strong> world");
        richTextBox.Select(HelloStartOffset, HelloLength);

        richTextBox.IsReadOnly = true;
        richTextBox.RefreshContextMenuState();

        await Assert.That(FindMenuItem(menu, "Cut")!.IsEnabled).IsFalse();
        await Assert.That(FindMenuItem(menu, "Copy")!.IsEnabled).IsTrue();
        await Assert.That(FindMenuItem(menu, PasteMenuHeader)!.IsEnabled).IsFalse();
        await Assert.That(FindMenuItem(menu, "Bold")!.IsEnabled).IsFalse();

        richTextBox.IsReadOnly = false;
        richTextBox.RefreshContextMenuState();
        await Assert.That(FindMenuItem(menu, "Bold")!.IsEnabled).IsTrue();

        richTextBox.IsReadOnly = true;
        menu.RaiseEvent(new(AvaloniaContextMenu.OpenedEvent, menu));

        await Assert.That(FindMenuItem(menu, "Bold")!.IsEnabled).IsFalse();
    }

    /// <summary>Provides the RichTextBox_CommandSurface_ExecutesEditingAndFormattingOperations member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_CommandSurface_ExecutesEditingAndFormattingOperations()
    {
        var clipboard = new FakeRichTextClipboardAdapter { PlainText = "Avalonia" };
        var richTextBox = new RichTextBox { ClipboardAdapter = clipboard };

        richTextBox.SetPlainText(HelloWorldText);
        richTextBox.Select(WorldStartOffset, WorldLength);
        richTextBox.ToggleBoldCommand.Execute(null);

        await Assert.That(richTextBox.Html).IsEqualTo("Hello <strong>world</strong>");

        richTextBox.Select(WorldStartOffset, WorldLength);
        richTextBox.CopyCommand.Execute(null);
        await Assert.That(clipboard.PlainText).IsEqualTo("world");
        await Assert.That(clipboard.HtmlText).IsEqualTo("<strong>world</strong>");

        richTextBox.Select(WorldStartOffset, WorldLength);
        richTextBox.PasteCommand.Execute(null);
        await Assert.That(richTextBox.PlainText).IsEqualTo(HelloWorldText);
    }
}
