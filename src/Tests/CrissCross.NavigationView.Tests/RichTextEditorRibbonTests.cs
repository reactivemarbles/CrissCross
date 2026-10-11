// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.Media;
using CrissCross.Avalonia.UI.Controls;
using Editor = CrissCross.Avalonia.UI.Controls.RichTextBox;
using RibbonButton = Avalonia.Controls.Button;
using RibbonPanel = Avalonia.Controls.StackPanel;

namespace CrissCross.NavigationView.Tests;

/// <summary>Exercises composed ribbon targeting and structural insertion history.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class RichTextEditorRibbonTests
{
    /// <summary>The rendered length of the initial selected word.</summary>
    private const int InitialWordLength = 5;

    /// <summary>The table dimension used by structural tests.</summary>
    private const int TestTableDimension = 2;

    /// <summary>The original read-only fixture.</summary>
    private const string BeforeText = "Before";

    /// <summary>The hyperlink address shared by insertion tests.</summary>
    private const string ExampleAddress = "https://example.com";

    /// <summary>The numbered list opening markup.</summary>
    private const string NumberedListMarkup = "<ol>";

    /// <summary>Paragraph commands preserve table cells when adding lists inside them.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ListsInsideTablePreserveCellStructure()
    {
        var editor = new Editor();
        editor.SetHtml("<table><tr><td style='color:blue'><strong>Cell</strong></td></tr></table>");
        editor.SelectAll();
        editor.ToggleBulletedList();
        await Assert.That(editor.Html).Contains("<table>");
        await Assert.That(editor.Html).Contains("<td");
        await Assert.That(editor.Html).Contains("<ul><li><strong>Cell</strong></li></ul>");
    }

    /// <summary>Paragraph operations preserve rich inline markup and neighboring paragraphs.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ParagraphAlignmentAndListsPreserveRichSource()
    {
        var editor = new Editor();
        editor.SetHtml("<p><strong>First</strong></p><p><em>Second</em></p><p><a href='https://example.com'>Third</a></p>");
        editor.SelectAll();
        editor.SetSelectionParagraphAlignment(TextAlignment.Center);
        await Assert.That(editor.Html).Contains("text-align:center");
        await Assert.That(editor.Html).Contains("<strong>First</strong>");
        editor.SelectAll();
        editor.ToggleBulletedList();
        await Assert.That(editor.Html).Contains("<ul>");
        await Assert.That(editor.Html).Contains("<em>Second</em>");
        editor.SelectAll();
        editor.ToggleNumberedList();
        await Assert.That(editor.Html).Contains(NumberedListMarkup);
        await Assert.That(editor.Html).DoesNotContain("<ul>");
        editor.SelectAll();
        editor.ToggleNumberedList();
        await Assert.That(editor.Html).DoesNotContain("<ol>");
        await Assert.That(editor.Html).Contains("href=");
        editor.Undo();
        await Assert.That(editor.Html).Contains(NumberedListMarkup);
    }

    /// <summary>Paragraph formatting only changes paragraphs touched by the selection.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ParagraphFormattingKeepsUnselectedNeighbors()
    {
        var editor = new Editor();
        editor.SetHtml("<p>First</p><p>Second</p>");
        editor.Select(0, "First".Length);
        editor.SetSelectionParagraphAlignment(TextAlignment.Right);
        await Assert.That(editor.Html).Contains("text-align:right");
        await Assert.That(editor.Html).Contains("<p>Second</p>");
        editor.ToggleBulletedList();
        await Assert.That(editor.Html).Contains("<p>Second</p>");
        editor.IsReadOnly = true;
        var before = editor.Html;
        editor.ToggleNumberedList();
        await Assert.That(editor.Html).IsEqualTo(before);
    }

    /// <summary>Hyperlink insertion escapes labels and is reversible.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task HyperlinkInsertionPreservesLabelAndUndo()
    {
        var editor = new Editor();
        editor.SetPlainText("Hello world");
        editor.Select(0, InitialWordLength);
        editor.InsertHyperlink(new("https://example.com/a?b=1&c=2"), "A < B");
        await Assert.That(editor.Html).Contains("&amp;c=2");
        await Assert.That(editor.Html).Contains("A &lt; B");
        await Assert.That(editor.PlainText).Contains("A < B");
        editor.Undo();
        await Assert.That(editor.PlainText).IsEqualTo("Hello world");
    }

    /// <summary>Document elements obey read-only state and preserve history.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task StructuralInsertionsRespectReadOnlyAndUndo()
    {
        var editor = new Editor();
        editor.SetPlainText(BeforeText);
        editor.Select(editor.Document.Length, 0);
        editor.InsertTable(TestTableDimension, TestTableDimension);
        await Assert.That(editor.Html).Contains("<table");
        editor.Undo();
        await Assert.That(editor.PlainText).IsEqualTo(BeforeText);
        editor.IsReadOnly = true;
        editor.InsertTable(1, 1);
        editor.InsertPageBreak();
        editor.InsertHyperlink(new(ExampleAddress));
        await Assert.That(editor.PlainText).IsEqualTo(BeforeText);
    }

    /// <summary>Changing the target routes formatting commands exclusively to the new editor.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task RibbonRetargetsCommandsAndPreservesSelection()
    {
        var first = new Editor();
        var second = new Editor();
        first.SetPlainText("First");
        second.SetPlainText("Second");
        first.SelectAll();
        second.SelectAll();
        var ribbon = new RichTextEditorRibbon { Editor = first };
        ribbon.Editor = second;
        var group = (WrapPanel)((RibbonPanel)ribbon.Content!).Children[0];
        RibbonButton? candidate = null;
        foreach (var child in group.Children)
        {
            if (child is RibbonButton { Content: "Bold" } button)
            {
                candidate = button;
            }
        }

        var bold = await Assert.That(candidate).IsTypeOf<RibbonButton>() ?? throw new InvalidOperationException("The ribbon has no bold button.");
        await Assert.That(bold.Focusable).IsFalse();
        bold.Command!.Execute(null);
        await Assert.That(second.Html).Contains("<strong>");
        await Assert.That(first.Html).DoesNotContain("<strong>");
        await Assert.That(second.HasSelection).IsTrue();
        ribbon.Editor = null;
        group = (WrapPanel)((RibbonPanel)ribbon.Content!).Children[0];
        foreach (var child in group.Children)
        {
            if (child is RibbonButton button)
            {
                await Assert.That(button.IsEnabled).IsFalse();
            }
        }
    }

    /// <summary>The two-editor clipboard workflow retains inserted document markup.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ClipboardTransfersInsertedLinkAndTable()
    {
        var clipboard = new RichTextMemoryClipboardAdapter();
        var source = new Editor { ClipboardAdapter = clipboard };
        var destination = new Editor { ClipboardAdapter = clipboard };
        source.InsertHyperlink(new(ExampleAddress), "Visit");
        source.Select(source.Document.Length, 0);
        source.InsertTable(1, TestTableDimension);
        source.SelectAll();
        source.Copy();
        destination.Paste();
        await Assert.That(destination.Html).Contains("href=");
        await Assert.That(destination.Html).Contains("<table");
    }
}
