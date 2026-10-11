// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides accessible document coordinates and editing actions.</summary>
public partial class RichTextBox
{
    /// <summary>Gets whether accessibility clients may change document content.</summary>
    internal bool IsAutomationReadOnly => IsReadOnlyInternal;

    /// <summary>Gets separate measured page-story peers without changing body offsets.</summary>
    /// <returns>The page-story peers.</returns>
    internal IReadOnlyList<AutomationPeer> GetAutomationStories() => IsPageLayoutEnabled ? _pagedPresenter?.CreateAutomationStories(this) ?? [] : [];

    /// <summary>Gets measured range rectangles in top-level coordinates.</summary>
    /// <param name="start">The first document position.</param>
    /// <param name="length">The range length.</param>
    /// <returns>The measured rectangles, or an empty list before layout.</returns>
    internal IReadOnlyList<Rect> GetAutomationRangeRectangles(int start, int length)
    {
        var presenter = IsPageLayoutEnabled ? (Control?)_pagedPresenter : _formattedPresenter;
        var root = global::Avalonia.Controls.TopLevel.GetTopLevel(this);
        if (presenter is null || root is null || presenter.TransformToVisual(root) is not { } transform)
        {
            return [];
        }

        var rectangles = IsPageLayoutEnabled
            ? _pagedPresenter!.GetDocumentRangeRectangles(start, length)
            : _formattedPresenter!.GetDocumentRangeRectangles(start, length);
        var transformed = new List<Rect>(rectangles.Count);
        foreach (var rectangle in rectangles)
        {
            transformed.Add(rectangle.TransformToAABB(transform));
        }

        return transformed;
    }

    /// <summary>Maps a top-level point to a document offset.</summary>
    /// <param name="point">The point in top-level coordinates.</param>
    /// <returns>The nearest insertion position.</returns>
    internal int GetAutomationDocumentPosition(Point point)
    {
        var presenter = IsPageLayoutEnabled ? (Control?)_pagedPresenter : _formattedPresenter;
        var root = global::Avalonia.Controls.TopLevel.GetTopLevel(this);
        if (presenter is null || root is null || root.TransformToVisual(presenter) is not { } transform)
        {
            return 0;
        }

        var local = point.Transform(transform);
        return IsPageLayoutEnabled ? _pagedPresenter!.GetDocumentPosition(local) : _formattedPresenter!.GetDocumentPosition(local);
    }

    /// <summary>Scrolls an accessible range into view.</summary>
    /// <param name="offset">The document insertion position.</param>
    internal void BringAutomationPositionIntoView(int offset)
    {
        if (IsPageLayoutEnabled && _pagedPresenter is not null)
        {
            _pagedPresenter.BringIntoView(_pagedPresenter.GetDocumentCaretRect(offset));
        }
        else if (_formattedPresenter is not null)
        {
            _formattedPresenter.BringIntoView(_formattedPresenter.GetDocumentCaretRect(offset));
        }
    }

    /// <summary>Activates a hyperlink through the same host event as pointer input.</summary>
    /// <param name="uri">The navigation URI.</param>
    /// <param name="target">The target frame.</param>
    internal void InvokeAutomationHyperlink(string uri, string? target) =>
        HyperlinkActivated?.Invoke(this, new(uri, target));

    /// <inheritdoc/>
    protected override AutomationPeer OnCreateAutomationPeer() => new RichTextBoxAutomationPeer(this);
}
