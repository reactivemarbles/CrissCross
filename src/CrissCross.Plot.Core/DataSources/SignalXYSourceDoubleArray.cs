// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace CrissCross.Plot.DataSources;

/// <summary>Provides signal data backed by X and Y coordinate arrays.</summary>
public class SignalXYSourceDoubleArray : ISignalXYSource, IDataSource, IGetNearest
{
    /// <summary>The minimum number of points required to compute extrema.</summary>
    private const int MinimumPointCountForExtrema = 3;

    /// <summary>The fractional overlap added to pixel ranges to avoid missing points.</summary>
    private const double PixelOverlapFraction = 0.01;

    /// <summary>The X coordinates supplied to this source.</summary>
    private readonly double[] _xs;

    /// <summary>The Y coordinates supplied to this source.</summary>
    private readonly double[] _ys;

    /// <summary>Initializes a new instance of the <see cref="SignalXYSourceDoubleArray"/> class from equally sized coordinate arrays.</summary>
    /// <param name="xs">The X coordinates.</param>
    /// <param name="ys">The Y coordinates; must have the same length as <paramref name="xs"/>.</param>
    /// <exception cref="ArgumentException">The arrays have different lengths.</exception>
    public SignalXYSourceDoubleArray(double[] xs, double[] ys)
    {
        if (xs.Length != ys.Length)
        {
            throw new ArgumentException($"{nameof(xs)} and {nameof(ys)} must have equal length");
        }

        _xs = xs;
        _ys = ys;
        MaximumIndex = xs.Length - 1;
    }

    /// <summary>Gets the number of coordinate pairs.</summary>
    public int Count => _xs.Length;

    /// <summary>Gets or sets whether the coordinate axes are rotated.</summary>
    public bool Rotated { get; set; }

    /// <summary>Gets or sets the offset applied to X coordinates.</summary>
    public double XOffset { get; set; }

    /// <summary>Gets or sets the offset applied to Y coordinates.</summary>
    public double YOffset { get; set; }

    /// <summary>Gets or sets the scale applied to Y coordinates.</summary>
    public double YScale { get; set; } = 1;

    /// <summary>Gets or sets the scale applied to X coordinates.</summary>
    public double XScale { get; set; } = 1;

    /// <summary>Gets or sets the first data index included in rendering.</summary>
    public int MinimumIndex { get; set; }

    /// <summary>Gets or sets the last data index included in rendering.</summary>
    public int MaximumIndex { get; set; }

    /// <summary>Gets whether the data source prefers coordinate-based rendering.</summary>
    bool IDataSource.PreferCoordinates => false;

    /// <summary>Gets the number of data points.</summary>
    int IDataSource.Length => _xs.Length;

    /// <summary>Gets the first index included in rendering.</summary>
    int IDataSource.MinRenderIndex => MinimumIndex;

    /// <summary>Gets the last index included in rendering.</summary>
    int IDataSource.MaxRenderIndex => MaximumIndex;

    /// <summary>Gets whether X pixel intervals include a small overlap.</summary>
    public bool UsePixelOverlap { get; }

    /// <summary>Gets the axis limits covering the selected data range.</summary>
    /// <returns>The X and Y limits for the selected data.</returns>
    public AxisLimits GetAxisLimits()
    {
        if (_xs.Length == 0)
        {
            return AxisLimits.NoLimits;
        }

        double minimumX = (_xs[MinimumIndex] * XScale) + XOffset;
        double maximumX = (_xs[MaximumIndex] * XScale) + XOffset;

        CoordinateRange horizontalRange = new(minimumX, maximumX);
        CoordinateRange verticalRange = GetRangeY(MinimumIndex, MaximumIndex);
        return new(horizontalRange, verticalRange);
    }

    /// <summary>Returns the pixels representing the visible signal.</summary>
    /// <param name="rp">The render pack containing the data rectangle.</param>
    /// <param name="axes">The axes used to convert coordinates to pixels.</param>
    /// <param name="connectStyle">The line-connection style used at the visible edges.</param>
    /// <returns>The pixels to draw in rendering order.</returns>
    public IReadOnlyList<Pixel> GetPixelsToDraw(RenderPack rp, IAxes axes, ConnectStyle connectStyle) =>
        Rotated
            ? GetPixelsToDrawVertically(rp, axes, connectStyle)
            : GetPixelsToDrawHorizontally(rp, axes, connectStyle);

