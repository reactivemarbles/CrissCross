// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace CrissCross;

/// <summary>
/// Represents a platform-neutral date/time range for filtering, reporting, and dashboard controls.
/// </summary>
public sealed class DateTimeRange
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeRange"/> class.
    /// </summary>
    /// <param name="start">The inclusive range start.</param>
    /// <param name="end">The inclusive or exclusive range end.</param>
    /// <param name="preset">The preset that produced the range.</param>
    /// <param name="label">Optional user-facing label used by range picker controls.</param>
    /// <param name="isEndInclusive">A value indicating whether the end instant is inclusive.</param>
    /// <param name="maximumDuration">Optional maximum allowed duration.</param>
    public DateTimeRange(
        DateTimeOffset? start,
        DateTimeOffset? end,
        DateTimeRangePreset preset = DateTimeRangePreset.Custom,
        string? label = null,
        bool isEndInclusive = true,
        TimeSpan? maximumDuration = null)
    {
        Start = start;
        End = end;
        Preset = preset;
        Label = ResolveLabel(label, preset);
        IsEndInclusive = isEndInclusive;
        MaximumDuration = maximumDuration;
        IsReversed = Start.HasValue && End.HasValue && Start.Value > End.Value;
        ExceedsMaximumDuration = !IsReversed && MaximumDuration.HasValue && Duration > MaximumDuration.Value;
    }

    /// <summary>
    /// Gets the inclusive range start.
    /// </summary>
    public DateTimeOffset? Start { get; }

    /// <summary>
    /// Gets the inclusive or exclusive range end.
    /// </summary>
    public DateTimeOffset? End { get; }

    /// <summary>
    /// Gets the preset that produced the range.
    /// </summary>
    public DateTimeRangePreset Preset { get; }

    /// <summary>
    /// Gets the user-facing label used by range picker controls.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets a value indicating whether the end instant is inclusive.
    /// </summary>
    public bool IsEndInclusive { get; }

    /// <summary>
    /// Gets the optional maximum allowed duration.
    /// </summary>
    public TimeSpan? MaximumDuration { get; }

    /// <summary>
    /// Gets a value indicating whether both range endpoints are present.
    /// </summary>
    public bool HasValue => Start.HasValue && End.HasValue;

    /// <summary>
    /// Gets a value indicating whether the start instant is after the end instant.
    /// </summary>
    public bool IsReversed { get; }

    /// <summary>
    /// Gets a value indicating whether the configured maximum duration is exceeded.
    /// </summary>
    public bool ExceedsMaximumDuration { get; }

    /// <summary>
    /// Gets the valid range duration, or <see cref="TimeSpan.Zero"/> when the range is incomplete or invalid.
    /// </summary>
    public TimeSpan Duration => HasValue && !IsReversed ? End!.Value - Start!.Value : TimeSpan.Zero;

    /// <summary>
    /// Gets a value indicating whether the range is complete and satisfies validation constraints.
    /// </summary>
    public bool IsValid => HasValue && !IsReversed && !ExceedsMaximumDuration;

    /// <summary>
    /// Gets validation text for invalid or incomplete ranges.
    /// </summary>
    public string? ValidationMessage
    {
        get
        {
            if (!HasValue)
            {
                return "Start and end are required.";
            }

            if (IsReversed)
            {
                return "Start must be before or equal to end.";
            }

            return ExceedsMaximumDuration ? "Range exceeds the maximum allowed duration." : null;
        }
    }

    /// <summary>
    /// Gets compact user-facing range text.
    /// </summary>
    public string DisplayText => IsValid
        ? string.Format(CultureInfo.InvariantCulture, "{0}: {1} - {2}", Label, FormatDateTime(Start!.Value), FormatDateTime(End!.Value))
        : string.Format(CultureInfo.InvariantCulture, "{0}: invalid range", Label);

    /// <summary>
    /// Determines whether the supplied value falls inside this range.
    /// </summary>
    /// <param name="value">The value to test.</param>
    /// <returns><c>true</c> when the value is contained by the range; otherwise, <c>false</c>.</returns>
    public bool Contains(DateTimeOffset value)
    {
        if (!IsValid)
        {
            return false;
        }

        return value >= Start!.Value && (IsEndInclusive ? value <= End!.Value : value < End!.Value);
    }

    private static string FormatDateTime(DateTimeOffset value) => value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

    private static string ResolveLabel(string? label, DateTimeRangePreset preset) => string.IsNullOrWhiteSpace(label) ? ResolvePresetLabel(preset) : label!;

    private static string ResolvePresetLabel(DateTimeRangePreset preset) => preset switch
    {
        DateTimeRangePreset.Today => "Today",
        DateTimeRangePreset.Yesterday => "Yesterday",
        DateTimeRangePreset.LastSevenDays => "Last 7 days",
        DateTimeRangePreset.ThisMonth => "This month",
        _ => "Custom"
    };
}
