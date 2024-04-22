// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

//// Based on VirtualizingWrapPanel created by S. Bäumlisberger licensed under MIT license.
//// https://github.com/sbaeumlisberger/VirtualizingWrapPanel
//// Copyright (C) S. Bäumlisberger
//// All Rights Reserved.

using System.Drawing;
using System.Windows.Controls;

// ReSharper disable once CheckNamespace
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
