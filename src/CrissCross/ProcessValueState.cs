// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Globalization;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Provides an immutable, platform-neutral snapshot for an industrial process-value indicator.</summary>
public sealed class ProcessValueState
{
    /// <summary>Converts a normalized value to a percentage.</summary>
    private const double PercentageScale = 100;

    /// <summary>Scales extreme finite endpoints before calculating their difference.</summary>
    private const double RangeScale = 2;

    /// <inheritdoc />
    public ProcessValueState(string? label, double? value, string? unit, double minimum, double maximum)
        : this(label, value, unit, minimum, maximum, null) { }

    /// <summary>Initializes a new instance of the <see cref="ProcessValueState"/> class with alarm limits and good quality.</summary>
    /// <param name="label">The measurement name.</param>
    /// <param name="value">The measurement, or null when unavailable.</param>
    /// <param name="unit">The engineering unit.</param>
    /// <param name="minimum">The lower display endpoint.</param>
    /// <param name="maximum">The upper display endpoint.</param>
    /// <param name="lowAlarmLimit">The optional inclusive low alarm limit.</param>
    /// <param name="highAlarmLimit">The optional inclusive high alarm limit.</param>
    public ProcessValueState(string? label, double? value, string? unit, double minimum, double maximum, double? lowAlarmLimit, double? highAlarmLimit)
        : this(label, value, unit, minimum, maximum, new ProcessValueOptions { LowAlarmLimit = lowAlarmLimit, HighAlarmLimit = highAlarmLimit }) { }

    /// <summary>Initializes a new instance of the <see cref="ProcessValueState"/> class.</summary>
    /// <param name="label">The measurement name.</param>
    /// <param name="value">The measured value, or <see langword="null"/> when unavailable.</param>
    /// <param name="unit">The engineering unit.</param>
    /// <param name="minimum">The lower display range endpoint.</param>
    /// <param name="maximum">The upper display range endpoint, strictly greater than the minimum.</param>
    /// <param name="options">The alarm and quality options copied into this snapshot, or null for defaults.</param>
    public ProcessValueState(
        string? label,
        double? value,
        string? unit,
        double minimum,
        double maximum,
        ProcessValueOptions? options)
    {
        Label = label?.Trim() ?? string.Empty;
        Value = value;
        Unit = unit?.Trim() ?? string.Empty;
        Minimum = minimum;
        Maximum = maximum;
        LowAlarmLimit = options?.LowAlarmLimit;
        HighAlarmLimit = options?.HighAlarmLimit;
        IsGoodQuality = options?.IsGoodQuality ?? true;
        Status = ResolveStatus();
    }

    /// <summary>Gets the measurement name.</summary>
    public string Label { get; }

    /// <summary>Gets the original measurement, including invalid source values.</summary>
    public double? Value { get; }

    /// <summary>Gets the engineering unit.</summary>
    public string Unit { get; }

    /// <summary>Gets the lower display range endpoint.</summary>
    public double Minimum { get; }

    /// <summary>Gets the upper display range endpoint.</summary>
    public double Maximum { get; }

    /// <summary>Gets the optional inclusive low alarm limit.</summary>
    public double? LowAlarmLimit { get; }

    /// <summary>Gets the optional inclusive high alarm limit.</summary>
    public double? HighAlarmLimit { get; }

    /// <summary>Gets a value indicating whether the source reports good data quality.</summary>
    public bool IsGoodQuality { get; }

    /// <summary>Gets the resolved condition; invalid configuration takes precedence over data quality and alarms.</summary>
    public ProcessValueStatus Status { get; }

    /// <summary>Gets a value indicating whether the measurement can be displayed as a valid reading.</summary>
    public bool HasValidValue => Status is ProcessValueStatus.Normal or ProcessValueStatus.LowAlarm or ProcessValueStatus.HighAlarm;

    /// <summary>Gets a value indicating whether an alarm limit has been reached.</summary>
    public bool IsAlarm => Status is ProcessValueStatus.LowAlarm or ProcessValueStatus.HighAlarm;

