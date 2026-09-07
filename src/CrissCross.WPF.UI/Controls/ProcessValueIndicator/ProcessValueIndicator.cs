// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
using ProcessValueState = CrissCross.Reactive.ProcessValueState;
using ProcessValueStatus = CrissCross.Reactive.ProcessValueStatus;
#else
using ProcessValueState = CrissCross.ProcessValueState;
using ProcessValueStatus = CrissCross.ProcessValueStatus;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Displays a compact industrial process value with range progress and textual condition.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ProcessValueIndicator : System.Windows.Controls.Control
{
    /// <summary>Identifies the <see cref="State"/> dependency property.</summary>
    public static readonly DependencyProperty StateProperty;

    /// <summary>Identifies the <see cref="Label"/> dependency property.</summary>
    public static readonly DependencyProperty LabelProperty;

    /// <summary>Identifies the <see cref="DisplayText"/> dependency property.</summary>
    public static readonly DependencyProperty DisplayTextProperty;

    /// <summary>Identifies the <see cref="StatusText"/> dependency property.</summary>
    public static readonly DependencyProperty StatusTextProperty;

    /// <summary>Identifies the <see cref="Percentage"/> dependency property.</summary>
    public static readonly DependencyProperty PercentageProperty;

    /// <summary>Identifies the <see cref="NormalizedValue"/> dependency property.</summary>
    public static readonly DependencyProperty NormalizedValueProperty;

    /// <summary>Identifies the <see cref="HasValidValue"/> dependency property.</summary>
    public static readonly DependencyProperty HasValidValueProperty;

    /// <summary>Identifies the <see cref="IsAlarm"/> dependency property.</summary>
    public static readonly DependencyProperty IsAlarmProperty;

    /// <summary>Identifies the <see cref="Status"/> dependency property.</summary>
    public static readonly DependencyProperty StatusProperty;

    /// <summary>Fallback state used when no process-value snapshot has been supplied.</summary>
    private static readonly ProcessValueState EmptyState = new(
        "Process value",
        null,
        string.Empty,
        0D,
        100D,
        new() { IsGoodQuality = false });

    /// <summary>Stores the read-only dependency property key for <see cref="Label"/>.</summary>
    private static readonly DependencyPropertyKey LabelPropertyKey;

    /// <summary>Stores the read-only dependency property key for <see cref="DisplayText"/>.</summary>
    private static readonly DependencyPropertyKey DisplayTextPropertyKey;

    /// <summary>Stores the read-only dependency property key for <see cref="StatusText"/>.</summary>
    private static readonly DependencyPropertyKey StatusTextPropertyKey;

    /// <summary>Stores the read-only dependency property key for <see cref="Percentage"/>.</summary>
    private static readonly DependencyPropertyKey PercentagePropertyKey;

    /// <summary>Stores the read-only dependency property key for <see cref="NormalizedValue"/>.</summary>
    private static readonly DependencyPropertyKey NormalizedValuePropertyKey;

    /// <summary>Stores the read-only dependency property key for <see cref="HasValidValue"/>.</summary>
    private static readonly DependencyPropertyKey HasValidValuePropertyKey;

    /// <summary>Stores the read-only dependency property key for <see cref="IsAlarm"/>.</summary>
    private static readonly DependencyPropertyKey IsAlarmPropertyKey;

    /// <summary>Stores the read-only dependency property key for <see cref="Status"/>.</summary>
    private static readonly DependencyPropertyKey StatusPropertyKey;

    /// <summary>Initializes static members of the <see cref="ProcessValueIndicator"/> class.</summary>
    static ProcessValueIndicator()
    {
        StateProperty = DependencyProperty.Register(
            nameof(State),
            typeof(ProcessValueState),
            typeof(ProcessValueIndicator),
            new FrameworkPropertyMetadata(null, OnStateChanged));

        LabelPropertyKey = CreateReadOnlyProperty(nameof(Label), typeof(string), EmptyState.Label);
        LabelProperty = LabelPropertyKey.DependencyProperty;
        DisplayTextPropertyKey = CreateReadOnlyProperty(nameof(DisplayText), typeof(string), EmptyState.DisplayText);
        DisplayTextProperty = DisplayTextPropertyKey.DependencyProperty;
        StatusTextPropertyKey = CreateReadOnlyProperty(nameof(StatusText), typeof(string), EmptyState.StatusText);
        StatusTextProperty = StatusTextPropertyKey.DependencyProperty;
        PercentagePropertyKey = CreateReadOnlyProperty(nameof(Percentage), typeof(double), EmptyState.Percentage);
        PercentageProperty = PercentagePropertyKey.DependencyProperty;
        NormalizedValuePropertyKey = CreateReadOnlyProperty(nameof(NormalizedValue), typeof(double), EmptyState.NormalizedValue);
        NormalizedValueProperty = NormalizedValuePropertyKey.DependencyProperty;
        HasValidValuePropertyKey = CreateReadOnlyProperty(nameof(HasValidValue), typeof(bool), EmptyState.HasValidValue);
        HasValidValueProperty = HasValidValuePropertyKey.DependencyProperty;
        IsAlarmPropertyKey = CreateReadOnlyProperty(nameof(IsAlarm), typeof(bool), EmptyState.IsAlarm);
        IsAlarmProperty = IsAlarmPropertyKey.DependencyProperty;
        StatusPropertyKey = CreateReadOnlyProperty(nameof(Status), typeof(ProcessValueStatus), EmptyState.Status);
        StatusProperty = StatusPropertyKey.DependencyProperty;

        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ProcessValueIndicator),
            new FrameworkPropertyMetadata(typeof(ProcessValueIndicator)));
    }

    /// <summary>Gets or sets the immutable process value snapshot displayed by the indicator.</summary>
    public ProcessValueState? State
    {
        get => (ProcessValueState?)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>Gets the measurement label projected from <see cref="State"/>.</summary>
    public string Label => (string)GetValue(LabelProperty);

    /// <summary>Gets the formatted value and unit projected from <see cref="State"/>.</summary>
    public string DisplayText => (string)GetValue(DisplayTextProperty);

    /// <summary>Gets the textual status projected from <see cref="State"/>.</summary>
    public string StatusText => (string)GetValue(StatusTextProperty);

    /// <summary>Gets the clamped range percentage projected from <see cref="State"/>.</summary>
    public double Percentage => (double)GetValue(PercentageProperty);

    /// <summary>Gets the clamped normalized range position projected from <see cref="State"/>.</summary>
    public double NormalizedValue => (double)GetValue(NormalizedValueProperty);

    /// <summary>Gets a value indicating whether <see cref="State"/> contains a displayable value.</summary>
    public bool HasValidValue => (bool)GetValue(HasValidValueProperty);

    /// <summary>Gets a value indicating whether <see cref="State"/> resolves to an alarm status.</summary>
    public bool IsAlarm => (bool)GetValue(IsAlarmProperty);

    /// <summary>Gets the semantic status projected from <see cref="State"/>.</summary>
    public ProcessValueStatus Status => (ProcessValueStatus)GetValue(StatusProperty);

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => string.IsNullOrEmpty(Label) ? GetType().Name : $"{Label}: {DisplayText}";

    /// <summary>Copies the immutable state snapshot into template-bindable dependency properties.</summary>
    /// <param name="state">The process value state, or <see langword="null"/> for the empty fallback.</param>
    protected virtual void OnStateChanged(ProcessValueState? state) => ApplyState(state ?? EmptyState);

    /// <summary>Creates a read-only dependency property key with shared owner metadata.</summary>
    /// <param name="propertyName">The dependency property name.</param>
    /// <param name="propertyType">The dependency property value type.</param>
    /// <param name="defaultValue">The dependency property default value.</param>
    /// <returns>The registered read-only dependency property key.</returns>
    private static DependencyPropertyKey CreateReadOnlyProperty(
        string propertyName,
        Type propertyType,
        object defaultValue) =>
        DependencyProperty.RegisterReadOnly(
            propertyName,
            propertyType,
            typeof(ProcessValueIndicator),
            new FrameworkPropertyMetadata(defaultValue));

    /// <summary>Handles changes to the <see cref="State"/> dependency property.</summary>
    /// <param name="dependencyObject">The dependency object that received the state.</param>
    /// <param name="args">The dependency property change data.</param>
    private static void OnStateChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not ProcessValueIndicator indicator)
        {
            return;
        }

        indicator.OnStateChanged((ProcessValueState?)args.NewValue);
    }

    /// <summary>Applies the projected values from a process value state.</summary>
    /// <param name="state">The process value state snapshot.</param>
    private void ApplyState(ProcessValueState state)
    {
        SetValue(LabelPropertyKey, state.Label);
        SetValue(DisplayTextPropertyKey, state.DisplayText);
        SetValue(StatusTextPropertyKey, state.StatusText);
        SetValue(PercentagePropertyKey, state.Percentage);
        SetValue(NormalizedValuePropertyKey, state.NormalizedValue);
        SetValue(HasValidValuePropertyKey, state.HasValidValue);
        SetValue(IsAlarmPropertyKey, state.IsAlarm);
        SetValue(StatusPropertyKey, state.Status);
    }
}
