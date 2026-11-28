// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators.TimeUnits;

/// <summary>Represents a year time unit.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST2016",
    Justification = "This implementation must retain DateTime signatures defined by the public ITimeUnit API.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST2309",
    Justification = "The default increment is part of the public ITimeUnit API contract.")]
public class Year : ITimeUnit
{
    /// <inheritdoc/>
    public IReadOnlyList<int> Divisors => StandardDivisors.Years;

    /// <inheritdoc/>
    public TimeSpan MinSize => TimeSpan.FromDays(TimeUnitConstants.ApproximateDaysPerYear);

    /// <inheritdoc/>
    public DateTime Snap(DateTime dateTime) => new(dateTime.Year, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

    /// <inheritdoc/>
    public string GetDateTimeFormatString() => "yyyy";

    /// <inheritdoc/>
    public DateTime Next(DateTime dateTime, int increment = 1)
    {
        int newYear = dateTime.Year + increment;

        if (newYear <= TimeUnitConstants.MinimumSupportedYear)
        {
            return new(TimeUnitConstants.MinimumSupportedYear, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        }

        return newYear > TimeUnitConstants.FirstUnsupportedYear
            ? new DateTime(TimeUnitConstants.MaximumSupportedYear, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
            : dateTime.AddYears(increment);
    }
}
