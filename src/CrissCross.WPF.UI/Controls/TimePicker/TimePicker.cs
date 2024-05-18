// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a control that allows a user to pick a time value.
/// </summary>
public class TimePicker : System.Windows.Controls.Primitives.ButtonBase
{
    /// <summary>
    /// Property for <see cref="Header"/>.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(object),
        typeof(TimePicker),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="Time"/>.
    /// </summary>
    public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(
        nameof(Time),
        typeof(TimeSpan),
        typeof(TimePicker),
        new PropertyMetadata(TimeSpan.Zero));

    /// <summary>
    /// Property for <see cref="SelectedTime"/>.
    /// </summary>
    public static readonly DependencyProperty SelectedTimeProperty = DependencyProperty.Register(
        nameof(SelectedTime),
        typeof(TimeSpan?),
        typeof(TimePicker),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="MinuteIncrement"/>.
    /// </summary>
    public static readonly DependencyProperty MinuteIncrementProperty = DependencyProperty.Register(
        nameof(MinuteIncrement),
        typeof(int),
        typeof(TimePicker),
        new PropertyMetadata(1));

    /// <summary>
    /// Property for <see cref="ClockIdentifier"/>.
    /// </summary>
    public static readonly DependencyProperty ClockIdentifierProperty = DependencyProperty.Register(
        nameof(ClockIdentifier),
        typeof(ClockIdentifier),
        typeof(TimePicker),
        new PropertyMetadata(ClockIdentifier.Clock24Hour));

    /// <summary>
    /// Gets or sets the content for the control's header.
    /// </summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the time currently set in the time picker.
    /// </summary>
    public TimeSpan Time
    {
        get => (TimeSpan)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    /// <summary>
    /// Gets or sets the time currently selected in the time picker.
    /// </summary>
    public TimeSpan? SelectedTime
    {
        get => (TimeSpan?)GetValue(SelectedTimeProperty);
        set => SetValue(SelectedTimeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the time increments shown in the minute picker.
    /// For example, 15 specifies that the TimePicker minute control displays only the choices 00, 15, 30, 45.
    /// </summary>
    public int MinuteIncrement
    {
        get => (int)GetValue(MinuteIncrementProperty);
        set => SetValue(MinuteIncrementProperty, value);
    }

    /// <summary>
    /// Gets or sets the clock system to use.
    /// </summary>
    public ClockIdentifier ClockIdentifier
    {
        get => (ClockIdentifier)GetValue(ClockIdentifierProperty);
        set => SetValue(ClockIdentifierProperty, value);
    }
}
