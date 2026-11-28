// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators.Financial;

/// <summary>Generates financial axis ticks at year boundaries.</summary>
public class Years : IFinancialTickGenerator
{
    /// <summary>Gets labeled ticks for the year boundaries in view.</summary>
    /// <param name="dateTimes">The date values represented by the axis.</param>
    /// <param name="minIndexInView">The first date index in view.</param>
    /// <param name="maxIndexInView">The last date index in view.</param>
    /// <returns>Tick positions paired with their labels.</returns>
    public List<(int Index, string Label)> GetTicks(DateTime[] dateTimes, int minIndexInView, int maxIndexInView)
    {
        List<(int Index, string Label)> ticks = [];

        int lastYear = dateTimes[0].Year;
        for (int i = minIndexInView; i <= maxIndexInView; i++)
        {
            DateTime dt = dateTimes[i];
            if (dt.Year != lastYear)
            {
                ticks.Add((i, dt.Year.ToString()));
                lastYear = dt.Year;
            }
        }

        return ticks;
    }
}
