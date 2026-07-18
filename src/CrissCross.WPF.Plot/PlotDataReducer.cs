// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Provides shape-preserving reduction for very large historic data sets.</summary>
public static class PlotDataReducer
{
    /// <summary>Provides the minimum useful LTTB rendering budget.</summary>
    private const int MinimumPointCount = 3;

    /// <summary>Provides the number of fixed endpoints in an LTTB result.</summary>
    private const int EndpointCount = 2;

    /// <summary>Reduces a series using the Largest-Triangle-Three-Buckets algorithm.</summary>
    /// <param name="series">The historic series to reduce.</param>
    /// <param name="targetPointCount">The desired maximum point count. It must be at least three.</param>
    /// <returns>The reduced series, or the original series when reduction is unnecessary.</returns>
    public static PlotSeriesData LargestTriangleThreeBuckets(PlotSeriesData series, int targetPointCount)
    {
        if (series is null)
        {
            throw new ArgumentNullException(nameof(series));
        }

        if (targetPointCount < MinimumPointCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(targetPointCount),
                targetPointCount,
                "At least three points are required for LTTB reduction.");
        }

        if (series.X.Count <= targetPointCount)
        {
            return series;
        }

        var sampledX = new double[targetPointCount];
        var sampledY = new double[targetPointCount];
        sampledX[0] = series.X[0];
        sampledY[0] = series.Y[0];

        var bucketSize = (series.X.Count - EndpointCount) / (double)(targetPointCount - EndpointCount);
        var selectedIndex = 0;
        for (var bucket = 0; bucket < targetPointCount - EndpointCount; bucket++)
        {
            var average = CalculateNextBucketAverage(series, bucket, bucketSize);
            selectedIndex = SelectLargestTrianglePoint(series, bucket, bucketSize, selectedIndex, average);
            sampledX[bucket + 1] = series.X[selectedIndex];
            sampledY[bucket + 1] = series.Y[selectedIndex];
        }

        sampledX[targetPointCount - 1] = series.X[series.X.Count - 1];
        sampledY[targetPointCount - 1] = series.Y[series.Y.Count - 1];
        return new(series.Key, sampledX, sampledY, series.XAxisKind);
    }

    /// <summary>Calculates the centroid of the next LTTB bucket.</summary>
    /// <param name="series">The source series.</param>
    /// <param name="bucket">The current bucket index.</param>
    /// <param name="bucketSize">The fractional bucket size.</param>
    /// <returns>The next bucket centroid.</returns>
    private static (double X, double Y) CalculateNextBucketAverage(PlotSeriesData series, int bucket, double bucketSize)
    {
        var averageStart = Math.Min(series.X.Count - 1, (int)Math.Floor((bucket + 1) * bucketSize) + 1);
        var averageEnd = Math.Min(series.X.Count, (int)Math.Floor((bucket + EndpointCount) * bucketSize) + 1);
        var averageCount = Math.Max(1, averageEnd - averageStart);
        var averageX = 0D;
        var averageY = 0D;
        for (var i = averageStart; i < averageEnd; i++)
        {
            averageX += series.X[i];
            averageY += series.Y[i];
        }

        return (averageX / averageCount, averageY / averageCount);
    }

    /// <summary>Selects the point that forms the largest triangle for one LTTB bucket.</summary>
    /// <param name="series">The source series.</param>
    /// <param name="bucket">The current bucket index.</param>
    /// <param name="bucketSize">The fractional bucket size.</param>
    /// <param name="selectedIndex">The previously selected point index.</param>
    /// <param name="average">The next bucket centroid.</param>
    /// <returns>The selected source point index.</returns>
    private static int SelectLargestTrianglePoint(
        PlotSeriesData series,
        int bucket,
        double bucketSize,
        int selectedIndex,
        (double X, double Y) average)
    {
        var rangeStart = Math.Min(series.X.Count - EndpointCount, (int)Math.Floor(bucket * bucketSize) + 1);
        var rangeEnd = Math.Min(series.X.Count - 1, (int)Math.Floor((bucket + 1) * bucketSize) + 1);
        var pointAx = series.X[selectedIndex];
        var pointAy = series.Y[selectedIndex];
        var maximumArea = double.MinValue;
        var nextIndex = rangeStart;
        for (var i = rangeStart; i <= rangeEnd; i++)
        {
            var area = Math.Abs(
                ((pointAx - average.X) * (series.Y[i] - pointAy)) - ((pointAx - series.X[i]) * (average.Y - pointAy)));
            if (area <= maximumArea)
            {
                continue;
            }

            maximumArea = area;
            nextIndex = i;
        }

        return nextIndex;
    }
}
