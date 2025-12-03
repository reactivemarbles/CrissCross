// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Extended <see cref="Avalonia.Controls.Primitives.ToggleButton"/>.
/// </summary>
public class ToggleButton : global::Avalonia.Controls.Primitives.ToggleButton
{
    /// <summary>
    /// The TreeView item chevron size property.
    /// </summary>
    public static readonly StyledProperty<double> ChevronSizeProperty = AvaloniaProperty.Register<ToggleButton, double>(
        nameof(ChevronSize), 10d);

    /// <summary>
    /// Gets or sets the size of the TreeView item chevron.
    /// </summary>
    public double ChevronSize
    {
        get => GetValue(ChevronSizeProperty);
        set => SetValue(ChevronSizeProperty, value);
    }
}
