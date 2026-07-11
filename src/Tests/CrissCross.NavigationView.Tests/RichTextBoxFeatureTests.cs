// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Text;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CrissCross.Avalonia.UI.Controls;
using AvaloniaContextMenu = Avalonia.Controls.ContextMenu;
using AvaloniaMenuItem = Avalonia.Controls.MenuItem;

namespace CrissCross.NavigationView.Tests;

/// <summary>Comprehensive feature coverage for the Avalonia RichTextBox control surface.</summary>
public sealed class RichTextBoxFeatureTests
{
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
        await Assert.That(styled.FontSize).IsEqualTo(ParsedStyledFontSize);
        await Assert.That(GetSolidColor(styled.Foreground)).IsEqualTo(Colors.Red);
        await Assert.That(GetSolidColor(styled.Background)).IsEqualTo(Colors.Yellow);
        await Assert.That(image.ImageSource).IsEqualTo("file:///tmp/photo.png");
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
        await AssertFormattingAsync(box => box.ToggleBold(), "Hello <strong>world</strong>");
        await AssertFormattingAsync(box => box.ToggleItalic(), "Hello <em>world</em>");
        await AssertFormattingAsync(box => box.ToggleUnderline(), "Hello <u>world</u>");
        await AssertFormattingAsync(box => box.ToggleStrikethrough(), "Hello <s>world</s>");
    }

    /// <summary>Provides the RichTextBox_StyleFormattingCommands_ApplyFontAndBrushStylesToSelection member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_StyleFormattingCommands_ApplyFontAndBrushStylesToSelection()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetPlainText("Hello world");
        richTextBox.Select(WorldStartOffset, WorldLength);
        richTextBox.SetSelectionFontFamily("Consolas");
        richTextBox.Select(WorldStartOffset, WorldLength);
        richTextBox.SetSelectionFontSize(StyleCommandFontSize);
        richTextBox.Select(WorldStartOffset, WorldLength);
        richTextBox.SetSelectionForeground(Colors.DeepSkyBlue);
        richTextBox.Select(WorldStartOffset, WorldLength);
        richTextBox.SetSelectionHighlight(Colors.LightGreen);

        await Assert.That(richTextBox.PlainText).IsEqualTo("Hello world");
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

        await Assert.That(richTextBox.PlainText).IsEqualTo("Hello world");
        await Assert.That(richTextBox.Html).IsEqualTo("Hello world");
        await Assert.That(richTextBox.ClearFormattingCommand.CanExecute(null)).IsFalse();
    }

    /// <summary>Provides the RichTextBox_MixedSelectionFormatting_AppliesFormattingAcrossFormattedAndPlainRuns member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_MixedSelectionFormatting_AppliesFormattingAcrossFormattedAndPlainRuns()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetHtml("Hello <strong>bold</strong> plain");
        richTextBox.Select(WorldStartOffset, MixedSelectionLength);
        richTextBox.ToggleItalic();

        await Assert.That(richTextBox.PlainText).IsEqualTo("Hello bold plain");
        await Assert.That(richTextBox.Document.Segments.Where(segment => segment.Text is "bold" or " plain").All(segment => segment.IsItalic)).IsTrue();
    }

    /// <summary>Provides the RichTextBox_ContextMenu_ContainsExpectedCommandsAndReflectsCommandEnablement member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_ContextMenu_ContainsExpectedCommandsAndReflectsCommandEnablement()
    {
        var clipboard = new FakeRichTextClipboardAdapter { PlainText = "paste" };
        var richTextBox = new RichTextBox { ClipboardAdapter = clipboard };

        richTextBox.SetPlainText("Hello world");
        richTextBox.Select(HelloStartOffset, HelloLength);
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

        InvokeMenuItem(FindMenuItem(menu, "Bold")!);
        await Assert.That(richTextBox.Html).IsEqualTo("<strong>Hello</strong> world");
        richTextBox.Select(HelloStartOffset, HelloLength);

        richTextBox.IsReadOnly = true;
        richTextBox.RefreshContextMenuState();

        await Assert.That(FindMenuItem(menu, "Cut")!.IsEnabled).IsFalse();
        await Assert.That(FindMenuItem(menu, "Copy")!.IsEnabled).IsTrue();
        await Assert.That(FindMenuItem(menu, "Paste")!.IsEnabled).IsFalse();
        await Assert.That(FindMenuItem(menu, "Bold")!.IsEnabled).IsFalse();

        richTextBox.IsReadOnly = false;
        richTextBox.RefreshContextMenuState();
        await Assert.That(FindMenuItem(menu, "Bold")!.IsEnabled).IsTrue();

        richTextBox.IsReadOnly = true;
        menu.RaiseEvent(new RoutedEventArgs(AvaloniaContextMenu.OpenedEvent, menu));

        await Assert.That(FindMenuItem(menu, "Bold")!.IsEnabled).IsFalse();
    }

    /// <summary>Provides the RichTextBox_CommandSurface_ExecutesEditingAndFormattingOperations member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_CommandSurface_ExecutesEditingAndFormattingOperations()
    {
        var clipboard = new FakeRichTextClipboardAdapter { PlainText = "Avalonia" };
        var richTextBox = new RichTextBox { ClipboardAdapter = clipboard };

        richTextBox.SetPlainText("Hello world");
        richTextBox.Select(WorldStartOffset, WorldLength);
        richTextBox.ToggleBoldCommand.Execute(null);

        await Assert.That(richTextBox.Html).IsEqualTo("Hello <strong>world</strong>");

        richTextBox.Select(WorldStartOffset, WorldLength);
        richTextBox.CopyCommand.Execute(null);
        await Assert.That(clipboard.PlainText).IsEqualTo("world");
        await Assert.That(clipboard.HtmlText).IsEqualTo("<strong>world</strong>");

        richTextBox.Select(WorldStartOffset, WorldLength);
        richTextBox.PasteCommand.Execute(null);
        await Assert.That(richTextBox.PlainText).IsEqualTo("Hello world");
    }

    /// <summary>Provides the RichTextBox_DropText_AcceptsRawAndHtmlTextAndRejectsInvalidOrReadOnlyDrops member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_DropText_AcceptsRawAndHtmlTextAndRejectsInvalidOrReadOnlyDrops()
    {
        var richTextBox = new RichTextBox();

        await Assert.That(richTextBox.TryDropText("plain text")).IsTrue();
        await Assert.That(richTextBox.PlainText).IsEqualTo("plain text");

        richTextBox.Select(richTextBox.Document.Length, 0);
        await Assert.That(richTextBox.TryDropText("<em> rich</em>")).IsTrue();
        await Assert.That(richTextBox.Html).IsEqualTo("plain text<em> rich</em>");

        await Assert.That(richTextBox.TryDropText("   ")).IsFalse();
        richTextBox.IsReadOnly = true;
        await Assert.That(richTextBox.TryDropText("blocked")).IsFalse();
        await Assert.That(richTextBox.PlainText).IsEqualTo("plain text rich");
    }

    /// <summary>Provides the RichTextBox_DropImage_AcceptsImagePathsAndDataUrisButRejectsUnsupportedImages member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_DropImage_AcceptsImagePathsAndDataUrisButRejectsUnsupportedImages()
    {
        var richTextBox = new RichTextBox();

        await Assert.That(richTextBox.TryDropImage("file:///tmp/photo.png")).IsTrue();
        await Assert.That(richTextBox.Document.Segments.Any(segment => segment.IsImage && segment.ImageSource == "file:///tmp/photo.png")).IsTrue();

        await Assert.That(richTextBox.TryDropImage("data:image/png;base64,AAAA")).IsTrue();
        await Assert.That(richTextBox.Document.Segments.Any(segment => segment.IsImage && segment.ImageSource == "data:image/png;base64,AAAA")).IsTrue();

        var before = richTextBox.Html;
        await Assert.That(richTextBox.TryDropImage("file:///tmp/readme.txt")).IsFalse();
        richTextBox.IsImageDropEnabled = false;
        await Assert.That(richTextBox.TryDropImage("file:///tmp/blocked.png")).IsFalse();
        await Assert.That(richTextBox.Html).IsEqualTo(before);
    }

    /// <summary>Provides the RichTextBox_DropImage_ConsumesSingleRenderedPositionAndKeepsFollowingDropsOrdered member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_DropImage_ConsumesSingleRenderedPositionAndKeepsFollowingDropsOrdered()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetPlainText("abcdef");
        richTextBox.Select(richTextBox.Document.Length, 0);
        await Assert.That(richTextBox.TryDropImage("file:///tmp/photo.png")).IsTrue();
        richTextBox.ReplaceSelection("Z");

        await Assert.That(richTextBox.Document.Length).IsEqualTo(ImageDropDocumentLength);
        await Assert.That(richTextBox.CaretIndex).IsEqualTo(ImageDropDocumentLength);
        await Assert.That(richTextBox.Html).IsEqualTo("abcdef<img src=\"file:///tmp/photo.png\" />Z");
    }

    /// <summary>Provides the RichTextBox_GetPositionFromPoint_MapsPointToRenderedDocumentOffset member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_GetPositionFromPoint_MapsPointToRenderedDocumentOffset()
    {
        var richTextBox = new RichTextBox { Width = HitTestWidth, Height = HitTestHeight };

        richTextBox.SetPlainText("abcdef");
        richTextBox.Select(richTextBox.Document.Length, 0);

        await Assert.That(richTextBox.GetPositionFromPoint(new global::Avalonia.Point(0, 0), snapToText: true)?.Offset).IsEqualTo(0);
        await Assert.That(richTextBox.GetPositionFromPoint(new global::Avalonia.Point(FarRightPointX, 0), snapToText: true)?.Offset).IsEqualTo(richTextBox.Document.Length);
        await Assert.That(richTextBox.GetPositionFromPoint(new global::Avalonia.Point(OutsideLeftPointX, 0), snapToText: false)).IsNull();
    }

    /// <summary>Provides the RichTextBox_RuntimeDragOver_AdvertisesCopyOnlyForSupportedPayloads member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_RuntimeDragOver_AdvertisesCopyOnlyForSupportedPayloads()
    {
        var richTextBox = new RichTextBox();

        var unsupported = RaiseDragOver(richTextBox, TestDataTransfer.Empty, new global::Avalonia.Point(0, 0));
        var supported = RaiseDragOver(richTextBox, TestDataTransfer.Text("drop"), new global::Avalonia.Point(0, 0));

        await Assert.That(unsupported.DragEffects).IsEqualTo(DragDropEffects.None);
        await Assert.That(unsupported.Handled).IsTrue();
        await Assert.That(supported.DragEffects).IsEqualTo(DragDropEffects.Copy);
        await Assert.That(supported.Handled).IsTrue();
    }

    /// <summary>Provides the RichTextBox_RuntimeDrop_TextUsesHitTestedDropPosition member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_RuntimeDrop_TextUsesHitTestedDropPosition()
    {
        var richTextBox = new RichTextBox { Width = HitTestWidth, Height = HitTestHeight };

        richTextBox.SetPlainText("abcdef");
        richTextBox.Select(richTextBox.Document.Length, 0);

        _ = RaiseDrop(richTextBox, TestDataTransfer.Text("XX"), new global::Avalonia.Point(0, 0));

        await Assert.That(richTextBox.PlainText).IsEqualTo("XXabcdef");
        await Assert.That(richTextBox.CaretIndex).IsEqualTo(DroppedTextLength);
    }

    /// <summary>Provides the RichTextBox_RuntimeDrop_EnforcesImageAndTextFilePolicies member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_RuntimeDrop_EnforcesImageAndTextFilePolicies()
    {
        var imageDisabled = new RichTextBox { IsImageDropEnabled = false };

        _ = RaiseDrop(imageDisabled, TestDataTransfer.File(CreateStorageFile(".png", string.Empty)), new global::Avalonia.Point(0, 0));

        await Assert.That(imageDisabled.Html).DoesNotContain("<img");

        var unsupported = new RichTextBox();
        _ = RaiseDrop(unsupported, TestDataTransfer.File(CreateStorageFile(".exe", "should-not-load")), new global::Avalonia.Point(0, 0));
        await Task.Delay(DropProcessingDelayMilliseconds);

        await Assert.That(unsupported.PlainText).IsEmpty();

        var oversized = new RichTextBox { MaxDroppedTextFileBytes = DroppedTextFileByteLimit };
        _ = RaiseDrop(oversized, TestDataTransfer.File(CreateStorageFile(".txt", "12345")), new global::Avalonia.Point(0, 0));
        await Task.Delay(DropProcessingDelayMilliseconds);

        await Assert.That(oversized.PlainText).IsEmpty();
    }

    /// <summary>Verifies that a dropped image file is embedded and inserted into the rich document.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_DroppedImageFile_EmbedsAndInsertsSupportedImage()
    {
        const string pngBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAIAAAACCAYAAABytg0kAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAQSURBVBhXY/jPwPCfARkAAB7zAf+x9MCaAAAAAElFTkSuQmCC";
        var imageBytes = Convert.FromBase64String(pngBase64);
        var droppedFile = CreateStorageFile(".png", imageBytes);

        var imageSource = await RichTextBox.TryCreateDroppedImageSourceAsync(droppedFile);
        var richTextBox = new RichTextBox();

        await Assert.That(imageSource).StartsWith("file:");
        await Assert.That(richTextBox.TryDropImage(imageSource)).IsTrue();

        var image = richTextBox.Document.Segments.FirstOrDefault(segment => segment.IsImage);
        await Assert.That(image).IsNotNull();
        await Assert.That(image!.ImageSource).IsEqualTo(imageSource);
    }

    /// <summary>Verifies that an image extension cannot cause content with the wrong signature to be inserted.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_DroppedImageFile_RejectsInvalidImageSignature()
    {
        var droppedFile = CreateStorageFile(".png", "not an image"u8.ToArray());

        var imageSource = await RichTextBox.TryCreateDroppedImageSourceAsync(droppedFile);

        await Assert.That(imageSource).IsNull();
    }

    /// <summary>Provides the FormattedTextPresenter_ImageInline_KeepsPreviousTextRunSplitSafe member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task FormattedTextPresenter_ImageInline_KeepsPreviousTextRunSplitSafe()
    {
        var document = new FlowDocument();
        const string imageSource = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAIAAAACCAYAAABytg0kAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAQSURBVBhXY/jPwPCfARkAAB7zAf+x9MCaAAAAAElFTkSuQmCC";
        var presenter = new FormattedTextPresenter();

        document.SetText($"abcdef<img src=\"{imageSource}\" width=\"{ExplicitImageWidth}\" height=\"{ExplicitImageWidth}\" />");
        presenter.Document = document;
        presenter.UpdateInlines();

        var firstRun = presenter.Inlines?.OfType<global::Avalonia.Controls.Documents.Run>().FirstOrDefault();
        await Assert.That(firstRun?.Text).IsEqualTo("abcdef\u200B");
    }

    /// <summary>Provides the FormattedTextPresenter_RemoteImages_AreDisabledByDefaultAndUseOptInLoader member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task FormattedTextPresenter_RemoteImages_AreDisabledByDefaultAndUseOptInLoader()
    {
        var document = new FlowDocument();
        var remoteLoaderCalls = 0;
        var loadedImage = new TestImage(new global::Avalonia.Size(RemoteImageNaturalWidth, RemoteImageNaturalHeight));
        var presenter = new FormattedTextPresenter
        {
            RemoteImageLoader = _ =>
            {
                remoteLoaderCalls++;
                return loadedImage;
            }
        };

        document.SetText("<img src=\"https://example.invalid/photo.png\" align=\"Right\" />");
        var imageSegment = document.Segments.Single(segment => segment.IsImage);

        await Assert.That(presenter.CreateImageElement(imageSegment)).IsNull();
        await Assert.That(remoteLoaderCalls).IsEqualTo(0);

        presenter.IsRemoteImageLoadingEnabled = true;
        var imageElement = presenter.CreateImageElement(imageSegment);

        await Assert.That(remoteLoaderCalls).IsEqualTo(1);
        await Assert.That(imageElement).IsNotNull();
        await Assert.That(imageElement!.Source).IsSameReferenceAs(loadedImage);
        await Assert.That(imageElement.Width).IsEqualTo(RemoteImageRenderedWidth);
        await Assert.That(imageElement.Height).IsEqualTo(RemoteImageRenderedHeight);
        await Assert.That(imageElement.HorizontalAlignment).IsEqualTo(HorizontalAlignment.Right);

        imageSegment.ImageWidth = ExplicitImageWidth;
        imageSegment.ImageHeight = ExplicitImageHeight;
        var explicitlySizedImage = presenter.CreateImageElement(imageSegment);
        await Assert.That(explicitlySizedImage!.Width).IsEqualTo(ExplicitImageWidth);
        await Assert.That(explicitlySizedImage.Height).IsEqualTo(ExplicitImageHeight);

        presenter.RemoteImageLoader = _ => throw new InvalidOperationException("Loader failure");
        await Assert.That(presenter.CreateImageElement(imageSegment)).IsNull();
    }

    /// <summary>Provides the RichTextBox_Clipboard_PreservesRichSelectionAndFallsBackToPlainTextPaste member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_Clipboard_PreservesRichSelectionAndFallsBackToPlainTextPaste()
    {
        var clipboard = new FakeRichTextClipboardAdapter();
        var richTextBox = new RichTextBox { ClipboardAdapter = clipboard };

        richTextBox.SetHtml("<strong>Hello</strong> <em>world</em>");
        richTextBox.Select(WorldStartOffset, WorldLength);
        richTextBox.Copy();

        await Assert.That(clipboard.PlainText).IsEqualTo("world");
        await Assert.That(clipboard.HtmlText).IsEqualTo("<em>world</em>");

        clipboard.HtmlText = null;
        clipboard.PlainText = "Avalonia";
        richTextBox.Select(HelloStartOffset, HelloLength);
        richTextBox.Paste();

        await Assert.That(richTextBox.PlainText).IsEqualTo("Avalonia world");
    }

    /// <summary>Provides the RichTextBox_Clipboard_CutAndPasteImagesHonorImagePolicyAndReadOnlyMode member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_Clipboard_CutAndPasteImagesHonorImagePolicyAndReadOnlyMode()
    {
        var clipboard = new FakeRichTextClipboardAdapter { ImageSource = "data:image/png;base64,AAAA" };
        var richTextBox = new RichTextBox { ClipboardAdapter = clipboard };

        await Assert.That(richTextBox.CanPaste).IsTrue();
        richTextBox.Paste();

        await Assert.That(richTextBox.Document.Segments.Any(segment => segment.IsImage && segment.ImageSource == "data:image/png;base64,AAAA")).IsTrue();

        var before = richTextBox.Html;
        clipboard.ImageSource = "data:image/png;base64,BBBB";
        richTextBox.IsImagePasteEnabled = false;
        richTextBox.Paste();
        await Assert.That(richTextBox.Html).IsEqualTo(before);

        richTextBox.IsImagePasteEnabled = true;
        richTextBox.IsReadOnly = true;
        richTextBox.Paste();
        await Assert.That(richTextBox.Html).IsEqualTo(before);
    }

    /// <summary>Provides the RichTextBox_DisplayMode_DisablesMutationsButPreservesContentAndCopySelection member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_DisplayMode_DisablesMutationsButPreservesContentAndCopySelection()
    {
        var clipboard = new FakeRichTextClipboardAdapter { PlainText = "blocked" };
        var richTextBox = new RichTextBox { ClipboardAdapter = clipboard };

        richTextBox.SetPlainText("Hello world");
        richTextBox.Select(HelloStartOffset, HelloLength);
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
        await Assert.That(richTextBox.PlainText).IsEqualTo("Hello world");

        richTextBox.EditMode = RichTextEditMode.Edit;
        richTextBox.Select(WorldStartOffset, WorldLength);
        richTextBox.Paste();
        await Assert.That(richTextBox.PlainText).IsEqualTo("Hello Hello");
    }

    /// <summary>Provides the AssertFormattingAsync member.</summary>
    /// <param name="apply">The apply value.</param>
    /// <param name="expectedHtml">The expectedHtml value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private static async Task AssertFormattingAsync(Action<RichTextBox> apply, string expectedHtml)
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetPlainText("Hello world");
        richTextBox.Select(WorldStartOffset, WorldLength);
        apply(richTextBox);

        await Assert.That(richTextBox.Html).IsEqualTo(expectedHtml);
        await Assert.That(richTextBox.PlainText).IsEqualTo("Hello world");
    }

    /// <summary>Provides the SingleTextSegment member.</summary>
    /// <param name="document">The document value.</param>
    /// <param name="text">The text value.</param>
    /// <returns>The result.</returns>
    private static TextSegment SingleTextSegment(FlowDocument document, string text) => document.Segments.Single(segment => segment.Text == text);

    /// <summary>Provides the GetSolidColor member.</summary>
    /// <param name="brush">The brush value.</param>
    /// <returns>The result.</returns>
    private static Color? GetSolidColor(IBrush? brush) => brush is SolidColorBrush solidColorBrush ? solidColorBrush.Color : null;

    /// <summary>Provides the RaiseDragOver member.</summary>
    /// <param name="richTextBox">The richTextBox value.</param>
    /// <param name="dataTransfer">The dataTransfer value.</param>
    /// <param name="point">The point value.</param>
    /// <returns>The result.</returns>
    private static DragEventArgs RaiseDragOver(RichTextBox richTextBox, IDataTransfer dataTransfer, global::Avalonia.Point point)
    {
        var args = new DragEventArgs(DragDrop.DragOverEvent, dataTransfer, richTextBox, point, KeyModifiers.None);
        richTextBox.RaiseEvent(args);
        return args;
    }

    /// <summary>Provides the RaiseDrop member.</summary>
    /// <param name="richTextBox">The richTextBox value.</param>
    /// <param name="dataTransfer">The dataTransfer value.</param>
    /// <param name="point">The point value.</param>
    /// <returns>The result.</returns>
    private static DragEventArgs RaiseDrop(RichTextBox richTextBox, IDataTransfer dataTransfer, global::Avalonia.Point point)
    {
        var args = new DragEventArgs(DragDrop.DropEvent, dataTransfer, richTextBox, point, KeyModifiers.None);
        richTextBox.RaiseEvent(args);
        return args;
    }

    /// <summary>Provides the CreateStorageFile member.</summary>
    /// <param name="extension">The extension value.</param>
    /// <param name="content">The content value.</param>
    /// <returns>The result.</returns>
    private static IStorageFile CreateStorageFile(string extension, string content)
    {
        var path = Path.Combine(Path.GetTempPath(), $"rtb-drop-{Guid.NewGuid():N}{extension}");
        File.WriteAllText(path, content, Encoding.UTF8);
        var storageFileType = typeof(IStorageFile).Assembly.GetType("Avalonia.Platform.Storage.FileIO.BclStorageFile", throwOnError: true)!;
        return (IStorageFile)Activator.CreateInstance(storageFileType, new FileInfo(path))!;
    }

    /// <summary>Creates an Avalonia storage file containing binary test data.</summary>
    /// <param name="extension">The extension value.</param>
    /// <param name="content">The binary content value.</param>
    /// <returns>The storage file.</returns>
    private static IStorageFile CreateStorageFile(string extension, byte[] content)
    {
        var path = Path.Combine(Path.GetTempPath(), $"rtb-drop-{Guid.NewGuid():N}{extension}");
        File.WriteAllBytes(path, content);
        var storageFileType = typeof(IStorageFile).Assembly.GetType("Avalonia.Platform.Storage.FileIO.BclStorageFile", throwOnError: true)!;
        return (IStorageFile)Activator.CreateInstance(storageFileType, new FileInfo(path))!;
    }

    /// <summary>Provides the FindMenuItem member.</summary>
    /// <param name="menu">The menu value.</param>
    /// <param name="header">The header value.</param>
    /// <returns>The result.</returns>
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

    /// <summary>Provides the InvokeMenuItem member.</summary>
    /// <param name="menuItem">The menuItem value.</param>
    private static void InvokeMenuItem(AvaloniaMenuItem menuItem) =>
        menuItem.RaiseEvent(new RoutedEventArgs(AvaloniaMenuItem.ClickEvent, menuItem));

    /// <summary>Provides the FlattenMenuItems member.</summary>
    /// <param name="source">The source value.</param>
    /// <returns>The result.</returns>
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

    /// <summary>An image test double that avoids requiring a platform renderer.</summary>
    /// <param name="size">The natural image size.</param>
    private sealed class TestImage(global::Avalonia.Size size) : IImage
    {
        /// <inheritdoc/>
        public global::Avalonia.Size Size { get; } = size;

        /// <inheritdoc/>
        public void Draw(DrawingContext context, global::Avalonia.Rect sourceRect, global::Avalonia.Rect destRect)
        {
        }
    }

    /// <summary>Provides the TestDataTransfer member.</summary>
    /// <param name="items">The items value.</param>
    /// <returns>The result.</returns>
    private sealed class TestDataTransfer(params IDataTransferItem[] items) : IDataTransfer
    {
        /// <summary>Provides the Empty member.</summary>
        public static readonly TestDataTransfer Empty = new();

        /// <summary>Gets the available formats.</summary>
        public IReadOnlyList<DataFormat> Formats { get; } = items.SelectMany(item => item.Formats).Distinct().ToArray();

        /// <summary>Gets the data transfer items.</summary>
        public IReadOnlyList<IDataTransferItem> Items { get; } = items;

        /// <summary>Provides the Text member.</summary>
        /// <param name="text">The text value.</param>
        /// <returns>The result.</returns>
        public static TestDataTransfer Text(string text) => new(new TestDataTransferItem(DataFormat.Text, text));

        /// <summary>Provides the File member.</summary>
        /// <param name="file">The file value.</param>
        /// <returns>The result.</returns>
        public static TestDataTransfer File(IStorageItem file) => new(new TestDataTransferItem(DataFormat.File, file));

        /// <summary>Provides the Dispose member.</summary>
        public void Dispose()
        {
        }
    }

    /// <summary>Provides the TestDataTransferItem member.</summary>
    /// <param name="format">The format value.</param>
    /// <param name="value">The raw value.</param>
    /// <returns>The result.</returns>
    private sealed class TestDataTransferItem(DataFormat format, object value) : IDataTransferItem
    {
        /// <summary>Gets the available formats.</summary>
        public IReadOnlyList<DataFormat> Formats { get; } = [format];

        /// <summary>Provides the TryGetRaw member.</summary>
        /// <param name="format">The format value.</param>
        /// <returns>The result.</returns>
        public object? TryGetRaw(DataFormat format) => Formats.Contains(format) ? value : null;
    }
}
