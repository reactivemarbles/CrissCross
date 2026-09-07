// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.Primitives;
#if REACTIVELIST_REACTIVE
using ProcessValueState = global::CrissCross.Reactive.ProcessValueState;
using ProcessValueStatus = global::CrissCross.Reactive.ProcessValueStatus;
#else
using ProcessValueState = global::CrissCross.ProcessValueState;
using ProcessValueStatus = global::CrissCross.ProcessValueStatus;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Displays an industrial process value with range, quality, and alarm state.</summary>
public class ProcessValueIndicator : TemplatedControl
{
    /// <summary>Property for <see cref="State"/>.</summary>
    public static readonly StyledProperty<ProcessValueState?> StateProperty =
        AvaloniaProperty.Register<ProcessValueIndicator, ProcessValueState?>(nameof(State));

    /// <summary>Property for <see cref="Label"/>.</summary>
    public static readonly DirectProperty<ProcessValueIndicator, string> LabelProperty =
        AvaloniaProperty.RegisterDirect<ProcessValueIndicator, string>(
            nameof(Label),
            static x => x.Label);

    /// <summary>Property for <see cref="DisplayText"/>.</summary>
    public static readonly DirectProperty<ProcessValueIndicator, string> DisplayTextProperty =
        AvaloniaProperty.RegisterDirect<ProcessValueIndicator, string>(
            nameof(DisplayText),
            static x => x.DisplayText);

    /// <summary>Property for <see cref="StatusText"/>.</summary>
    public static readonly DirectProperty<ProcessValueIndicator, string> StatusTextProperty =
        AvaloniaProperty.RegisterDirect<ProcessValueIndicator, string>(
            nameof(StatusText),
            static x => x.StatusText);

    /// <summary>Property for <see cref="Percentage"/>.</summary>
    public static readonly DirectProperty<ProcessValueIndicator, double> PercentageProperty =
        AvaloniaProperty.RegisterDirect<ProcessValueIndicator, double>(
            nameof(Percentage),
            static x => x.Percentage);

    /// <summary>Property for <see cref="NormalizedValue"/>.</summary>
    public static readonly DirectProperty<ProcessValueIndicator, double> NormalizedValueProperty =
        AvaloniaProperty.RegisterDirect<ProcessValueIndicator, double>(
            nameof(NormalizedValue),
            static x => x.NormalizedValue);

    /// <summary>Property for <see cref="HasValidValue"/>.</summary>
    public static readonly DirectProperty<ProcessValueIndicator, bool> HasValidValueProperty =
        AvaloniaProperty.RegisterDirect<ProcessValueIndicator, bool>(
            nameof(HasValidValue),
            static x => x.HasValidValue);

    /// <summary>Property for <see cref="IsAlarm"/>.</summary>
    public static readonly DirectProperty<ProcessValueIndicator, bool> IsAlarmProperty =
        AvaloniaProperty.RegisterDirect<ProcessValueIndicator, bool>(
            nameof(IsAlarm),
            static x => x.IsAlarm);

    /// <summary>Property for <see cref="Status"/>.</summary>
    public static readonly DirectProperty<ProcessValueIndicator, ProcessValueStatus> StatusProperty =
        AvaloniaProperty.RegisterDirect<ProcessValueIndicator, ProcessValueStatus>(
            nameof(Status),
            static x => x.Status);

    /// <summary>The fallback process-value snapshot.</summary>
    private static readonly ProcessValueState EmptyState = new("Process value", null, string.Empty, 0D, 100D);

    /// <summary>Provides the <see cref="ProcessValueIndicator"/> member.</summary>
    static ProcessValueIndicator() =>
        _ = StateProperty.Changed.AddClassHandler<ProcessValueIndicator>(static (x, _) => x.UpdateProjectedState());

    /// <summary>Gets or sets the immutable process-value snapshot displayed by the indicator.</summary>
    public ProcessValueState? State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>Gets the projected measurement label.</summary>
    public string Label
    {
        get;
        private set => SetAndRaise(LabelProperty, ref field, value);
    } = EmptyState.Label;

    /// <summary>Gets the projected value and unit display text.</summary>
    public string DisplayText
    {
        get;
        private set => SetAndRaise(DisplayTextProperty, ref field, value);
    } = EmptyState.DisplayText;

    /// <summary>Gets the projected status text.</summary>
    public string StatusText
    {
        get;
        private set => SetAndRaise(StatusTextProperty, ref field, value);
    } = EmptyState.StatusText;

    /// <summary>Gets the projected range-bar percentage from zero to one hundred.</summary>
    public double Percentage
    {
        get;
        private set => SetAndRaise(PercentageProperty, ref field, value);
    } = EmptyState.Percentage;

    /// <summary>Gets the projected normalized value from zero to one.</summary>
    public double NormalizedValue
    {
        get;
        private set => SetAndRaise(NormalizedValueProperty, ref field, value);
    } = EmptyState.NormalizedValue;

    /// <summary>Gets a value indicating whether the snapshot has a valid measurement value.</summary>
    public bool HasValidValue
    {
        get;
        private set => SetAndRaise(HasValidValueProperty, ref field, value);
    } = EmptyState.HasValidValue;

    /// <summary>Gets a value indicating whether the snapshot is in a low or high alarm state.</summary>
    public bool IsAlarm
    {
        get;
        private set => SetAndRaise(IsAlarmProperty, ref field, value);
    } = EmptyState.IsAlarm;

    /// <summary>Gets the projected process-value status.</summary>
    public ProcessValueStatus Status
    {
        get;
        private set => SetAndRaise(StatusProperty, ref field, value);
    } = EmptyState.Status;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        UpdateProjectedState();
    }

    /// <summary>Refreshes the public projection from the current immutable state snapshot.</summary>
    private void UpdateProjectedState()
    {
        var state = State ?? EmptyState;

        Label = state.Label;
        DisplayText = state.DisplayText;
        StatusText = state.StatusText;
        Percentage = state.Percentage;
        NormalizedValue = state.NormalizedValue;
        HasValidValue = state.HasValidValue;
        IsAlarm = state.IsAlarm;
        Status = state.Status;
    }
}
