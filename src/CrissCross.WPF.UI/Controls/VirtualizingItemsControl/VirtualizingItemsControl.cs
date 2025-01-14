// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
using System.Windows.Controls;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Virtualized <see cref="ItemsControl"/>.
/// <para>Based on <see href="https://github.com/sbaeumlisberger/VirtualizingWrapPanel"/>.</para>
/// </summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(VirtualizingItemsControl), "VirtualizingItemsControl.bmp")]
public class VirtualizingItemsControl : ItemsControl
{
    /// <summary>
    /// Property for <see cref="CacheLengthUnit"/>.
    /// </summary>
    public static readonly DependencyProperty CacheLengthUnitProperty = DependencyProperty.Register(
        nameof(CacheLengthUnit),
        typeof(VirtualizationCacheLengthUnit),
        typeof(VirtualizingItemsControl),
        new FrameworkPropertyMetadata(VirtualizationCacheLengthUnit.Page));

    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualizingItemsControl"/> class.
    /// </summary>
    public VirtualizingItemsControl()
    {
        VirtualizingPanel.SetCacheLengthUnit(this, CacheLengthUnit);
        VirtualizingPanel.SetCacheLength(this, new VirtualizationCacheLength(1));
        VirtualizingPanel.SetIsVirtualizingWhenGrouping(this, true);
    }

    /// <summary>
    /// Gets or sets the cache length unit.
    /// </summary>
    public VirtualizationCacheLengthUnit CacheLengthUnit
    {
        get => VirtualizingPanel.GetCacheLengthUnit(this);
        set
        {
            SetValue(CacheLengthUnitProperty, value);
            VirtualizingPanel.SetCacheLengthUnit(this, value);
        }
    }
}
