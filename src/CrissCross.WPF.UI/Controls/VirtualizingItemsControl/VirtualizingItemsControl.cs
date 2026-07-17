// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
using System.Windows.Controls;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Virtualized ItemsControl.</summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(VirtualizingItemsControl), "VirtualizingItemsControl.bmp")]
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
}
