// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a button control that displays a header, an optional icon, and supports customizable corner radius.
/// </summary>
public class CardControl : global::Avalonia.Controls.Button, IIconControl
{
    /// <summary>
    /// Property for <see cref="Header"/>.
    /// </summary>
    public static readonly StyledProperty<object> HeaderProperty = AvaloniaProperty.Register<CardControl, object>(nameof(Header));

    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<CardControl, object?>(
        nameof(Icon), null);

    /// <summary>
    /// Property for <see cref="CornerRadius"/>.
    /// </summary>
    public static new readonly StyledProperty<CornerRadius> CornerRadiusProperty = AvaloniaProperty.Register<CardControl, CornerRadius>(
        nameof(CornerRadius), new CornerRadius(0));

    /// <summary>
    /// Gets or sets header is the data used to for the header of each item in the control.
    /// </summary>
    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets displayed icon.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius of the control.
    /// </summary>
    public new CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
}
