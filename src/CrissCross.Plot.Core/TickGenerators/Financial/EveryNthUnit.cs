// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators.Financial;

/// <summary>Generates financial ticks at regular index intervals.</summary>
public class EveryNthUnit : IFinancialTickGenerator
{
    /// <summary>Initializes a new instance of the <see cref="EveryNthUnit"/> class.</summary>
    /// <param name="placeEveryNthUnit">The number of date indices between ticks.</param>
    /// <param name="startIndex">The first date index to use, or -1 to begin at the view boundary.</param>
    public EveryNthUnit(int placeEveryNthUnit, int startIndex)
    {
        PlaceEveryNthUnit = placeEveryNthUnit;
        StartIndex = startIndex;
    }

    /// <summary>Gets the number of date indices between ticks.</summary>
    private int PlaceEveryNthUnit { get; }

    /// <summary>Gets or sets the first date index used when generating ticks.</summary>
    private int StartIndex { get; set; }

    /// <summary>Gets labeled ticks at the configured interval within the visible range.</summary>
    /// <param name="dateTimes">The date values represented by the axis.</param>
    /// <param name="minIndexInView">The first date index in view.</param>
    /// <param name="maxIndexInView">The last date index in view.</param>
    /// <returns>Tick positions paired with their labels.</returns>
    public List<(int Index, string Label)> GetTicks(DateTime[] dateTimes, int minIndexInView, int maxIndexInView)
    {
        List<(int Index, string Label)> ticks = [];

        if (StartIndex == -1)
        {
            StartIndex = minIndexInView;
        }

        while (StartIndex < minIndexInView)
        {
            StartIndex += PlaceEveryNthUnit;
        }

        if ((StartIndex - minIndexInView) > PlaceEveryNthUnit)
        {
            var multiplier = (StartIndex - minIndexInView) / PlaceEveryNthUnit;
            StartIndex -= multiplier * PlaceEveryNthUnit;
        }

        for (int i = StartIndex; i <= maxIndexInView; i += PlaceEveryNthUnit)
        {
            DateTime dt = dateTimes[i];
            var idx = i - PlaceEveryNthUnit;
            if (idx < 0)
            {
                ticks.Add((i, dt.ToString("MMM - MM")));
            }
            else if (dt.Day > dateTimes[idx].Day)
            {
                if (dt.Month > dateTimes[idx].Month)
                {
                    ticks.Add((i, dt.ToString("MMM - MM")));
                }
                else
                {
                    ticks.Add((i, dt.ToString("ddd - dd")));
                }
            }
            else
            {
                ticks.Add((i, dt.ToString("HH:mm:ss")));
            }
        }

        return ticks;
    }
}
