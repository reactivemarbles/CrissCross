// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
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
    public static readonly StyledProperty<object?> HeaderProperty = AvaloniaProperty.Register<TimePicker, object?>(
        nameof(Header));

    /// <summary>
    /// Property for <see cref="Time"/>.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> TimeProperty = AvaloniaProperty.Register<TimePicker, TimeSpan>(
        nameof(Time), TimeSpan.Zero);

    /// <summary>
    /// Property for <see cref="ClockIdentifier"/>.
    /// </summary>
    public static new readonly StyledProperty<ClockIdentifier> ClockIdentifierProperty = AvaloniaProperty.Register<TimePicker, ClockIdentifier>(
        nameof(ClockIdentifier), ClockIdentifier.Clock24Hour);

    /// <summary>
    /// Initializes a new instance of the <see cref="TimePicker"/> class.
    /// </summary>
    public TimePicker()
    {
        // Sync Time property with SelectedTime
        this.GetObservable(SelectedTimeProperty).Subscribe(selectedTime =>
        {
            if (selectedTime.HasValue)
            {
                Time = selectedTime.Value;
            }
        });

        this.GetObservable(TimeProperty).Subscribe(time =>
        {
            if (SelectedTime != time)
            {
                SelectedTime = time;
            }
        });
    }

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
        get => GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    /// <summary>
    /// Gets or sets the clock system to use (12-hour or 24-hour).
    /// </summary>
    public new ClockIdentifier ClockIdentifier
    {
        get => GetValue(ClockIdentifierProperty);
        set => SetValue(ClockIdentifierProperty, value);
    }
}
