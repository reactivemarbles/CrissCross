// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls.Templates;
using CrissCross.Avalonia.UI.Controls;
using AvaloniaWindow = Avalonia.Controls.Window;
using ScrollViewer = Avalonia.Controls.ScrollViewer;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies measured page stories remain independent of body positions.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextStoryAutomationPeerTests
{
    /// <summary>Reads resolved story fields, rich semantics and measured bounds.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task PageStories_SeparateContent_ExposeReadOnlyValuesAndGeometry()
    {
        const double width = 300;
        const double height = 250;
        const double margin = 35;
        const string noteText = "Footnote reading text";
        var surface = new PagedDocumentPresenter();
        var scroll = new ScrollViewer { Content = surface };
        var editor = CreateEditor(surface, scroll, width, height, margin);
        editor.SetPlainText("Body text");
        editor.Select(0, 0);
        editor.InsertNote("1", noteText);
        editor.SetPageRegion("<a href='https://example.com'>Header link</a> <span data-page-field='PAGE'>0</span>/<span data-page-field='NUMPAGES'>0</span>");
        editor.SetPageRegion("Footer reading text", isFooter: true);
        var body = editor.Document.PlainText;
        var window = new AvaloniaWindow { Width = width, Height = height, Content = editor, ShowInTaskbar = false };
        try
        {
            window.Show();
            editor.ApplyTemplate();
            editor.UpdateLayout();
            var peer = new RichTextBoxAutomationPeer(editor);
            var header = FindStory(peer, "RichTextStory-1-Header");
            var footer = FindStory(peer, "RichTextStory-1-Footer");
            var notes = FindStory(peer, "RichTextStory-1-Footnotes");
            var headerValue = header.GetProvider<IValueProvider>() ?? throw new InvalidOperationException("The story value provider is absent.");
            await Assert.That(headerValue.IsReadOnly).IsTrue();
            await Assert.That(headerValue.Value).Contains("Header link 1/1");
            await Assert.That(() => headerValue.SetValue("Replacement")).Throws<InvalidOperationException>();
            await Assert.That(notes.GetProvider<IValueProvider>()?.Value).Contains(noteText);
            await Assert.That(footer.GetProvider<IValueProvider>()?.Value).Contains("Footer reading text");
            var link = FindHyperlink(header);
            await Assert.That(link.GetProvider<IInvokeProvider>()).IsNotNull();
            await Assert.That(link.GetHelpText()).IsEqualTo("https://example.com");
            await Assert.That(header.GetBoundingRectangle().Height > 0).IsTrue();
            await Assert.That(link.GetBoundingRectangle().Width > 0).IsTrue();
            await Assert.That(header.IsOffscreen()).IsFalse();
            await Assert.That(peer.DocumentRange.GetText()).IsEqualTo(body);
            await Assert.That(body.Contains("Header link", StringComparison.Ordinal)).IsFalse();
            await Assert.That(body.Contains(noteText, StringComparison.Ordinal)).IsFalse();
            var identity = header.GetAutomationId() ?? throw new InvalidOperationException("The story automation identifier is absent.");
            editor.SetPageRegion("Updated header");
            editor.UpdateLayout();
            var updated = FindStory(peer, identity);
            await Assert.That(updated.GetProvider<IValueProvider>()?.Value).Contains("Updated header");
            await Assert.That(updated.GetAutomationId()).IsEqualTo(identity);
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Creates the same composed page viewport used by the editor.</summary>
    /// <param name="surface">The page presenter.</param>
    /// <param name="scroll">The page viewport.</param>
    /// <param name="width">The page width.</param>
    /// <param name="height">The page height.</param>
    /// <param name="margin">The page margin.</param>
    /// <returns>The composed editor.</returns>
    private static RichTextBox CreateEditor(PagedDocumentPresenter surface, ScrollViewer scroll, double width, double height, double margin) => new()
    {
        IsPageLayoutEnabled = true,
        PageSize = new(width, height),
        PagePadding = new(margin),
        Template = new FuncControlTemplate<RichTextBox>((_, scope) =>
        {
            scope.Register("PART_FormattedPresenter", new FormattedTextPresenter { IsVisible = false });
            scope.Register("PART_PagedPresenter", surface);
            scope.Register("PART_PageScrollViewer", scroll);
            return scroll;
        }),
    };

    /// <summary>Finds a measured separate story by its stable identity.</summary>
    /// <param name="peer">The editor peer.</param>
    /// <param name="identity">The story identity.</param>
    /// <returns>The matching story.</returns>
    private static AutomationPeer FindStory(RichTextBoxAutomationPeer peer, string identity)
    {
        foreach (var child in peer.GetChildren())
        {
            if (child.GetAutomationId() == identity)
            {
                return child;
            }
        }

        throw new InvalidOperationException($"The measured page story '{identity}' is absent.");
    }

    /// <summary>Finds a hyperlink in the measured page story.</summary>
    /// <param name="story">The measured story.</param>
    /// <returns>The hyperlink peer.</returns>
    private static AutomationPeer FindHyperlink(AutomationPeer story)
    {
        foreach (var child in story.GetChildren())
        {
            if (child.GetAutomationControlType() == AutomationControlType.Hyperlink)
            {
                return child;
            }
        }

        throw new InvalidOperationException("The header hyperlink is absent.");
    }
}
