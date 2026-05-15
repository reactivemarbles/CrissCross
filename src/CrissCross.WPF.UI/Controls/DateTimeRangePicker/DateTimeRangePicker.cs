// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using CrissCross;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a date/time range picker surface for reports, logs, and dashboard filters.
/// </summary>
public class DateTimeRangePicker : Control
{
    /// <summary>
    /// Property for <see cref="Start"/>.
    /// </summary>
    public static readonly DependencyProperty StartProperty = DependencyProperty.Register(
        nameof(Start),
        typeof(DateTimeOffset?),
        typeof(DateTimeRangePicker),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRangeInputChanged));

    /// <summary>
    /// Property for <see cref="End"/>.
    /// </summary>
    public static readonly DependencyProperty EndProperty = DependencyProperty.Register(
        nameof(End),
        typeof(DateTimeOffset?),
        typeof(DateTimeRangePicker),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRangeInputChanged));

    /// <summary>
    /// Property for <see cref="CurrentRange"/>.
    /// </summary>
    public static readonly DependencyProperty CurrentRangeProperty = DependencyProperty.Register(
        nameof(CurrentRange),
        typeof(DateTimeRange),
        typeof(DateTimeRangePicker),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="SelectedPreset"/>.
    /// </summary>
    public static readonly DependencyProperty SelectedPresetProperty = DependencyProperty.Register(
        nameof(SelectedPreset),
        typeof(DateTimeRangePreset),
        typeof(DateTimeRangePicker),
        new PropertyMetadata(DateTimeRangePreset.Custom, OnRangeInputChanged));

    /// <summary>
    /// Property for <see cref="RangeLabel"/>.
    /// </summary>
    public static readonly DependencyProperty RangeLabelProperty = DependencyProperty.Register(
        nameof(RangeLabel),
        typeof(string),
        typeof(DateTimeRangePicker),
        new PropertyMetadata(null, OnRangeInputChanged));

    /// <summary>
    /// Property for <see cref="ReferenceTime"/>.
    /// </summary>
    public static readonly DependencyProperty ReferenceTimeProperty = DependencyProperty.Register(
        nameof(ReferenceTime),
        typeof(DateTimeOffset),
        typeof(DateTimeRangePicker),
        new PropertyMetadata(default(DateTimeOffset)));

    /// <summary>
    /// Property for <see cref="RangeChangedCommand"/>.
    /// </summary>
    public static readonly DependencyProperty RangeChangedCommandProperty = DependencyProperty.Register(
        nameof(RangeChangedCommand),
        typeof(ICommand),
        typeof(DateTimeRangePicker),
        new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeRangePicker"/> class.
    /// </summary>
    public DateTimeRangePicker()
    {
        ApplyRangeCommand = new RangePickerCommand(ApplyRange);
        ApplyPresetCommand = new RangePickerParameterCommand(ApplyPreset);
        CurrentRange = CreateRange();
    }

    /// <summary>
    /// Gets or sets the selected range start.
    /// </summary>
    public DateTimeOffset? Start
    {
        get => (DateTimeOffset?)GetValue(StartProperty);
        set => SetValue(StartProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected range end.
    /// </summary>
    public DateTimeOffset? End
    {
        get => (DateTimeOffset?)GetValue(EndProperty);
        set => SetValue(EndProperty, value);
    }

    /// <summary>
    /// Gets or sets the current resolved range.
    /// </summary>
    public DateTimeRange? CurrentRange
    {
        get => (DateTimeRange?)GetValue(CurrentRangeProperty);
        set => SetValue(CurrentRangeProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected preset used to resolve the range.
    /// </summary>
    public DateTimeRangePreset SelectedPreset
    {
        get => (DateTimeRangePreset)GetValue(SelectedPresetProperty);
        set => SetValue(SelectedPresetProperty, value);
    }

    /// <summary>
    /// Gets or sets the optional custom label used when formatting the range.
    /// </summary>
    public string? RangeLabel
    {
        get => (string?)GetValue(RangeLabelProperty);
        set => SetValue(RangeLabelProperty, value);
    }

    /// <summary>
    /// Gets or sets the reference instant used to resolve relative presets.
    /// </summary>
    public DateTimeOffset ReferenceTime
    {
        get => (DateTimeOffset)GetValue(ReferenceTimeProperty);
        set => SetValue(ReferenceTimeProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked after the control emits a range.
    /// </summary>
    public ICommand? RangeChangedCommand
    {
        get => (ICommand?)GetValue(RangeChangedCommandProperty);
        set => SetValue(RangeChangedCommandProperty, value);
    }

    /// <summary>
    /// Gets the command that applies the current custom range values.
    /// </summary>
    public ICommand ApplyRangeCommand { get; }

    /// <summary>
    /// Gets the command that applies a named range preset.
    /// </summary>
    public ICommand ApplyPresetCommand { get; }

    /// <summary>
    /// Creates a range from the current picker state.
    /// </summary>
    /// <returns>The resolved date/time range.</returns>
    public DateTimeRange CreateRange() => new(Start, End, SelectedPreset, RangeLabel);

    /// <summary>
    /// Applies the current custom range values and emits a range-changed command.
    /// </summary>
    public void ApplyRange() => EmitRange(CreateRange());

    /// <summary>
    /// Applies a named preset and emits a range-changed command.
    /// </summary>
    /// <param name="preset">The preset identifier or name.</param>
    public void ApplyPreset(object? preset)
    {
        var presetValue = ParsePreset(preset);
        var definition = ResolvePresetDefinition(presetValue);
        var range = definition.CreateRange(ResolveReferenceTime());

        SetCurrentValue(SelectedPresetProperty, presetValue);
        SetCurrentValue(RangeLabelProperty, definition.DisplayName);
        SetCurrentValue(StartProperty, range.Start);
        SetCurrentValue(EndProperty, range.End);
        EmitRange(range);
    }

    private static void OnRangeInputChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is DateTimeRangePicker picker)
        {
            picker.SetCurrentValue(CurrentRangeProperty, picker.CreateRange());
        }
    }

    private static DateTimeRangePreset ParsePreset(object? value)
    {
        if (value is DateTimeRangePreset preset)
        {
            return preset;
        }

        return Enum.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), ignoreCase: true, out DateTimeRangePreset parsed)
            ? parsed
            : DateTimeRangePreset.Custom;
    }

    private static DateTimeRangePresetDefinition ResolvePresetDefinition(DateTimeRangePreset preset) => preset switch
    {
        DateTimeRangePreset.Today => DateTimeRangePresetDefinition.Today,
        DateTimeRangePreset.Yesterday => DateTimeRangePresetDefinition.Yesterday,
        DateTimeRangePreset.LastSevenDays => DateTimeRangePresetDefinition.LastSevenDays,
        DateTimeRangePreset.ThisMonth => DateTimeRangePresetDefinition.ThisMonth,
        _ => DateTimeRangePresetDefinition.Custom
    };

    private void EmitRange(DateTimeRange range)
    {
        CurrentRange = range;
        if (RangeChangedCommand?.CanExecute(range) == true)
        {
            RangeChangedCommand.Execute(range);
        }
    }

    private DateTimeOffset ResolveReferenceTime() => ReferenceTime == default ? DateTimeOffset.Now : ReferenceTime;

    private sealed class RangePickerCommand : ICommand
    {
        private readonly Action _execute;

        public RangePickerCommand(Action execute) => _execute = execute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _execute();
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private sealed class RangePickerParameterCommand : ICommand
    {
        private readonly Action<object?> _execute;

        public RangePickerParameterCommand(Action<object?> execute) => _execute = execute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _execute(parameter);
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
