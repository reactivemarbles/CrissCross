// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AngleSharp.Dom;
using AngleSharp.Html.Parser;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Edits named document structures while retaining HTML and undo history.</summary>
public partial class RichTextBox
{
    /// <summary>Unwraps an element or removes it.</summary>
    /// <param name="element">The annotation.</param>
    /// <param name="removeContent">Whether to delete its contents.</param>
    private static void ResolveElement(IElement element, bool removeContent)
    {
        if (!removeContent && element.Parent is { } parent)
        {
            while (element.FirstChild is { } child)
            {
                _ = parent.InsertBefore(child, element);
            }
        }

        element.Remove();
    }

    /// <summary>Finds a structure without treating its identifier as CSS.</summary>
    /// <param name="document">The source document.</param>
    /// <param name="attribute">The identifier attribute.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>The matching element, or null.</returns>
    private static IElement? FindStructure(IDocument document, string attribute, string id)
    {
        foreach (var element in document.QuerySelectorAll($"[{attribute}]"))
        {
            if (element.GetAttribute(attribute) == id)
            {
                return element;
            }
        }

        return null;
    }

    /// <summary>Wraps the current selection without flattening inline formatting.</summary>
    /// <param name="opening">The opening markup.</param>
    /// <param name="closing">The closing markup.</param>
    private void WrapStructureSelection(string opening, string closing)
    {
        if (!CanApplyFormatting)
        {
            return;
        }

        var start = Math.Min(SelectionStart, SelectionEnd);
        var length = Math.Abs(SelectionEnd - SelectionStart);
        var before = CaptureHistory();
        SetHtmlCore(HtmlFormattingHelper.WrapMarkup(Document.Text, start, length, opening, closing), resetUndo: false);
        Select(start, length);
        CommitHistory(before);
    }

    /// <summary>Parses the source while retaining element positions.</summary>
    /// <returns>The parsed document.</returns>
    private IDocument ParseStructures() => new HtmlParser(new HtmlParserOptions { IsKeepingSourceReferences = true }).ParseDocument(Document.Text);

    /// <summary>Rejects duplicate structure identifiers.</summary>
    /// <param name="attribute">The identifier attribute.</param>
    /// <param name="id">The requested identifier.</param>
    private void EnsureUniqueStructure(string attribute, string id)
    {
        if (FindStructure(ParseStructures(), attribute, id) is null)
        {
            return;
        }

        throw new ArgumentException("The document already contains this identifier.", nameof(id));
    }

    /// <summary>Resolves all insertion and deletion elements of one revision.</summary>
    /// <param name="id">The revision identifier.</param>
    /// <param name="accept">Whether to accept the revision.</param>
    private void ResolveRevision(string id, bool accept)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        MutateStructures(document =>
        {
            foreach (var element in document.QuerySelectorAll("[data-revision-id]"))
            {
                if (element.GetAttribute("data-revision-id") == id)
                {
                    ResolveElement(element, removeContent: (element.LocalName == "del") == accept);
                }
            }
        });
    }

    /// <summary>Removes an annotation wrapper or its content.</summary>
    /// <param name="attribute">The identifying attribute.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="removeContent">Whether to remove the wrapped text.</param>
    private void ResolveStructure(string attribute, string id, bool removeContent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        MutateStructures(document =>
        {
            foreach (var element in document.QuerySelectorAll($"[{attribute}]"))
            {
                if (element.GetAttribute(attribute) == id)
                {
                    ResolveElement(element, removeContent);
                }
            }
        });
    }

    /// <summary>Commits a source structure mutation as a single undo operation.</summary>
    /// <param name="mutation">The mutation.</param>
    private void MutateStructures(Action<IDocument> mutation)
    {
        if (IsReadOnlyInternal)
        {
            return;
        }

        var document = ParseStructures();
        var before = CaptureHistory();
        mutation(document);
        var start = Math.Min(SelectionStart, SelectionEnd);
        SetHtmlCore(document.Head is { ChildElementCount: > 0 } ? document.DocumentElement.OuterHtml : document.Body?.InnerHtml ?? string.Empty, resetUndo: false);
        Select(Math.Min(start, Document.Length), 0);
        CommitHistory(before);
    }
}
