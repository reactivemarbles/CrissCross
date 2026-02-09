// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents an items control that supports virtualization of its item containers to improve performance when
/// displaying large collections.
/// </summary>
/// <remarks>VirtualizingItemsControl creates and manages item containers only for items that are visible in the
/// viewport, reducing memory usage and layout overhead. This control is useful when working with large data sets where
/// rendering all items at once would be inefficient. Virtualization behavior may depend on the panel used for layout
/// and the configuration of related properties.</remarks>
public class VirtualizingItemsControl : global::Avalonia.Controls.ItemsControl
{
    /// <summary>
    /// Property for <see cref="CacheLengthUnit"/>.
    /// </summary>
    public static readonly StyledProperty<bool> CacheLengthUnitProperty = AvaloniaProperty.Register<VirtualizingItemsControl, bool>(
        nameof(CacheLengthUnit), false);

    /// <summary>
    /// Gets or sets a value indicating whether the cache length unit is enabled.
    /// </summary>
    public bool CacheLengthUnit
    {
        get => GetValue(CacheLengthUnitProperty);
        set => SetValue(CacheLengthUnitProperty, value);
    }
}