    /// <summary>Finds the nearest data point to a coordinate.</summary>
    /// <param name="location">The coordinate to search from.</param>
    /// <param name="renderInfo">The current render details.</param>
    /// <param name="maxDistance">The maximum search distance in pixels.</param>
    /// <returns>The nearest data point within the search distance.</returns>
    public DataPoint GetNearest(Coordinates location, RenderDetails renderInfo, float maxDistance = 15) =>
        DataSourceUtilities.GetNearestFast(this, location, renderInfo, maxDistance);

    /// <summary>Finds the nearest data point by horizontal coordinate.</summary>
    /// <param name="location">The coordinate to search from.</param>
    /// <param name="renderInfo">The current render details.</param>
    /// <param name="maxDistance">The maximum search distance in pixels.</param>
    /// <returns>The nearest data point within the search distance.</returns>
    public DataPoint GetNearestX(Coordinates location, RenderDetails renderInfo, float maxDistance = 15) =>
        DataSourceUtilities.GetNearestXFast(this, location, renderInfo, maxDistance);

    /// <summary>Collects visible pixels when the data is rendered horizontally.</summary>
    /// <param name="renderPack">The render pack containing the data rectangle.</param>
    /// <param name="axes">The axes used to convert coordinates to pixels.</param>
    /// <param name="connectStyle">The line-connection style used at the visible edges.</param>
    /// <returns>The visible pixels with interpolated edge points.</returns>
    private Pixel[] GetPixelsToDrawHorizontally(RenderPack renderPack, IAxes axes, ConnectStyle connectStyle)
    {
        // determine the range of data in view
        (Pixel[] pointsBefore, int dataIndexFirst) = GetFirstPointX(axes);
        (Pixel[] pointsAfter, int dataIndexLast) = GetLastPointX(axes);
        IndexRange visibleRange = new(dataIndexFirst, dataIndexLast);

        if (visibleRange.IsValid && (_xs[dataIndexFirst] > _xs[dataIndexLast]))
        {
            throw new InvalidDataException("Xs must contain only ascending values. "
                + $"The value at index {dataIndexFirst} ({_xs[dataIndexFirst]}) is greater than the value at index {dataIndexLast} ({_xs[dataIndexLast]})");
        }

        Pixel[] leftOutsidePoint = pointsBefore;
        Pixel[] rightOutsidePoint = pointsAfter;
        if (axes.XAxis.Range.Span < 0)
        {
            leftOutsidePoint = pointsAfter;
            rightOutsidePoint = pointsBefore;
        }

        List<Pixel> visiblePixels = [with(capacity: leftOutsidePoint.Length + rightOutsidePoint.Length)];
        visiblePixels.AddRange(leftOutsidePoint);

        if (visibleRange.Length > 0)
        {
            int pixelColumnCount = (int)Math.Ceiling(renderPack.DataRect.Width);
            for (int pixelColumn = 0; pixelColumn < pixelColumnCount; pixelColumn++)
            {
                visiblePixels.AddRange(GetColumnPixelsX(pixelColumn, visibleRange, renderPack, axes));
            }
        }

        visiblePixels.AddRange(rightOutsidePoint);
        Pixel[] points = visiblePixels.ToArray();

        // use interpolation at the edges to prevent points from going way off the screen
        if (leftOutsidePoint.Length > 0)
        {
            SignalInterpolation.InterpolateBeforeX(renderPack, points, connectStyle);
        }

        if (rightOutsidePoint.Length > 0)
        {
            SignalInterpolation.InterpolateAfterX(renderPack, points, connectStyle);
        }

        return points;
    }

