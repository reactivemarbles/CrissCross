// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Provides an XY signal data source backed by read-only generic lists.</summary>
/// <typeparam name="Tx">The type of the X-coordinate values.</typeparam>
/// <typeparam name="Ty">The type of the Y-coordinate values.</typeparam>
public class SignalXYSourceGenericList<Tx, Ty> : ISignalXYSource, IDataSource, IGetNearest
{
    /// <summary>The fraction of a pixel used to overlap adjacent render ranges.</summary>
    private const double PixelOverlapFraction = 0.01;

    /// <summary>The minimum number of points required to include extrema.</summary>
    private const int MinimumPointCountForExtrema = 2;

    /// <summary>The read-only X-coordinate values.</summary>
    private readonly IReadOnlyList<Tx> _xs;

    /// <summary>The read-only Y-coordinate values.</summary>
    private readonly IReadOnlyList<Ty> _ys;

    /// <summary>The optional upper bound for the rendered data indices.</summary>
    private int? _maximumIndex;

    /// <summary>Initializes a new instance of the <see cref="SignalXYSourceGenericList{Tx, Ty}"/> class with matching X and Y values.</summary>
    /// <param name="xs">The X-coordinate values.</param>
    /// <param name="ys">The Y-coordinate values.</param>
    /// <exception cref="ArgumentException">The lists have different lengths.</exception>
    public SignalXYSourceGenericList(IReadOnlyList<Tx> xs, IReadOnlyList<Ty> ys)
    {
        if (xs.Count != ys.Count)
        {
            throw new ArgumentException($"{nameof(xs)} and {nameof(ys)} must have equal length");
        }

        _xs = xs;
        _ys = ys;
    }

    /// <summary>Gets the number of paired X and Y values.</summary>
    public int Count => Math.Min(_xs.Count, _ys.Count);

    /// <summary>Gets or sets whether the signal is rendered with its axes exchanged.</summary>
    public bool Rotated { get; set; }

    /// <summary>Gets or sets the offset applied to each X-coordinate.</summary>
    public double XOffset { get; set; }

    /// <summary>Gets or sets the offset applied to each Y-coordinate.</summary>
    public double YOffset { get; set; }

    /// <summary>Gets or sets the scale applied to each Y-coordinate.</summary>
    public double YScale { get; set; } = 1;

    /// <summary>Gets or sets the scale applied to each X-coordinate.</summary>
    public double XScale { get; set; } = 1;

    /// <summary>Gets or sets the first data index included in rendering.</summary>
    public int MinimumIndex { get; set; }

