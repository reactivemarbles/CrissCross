// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using CrissCross;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents a date/time range picker surface for reports, logs, and dashboard filters.</summary>
public class DateTimeRangePicker : Control
{
    /// <summary>Property for <see cref="Start"/>.</summary>
    public static readonly DependencyProperty StartProperty = DependencyProperty.Register(
        nameof(Start),
        typeof(DateTimeOffset?),
        typeof(DateTimeRangePicker),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnRangeInputChanged));

    /// <summary>Property for <see cref="End"/>.</summary>
    public static readonly DependencyProperty EndProperty = DependencyProperty.Register(
        nameof(End),
        typeof(DateTimeOffset?),
        typeof(DateTimeRangePicker),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnRangeInputChanged));

    /// <summary>Property for <see cref="CurrentRange"/>.</summary>
    public static readonly DependencyProperty CurrentRangeProperty = DependencyProperty.Register(
        nameof(CurrentRange),
        typeof(DateTimeRange),
        typeof(DateTimeRangePicker),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="SelectedPreset"/>.</summary>
    public static readonly DependencyProperty SelectedPresetProperty = DependencyProperty.Register(
        nameof(SelectedPreset),
        typeof(DateTimeRangePreset),
        typeof(DateTimeRangePicker),
        new PropertyMetadata(DateTimeRangePreset.Custom, OnRangeInputChanged));

    /// <summary>Property for <see cref="RangeLabel"/>.</summary>
    public static readonly DependencyProperty RangeLabelProperty = DependencyProperty.Register(
        nameof(RangeLabel),
        typeof(string),
        typeof(DateTimeRangePicker),
        new PropertyMetadata(null, OnRangeInputChanged));

    /// <summary>Property for <see cref="ReferenceTime"/>.</summary>
    public static readonly DependencyProperty ReferenceTimeProperty = DependencyProperty.Register(
        nameof(ReferenceTime),
        typeof(DateTimeOffset),
        typeof(DateTimeRangePicker),
        new PropertyMetadata(default(DateTimeOffset)));

    /// <summary>Property for <see cref="RangeChangedCommand"/>.</summary>
    public static readonly DependencyProperty RangeChangedCommandProperty = DependencyProperty.Register(
        nameof(RangeChangedCommand),
        typeof(ICommand),
        typeof(DateTimeRangePicker),
        new PropertyMetadata(null));

    /// <summary>Initializes a new instance of the <see cref="DateTimeRangePicker"/> class.</summary>
    public DateTimeRangePicker()
    {
        ApplyRangeCommand = new RangePickerCommand(ApplyRange);
        ApplyPresetCommand = new RangePickerParameterCommand(ApplyPreset);
        CurrentRange = CreateRange();
    }

    /// <summary>Gets or sets the selected range start.</summary>
    public DateTimeOffset? Start
    {
        get => (DateTimeOffset?)GetValue(StartProperty);
        set => SetValue(StartProperty, value);
    }

    /// <summary>Gets or sets the selected range end.</summary>
    public DateTimeOffset? End
    {
        get => (DateTimeOffset?)GetValue(EndProperty);
        set => SetValue(EndProperty, value);
    }

    /// <summary>Gets or sets the current resolved range.</summary>
    public DateTimeRange? CurrentRange
    {
        get => (DateTimeRange?)GetValue(CurrentRangeProperty);
        set => SetValue(CurrentRangeProperty, value);
    }

    /// <summary>Gets or sets the selected preset used to resolve the range.</summary>
    public DateTimeRangePreset SelectedPreset
    {
        get => (DateTimeRangePreset)GetValue(SelectedPresetProperty);
        set => SetValue(SelectedPresetProperty, value);
    }

    /// <summary>Gets or sets the optional custom label used when formatting the range.</summary>
    public string? RangeLabel
    {
        get => (string?)GetValue(RangeLabelProperty);
        set => SetValue(RangeLabelProperty, value);
    }

    /// <summary>Gets or sets the reference instant used to resolve relative presets.</summary>
    public DateTimeOffset ReferenceTime
    {
        get => (DateTimeOffset)GetValue(ReferenceTimeProperty);
        set => SetValue(ReferenceTimeProperty, value);
    }

    /// <summary>Gets or sets the command invoked after the control emits a range.</summary>
    public ICommand? RangeChangedCommand
    {
        get => (ICommand?)GetValue(RangeChangedCommandProperty);
        set => SetValue(RangeChangedCommandProperty, value);
    }

    /// <summary>Gets the command that applies the current custom range values.</summary>
    public ICommand ApplyRangeCommand { get; }

    /// <summary>Gets the command that applies a named range preset.</summary>
    public ICommand ApplyPresetCommand { get; }

    /// <summary>Creates a range from the current picker state.</summary>
    /// <returns>The resolved date/time range.</returns>
    public DateTimeRange CreateRange() => new(Start, End, SelectedPreset, RangeLabel);

    /// <summary>Applies the current custom range values and emits a range-changed command.</summary>
    public void ApplyRange() => EmitRange(CreateRange());

    /// <summary>Applies a named preset and emits a range-changed command.</summary>
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

    /// <summary>Provides the OnRangeInputChanged member.</summary>
    /// <param name="dependencyObject">The dependencyObject value.</param>
    /// <param name="args">The event arguments.</param>
    private static void OnRangeInputChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        _ = args;
        if (dependencyObject is not DateTimeRangePicker picker)
        {
            return;
        }

        picker.SetCurrentValue(CurrentRangeProperty, picker.CreateRange());
    }

    /// <summary>Provides the ParsePreset member.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
    private static DateTimeRangePreset ParsePreset(object? value)
    {
        if (value is DateTimeRangePreset preset)
        {
            return preset;
        }

        return Enum.TryParse(
            Convert.ToString(value, CultureInfo.InvariantCulture),
            ignoreCase: true,
            out DateTimeRangePreset parsed)
            ? parsed
            : DateTimeRangePreset.Custom;
    }

    /// <summary>Provides the ResolvePresetDefinition member.</summary>
    /// <param name="preset">The preset value.</param>
    /// <returns>The result.</returns>
    private static DateTimeRangePresetDefinition ResolvePresetDefinition(DateTimeRangePreset preset) =>
        preset switch
        {
            DateTimeRangePreset.Today => DateTimeRangePresetDefinition.Today,
            DateTimeRangePreset.Yesterday => DateTimeRangePresetDefinition.Yesterday,
            DateTimeRangePreset.LastSevenDays => DateTimeRangePresetDefinition.LastSevenDays,
            DateTimeRangePreset.ThisMonth => DateTimeRangePresetDefinition.ThisMonth,
            _ => DateTimeRangePresetDefinition.Custom,
        };

    /// <summary>Provides the EmitRange member.</summary>
    /// <param name="range">The range value.</param>
    private void EmitRange(DateTimeRange range)
    {
        CurrentRange = range;
        if (RangeChangedCommand?.CanExecute(range) != true)
        {
            return;
        }

        RangeChangedCommand.Execute(range);
    }

    /// <summary>Provides the ResolveReferenceTime member.</summary>
    /// <returns>The result.</returns>
    private DateTimeOffset ResolveReferenceTime() => ReferenceTime == default ? DateTimeOffset.Now : ReferenceTime;

    /// <summary>Provides the RangePickerCommand member.</summary>
    private sealed class RangePickerCommand : ICommand
    {
        /// <summary>Stores the _execute value.</summary>
        private readonly Action _execute;

        /// <summary>Initializes a new instance of the <see cref="RangePickerCommand"/> class.</summary>
        /// <param name="execute">The execute value.</param>
        public RangePickerCommand(Action execute) => _execute = execute;

        /// <summary>Provides the CanExecuteChanged member.</summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>Provides the CanExecute member.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The result.</returns>
        public bool CanExecute(object? parameter) => true;

        /// <summary>Provides the Execute member.</summary>
        /// <param name="parameter">The parameter.</param>
        public void Execute(object? parameter)
        {
            _execute();
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>Provides the RangePickerParameterCommand member.</summary>
    private sealed class RangePickerParameterCommand : ICommand
    {
        /// <summary>Stores the _execute value.</summary>
        private readonly Action<object?> _execute;

        /// <summary>Initializes a new instance of the <see cref="RangePickerParameterCommand"/> class.</summary>
        /// <param name="execute">The execute value.</param>
        public RangePickerParameterCommand(Action<object?> execute) => _execute = execute;

        /// <summary>Provides the CanExecuteChanged member.</summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>Provides the CanExecute member.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The result.</returns>
        public bool CanExecute(object? parameter) => true;

        /// <summary>Provides the Execute member.</summary>
        /// <param name="parameter">The parameter.</param>
        public void Execute(object? parameter)
        {
            _execute(parameter);
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
