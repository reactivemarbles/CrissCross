// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// A repeat button with a beveled/bezel appearance.
/// </summary>
public class BezelRepeatButton : global::Avalonia.Controls.RepeatButton
{
    /// <summary>
    /// Property for <see cref="GlareOpacityMask"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush?> GlareOpacityMaskProperty =
        AvaloniaProperty.Register<BezelRepeatButton, IBrush?>(nameof(GlareOpacityMask));

    /// <summary>
    /// Property for <see cref="PressedBrush"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush?> PressedBrushProperty =
        AvaloniaProperty.Register<BezelRepeatButton, IBrush?>(nameof(PressedBrush), Brushes.Green);

    /// <summary>
    /// Property for <see cref="OuterCornerRadius"/>.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> OuterCornerRadiusProperty =
        AvaloniaProperty.Register<BezelRepeatButton, CornerRadius>(nameof(OuterCornerRadius), new CornerRadius(3.0));

    /// <summary>
    /// Property for <see cref="InnerCornerRadius"/>.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> InnerCornerRadiusProperty =
        AvaloniaProperty.Register<BezelRepeatButton, CornerRadius>(nameof(InnerCornerRadius), new CornerRadius(2.0));

    /// <summary>
    /// Gets or sets the glare opacity mask.
    /// </summary>
    public IBrush? GlareOpacityMask
    {
        get => GetValue(GlareOpacityMaskProperty);
        set => SetValue(GlareOpacityMaskProperty, value);
    }

    /// <summary>
    /// Gets or sets the pressed brush.
    /// </summary>
    public IBrush? PressedBrush
    {
        get => GetValue(PressedBrushProperty);
        set => SetValue(PressedBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the outer corner radius.
    /// </summary>
    public CornerRadius OuterCornerRadius
    {
        get => GetValue(OuterCornerRadiusProperty);
        set => SetValue(OuterCornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the inner corner radius.
    /// </summary>
    public CornerRadius InnerCornerRadius
    {
        get => GetValue(InnerCornerRadiusProperty);
        set => SetValue(InnerCornerRadiusProperty, value);
    }
}
