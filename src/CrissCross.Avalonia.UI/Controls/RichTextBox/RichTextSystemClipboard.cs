// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Media.Imaging;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Bridges rich text clipboard content to Avalonia's platform clipboard.</summary>
/// <param name="getClipboard">Resolves the clipboard for the current top-level control.</param>
internal sealed class RichTextSystemClipboard(Func<IClipboard?> getClipboard)
{
    /// <summary>The platform HTML clipboard format.</summary>
    private static readonly DataFormat<string> HtmlFormat = DataFormat.CreateStringPlatformFormat(
        OperatingSystem.IsWindows() ? "HTML Format" : "text/html");

    /// <summary>Gets a value indicating whether a platform clipboard is available.</summary>
    public bool IsAvailable => getClipboard() is not null;

    /// <summary>Writes plain and rich text representations to the platform clipboard.</summary>
    /// <param name="plainText">The plain-text representation.</param>
    /// <param name="htmlText">The optional HTML representation.</param>
    /// <returns>A task that completes when the clipboard has been updated.</returns>
    public async Task WriteAsync(string plainText, string? htmlText)
    {
        var clipboard = getClipboard();
        if (clipboard is null)
        {
            return;
        }

        var item = new DataTransferItem();
        item.SetText(plainText);
        if (!string.IsNullOrWhiteSpace(htmlText))
        {
            item.Set(HtmlFormat, HtmlClipboardUtilities.CreateClipboardHtml(htmlText));
        }

        var transfer = new DataTransfer();
        transfer.Add(item);
        await clipboard.SetDataAsync(transfer).ConfigureAwait(true);
        await clipboard.FlushAsync().ConfigureAwait(true);
    }

    /// <summary>Reads supported rich text representations from the platform clipboard.</summary>
    /// <returns>The clipboard content.</returns>
    public async Task<RichTextClipboardContent> ReadAsync()
    {
        var clipboard = getClipboard();
        if (clipboard is null)
        {
            return default;
        }

        using var transfer = await clipboard.TryGetDataAsync().ConfigureAwait(true);
        if (transfer is null)
        {
            return default;
        }

        var plainText = await transfer.TryGetTextAsync().ConfigureAwait(true);
        var htmlText = transfer.Contains(HtmlFormat)
            ? await transfer.TryGetValueAsync(HtmlFormat).ConfigureAwait(true)
            : null;

        string? imageSource = null;
        var bitmap = await transfer.TryGetBitmapAsync().ConfigureAwait(true);
        if (bitmap is not null)
        {
            using (bitmap)
            {
                await using var stream = new MemoryStream();
                bitmap.Save(stream, PngBitmapEncoderOptions.Default);
                imageSource = "data:image/png;base64," + Convert.ToBase64String(stream.ToArray());
            }
        }

        return new(plainText, htmlText, imageSource);
    }
}
