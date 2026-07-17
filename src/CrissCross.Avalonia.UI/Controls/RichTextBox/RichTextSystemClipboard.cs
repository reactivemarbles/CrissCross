// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Media.Imaging;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Bridges rich text clipboard content to Avalonia's platform clipboard.</summary>
internal static class RichTextSystemClipboard
{
    /// <summary>The platform HTML clipboard format.</summary>
    private static readonly DataFormat<string> HtmlFormat = DataFormat.CreateStringPlatformFormat(
        OperatingSystem.IsWindows() ? "HTML Format" : "text/html");

    /// <summary>Gets a value indicating whether a platform clipboard is available.</summary>
    /// <param name="clipboard">The platform clipboard.</param>
    /// <returns><see langword="true"/> when the clipboard is available.</returns>
    public static bool IsAvailable(IClipboard? clipboard) => clipboard is not null;

    /// <summary>Writes plain and rich text representations to the platform clipboard.</summary>
    /// <param name="clipboard">The platform clipboard.</param>
    /// <param name="plainText">The plain-text representation.</param>
    /// <param name="htmlText">The optional HTML representation.</param>
    /// <returns>A task that completes when the clipboard has been updated.</returns>
    public static async Task WriteAsync(IClipboard? clipboard, string plainText, string? htmlText)
    {
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
    /// <param name="clipboard">The platform clipboard.</param>
    /// <returns>The clipboard content.</returns>
    public static async Task<RichTextClipboardContent> ReadAsync(IClipboard? clipboard)
    {
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