    /// <summary>Gets or sets the final data index included in rendering.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Style",
        "SST2200",
        Justification = "Older target frameworks do not support the field keyword; nullable state distinguishes default and explicit maxima.")]
    public int MaximumIndex
    {
        get => _maximumIndex ?? (Count - 1);
        set => _maximumIndex = value;
    }

    bool IDataSource.PreferCoordinates => false;

    int IDataSource.Length => _xs.Count;

    int IDataSource.MinRenderIndex => MinimumIndex;

    int IDataSource.MaxRenderIndex => MaximumIndex;

    /// <summary>Gets whether a small overlap is added when selecting pixel columns to avoid edge gaps.</summary>
    public bool UsePixelOverlap { get; }

    /// <summary>Gets the axis limits covered by the configured data-index range.</summary>
    /// <returns>The limits of the selected data, or <see cref="AxisLimits.NoLimits" /> when empty.</returns>
    public AxisLimits GetAxisLimits()
    {
        if (_xs.Count == 0)
        {
            return AxisLimits.NoLimits;
        }

        double minimumX = (NumericConversion.GenericToDouble(_xs, MinimumIndex) * XScale) + XOffset;
        double maximumX = (NumericConversion.GenericToDouble(_xs, MaximumIndex) * XScale) + XOffset;
        CoordinateRange horizontalRange = new(minimumX, maximumX);
        CoordinateRange verticalRange = GetRangeY(MinimumIndex, MaximumIndex);
        return new(horizontalRange, verticalRange);
    }

    /// <summary>Gets the pixels to draw for the current signal orientation.</summary>
    /// <param name="rp">The render information and data rectangle.</param>
    /// <param name="axes">The axes used to convert coordinates to pixels.</param>
    /// <param name="connectStyle">The line-connection style used at the visible range edges.</param>
    /// <returns>The pixels that represent the visible signal.</returns>
    public IReadOnlyList<Pixel> GetPixelsToDraw(RenderPack rp, IAxes axes, ConnectStyle connectStyle) =>
        Rotated
            ? GetPixelsToDrawVertically(rp, axes, connectStyle)
            : GetPixelsToDrawHorizontally(rp, axes, connectStyle);

    /// <summary>Gets the signal pixels grouped across horizontal pixel columns.</summary>
    /// <param name="rp">The render information and data rectangle.</param>
    /// <param name="axes">The axes used to convert coordinates to pixels.</param>
    /// <param name="connectStyle">The line-connection style used at the visible range edges.</param>
    /// <returns>The pixels that represent the visible signal.</returns>
    public Pixel[] GetPixelsToDrawHorizontally(RenderPack rp, IAxes axes, ConnectStyle connectStyle)
    {
        // determine the range of data in view
        (Pixel[] PointBefore, int dataIndexFirst) = GetFirstPointX(axes);
        (Pixel[] PointAfter, int dataIndexLast) = GetLastPointX(axes);
        IndexRange visibleRange = new(dataIndexFirst, dataIndexLast);

        if (visibleRange.IsValid && NumericConversion.GenericToDouble(_xs, dataIndexFirst) > NumericConversion.GenericToDouble(_xs, dataIndexLast))
        {
            throw new InvalidDataException("Xs must contain only ascending values. "
                + $"The value at index {dataIndexFirst} ({_xs[dataIndexFirst]}) is greater than the value at index {dataIndexLast} ({_xs[dataIndexLast]})");
        }

        // get all points in view
        Pixel[] visiblePoints = [];
        if (visibleRange.Length > 0)
        {
            List<Pixel> visiblePixelList = [];
            int pixelColumnCount = (int)Math.Ceiling(rp.DataRect.Width);
            for (int pixelColumnIndex = 0; pixelColumnIndex < pixelColumnCount; pixelColumnIndex++)
            {
                visiblePixelList.AddRange(GetColumnPixelsX(pixelColumnIndex, visibleRange, rp, axes));
            }

            visiblePoints = visiblePixelList.ToArray();
        }

        Pixel[] leftOutsidePoint = PointBefore;
        Pixel[] rightOutsidePoint = PointAfter;
        if (axes.XAxis.Range.Span < 0)
        {
            leftOutsidePoint = PointAfter;
            rightOutsidePoint = PointBefore;
        }

        // combine with one extra point before and after
        Pixel[] points = [.. leftOutsidePoint, .. visiblePoints, .. rightOutsidePoint];

        // use interpolation at the edges to prevent points from going way off the screen
        if (leftOutsidePoint.Length > 0)
        {
            SignalInterpolation.InterpolateBeforeX(rp, points, connectStyle);
        }

        if (rightOutsidePoint.Length > 0)
        {
            SignalInterpolation.InterpolateAfterX(rp, points, connectStyle);
        }

        return points;
    }

    /// <summary>Gets the signal pixels grouped across vertical pixel rows.</summary>
    /// <param name="rp">The render information and data rectangle.</param>
    /// <param name="axes">The axes used to convert coordinates to pixels.</param>
    /// <param name="connectStyle">The line-connection style used at the visible range edges.</param>
    /// <returns>The pixels that represent the visible signal.</returns>
    public Pixel[] GetPixelsToDrawVertically(RenderPack rp, IAxes axes, ConnectStyle connectStyle)
    {
        // determine the range of data in view
        (Pixel[] PointBefore, int dataIndexFirst) = GetFirstPointY(axes);
        (Pixel[] PointAfter, int dataIndexLast) = GetLastPointY(axes);
        IndexRange visibleRange = new(dataIndexFirst, dataIndexLast);

        if (visibleRange.IsValid && NumericConversion.GenericToDouble(_xs, dataIndexFirst) > NumericConversion.GenericToDouble(_xs, dataIndexLast))
        {
            throw new InvalidDataException("Xs must contain only ascending values. "
                + $"The value at index {dataIndexFirst} ({_xs[dataIndexFirst]}) is greater than the value at index {dataIndexLast} ({_xs[dataIndexLast]})");
        }

        // get all points in view
        Pixel[] visiblePoints = [];
        if (visibleRange.Length > 0)
        {
            List<Pixel> visiblePixelList = [];
            int pixelRowCount = (int)Math.Ceiling(rp.DataRect.Height);
            for (int pixelRowIndex = 0; pixelRowIndex < pixelRowCount; pixelRowIndex++)
            {
                visiblePixelList.AddRange(GetColumnPixelsY(pixelRowIndex, visibleRange, rp, axes));
            }

            visiblePoints = visiblePixelList.ToArray();
        }

        Pixel[] bottomOutsidePoint = PointBefore;
        Pixel[] topOutsidePoint = PointAfter;
        if (axes.YAxis.Range.Span < 0)
        {
            bottomOutsidePoint = PointAfter;
            topOutsidePoint = PointBefore;
        }

        // combine with one extra point before and after
        Pixel[] points = [.. bottomOutsidePoint, .. visiblePoints, .. topOutsidePoint];

        // use interpolation at the edges to prevent points from going way off the screen
        if (bottomOutsidePoint.Length > 0)
        {
            SignalInterpolation.InterpolateBeforeY(rp, points, connectStyle);
        }

        if (topOutsidePoint.Length > 0)
        {
            SignalInterpolation.InterpolateAfterY(rp, points, connectStyle);
        }

        return points;
    }

    /// <summary>Gets the vertical range covered by data between two indices, inclusive.</summary>
    /// <param name="index1">The first data index.</param>
    /// <param name="index2">The second data index.</param>
    /// <returns>The minimum and maximum scaled Y values in the requested range.</returns>
    public CoordinateRange GetRangeY(int index1, int index2)
    {
        double min = NumericConversion.GenericToDouble(_ys, index1);
        double max = NumericConversion.GenericToDouble(_ys, index1);

        var minIndex = Math.Min(index1, index2);
        var maxIndex = Math.Max(index1, index2);

        for (int i = minIndex; i <= maxIndex; i++)
        {
            double value = NumericConversion.GenericToDouble(_ys, i);
            min = Math.Min(value, min);
            max = Math.Max(value, max);
        }

        return new((min * YScale) + YOffset, (max * YScale) + YOffset);
    }

    /// <summary>Gets the data index associated with an X position.</summary>
    /// <param name="x">The X position to search for.</param>
    /// <returns>The nearest index in the configured data range.</returns>
    public int GetIndex(double x)
    {
        IndexRange range = new(MinimumIndex, MaximumIndex);
        return GetIndex(x, range);
    }

    /// <summary>Gets the data index associated with an X position within a specified range.</summary>
    /// <param name="x">The X position to search for.</param>
    /// <param name="indexRange">The inclusive range of indices to search.</param>
    /// <returns>The nearest index in the specified range.</returns>
    public int GetIndex(double x, IndexRange indexRange)
    {
        var (_, index) = SearchIndex(x, indexRange);
        return index;
    }

    /// <summary>Gets the signal pixels intersecting one horizontal pixel column.</summary>
    /// <param name="pixelColumnIndex">The zero-based pixel-column index.</param>
    /// <param name="rng">The inclusive range of data indices to inspect.</param>
    /// <param name="rp">The render information and data rectangle.</param>
    /// <param name="axes">The axes used to convert coordinates to pixels.</param>
    /// <returns>No pixels for an empty column, one pixel for a single point, or entry, extrema, and exit pixels for multiple points.</returns>
    public IEnumerable<Pixel> GetColumnPixelsX(int pixelColumnIndex, IndexRange rng, RenderPack rp, IAxes axes)
    {
        float pixelX = pixelColumnIndex + rp.DataRect.Left;
        double unitsPerPixelX = axes.XAxis.Width / rp.DataRect.Width;
        double start = axes.XAxis.Min + (unitsPerPixelX * pixelColumnIndex);
        double end = start + unitsPerPixelX;

        if (UsePixelOverlap)
        {
            end += unitsPerPixelX * PixelOverlapFraction;
        }

        var (startIndex, _) = SearchIndex(start, rng);
        var (endIndex, _) = SearchIndex(end, rng);
        int pointsInRange = Math.Abs(endIndex - startIndex);

        if (pointsInRange == 0)
        {
            yield break;
        }

        int firstIndex = startIndex < endIndex ? startIndex : startIndex - 1;
        int lastIndex = startIndex < endIndex ? endIndex - 1 : endIndex;
        double startY = (NumericConversion.GenericToDouble(_ys, firstIndex) * YScale) + YOffset;
        double endY = (NumericConversion.GenericToDouble(_ys, lastIndex) * YScale) + YOffset;

        yield return new Pixel(pixelX, axes.GetPixelY(startY)); // enter

        if (pointsInRange > MinimumPointCountForExtrema)
        {
            CoordinateRange verticalRange = GetRangeY(firstIndex, lastIndex); // YOffset is added in GetRangeY

            if (startY > endY)
            { // signal amplitude is decreasing, so we'll return the maximum before the minimum
                yield return new Pixel(pixelX, axes.GetPixelY(verticalRange.Max)); // max
                yield return new Pixel(pixelX, axes.GetPixelY(verticalRange.Min)); // min
            }
            else
            { // signal amplitude is increasing, so we'll return the minimum before the maximum
                yield return new Pixel(pixelX, axes.GetPixelY(verticalRange.Min)); // min
                yield return new Pixel(pixelX, axes.GetPixelY(verticalRange.Max)); // max
            }
        }

        if (pointsInRange <= 1)
        {
            yield break;
        }

        yield return new Pixel(pixelX, axes.GetPixelY(endY)); // exit
    }

    /// <summary>Gets the signal pixels intersecting one vertical pixel row.</summary>
    /// <param name="pixelColumnIndex">The zero-based pixel-row index.</param>
    /// <param name="rng">The inclusive range of data indices to inspect.</param>
    /// <param name="rp">The render information and data rectangle.</param>
    /// <param name="axes">The axes used to convert coordinates to pixels.</param>
    /// <returns>No pixels for an empty row, one pixel for a single point, or entry, extrema, and exit pixels for multiple points.</returns>
    public IEnumerable<Pixel> GetColumnPixelsY(int pixelColumnIndex, IndexRange rng, RenderPack rp, IAxes axes)
    {
        // The row index counts upwards from the bottom while pixel coordinates count from the top.
        float pixelY = rp.DataRect.Bottom - pixelColumnIndex;
        double unitsPerPixelY = axes.YAxis.Height / rp.DataRect.Height;
        double start = axes.YAxis.Min + (unitsPerPixelY * pixelColumnIndex);
        double end = start + unitsPerPixelY;

        // add slight overlap to prevent floating point errors from missing points
        double overlap = unitsPerPixelY * PixelOverlapFraction;
        end += overlap;

        var (startIndex, _) = SearchIndex(start, rng);
        var (endIndex, _) = SearchIndex(end, rng);
        int pointsInRange = Math.Abs(endIndex - startIndex);

        if (pointsInRange == 0)
        {
            yield break;
        }

        int firstIndex = startIndex < endIndex ? startIndex : startIndex - 1;
        int lastIndex = startIndex < endIndex ? endIndex - 1 : endIndex;
        double startY = (NumericConversion.GenericToDouble(_ys, firstIndex) * YScale) + YOffset;
        double endY = (NumericConversion.GenericToDouble(_ys, lastIndex) * YScale) + YOffset;

        yield return new Pixel(axes.GetPixelX(startY), pixelY); // enter

        if (pointsInRange > MinimumPointCountForExtrema)
        {
            CoordinateRange verticalRange = GetRangeY(firstIndex, lastIndex); // YOffset is added in GetRangeY

            if (startY > endY)
            { // signal amplitude is decreasing, so we'll return the maximum before the minimum
                yield return new Pixel(axes.GetPixelX(verticalRange.Max), pixelY); // max
                yield return new Pixel(axes.GetPixelX(verticalRange.Min), pixelY); // min
            }
            else
            { // signal amplitude is increasing, so we'll return the minimum before the maximum
                yield return new Pixel(axes.GetPixelX(verticalRange.Min), pixelY); // min
                yield return new Pixel(axes.GetPixelX(verticalRange.Max), pixelY); // max
            }
        }

        if (pointsInRange <= 1)
        {
            yield break;
        }

        yield return new Pixel(axes.GetPixelX(endY), pixelY); // exit
    }

    /// <summary>Gets the nearest data point to a coordinate.</summary>
    /// <param name="location">The coordinate to search around.</param>
    /// <param name="renderInfo">The current render information.</param>
    /// <param name="maxDistance">The maximum pixel distance for a match.</param>
    /// <returns>The nearest matching data point.</returns>
    public DataPoint GetNearest(Coordinates location, RenderDetails renderInfo, float maxDistance = 15) =>
        DataSourceUtilities.GetNearestFast(this, location, renderInfo, maxDistance);

    /// <summary>Gets the nearest data point using X-axis distance.</summary>
    /// <param name="location">The coordinate to search around.</param>
    /// <param name="renderInfo">The current render information.</param>
    /// <param name="maxDistance">The maximum pixel distance for a match.</param>
    /// <returns>The nearest matching data point.</returns>
    public DataPoint GetNearestX(Coordinates location, RenderDetails renderInfo, float maxDistance = 15) =>
        DataSourceUtilities.GetNearestXFast(this, location, renderInfo, maxDistance);

    /// <summary>Finds the first data point and any point immediately before the visible X range.</summary>
    /// <param name="axes">The axes defining the visible range.</param>
    /// <returns>The optional preceding pixel and the first visible data index.</returns>
    private (Pixel[] pointBefore, int firstIndex) GetFirstPointX(IAxes axes)
    {
        if (_xs.Count == 1)
        {
            return ([], MinimumIndex);
        }

        var (firstPointIndex, _) = SearchIndex(axes.XAxis.Range.Span > 0 ? axes.XAxis.Min : axes.XAxis.Max); // if axis is reversed first index will on the right limit of the plot

        if (firstPointIndex > MinimumIndex)
        {
            double x = (NumericConversion.GenericToDouble(_xs, firstPointIndex - 1) * XScale) + XOffset;
            double y = (NumericConversion.GenericToDouble(_ys, firstPointIndex - 1) * YScale) + YOffset;
            float beforeX = axes.GetPixelX(x);
            float beforeY = axes.GetPixelY(y);
            Pixel beforePoint = new(beforeX, beforeY);
            return ([beforePoint], firstPointIndex);
        }

        return ([], MinimumIndex);
    }

    /// <summary>Finds the first data point and any point immediately before the visible Y range.</summary>
    /// <param name="axes">The axes defining the visible range.</param>
    /// <returns>The optional preceding pixel and the first visible data index.</returns>
    private (Pixel[] pointBefore, int firstIndex) GetFirstPointY(IAxes axes)
    {
        if (_xs.Count == 1)
        {
            return ([], MinimumIndex);
        }

        var (firstPointIndex, _) = SearchIndex(axes.YAxis.Range.Span > 0 ? axes.YAxis.Min : axes.YAxis.Max); // if axis is reversed first index will on the top limit of the plot

        if (firstPointIndex > MinimumIndex)
        {
            double x = (NumericConversion.GenericToDouble(_xs, firstPointIndex - 1) * XScale) + XOffset;
            double y = (NumericConversion.GenericToDouble(_ys, firstPointIndex - 1) * YScale) + YOffset;
            float beforeX = axes.GetPixelX(y);
            float beforeY = axes.GetPixelY(x);
            Pixel beforePoint = new(beforeX, beforeY);
            return ([beforePoint], firstPointIndex);
        }

        return ([], MinimumIndex);
    }

    /// <summary>Finds the last data point and any point immediately after the visible X range.</summary>
    /// <param name="axes">The axes defining the visible range.</param>
    /// <returns>The optional following pixel and the last visible data index.</returns>
    private (Pixel[] pointAfter, int lastIndex) GetLastPointX(IAxes axes)
    {
        if (_xs.Count == 1)
        {
            return ([], MaximumIndex);
        }

        var (lastPointIndex, _) = SearchIndex(axes.XAxis.Range.Span > 0 ? axes.XAxis.Max : axes.XAxis.Min); // if axis is reversed last index will on the left limit of the plot

        if (lastPointIndex <= MaximumIndex)
        {
            double x = (NumericConversion.GenericToDouble(_xs, lastPointIndex) * XScale) + XOffset;
            double y = (NumericConversion.GenericToDouble(_ys, lastPointIndex) * YScale) + YOffset;
            float afterX = axes.GetPixelX(x);
            float afterY = axes.GetPixelY(y);
            Pixel afterPoint = new(afterX, afterY);
            return ([afterPoint], lastPointIndex - 1);
        }

        return ([], MaximumIndex);
    }

    /// <summary>Finds the last data point and any point immediately after the visible Y range.</summary>
    /// <param name="axes">The axes defining the visible range.</param>
    /// <returns>The optional following pixel and the last visible data index.</returns>
    private (Pixel[] pointAfter, int lastIndex) GetLastPointY(IAxes axes)
    {
        if (_xs.Count == 1)
        {
            return ([], MaximumIndex);
        }

        var (lastPointIndex, _) = SearchIndex(axes.YAxis.Range.Span > 0 ? axes.YAxis.Max : axes.YAxis.Min); // if axis is reversed last index will on the bottom limit of the plot

        if (lastPointIndex <= MaximumIndex)
        {
            double x = (NumericConversion.GenericToDouble(_xs, lastPointIndex) * XScale) + XOffset;
            double y = (NumericConversion.GenericToDouble(_ys, lastPointIndex) * YScale) + YOffset;
            float afterX = axes.GetPixelX(y);
            float afterY = axes.GetPixelY(x);
            Pixel afterPoint = new(afterX, afterY);
            return ([afterPoint], lastPointIndex - 1);
        }

        return ([], MaximumIndex);
    }

    /// <summary>Finds the sorted-list position associated with an X position.</summary>
    /// <param name="x">The X position to search for.</param>
    /// <returns>The insertion position and its index limited to the configured data range.</returns>
    private (int SearchedPosition, int LimitedIndex) SearchIndex(double x)
    {
        IndexRange range = new(MinimumIndex, MaximumIndex);
        return SearchIndex(x, range);
    }

    /// <summary>Finds the sorted-list position associated with an X position within a range.</summary>
    /// <param name="x">The X position to search for.</param>
    /// <param name="indexRange">The inclusive range of indices to search.</param>
    /// <returns>The insertion position and its index limited to the specified range.</returns>
    private (int SearchedPosition, int LimitedIndex) SearchIndex(double x, IndexRange indexRange)
    {
        NumericConversion.DoubleToGeneric((x - XOffset) / XScale, out Tx x2);
        int index = BinarySearchHelper.BinarySearch(_xs, indexRange.Min, indexRange.Max, x2);

        // For an unmatched value, BinarySearch returns the bitwise complement of its insertion position.
        // An insertion before the range becomes zero; an insertion after it is capped at the final index.
        // This keeps the result valid while preserving the insertion order within the searched range.
        if (index < 0)
        {
            index = ~index; // read BinarySearch() docs
        }

        return (SearchedPosition: index, LimitedIndex: index > indexRange.Max ? indexRange.Max : index);
    }

    int IDataSource.GetXClosestIndex(Coordinates mouseLocation) =>
        Rotated
            ? GetIndex(mouseLocation.Y)
            : GetIndex(mouseLocation.X);

    Coordinates IDataSource.GetCoordinate(int index)
    {
        double x = NumericConversion.GenericToDouble(_xs, index);
        double y = NumericConversion.GenericToDouble(_ys, index);
        return new(x, y);
    }

    Coordinates IDataSource.GetCoordinateScaled(int index)
    {
        double x = DataSourceUtilities.ScaleXY(_xs, index, XScale, XOffset);
        double y = DataSourceUtilities.ScaleXY(_ys, index, YScale, YOffset);
        return new(x, y);
    }

    double IDataSource.GetX(int index) => Rotated
        ? NumericConversion.GenericToDouble(_ys, index)
        : NumericConversion.GenericToDouble(_xs, index);

    double IDataSource.GetXScaled(int index) => Rotated
        ? DataSourceUtilities.ScaleXY(_ys, index, YScale, YOffset)
        : DataSourceUtilities.ScaleXY(_xs, index, XScale, XOffset);

    double IDataSource.GetY(int index) => Rotated
        ? NumericConversion.GenericToDouble(_xs, index)
        : NumericConversion.GenericToDouble(_ys, index);

    double IDataSource.GetYScaled(int index) => Rotated
        ? DataSourceUtilities.ScaleXY(_xs, index, XScale, XOffset)
        : DataSourceUtilities.ScaleXY(_ys, index, YScale, YOffset);

    bool IDataSource.IsSorted() => true;

    /// <summary>Implements binary search for sorted generic coordinate values.</summary>
    private static class BinarySearchHelper
    {
        /// <summary>The divisor used to calculate the midpoint during binary search.</summary>
        private const int BinarySearchMidpointDivisor = 2;

        /// <summary>Searches a sorted list and returns the matching or insertion index.</summary>
        /// <param name="list">The sorted values to search.</param>
        /// <param name="lo">The first index in the inclusive search range.</param>
        /// <param name="hi">The last index in the inclusive search range.</param>
        /// <param name="value">The value to locate.</param>
        /// <returns>The matching index, or the bitwise complement of the insertion index.</returns>
        internal static int BinarySearch(IReadOnlyList<Tx> list, int lo, int hi, Tx value)
        {
            double valueAsDouble = NumericConversion.GenericToDouble(ref value);
            while (lo <= hi)
            {
                int mid = lo + ((hi - lo) / BinarySearchMidpointDivisor); // Avoid overflow from adding lo and hi directly.

                Tx genericX = list[mid];
                double x = NumericConversion.GenericToDouble(ref genericX);
                if (AreExactlyEqual(x, valueAsDouble))
                {
                    return mid;
                }

                if (valueAsDouble > x)
                {
                    lo = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }

            return ~lo; // The complemented insertion index maintains sorted order, like the BCL implementation.
        }

        /// <summary>Determines whether two doubles are exactly equal without introducing a search tolerance.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><see langword="true" /> when the values compare as exactly equal.</returns>
        private static bool AreExactlyEqual(double left, double right) => left >= right && left <= right;
    }
}
