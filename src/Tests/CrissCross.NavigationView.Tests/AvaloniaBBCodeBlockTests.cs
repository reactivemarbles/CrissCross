// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using CrissCross.Avalonia.UI.Controls;
using CrissCross.Avalonia.UI.Controls.BBCode;
using AvaloniaButton = Avalonia.Controls.Button;

namespace CrissCross.NavigationView.Tests;

/// <summary>Tests the Avalonia BBCodeBlock parser, renderer, interaction, and theme integration.</summary>
public sealed class AvaloniaBBCodeBlockTests
{
    /// <summary>The expected number of parsed document children.</summary>
    private const int ExpectedDocumentChildCount = 3;

    /// <summary>The expected number of safe rendered links.</summary>
    private const int ExpectedSafeLinkCount = 2;

    /// <summary>The expected number of blocked rendered links.</summary>
    private const int ExpectedBlockedLinkCount = 0;

    /// <summary>The expected font size from the size parameter.</summary>
    private const double ExpectedFontSize = 18D;

    /// <summary>The accepted precision for a rendered font size.</summary>
    private const double FontSizeTolerance = 0.001D;

    /// <summary>The minimum number of breaks contributed by layout markup.</summary>
    private const int MinimumLayoutLineBreakCount = 3;

    /// <summary>Verifies closing the current scope preserves the synthetic document root.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Parser_WhenClosingCurrentScope_PreservesDocumentRoot()
    {
        var document = new BbCodeParser("[b]bold[/b], [i]italic[/i]").Parse();

        await Assert.That(document.Name).IsEqualTo("root");
        await Assert.That(document.GetText()).IsEqualTo("bold, italic");
        await Assert.That(document.Children.Count).IsEqualTo(ExpectedDocumentChildCount);
    }

    /// <summary>Verifies nested formatting creates the expected span hierarchy.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task BBCode_WhenNestedFormattingIsProvided_RendersNestedSpans()
    {
        var block = new BBCodeBlock { BBCode = "[b]bold [i]italic[/i][/b]" };
        var hasBoldSpan = false;
        var hasItalicSpan = false;
        var renderedText = GetRenderedText(block.Inlines);

        foreach (var inline in EnumerateInlines(block.Inlines))
        {
            if (inline is not Span span)
            {
                continue;
            }

            hasBoldSpan |= span.FontWeight == FontWeight.Bold;
            hasItalicSpan |= span.FontStyle == FontStyle.Italic;
        }

        await Assert.That(hasBoldSpan).IsTrue();
        await Assert.That(hasItalicSpan).IsTrue();
        await Assert.That(renderedText).IsEqualTo("bold italic");
    }

    /// <summary>Verifies safe links are interactive and unsafe schemes remain normal text.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task BBCode_WhenLinksUseSafeSchemes_RendersInteractiveButtonsOnlyForSafeLinks()
    {
        var block = new BBCodeBlock { BBCode = "[url=https://example.com]web[/url] [email]user@example.com[/email] [url=javascript:alert(1)]blocked[/url]" };
        var buttonCount = 0;
        foreach (var inline in EnumerateInlines(block.Inlines))
        {
            if (inline is InlineUIContainer { Child: AvaloniaButton })
            {
                buttonCount++;
            }
        }

        await Assert.That(buttonCount).IsEqualTo(ExpectedSafeLinkCount);
        await Assert.That(GetRenderedText(block.Inlines)).Contains("blocked");
    }

    /// <summary>Verifies command links route their complete payload through ICommand.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task BBCode_WhenCommandLinkIsNavigated_ExecutesPayloadAndUpdatesCommandParameter()
    {
        var command = new RecordingCommand();
        var block = new BBCodeBlock { Command = command };

        block.Navigate(new("cmd:refresh:all"));

        await Assert.That(command.Parameter).IsEqualTo("refresh:all");
        await Assert.That(block.CommandParameter).IsEqualTo("refresh:all");
    }

    /// <summary>Verifies malformed and unknown markup remains readable rather than throwing.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task BBCode_WhenMarkupIsMalformed_PreservesReadableText()
    {
        var block = new BBCodeBlock { BBCode = "[b]open [widget]literal[/widget] [/i]" };
        var renderedText = GetRenderedText(block.Inlines);

        await Assert.That(renderedText).Contains("literal");
        await Assert.That(renderedText).Contains("[/i]");
    }

    /// <summary>Verifies links use a transparent surface and the native hyperlink class for both gallery themes.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task BBCode_WhenLinkIsRendered_UsesThemeCompatibleLinkPresentation()
    {
        var block = new BBCodeBlock { BBCode = "[url=cmd:theme-check]check[/url]" };
        AvaloniaButton? button = null;
        foreach (var inline in EnumerateInlines(block.Inlines))
        {
            if (inline is InlineUIContainer { Child: AvaloniaButton linkButton })
            {
                button = linkButton;
                break;
            }
        }

        ArgumentNullException.ThrowIfNull(button);

        await Assert.That(button.Background).IsEqualTo(Brushes.Transparent);
        await Assert.That(button.Classes.Contains("hyperlink")).IsTrue();
    }

