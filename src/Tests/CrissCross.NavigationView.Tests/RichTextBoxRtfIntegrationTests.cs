// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Text;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies RTF stream and clipboard interoperability.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextBoxRtfIntegrationTests
{
    /// <summary>The length of the replaced plain text.</summary>
    private const int ReplacedPlainTextLength = 3;

    /// <summary>The length of the pasted rich text.</summary>
    private const int PastedRichTextLength = 4;

    /// <summary>The length of the copied rich selection.</summary>
    private const int CopiedRichTextLength = 7;

    /// <summary>The initial prefix length before the copied rich selection.</summary>
    private const int RichTextPrefixLength = 7;

    /// <summary>The expected read-only selection start.</summary>
    private const int ReadOnlySelectionStart = 1;

    /// <summary>The prefix length before the selected clipboard text.</summary>
    private const int ClipboardSelectionStart = 6;

    /// <summary>Verifies a stream round trip preserves Unicode and rich styling.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task SaveLoad_RtfPreservesUnicodeAndBoldFormatting()
    {
        var source = new RichTextBox();
        source.SetHtml("<strong>日本語 😀 café</strong>");
        await using var stream = new MemoryStream();
        source.Save(stream, RichTextDataFormat.Rtf);
        await Assert.That(Encoding.UTF8.GetString(stream.ToArray())).StartsWith("{\\rtf");
        stream.Position = 0;
        var destination = new RichTextBox();
        destination.Load(stream, RichTextDataFormat.Rtf);
        await Assert.That(destination.PlainText).IsEqualTo("日本語 😀 café");
        destination.SelectAll();
        var allSelectedSegmentsAreBold = true;
        foreach (var segment in HtmlContentParser.Parse(destination.SelectedHtml))
        {
            allSelectedSegmentsAreBold &= segment.IsBold;
        }

        await Assert.That(allSelectedSegmentsAreBold).IsTrue();
    }

    /// <summary>Verifies external RTF without HTML or plain text is pasteable.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Paste_RtfOnlyClipboardReplacesSelectionAndSupportsHistory()
    {
        var clipboard = new RichTextMemoryClipboardAdapter { RtfText = "{\\rtf1\\ansi\\b Word\\b0}" };
        var editor = new RichTextBox { ClipboardAdapter = clipboard };
        editor.SetPlainText("Hello old");
        editor.Select(ClipboardSelectionStart, ReplacedPlainTextLength);
        await editor.PasteFromClipboardAsync();
        await Assert.That(editor.PlainText).IsEqualTo("Hello Word");
        editor.Select(ClipboardSelectionStart, PastedRichTextLength);
        var allSelectedSegmentsAreBold = true;
        foreach (var segment in HtmlContentParser.Parse(editor.SelectedHtml))
        {
            allSelectedSegmentsAreBold &= segment.IsBold;
        }

        await Assert.That(allSelectedSegmentsAreBold).IsTrue();
        editor.Undo();
        await Assert.That(editor.PlainText).IsEqualTo("Hello old");
        editor.Redo();
        await Assert.That(editor.PlainText).IsEqualTo("Hello Word");
    }

    /// <summary>Verifies rich copy supplies RTF that works without HTML.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Copy_RtfSelectionRemainsRichWhenHtmlIsUnavailable()
    {
        var clipboard = new RichTextMemoryClipboardAdapter();
        var source = new RichTextBox { ClipboardAdapter = clipboard };
        source.SetHtml("prefix <em>café 😀</em> suffix");
        source.Select(RichTextPrefixLength, CopiedRichTextLength);
        await source.CopyToClipboardAsync();
        await Assert.That(clipboard.RtfText).IsNotNull();
        clipboard.HtmlText = null;
        clipboard.PlainText = null;
        var destination = new RichTextBox { ClipboardAdapter = clipboard };
        await destination.PasteFromClipboardAsync();
        await Assert.That(destination.PlainText).IsEqualTo("café 😀");
        destination.SelectAll();
        var allSelectedSegmentsAreItalic = true;
        foreach (var segment in HtmlContentParser.Parse(destination.SelectedHtml))
        {
            allSelectedSegmentsAreItalic &= segment.IsItalic;
        }

        await Assert.That(allSelectedSegmentsAreItalic).IsTrue();
    }

    /// <summary>Verifies plain copying clears obsolete rich and image payloads.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Copy_PlainModeClearsStaleFormatsBeforePasting()
    {
        var clipboard = new RichTextMemoryClipboardAdapter { HtmlText = "<b>stale</b>", RtfText = "{\\rtf1 stale}", ImageSource = "data:image/png;base64,AQ==" };
        var source = new RichTextBox { ClipboardAdapter = clipboard, IsRichClipboardEnabled = false };
        source.SetPlainText("fresh");
        source.SelectAll();
        await source.CopyToClipboardAsync();
        await Assert.That(clipboard.HtmlText).IsNull();
        await Assert.That(clipboard.RtfText).IsNull();
        await Assert.That(clipboard.ImageSource).IsNull();
        var destination = new RichTextBox { ClipboardAdapter = clipboard };
        await destination.PasteFromClipboardAsync();
        await Assert.That(destination.PlainText).IsEqualTo("fresh");
    }

    /// <summary>Verifies read-only editors reject RTF clipboard mutations.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Paste_ReadOnlyPreservesDocumentAndSelection()
    {
        var clipboard = new RichTextMemoryClipboardAdapter { RtfText = "{\\rtf1 changed}" };
        var editor = new RichTextBox { ClipboardAdapter = clipboard, IsReadOnly = true };
        editor.SetPlainText("original");
        editor.Select(ReadOnlySelectionStart, ReplacedPlainTextLength);
        await editor.PasteFromClipboardAsync();
        await Assert.That(editor.PlainText).IsEqualTo("original");
        await Assert.That(editor.SelectedText).IsEqualTo("rig");
    }
}
