// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Inherited from the <see cref="Avalonia.Controls.Button"/>, adding icon support.
/// </summary>
public class Button : global::Avalonia.Controls.Button, IAppearanceControl, IIconControl
{
    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly StyledProperty<object> IconProperty = AvaloniaProperty.Register<Button, object>(
        nameof(Icon));

    /// <summary>
    /// Property for <see cref="Appearance"/>.
    /// </summary>
    public static readonly StyledProperty<ControlAppearance> AppearanceProperty = AvaloniaProperty.Register<Button, ControlAppearance>(
        nameof(Appearance), ControlAppearance.Primary);

    /// <summary>
    /// Property for <see cref="MouseOverBackground"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> MouseOverBackgroundProperty = AvaloniaProperty.Register<Button, IBrush>(
        nameof(MouseOverBackground));

    /// <summary>
    /// Property for <see cref="MouseOverBorderBrush"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> MouseOverBorderBrushProperty = AvaloniaProperty.Register<Button, IBrush>(
        nameof(MouseOverBorderBrush));

    /// <summary>
    /// Property for <see cref="PressedForeground"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> PressedForegroundProperty = AvaloniaProperty.Register<Button, IBrush>(
        nameof(PressedForeground));

    /// <summary>
    /// Property for <see cref="PressedBackground"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> PressedBackgroundProperty = AvaloniaProperty.Register<Button, IBrush>(
        nameof(PressedBackground));

    /// <summary>
    /// Property for <see cref="PressedBorderBrush"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> PressedBorderBrushProperty = AvaloniaProperty.Register<Button, IBrush>(
        nameof(PressedBorderBrush));

    /// <summary>
    /// Property for <see cref="CornerRadius"/>.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty = AvaloniaProperty.Register<Button, CornerRadius>(
        nameof(CornerRadius));

    /// <summary>
    /// Gets or sets displayed icon.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <inheritdoc />
    public ControlAppearance Appearance
    {
        get => GetValue(AppearanceProperty);
        set => SetValue(AppearanceProperty, value);
    }

    /// <summary>
    /// Gets or sets background brush when the user interacts with an element with a pointing device.
    /// </summary>
    public IBrush MouseOverBackground
    {
        get => GetValue(MouseOverBackgroundProperty);
        set => SetValue(MouseOverBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets border brush when the user interacts with an element with a pointing device.
    /// </summary>
    public IBrush MouseOverBorderBrush
    {
        get => GetValue(MouseOverBorderBrushProperty);
        set => SetValue(MouseOverBorderBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets foreground when pressed.
    /// </summary>
    public IBrush PressedForeground
    {
        get => GetValue(PressedForegroundProperty);
        set => SetValue(PressedForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets background brush when the user clicks the button.
    /// </summary>
    public IBrush PressedBackground
    {
        get => GetValue(PressedBackgroundProperty);
        set => SetValue(PressedBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets border brush when the user clicks the button.
    /// </summary>
    public IBrush PressedBorderBrush
    {
        get => GetValue(PressedBorderBrushProperty);
        set => SetValue(PressedBorderBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that represents the degree to which the corners are rounded.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
}
