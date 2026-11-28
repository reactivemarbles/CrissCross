// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Threading.Tasks;

using CrissCross.Plot.DataSources;

namespace CrissCross.Plot;

/// <summary>Provides cached min/max ranges for repeated queries over a numeric list.</summary>
public class MinMaxCache
{
    /// <summary>The default number of source values in each cached range.</summary>
    private const int DefaultCachePeriod = 1000;

    /// <summary>The source values queried by this cache.</summary>
    private readonly IReadOnlyList<double> _data;

    /// <summary>Cached ranges for complete cache periods.</summary>
    private readonly SignalRangeY[] _cache;

    /// <summary>Initializes a new instance of the <see cref="MinMaxCache"/> class using the default period.</summary>
    /// <param name="data">The values to query.</param>
    public MinMaxCache(IReadOnlyList<double> data)
        : this(data, DefaultCachePeriod)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="MinMaxCache"/> class using the specified period.</summary>
    /// <param name="data">The values to query.</param>
    /// <param name="cachePeriod">The number of values in each cached range.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="cachePeriod"/> is not positive.</exception>
    public MinMaxCache(IReadOnlyList<double> data, int cachePeriod)
    {
        data = ThrowIfNull(data, nameof(data));
#if NET7_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cachePeriod);
#else
        if (cachePeriod <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cachePeriod), "The cache period must be greater than zero.");
        }
#endif

        _data = data;
        CachePeriod = cachePeriod;
        int cacheSize = data.Count / cachePeriod;
        _cache = new SignalRangeY[cacheSize];

        _ = Parallel.For(0, cacheSize, i =>
        {
            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;

            for (int j = 0; j < cachePeriod; j++)
            {
                double sample = data[(i * cachePeriod) + j];
                AddSample(sample, ref min, ref max);
            }

            _cache[i] = new(min, max);
        });
    }

    /// <summary>Gets the number of source values in each cached period.</summary>
    public int CachePeriod { get; }

    /// <summary>Gets the minimum and maximum values in the specified half-open index range.</summary>
    /// <param name="start">The inclusive start index.</param>
    /// <param name="end">The exclusive end index.</param>
    /// <returns>The minimum and maximum values in the range.</returns>
    public SignalRangeY GetMinMax(int start, int end)
    {
        double min = double.PositiveInfinity;
        double max = double.NegativeInfinity;

        int firstCachedPeriod = (start / CachePeriod) + 1;
        int startOfCachedRange = firstCachedPeriod * CachePeriod;
        if (end - start < CachePeriod || startOfCachedRange >= end)
        {
            AddDataRange(start, end, ref min, ref max);
            return new(min, max);
        }

        int lastCachedPeriod = end / CachePeriod;
        int endOfCachedRange = lastCachedPeriod * CachePeriod;
        AddDataRange(start, startOfCachedRange, ref min, ref max);

        for (int i = firstCachedPeriod; i < lastCachedPeriod; i++)
        {
            SignalRangeY cachedRange = _cache[i];
            AddSample(cachedRange.Min, ref min, ref max);
            AddSample(cachedRange.Max, ref min, ref max);
        }

        AddDataRange(endOfCachedRange, end, ref min, ref max);
        return new(min, max);
    }

    /// <summary>Adds a value to the accumulated bounds.</summary>
    /// <param name="sample">The value to add.</param>
    /// <param name="min">The minimum accumulated value.</param>
    /// <param name="max">The maximum accumulated value.</param>
    private static void AddSample(double sample, ref double min, ref double max)
    {
        min = sample < min ? sample : min;
        max = max < sample ? sample : max;
    }

    /// <summary>Throws when a required reference parameter is null.</summary>
    /// <typeparam name="T">The reference type to validate.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The validated non-null value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "PSH1409", Justification = "ArgumentNullException.ThrowIfNull is unavailable for the supported .NET Framework targets.")]
    private static T ThrowIfNull<T>(T value, string parameterName)
        where T : class
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(value, parameterName);
        return value;
#else
        return value ?? throw new ArgumentNullException(parameterName, "The data collection must not be null.");
#endif
    }

    /// <summary>Adds source values in the half-open range to the accumulated bounds.</summary>
    /// <param name="start">The inclusive start index.</param>
    /// <param name="end">The exclusive end index.</param>
    /// <param name="min">The minimum accumulated value.</param>
    /// <param name="max">The maximum accumulated value.</param>
    private void AddDataRange(int start, int end, ref double min, ref double max)
    {
        for (int i = start; i < end; i++)
        {
            AddSample(_data[i], ref min, ref max);
        }
    }
}
