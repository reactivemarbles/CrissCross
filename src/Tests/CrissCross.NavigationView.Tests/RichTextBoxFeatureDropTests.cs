// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Input;
using Avalonia.Layout;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Comprehensive feature coverage for the Avalonia RichTextBox control surface.</summary>
public sealed partial class RichTextBoxFeatureTests
{
    /// <summary>Verifies the documented behavior.</summary>
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

    /// <summary>Verifies the documented behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_DropImage_AcceptsImagePathsAndDataUrisButRejectsUnsupportedImages()
    {
        var richTextBox = new RichTextBox();

        await Assert.That(richTextBox.TryDropImage(TestImageFileUri)).IsTrue();
        await Assert
            .That(
                richTextBox.Document.Segments.Any(segment =>
                    segment.IsImage && segment.ImageSource == TestImageFileUri))
            .IsTrue();

        await Assert.That(richTextBox.TryDropImage(TestImageDataUri)).IsTrue();
        await Assert
            .That(
                richTextBox.Document.Segments.Any(segment =>
                    segment.IsImage && segment.ImageSource == TestImageDataUri))
            .IsTrue();

        var before = richTextBox.Html;
        await Assert.That(richTextBox.TryDropImage("file:///tmp/readme.txt")).IsFalse();
        richTextBox.IsImageDropEnabled = false;
        await Assert.That(richTextBox.TryDropImage("file:///tmp/blocked.png")).IsFalse();
        await Assert.That(richTextBox.Html).IsEqualTo(before);
    }

    /// <summary>Verifies the documented behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_DropImage_ConsumesSingleRenderedPositionAndKeepsFollowingDropsOrdered()
    {
        var richTextBox = new RichTextBox();

        richTextBox.SetPlainText(AlphabetText);
        richTextBox.Select(richTextBox.Document.Length, 0);
        await Assert.That(richTextBox.TryDropImage(TestImageFileUri)).IsTrue();
        richTextBox.ReplaceSelection("Z");

        await Assert.That(richTextBox.Document.Length).IsEqualTo(ImageDropDocumentLength);
        await Assert.That(richTextBox.CaretIndex).IsEqualTo(ImageDropDocumentLength);
        await Assert.That(richTextBox.Html).IsEqualTo($"{AlphabetText}<img src=\"{TestImageFileUri}\" />Z");
    }

    /// <summary>Provides the RichTextBox_GetPositionFromPoint_MapsPointToRenderedDocumentOffset member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_GetPositionFromPoint_MapsPointToRenderedDocumentOffset()
    {
        var richTextBox = new RichTextBox { Width = HitTestWidth, Height = HitTestHeight };

        richTextBox.SetPlainText(AlphabetText);
        richTextBox.Select(richTextBox.Document.Length, 0);

        await Assert
            .That(richTextBox.GetPositionFromPoint(new global::Avalonia.Point(0, 0), snapToText: true)?.Offset)
            .IsEqualTo(0);
        await Assert
            .That(
                richTextBox
                    .GetPositionFromPoint(new global::Avalonia.Point(FarRightPointX, 0), snapToText: true)
                    ?.Offset)
            .IsEqualTo(richTextBox.Document.Length);
        await Assert
            .That(richTextBox.GetPositionFromPoint(new global::Avalonia.Point(OutsideLeftPointX, 0), snapToText: false))
            .IsNull();
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

        richTextBox.SetPlainText(AlphabetText);
        richTextBox.Select(richTextBox.Document.Length, 0);

        _ = RaiseDrop(richTextBox, TestDataTransfer.Text("XX"), new global::Avalonia.Point(0, 0));

        await Assert.That(richTextBox.PlainText).IsEqualTo($"XX{AlphabetText}");
        await Assert.That(richTextBox.CaretIndex).IsEqualTo(DroppedTextLength);
    }

    /// <summary>Provides the RichTextBox_RuntimeDrop_EnforcesImageAndTextFilePolicies member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_RuntimeDrop_EnforcesImageAndTextFilePolicies()
    {
        var imageDisabled = new RichTextBox { IsImageDropEnabled = false };

        _ = RaiseDrop(
            imageDisabled,
            TestDataTransfer.File(CreateStorageFile(".png", string.Empty)),
            new global::Avalonia.Point(0, 0));

        await Assert.That(imageDisabled.Html).DoesNotContain("<img");

        var unsupported = new RichTextBox();
        _ = RaiseDrop(
            unsupported,
            TestDataTransfer.File(CreateStorageFile(".exe", "should-not-load")),
            new global::Avalonia.Point(0, 0));
        await Task.Delay(DropProcessingDelayMilliseconds);

        await Assert.That(unsupported.PlainText).IsEmpty();

        var oversized = new RichTextBox { MaxDroppedTextFileBytes = DroppedTextFileByteLimit };
        _ = RaiseDrop(
            oversized,
            TestDataTransfer.File(CreateStorageFile(".txt", "12345")),
            new global::Avalonia.Point(0, 0));
        await Task.Delay(DropProcessingDelayMilliseconds);

        await Assert.That(oversized.PlainText).IsEmpty();
    }

    /// <summary>Verifies that a dropped image file is embedded and inserted into the rich document.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_DroppedImageFile_EmbedsAndInsertsSupportedImage()
    {
        const string pngBase64 =
            "iVBORw0KGgoAAAANSUhEUgAAAAIAAAACCAYAAABytg0kAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQU"
            + "AAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAQSURBVBhXY/jPwPCfARkAAB7zAf+x9MCaAAAAAElFTkSuQmCC";
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

    /// <summary>Provides the RichTextBox_DroppedImageFile_RejectsInvalidImageSignature member.</summary>
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
        const string imageSource =
            "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAIAAAACCAYAAABytg0kAAAAAXNSR0IArs4c6QAAA"
            + "ARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAQSURBVBhXY/jPwPCfARkAAB7zAf+x9MCaAAAAAElFTkSuQmCC";
        var presenter = new FormattedTextPresenter();

        document.SetText(
            $"{AlphabetText}<img src=\"{imageSource}\" "
                + $"width=\"{ExplicitImageWidth}\" height=\"{ExplicitImageWidth}\" />");
        presenter.Document = document;
        presenter.UpdateInlines();

        var firstRun = presenter.Inlines?.OfType<global::Avalonia.Controls.Documents.Run>().FirstOrDefault();
        await Assert.That(firstRun?.Text).IsEqualTo($"{AlphabetText}\u200B");
    }

    /// <summary>Verifies the documented behavior.</summary>
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
            },
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
}
