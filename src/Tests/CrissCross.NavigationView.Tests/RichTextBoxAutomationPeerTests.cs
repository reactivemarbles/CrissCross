// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls.Templates;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Exercises supported Avalonia providers and document accessibility contracts.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextBoxAutomationPeerTests
{
    /// <summary>The before text value used by these tests.</summary>
    private const string BeforeText = "Before";

    /// <summary>The after text value used by these tests.</summary>
    private const string AfterText = "After";

    /// <summary>The direct document edit text value used by these tests.</summary>
    private const string DirectDocumentEditText = "Direct document edit";

    /// <summary>The expected read only transition count value used by these tests.</summary>
    private const int ExpectedReadOnlyTransitionCount = 6;

    /// <summary>The reversed selection offset value used by these tests.</summary>
    private const int ReversedSelectionOffset = 5;

    /// <summary>The selected text length value used by these tests.</summary>
    private const int SelectedTextLength = 2;

    /// <summary>The invalid range start value used by these tests.</summary>
    private const int InvalidRangeStart = 2;

    /// <summary>The expected table cell count value used by these tests.</summary>
    private const int ExpectedTableCellCount = 2;

    /// <summary>The expected page count value used by these tests.</summary>
    private const int ExpectedPageCount = 3;

    /// <summary>The minimum rectangle count value used by these tests.</summary>
    private const int MinimumRectangleCount = 3;

    /// <summary>The readable text line count value used by these tests.</summary>
    private const int ReadableTextLineCount = 20;

    /// <summary>The caret vertical center factor value used by these tests.</summary>
    private const double CaretVerticalCenterFactor = 0.5;

    /// <summary>Exposes rendered content and an actual Avalonia value provider.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task CreatePeer_RichMarkup_ExposesRenderedValue()
    {
        var editor = new RichTextBox { Text = "<p>Hello <b>world</b></p>", IsDocumentEnabled = true, IsPageLayoutEnabled = true };
        var peer = await Assert.That(ControlAutomationPeer.CreatePeerForElement(editor)).IsTypeOf<RichTextBoxAutomationPeer>()
            ?? throw new InvalidOperationException("The rich editor peer was not created.");
        await Assert.That(peer.GetProvider<IValueProvider>()).IsEqualTo(peer);
        await Assert.That(peer.GetAutomationControlType()).IsEqualTo(AutomationControlType.Edit);
        await Assert.That(peer.Value).IsEqualTo(editor.Document.PlainText);
        await Assert.That(peer.Value.Contains("<b>", StringComparison.Ordinal)).IsFalse();
        await Assert.That(peer.DocumentRange.GetText()).IsEqualTo(editor.Document.PlainText);
    }

    /// <summary>Writes literal text and routes the change through editor history.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task SetValue_LiteralMarkup_EncodesAndSupportsUndo()
    {
        var editor = new RichTextBox { Text = BeforeText, IsPageLayoutEnabled = true };
        var peer = new RichTextBoxAutomationPeer(editor);
        peer.SetValue("<b>literal</b>");
        await Assert.That(peer.Value).IsEqualTo("<b>literal</b>");
        editor.Undo();
        await Assert.That(peer.Value).IsEqualTo(BeforeText);
    }

    /// <summary>Rejects writes for every effective read-only mode and resumes when editable.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task SetValue_ReadOnlyLifecycle_RejectsWritesAndAnnouncesTransitions()
    {
        var editor = new RichTextBox { Text = BeforeText, IsPageLayoutEnabled = true };
        var peer = new RichTextBoxAutomationPeer(editor);
        var transitions = new List<bool>();
        peer.PropertyChanged += (_, args) =>
        {
            if (args.Property != ValuePatternIdentifiers.IsReadOnlyProperty || args.NewValue is not bool state)
            {
                return;
            }

            transitions.Add(state);
        };
        editor.IsReadOnly = true;
        await Assert.That(() => peer.SetValue(AfterText)).Throws<InvalidOperationException>();
        peer.GetRange(0, 1).Select();
        await Assert.That(peer.SelectionRange.GetText()).IsEqualTo("B");
        editor.IsReadOnly = false;
        editor.EditMode = RichTextEditMode.Display;
        await Assert.That(() => peer.SetValue(AfterText)).Throws<InvalidOperationException>();
        editor.EditMode = RichTextEditMode.EditOnFocus;
        editor.IsTextSelectionEnabled = true;
        await Assert.That(() => peer.SetValue(AfterText)).Throws<InvalidOperationException>();
        editor.IsTextSelectionEnabled = false;
        peer.SetValue(AfterText);
        await Assert.That(peer.Value).IsEqualTo(AfterText);
        await Assert.That(transitions.Count).IsEqualTo(ExpectedReadOnlyTransitionCount);
    }

    /// <summary>Rejects disabled automation writes without changing content.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task SetValue_Disabled_RejectsMutation()
    {
        var editor = new RichTextBox { Text = BeforeText, IsEnabled = false };
        var peer = new RichTextBoxAutomationPeer(editor);
        await Assert.That(() => peer.SetValue(AfterText)).Throws<ElementNotEnabledException>();
        await Assert.That(() => peer.DocumentRange.Select()).Throws<ElementNotEnabledException>();
        await Assert.That(peer.Value).IsEqualTo(BeforeText);
    }

    /// <summary>Normalizes reversed selections and clamps retained ranges after document replacement.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task GetSelection_ReversedRange_ReturnsOrderedTextAndChangeEvent()
    {
        var editor = new RichTextBox { Text = "abcdef", IsPageLayoutEnabled = true };
        var peer = new RichTextBoxAutomationPeer(editor);
        var changes = 0;
        peer.TextSelectionChanged += (_, _) => changes++;
        editor.SelectionStart = ReversedSelectionOffset;
        editor.SelectionEnd = 1;
        editor.CaretIndex = 1;
        var range = peer.SelectionRange;
        await Assert.That(range.GetText()).IsEqualTo("bcde");
        range.Select();
        await Assert.That(changes > 0).IsTrue();
        await Assert.That(range.GetText(SelectedTextLength)).IsEqualTo("bc");
        editor.Text = "x";
        await Assert.That(range.Start).IsEqualTo(1);
        await Assert.That(range.End).IsEqualTo(1);
        await Assert.That(range.GetText()).IsEqualTo(string.Empty);
        await Assert.That(() => peer.GetRange(InvalidRangeStart, 0)).Throws<ArgumentOutOfRangeException>();
    }

    /// <summary>Exposes hyperlink invocation, image roles, and table-cell reading order.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task GetChildren_EmbeddedObjects_ExposesSemanticsAndHyperlinkInvoke()
    {
        var editor = new RichTextBox
        {
            IsDocumentEnabled = true,
            IsPageLayoutEnabled = true,
            Text = "<a href=\"https://example.com\" target=\"_blank\">Visit</a><img src=\"missing.png\" alt=\"Company emblem\" /><table><tr><td>One</td><td>Two</td></tr></table>",
        };
        var peer = new RichTextBoxAutomationPeer(editor);
        var children = peer.GetChildren();
        var hyperlink = GetSingleChild(children, AutomationControlType.Hyperlink);
        string? invoked = null;
        editor.HyperlinkActivated += (_, args) => invoked = args.NavigationUri;
        var invoke = await Assert.That(hyperlink.GetProvider<IInvokeProvider>()).IsNotNull() ?? throw new InvalidOperationException("The hyperlink invoke provider was not created.");
        invoke.Invoke();
        await Assert.That(invoked).IsEqualTo("https://example.com");
        await Assert.That(hyperlink.GetParent()).IsEqualTo(peer);
        var image = GetSingleChild(children, AutomationControlType.Image);
        await Assert.That(image.GetName()).IsEqualTo("Company emblem");
        await Assert.That(image.GetProvider<IInvokeProvider>()).IsNull();
        var table = GetSingleChild(children, AutomationControlType.Table);
        await Assert.That(table.GetChildren().Count).IsEqualTo(ExpectedTableCellCount);
        await Assert.That(table.GetChildren()[0].GetName().Contains("One", StringComparison.Ordinal)).IsTrue();
        await Assert.That(table.GetChildren()[1].GetParent()).IsEqualTo(table);
    }

    /// <summary>Announces rendered value and refreshed semantic children after replacement.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task TextReplacement_RaisesValueAndChildrenEvents()
    {
        var editor = new RichTextBox { Text = BeforeText };
        var peer = new RichTextBoxAutomationPeer(editor);
        var valueChanges = 0;
        var childChanges = 0;
        peer.PropertyChanged += (_, args) => valueChanges += args.Property == ValuePatternIdentifiers.ValueProperty ? 1 : 0;
        peer.ChildrenChanged += (_, _) => childChanges++;
        _ = peer.GetChildren();
        editor.Text = AfterText;
        await Assert.That(valueChanges > 0).IsTrue();
        await Assert.That(childChanges > 0).IsTrue();
        await Assert.That(peer.GetChildren()[0].GetName()).IsEqualTo(AfterText);
    }

    /// <summary>Observes model changes from markup, mutations, formatting, undo, and direct document APIs.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task DocumentChanges_AllEditingPaths_AnnounceRenderedValuesAndSemanticChanges()
    {
        var editor = new RichTextBox { IsPageLayoutEnabled = true };
        editor.SetPlainText(BeforeText);
        var peer = new RichTextBoxAutomationPeer(editor);
        var values = new List<string>();
        var childChanges = 0;
        peer.PropertyChanged += (_, args) =>
        {
            if (args.Property != ValuePatternIdentifiers.ValueProperty || args.NewValue is not string value)
            {
                return;
            }

            values.Add(value);
        };
        peer.ChildrenChanged += (_, _) => childChanges++;
        editor.SetHtml("<b>After</b>");
        await Assert.That(values[^1]).IsEqualTo(AfterText);
        editor.Select(0, editor.Document.Length);
        editor.ReplaceSelection("Replacement");
        await Assert.That(values[^1]).IsEqualTo("Replacement");
        editor.Select(0, editor.Document.Length);
        var beforeFormatting = childChanges;
        var valuesBeforeFormatting = values.Count;
        editor.ToggleItalic();
        await Assert.That(childChanges > beforeFormatting).IsTrue();
        await Assert.That(values.Count).IsEqualTo(valuesBeforeFormatting);
        editor.Undo();
        await Assert.That(childChanges > beforeFormatting + 1).IsTrue();
        editor.Document.SetText(DirectDocumentEditText);
        await Assert.That(values[^1]).IsEqualTo(DirectDocumentEditText);
        await Assert.That(peer.DocumentRange.GetText()).IsEqualTo(DirectDocumentEditText);
    }

    /// <summary>Translates ranges across paper margins and inter-sheet spacing.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task GetDocumentRangeRectangles_MultipleSheets_UsesPageCoordinates()
    {
        const double pageWidth = 200;
        const double pageHeight = 44;
        const double pageMargin = 10;
        const double pageSpacing = 25;
        var document = new FlowDocument();
        document.SetText("First\nSecond\nThird");
        var presenter = new PagedDocumentPresenter { Document = document, PageSize = new(pageWidth, pageHeight), PagePadding = new(pageMargin), PageSpacing = pageSpacing };
        presenter.Measure(new(pageWidth, double.PositiveInfinity));
        presenter.Arrange(new(presenter.DesiredSize));
        var rectangles = presenter.GetDocumentRangeRectangles(0, document.Length);
        await Assert.That(presenter.PageCount).IsEqualTo(ExpectedPageCount);
        await Assert.That(rectangles.Count >= MinimumRectangleCount).IsTrue();
        var spansLaterPage = false;
        var rectanglesStayWithinPageWidth = true;
        foreach (var rectangle in rectangles)
        {
            spansLaterPage |= rectangle.Y >= pageHeight + pageSpacing + pageMargin;
            rectanglesStayWithinPageWidth &= rectangle.X >= pageMargin && rectangle.Right <= pageWidth - pageMargin;
        }

        await Assert.That(spansLaterPage).IsTrue();
        await Assert.That(rectanglesStayWithinPageWidth).IsTrue();
        await Assert.That(() => presenter.GetDocumentRangeRectangles(-1, 0)).Throws<ArgumentOutOfRangeException>();
    }

    /// <summary>Uses the scrolled visual transform for range rectangles and reverse point mapping.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task RangeGeometry_ScrolledWindow_MapsTopLevelCoordinates()
    {
        const double width = 800;
        const double height = 200;
        const double scrollDistance = 100;
        var surface = new PagedDocumentPresenter();
        var scroll = new global::Avalonia.Controls.ScrollViewer { Content = surface };
        var editor = new RichTextBox
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
        editor.SetPlainText(string.Join(Environment.NewLine, Enumerable.Repeat("Readable text", ReadableTextLineCount)));
        var window = new global::Avalonia.Controls.Window { Width = width, Height = height, Content = editor, ShowInTaskbar = false };
        try
        {
            window.Show();
            editor.ApplyTemplate();
            editor.UpdateLayout();
            scroll.Offset = new(0, scrollDistance);
            editor.UpdateLayout();
            var peer = new RichTextBoxAutomationPeer(editor);
            var range = peer.GetRange(0, 1);
            var rectangle = range.GetBoundingRectangles()[0];
            var transform = surface.TransformToVisual(window) ?? throw new InvalidOperationException("The page transform is absent.");
            var expected = surface.GetDocumentRangeRectangles(0, 1)[0].TransformToAABB(transform);
            await Assert.That(rectangle).IsEqualTo(expected);
            var caret = surface.GetDocumentCaretRect(0).TransformToAABB(transform);
            await Assert.That(peer.RangeFromPoint(new(caret.X, caret.Y + (caret.Height * CaretVerticalCenterFactor))).Start).IsEqualTo(0);
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Returns the only child with the requested automation control type.</summary>
    /// <param name="children">The children exposed by an automation peer.</param>
    /// <param name="controlType">The control type to locate.</param>
    /// <returns>The matching child.</returns>
    private static AutomationPeer GetSingleChild(IEnumerable<AutomationPeer> children, AutomationControlType controlType)
    {
        AutomationPeer? result = null;
        foreach (var child in children)
        {
            if (child.GetAutomationControlType() != controlType)
            {
                continue;
            }

            if (result is not null)
            {
                throw new InvalidOperationException($"More than one {controlType} child was exposed.");
            }

            result = child;
        }

        return result ?? throw new InvalidOperationException($"No {controlType} child was exposed.");
    }
}
