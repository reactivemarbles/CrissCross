// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// A button with a gel/glass-like appearance.
/// </summary>
public class GelButton : Button
{
    /// <summary>
    /// Property for <see cref="GlareBrush"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush?> GlareBrushProperty =
        AvaloniaProperty.Register<GelButton, IBrush?>(nameof(GlareBrush));

    /// <summary>
    /// Property for <see cref="FocusBrush"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush?> FocusBrushProperty =
        AvaloniaProperty.Register<GelButton, IBrush?>(nameof(FocusBrush), Brushes.Orange);

    /// <summary>
    /// Property for <see cref="FocusBorderThickness"/>.
    /// </summary>
    public static readonly StyledProperty<Thickness> FocusBorderThicknessProperty =
        AvaloniaProperty.Register<GelButton, Thickness>(nameof(FocusBorderThickness), new Thickness(2.0));

    /// <summary>
    /// Property for <see cref="OuterCornerRadius"/>.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> OuterCornerRadiusProperty =
        AvaloniaProperty.Register<GelButton, CornerRadius>(nameof(OuterCornerRadius), new CornerRadius(3.0));

    /// <summary>
    /// Property for <see cref="InnerCornerRadius"/>.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> InnerCornerRadiusProperty =
        AvaloniaProperty.Register<GelButton, CornerRadius>(nameof(InnerCornerRadius), new CornerRadius(2.0));

    /// <summary>
    /// Gets or sets the glare brush.
    /// </summary>
    public IBrush? GlareBrush
    {
        get => GetValue(GlareBrushProperty);
        set => SetValue(GlareBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the focus brush.
    /// </summary>
    public IBrush? FocusBrush
    {
        get => GetValue(FocusBrushProperty);
        set => SetValue(FocusBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the focus border thickness.
    /// </summary>
    public Thickness FocusBorderThickness
    {
        get => GetValue(FocusBorderThicknessProperty);
        set => SetValue(FocusBorderThicknessProperty, value);
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
