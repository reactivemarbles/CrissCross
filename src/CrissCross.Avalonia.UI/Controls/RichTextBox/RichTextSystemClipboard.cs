// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
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
    private static readonly DataFormat<byte[]> HtmlFormat = DataFormat.CreateBytesPlatformFormat(
        GetPlatformFormatName("HTML Format", "public.html", "text/html"));

    /// <summary>The platform RTF clipboard format.</summary>
    private static readonly DataFormat<byte[]> RtfFormat = DataFormat.CreateBytesPlatformFormat(
        GetPlatformFormatName("Rich Text Format", "public.rtf", "text/rtf"));

    /// <summary>Gets a value indicating whether a platform clipboard is available.</summary>
    /// <param name="clipboard">The platform clipboard.</param>
    /// <returns><see langword="true"/> when the clipboard is available.</returns>
    internal static bool IsAvailable(IClipboard? clipboard) => clipboard is not null;

    /// <summary>Writes plain and rich text representations to the platform clipboard.</summary>
    /// <param name="clipboard">The platform clipboard.</param>
    /// <param name="plainText">The plain-text representation.</param>
    /// <param name="htmlText">The optional HTML representation.</param>
    /// <returns>A task that completes when the clipboard has been updated.</returns>
    internal static async Task WriteAsync(IClipboard? clipboard, string plainText, string? htmlText)
    {
        if (clipboard is null)
        {
            return;
        }

        var item = new DataTransferItem();
        item.SetText(plainText);
        if (!string.IsNullOrWhiteSpace(htmlText))
        {
            htmlText = HtmlDocumentNormalizer.ToStandaloneFragment(htmlText);
            var payload = OperatingSystem.IsWindows() ? HtmlClipboardUtilities.CreateClipboardHtml(htmlText) : htmlText;
            item.Set(HtmlFormat, Encoding.UTF8.GetBytes(payload));
            if (RtfDocumentConverter.TryToRtf(htmlText) is { } rtf)
            {
                item.Set(RtfFormat, Encoding.Latin1.GetBytes(rtf));
            }
        }

        var transfer = new DataTransfer();
        transfer.Add(item);
        await clipboard.SetDataAsync(transfer).ConfigureAwait(true);
        await clipboard.FlushAsync().ConfigureAwait(true);
    }

    /// <summary>Reads supported rich text representations from the platform clipboard.</summary>
    /// <param name="clipboard">The platform clipboard.</param>
    /// <returns>The clipboard content.</returns>
    internal static async Task<RichTextClipboardContent> ReadAsync(IClipboard? clipboard)
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
        var htmlBytes = transfer.Contains(HtmlFormat)
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
                imageSource = $"data:image/png;base64,{Convert.ToBase64String(stream.ToArray())}";
            }
        }

        var rtfBytes = transfer.Contains(RtfFormat)
            ? await transfer.TryGetValueAsync(RtfFormat).ConfigureAwait(true)
            : null;
        return new(
            plainText,
            htmlBytes is null ? null : Encoding.UTF8.GetString(htmlBytes).TrimEnd('\0'),
            imageSource,
            rtfBytes is null ? null : Encoding.Latin1.GetString(rtfBytes).TrimEnd('\0'));
    }

    /// <summary>Gets the native clipboard identifier for the current platform.</summary>
    /// <param name="windows">The Windows identifier.</param>
    /// <param name="mac">The macOS uniform type identifier.</param>
    /// <param name="unix">The Unix MIME type.</param>
    /// <returns>The native identifier.</returns>
    private static string GetPlatformFormatName(string windows, string mac, string unix)
    {
        if (OperatingSystem.IsWindows())
        {
            return windows;
        }

        return OperatingSystem.IsMacOS() ? mac : unix;
    }
}
