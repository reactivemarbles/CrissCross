// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators.TimeUnits;

/// <summary>Represents a day time unit.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST2016",
    Justification = "This implementation must retain DateTime signatures defined by the public ITimeUnit API.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST2309",
    Justification = "The default increment is part of the public ITimeUnit API contract.")]
public class Day : ITimeUnit
{
    /// <inheritdoc/>
    public IReadOnlyList<int> Divisors => StandardDivisors.Days;

    /// <inheritdoc/>
    public TimeSpan MinSize => TimeSpan.FromDays(1);

    /// <inheritdoc/>
    public DateTime Snap(DateTime dateTime) => new(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Unspecified);

    /// <inheritdoc/>
    public string GetDateTimeFormatString() => "d";

    /// <inheritdoc/>
    public DateTime Next(DateTime dateTime, int increment = 1) => dateTime.AddDays(increment);
}