    /// <summary>Collects visible pixels when the data is rendered vertically.</summary>
    /// <param name="renderPack">The render pack containing the data rectangle.</param>
    /// <param name="axes">The axes used to convert coordinates to pixels.</param>
    /// <param name="connectStyle">The line-connection style used at the visible edges.</param>
    /// <returns>The visible pixels with interpolated edge points.</returns>
    private Pixel[] GetPixelsToDrawVertically(RenderPack renderPack, IAxes axes, ConnectStyle connectStyle)
    {
        // determine the range of data in view
        (Pixel[] pointsBefore, int dataIndexFirst) = GetFirstPointY(axes);
        (Pixel[] pointsAfter, int dataIndexLast) = GetLastPointY(axes);
        IndexRange visibleRange = new(dataIndexFirst, dataIndexLast);

        if (visibleRange.IsValid && (_xs[dataIndexFirst] > _xs[dataIndexLast]))
        {
            throw new InvalidDataException("Xs must contain only ascending values. "
                + $"The value at index {dataIndexFirst} ({_xs[dataIndexFirst]}) is greater than the value at index {dataIndexLast} ({_xs[dataIndexLast]})");
        }

        Pixel[] bottomOutsidePoint = pointsBefore;
        Pixel[] topOutsidePoint = pointsAfter;
        if (axes.YAxis.Range.Span < 0)
        {
            bottomOutsidePoint = pointsAfter;
            topOutsidePoint = pointsBefore;
        }

        List<Pixel> visiblePixels = [with(capacity: bottomOutsidePoint.Length + topOutsidePoint.Length)];
        visiblePixels.AddRange(bottomOutsidePoint);

        if (visibleRange.Length > 0)
        {
            int pixelRowCount = (int)Math.Ceiling(renderPack.DataRect.Height);
            for (int pixelRow = 0; pixelRow < pixelRowCount; pixelRow++)
            {
                visiblePixels.AddRange(GetColumnPixelsY(pixelRow, visibleRange, renderPack, axes));
            }
        }

        visiblePixels.AddRange(topOutsidePoint);
        Pixel[] points = visiblePixels.ToArray();

        // use interpolation at the edges to prevent points from going way off the screen
        if (bottomOutsidePoint.Length > 0)
        {
            SignalInterpolation.InterpolateBeforeY(renderPack, points, connectStyle);
        }

        if (topOutsidePoint.Length > 0)
        {
            SignalInterpolation.InterpolateAfterY(renderPack, points, connectStyle);
        }

        return points;
    }

    /// <summary>Returns the Y range covered by the given indices, inclusive.</summary>
    /// <param name="index1">One endpoint of the data range.</param>
    /// <param name="index2">The other endpoint of the data range.</param>
    /// <returns>The scaled Y range.</returns>
    private CoordinateRange GetRangeY(int index1, int index2)
    {
        double min = _ys[index1];
        double max = _ys[index1];

        var minIndex = Math.Min(index1, index2);
        var maxIndex = Math.Max(index1, index2);

        for (int i = minIndex; i <= maxIndex; i++)
        {
            min = Math.Min(_ys[i], min);
            max = Math.Max(_ys[i], max);
        }

        return new((min * YScale) + YOffset, (max * YScale) + YOffset);
    }

    /// <summary>Gets the data index associated with an X position.</summary>
    /// <param name="x">The X position to search for.</param>
    /// <returns>The matching or nearest bounded data index.</returns>
    private int GetIndex(double x)
    {
        IndexRange range = new(MinimumIndex, MaximumIndex);
        return GetIndex(x, range);
    }

    /// <summary>Gets the data index associated with an X position within a range.</summary>
    /// <param name="x">The X position to search for.</param>
    /// <param name="indexRange">The inclusive index range to search.</param>
    /// <returns>The matching or nearest bounded data index.</returns>
    private int GetIndex(double x, IndexRange indexRange)
    {
        var (_, index) = SearchIndex(x, indexRange);
        return index;
    }

