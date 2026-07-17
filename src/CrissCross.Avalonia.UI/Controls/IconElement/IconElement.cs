// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Represents the base class for an icon UI element.</summary>
public class IconElement : Control
{
    /// <summary>Property for <see cref="Foreground"/>.</summary>
    public static readonly StyledProperty<IBrush?> ForegroundProperty = AvaloniaProperty.Register<IconElement, IBrush?>(
        nameof(Foreground),
        defaultValue: Brushes.Black);

    /// <summary>Provides the IconElement member.</summary>
    static IconElement()
    {
        FocusableProperty.OverrideDefaultValue<IconElement>(false);
    }

    /// <summary>Gets or sets the foreground brush.</summary>
    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    /// <summary>Provides the Coerce member.</summary>
    /// <param name="o">The dependency object.</param>
    /// <param name="baseValue">The value to be coerced.</param>
    /// <returns>An IconElement, either directly or derived from an IconSourceElement.</returns>
    public static object? Coerce(AvaloniaObject o, object? baseValue) =>
        baseValue switch
        {
            IconSourceElement iconSourceElement => iconSourceElement.CreateIconElement(),
            IconElement or null => baseValue,
            _ => throw new ArgumentException(
                message: $"Expected either '{typeof(IconSourceElement)}' or '{typeof(IconElement)}' "
                    + $"but got '{baseValue.GetType()}'.",
                paramName: nameof(baseValue)),
        };
}
