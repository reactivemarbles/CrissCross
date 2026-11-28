// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators;

/// <summary>Generates date-time ticks using automatically selected time units.</summary>
public class DateTimeAutomatic : IDateTimeTickGenerator
{
    /// <summary>The supported time units in increasing order of duration.</summary>
    private static readonly List<ITimeUnit> TheseTimeUnits =
    [
        new TimeUnits.Millisecond(),
        new TimeUnits.Centisecond(),
        new TimeUnits.Decisecond(),
        new TimeUnits.Second(),
        new TimeUnits.Minute(),
        new TimeUnits.Hour(),
        new TimeUnits.Day(),
        new TimeUnits.Month(),
        new TimeUnits.Year(),
    ];

    /// <summary>Gets or sets if assigned, this function will be used to create tick labels.</summary>
    public Func<DateTime, string>? LabelFormatter { get; set; }

    /// <summary>Gets the time unit currently used to generate ticks.</summary>
    public ITimeUnit? TimeUnit { get; private set; }

    /// <summary>Gets or sets the generated ticks.</summary>
    public Tick[] Ticks { get; set; } = [];

    /// <summary>Gets or sets the maximum number of ticks to generate.</summary>
    public int MaxTickCount { get; set; } = 10_000;

    /// <summary>Converts dates to coordinate-space values.</summary>
    /// <param name="dates">The dates to convert.</param>
    /// <returns>The coordinate-space values corresponding to the dates.</returns>
    public IEnumerable<double> ConvertToCoordinateSpace(IEnumerable<DateTime> dates)
    {
        foreach (DateTime date in dates)
        {
            yield return NumericConversion.ToNumber(date);
        }
    }

    /// <summary>Regenerates ticks for the specified coordinate range and axis presentation.</summary>
    /// <param name="range">The visible coordinate range.</param>
    /// <param name="edge">The axis edge.</param>
    /// <param name="size">The axis size.</param>
    /// <param name="paint">The paint settings.</param>
    /// <param name="labelStyle">The tick-label style.</param>
    public void Regenerate(CoordinateRange range, Edge edge, PixelLength size, Paint paint, LabelStyle labelStyle)
    {
        const int MinimumTickLabelWidth = 16;
        const int TickLabelHeight = 12;
        const int TickLabelPadding = 10;

        if (range.Length >= TimeSpan.MaxValue.Days || double.IsNaN(range.Length) || double.IsInfinity(range.Length) || (size.Length <= 0))
        {
            // cases of extreme zoom (10,000 years)
            Ticks = [];
            return;
        }

        TimeSpan span = TimeSpan.FromDays(range.Length);
        ITimeUnit? timeUnit = GetAppropriateTimeUnit(span);

        // estimate the size of the largest tick label for this unit this unit
        int maxExpectedTickLabelWidth = (int)Math.Max(MinimumTickLabelWidth, span.TotalDays / MaxTickCount);
        PixelSize tickLabelBounds = new(maxExpectedTickLabelWidth, TickLabelHeight);
        double coordinatesPerPixel = range.Length / size.Length;

        while (true)
        {
            // determine the ideal spacing to use between ticks
            double increment = coordinatesPerPixel * tickLabelBounds.Width / timeUnit.MinSize.TotalDays;
            int? niceIncrement = LeastMemberGreaterThan(increment, timeUnit.Divisors);
            if (niceIncrement is null)
            {
                timeUnit = TheseTimeUnits.Find(t => t.MinSize > timeUnit.MinSize);
                if (timeUnit is not null)
                {
                    continue;
                }

                timeUnit = TheseTimeUnits[TheseTimeUnits.Count - 1];
                niceIncrement = (int)Math.Ceiling(increment);
            }

            TimeUnit = timeUnit;

            // attempt to generate the ticks given these conditions
            (List<Tick>? ticks, PixelSize? largestTickLabelSize) = GenerateTicks(range, timeUnit, niceIncrement.Value, tickLabelBounds, paint, labelStyle);

            // if ticks were returned, use them
            if (ticks is not null)
            {
                Ticks = [.. ticks];
                return;
            }

            // if no ticks were returned it means the conditions were too dense and tick labels
            // overlapped, so expand the tick label bounds and try again.
            if (largestTickLabelSize is not null)
            {
                tickLabelBounds = tickLabelBounds.Max(largestTickLabelSize.Value);
                tickLabelBounds = new(tickLabelBounds.Width + TickLabelPadding, tickLabelBounds.Height + TickLabelPadding);
                continue;
            }

            throw new InvalidOperationException($"{nameof(ticks)} and {nameof(largestTickLabelSize)} are both null");
        }
    }

