// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays and edits a shared date/time range snapshot.</summary>
public class DateTimeRangePicker : ContentView
{
    /// <summary>Bindable property for <see cref="Range"/>.</summary>
    public static readonly BindableProperty RangeProperty = BindableProperty.Create(
        nameof(Range),
        typeof(DateTimeRange),
        typeof(DateTimeRangePicker),
        propertyChanged: static (bindable, _, newValue) => OnRangeChanged(bindable, newValue));

    /// <summary>Bindable property for <see cref="ApplyRangeCommand"/>.</summary>
    public static readonly BindableProperty ApplyRangeCommandProperty = BindableProperty.Create(
        nameof(ApplyRangeCommand),
        typeof(ICommand),
        typeof(DateTimeRangePicker),
        propertyChanged: OnApplyRangeCommandChanged);

    /// <summary>Provides spacing for the composed range picker rows.</summary>
    private const double LayoutSpacing = 8;

    /// <summary>Displays the current range summary or validation message.</summary>
    private readonly Label _summaryLabel = CreateLabel(isEmphasized: true);

    /// <summary>Edits the start date.</summary>
    private readonly DatePicker _startDatePicker = new();

    /// <summary>Edits the start time.</summary>
    private readonly TimePicker _startTimePicker = new();

    /// <summary>Edits the end date.</summary>
    private readonly DatePicker _endDatePicker = new();

    /// <summary>Edits the end time.</summary>
    private readonly TimePicker _endTimePicker = new();

    /// <summary>Applies the current draft range.</summary>
    private readonly Button _applyButton = new() { Text = "Apply range" };

    /// <summary>Gates range application against current validation and command availability.</summary>
    private readonly Command _applyCommand;

    /// <summary>Stores the external command currently observed for availability changes.</summary>
    private ICommand? _observedApplyRangeCommand;

    /// <summary>Stores the weak availability-change handler attached to the external command.</summary>
    private EventHandler? _applyRangeCanExecuteChangedHandler;

    /// <summary>Prevents control-generated changes while projecting a new state snapshot.</summary>
    private bool _isApplyingState;

    /// <summary>Initializes a new instance of the <see cref="DateTimeRangePicker"/> class.</summary>
    public DateTimeRangePicker()
    {
        _applyCommand = new(ExecuteCurrentRange, CanApplyCurrentRange);
        _startDatePicker.DateSelected += OnDraftChanged;
        _endDatePicker.DateSelected += OnDraftChanged;
        _startTimePicker.PropertyChanged += OnTimePickerPropertyChanged;
        _endTimePicker.PropertyChanged += OnTimePickerPropertyChanged;
        _applyButton.Command = _applyCommand;
        _applyButton.SetDynamicResource(Button.BackgroundColorProperty, "CrissCrossAccentColor");
        _applyButton.SetDynamicResource(Button.TextColorProperty, "CrissCrossAccentTextColor");
        Content = CreateLayout();
        ApplyRange(Range);
    }

    /// <summary>Gets or sets the shared CrissCross state projected by this control.</summary>
    public DateTimeRange? Range
    {
        get => (DateTimeRange?)GetValue(RangeProperty);
        set => SetValue(RangeProperty, value);
    }

    /// <summary>Gets or sets the command invoked by the control surface.</summary>
    public ICommand? ApplyRangeCommand
    {
        get => (ICommand?)GetValue(ApplyRangeCommandProperty);
        set => SetValue(ApplyRangeCommandProperty, value);
    }

    /// <summary>Applies the current date/time draft through <see cref="ApplyRangeCommand"/> when valid.</summary>
    /// <returns><c>true</c> when a valid range command was invoked; otherwise, <c>false</c>.</returns>
    public bool ApplyCurrentRange()
    {
        var range = CreateRangeFromInputs();
        Range = range;
        if (!range.IsValid || ApplyRangeCommand?.CanExecute(range) != true)
        {
            return false;
        }

        ApplyRangeCommand.Execute(range);
        return true;
    }

    /// <summary>Combines a selected local date and time without inventing missing input.</summary>
    /// <param name="date">The selected date.</param>
    /// <param name="time">The selected time.</param>
    /// <param name="offset">The existing endpoint offset, when supplied.</param>
    /// <returns>The selected endpoint, or null for incomplete input.</returns>
    private static DateTimeOffset? CreateEndpoint(DateTime? date, TimeSpan? time, TimeSpan? offset)
    {
        if (date is null || time is null)
        {
            return null;
        }

        var localDateTime = DateOnly.FromDateTime(date.Value).ToDateTime(TimeOnly.FromTimeSpan(time.Value));
        return new(localDateTime, offset ?? TimeZoneInfo.Local.GetUtcOffset(localDateTime));
    }

    /// <summary>Creates a display label.</summary>
    /// <param name="isEmphasized">A value indicating whether the label should use emphasized text.</param>
    /// <returns>The configured label.</returns>
    private static Label CreateLabel(bool isEmphasized)
    {
        var label = new Label { FontAttributes = isEmphasized ? FontAttributes.Bold : FontAttributes.None };
        label.SetDynamicResource(Label.TextColorProperty, "CrissCrossTextColor");
        return label;
    }

    /// <summary>Applies a bindable range state change.</summary>
    /// <param name="bindable">The bindable control.</param>
    /// <param name="newValue">The new range value.</param>
    private static void OnRangeChanged(BindableObject bindable, object newValue)
    {
        if (bindable is not DateTimeRangePicker picker)
        {
            return;
        }

        picker.ApplyRange(newValue as DateTimeRange);
    }

