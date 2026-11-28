// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators;

/// <summary>Defines operations for a unit used to generate date-time ticks.</summary>
public interface ITimeUnit
{
    /// <summary>Gets divisors that subdivide this time unit into suitable tick intervals.</summary>
    IReadOnlyList<int> Divisors { get; }

    /// <summary>Gets the minimum span represented by this time unit.</summary>
    TimeSpan MinSize { get; }

    /// <summary>Gets the format string used to display labels for this time unit.</summary>
    /// <returns>A standard or custom date-time format string.</returns>
    string GetDateTimeFormatString();

    /// <summary>Gets a date-time value advanced by the specified number of this unit.</summary>
    /// <param name="dateTime">The date-time value to advance.</param>
    /// <param name="increment">The number of units to advance.</param>
    /// <returns>The advanced date-time value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2016",
        Justification = "This API performs calendar tick calculations without timezone conversion; changing DateTime to DateTimeOffset would break implementers.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The default increment is retained for source and binary compatibility.")]
    DateTime Next(DateTime dateTime, int increment = 1);

    /// <summary>Snaps the specified date-time value to a suitable tick position.</summary>
    /// <param name="dateTime">The date-time value to snap.</param>
    /// <returns>The snapped date-time value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2016",
        Justification = "This API performs calendar tick calculations without timezone conversion; changing DateTime to DateTimeOffset would break implementers.")]
    DateTime Snap(DateTime dateTime);
}
