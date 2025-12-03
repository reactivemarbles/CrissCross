// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a control that allows a user to pick a time value.
/// </summary>
public class TimePicker : global::Avalonia.Controls.TimePicker
{
    /// <summary>
    /// Property for <see cref="Header"/>.
    /// </summary>
    public static readonly StyledProperty<object> HeaderProperty = AvaloniaProperty.Register<TimePicker, object>(
        nameof(Header), null);

    /// <summary>
    /// Property for <see cref="MinuteIncrement"/>.
    /// </summary>
    public static readonly StyledProperty<int> MinuteIncrementProperty = AvaloniaProperty.Register<TimePicker, int>(
        nameof(MinuteIncrement), 1);

    /// <summary>
    /// Gets or sets the content for the control's header.
    /// </summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the time increments shown in the minute picker.
    /// </summary>
    public int MinuteIncrement
    {
        get => GetValue(MinuteIncrementProperty);
        set => SetValue(MinuteIncrementProperty, value);
    }
}
