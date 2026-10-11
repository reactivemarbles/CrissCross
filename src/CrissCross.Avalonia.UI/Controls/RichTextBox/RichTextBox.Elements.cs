// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Net;
using System.Text;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides document element insertion for <see cref="RichTextBox"/>.</summary>
public partial class RichTextBox
{
    /// <summary>The maximum number of inserted table rows.</summary>
    private const int MaximumInsertedTableRows = 20;

    /// <summary>The maximum number of inserted table columns.</summary>
    private const int MaximumInsertedTableColumns = 10;

    /// <summary>HTML list element names.</summary>
    private const string BulletedListTag = "ul";

    /// <summary>The numbered list element name.</summary>
    private const string NumberedListTag = "ol";

    /// <summary>The inline style attribute name.</summary>
    private const string InlineStyleAttribute = "style";

    /// <summary>The paragraph element name.</summary>
    private const string ParagraphTag = "p";

    /// <summary>The list item element name.</summary>
    private const string ListItemTag = "li";

    /// <summary>The table cell element name.</summary>
    private const string TableCellTag = "td";

    /// <summary>The table header cell element name.</summary>
    private const string TableHeaderCellTag = "th";

    /// <summary>Inserts a hyperlink using the selected text or address as its label.</summary>
    /// <param name="address">The absolute HTTP, HTTPS, or mail address.</param>
    public void InsertHyperlink(Uri address) => InsertHyperlink(address, null);

    /// <summary>Inserts a hyperlink, replacing the selected text.</summary>
    /// <param name="address">The absolute HTTP, HTTPS, or mail address.</param>
    /// <param name="text">The label; defaults to the selection or address.</param>
    public void InsertHyperlink(Uri address, string? text)
    {
        ArgumentNullException.ThrowIfNull(address);
        if (!address.IsAbsoluteUri || (address.Scheme != Uri.UriSchemeHttp && address.Scheme != Uri.UriSchemeHttps && address.Scheme != Uri.UriSchemeMailto))
        {
            throw new ArgumentException("An absolute HTTP, HTTPS, or mail address is required.", nameof(address));
        }

        var label = text ?? (HasSelection ? SelectedText : address.AbsoluteUri);
        ReplaceSelectionWithHtml($"<a href=\"{WebUtility.HtmlEncode(address.AbsoluteUri)}\">{WebUtility.HtmlEncode(label)}</a>");
    }

