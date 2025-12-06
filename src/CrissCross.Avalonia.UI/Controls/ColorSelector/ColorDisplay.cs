// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// A control that displays a color preview.
/// </summary>
public class ColorDisplay : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="Color"/>.
    /// </summary>
    public static readonly StyledProperty<Color> ColorProperty =
        AvaloniaProperty.Register<ColorDisplay, Color>(nameof(Color), Colors.Transparent);

    /// <summary>
    /// Property for <see cref="SecondColor"/>.
    /// </summary>
    public static readonly StyledProperty<Color?> SecondColorProperty =
        AvaloniaProperty.Register<ColorDisplay, Color?>(nameof(SecondColor));

    /// <summary>
    /// Property for <see cref="ShowSecondColor"/>.
    /// </summary>
    public static readonly StyledProperty<bool> ShowSecondColorProperty =
        AvaloniaProperty.Register<ColorDisplay, bool>(nameof(ShowSecondColor), false);

    /// <summary>
    /// Property for <see cref="CornerRadius"/>.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<ColorDisplay, CornerRadius>(nameof(CornerRadius), new CornerRadius(4));

    /// <summary>
    /// Gets or sets the primary color to display.
    /// </summary>
    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the secondary color to display.
    /// </summary>
    public Color? SecondColor
    {
        get => GetValue(SecondColorProperty);
        set => SetValue(SecondColorProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the second color.
    /// </summary>
    public bool ShowSecondColor
    {
        get => GetValue(ShowSecondColorProperty);
        set => SetValue(ShowSecondColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
}
