// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a separator item in a NavigationView control.
/// </summary>
public class NavigationViewItemSeparator : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="Orientation"/>.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<NavigationViewItemSeparator, Orientation>(
            nameof(Orientation),
            Orientation.Horizontal);

    /// <summary>
    /// Gets or sets the orientation of the separator.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
}