    /// <summary>Inserts an editable table with the specified dimensions.</summary>
    /// <param name="rows">The number of rows, between one and twenty.</param>
    /// <param name="columns">The number of columns, between one and ten.</param>
    public void InsertTable(int rows, int columns)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(rows, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(rows, MaximumInsertedTableRows);
        ArgumentOutOfRangeException.ThrowIfLessThan(columns, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(columns, MaximumInsertedTableColumns);
        var html = new StringBuilder("<table style=\"border-collapse:collapse;width:100%\">");
        for (var row = 0; row < rows; row++)
        {
            _ = html.Append("<tr>");
            for (var column = 0; column < columns; column++)
            {
                _ = html.Append("<td style=\"border:1px solid #808080;padding:8px\">Cell</td>");
            }

            _ = html.Append("</tr>");
        }

        _ = html.Append("</table>");
        ReplaceSelectionWithHtml(html.ToString());
    }

    /// <summary>Inserts an explicit page break at the selection.</summary>
    public void InsertPageBreak() => ReplaceSelectionWithHtml("<br style=\"break-before:page\" />");

    /// <summary>Sets the alignment of paragraphs intersecting the selection.</summary>
    /// <param name="alignment">The paragraph alignment.</param>
    public void SetSelectionParagraphAlignment(TextAlignment alignment)
    {
        var value = alignment switch
        {
            TextAlignment.Left => "left",
            TextAlignment.Center => "center",
            TextAlignment.Right => "right",
            TextAlignment.Justify => "justify",
            _ => throw new ArgumentOutOfRangeException(nameof(alignment)),
        };
        TransformSelectedParagraphs((document, paragraphs) =>
        {
            foreach (var paragraph in paragraphs)
            {
                var inlineStyle = paragraph.GetAttribute(InlineStyleAttribute) ?? string.Empty;
                paragraph.SetAttribute(InlineStyleAttribute, $"{inlineStyle};text-align:{value}");
            }
        });
    }

    /// <summary>Toggles bulleted lists for the selected paragraphs, retaining inline markup.</summary>
    public void ToggleBulletedList() => ToggleSelectionList(BulletedListTag);

    /// <summary>Toggles numbered lists for the selected paragraphs, retaining inline markup.</summary>
    public void ToggleNumberedList() => ToggleSelectionList(NumberedListTag);

    /// <summary>Finds complete paragraphs intersecting a selected source interval.</summary>
    /// <param name="document">The parsed source.</param>
    /// <param name="source">The original markup.</param>
    /// <param name="start">The source selection start.</param>
    /// <param name="end">The source selection end.</param>
    /// <returns>The selected paragraph elements.</returns>
    private static List<IElement> GetSelectedParagraphs(IDocument document, string source, int start, int end)
    {
        var paragraphs = new List<IElement>();
        foreach (var paragraph in document.QuerySelectorAll("p,h1,h2,h3,h4,h5,h6,li,td,th"))
        {
            if (paragraph.SourceReference is not { } reference || paragraph.QuerySelector("p,li") is not null)
            {
                continue;
            }

            var opening = reference.Position.Index;
            var closing = source.IndexOf($"</{paragraph.LocalName}", opening, StringComparison.OrdinalIgnoreCase);
            var paragraphEnd = closing < 0 ? source.Length : source.IndexOf('>', closing) + 1;
            if (paragraph.NextElementSibling?.SourceReference is { } next)
            {
                paragraphEnd = Math.Min(paragraphEnd, next.Position.Index);
            }

            if (opening < end && paragraphEnd > start)
            {
                paragraphs.Add(paragraph);
            }
        }

        return paragraphs;
    }

    /// <summary>Splits a list around one item so unrelated list items retain their structure.</summary>
    /// <param name="document">The owning document.</param>
    /// <param name="item">The selected item.</param>
    /// <param name="list">The containing list.</param>
    /// <param name="markupElementName">The requested list kind.</param>
    private static void TransformListItem(IDocument document, IElement item, IElement list, string markupElementName)
    {
        if (list.ParentElement is not { } container)
        {
            return;
        }

        var before = CopyParagraph(document, list, list.LocalName);
        before.InnerHtml = string.Empty;
        while (list.FirstElementChild is { } sibling && !ReferenceEquals(sibling, item))
        {
            _ = before.AppendChild(sibling);
        }

        if (before.ChildElementCount > 0)
        {
            _ = container.InsertBefore(before, list);
        }

        IElement replacement;
        if (list.LocalName == markupElementName)
        {
            replacement = CopyParagraph(document, item, ParagraphTag);
        }
        else
        {
            replacement = document.CreateElement(markupElementName);
            _ = replacement.AppendChild(CopyParagraph(document, item, ListItemTag));
        }

        _ = container.InsertBefore(replacement, list);
        item.Remove();
        if (list.ChildElementCount != 0)
        {
            return;
        }

        list.Remove();
    }

    /// <summary>Copies a paragraph's attributes and children to a different block type.</summary>
    /// <param name="document">The owning document.</param>
    /// <param name="paragraph">The source paragraph.</param>
    /// <param name="markupElementName">The new block type.</param>
    /// <returns>The copied block.</returns>
    private static IElement CopyParagraph(IDocument document, IElement paragraph, string markupElementName)
    {
        var replacement = document.CreateElement(markupElementName);
        foreach (var attribute in paragraph.Attributes)
        {
            replacement.SetAttribute(attribute.Name, attribute.Value);
        }

        if (paragraph.ParentElement is { LocalName: BulletedListTag or NumberedListTag } parent && parent.GetAttribute(InlineStyleAttribute) is { Length: > 0 } inheritedStyle)
        {
            replacement.SetAttribute(InlineStyleAttribute, $"{inheritedStyle};{replacement.GetAttribute(InlineStyleAttribute)}");
        }

        replacement.InnerHtml = paragraph.InnerHtml;
        return replacement;
    }

    /// <summary>Changes the list structure of one selected paragraph or table cell.</summary>
    /// <param name="document">The owning document.</param>
    /// <param name="paragraph">The selected paragraph.</param>
    /// <param name="markupElementName">The requested list kind.</param>
    private static void ApplyListToParagraph(IDocument document, IElement paragraph, string markupElementName)
    {
        if (paragraph.ParentElement is not { } parent)
        {
            return;
        }

        if (paragraph.LocalName is TableCellTag or TableHeaderCellTag)
        {
            var list = document.CreateElement(markupElementName);
            var item = document.CreateElement(ListItemTag);
            item.InnerHtml = paragraph.InnerHtml;
            paragraph.InnerHtml = string.Empty;
            _ = list.AppendChild(item);
            _ = paragraph.AppendChild(list);
        }
        else if (paragraph.LocalName == ListItemTag && parent.LocalName is BulletedListTag or NumberedListTag)
        {
            TransformListItem(document, paragraph, parent, markupElementName);
        }
        else
        {
            var list = document.CreateElement(markupElementName);
            var item = CopyParagraph(document, paragraph, ListItemTag);
            _ = list.AppendChild(item);
            _ = parent.ReplaceChild(list, paragraph);
        }
    }

    /// <summary>Transforms complete selected paragraphs in the source document.</summary>
    /// <param name="transform">The structural transformation.</param>
    private void TransformSelectedParagraphs(Action<IDocument, List<IElement>> transform)
    {
        if (!CanApplyFormatting)
        {
            return;
        }

        var start = Math.Min(SelectionStart, SelectionEnd);
        var length = Math.Abs(SelectionEnd - SelectionStart);
        var projection = HtmlTextProjection.Create(Document.Text);
        var (sourceStart, sourceLength) = projection.GetSourceRange(start, length);
        var parser = new HtmlParser(new HtmlParserOptions { IsKeepingSourceReferences = true });
        var document = parser.ParseDocument(Document.Text);
        if (document.Body is not { } body)
        {
            return;
        }

        var paragraphs = GetSelectedParagraphs(document, Document.Text, sourceStart, sourceStart + sourceLength);
        if (paragraphs.Count == 0)
        {
            var paragraph = document.CreateElement(ParagraphTag);
            paragraph.InnerHtml = body.InnerHtml;
            body.InnerHtml = string.Empty;
            _ = body.AppendChild(paragraph);
            paragraphs.Add(paragraph);
        }

        var before = CaptureHistory();
        transform(document, paragraphs);
        var markup = document.Head is { ChildElementCount: > 0 } ? document.DocumentElement.OuterHtml : body.InnerHtml;
        SetHtmlCore(markup, resetUndo: false);
        Select(start, Math.Min(length, Document.Length - start));
        CommitHistory(before);
    }

    /// <summary>Changes or removes list structure around selected paragraphs.</summary>
    /// <param name="markupElementName">The requested list kind.</param>
    private void ToggleSelectionList(string markupElementName) => TransformSelectedParagraphs((document, paragraphs) =>
    {
        foreach (var paragraph in paragraphs)
        {
            ApplyListToParagraph(document, paragraph, markupElementName);
        }

        foreach (var list in document.QuerySelectorAll(markupElementName))
        {
            if (list.PreviousElementSibling is { } previous && previous.LocalName == markupElementName && previous.Attributes.Length == 0 && list.Attributes.Length == 0)
            {
                while (list.FirstChild is { } child)
                {
                    _ = previous.AppendChild(child);
                }

                list.Remove();
            }
        }
    });
}
