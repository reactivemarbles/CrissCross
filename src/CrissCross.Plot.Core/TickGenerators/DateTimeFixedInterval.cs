// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.TickGenerators.TimeUnits;

namespace CrissCross.Plot.TickGenerators;

/// <summary>Generates date and time ticks at fixed intervals.</summary>
/// <param name="interval">The time unit used for major ticks.</param>
/// <param name="intervalsPerTick">The number of time units between major ticks.</param>
/// <param name="minorInterval">The time unit used for minor ticks, or <see langword="null"/> to disable minor ticks.</param>
/// <param name="minorIntervalsPerTick">The number of minor time units between minor ticks.</param>
/// <param name="getIntervalStartFunc">An optional function that chooses the first major tick from the axis start.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Style",
    "SST2309",
    Justification = "These optional arguments are part of the established public constructor API; removing their defaults would break existing source callers.")]
public class DateTimeFixedInterval(
    ITimeUnit interval,
    int intervalsPerTick = 1,
    ITimeUnit? minorInterval = null,
    int minorIntervalsPerTick = 1,
    Func<DateTime, DateTime>? getIntervalStartFunc = null) : IDateTimeTickGenerator
{
    /// <summary>Gets or sets the time unit used for major ticks.</summary>
    public ITimeUnit Interval { get; set; } = interval;

    /// <summary>Gets or sets the number of time units between major ticks.</summary>
    public int IntervalsPerTick { get; set; } = intervalsPerTick;

    /// <summary>Gets or sets the time unit used for minor ticks, or <see langword="null"/> when they are disabled.</summary>
    public ITimeUnit? MinorInterval { get; set; } = minorInterval;

    /// <summary>Gets or sets the number of minor time units between minor ticks.</summary>
    public int MinorIntervalsPerTick { get; set; } = minorIntervalsPerTick;

    /// <summary>
    /// Gets or sets an optional function to override where the intervals for ticks start. The DateTime argument provided is
    /// the start range of the axis (i.e. <see cref="IAxis.Min"/>).
    /// </summary>
    /// <remarks>
    /// If omitted, the ticks will start from <see cref="IAxis.Min"/>. This may have undesirable effects when zooming
    /// and panning. If provided, the ticks will start from the returned DateTime.
    /// </remarks>
    /// <example>
    /// If the plot contains weekly data and ticks should appear on the first day of each month, provide:
    /// <c>dt =&gt; new DateTime(dt.Year, dt.Month, 1);</c>
    /// If the plot contains hourly data and ticks should appear every six hours, set <see cref="Interval"/> to
    /// <see cref="Hour"/>, set <see cref="IntervalsPerTick"/> to 6, and provide:
    /// <c>dt =&gt; new DateTime(dt.Year, dt.Month, dt.Day);</c>
    /// </example>
    public Func<DateTime, DateTime>? GetIntervalStartFunc { get; set; } = getIntervalStartFunc;

    /// <summary>Gets or sets the function used to create tick labels, or <see langword="null"/> to use the interval format.</summary>
    public Func<DateTime, string>? LabelFormatter { get; set; }

    /// <summary>Gets or sets the generated ticks.</summary>
    public Tick[] Ticks { get; set; } = Array.Empty<Tick>();

    /// <summary>Gets or sets the maximum number of ticks to generate.</summary>
    public int MaxTickCount { get; set; } = 10_000;

    /// <summary>Converts date values to plot coordinate values.</summary>
    /// <param name="dates">The dates to convert.</param>
    /// <returns>The corresponding coordinate values.</returns>
    public IEnumerable<double> ConvertToCoordinateSpace(IEnumerable<DateTime> dates)
    {
        foreach (DateTime date in dates)
        {
            yield return NumericConversion.ToNumber(date);
        }
    }

    /// <summary>Regenerates ticks that fall within the specified axis range.</summary>
    /// <param name="range">The visible coordinate range.</param>
    /// <param name="edge">The edge on which ticks are drawn.</param>
    /// <param name="size">The pixel size of the axis.</param>
    /// <param name="paint">The paint used to measure labels.</param>
    /// <param name="labelStyle">The style used to measure tick labels.</param>
    public void Regenerate(CoordinateRange range, Edge edge, PixelLength size, Paint paint, LabelStyle labelStyle)
    {
        List<Tick> ticks = new();
        HashSet<DateTime> timesWithTicks = new(); // Avoid having minor and major ticks at same time

        // Modify the start time of the ticks if a delegate was provided
        DateTime rangeMin = NumericConversion.ToDateTime(range.Min);
        DateTime start = GetIntervalStartFunc?.Invoke(rangeMin) ?? Interval.Next(rangeMin, -1 * IntervalsPerTick);

        DateTime end = Interval.Next(NumericConversion.ToDateTime(range.Max));
        for (DateTime dt = start; dt <= end; dt = Interval.Next(dt, IntervalsPerTick))
        {
            string tickLabel = LabelFormatter is null
                ? dt.ToString(Interval.GetDateTimeFormatString())
                : LabelFormatter(dt);

            ticks.Add(new(NumericConversion.ToNumber(dt), tickLabel, true));
            _ = timesWithTicks.Add(dt);
        }

        if (MinorInterval is not null)
        {
            for (DateTime dt = start; dt <= end; dt = MinorInterval.Next(dt, MinorIntervalsPerTick))
            {
                if (timesWithTicks.Contains(dt))
                {
                    continue;
                }

                ticks.Add(new(NumericConversion.ToNumber(dt), dt.ToString(Interval.GetDateTimeFormatString()), false));
            }
        }

        List<Tick> filteredTicks = [with(capacity: ticks.Count)];
        foreach (Tick tick in ticks)
        {
            if (range.Contains(tick.Position))
            {
                filteredTicks.Add(tick);
            }
        }

        Ticks = filteredTicks.ToArray();
    }
}
