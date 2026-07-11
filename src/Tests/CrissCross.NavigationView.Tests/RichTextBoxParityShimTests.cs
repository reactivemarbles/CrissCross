// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Regression tests for the WPF interaction parity shims.</summary>
public sealed class RichTextBoxParityShimTests
{
    /// <summary>Verifies that plain-text typing does not expose or discard surrounding markup.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task EditingShim_InsertsRenderedTextWithoutExposingMarkup()
    {
        var document = new FlowDocument();
        document.SetText("<strong>Hello</strong> world");

        var changed = RichTextEditingShim.ApplyPlainTextChange(document, "Hello brave world");

        await Assert.That(changed).IsTrue();
        await Assert.That(document.PlainText).IsEqualTo("Hello brave world");
        await Assert.That(document.Text).Contains("<strong>Hello");
        await Assert.That(document.Text).DoesNotContain("&lt;strong&gt;");
    }

    /// <summary>Verifies that rendered-text deletion preserves formatting outside the changed range.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task EditingShim_DeletesRenderedRangeAndPreservesRemainingFormatting()
    {
        var document = new FlowDocument();
        document.SetText("<strong>Hello old</strong> <em>world</em>");

        var changed = RichTextEditingShim.ApplyPlainTextChange(document, "Hello world");

        await Assert.That(changed).IsTrue();
        await Assert.That(document.PlainText).IsEqualTo("Hello world");
        await Assert.That(document.Text).Contains("<strong>Hello </strong>");
        await Assert.That(document.Text).Contains("<em>world</em>");
    }

    /// <summary>Verifies that an unchanged editor snapshot does not mutate the document.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task EditingShim_UnchangedRenderedTextDoesNotMutateDocument()
    {
        var document = new FlowDocument();
        document.SetText("<strong>Hello</strong>");

        var changed = RichTextEditingShim.ApplyPlainTextChange(document, "Hello");

        await Assert.That(changed).IsFalse();
        await Assert.That(document.Text).IsEqualTo("<strong>Hello</strong>");
    }

    /// <summary>Verifies that replacing mapped paragraph breaks does not delete the same source tag twice.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task EditingShim_ReplacesMultiParagraphDocument()
    {
        var document = new FlowDocument();
        document.SetText("<p><strong>First</strong></p><p>Second</p>");

        var replacement = $"Replacement{Environment.NewLine}Line";
        var changed = RichTextEditingShim.ApplyPlainTextChange(document, replacement);

        await Assert.That(changed).IsTrue();
        await Assert.That(document.PlainText).IsEqualTo(replacement);
        await Assert.That(document.Text).IsEqualTo("Replacement<br />Line");
    }

    /// <summary>Verifies that HTML clipboard envelopes round-trip Unicode fragments.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task HtmlClipboardEnvelope_RoundTripsUnicodeFragment()
    {
        const string fragment = "<strong>CrissCross ©</strong>";

        var clipboardHtml = HtmlClipboardUtilities.CreateClipboardHtml(fragment);
        var extracted = HtmlClipboardUtilities.ExtractFragment(clipboardHtml);

        await Assert.That(clipboardHtml).Contains("StartHTML:");
        await Assert.That(clipboardHtml).Contains("StartFragment:");
        await Assert.That(extracted).IsEqualTo(fragment);
    }

    /// <summary>Verifies empty and plain clipboard payload handling.</summary>
    /// <param name="payload">The clipboard payload.</param>
    /// <param name="expected">The expected fragment.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    [Arguments(null, "")]
    [Arguments("   ", "")]
    [Arguments("plain html", "plain html")]
    public async Task HtmlClipboardEnvelope_HandlesEmptyAndUnwrappedPayloads(string? payload, string expected)
    {
        var extracted = HtmlClipboardUtilities.ExtractFragment(payload!);

        await Assert.That(extracted).IsEqualTo(expected);
    }

    /// <summary>Verifies that an empty fragment survives a Windows HTML clipboard round trip.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task HtmlClipboardEnvelope_RoundTripsEmptyFragment()
    {
        var clipboardHtml = HtmlClipboardUtilities.CreateClipboardHtml(string.Empty);

        await Assert.That(HtmlClipboardUtilities.ExtractFragment(clipboardHtml)).IsEmpty();
    }

    /// <summary>Verifies numeric offsets when a producer omits the optional fragment comments.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task HtmlClipboardEnvelope_UsesNumericOffsetsWithoutMarkers()
    {
        var payload = "StartFragment:0000000060\r\nEndFragment:0000000063\r\n".PadRight(60) + "abc";

        await Assert.That(HtmlClipboardUtilities.ExtractFragment(payload)).IsEqualTo("abc");
    }

    /// <summary>Verifies malformed numeric offsets leave the original payload unchanged.</summary>
    /// <param name="payload">The malformed payload.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    [Arguments("StartFragment:x\r\nEndFragment:3\r\nabc")]
    [Arguments("StartFragment:999999999999999999999\r\nEndFragment:3\r\nabc")]
    [Arguments("StartFragment:9\r\nEndFragment:3\r\nabc")]
    public async Task HtmlClipboardEnvelope_PreservesMalformedOffsetPayload(string payload)
    {
        await Assert.That(HtmlClipboardUtilities.ExtractFragment(payload)).IsEqualTo(payload);
    }

    /// <summary>Verifies plain text is HTML encoded and all newline forms become breaks.</summary>
    /// <param name="text">The plain text.</param>
    /// <param name="expected">The expected HTML.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    [Arguments(null, "")]
    [Arguments("", "")]
    [Arguments("<&>\r\nA\nB\rC", "&lt;&amp;&gt;<br />A<br />B<br />C")]
    public async Task HtmlClipboardEnvelope_EncodesPlainText(string? text, string expected)
    {
        await Assert.That(HtmlClipboardUtilities.EncodePlainText(text)).IsEqualTo(expected);
    }

    /// <summary>Verifies that HTML block boundaries remain visible to the editing surface.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task FlowDocument_ProjectsParagraphBoundariesAsNewLines()
    {
        var document = new FlowDocument();

        document.SetText("<p>First</p><p>Second</p>");

        await Assert.That(document.PlainText).IsEqualTo($"First{Environment.NewLine}Second{Environment.NewLine}");
    }
}
