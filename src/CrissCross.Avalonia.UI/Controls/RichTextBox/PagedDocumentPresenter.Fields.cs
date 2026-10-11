// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Resolves page count fields after section pagination completes.</summary>
public sealed partial class PagedDocumentPresenter
{
    /// <summary>Page regions containing dynamic page fields.</summary>
    private readonly List<PageFieldRegion> _pageFieldRegions = [];

    /// <summary>Refreshes region fields with the measured physical sheet count.</summary>
    private void UpdatePageFields()
    {
        foreach (var region in _pageFieldRegions)
        {
            region.Document.SetText(RichPageRegion.RenderFields(region.Html, region.Number, _pages.Count));
        }
    }

    /// <summary>A dynamic region and its owning physical sheet.</summary>
    /// <param name="Document">The region document.</param>
    /// <param name="Html">The field source.</param>
    /// <param name="Number">The one-based sheet number.</param>
    private sealed record PageFieldRegion(FlowDocument Document, string Html, int Number);
}
