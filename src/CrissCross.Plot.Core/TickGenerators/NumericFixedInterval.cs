// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators;

/// <summary>Generates numeric ticks using a fixed interval.</summary>
/// <param name="interval">The distance between adjacent ticks.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional interval default is retained for source and binary compatibility.")]
public class NumericFixedInterval(double interval = 1) : ITickGenerator
{
    /// <summary>Gets or sets the distance between adjacent ticks.</summary>
    public double Interval { get; set; } = interval;

    /// <summary>Gets or sets the formatter used for tick labels.</summary>
    public Func<double, string> LabelFormatter { get; set; } = LabelFormatters.Numeric;

    /// <summary>Gets or sets the generated ticks.</summary>
    public Tick[] Ticks { get; set; } = [];

    /// <summary>Gets or sets the maximum number of ticks to generate.</summary>
    public int MaxTickCount { get; set; } = 10_000;

    /// <summary>Regenerates ticks for the specified range and axis presentation.</summary>
    /// <param name="range">The visible coordinate range.</param>
    /// <param name="edge">The axis edge.</param>
    /// <param name="size">The axis size.</param>
    /// <param name="paint">The paint settings.</param>
    /// <param name="labelStyle">The tick-label style.</param>
    public void Regenerate(CoordinateRange range, Edge edge, PixelLength size, Paint paint, LabelStyle labelStyle)
    {
        List<Tick> ticks = [];

        double lowest = range.Min - (range.Min % Interval) - Interval;
        double highest = range.Max - (range.Max % Interval) + Interval;
        int tickCount = (int)((highest - lowest) / Interval);
        tickCount = Math.Min(tickCount, MaxTickCount);

        for (int i = 0; i < tickCount; i++)
        {
            double position = range.IsInverted
                ? lowest + (i * Interval)
                : highest - (i * Interval);

            string label = LabelFormatter(position);
            Tick tick = new(position, label, true);
            if (range.Contains(tick.Position))
            {
                ticks.Add(tick);
            }
        }

        Ticks = [.. ticks];
    }
}
