// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Statistics;

/// <summary>Provides moving-window calculations for numeric series.</summary>
public static class Series
{
    /// <summary>The minimum supported number of values in a moving window.</summary>
    private const int MinimumWindowSize = 2;

    /// <summary>Calculates a moving-window average for the given data.</summary>
    /// <param name="values">The input data.</param>
    /// <param name="window">The number of values in each window.</param>
    /// <param name="preserveLength">Whether to retain the leading NaN values.</param>
    /// <returns>The moving averages.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional default is retained for source and binary compatibility.")]
    public static double[] MovingAverage(double[] values, int window, bool preserveLength = false)
    {
        if (window < MinimumWindowSize)
        {
            throw new ArgumentException("period must be 2 or greater");
        }

        if (window > values.Length)
        {
            throw new ArgumentException("period cannot be longer than number of values");
        }

        double[] sma = new double[values.Length];

        for (int i = 0; i < values.Length; i++)
        {
            if (i < window)
            {
                sma[i] = double.NaN;
            }
            else
            {
                var periodValues = new double[window];
                Array.Copy(values, i - window + 1, periodValues, 0, window);
                sma[i] = Descriptive.Mean(periodValues);
            }
        }

        if (!preserveLength)
        {
            double[] trimmedAverage = new double[sma.Length - window];
            Array.Copy(sma, window, trimmedAverage, 0, trimmedAverage.Length);
            sma = trimmedAverage;
        }

        return sma;
    }

    /// <summary>Calculates a moving-window standard deviation for the given data.</summary>
    /// <param name="values">The input data.</param>
    /// <param name="window">The number of values in each window.</param>
    /// <param name="preserveLength">Whether to retain the leading NaN values.</param>
    /// <returns>The moving standard deviations.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional default is retained for source and binary compatibility.")]
    public static double[] SimpleMovingStandardDeviation(double[] values, int window, bool preserveLength = false)
    {
        if (window < MinimumWindowSize)
        {
            throw new ArgumentException("period must be 2 or greater");
        }

        if (window > values.Length)
        {
            throw new ArgumentException("period cannot be longer than number of values");
        }

        double[] standardDeviations = new double[values.Length];

        for (int i = 0; i < values.Length; i++)
        {
            if (i < window)
            {
                standardDeviations[i] = double.NaN;
                continue;
            }

            var periodValues = new double[window];
            Array.Copy(values, i - window + 1, periodValues, 0, window);
            standardDeviations[i] = Descriptive.StandardDeviation(periodValues);
        }

        if (!preserveLength)
        {
            double[] trimmedDeviations = new double[standardDeviations.Length - window];
            Array.Copy(standardDeviations, window, trimmedDeviations, 0, trimmedDeviations.Length);
            standardDeviations = trimmedDeviations;
        }

        return standardDeviations;
    }
}
