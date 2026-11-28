// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.Statistics;

namespace CrissCross.Plot.Finance;

/// <summary>Calculates a simple moving average and corresponding date arrays.</summary>
public class SimpleMovingAverage
{
    /// <summary>Gets the moving-average values.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "The public field is retained for existing binary API compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2322", Justification = "Callers rely on the existing mutable array API.")]
    public readonly double[] Means;

    /// <summary>Gets the timestamps corresponding to the moving-average values.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "The public field is retained for existing binary API compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2322", Justification = "Callers rely on the existing mutable array API.")]
    public readonly DateTime[] DateTimes;

    /// <summary>Gets the OLE Automation dates corresponding to the moving-average values.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "The public field is retained for existing binary API compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2322", Justification = "Callers rely on the existing mutable array API.")]
    public readonly double[] Dates;

    /// <summary>Initializes a new instance of the <see cref="SimpleMovingAverage"/> class.</summary>
    /// <param name="ohlcs">The OHLC data.</param>
    /// <param name="n">The moving-average window size.</param>
    public SimpleMovingAverage(IReadOnlyList<OHLC> ohlcs, int n)
    {
        double[] prices = new double[ohlcs.Count];
        for (int i = 0; i < ohlcs.Count; i++)
        {
            prices[i] = ohlcs[i].Close;
        }

        Means = Series.MovingAverage(prices, n);
        int skippedCount = Math.Max(0, n);
        DateTimes = new DateTime[Math.Max(0, ohlcs.Count - skippedCount)];
        Dates = new double[DateTimes.Length];
        for (int i = 0; i < DateTimes.Length; i++)
        {
            DateTimes[i] = ohlcs[i + skippedCount].DateTime;
            Dates[i] = DateTimes[i].ToOADate();
        }
    }
}
