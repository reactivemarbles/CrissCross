// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Contains virtualizing-items initialization behavior.</summary>
public partial class VirtualizingItemsControl
{
    /// <inheritdoc />
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        VirtualizingPanel.SetCacheLengthUnit(this, CacheLengthUnit);
        VirtualizingPanel.SetCacheLength(this, new(1));
        VirtualizingPanel.SetIsVirtualizingWhenGrouping(this, true);
    }
}
