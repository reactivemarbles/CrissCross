// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Virtualized <see cref="Avalonia.Controls.ItemsControl"/>.
/// </summary>
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