    /// <summary>Selects a time unit appropriate for the specified span.</summary>
    /// <param name="timeSpan">The coordinate range duration.</param>
    /// <param name="targetTickCount">The target number of ticks.</param>
    /// <returns>The selected time unit.</returns>
    private static ITimeUnit GetAppropriateTimeUnit(TimeSpan timeSpan, int targetTickCount = 10)
    {
        const int TickCountTolerance = 3;

        foreach (var timeUnit in TheseTimeUnits)
        {
            long estimatedUnitTicks = timeSpan.Ticks / timeUnit.MinSize.Ticks;
            foreach (var increment in timeUnit.Divisors)
            {
                long estimatedTicks = estimatedUnitTicks / increment;
                if (estimatedTicks > targetTickCount / TickCountTolerance && estimatedTicks < targetTickCount * TickCountTolerance)
                {
                    return timeUnit;
                }
            }
        }

        return TheseTimeUnits[TheseTimeUnits.Count - 1];
    }

    /// <summary>Finds the first list member greater than the specified value.</summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="list">The ordered candidate values.</param>
    /// <returns>The first greater member, or <see langword="null"/> when none exists.</returns>
    private static int? LeastMemberGreaterThan(double value, IReadOnlyList<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] > value)
            {
                return list[i];
            }
        }

        return null;
    }

    /// <summary>Gets the next larger supported unit after the specified unit.</summary>
    /// <param name="timeUnit">The current time unit.</param>
    /// <returns>The next larger supported time unit.</returns>
    private ITimeUnit GetLargerTimeUnit(ITimeUnit timeUnit)
    {
        for (int i = 0; i < TheseTimeUnits.Count - 1; i++)
        {
            if (timeUnit.GetType() == TheseTimeUnits[i].GetType())
            {
                return TheseTimeUnits[i + 1];
            }
        }

        return TheseTimeUnits[TheseTimeUnits.Count - 1];
    }

    /// <summary>Attempts to find ticks that fit within the supplied label bounds.</summary>
    /// <param name="range">The coordinate range.</param>
    /// <param name="unit">The selected time unit.</param>
    /// <param name="increment">The increment between ticks.</param>
    /// <param name="tickLabelBounds">The maximum label size.</param>
    /// <param name="paint">The paint settings used for measuring labels.</param>
    /// <param name="labelStyle">The tick-label style.</param>
    /// <returns>The ticks and the largest label size if the labels do not fit.</returns>
    private (List<Tick>? Positions, PixelSize? PixelSize) GenerateTicks(in CoordinateRange range, ITimeUnit unit, int increment, PixelSize tickLabelBounds, Paint paint, LabelStyle labelStyle)
    {
        DateTime rangeMin = NumericConversion.ToDateTime(range.Min);
        DateTime rangeMax = NumericConversion.ToDateTime(range.Max);

        // range.Min could be anything, but when calculating start it must be "snapped" to the best tick
        DateTime start = GetLargerTimeUnit(unit).Snap(rangeMin);

        start = unit.Next(start, -increment);

        List<Tick> ticks = [];

        // if the increment is 0 or negative, something has gone wrong further up but bail out now.
        // Also check that unit.Next is actually going to move us forward... otherwise we could loop forever
        if (increment <= 0 || (unit.Next(start, increment) <= start))
        {
            return (ticks, null);
        }

        const int maxTickCount = 1000;
        for (DateTime dt = start; dt <= rangeMax; dt = unit.Next(dt, increment))
        {
            // this test is dangerous because it can cause an infinite loop if dt is not
            // advancing.  It might be better to initialize dt to Max(start,rangeMin) but I'm guessing the#
            // intentions of having this here is to allow the "pre-roll" to advance so that the first
            // tick is beyond rangeMin ?
            if (dt < rangeMin)
            {
                continue;
            }

            string tickLabel = LabelFormatter is null
                ? dt.ToString(unit.GetDateTimeFormatString())
                : LabelFormatter(dt);

            PixelSize tickLabelSize = labelStyle.Measure(tickLabel, paint).Size;

            bool tickLabelIsTooLarge = !tickLabelBounds.Contains(tickLabelSize);
            if (tickLabelIsTooLarge)
            {
                return (null, tickLabelSize);
            }

            double tickPosition = NumericConversion.ToNumber(dt);
            Tick tick = new(tickPosition, tickLabel, isMajor: true);
            ticks.Add(tick);

            // this prevents infinite loops with weird axis limits or small delta (e.g., DateTime)
            if (ticks.Count >= maxTickCount)
            {
                break;
            }
        }

        return (ticks, null);
    }
}