    /// <summary>Applies command availability changes to the current presentation.</summary>
    /// <param name="bindable">The bindable control.</param>
    /// <param name="oldValue">The previous command value.</param>
    /// <param name="newValue">The new command value.</param>
    private static void OnApplyRangeCommandChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not DateTimeRangePicker picker)
        {
            return;
        }

        picker.ObserveApplyRangeCommand(oldValue as ICommand, newValue as ICommand);
        picker.UpdateValidation(picker.CreateRangeFromInputs());
    }

    /// <summary>Creates the native MAUI layout.</summary>
    /// <returns>The composed view.</returns>
    private VerticalStackLayout CreateLayout()
    {
        var root = new VerticalStackLayout { Spacing = LayoutSpacing };
        var startRow = new HorizontalStackLayout { Spacing = LayoutSpacing };
        var endRow = new HorizontalStackLayout { Spacing = LayoutSpacing };
        startRow.Children.Add(_startDatePicker);
        startRow.Children.Add(_startTimePicker);
        endRow.Children.Add(_endDatePicker);
        endRow.Children.Add(_endTimePicker);
        root.Children.Add(_summaryLabel);
        root.Children.Add(startRow);
        root.Children.Add(endRow);
        root.Children.Add(_applyButton);
        return root;
    }

    /// <summary>Projects the current range into the native date/time inputs.</summary>
    /// <param name="range">The range to project.</param>
    private void ApplyRange(DateTimeRange? range)
    {
        _isApplyingState = true;
        try
        {
            _startDatePicker.Date = range?.Start?.Date;
            _startTimePicker.Time = range?.Start?.TimeOfDay;
            _endDatePicker.Date = range?.End?.Date;
            _endTimePicker.Time = range?.End?.TimeOfDay;
        }
        finally
        {
            _isApplyingState = false;
        }

        UpdateValidation(range ?? CreateRangeFromInputs());
    }

    /// <summary>Creates a shared range snapshot from the current native inputs.</summary>
    /// <returns>The draft range.</returns>
    private DateTimeRange CreateRangeFromInputs()
    {
        var existingRange = Range;
        var start = CreateEndpoint(_startDatePicker.Date, _startTimePicker.Time, existingRange?.Start?.Offset);
        var end = CreateEndpoint(_endDatePicker.Date, _endTimePicker.Time, existingRange?.End?.Offset);
        return existingRange is null ? new(start, end) : new(
            start,
            end,
            existingRange.Preset,
            existingRange.Label,
            existingRange.IsEndInclusive,
            existingRange.MaximumDuration);
    }

    /// <summary>Updates validation and apply affordance for a draft range.</summary>
    /// <param name="range">The draft range.</param>
    private void UpdateValidation(DateTimeRange range)
    {
        _summaryLabel.Text = range.IsValid ? range.DisplayText : range.ValidationMessage;
        _applyButton.CommandParameter = range;
        _applyCommand.ChangeCanExecute();
        _applyButton.IsEnabled = _applyCommand.CanExecute(range);
    }

    /// <summary>Observes external command availability without retaining the control from the command event.</summary>
    /// <param name="oldCommand">The previous command value.</param>
    /// <param name="newCommand">The new command value.</param>
    private void ObserveApplyRangeCommand(ICommand? oldCommand, ICommand? newCommand)
    {
        if (ReferenceEquals(_observedApplyRangeCommand, newCommand))
        {
            return;
        }

        if (oldCommand is not null && _applyRangeCanExecuteChangedHandler is not null)
        {
            oldCommand.CanExecuteChanged -= _applyRangeCanExecuteChangedHandler;
        }

        _observedApplyRangeCommand = newCommand;
        if (newCommand is null)
        {
            _applyRangeCanExecuteChangedHandler = null;
            return;
        }

        var weakPicker = new WeakReference<DateTimeRangePicker>(this);
        _applyRangeCanExecuteChangedHandler = (_, _) =>
        {
            if (!weakPicker.TryGetTarget(out var picker))
            {
                return;
            }

            picker.UpdateValidation(picker.CreateRangeFromInputs());
        };
        newCommand.CanExecuteChanged += _applyRangeCanExecuteChangedHandler;
    }

    /// <summary>Updates validation after a date input changes.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The date change arguments.</param>
    private void OnDraftChanged(object? sender, DateChangedEventArgs e)
    {
        _ = sender;
        _ = e;
        if (_isApplyingState)
        {
            return;
        }

        UpdateValidation(CreateRangeFromInputs());
    }

    /// <summary>Updates validation after a time input changes.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The property change arguments.</param>
    private void OnTimePickerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        _ = sender;
        if (_isApplyingState || e.PropertyName != nameof(TimePicker.Time))
        {
            return;
        }

        UpdateValidation(CreateRangeFromInputs());
    }

    /// <summary>Applies a valid draft from the native button.</summary>
    private void ExecuteCurrentRange() => _ = ApplyCurrentRange();

    /// <summary>Checks whether the current draft can be applied.</summary>
    /// <returns>Whether the external command accepts the valid draft.</returns>
    private bool CanApplyCurrentRange()
    {
        var range = CreateRangeFromInputs();
        return range.IsValid && ApplyRangeCommand?.CanExecute(range) == true;
    }
}
