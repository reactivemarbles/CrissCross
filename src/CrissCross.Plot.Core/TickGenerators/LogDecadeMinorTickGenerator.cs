// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators;

/// <summary>Generates logarithmically spaced minor ticks within each visible decade.</summary>
public class LogDecadeMinorTickGenerator : IMinorTickGenerator
{
    /// <summary>The base used to distribute ticks within each decade.</summary>
    private const int DecimalBase = 10;

    /// <summary>Gets or sets the number of tick positions in each decade.</summary>
    public int TicksPerDecade { get; set; } = DecimalBase;

    /// <summary>Gets minor tick positions within the visible range.</summary>
    /// <param name="majorTicks">The major tick positions.</param>
    /// <param name="visibleRange">The visible coordinate range.</param>
    /// <returns>The generated minor tick positions.</returns>
    public IEnumerable<double> GetMinorTicks(double[] majorTicks, CoordinateRange visibleRange)
    {
        var minDecadeTick = Math.Floor(visibleRange.Min);
        var maxDecadeTick = Math.Ceiling(visibleRange.Max);

        if (minDecadeTick >= maxDecadeTick)
        {
            return [];
        }

        int offsetCount = TicksPerDecade - 1;
        if (offsetCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(TicksPerDecade));
        }

        // Precalculate the log-distributed offset positions between decades.
        List<double> minorTickOffsets = [];
        for (int tick = 1; tick <= offsetCount; tick++)
        {
            minorTickOffsets.Add(Math.Log10((double)(tick * DecimalBase) / TicksPerDecade));
        }

        // Iterate decades and collect minor ticks with offsets.
        List<double> minorTicks = [];
        for (double major = minDecadeTick; major <= maxDecadeTick; ++major)
        {
            foreach (double offset in minorTickOffsets)
            {
                minorTicks.Add(major + offset);
            }
        }

        return minorTicks;
    }
}
