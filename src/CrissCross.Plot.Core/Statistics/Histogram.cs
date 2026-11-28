// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Statistics;

/// <summary>A histogram that accumulates the number of values observed in a continuous range of evenly-sized, user defined bins.</summary>
public sealed class Histogram
{
    /// <summary>The exception message used when a histogram receives no values.</summary>
    private const string EmptySequenceMessage = "Sequence contains no elements";

    /// <summary>Initializes a new instance of the <see cref="Histogram"/> class.</summary>
    /// <param name="edges">The evenly spaced bin edges.</param>
    private Histogram(IEnumerable<double> edges)
    {
        // WARNING: Evenly-spaced edges are REQUIRED.
        // NOTE: Previously this class supported arbitrary user-defined bins.
        // The present class assumes all bins have the same size, so this method is private.
        // NOTE: Edges are stored for easy access as arrays for plotting, not for internal use filling bins.
        Edges = [.. edges];
        Bins = new double[Edges.Length - 1];
        Array.Copy(Edges, Bins, Bins.Length);
        Counts = new int[Edges.Length - 1];
    }

    /// <summary>Gets number of values in each bin.</summary>
    public int[] Counts { get; }

    /// <summary>Gets lower edge of each bin.</summary>
    public double[] Bins { get; }

    /// <summary>Gets lower edge of each bin plus a final value representing the upper edge of the last bin.</summary>
    public double[] Edges { get; }

    /// <summary>Gets size of the first bin (distance between the first pair of bin edges).</summary>
    public double FirstBinSize => Edges[1] - Edges[0];

    /// <summary>Gets or sets whether values outside the bin range are accumulated in the lowest or highest bin.</summary>
    public bool IncludeOutliers { get; set; }

    /// <summary>Creates bins of the specified size between the first and last bin edges.</summary>
    /// <param name="binSize">The size of each bin.</param>
    /// <param name="firstBin">The left edge of the first bin.</param>
    /// <param name="lastBin">The left edge of the last bin.</param>
    /// <returns>The histogram with evenly spaced bins.</returns>
    public static Histogram WithBinSize(double binSize, double firstBin, double lastBin)
    {
        if (lastBin <= firstBin)
        {
            throw new ArgumentException($"{lastBin} must be greater than {nameof(firstBin)}");
        }

        double span = lastBin - firstBin;
        int binCount = (int)(span / binSize);
        double[] edges = new double[binCount + 1];
        for (int i = 0; i < edges.Length; i++)
        {
            edges[i] = firstBin + (i * binSize);
        }

        return new(edges);
    }

    /// <summary>Creates bins of the specified size covering the values.</summary>
    /// <param name="binSize">The size of each bin.</param>
    /// <param name="values">The values whose range determines the histogram bounds.</param>
    /// <returns>The populated histogram.</returns>
    public static Histogram WithBinSize(double binSize, IEnumerable<double> values)
    {
        ArgumentChecks.ThrowIfNull(values, nameof(values));
        double[] valueArray = [.. values];
        if (valueArray.Length == 0)
        {
            throw new InvalidOperationException(EmptySequenceMessage);
        }

        double min = valueArray[0];
        double max = valueArray[0];
        for (int i = 1; i < valueArray.Length; i++)
        {
            min = Math.Min(min, valueArray[i]);
            max = Math.Max(max, valueArray[i]);
        }

        Histogram hist = WithBinSize(binSize, min, max);
        hist.AddRange(valueArray);
        return hist;
    }

    /// <summary>Creates the specified number of evenly sized bins over the given range.</summary>
    /// <param name="count">The number of bins.</param>
    /// <param name="minValue">The lower edge of the first bin.</param>
    /// <param name="maxValue">The lower edge of the last bin.</param>
    /// <returns>The histogram with evenly spaced bins.</returns>
    public static Histogram WithBinCount(int count, double minValue, double maxValue)
    {
        if (maxValue <= minValue)
        {
            throw new ArgumentException($"{maxValue} must be greater than {nameof(minValue)}");
        }

        double[] edges = new double[count + 1];

        for (int i = 0; i < count; i++)
        {
            edges[i] = minValue + (i * (maxValue - minValue) / count);
        }

        edges[^1] = maxValue;

        return new(edges);
    }

    /// <summary>Creates the specified number of evenly sized bins covering the values.</summary>
    /// <param name="count">The number of bins.</param>
    /// <param name="values">The values whose range determines the histogram bounds.</param>
    /// <returns>The populated histogram.</returns>
    public static Histogram WithBinCount(int count, IEnumerable<double> values)
    {
        ArgumentChecks.ThrowIfNull(values, nameof(values));
        double[] valueArray = [.. values];
        if (valueArray.Length == 0)
        {
            throw new InvalidOperationException(EmptySequenceMessage);
        }

        double min = valueArray[0];
        double max = valueArray[0];
        for (int i = 1; i < valueArray.Length; i++)
        {
            min = Math.Min(min, valueArray[i]);
            max = Math.Max(max, valueArray[i]);
        }

        Histogram hist = WithBinCount(count, min, max);
        hist.AddRange(valueArray);
        return hist;
    }

