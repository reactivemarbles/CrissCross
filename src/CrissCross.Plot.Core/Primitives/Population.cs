// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Stores a population and its descriptive statistics.</summary>
public class Population
{
    /// <summary>Initializes a new instance of the <see cref="Population"/> class.</summary>
    /// <param name="values">The population values.</param>
    public Population(double[] values)
    {
        ArgumentChecks.ThrowIfNull(values, nameof(values));
        if (values.Length == 0)
        {
            throw new InvalidOperationException("Sequence contains no elements");
        }

        Values = [.. values];
        double[] sortedValues = [.. Values];
        Array.Sort(sortedValues);
        SortedValues = sortedValues;
        Count = values.Length;

        double min = values[0];
        double max = values[0];
        for (int i = 1; i < values.Length; i++)
        {
            min = Math.Min(min, values[i]);
            max = Math.Max(max, values[i]);
        }

        Min = min;
        Max = max;
        Mean = Statistics.Descriptive.Mean(values);
        Variance = Statistics.Descriptive.Variance(values);
        StandardDeviation = Statistics.Descriptive.StandardDeviation(values);
        StandardError = Statistics.Descriptive.StandardError(values);
        Median = Statistics.Descriptive.SortedMedian(SortedValues);
    }

    /// <summary>Gets the values in their original order.</summary>
    public IReadOnlyList<double> Values { get; }

    /// <summary>Gets the values in ascending order.</summary>
    public IReadOnlyList<double> SortedValues { get; }

    /// <summary>Gets the number of values.</summary>
    public int Count { get; }

    /// <summary>Gets the smallest value.</summary>
    public double Min { get; }

    /// <summary>Gets the largest value.</summary>
    public double Max { get; }

    /// <summary>Gets the arithmetic mean.</summary>
    public double Mean { get; }

    /// <summary>Gets the population variance.</summary>
    public double Variance { get; }

    /// <summary>Gets the population standard deviation.</summary>
    public double StandardDeviation { get; }

    /// <summary>Gets the standard error of the mean.</summary>
    public double StandardError { get; }

    /// <summary>Gets the median value.</summary>
    public double Median { get; }

    /// <summary>Gets the value at the specified percentile.</summary>
    /// <param name="percentile">The percentile to retrieve, from 0 to 100.</param>
    /// <returns>The value at the requested percentile.</returns>
    public double GetPercentile(double percentile) => Statistics.Descriptive.SortedPercentile(SortedValues, percentile);
}
