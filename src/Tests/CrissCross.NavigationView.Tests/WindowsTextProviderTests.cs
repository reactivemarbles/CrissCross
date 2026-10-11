// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Versioning;
using Avalonia.Controls.Templates;
using CrissCross.Avalonia.UI.Controls;
using CrissCross.Avalonia.UI.Controls.Automation.Windows;

namespace CrissCross.NavigationView.Tests;

/// <summary>Exercises measured native text visibility through real scroll viewports.</summary>
[SupportedOSPlatform("windows")]
[RequiresDynamicCode("Tests exercise the native Windows COM provider.")]
[RequiresUnreferencedCode("Tests exercise the native Windows COM provider.")]
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class WindowsTextProviderTests
{
    /// <summary>Clips wrapped text on later paper sheets to the arranged viewport.</summary>
    /// <param name="repetitions">The number of wrapped text phrases.</param>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    [Arguments(8)]
    [Arguments(80)]
    public async Task GetAutomationVisibleRectangles_WrappedScrolledPages_ClipsToViewport(int repetitions)
    {
        const double width = 220;
        const double height = 120;
        const double pageHeight = 160;
        const double pagePadding = 10;
        const double scrollDistance = 180;
        var surface = new PagedDocumentPresenter();
        var scroll = new global::Avalonia.Controls.ScrollViewer { Content = surface };
        var editor = CreateEditor(surface, scroll);
        editor.PageSize = new(width, pageHeight);
        editor.PagePadding = new(pagePadding);
        var text = new System.Text.StringBuilder();
        for (var index = 0; index < repetitions; index++)
        {
            _ = text.Append("Wrapped document words ");
        }

        editor.SetPlainText(text.ToString());
        var window = new global::Avalonia.Controls.Window { Width = width, Height = height, Content = editor, ShowInTaskbar = false };
        try
        {
            window.Show();
            editor.ApplyTemplate();
            editor.UpdateLayout();
            scroll.Offset = new(0, scrollDistance);
            editor.UpdateLayout();
            var visible = editor.GetAutomationVisibleRectangles(0, editor.Document.Length);
            var measured = editor.GetAutomationRangeRectangles(0, editor.Document.Length);
            await Assert.That(surface.PageCount > 1).IsTrue();
            await Assert.That(visible.Count > 0).IsTrue();
            await Assert.That(visible.Count < measured.Count).IsTrue();
            foreach (var rectangle in visible)
            {
                await Assert.That(rectangle.Top >= 0 && rectangle.Bottom <= window.Bounds.Height).IsTrue();
            }

            await Assert.That(editor.GetAutomationVisibleRectangles(0, 1).Count).IsEqualTo(0);
            using var context = new WindowsAutomationContext(window);
            var provider = new WindowsTextProvider(context.Node(new RichTextBoxAutomationPeer(editor)));
            var ranges = provider.GetVisibleRanges();
            await Assert.That(ranges.Length > 0).IsTrue();
            await VerifyVisibleGraphemes(editor, provider, ranges);
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Reports a measured empty-document insertion point only while its editor is visible.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task IsAutomationCaretVisible_EmptyDocument_TracksEditorVisibility()
    {
        const double width = 400;
        const double height = 200;
        var surface = new PagedDocumentPresenter();
        var scroll = new global::Avalonia.Controls.ScrollViewer { Content = surface };
        var editor = CreateEditor(surface, scroll);
        var window = new global::Avalonia.Controls.Window { Width = width, Height = height, Content = editor, ShowInTaskbar = false };
        try
        {
            window.Show();
            editor.ApplyTemplate();
            editor.UpdateLayout();
            await Assert.That(editor.IsAutomationCaretVisible()).IsTrue();
            using var context = new WindowsAutomationContext(window);
            var provider = new WindowsTextProvider(context.Node(new RichTextBoxAutomationPeer(editor)));
            await Assert.That(provider.GetVisibleRanges().Length).IsEqualTo(1);
            editor.IsVisible = false;
            await Assert.That(editor.IsAutomationCaretVisible()).IsFalse();
            await Assert.That(provider.GetVisibleRanges().Length).IsEqualTo(0);
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Compares native range coverage with independently measured grapheme visibility.</summary>
    /// <param name="editor">The measured editor.</param>
    /// <param name="provider">The native text provider.</param>
    /// <param name="ranges">The returned visible ranges.</param>
    /// <returns>The assertion task.</returns>
    private static async Task VerifyVisibleGraphemes(RichTextBox editor, WindowsTextProvider provider, IWindowsTextRangeProvider[] ranges)
    {
        const int maximumOraclePositions = 200;
        var document = provider.GetDocumentRange();
        var bounds = new List<(int Start, int End)>();
        foreach (var range in ranges)
        {
            bounds.Add((range.CompareEndpoints(WindowsTextPatternRangeEndpoint.Start, document, WindowsTextPatternRangeEndpoint.Start),
                range.CompareEndpoints(WindowsTextPatternRangeEndpoint.End, document, WindowsTextPatternRangeEndpoint.Start)));
        }

        var positions = StringInfo.ParseCombiningCharacters(editor.Document.PlainText);
        var first = Math.Max(0, bounds[0].Start - 1);
        foreach (var position in positions)
        {
            if (position < first || position >= first + maximumOraclePositions)
            {
                continue;
            }

            var next = position + StringInfo.GetNextTextElementLength(editor.Document.PlainText, position);
            var included = false;
            foreach (var bound in bounds)
            {
                included |= position >= bound.Start && next <= bound.End;
            }

            await Assert.That(included).IsEqualTo(editor.GetAutomationVisibleRectangles(position, next - position).Count > 0);
        }
    }

    /// <summary>Creates an editor template with an actual paged scroll viewport.</summary>
    /// <param name="surface">The measured page presenter.</param>
    /// <param name="scroll">The containing scroll viewport.</param>
    /// <returns>The rich editor.</returns>
    private static RichTextBox CreateEditor(PagedDocumentPresenter surface, global::Avalonia.Controls.ScrollViewer scroll) => new()
    {
        IsPageLayoutEnabled = true,
        Template = new FuncControlTemplate<RichTextBox>((_, scope) =>
        {
            var formatted = new FormattedTextPresenter { IsVisible = false };
            scope.Register("PART_FormattedPresenter", formatted);
            scope.Register("PART_PagedPresenter", surface);
            scope.Register("PART_PageScrollViewer", scroll);
            return new global::Avalonia.Controls.Grid { Children = { formatted, scroll } };
        }),
    };
}
