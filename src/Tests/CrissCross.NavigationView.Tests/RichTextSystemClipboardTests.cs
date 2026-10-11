// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text;
using Avalonia.Input;
using Avalonia.Input.Platform;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies native rich clipboard formats contain correctly encoded bytes.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextSystemClipboardTests
{
    /// <summary>The missing native clipboard diagnostic.</summary>
    private const string ClipboardUnavailable = "The native clipboard is unavailable.";

    /// <summary>The Windows HTML byte format used by Word.</summary>
    private static readonly DataFormat<byte[]> HtmlFormat = DataFormat.CreateBytesPlatformFormat("HTML Format");

    /// <summary>The Windows RTF byte format used by Word.</summary>
    private static readonly DataFormat<byte[]> RtfFormat = DataFormat.CreateBytesPlatformFormat("Rich Text Format");

    /// <summary>Verifies a full Word document exports one valid HTML body and styled RTF.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task FullDocumentExportPreservesStylesWithoutNestedHtml()
    {
        const string html = "<html><head><style>p.MsoNormal{font-family:Consolas;color:red;background-color:yellow;}</style></head><body><p class='MsoNormal'>Styled</p></body></html>";
        var window = new global::Avalonia.Controls.Window();
        try
        {
            var clipboard = window.Clipboard ?? throw new InvalidOperationException(ClipboardUnavailable);
            await RichTextSystemClipboard.WriteAsync(clipboard, "Styled", html);
            using var data = await clipboard.TryGetDataAsync() ?? throw new InvalidOperationException("Clipboard data is missing.");
            var htmlBytes = await data.TryGetValueAsync(HtmlFormat);
            var rtfBytes = await data.TryGetValueAsync(RtfFormat);
            var fragment = HtmlClipboardUtilities.ExtractFragment(Encoding.UTF8.GetString(htmlBytes!));
            await Assert.That(fragment.Contains("<html", StringComparison.OrdinalIgnoreCase)).IsFalse();
            var roundTrip = RtfDocumentConverter.ToHtml(Encoding.Latin1.GetString(rtfBytes!));
            await Assert.That(roundTrip.Contains("Consolas", StringComparison.Ordinal)).IsTrue();
            await Assert.That(roundTrip.Contains("#ff0000", StringComparison.OrdinalIgnoreCase)).IsTrue();
            await Assert.That(roundTrip.Contains("#ffff00", StringComparison.OrdinalIgnoreCase)).IsTrue();
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies Word's UTF8 HTML is decoded independently of platform string encoding.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ReadWordByteFormatsPreservesUnicodeAndRtf()
    {
        const string fragment = "<p>日本語 😀 café</p>";
        const string rtf = "{\\rtf1\\ansi\\b Word\\b0}";
        var item = new DataTransferItem();
        item.Set(HtmlFormat, Encoding.UTF8.GetBytes(HtmlClipboardUtilities.CreateClipboardHtml(fragment) + '\0'));
        item.Set(RtfFormat, Encoding.Latin1.GetBytes(rtf + '\0'));
        var transfer = new DataTransfer();
        transfer.Add(item);
        var window = new global::Avalonia.Controls.Window();
        try
        {
            var clipboard = window.Clipboard ?? throw new InvalidOperationException(ClipboardUnavailable);
            await clipboard.SetDataAsync(transfer);
            var content = await RichTextSystemClipboard.ReadAsync(clipboard);
            await Assert.That(HtmlClipboardUtilities.ExtractFragment(content.HtmlText!)).IsEqualTo(fragment);
            await Assert.That(content.RtfText).IsEqualTo(rtf);
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies native export writes UTF8 HTML and ASCII-compatible RTF rather than UTF16.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task WriteNativeByteFormatsCanBeReadByOtherApplications()
    {
        const string fragment = "<b>日本語 😀 café</b>";
        var window = new global::Avalonia.Controls.Window();
        try
        {
            var clipboard = window.Clipboard ?? throw new InvalidOperationException(ClipboardUnavailable);
            await RichTextSystemClipboard.WriteAsync(clipboard, "日本語 😀 café", fragment);
            using var data = await clipboard.TryGetDataAsync() ?? throw new InvalidOperationException("Clipboard data is missing.");
            var html = await data.TryGetValueAsync(HtmlFormat);
            var rtf = await data.TryGetValueAsync(RtfFormat);
            await Assert.That(html).IsNotNull();
            await Assert.That(rtf).IsNotNull();
            await Assert.That(HtmlClipboardUtilities.ExtractFragment(Encoding.UTF8.GetString(html!))).IsEqualTo(fragment);
            var roundTrip = new FlowDocument();
            roundTrip.SetText(RtfDocumentConverter.ToHtml(Encoding.Latin1.GetString(rtf!)));
            await Assert.That(roundTrip.PlainText).IsEqualTo("日本語 😀 café");
        }
        finally
        {
            window.Close();
        }
    }
}
