// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators;

/// <summary>Calculates decimal tick positions for an axis.</summary>
public class DecimalTickSpacingCalculator
{
    /// <summary>Gets or sets the maximum number of ticks to calculate.</summary>
    public int MaximumTickCount { get; set; } = 1000;

    /// <summary>Generates tick positions that fit the range and available label space.</summary>
    /// <param name="range">The coordinate range.</param>
    /// <param name="axisLength">The length of the axis in pixels.</param>
    /// <param name="maxLabelLength">The maximum label length in pixels.</param>
    /// <returns>The generated major tick positions.</returns>
    public double[] GenerateTickPositions(CoordinateRange range, PixelLength axisLength, PixelLength maxLabelLength)
    {
        const int CandidateTickAdjustment = 2;

        double tickSpacing = GetIdealTickSpacing(range, axisLength, maxLabelLength);

        double firstTickOffset = range.Min % tickSpacing;
        int tickCount = (int)(range.Length / tickSpacing) + CandidateTickAdjustment;
        tickCount = Math.Min(MaximumTickCount, tickCount);
        tickCount = Math.Max(1, tickCount);

        List<double> majorTickPositions = [with(capacity: tickCount)];
        for (int i = 0; i < tickCount; i++)
        {
            double position = range.Min - firstTickOffset + (tickSpacing * i);
            if (range.Contains(position))
            {
                majorTickPositions.Add(position);
            }
        }

        if (majorTickPositions.Count < 2)
        {
            double tickBelow = range.Min - firstTickOffset;
            double firstTick = majorTickPositions.Count > 0 ? majorTickPositions[0] : tickBelow;
            double nextTick = firstTick + tickSpacing;
            return [firstTick, nextTick];
        }

        return [.. majorTickPositions];
    }

    /// <summary>Calculates the spacing between major ticks.</summary>
    /// <param name="range">The coordinate range.</param>
    /// <param name="axisLength">The length of the axis in pixels.</param>
    /// <param name="maxLabelLength">The maximum label length in pixels.</param>
    /// <returns>The selected tick spacing.</returns>
    private double GetIdealTickSpacing(in CoordinateRange range, PixelLength axisLength, PixelLength maxLabelLength)
    {
        const int Radix = 10;
        const double FirstDecimalDivisor = 2;
        const double SecondDecimalDivisor = 2;
        const double FractionalDecimalDivisor = 2.5;
        const double SmallLabelThreshold = 10;
        const double SmallLabelSpacingScale = 2;
        const double MediumLabelThreshold = 25;
        const double MediumLabelSpacingScale = 1.5;
        const double LargeLabelSpacingScale = 1.2;

        int targetTickCount = (int)(axisLength.Length / maxLabelLength.Length) + 1;

        int exponent = (int)Math.Log(range.Length, Radix) + 1;
        double initialSpace = Math.Pow(Radix, exponent);
        List<double> tickSpacings = [initialSpace];
        double[] divisors = [FirstDecimalDivisor, SecondDecimalDivisor, FractionalDecimalDivisor];

        // Generate possible tick spacings.
        while (tickSpacings.Count < MaximumTickCount)
        {
            double divisor = divisors[tickSpacings.Count % divisors.Length];
            double smallerSpacing = tickSpacings[tickSpacings.Count - 1] / divisor;
            tickSpacings.Add(smallerSpacing);
            int tickCount = (int)(range.Length / tickSpacings[tickSpacings.Count - 1]);
            if (tickCount > targetTickCount)
            {
                break;
            }
        }

        // Choose the densest tick spacing that is still suitable.
        for (int i = 0; i < tickSpacings.Count; i++)
        {
            double thisTickSpacing = tickSpacings[tickSpacings.Count - 1 - i];
            double thisTickCount = range.Length / thisTickSpacing;
            double spacePerTick = axisLength.Length / thisTickCount;
            double neededSpacePerTick = maxLabelLength.Length;

            if (neededSpacePerTick < SmallLabelThreshold)
            {
                neededSpacePerTick *= SmallLabelSpacingScale;
            }
            else if (neededSpacePerTick < MediumLabelThreshold)
            {
                neededSpacePerTick *= MediumLabelSpacingScale;
            }
            else
            {
                neededSpacePerTick *= LargeLabelSpacingScale;
            }

            if (spacePerTick > neededSpacePerTick)
            {
                return thisTickSpacing;
            }
        }

        return tickSpacings[0];
    }
}
