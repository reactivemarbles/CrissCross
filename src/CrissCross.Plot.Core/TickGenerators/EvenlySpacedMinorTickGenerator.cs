// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators;

/// <summary>Generates evenly spaced minor ticks between major ticks.</summary>
/// <param name="minorTickPerMajorTick">The number of minor tick intervals per major tick interval.</param>
public class EvenlySpacedMinorTickGenerator(double minorTickPerMajorTick) : IMinorTickGenerator
{
    /// <summary>Gets or sets the number of minor tick intervals per major tick interval.</summary>
    public double MinorTicksPerMajorTick { get; set; } = minorTickPerMajorTick;

    /// <summary>Gets minor tick positions within the visible range.</summary>
    /// <param name="majorTicks">The positions of the major ticks.</param>
    /// <param name="visibleRange">The visible coordinate range.</param>
    /// <returns>The positions of minor ticks in the visible range.</returns>
    public IEnumerable<double> GetMinorTicks(double[] majorTicks, CoordinateRange visibleRange)
    {
        if (majorTicks is null || majorTicks.Length < 2)
        {
            return [];
        }

        double majorTickSpacing = majorTicks[1] - majorTicks[0];
        double minorTickSpacing = majorTickSpacing / MinorTicksPerMajorTick;

        List<double> majorTicksWithPadding = [majorTicks[0] - majorTickSpacing, .. majorTicks];

        List<double> minorTicks = [];
        foreach (var majorTickPosition in majorTicksWithPadding)
        {
            for (int i = 1; i < MinorTicksPerMajorTick; i++)
            {
                double minorTickPosition = majorTickPosition + (minorTickSpacing * i);
                if (visibleRange.Contains(minorTickPosition))
                {
                    minorTicks.Add(minorTickPosition);
                }
            }
        }

        return minorTicks;
    }
}