    /// <summary>Verifies parameterized, layout, and alias markup render into the expected Avalonia inline tree.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task BBCode_WhenParameterizedAndLayoutMarkupIsProvided_RendersExpectedInlineProperties()
    {
        var block = new BBCodeBlock { BBCode = "[color=#FF112233]colour[/color][size=18]size[/size][font=Consolas]font[/font][p]paragraph[/p][li]item[/li][br][code]code[/code]" };
        var hasColor = false;
        var hasSize = false;
        var hasFont = false;
        var lineBreaks = 0;

        foreach (var inline in EnumerateInlines(block.Inlines))
        {
            if (inline is Span span)
            {
                hasColor |= span.Foreground is SolidColorBrush;
                hasSize |= Math.Abs(span.FontSize - ExpectedFontSize) <= FontSizeTolerance;
                hasFont |= span.FontFamily is not null;
            }

            if (inline is LineBreak)
            {
                lineBreaks++;
            }
        }

        await Assert.That(hasColor).IsTrue();
        await Assert.That(hasSize).IsTrue();
        await Assert.That(hasFont).IsTrue();
        await Assert.That(lineBreaks).IsGreaterThanOrEqualTo(MinimumLayoutLineBreakCount);
    }

    /// <summary>Verifies external link routing observes the opt-in setting and exposes the selected URI.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Navigate_WhenExternalLinksAreEnabled_RaisesExternalLinkRequested()
    {
        var block = new BBCodeBlock { OpenExternalLinks = true };
        var recorder = new ExternalLinkRecorder();
        block.ExternalLinkRequested += recorder.Record;

        Uri requestedUri = new("https://example.com/path");
        block.Navigate(requestedUri);

        await Assert.That(recorder.RequestedUri).IsSameReferenceAs(requestedUri);
        block.OpenExternalLinks = false;
        recorder.Reset();
        Uri blockedUri = new("https://example.com/blocked");
        block.Navigate(blockedUri);
        await Assert.That(recorder.RequestedUri).IsNull();
    }

    /// <summary>Verifies invalid parameter values and blocked link schemes preserve readable child text.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task BBCode_WhenParameterizedValuesAreInvalid_PreservesChildrenWithoutInteractiveLinks()
    {
        var block = new BBCodeBlock { BBCode = "[color=invalid]colour[/color][size=0]size[/size][font= ]font[/font][url=ftp://example.com]link[/url]" };
        var buttonCount = 0;

        foreach (var inline in EnumerateInlines(block.Inlines))
        {
            if (inline is InlineUIContainer { Child: AvaloniaButton })
            {
                buttonCount++;
            }
        }

        await Assert.That(GetRenderedText(block.Inlines)).IsEqualTo("coloursizefontlink");
        await Assert.That(buttonCount).IsEqualTo(ExpectedBlockedLinkCount);
    }

    /// <summary>Enumerates an inline tree in document order.</summary>
    /// <param name="inlines">The root inline collection.</param>
    /// <returns>The inline sequence.</returns>
    private static IEnumerable<Inline> EnumerateInlines(InlineCollection? inlines)
    {
        if (inlines is null)
        {
            yield break;
        }

        foreach (var inline in inlines)
        {
            yield return inline;
            if (inline is Span span)
            {
                foreach (var child in EnumerateInlines(span.Inlines))
                {
                    yield return child;
                }
            }
        }
    }

    /// <summary>Gets the text represented by run inlines.</summary>
    /// <param name="inlines">The root inline collection.</param>
    /// <returns>The rendered text.</returns>
    private static string GetRenderedText(InlineCollection? inlines)
    {
        var text = new StringBuilder();
        foreach (var inline in EnumerateInlines(inlines))
        {
            if (inline is Run run)
            {
                _ = text.Append(run.Text);
            }
        }

        return text.ToString();
    }

    /// <summary>Records command execution for command-link verification.</summary>
    private sealed class RecordingCommand : ICommand
    {
        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        /// <summary>Gets the most recent parameter.</summary>
        public string? Parameter { get; private set; }

        /// <inheritdoc/>
        public bool CanExecute(object? parameter) => true;

        /// <inheritdoc/>
        public void Execute(object? parameter) => Parameter = parameter as string;
    }

    /// <summary>Records external BBCode link requests.</summary>
    private sealed class ExternalLinkRecorder
    {
        /// <summary>Gets the most recently requested URI.</summary>
        public Uri? RequestedUri { get; private set; }

        /// <summary>Records an external link request.</summary>
        /// <param name="sender">The source BBCode block.</param>
        /// <param name="eventArgs">The requested link details.</param>
        public void Record(object? sender, BBCodeLinkRequestedEventArgs eventArgs) => RequestedUri = eventArgs.Uri;

        /// <summary>Clears the recorded URI.</summary>
        public void Reset() => RequestedUri = null;
    }
}
