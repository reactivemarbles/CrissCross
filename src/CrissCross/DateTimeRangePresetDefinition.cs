// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace CrissCross;

/// <summary>
/// Describes a reusable date/time range preset that can create a range from a deterministic reference instant.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DateTimeRangePresetDefinition"/> class.
/// </remarks>
/// <param name="preset">The preset identifier.</param>
/// <param name="displayName">The user-facing preset name.</param>
public sealed class DateTimeRangePresetDefinition(DateTimeRangePreset preset, string displayName)
{
    /// <summary>
    /// Gets the today preset definition.
    /// </summary>
    public static DateTimeRangePresetDefinition Today { get; } = new(DateTimeRangePreset.Today, "Today");

    /// <summary>
    /// Gets the yesterday preset definition.
    /// </summary>
    public static DateTimeRangePresetDefinition Yesterday { get; } = new(DateTimeRangePreset.Yesterday, "Yesterday");

    /// <summary>
    /// Gets the trailing seven-day preset definition.
    /// </summary>
    public static DateTimeRangePresetDefinition LastSevenDays { get; } = new(DateTimeRangePreset.LastSevenDays, "Last 7 days");

    /// <summary>
    /// Gets the current-month preset definition.
    /// </summary>
    public static DateTimeRangePresetDefinition ThisMonth { get; } = new(DateTimeRangePreset.ThisMonth, "This month");

    /// <summary>
    /// Gets the custom preset definition.
    /// </summary>
    public static DateTimeRangePresetDefinition Custom { get; } = new(DateTimeRangePreset.Custom, "Custom");

    /// <summary>
    /// Gets the preset identifier.
    /// </summary>
    public DateTimeRangePreset Preset { get; } = preset;

    /// <summary>
    /// Gets the user-facing preset name.
    /// </summary>
    public string DisplayName { get; } = displayName;

    /// <summary>
    /// Creates a concrete range from the specified reference instant.
    /// </summary>
    /// <param name="referenceTime">The reference instant used to resolve relative presets.</param>
    /// <returns>The resolved date/time range.</returns>
    public DateTimeRange CreateRange(DateTimeOffset referenceTime)
    {
        var start = ResolveStart(referenceTime);
        var end = ResolveEnd(referenceTime);
        return new DateTimeRange(start, end, Preset, DisplayName);
    }

    private DateTimeOffset? ResolveStart(DateTimeOffset referenceTime) => Preset switch
    {
        DateTimeRangePreset.Today => new DateTimeOffset(referenceTime.Year, referenceTime.Month, referenceTime.Day, 0, 0, 0, referenceTime.Offset),
        DateTimeRangePreset.Yesterday => new DateTimeOffset(referenceTime.Year, referenceTime.Month, referenceTime.Day, 0, 0, 0, referenceTime.Offset).AddDays(-1),
        DateTimeRangePreset.LastSevenDays => referenceTime.AddDays(-7),
        DateTimeRangePreset.ThisMonth => new DateTimeOffset(referenceTime.Year, referenceTime.Month, 1, 0, 0, 0, referenceTime.Offset),
        _ => null
    };

    private DateTimeOffset? ResolveEnd(DateTimeOffset referenceTime) => Preset switch
    {
        DateTimeRangePreset.Yesterday => new DateTimeOffset(referenceTime.Year, referenceTime.Month, referenceTime.Day, 0, 0, 0, referenceTime.Offset).AddTicks(-1),
        DateTimeRangePreset.Custom => null,
        _ => referenceTime
    };
}