    /// <summary>Returns representative signal pixels for one screen column.</summary>
    /// <param name="pixelColumnIndex">The zero-based screen column.</param>
    /// <param name="indexRange">The visible data index range.</param>
    /// <param name="renderPack">The render pack containing the data rectangle.</param>
    /// <param name="axes">The axes used to convert coordinates to pixels.</param>
    /// <returns>Zero, one, or four pixels representing the column.</returns>
    private IEnumerable<Pixel> GetColumnPixelsX(
        int pixelColumnIndex,
        IndexRange indexRange,
        RenderPack renderPack,
        IAxes axes)
    {
        float pixelX = pixelColumnIndex + renderPack.DataRect.Left;
        double unitsPerPixelX = axes.XAxis.Width / renderPack.DataRect.Width;
        double start = axes.XAxis.Min + (unitsPerPixelX * pixelColumnIndex);
        double end = start + unitsPerPixelX;

        if (UsePixelOverlap)
        {
            end += unitsPerPixelX * PixelOverlapFraction;
        }

        var (startIndex, _) = SearchIndex(start, indexRange);
        var (endIndex, _) = SearchIndex(end, indexRange);
        int pointsInRange = Math.Abs(endIndex - startIndex);

        if (pointsInRange == 0)
        {
            yield break;
        }

        int firstIndex = startIndex < endIndex ? startIndex : startIndex - 1;
        int lastIndex = startIndex < endIndex ? endIndex - 1 : endIndex;
        yield return new Pixel(pixelX, axes.GetPixelY((_ys[firstIndex] * YScale) + YOffset)); // enter

        if (pointsInRange >= MinimumPointCountForExtrema)
        {
            CoordinateRange verticalRange = GetRangeY(firstIndex, lastIndex); // YOffset is added in GetRangeY
            if (_ys[firstIndex] > _ys[lastIndex])
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

        yield return new Pixel(pixelX, axes.GetPixelY((_ys[lastIndex] * YScale) + YOffset)); // exit
    }

    /// <summary>Returns representative signal pixels for one screen row.</summary>
    /// <param name="pixelRowIndex">The zero-based screen row, counted upward from the bottom.</param>
    /// <param name="indexRange">The visible data index range.</param>
    /// <param name="renderPack">The render pack containing the data rectangle.</param>
    /// <param name="axes">The axes used to convert coordinates to pixels.</param>
    /// <returns>Zero, one, or four pixels representing the row.</returns>
    private IEnumerable<Pixel> GetColumnPixelsY(
        int pixelRowIndex,
        IndexRange indexRange,
        RenderPack renderPack,
        IAxes axes)
    {
        // here pixelRowIndex will count upwards from the bottom, but pixels are measured from the top of the plot
        float pixelY = renderPack.DataRect.Bottom - pixelRowIndex;
        double unitsPerPixelY = axes.YAxis.Height / renderPack.DataRect.Height;
        double start = axes.YAxis.Min + (unitsPerPixelY * pixelRowIndex);
        double end = start + unitsPerPixelY;

        // add slight overlap to prevent floating point errors from missing points
        double overlap = unitsPerPixelY * PixelOverlapFraction;
        end += overlap;

        var (startIndex, _) = SearchIndex(start, indexRange);
        var (endIndex, _) = SearchIndex(end, indexRange);
        int pointsInRange = Math.Abs(endIndex - startIndex);

        if (pointsInRange == 0)
        {
            yield break;
        }

        int firstIndex = startIndex < endIndex ? startIndex : startIndex - 1;
        int lastIndex = startIndex < endIndex ? endIndex - 1 : endIndex;
        yield return new Pixel(axes.GetPixelX((_ys[firstIndex] * YScale) + YOffset), pixelY); // enter

        if (pointsInRange >= MinimumPointCountForExtrema)
        {
            CoordinateRange verticalRange = GetRangeY(firstIndex, lastIndex);
            if (_ys[firstIndex] > _ys[lastIndex])
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

        yield return new Pixel(axes.GetPixelX((_ys[lastIndex] * YScale) + YOffset), pixelY); // exit
    }

    /// <summary>Finds the closest point before the visible X range.</summary>
    /// <param name="axes">The axes defining the visible range.</param>
    /// <returns>The point before the range, if any, and the first visible index.</returns>
    private (Pixel[] pointBefore, int firstIndex) GetFirstPointX(IAxes axes)
    {
        if (_xs.Length == 1)
        {
            return ([], MinimumIndex);
        }

        var (firstPointIndex, _) = SearchIndex(axes.XAxis.Range.Span > 0 ? axes.XAxis.Min : axes.XAxis.Max); // if axis is reversed first index will on the right limit of the plot

        if (firstPointIndex > MinimumIndex)
        {
            float beforeX = axes.GetPixelX((_xs[firstPointIndex - 1] * XScale) + XOffset);
            float beforeY = axes.GetPixelY((_ys[firstPointIndex - 1] * YScale) + YOffset);
            Pixel beforePoint = new(beforeX, beforeY);
            return ([beforePoint], firstPointIndex);
        }

        return ([], MinimumIndex);
    }

    /// <summary>Finds the closest point before the visible Y range.</summary>
    /// <param name="axes">The axes defining the visible range.</param>
    /// <returns>The point before the range, if any, and the first visible index.</returns>
    private (Pixel[] pointBefore, int firstIndex) GetFirstPointY(IAxes axes)
    {
        if (_xs.Length == 1)
        {
            return ([], MinimumIndex);
        }

        var (firstPointIndex, _) = SearchIndex(axes.YAxis.Range.Span > 0 ? axes.YAxis.Min : axes.YAxis.Max); // if axis is reversed first index will on the top limit of the plot

        if (firstPointIndex > MinimumIndex)
        {
            float beforeY = axes.GetPixelY((_xs[firstPointIndex - 1] * XScale) + XOffset);
            float beforeX = axes.GetPixelX((_ys[firstPointIndex - 1] * YScale) + YOffset);
            Pixel beforePoint = new(beforeX, beforeY);
            return ([beforePoint], firstPointIndex);
        }

        return ([], MinimumIndex);
    }

    /// <summary>Finds the closest point after the visible X range.</summary>
    /// <param name="axes">The axes defining the visible range.</param>
    /// <returns>The point after the range, if any, and the last visible index.</returns>
    private (Pixel[] pointAfter, int lastIndex) GetLastPointX(IAxes axes)
    {
        if (_xs.Length == 1)
        {
            return ([], MaximumIndex);
        }

        var (lastPointIndex, _) = SearchIndex(axes.XAxis.Range.Span > 0 ? axes.XAxis.Max : axes.XAxis.Min); // if axis is reversed last index will on the left limit of the plot

        if (lastPointIndex <= MaximumIndex)
        {
            float afterX = axes.GetPixelX((_xs[lastPointIndex] * XScale) + XOffset);
            float afterY = axes.GetPixelY((_ys[lastPointIndex] * YScale) + YOffset);
            Pixel afterPoint = new(afterX, afterY);
            return ([afterPoint], lastPointIndex - 1);
        }

        return ([], MaximumIndex);
    }

    /// <summary>Finds the closest point after the visible Y range.</summary>
    /// <param name="axes">The axes defining the visible range.</param>
    /// <returns>The point after the range, if any, and the last visible index.</returns>
    private (Pixel[] pointAfter, int lastIndex) GetLastPointY(IAxes axes)
    {
        if (_xs.Length == 1)
        {
            return ([], MaximumIndex);
        }

        var (lastPointIndex, _) = SearchIndex(axes.YAxis.Range.Span > 0 ? axes.YAxis.Max : axes.YAxis.Min); // if axis is reversed last index will on the bottom limit of the plot

        if (lastPointIndex <= MaximumIndex)
        {
            float afterY = axes.GetPixelY((_xs[lastPointIndex] * XScale) + XOffset);
            float afterX = axes.GetPixelX((_ys[lastPointIndex] * YScale) + YOffset);
            Pixel afterPoint = new(afterX, afterY);
            return ([afterPoint], lastPointIndex - 1);
        }

        return ([], MaximumIndex);
    }

    /// <summary>Searches for the index associated with an X position.</summary>
    /// <param name="x">The X position to search for.</param>
    /// <returns>The insertion position and its bounded data index.</returns>
    private (int SearchedPosition, int LimitedIndex) SearchIndex(double x)
    {
        IndexRange range = new(MinimumIndex, MaximumIndex);
        return SearchIndex(x, range);
    }

    /// <summary>Searches for an X position within a bounded index range.</summary>
    /// <param name="x">The X position to search for.</param>
    /// <param name="indexRange">The inclusive index range to search.</param>
    /// <returns>The insertion position and its bounded data index.</returns>
    private (int SearchedPosition, int LimitedIndex) SearchIndex(double x, IndexRange indexRange)
    {
        int index = Array.BinarySearch(_xs, indexRange.Min, indexRange.Length, (x - XOffset) / XScale);

        // When x is not matched, BinarySearch returns a negative value. Bitwise negation gives
        // the position where x would be inserted (the next highest index).
        // Below the minimum, this is index 0. Above the maximum, it is one past the end, so the
        // returned bounded index is clamped to the last array element.
        if (index < 0)
        {
            index = ~index; // read BinarySearch() docs
        }

        return (SearchedPosition: index, LimitedIndex: index > indexRange.Max ? indexRange.Max : index);
    }

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

    int IDataSource.GetXClosestIndex(Coordinates mouseLocation) => Rotated
        ? GetIndex(mouseLocation.Y)
        : GetIndex(mouseLocation.X);

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
}
