// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.Statistics;

namespace CrissCross.Plot.Finance;

/// <summary>Calculates Bollinger Bands and their corresponding dates for OHLC data.</summary>
public class BollingerBands
{
    /// <summary>Gets the simple moving average values.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "The public field is retained for existing binary API compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2322", Justification = "Callers rely on the existing mutable array API.")]
    public readonly double[] Means;

    /// <summary>Gets the upper band values.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "The public field is retained for existing binary API compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2322", Justification = "Callers rely on the existing mutable array API.")]
    public readonly double[] UpperValues;

    /// <summary>Gets the lower band values.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "The public field is retained for existing binary API compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2322", Justification = "Callers rely on the existing mutable array API.")]
    public readonly double[] LowerValues;

    /// <summary>Gets the timestamps corresponding to the band values.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "The public field is retained for existing binary API compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2322", Justification = "Callers rely on the existing mutable array API.")]
    public readonly DateTime[] DateTimes;

    /// <summary>Gets the OLE Automation dates corresponding to the band values.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "The public field is retained for existing binary API compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2322", Justification = "Callers rely on the existing mutable array API.")]
    public readonly double[] Dates;

    /// <summary>Initializes a new instance of the <see cref="BollingerBands"/> class.</summary>
    /// <param name="ohlcs">The OHLC data.</param>
    /// <param name="n">The moving-average window size.</param>
    /// <param name="sdCoeff">The standard-deviation multiplier.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional multiplier is retained for existing callers.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "The parameter name is retained for existing named-argument callers.")]
    public BollingerBands(List<OHLC> ohlcs, int n, double sdCoeff = 2)
    {
        double[] prices = new double[ohlcs.Count];
        for (int i = 0; i < ohlcs.Count; i++)
        {
            prices[i] = ohlcs[i].Close;
        }

        double[] simpleMovingAverage = Series.MovingAverage(prices, n, preserveLength: true);
        double[] movingStandardDeviation = Series.SimpleMovingStandardDeviation(prices, n, preserveLength: true);
        int skippedCount = Math.Max(0, n);
        int resultCount = Math.Max(0, prices.Length - skippedCount);
        Means = new double[resultCount];
        UpperValues = new double[resultCount];
        LowerValues = new double[resultCount];
        DateTimes = new DateTime[resultCount];
        Dates = new double[resultCount];

        for (int i = 0; i < resultCount; i++)
        {
            int sourceIndex = i + skippedCount;
            Means[i] = simpleMovingAverage[sourceIndex];
            LowerValues[i] = simpleMovingAverage[sourceIndex] - (sdCoeff * movingStandardDeviation[sourceIndex]);
            UpperValues[i] = simpleMovingAverage[sourceIndex] + (sdCoeff * movingStandardDeviation[sourceIndex]);
            DateTimes[i] = ohlcs[sourceIndex].DateTime;
            Dates[i] = DateTimes[i].ToOADate();
        }
    }
}
