// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;

namespace CrissCross.Plot.DataSources;

/// <summary>Stores and prepares ordered coordinates for rendering as a data logger.</summary>
/// <param name="coordinates">The mutable coordinate collection to store and render.</param>
public class DataLoggerSource(List<Coordinates> coordinates)
{
    /// <summary>The fractional overlap used when selecting data within a pixel column.</summary>
    private const double SearchOverlapFraction = 0.01;

    /// <summary>Indicates whether the source has changed since its last render.</summary>
    private volatile bool _hasNewData;

    /// <summary>Indicates whether the source has been rendered.</summary>
    private volatile bool _wasRendered;

    /// <summary>Gets the mutable coordinates stored by this source.</summary>
    public List<Coordinates> Coordinates { get; } = coordinates;

    /// <summary>Gets or sets the horizontal offset applied during rendering.</summary>
    public double XOffset { get; set; }

    /// <summary>Gets or sets the vertical offset applied during rendering.</summary>
    public double YOffset { get; set; }

    /// <summary>Gets or sets the scale applied to vertical coordinate values.</summary>
    public double YScale { get; set; } = 1;

    /// <summary>Gets or sets whether coordinates have changed since the last render.</summary>
    public bool HasNewData
    {
        get => _hasNewData;
        set => _hasNewData = value;
    }

    /// <summary>Gets or sets whether this source has been rendered.</summary>
    public bool WasRendered
    {
        get => _wasRendered;
        set => _wasRendered = value;
    }

    /// <summary>Gets the final coordinate index, or -1 when the source is empty.</summary>
    private int LastIndex => Coordinates.Count - 1;

    /// <summary>Adds a coordinate, requiring its X value to be no less than the previous value.</summary>
    /// <param name="coordinates">The coordinate to append.</param>
    /// <exception cref="ArgumentException">The coordinate's X value is less than the previous X value.</exception>
    public void Add(Coordinates coordinates)
    {
        // Check that X coordinates are in ascending order
        if (Coordinates.Count > 0 && coordinates.X < Coordinates[LastIndex].X)
        {
            throw new ArgumentException("X coordinates must be in ascending order", nameof(coordinates));
        }

        Coordinates.Add(coordinates);

        HasNewData = true;
    }

    /// <summary>Removes all coordinates and marks the source as changed.</summary>
    public void Clear()
    {
        Coordinates.Clear();
        HasNewData = true;
    }

    /// <summary>Marks this source as rendered and clears its changed state.</summary>
    public void OnRendered()
    {
        HasNewData = false;
        WasRendered = true;
    }

    /// <summary>Gets the range of X values, including the horizontal offset.</summary>
    /// <returns>The range of X values, or <see cref="CoordinateRange.NoLimits"/> when no coordinates are stored.</returns>
    /// <remarks>This remains a method to preserve the established public API.</remarks>
    [SuppressMessage(
        "Design",
        "CA1024:Use properties where appropriate",
        Justification = "Changing this established public method to a property would break source and binary compatibility.")]
    public CoordinateRange GetRangeX() => Coordinates.Count == 0
        ? CoordinateRange.NoLimits
        : new CoordinateRange(Coordinates[0].X + XOffset, Coordinates[^1].X + XOffset);

    /// <summary>Gets the vertical range of coordinates whose X values fall within the specified range.</summary>
    /// <param name="newRangeX">The X range used to select coordinates.</param>
    /// <returns>The scaled and offset Y range of the selected coordinates, or <see cref="CoordinateRange.NoLimits"/> when no coordinates are stored.</returns>
    public CoordinateRange GetRangeY(CoordinateRange newRangeX)
    {
        if (Coordinates.Count == 0)
        {
            return CoordinateRange.NoLimits;
        }

        int startIndex = GetIndex(newRangeX.Min);
        int endIndex = GetIndex(newRangeX.Max);

        return GetRangeY(startIndex, endIndex);
    }