    /// <summary>Sets all bin counts to zero.</summary>
    public void Clear()
    {
        for (int i = 0; i < Counts.Length; i++)
        {
            Counts[i] = 0;
        }
    }

    /// <summary>Adds a value to the corresponding bin.</summary>
    /// <param name="value">The value to add.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1473",
        Justification = "Exact equality assigns the final edge to the last inclusive bin; a tolerance would misclassify nearby values.")]
    public void Add(double value)
    {
        if (value < Edges[0])
        {
            if (IncludeOutliers)
            {
                Counts[0]++;
            }

            return;
        }

        if (value > Edges[^1])
        {
            if (IncludeOutliers)
            {
                Counts[^1]++;
            }

            return;
        }

        // NOTE: This works, but requires that this class only uses evenly sized and spaced bins.
        double min = Edges[0];
        double binWidth = (Edges[^1] - min) / Counts.Length;
        int index = (int)((value - min) / binWidth);

        if (value == Edges[^1])
        {
            index = Counts.Length - 1;
        }

        Counts[index]++;
    }

    /// <summary>Adds each value to the corresponding bin.</summary>
    /// <param name="values">The values to add.</param>
    public void AddRange(IEnumerable<double> values)
    {
        foreach (double value in values)
        {
            Add(value);
        }
    }

    /// <summary>Returns counts normalized so their sum equals one.</summary>
    /// <returns>The normalized bin probabilities.</returns>
    public double[] GetProbability() => GetProbability(1);

    /// <summary>Returns counts normalized so the sum of all values equals the specified scale.</summary>
    /// <param name="scale">The sum of the returned values.</param>
    /// <returns>The normalized bin probabilities.</returns>
    public double[] GetProbability(double scale)
    {
        int valuesCounted = 0;
        for (int i = 0; i < Counts.Length; i++)
        {
            valuesCounted += Counts[i];
        }

        double[] probabilities = new double[Counts.Length];
        for (int i = 0; i < Counts.Length; i++)
        {
            probabilities[i] = scale * Counts[i] / valuesCounted;
        }

        return probabilities;
    }

    /// <summary>Returns each bin count scaled so the peak equals one.</summary>
    /// <returns>The normalized bin counts.</returns>
    public double[] GetNormalized() => GetNormalized(1);

    /// <summary>Returns each bin count scaled so the peak equals the specified value.</summary>
    /// <param name="maxValue">The value of the largest returned bin.</param>
    /// <returns>The normalized bin counts.</returns>
    public double[] GetNormalized(double maxValue)
    {
        if (Counts.Length == 0)
        {
            throw new InvalidOperationException(EmptySequenceMessage);
        }

        int maximumCount = Counts[0];
        for (int i = 1; i < Counts.Length; i++)
        {
            maximumCount = Math.Max(maximumCount, Counts[i]);
        }

        double scale = maxValue / maximumCount;
        double[] normalized = new double[Counts.Length];
        for (int i = 0; i < Counts.Length; i++)
        {
            normalized[i] = Counts[i] * scale;
        }

        return normalized;
    }

    /// <summary>Returns the cumulative sum of the bin counts.</summary>
    /// <returns>Each bin count added to all counts in lower bins.</returns>
    public int[] GetCumulativeCounts()
    {
        int[] cumulative = new int[Counts.Length];
        cumulative[0] = Counts[0];
        for (int i = 1; i < Counts.Length; i++)
        {
            cumulative[i] = cumulative[i - 1] + Counts[i];
        }

        return cumulative;
    }

    /// <summary>Returns the cumulative probability histogram with a total scale of one.</summary>
    /// <returns>The fraction of counts in each bin and all lower bins.</returns>
    public double[] GetCumulativeProbability() => GetCumulativeProbability(1.0);

    /// <summary>Returns the cumulative probability histogram with the specified total scale.</summary>
    /// <param name="scale">The sum of the returned cumulative probabilities.</param>
    /// <returns>The fraction of counts in each bin and all lower bins.</returns>
    public double[] GetCumulativeProbability(double scale)
    {
        int[] cumulative = GetCumulativeCounts();
        double final = cumulative[^1];
        double[] probabilities = new double[cumulative.Length];
        for (int i = 0; i < cumulative.Length; i++)
        {
            probabilities[i] = scale * cumulative[i] / final;
        }

        return probabilities;
    }
}
