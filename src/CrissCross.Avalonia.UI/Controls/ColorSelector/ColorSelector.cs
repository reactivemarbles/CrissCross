// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a control that allows users to select a color.
/// </summary>
public class ColorSelector : Control
{
    /// <summary>
    /// Property for <see cref="SelectedColor"/>.
    /// </summary>
    public static readonly StyledProperty<Color> SelectedColorProperty =
        AvaloniaProperty.Register<ColorSelector, Color>(nameof(SelectedColor), Colors.White);

    /// <summary>
    /// Gets or sets the selected color.
    /// </summary>
    public Color SelectedColor
    {
        get => GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }
}