    /// <summary>Gets the pixels representing the data source when rendered along the horizontal axis.</summary>
    /// <param name="rp">The render pack containing the data rectangle.</param>
    /// <param name="axes">The axes used to transform coordinates to pixels.</param>
    /// <param name="connectStyle">The style used to interpolate at the visible range boundaries.</param>
    /// <returns>The pixels to draw in rendering order.</returns>
    public Pixel[] GetPixelsToDrawHorizontally(RenderPack rp, IAxes axes, ConnectStyle connectStyle)
    {
        if (Coordinates.Count == 0)
        {
            return [];
        }

        // determine the range of data in view
        (Pixel[] pointBefore, int dataIndexFirst) = GetFirstPointX(axes);
        (Pixel[] pointAfter, int dataIndexLast) = GetLastPointX(axes);
        IndexRange visibleRange = new(dataIndexFirst, dataIndexLast);

        // get all points in view
        List<Pixel> visiblePoints = [];
        int columnCount = (int)Math.Ceiling(rp.DataRect.Width);
        for (int pixelColumn = 0; pixelColumn < columnCount; pixelColumn++)
        {
            visiblePoints.AddRange(GetColumnPixelsX(pixelColumn, visibleRange, rp, axes));
        }

        Pixel[] leftOutsidePoint = pointBefore;
        Pixel[] rightOutsidePoint = pointAfter;
        if (axes.XAxis.Range.Span < 0)
        {
            leftOutsidePoint = pointAfter;
            rightOutsidePoint = pointBefore;
        }

        // duplicate the last point to ensure it is always rendered
        int lastPointIndex = axes.XAxis.IsInverted() ? dataIndexFirst : dataIndexLast;
        Pixel lastPoint = axes.GetPixel(Coordinates[lastPointIndex]);

        // combine with one extra point before and after
        Pixel[] points = [.. leftOutsidePoint, .. visiblePoints, .. rightOutsidePoint, lastPoint];

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

    /// <summary>Gets the pixels representing the data source when rendered along the vertical axis.</summary>
    /// <param name="rp">The render pack containing the data rectangle.</param>
    /// <param name="axes">The axes used to transform coordinates to pixels.</param>
    /// <param name="connectStyle">The style used to interpolate at the visible range boundaries.</param>
    /// <returns>The pixels to draw in rendering order.</returns>
    public Pixel[] GetPixelsToDrawVertically(RenderPack rp, IAxes axes, ConnectStyle connectStyle)
    {
        if (Coordinates.Count == 0)
        {
            return [];
        }

        // determine the range of data in view
        (Pixel[] pointBefore, int dataIndexFirst) = GetFirstPointY(axes);
        (Pixel[] pointAfter, int dataIndexLast) = GetLastPointY(axes);
        IndexRange visibleRange = new(dataIndexFirst, dataIndexLast);

        // get all points in view
        List<Pixel> visiblePoints = [];
        int rowCount = (int)Math.Ceiling(rp.DataRect.Height);
        for (int pixelRow = 0; pixelRow < rowCount; pixelRow++)
        {
            visiblePoints.AddRange(GetColumnPixelsY(pixelRow, visibleRange, rp, axes));
        }

        Pixel[] bottomOutsidePoint = pointBefore;
        Pixel[] topOutsidePoint = pointAfter;
        if (axes.YAxis.Range.Span < 0)
        {
            bottomOutsidePoint = pointAfter;
            topOutsidePoint = pointBefore;
        }

        // duplicate the last point to ensure it is always rendered
        int lastPointIndex = axes.YAxis.IsInverted() ? dataIndexFirst : dataIndexLast;
        Pixel lastPoint = axes.GetPixel(new(Coordinates[lastPointIndex].Y, Coordinates[lastPointIndex].X));

        // combine with one extra point before and after
        Pixel[] points = [.. bottomOutsidePoint, .. visiblePoints, .. topOutsidePoint, lastPoint];

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

    /// <summary>Gets the index associated with the specified X position.</summary>
    /// <param name="x">The X value to locate.</param>
    /// <returns>The nearest index at or following the requested X position, limited to stored coordinates.</returns>
    public int GetIndex(double x)
    {
        IndexRange range = new(0, LastIndex);
        return GetIndex(x, range);
    }

    /// <summary>Gets the vertical range covered by data between the specified indices, inclusive.</summary>
    /// <param name="index1">The first endpoint index.</param>
    /// <param name="index2">The second endpoint index.</param>
    /// <returns>The scaled and offset vertical range.</returns>
    private CoordinateRange GetRangeY(int index1, int index2)
    {
        double min = Coordinates[index1].Y;
        double max = Coordinates[index1].Y;

        var minIndex = Math.Min(index1, index2);
        var maxIndex = Math.Max(index1, index2);

        for (int i = minIndex; i <= maxIndex; i++)
        {
            min = Math.Min(Coordinates[i].Y, min);
            max = Math.Max(Coordinates[i].Y, max);
        }

        return new((min * YScale) + YOffset, (max * YScale) + YOffset);
    }

    /// <summary>Gets the index associated with the specified X position, limited to the given range.</summary>
    /// <param name="x">The X value to locate.</param>
    /// <param name="indexRange">The index range to search.</param>
    /// <returns>The nearest index at or following the requested X position, limited to the range.</returns>
    private int GetIndex(double x, IndexRange indexRange)
    {
        var (_, index) = SearchIndex(x, indexRange);
        return index;
    }

    /// <summary>
    /// Given a pixel column, return the pixels to render its line.
    /// If the column contains no data, no pixels are returned.
    /// If the column contains one point, return that one pixel.
    /// If the column contains multiple points, return 4 pixels: enter, min, max, and exit.
    /// </summary>
    /// <summary>Gets the representative pixels for data intersecting a pixel column.</summary>
    /// <param name="pixelColumnIndex">The zero-based pixel column.</param>
    /// <param name="rng">The index range of visible coordinates.</param>
    /// <param name="rp">The render pack containing the data rectangle.</param>
    /// <param name="axes">The axes used to transform coordinates to pixels.</param>
    /// <returns>The entry, extrema, and exit pixels for the column, when present.</returns>
    private IEnumerable<Pixel> GetColumnPixelsX(int pixelColumnIndex, IndexRange rng, RenderPack rp, IAxes axes)
    {
        float pixelX = pixelColumnIndex + rp.DataRect.Left;
        double unitsPerPixelX = axes.XAxis.Width / rp.DataRect.Width;
        double start = axes.XAxis.Min + (unitsPerPixelX * pixelColumnIndex);
        double end = start + unitsPerPixelX;

        // add slight overlap to prevent floating point errors from missing points
        double overlap = unitsPerPixelX * SearchOverlapFraction;
        end += overlap;

        var (startPosition, startIndex) = SearchIndex(start, rng);
        var (endPosition, endIndex) = SearchIndex(end, rng);
        int pointsInRange = Math.Abs(endPosition - startPosition);

        if (pointsInRange == 0)
        {
            yield break;
        }

        yield return new Pixel(pixelX, axes.GetPixelY((Coordinates[startIndex].Y * YScale) + YOffset)); // enter

        if (pointsInRange <= 1)
        {
            yield break;
        }

        int lastIndex = startIndex < endIndex ? endIndex - 1 : endIndex + 1;
        CoordinateRange verticalRange = GetRangeY(startIndex, lastIndex); // YOffset is added in GetRangeY.
        yield return new Pixel(pixelX, axes.GetPixelY(verticalRange.Min)); // min
        yield return new Pixel(pixelX, axes.GetPixelY(verticalRange.Max)); // max
        yield return new Pixel(pixelX, axes.GetPixelY((Coordinates[lastIndex].Y * YScale) + YOffset)); // exit
    }

    /// <summary>
    /// Given a pixel column, return the pixels to render its line.
    /// If the column contains no data, no pixels are returned.
    /// If the column contains one point, return that one pixel.
    /// If the column contains multiple points, return 4 pixels: enter, min, max, and exit.
    /// </summary>
    /// <summary>Gets the representative pixels for data intersecting a pixel row.</summary>
    /// <param name="rowColumnIndex">The zero-based pixel row.</param>
    /// <param name="rng">The index range of visible coordinates.</param>
    /// <param name="rp">The render pack containing the data rectangle.</param>
    /// <param name="axes">The axes used to transform coordinates to pixels.</param>
    /// <returns>The entry, extrema, and exit pixels for the row, when present.</returns>
    private IEnumerable<Pixel> GetColumnPixelsY(int rowColumnIndex, IndexRange rng, RenderPack rp, IAxes axes)
    {
        float pixelY = rp.DataRect.Bottom - rowColumnIndex;
        double unitsPerPixelY = axes.YAxis.Height / rp.DataRect.Height;
        double start = axes.YAxis.Min + (unitsPerPixelY * rowColumnIndex);
        double end = start + unitsPerPixelY;
        var (startPosition, startIndex) = SearchIndex(start, rng);
        var (endPosition, endIndex) = SearchIndex(end, rng);
        int pointsInRange = Math.Abs(endPosition - startPosition);

        if (pointsInRange == 0)
        {
            yield break;
        }

        yield return new Pixel(axes.GetPixelX(Coordinates[startIndex].Y + XOffset), pixelY); // enter

        if (pointsInRange <= 1)
        {
            yield break;
        }

        int lastIndex = startIndex < endIndex ? endIndex - 1 : endIndex + 1;
        CoordinateRange verticalRange = GetRangeY(startIndex, lastIndex);
        yield return new Pixel(axes.GetPixelX(verticalRange.Min), pixelY); // min
        yield return new Pixel(axes.GetPixelX(verticalRange.Max), pixelY); // max
        yield return new Pixel(axes.GetPixelX(Coordinates[lastIndex].Y + XOffset), pixelY); // exit
    }

    /// <summary>Gets a point immediately before the visible X range, if one exists.</summary>
    /// <param name="axes">The axes defining the visible range and pixel transformation.</param>
    /// <returns>The optional preceding pixel and the first visible coordinate index.</returns>
    private (Pixel[] pointsBefore, int firstIndex) GetFirstPointX(IAxes axes)
    {
        if (Coordinates.Count == 1)
        {
            return ([], 0);
        }

        var (firstPointPosition, firstPointIndex) = SearchIndex(axes.XAxis.Range.Span > 0 ? axes.XAxis.Min : axes.XAxis.Max); // if axis is reversed first index will on the right limit of the plot

        if (firstPointPosition <= 0)
        {
            return ([], 0);
        }

        float beforeX = axes.GetPixelX(Coordinates[firstPointIndex - 1].X + XOffset);
        float beforeY = axes.GetPixelY((Coordinates[firstPointIndex - 1].Y * YScale) + YOffset);
        Pixel beforePoint = new(beforeX, beforeY);
        return ([beforePoint], firstPointIndex);
    }

    /// <summary>Gets a point immediately before the visible Y range, if one exists.</summary>
    /// <param name="axes">The axes defining the visible range and pixel transformation.</param>
    /// <returns>The optional preceding pixel and the first visible coordinate index.</returns>
    private (Pixel[] pointsBefore, int firstIndex) GetFirstPointY(IAxes axes)
    {
        if (Coordinates.Count == 1)
        {
            return ([], 0);
        }

        var (firstPointPosition, firstPointIndex) = SearchIndex(axes.YAxis.Range.Span > 0 ? axes.YAxis.Min : axes.YAxis.Max); // if axis is reversed first index will on the top limit of the plot

        if (firstPointPosition <= 0)
        {
            return ([], 0);
        }

        float beforeY = axes.GetPixelY(Coordinates[firstPointIndex - 1].X + XOffset);
        float beforeX = axes.GetPixelX((Coordinates[firstPointIndex - 1].Y * YScale) + YOffset);
        Pixel beforePoint = new(beforeX, beforeY);
        return ([beforePoint], firstPointIndex);
    }

    /// <summary>Gets a point immediately after the visible X range, if one exists.</summary>
    /// <param name="axes">The axes defining the visible range and pixel transformation.</param>
    /// <returns>The optional following pixel and the last visible coordinate index.</returns>
    private (Pixel[] pointsAfter, int lastIndex) GetLastPointX(IAxes axes)
    {
        if (Coordinates.Count == 1)
        {
            return ([], LastIndex);
        }

        var (lastPointPosition, lastPointIndex) = SearchIndex(axes.XAxis.Range.Span > 0 ? axes.XAxis.Max : axes.XAxis.Min); // if axis is reversed last index will on the left limit of the plot

        if (lastPointPosition > LastIndex)
        {
            return ([], LastIndex);
        }

        float afterX = axes.GetPixelX(Coordinates[lastPointIndex].X + XOffset);
        float afterY = axes.GetPixelY((Coordinates[lastPointIndex].Y * YScale) + YOffset);
        Pixel afterPoint = new(afterX, afterY);
        return ([afterPoint], lastPointIndex);
    }

    /// <summary>Gets a point immediately after the visible Y range, if one exists.</summary>
    /// <param name="axes">The axes defining the visible range and pixel transformation.</param>
    /// <returns>The optional following pixel and the last visible coordinate index.</returns>
    private (Pixel[] pointsAfter, int lastIndex) GetLastPointY(IAxes axes)
    {
        if (Coordinates.Count == 1)
        {
            return ([], LastIndex);
        }

        var (lastPointPosition, lastPointIndex) = SearchIndex(axes.YAxis.Range.Span > 0 ? axes.YAxis.Max : axes.YAxis.Min); // if axis is reversed last index will on the bottom limit of the plot

        if (lastPointPosition > LastIndex)
        {
            return ([], LastIndex);
        }

        float afterY = axes.GetPixelY(Coordinates[lastPointIndex].X + XOffset);
        float afterX = axes.GetPixelX((Coordinates[lastPointIndex].Y * YScale) + YOffset);
        Pixel afterPoint = new(afterX, afterY);
        return ([afterPoint], lastPointIndex);
    }

    /// <summary>Searches for the insertion position of the specified X value.</summary>
    /// <param name="x">The X value to locate.</param>
    /// <returns>The insertion position and its corresponding bounded coordinate index.</returns>
    private (int SearchedPosition, int LimitedIndex) SearchIndex(double x)
    {
        IndexRange range = new(0, LastIndex);
        return SearchIndex(x, range);
    }

    /// <summary>Searches for the insertion position of the specified X value in a bounded range.</summary>
    /// <param name="x">The X value to locate.</param>
    /// <param name="indexRange">The index range to search.</param>
    /// <returns>The insertion position and its corresponding bounded coordinate index.</returns>
    private (int SearchedPosition, int LimitedIndex) SearchIndex(double x, IndexRange indexRange)
    {
        int index = Coordinates.BinarySearch(indexRange.Min, indexRange.Length, new(x - XOffset, 0), BinarySearchComparer.Instance);

        // A negative result encodes the insertion point; bitwise negation recovers it.
        // Insertion beyond the bounded range is clamped to its final index below.
        if (index < 0)
        {
            index = ~index; // read BinarySearch() docs
        }

        return (SearchedPosition: index, LimitedIndex: index > indexRange.Max ? indexRange.Max : index);
    }
}
