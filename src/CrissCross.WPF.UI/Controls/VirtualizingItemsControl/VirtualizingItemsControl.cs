// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
using System.Windows.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Virtualized ItemsControl.</summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(VirtualizingItemsControl), "VirtualizingItemsControl.bmp")]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public partial class VirtualizingItemsControl : ItemsControl
{
    /// <summary>Property for <see cref="CacheLengthUnit"/>.</summary>
    public static readonly DependencyProperty CacheLengthUnitProperty = DependencyProperty.Register(
        nameof(CacheLengthUnit),
        typeof(VirtualizationCacheLengthUnit),
        typeof(VirtualizingItemsControl),
        new FrameworkPropertyMetadata(VirtualizationCacheLengthUnit.Page));

    /// <summary>Gets or sets the cache length unit.</summary>
    public VirtualizationCacheLengthUnit CacheLengthUnit
    {
        get => VirtualizingPanel.GetCacheLengthUnit(this);
        set
        {
            SetValue(CacheLengthUnitProperty, value);
            VirtualizingPanel.SetCacheLengthUnit(this, value);
        }
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
