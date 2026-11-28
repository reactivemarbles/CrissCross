// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators.Financial;

/// <summary>Generates financial ticks for month boundaries and selected Mondays.</summary>
public class MonthsAndMondays : IFinancialTickGenerator
{
    /// <summary>The first day in the middle-of-month Monday range.</summary>
    private const int MiddleOfMonthMondayStartDay = 14;

    /// <summary>The last day in the middle-of-month Monday range.</summary>
    private const int MiddleOfMonthMondayEndDay = 27;

    /// <summary>Gets labeled ticks for month boundaries and selected Mondays in view.</summary>
    /// <param name="dateTimes">The date values represented by the axis.</param>
    /// <param name="minIndexInView">The first date index in view.</param>
    /// <param name="maxIndexInView">The last date index in view.</param>
    /// <returns>Tick positions paired with their labels.</returns>
    public List<(int Index, string Label)> GetTicks(DateTime[] dateTimes, int minIndexInView, int maxIndexInView)
    {
        List<(int Index, string Label)> ticks = [];

        int lastMonth = dateTimes[0].Month;
        for (int i = minIndexInView; i <= maxIndexInView; i++)
        {
            DateTime dt = dateTimes[i];
            if (dt.Month != lastMonth)
            {
                ticks.Add((i, dt.ToString("MMM")));
                lastMonth = dt.Month;
            }
            else if (dt.DayOfWeek == DayOfWeek.Monday
                && dt.Day > MiddleOfMonthMondayStartDay
                && dt.Day < MiddleOfMonthMondayEndDay)
            {
                ticks.Add((i, dt.Day.ToString()));
            }
        }

        return ticks;
    }
}
