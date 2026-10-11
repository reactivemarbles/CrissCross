// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides measured geometry for native text accessibility.</summary>
public partial class RichTextBox
{
    /// <summary>Gets whether the measured document-start caret intersects the visible viewport.</summary>
    /// <returns>Whether an empty document has a visible insertion position.</returns>
    internal bool IsAutomationCaretVisible()
    {
        var root = global::Avalonia.Controls.TopLevel.GetTopLevel(this);
        var presenter = IsPageLayoutEnabled ? (Control?)_pagedPresenter : _formattedPresenter;
        if (!IsVisible || root is null || presenter is null || this.TransformToVisual(root) is not { } editorTransform
            || presenter.TransformToVisual(root) is not { } presenterTransform)
        {
            return false;
        }

        var clip = new Rect(Bounds.Size).TransformToAABB(editorTransform).Intersect(new(root.Bounds.Size));
        clip = ClipAutomationAncestors(presenter, root, clip);
        var visible = GetAutomationCaretRectangle(0).TransformToAABB(presenterTransform).Intersect(clip);
        return visible.Width > 0 && visible.Height > 0;
    }

    /// <summary>Gets range rectangles clipped to the visible editor viewport in root DIPs.</summary>
    /// <param name="start">The first document position.</param>
    /// <param name="length">The range length.</param>
    /// <returns>Visible portions of measured text.</returns>
    internal List<Rect> GetAutomationVisibleRectangles(int start, int length)
    {
        var result = new List<Rect>();
        var root = global::Avalonia.Controls.TopLevel.GetTopLevel(this);
        if (!IsVisible || root is null || this.TransformToVisual(root) is not { } editorTransform)
        {
            return result;
        }

        var clip = new Rect(Bounds.Size).TransformToAABB(editorTransform).Intersect(new(root.Bounds.Size));
        var presenter = IsPageLayoutEnabled ? (Control?)_pagedPresenter : _formattedPresenter;
        if (presenter is null)
        {
            return result;
        }

        clip = ClipAutomationAncestors(presenter, root, clip);

        foreach (var rectangle in GetAutomationRangeRectangles(start, length))
        {
            var visible = rectangle.Intersect(clip);
            if (visible.Width > 0 && visible.Height > 0)
            {
                result.Add(visible);
            }
        }

        return result;
    }

    /// <summary>Gets actual page slices or formatted lines.</summary>
    /// <param name="pages">Whether to return page ranges.</param>
    /// <returns>Document ranges in reading order.</returns>
    internal List<(int Start, int End)> GetAutomationUnitRanges(bool pages)
    {
        if (IsPageLayoutEnabled)
        {
            return _pagedPresenter?.GetAutomationUnitRanges(pages) ?? [];
        }

        return pages ? [(0, Document.Length)] : _formattedPresenter?.GetAutomationLineRanges() ?? [];
    }

    /// <summary>Aligns a document range with the top or bottom of its scroll viewport.</summary>
    /// <param name="start">The range start.</param>
    /// <param name="end">The range end.</param>
    /// <param name="alignTop">Whether to align the first line with the viewport top.</param>
    internal void ScrollAutomationRange(int start, int end, bool alignTop)
    {
        var presenter = IsPageLayoutEnabled ? (Control?)_pagedPresenter : _formattedPresenter;
        var scroll = IsPageLayoutEnabled ? _pageScrollViewer : _displayScrollViewer;
        var viewport = scroll is null ? null : FindAutomationViewport(scroll);
        if (presenter is null || scroll is null || viewport is null || presenter.TransformToVisual(viewport) is not { } transform)
        {
            BringAutomationPositionIntoView(alignTop ? start : end);
            return;
        }

        AlignAutomationScroll(scroll, viewport, GetAutomationCaretRectangle(alignTop ? start : Math.Max(start, end - 1)).TransformToAABB(transform), alignTop);
    }

    /// <summary>Clips a root rectangle to each clipping visual ancestor.</summary>
    /// <param name="presenter">The active document presenter.</param>
    /// <param name="root">The coordinate root.</param>
    /// <param name="clip">The initial visible rectangle.</param>
    /// <returns>The clipped rectangle.</returns>
    private static Rect ClipAutomationAncestors(Control presenter, TopLevel root, Rect clip)
    {
        foreach (var ancestor in presenter.GetVisualAncestors())
        {
            if (ancestor is not ScrollContentPresenter && !ancestor.ClipToBounds)
            {
                continue;
            }

            if (ancestor.TransformToVisual(root) is not { } transform)
            {
                return default;
            }

            clip = clip.Intersect(new Rect(ancestor.Bounds.Size).TransformToAABB(transform));
        }

        return clip;
    }

    /// <summary>Aligns measured text with a viewport edge.</summary>
    /// <param name="scroll">The editor scroll viewer.</param>
    /// <param name="viewport">Its measured viewport.</param>
    /// <param name="target">The text rectangle in viewport coordinates.</param>
    /// <param name="alignTop">Whether to align with the top.</param>
    private static void AlignAutomationScroll(global::Avalonia.Controls.ScrollViewer scroll, ScrollContentPresenter viewport, in Rect target, bool alignTop)
    {
        var delta = alignTop ? target.Top : target.Bottom - viewport.Bounds.Height;
        scroll.Offset = new(scroll.Offset.X, Math.Clamp(scroll.Offset.Y + delta, 0, Math.Max(0, scroll.Extent.Height - scroll.Viewport.Height)));
    }

    /// <summary>Finds the measured viewport of the selected scroll viewer.</summary>
    /// <param name="scroll">The editor scroll viewer.</param>
    /// <returns>The viewport, or null before its template is applied.</returns>
    private static ScrollContentPresenter? FindAutomationViewport(global::Avalonia.Controls.ScrollViewer scroll)
    {
        foreach (var descendant in scroll.GetVisualDescendants())
        {
            if (descendant is ScrollContentPresenter viewport)
            {
                return viewport;
            }
        }

        return null;
    }

    /// <summary>Gets measured caret geometry in the active presenter.</summary>
    /// <param name="offset">The document insertion position.</param>
    /// <returns>The presenter-relative caret.</returns>
    private Rect GetAutomationCaretRectangle(int offset) => IsPageLayoutEnabled
        ? _pagedPresenter!.GetDocumentCaretRect(offset)
        : _formattedPresenter!.GetDocumentCaretRect(offset);
}