    /// <summary>Gets invariant measurement text, or an em dash when the reading is invalid.</summary>
    public string ValueText => HasValidValue ? Value!.Value.ToString("0.###", CultureInfo.InvariantCulture) : "—";

    /// <summary>Gets measurement text with its engineering unit.</summary>
    public string DisplayText => Unit.Length == 0 ? ValueText : $"{ValueText} {Unit}";

    /// <summary>Gets a textual condition so that alarm and quality information never relies on color alone.</summary>
    public string StatusText => Status switch
    {
        ProcessValueStatus.Normal => "Normal",
        ProcessValueStatus.LowAlarm => "Low alarm",
        ProcessValueStatus.HighAlarm => "High alarm",
        ProcessValueStatus.BadQuality => "Bad quality",
        _ => "Invalid configuration",
    };

    /// <summary>Gets the clamped position in the display range, from zero to one, or zero for an invalid reading.</summary>
    public double NormalizedValue
    {
        get
        {
            if (!HasValidValue || Value!.Value <= Minimum)
            {
                return 0;
            }

            return Value.Value >= Maximum ? 1 : NormalizeWithinRange(Value.Value);
        }
    }

    /// <summary>Gets the clamped display range percentage.</summary>
    public double Percentage => NormalizedValue * PercentageScale;

    /// <summary>Determines whether a number is finite on every supported framework.</summary>
    /// <param name="value">The number to inspect.</param>
    /// <returns>Whether the number is neither NaN nor infinity.</returns>
    private static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);

    /// <summary>Validates an optional finite alarm endpoint.</summary>
    /// <param name="value">The optional endpoint.</param>
    /// <returns>Whether the endpoint is absent or finite.</returns>
    private static bool IsValidLimit(double? value) => !value.HasValue || IsFinite(value.Value);

    /// <summary>Normalizes a valid interior measurement without overflowing wide ranges.</summary>
    /// <param name="value">The finite interior measurement.</param>
    /// <returns>The normalized fraction.</returns>
    private double NormalizeWithinRange(double value)
    {
        var span = Maximum - Minimum;
        return double.IsInfinity(span)
            ? ((value / RangeScale) - (Minimum / RangeScale)) / ((Maximum / RangeScale) - (Minimum / RangeScale))
            : (value - Minimum) / span;
    }

    /// <summary>Validates the range and optional alarm limits.</summary>
    /// <returns>Whether all configured endpoints are usable.</returns>
    private bool HasValidConfiguration() =>
        IsFinite(Minimum) && IsFinite(Maximum) && Maximum > Minimum
        && IsValidLimit(LowAlarmLimit) && IsValidLimit(HighAlarmLimit)
        && HasOrderedLimits();

    /// <summary>Validates the order of a pair of alarm limits.</summary>
    /// <returns>Whether the limits are absent or strictly ordered.</returns>
    private bool HasOrderedLimits() => !LowAlarmLimit.HasValue || !HighAlarmLimit.HasValue || LowAlarmLimit.Value < HighAlarmLimit.Value;

    /// <summary>Resolves configuration, quality, and inclusive alarm limits.</summary>
    /// <returns>The displayed condition.</returns>
    private ProcessValueStatus ResolveStatus()
    {
        if (!HasValidConfiguration())
        {
            return ProcessValueStatus.InvalidConfiguration;
        }

        if (!IsGoodQuality || !Value.HasValue || !IsFinite(Value.Value))
        {
            return ProcessValueStatus.BadQuality;
        }

        if (LowAlarmLimit.HasValue && Value.Value <= LowAlarmLimit.Value)
        {
            return ProcessValueStatus.LowAlarm;
        }

        return HighAlarmLimit.HasValue && Value.Value >= HighAlarmLimit.Value
            ? ProcessValueStatus.HighAlarm
            : ProcessValueStatus.Normal;
    }
}
