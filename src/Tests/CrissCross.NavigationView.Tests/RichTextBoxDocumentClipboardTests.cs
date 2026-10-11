// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Text;
using Avalonia.Media;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies document structure survives rich clipboard selections.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextBoxDocumentClipboardTests
{
    /// <summary>The expected inherited font size in points.</summary>
    private const double ExpectedFontSize = 24;

    /// <summary>The start offset of the second word in the partial selection fixture.</summary>
    private const int PartialSelectionStart = 6;

    /// <summary>The partial selection length.</summary>
    private const int PartialSelectionLength = 5;

    /// <summary>The cross-run selection start.</summary>
    private const int CrossRunSelectionStart = 3;

    /// <summary>The cross-run selection length.</summary>
    private const int CrossRunSelectionLength = 6;

    /// <summary>The selected fragment header template parsed once for formatting.</summary>
    private static readonly CompositeFormat HeaderFormat = CompositeFormat.Parse("Version:1.0\r\nStartFragment:{0:D10}\r\nEndFragment:{1:D10}\r\n");

    /// <summary>Verifies rendering stylesheet HTML does not change raw document editing coordinates.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task StylesheetRenderingPreservesExistingTextCoordinates()
    {
        var editor = new RichTextBox();
        editor.SetHtml("<style>p{font-family:Arial}</style><p>One\nTwo</p>");
        await Assert.That(editor.Document.Segments[0].Text).IsEqualTo("One\nTwo");
        await Assert.That(editor.PlainText.StartsWith("One\nTwo", StringComparison.Ordinal)).IsTrue();
        editor.Select("One\n".Length, "Two".Length);
        editor.ReplaceSelection("Changed");
        await Assert.That(editor.PlainText.StartsWith("One\nChanged", StringComparison.Ordinal)).IsTrue();
    }

    /// <summary>Verifies imported source wrapping retains a separating space between inline runs.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task WordSourceWhitespaceBetweenInlineRunsPreservesWordBoundary()
    {
        const string html = "<style>p{font-family:Arial}</style><p><b>One</b>\r\n<i>Two</i></p>\r\n<p>Three</p>";
        var editor = new RichTextBox { ClipboardAdapter = new RichTextMemoryClipboardAdapter { HtmlText = html } };
        await editor.PasteFromClipboardAsync();
        await Assert.That(editor.PlainText.StartsWith("One Two", StringComparison.Ordinal)).IsTrue();
        await Assert.That(editor.PlainText.Contains("Two\r\nThree", StringComparison.Ordinal)).IsTrue();
    }

    /// <summary>Verifies Word's shorthand highlight declaration becomes a visible run background.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task WordBackgroundShorthandPreservesHighlight()
    {
        var editor = new RichTextBox();
        editor.SetHtml("<span style='background:yellow;mso-highlight:yellow'>Highlighted</span>");
        await Assert.That((editor.Document.Segments[0].Background as ISolidColorBrush)?.Color).IsEqualTo(Colors.Yellow);
    }

    /// <summary>Verifies Word source wrapping does not introduce document line breaks.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task WordSourceWhitespaceCollapsesWhilePreformattedTextIsPreserved()
    {
        const string html = "<style>p.MsoNormal{font-family:Arial;}</style><p class='MsoNormal'>CrissCross\r\nWord <b>fixture</b></p>\r\n<pre>one\ntwo</pre>";
        var editor = new RichTextBox { ClipboardAdapter = new RichTextMemoryClipboardAdapter { HtmlText = html } };
        await editor.PasteFromClipboardAsync();
        await Assert.That(editor.PlainText.StartsWith("CrissCross Word fixture", StringComparison.Ordinal)).IsTrue();
        await Assert.That(editor.PlainText.Contains("one\ntwo", StringComparison.Ordinal)).IsTrue();
        editor.SelectAll();
        editor.ReplaceSelection("Replacement");
        await Assert.That(editor.PlainText).IsEqualTo("Replacement");
    }

    /// <summary>Verifies a full selection retains tables, links, images and page metadata.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task FullDocumentCopyPreservesSourceStructure()
    {
        const string html = "<div style='page-break-before:always'><p><a href='https://avaloniaui.net'>Link</a></p>"
            + "<table><tr><td style='color:red'>Cell</td></tr></table><img src='data:image/png;base64,AQ==' width='40'></div>";
        var clipboard = new RichTextMemoryClipboardAdapter();
        var editor = new RichTextBox { ClipboardAdapter = clipboard };
        editor.SetHtml(html);
        editor.SelectAll();
        await editor.CopyToClipboardAsync();
        await Assert.That(clipboard.HtmlText).IsEqualTo(html);
        var destination = new RichTextBox { ClipboardAdapter = clipboard };
        await destination.PasteFromClipboardAsync();
        await Assert.That(destination.Html).IsEqualTo(html);
    }

    /// <summary>Verifies a partial link selection retains ancestor attributes.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task PartialSelectionPreservesInheritedFormattingAndHyperlink()
    {
        const string html = "<span style='color:red;font-family:Arial'><a href='https://example.com'><strong>Hello world</strong></a></span>";
        var editor = new RichTextBox();
        editor.SetHtml(html);
        editor.Select(PartialSelectionStart, PartialSelectionLength);
        await Assert.That(editor.SelectedHtml).IsEqualTo("<span style='color:red;font-family:Arial'><a href='https://example.com'><strong>world</strong></a></span>");
    }

    /// <summary>Verifies clipboard offsets count UTF8 bytes rather than UTF16 characters.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ClipboardByteOffsetsSupportUnicodeWithoutFragmentMarkers()
    {
        const string fragment = "<p>日本語 😀 café</p>";
        const string prefix = "<html><body>é";
        const string suffix = "</body></html>";
        var placeholder = string.Format(CultureInfo.InvariantCulture, HeaderFormat, 0, 0);
        var start = Encoding.UTF8.GetByteCount(placeholder + prefix);
        var end = start + Encoding.UTF8.GetByteCount(fragment);
        var payload = string.Format(CultureInfo.InvariantCulture, HeaderFormat, start, end) + prefix + fragment + suffix;
        await Assert.That(HtmlClipboardUtilities.ExtractFragment(payload)).IsEqualTo(fragment);
    }

    /// <summary>Verifies selections spanning different ancestor elements remain balanced.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task CrossRunSelectionPreservesBothRunStyles()
    {
        var editor = new RichTextBox();
        editor.SetHtml("<b>Hello</b> <i>world</i>");
        editor.Select(CrossRunSelectionStart, CrossRunSelectionLength);
        await Assert.That(editor.SelectedHtml).IsEqualTo("<b>lo</b> <i>wor</i>");
    }

    /// <summary>Verifies Word-style class declarations survive fragment extraction.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ClipboardRetainsStylesheetOutsideSelectedFragment()
    {
        const string payload = "<html><head><style>p.MsoNormal{font-family:Arial;color:#ff0000;font-size:18pt;}</style></head>"
            + "<body><!--StartFragment--><p class='MsoNormal'>Word text</p><!--EndFragment--></body></html>";
        var editor = new RichTextBox { ClipboardAdapter = new RichTextMemoryClipboardAdapter { HtmlText = payload } };
        await editor.PasteFromClipboardAsync();
        TextSegment? matchingSegment = null;
        foreach (var segment in editor.Document.Segments)
        {
            if (segment.Text == "Word text")
            {
                matchingSegment = segment;
                break;
            }
        }

        await Assert.That(matchingSegment).IsNotNull();
        var run = matchingSegment!;
        await Assert.That(run.FontSize).IsEqualTo(ExpectedFontSize);
        await Assert.That(run.FontFamily?.Name).IsEqualTo("Arial");
        await Assert.That((run.Foreground as ISolidColorBrush)?.Color).IsEqualTo(Colors.Red);
    }

    /// <summary>Verifies Office metadata does not consume editable document positions.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task OfficeConditionalCommentsDoNotBecomeEditableText()
    {
        var editor = new RichTextBox();
        editor.SetHtml("<!--[if gte mso 9]><xml><o:DocumentProperties>Metadata</o:DocumentProperties></xml><![endif]--><b>Hello</b>");
        await Assert.That(editor.PlainText).IsEqualTo("Hello");
        editor.SelectAll();
        editor.ReplaceSelection("World");
        await Assert.That(editor.PlainText).IsEqualTo("World");
        editor.Undo();
        await Assert.That(editor.PlainText).IsEqualTo("Hello");
    }
}
