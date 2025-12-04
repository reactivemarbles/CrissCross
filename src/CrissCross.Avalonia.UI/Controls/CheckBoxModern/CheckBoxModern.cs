// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Check Box Modern.
/// </summary>
public class CheckBoxModern : global::Avalonia.Controls.CheckBox
{
    /// <summary>
    /// The box size property.
    /// </summary>
    public static readonly StyledProperty<double> BoxSizeProperty = AvaloniaProperty.Register<CheckBoxModern, double>(
        nameof(BoxSize), 40d);

    /// <summary>
    /// The check background property.
    /// </summary>
    public static readonly StyledProperty<IBrush> CheckBackgroundProperty = AvaloniaProperty.Register<CheckBoxModern, IBrush>(
        nameof(CheckBackground), Brushes.White);

    /// <summary>
    /// The is checked background property.
    /// </summary>
    public static readonly StyledProperty<IBrush> IsCheckedBackgroundProperty = AvaloniaProperty.Register<CheckBoxModern, IBrush>(
        nameof(IsCheckedBackground), Brushes.LightGray);

    /// <summary>
    /// The checked symbol property.
    /// </summary>
    public static readonly StyledProperty<Icons> CheckedSymbolProperty = AvaloniaProperty.Register<CheckBoxModern, Icons>(
        nameof(CheckedSymbol), Icons.Tick);

    /// <summary>
    /// The disabled state property.
    /// </summary>
    public static readonly StyledProperty<DisabledState> DisabledStateProperty = AvaloniaProperty.Register<CheckBoxModern, DisabledState>(
        nameof(DisabledState), DisabledState.Ignore);

    /// <summary>
    /// The dock side property.
    /// </summary>
    public static readonly StyledProperty<Dock> DockSideProperty = AvaloniaProperty.Register<CheckBoxModern, Dock>(
        nameof(DockSide), Dock.Left);

    /// <summary>
    /// The RadioButton style property.
    /// </summary>
    public static readonly StyledProperty<bool> RadioButtonStyleProperty = AvaloniaProperty.Register<CheckBoxModern, bool>(
        nameof(RadioButtonStyle), false);

    /// <summary>
    /// The stroke property.
    /// </summary>
    public static readonly StyledProperty<IBrush> StrokeProperty = AvaloniaProperty.Register<CheckBoxModern, IBrush>(
        nameof(Stroke), Brushes.Black);

    /// <summary>
    /// The stroke thickness property.
    /// </summary>
    public static readonly StyledProperty<double> StrokeThicknessProperty = AvaloniaProperty.Register<CheckBoxModern, double>(
        nameof(StrokeThickness), 1d);

    /// <summary>
    /// The unchecked symbol property.
    /// </summary>
    public static readonly StyledProperty<Icons> UncheckedSymbolProperty = AvaloniaProperty.Register<CheckBoxModern, Icons>(
        nameof(UncheckedSymbol), Icons.None);

    /// <summary>
    /// Gets or sets the size of the box.
    /// </summary>
    public double BoxSize
    {
        get => GetValue(BoxSizeProperty);
        set => SetValue(BoxSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the check background.
    /// </summary>
    public IBrush CheckBackground
    {
        get => GetValue(CheckBackgroundProperty);
        set => SetValue(CheckBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the checked background.
    /// </summary>
    public IBrush IsCheckedBackground
    {
        get => GetValue(IsCheckedBackgroundProperty);
        set => SetValue(IsCheckedBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the checked symbol.
    /// </summary>
    public Icons CheckedSymbol
    {
        get => GetValue(CheckedSymbolProperty);
        set => SetValue(CheckedSymbolProperty, value);
    }

    /// <summary>
    /// Gets or sets the disabled state.
    /// </summary>
    public DisabledState DisabledState
    {
        get => GetValue(DisabledStateProperty);
        set => SetValue(DisabledStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the dock side.
    /// </summary>
    public Dock DockSide
    {
        get => GetValue(DockSideProperty);
        set => SetValue(DockSideProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [RadioButton style].
    /// </summary>
    public bool RadioButtonStyle
    {
        get => GetValue(RadioButtonStyleProperty);
        set => SetValue(RadioButtonStyleProperty, value);
    }

    /// <summary>
    /// Gets or sets the stroke.
    /// </summary>
    public IBrush Stroke
    {
        get => GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    /// <summary>
    /// Gets or sets the stroke thickness.
    /// </summary>
    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets the unchecked symbol.
    /// </summary>
    public Icons UncheckedSymbol
    {
        get => GetValue(UncheckedSymbolProperty);
        set => SetValue(UncheckedSymbolProperty, value);
    }
}
