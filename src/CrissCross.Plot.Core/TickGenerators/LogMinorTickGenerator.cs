// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.TickGenerators;

/// <summary>Generates logarithmically distributed minor ticks between major ticks.</summary>
public class LogMinorTickGenerator : IMinorTickGenerator
{
    /// <summary>The base used to distribute minor ticks within each major interval.</summary>
    private const int DecimalBase = 10;

    /// <summary>Gets or sets the number of divisions between adjacent major ticks.</summary>
    public int Divisions { get; set; } = 5;

    /// <summary>Gets the minor tick positions between the supplied major ticks.</summary>
    /// <param name="majorTicks">The major tick positions.</param>
    /// <param name="visibleRange">The visible coordinate range.</param>
    /// <returns>The generated minor tick positions.</returns>
    public IEnumerable<double> GetMinorTicks(double[] majorTicks, CoordinateRange visibleRange)
    {
        if (majorTicks is null || majorTicks.Length < 2)
        {
            return [];
        }

        // determine range of major ticks to iterate (assume even spacing of all ticks)
        double deltaMajor = majorTicks[1] - majorTicks[0];
        double lowestMajor = majorTicks[0] - deltaMajor;
        double highestMajor = majorTicks[^1] + deltaMajor;

        // Precalculate the log-distributed offset positions between major ticks.
        int offsetCount = Divisions - 1;
        if (offsetCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(Divisions));
        }

        List<double> minorTickOffsets = [];
        for (int division = 1; division <= offsetCount; division++)
        {
            minorTickOffsets.Add(deltaMajor * Math.Log10((double)(division * DecimalBase) / Divisions));
        }

        // Iterate major ticks and collect minor ticks with offsets.
        List<double> minorTicks = [];
        for (double major = lowestMajor; major <= highestMajor; major += deltaMajor)
        {
            foreach (double offset in minorTickOffsets)
            {
                minorTicks.Add(major + offset);
            }
        }

        return minorTicks;
    }
}
