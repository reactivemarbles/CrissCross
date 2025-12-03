// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Layout;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Simple control that displays a grid of items.
/// </summary>
public class VirtualizingGridView : global::Avalonia.Controls.ListBox
{
    /// <summary>
    /// Property for <see cref="Orientation"/>.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty = AvaloniaProperty.Register<VirtualizingGridView, Orientation>(
        nameof(Orientation), Orientation.Vertical);

    /// <summary>
    /// Gets or sets the orientation.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
}
