// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Input.TextInput;
using CrissCross.Avalonia.UI.Controls;
using AvaloniaWindow = Avalonia.Controls.Window;
using Grid = Avalonia.Controls.Grid;
using ScrollViewer = Avalonia.Controls.ScrollViewer;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies the routed paper input method contract.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextBoxInputMethodTests
{
    /// <summary>The nominal paper width used by the fixture.</summary>
    private const double PaperWidth = 794;

    /// <summary>The viewport width used by the fixture.</summary>
    private const double ViewportWidth = 800;

    /// <summary>The viewport height used by the fixture.</summary>
    private const double ViewportHeight = 200;

    /// <summary>The vertical scroll offset and narrow composition sheet width.</summary>
    private const double ScrollDistance = 100;

    /// <summary>The narrow composition sheet height.</summary>
    private const double CompositionSheetHeight = 45;

    /// <summary>The composition sheet padding.</summary>
    private const double CompositionPadding = 5;

    /// <summary>The repeated text count used to force wrapping and scrolling.</summary>
    private const int RepeatedTextCount = 20;

    /// <summary>The pending character font size.</summary>
    private const double CompositionFontSize = 30;

    /// <summary>The tolerance used when comparing the measured composition font size.</summary>
    private const double FontSizeComparisonTolerance = 1E-10D;

    /// <summary>The rich document fixture used by composition tests.</summary>
    private const string RichTextHtml = "<b>abc</b>";

    /// <summary>The insertion offset after both preedit characters.</summary>
    private const int CompositionEndOffset = 2;

    /// <summary>The UTF-16 offset after the supplementary-plane character in the fixture text.</summary>
    private const int EmojiSelectionStart = 3;

    /// <summary>Composition uses pending insertion typography without consuming it before commit.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Preedit_PendingTypographyPersistsThroughCommit()
    {
        var surface = new PagedDocumentPresenter();
        var editor = new RichTextBox
        {
            IsPageLayoutEnabled = true,
            Template = new FuncControlTemplate<RichTextBox>((_, scope) =>
            {
                var formatted = new FormattedTextPresenter { IsVisible = false };
                scope.Register("PART_FormattedPresenter", formatted);
                scope.Register("PART_PagedPresenter", surface);
                return new global::Avalonia.Controls.Grid { Children = { formatted, surface } };
            }),
        };
        editor.SetPlainText("abc");
        editor.ApplyTemplate();
        editor.Select(1, 0);
        editor.SetSelectionFontSize(CompositionFontSize);
        var client = Request(editor).Client!;
        client.SetPreeditText("日", 1);
        Arrange(surface);
        await Assert.That(HasCompositionSegment(surface.Document!.Segments)).IsTrue();
        await Assert.That(editor.PlainText).IsEqualTo("abc");
        editor.RaiseEvent(new TextInputEventArgs { RoutedEvent = InputElement.TextInputEvent, Text = "日" });
        await Assert.That(HasCompositionSegment(editor.Document.Segments)).IsTrue();
        editor.Undo();
        await Assert.That(editor.PlainText).IsEqualTo("abc");
    }

    /// <summary>The platform cursor rectangle follows the scrolled paper surface transform.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task CursorRectangle_UsesEditorCoordinatesAfterScrolling()
    {
        var surface = new PagedDocumentPresenter();
        var scroll = new ScrollViewer { Content = surface };
        var editor = new RichTextBox
        {
            IsPageLayoutEnabled = true,
            Template = new FuncControlTemplate<RichTextBox>((_, scope) =>
            {
                var formatted = new FormattedTextPresenter { IsVisible = false };
                scope.Register("PART_FormattedPresenter", formatted);
                scope.Register("PART_PagedPresenter", surface);
                scope.Register("PART_PageScrollViewer", scroll);
                return new Grid { Children = { formatted, scroll } };
            }),
        };
        editor.SetPlainText(string.Join(Environment.NewLine, Enumerable.Repeat("日本語", RepeatedTextCount)));
        var window = new AvaloniaWindow { Width = ViewportWidth, Height = ViewportHeight, Content = editor, ShowInTaskbar = false };
        try
        {
            window.Show();
            editor.ApplyTemplate();
            editor.Select(editor.Document.Length, 0);
            editor.UpdateLayout();
            scroll.Offset = new(0, ScrollDistance);
            editor.UpdateLayout();
            var client = Request(editor).Client!;
            var transform = surface.TransformToVisual(editor) ?? throw new InvalidOperationException("The paper surface is detached from its editor.");
            var expected = surface.GetInputMethodCursorRectangle().TransformToAABB(transform);
            await Assert.That(client.CursorRectangle).IsEqualTo(expected);
            await Assert.That(client.TextViewVisual).IsSameReferenceAs(editor);
            await Assert.That(client.CursorRectangle.Y).IsLessThan(surface.GetInputMethodCursorRectangle().Y);
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Preedit advances the measured input cursor while leaving paged content unchanged.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Preedit_CursorUsesMeasuredCompositionAndCancellationRestoresCaret()
    {
        var document = new FlowDocument();
        document.SetText("abc");
        var surface = new PagedDocumentPresenter { Document = document, CaretIndex = 1 };
        Arrange(surface);
        var original = surface.GetInputMethodCursorRectangle();
        surface.SetPreedit("日本", 0, 1);
        Arrange(surface);
        await Assert.That(surface.GetInputMethodCursorRectangle()).IsEqualTo(original);
        surface.SetPreedit("日本", 1, 1);
        Arrange(surface);
        var first = surface.GetInputMethodCursorRectangle();
        surface.SetPreedit("日本", CompositionEndOffset, 1);
        Arrange(surface);
        var second = surface.GetInputMethodCursorRectangle();
        await Assert.That(first.X).IsGreaterThan(original.X);
        await Assert.That(second.X).IsGreaterThan(first.X);
        await Assert.That(second.Y).IsEqualTo(original.Y);
        await Assert.That(document.PlainText).IsEqualTo("abc");
        surface.SetPreedit(null, null, 0);
        Arrange(surface);
        await Assert.That(surface.GetInputMethodCursorRectangle()).IsEqualTo(original);
    }

    /// <summary>Composition replaces selected content in a repaginated preview and inherits rich markup.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Preedit_WrapsAcrossSheetsAndRestoresSelectedRichDocument()
    {
        var document = new FlowDocument();
        document.SetText(RichTextHtml);
        var surface = new PagedDocumentPresenter { Document = document, CaretIndex = 1, PageSize = new(ScrollDistance, CompositionSheetHeight), PagePadding = new(CompositionPadding) };
        Arrange(surface);
        var initialPages = surface.PageCount;
        var composition = string.Concat(Enumerable.Repeat("日本語", RepeatedTextCount));
        surface.SetPreedit(composition, composition.Length, 1, 1);
        Arrange(surface);
        await Assert.That(surface.Document!.PlainText).IsEqualTo($"a{composition}c");
        await Assert.That(HasBoldSegment(surface.Document.Segments)).IsTrue();
        await Assert.That(surface.PageCount).IsGreaterThan(initialPages);
        await Assert.That(surface.GetInputMethodCursorRectangle().Y).IsGreaterThan(surface.PageSize.Height);
        await Assert.That(document.Text).IsEqualTo(RichTextHtml);
        surface.SetPreedit(null, null, 0);
        Arrange(surface);
        await Assert.That(surface.Document).IsSameReferenceAs(document);
        await Assert.That(surface.PageCount).IsEqualTo(initialPages);
    }

    /// <summary>Preedit updates remain transient and the committed replacement is undoable once.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Preedit_CommitReplacesSelectionInOneUndoStep()
    {
        var editor = new RichTextBox { IsPageLayoutEnabled = true };
        editor.SetHtml(RichTextHtml);
        editor.Select(1, 1);
        var client = Request(editor).Client!;
        client.SetPreeditText("に", 1);
        client.SetPreeditText("日本", CompositionEndOffset);
        await Assert.That(editor.PlainText).IsEqualTo("abc");
        await Assert.That(editor.CanUndo).IsFalse();
        editor.RaiseEvent(new TextInputEventArgs { RoutedEvent = InputElement.TextInputEvent, Text = "日本" });
        await Assert.That(editor.PlainText).IsEqualTo("a日本c");
        editor.Undo();
        await Assert.That(editor.Html).IsEqualTo(RichTextHtml);
        await Assert.That(editor.CanUndo).IsFalse();
        editor.Redo();
        await Assert.That(editor.PlainText).IsEqualTo("a日本c");
    }

    /// <summary>The platform selection uses UTF-16 document offsets and preserves its direction.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task SurroundingText_SelectionUsesDocumentOffsets()
    {
        var editor = new RichTextBox { IsPageLayoutEnabled = true };
        editor.SetPlainText("a😀b");
        var client = Request(editor).Client!;
        client.Selection = new(EmojiSelectionStart, 1);
        await Assert.That(client.SurroundingText).IsEqualTo("a😀b");
        await Assert.That(client.Selection.Start).IsEqualTo(EmojiSelectionStart);
        await Assert.That(client.Selection.End).IsEqualTo(1);
        await Assert.That(editor.CaretIndex).IsEqualTo(1);
        await Assert.That(editor.SelectedText).IsEqualTo("😀");
        client.Selection = new(-1, int.MaxValue);
        await Assert.That(editor.SelectedText).IsEqualTo("a😀b");
    }

    /// <summary>Read-only transitions reset composition and withdraw the paper client.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ReadOnly_CancelsCompositionAndDoesNotSupplyClient()
    {
        var editor = new RichTextBox { IsPageLayoutEnabled = true };
        editor.SetPlainText("abc");
        var client = Request(editor).Client!;
        var resets = 0;
        client.ResetRequested += (_, _) => resets++;
        client.SetPreeditText("日本", 1);
        editor.IsReadOnly = true;
        await Assert.That(resets).IsGreaterThan(0);
        await Assert.That(Request(editor).Client).IsNull();
        editor.RaiseEvent(new TextInputEventArgs { RoutedEvent = InputElement.TextInputEvent, Text = "日本" });
        await Assert.That(editor.PlainText).IsEqualTo("abc");
        await Assert.That(editor.CanUndo).IsFalse();
    }

    /// <summary>Selection changes and Escape request cancellation without changing document text.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Cancellation_SelectionAndEscapeResetTransientComposition()
    {
        var editor = new RichTextBox { IsPageLayoutEnabled = true };
        editor.SetPlainText("abc");
        var client = Request(editor).Client!;
        var resets = 0;
        client.ResetRequested += (_, _) => resets++;
        client.SetPreeditText("に", 1);
        editor.Select(1, 0);
        await Assert.That(resets).IsGreaterThan(0);
        resets = 0;
        client.SetPreeditText("日本", 1);
        editor.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape });
        await Assert.That(resets).IsGreaterThan(0);
        await Assert.That(editor.PlainText).IsEqualTo("abc");
    }

    /// <summary>Raises the same client request used by Avalonia's platform input manager.</summary>
    /// <param name="editor">The paper editor.</param>
    /// <returns>The completed request.</returns>
    private static TextInputMethodClientRequestedEventArgs Request(RichTextBox editor)
    {
        var request = new TextInputMethodClientRequestedEventArgs { RoutedEvent = InputElement.TextInputMethodClientRequestedEvent };
        editor.RaiseEvent(request);
        return request;
    }

    /// <summary>Measures and arranges the transient paper document.</summary>
    /// <param name="surface">The paper surface.</param>
    private static void Arrange(PagedDocumentPresenter surface)
    {
        surface.Measure(new(PaperWidth, double.PositiveInfinity));
        surface.Arrange(new(0, 0, PaperWidth, surface.DesiredSize.Height));
    }

    /// <summary>Checks whether a document contains the pending character at the selected font size.</summary>
    /// <param name="segments">The document segments.</param>
    /// <returns><see langword="true"/> when the pending character has the expected size.</returns>
    private static bool HasCompositionSegment(IReadOnlyList<TextSegment> segments)
    {
        foreach (var segment in segments)
        {
            if (segment.FontSize.HasValue
                && Math.Abs(segment.FontSize.Value - CompositionFontSize) <= FontSizeComparisonTolerance
                && segment.Text.Contains("日", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Checks whether the document contains a bold text segment.</summary>
    /// <param name="segments">The document segments.</param>
    /// <returns><see langword="true"/> when a bold segment exists.</returns>
    private static bool HasBoldSegment(IReadOnlyList<TextSegment> segments)
    {
        foreach (var segment in segments)
        {
            if (segment.IsBold)
            {
                return true;
            }
        }

        return false;
    }
}
