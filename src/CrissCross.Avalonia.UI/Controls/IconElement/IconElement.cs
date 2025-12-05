// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents the base class for an icon UI element.
/// </summary>
public abstract class IconElement : Control
{
    /// <summary>
    /// Property for <see cref="Foreground"/>.
    /// </summary>
    public static new readonly StyledProperty<IBrush?> ForegroundProperty = AvaloniaProperty.Register<IconElement, IBrush?>(
        nameof(Foreground), defaultValue: Brushes.Black);

    static IconElement()
    {
        FocusableProperty.OverrideDefaultValue<IconElement>(false);
    }

    /// <summary>
    /// Gets or sets the foreground brush.
    /// </summary>
    public new IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    /// <summary>
    /// Coerces the value of an Icon dependency property, allowing the use of either IconElement or IconSourceElement.
    /// </summary>
    /// <param name="o">The dependency object.</param>
    /// <param name="baseValue">The value to be coerced.</param>
    /// <returns>An IconElement, either directly or derived from an IconSourceElement.</returns>
    public static object? Coerce(AvaloniaObject o, object? baseValue) => baseValue switch
    {
        IconSourceElement iconSourceElement => iconSourceElement.CreateIconElement(),
        IconElement or null => baseValue,
        _
            => throw new ArgumentException(
                message: $"Expected either '{typeof(IconSourceElement)}' or '{typeof(IconElement)}' but got '{baseValue.GetType()}'.",
                paramName: nameof(baseValue))
    };
}
