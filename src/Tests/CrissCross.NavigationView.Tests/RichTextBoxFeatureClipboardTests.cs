// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Text;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CrissCross.Avalonia.UI.Controls;
using AvaloniaContextMenu = Avalonia.Controls.ContextMenu;
using AvaloniaMenuItem = Avalonia.Controls.MenuItem;

namespace CrissCross.NavigationView.Tests;

/// <summary>Comprehensive feature coverage for the Avalonia RichTextBox control surface.</summary>
public sealed partial class RichTextBoxFeatureTests
{
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
        var clipboard = new FakeRichTextClipboardAdapter { ImageSource = TestImageDataUri };
        var richTextBox = new RichTextBox { ClipboardAdapter = clipboard };

        await Assert.That(richTextBox.CanPaste).IsTrue();
        richTextBox.Paste();

        await Assert
            .That(
                richTextBox.Document.Segments.Any(segment =>
                    segment.IsImage && segment.ImageSource == TestImageDataUri))
            .IsTrue();

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

    /// <summary>Verifies the documented behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RichTextBox_DisplayMode_DisablesMutationsButPreservesContentAndCopySelection()
    {
        var clipboard = new FakeRichTextClipboardAdapter { PlainText = "blocked" };
        var richTextBox = new RichTextBox { ClipboardAdapter = clipboard };

        richTextBox.SetPlainText(HelloWorldText);
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
        await Assert.That(richTextBox.PlainText).IsEqualTo(HelloWorldText);

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

        richTextBox.SetPlainText(HelloWorldText);
        richTextBox.Select(WorldStartOffset, WorldLength);
        apply(richTextBox);

        await Assert.That(richTextBox.Html).IsEqualTo(expectedHtml);
        await Assert.That(richTextBox.PlainText).IsEqualTo(HelloWorldText);
    }

    /// <summary>Provides the SingleTextSegment member.</summary>
    /// <param name="document">The document value.</param>
    /// <param name="text">The text value.</param>
    /// <returns>The result.</returns>
    private static TextSegment SingleTextSegment(FlowDocument document, string text) =>
        document.Segments.Single(segment => segment.Text == text);

    /// <summary>Provides the GetSolidColor member.</summary>
    /// <param name="brush">The brush value.</param>
    /// <returns>The result.</returns>
    private static Color? GetSolidColor(IBrush? brush) =>
        brush is SolidColorBrush solidColorBrush ? solidColorBrush.Color : null;

    /// <summary>Provides the RaiseDragOver member.</summary>
    /// <param name="richTextBox">The richTextBox value.</param>
    /// <param name="dataTransfer">The dataTransfer value.</param>
    /// <param name="point">The point value.</param>
    /// <returns>The result.</returns>
    private static DragEventArgs RaiseDragOver(
        RichTextBox richTextBox,
        IDataTransfer dataTransfer,
        global::Avalonia.Point point)
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
    private static DragEventArgs RaiseDrop(
        RichTextBox richTextBox,
        IDataTransfer dataTransfer,
        global::Avalonia.Point point)
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
        var storageFileType = typeof(IStorageFile).Assembly.GetType(
            "Avalonia.Platform.Storage.FileIO.BclStorageFile",
            throwOnError: true)!;
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
        var storageFileType = typeof(IStorageFile).Assembly.GetType(
            "Avalonia.Platform.Storage.FileIO.BclStorageFile",
            throwOnError: true)!;
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
        public void Draw(DrawingContext context, global::Avalonia.Rect sourceRect, global::Avalonia.Rect destRect) { }
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
        public void Dispose() { }
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
