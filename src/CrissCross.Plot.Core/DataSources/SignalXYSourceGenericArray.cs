// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Provides a signal data source backed by generic X and Y arrays.</summary>
/// <typeparam name="TX">The type of the X values.</typeparam>
/// <typeparam name="TY">The type of the Y values.</typeparam>
public class SignalXYSourceGenericArray<TX, TY> : ISignalXYSource, IDataSource, IGetNearest
{
    /// <summary>The fraction of a pixel used to overlap adjacent render ranges.</summary>
    private const double PixelOverlapFraction = 0.01;

    /// <summary>The point count that separates a line segment from an extrema range.</summary>
    private const int MinimumPointCountForExtrema = 2;

    /// <summary>Initializes a new instance of the <see cref="SignalXYSourceGenericArray{TX, TY}"/> class.</summary>
    /// <param name="xs">The array of X values.</param>
    /// <param name="ys">The array of Y values.</param>
    /// <exception cref="ArgumentException">The arrays have different lengths.</exception>
    public SignalXYSourceGenericArray(TX[] xs, TY[] ys)
    {
        if (xs.Length != ys.Length)
        {
            throw new ArgumentException($"{nameof(xs)} and {nameof(ys)} must have equal length");
        }

        Xs = xs;
        Ys = ys;
        MaximumIndex = xs.Length - 1;
    }

    /// <summary>Gets or sets the X values.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2305", Justification = "The replaceable array setter is part of the established public API.")]
    public TX[] Xs { get; set; }

    /// <summary>Gets or sets the Y values.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2305", Justification = "The replaceable array setter is part of the established public API.")]
    public TY[] Ys { get; set; }

    /// <summary>Gets the number of data points.</summary>
    public int Count => Xs.Length;

    /// <summary>Gets or sets whether the data is rendered vertically.</summary>
    public bool Rotated { get; set; }

    /// <summary>Gets or sets the offset applied to X values.</summary>
    public double XOffset { get; set; }

    /// <summary>Gets or sets the offset applied to Y values.</summary>
    public double YOffset { get; set; }

    /// <summary>Gets or sets the scale applied to Y values.</summary>
    public double YScale { get; set; } = 1;

    /// <summary>Gets or sets the scale applied to X values.</summary>
    public double XScale { get; set; } = 1;

    /// <summary>Gets or sets the first data index included in rendering.</summary>
    public int MinimumIndex { get; set; }

    /// <summary>Gets or sets the last data index included in rendering.</summary>
    public int MaximumIndex { get; set; }

    bool IDataSource.PreferCoordinates => false;

    int IDataSource.Length => Xs.Length;

    int IDataSource.MinRenderIndex => MinimumIndex;

    int IDataSource.MaxRenderIndex => MaximumIndex;

    /// <summary>Gets whether pixel ranges overlap slightly to avoid gaps at their edges.</summary>
    public bool UsePixelOverlap { get; }

    /// <summary>Gets the axis limits covered by the current data range.</summary>
    /// <returns>The limits of the X and Y data.</returns>
    public AxisLimits GetAxisLimits()
    {
        if (Xs.Length == 0)
        {
            return AxisLimits.NoLimits;
        }

        double horizontalMinimum = (NumericConversion.GenericToDouble(Xs, MinimumIndex) * XScale) + XOffset;
        double horizontalMaximum = (NumericConversion.GenericToDouble(Xs, MaximumIndex) * XScale) + XOffset;
        CoordinateRange horizontalRange = new(horizontalMinimum, horizontalMaximum);
        CoordinateRange verticalRange = GetRangeY(MinimumIndex, MaximumIndex);
        return new(horizontalRange, verticalRange);
    }

    /// <summary>Gets the pixels used to draw the signal.</summary>
    /// <param name="rp">The render pack.</param>
    /// <param name="axes">The axes used to transform data coordinates.</param>
    /// <param name="connectStyle">The style used to connect data points.</param>
    /// <returns>The pixels to draw.</returns>
    public IReadOnlyList<Pixel> GetPixelsToDraw(RenderPack rp, IAxes axes, ConnectStyle connectStyle) =>
        Rotated
            ? GetPixelsToDrawVertically(rp, axes, connectStyle)
            : GetPixelsToDrawHorizontally(rp, axes, connectStyle);

    /// <summary>Gets the pixels used to render the signal horizontally.</summary>
    /// <param name="rp">The render pack containing the data rectangle.</param>
    /// <param name="axes">The axes used to transform coordinates to pixels.</param>
    /// <param name="connectStyle">The style used to connect data points.</param>
    /// <returns>The pixels to draw.</returns>
    public Pixel[] GetPixelsToDrawHorizontally(RenderPack rp, IAxes axes, ConnectStyle connectStyle)
    {
        // determine the range of data in view
        (Pixel[] PointBefore, int dataIndexFirst) = GetFirstPointX(axes);
        (Pixel[] PointAfter, int dataIndexLast) = GetLastPointX(axes);
        IndexRange visibleRange = new(dataIndexFirst, dataIndexLast);

        if (visibleRange.IsValid && NumericConversion.GenericToDouble(Xs, dataIndexFirst) > NumericConversion.GenericToDouble(Xs, dataIndexLast))
        {
            throw new InvalidDataException("Xs must contain only ascending values. "
                + $"The value at index {dataIndexFirst} ({Xs[dataIndexFirst]}) is greater than the value at index {dataIndexLast} ({Xs[dataIndexLast]})");
        }

        // get all points in view
        List<Pixel> visiblePixelList = [];
        if (visibleRange.Length > 0)
        {
            int pixelColumnCount = (int)Math.Ceiling(rp.DataRect.Width);
            for (int pixelColumnIndex = 0; pixelColumnIndex < pixelColumnCount; pixelColumnIndex++)
            {
                visiblePixelList.AddRange(GetColumnPixelsX(pixelColumnIndex, visibleRange, rp, axes));
            }
        }

        Pixel[] visiblePoints = [.. visiblePixelList];

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

    /// <summary>Gets the pixels used to render the signal vertically.</summary>
    /// <param name="rp">The render pack containing the data rectangle.</param>
    /// <param name="axes">The axes used to transform coordinates to pixels.</param>
    /// <param name="connectStyle">The style used to connect data points.</param>
    /// <returns>The pixels to draw.</returns>
    public Pixel[] GetPixelsToDrawVertically(RenderPack rp, IAxes axes, ConnectStyle connectStyle)
    {
        // determine the range of data in view
        (Pixel[] PointBefore, int dataIndexFirst) = GetFirstPointY(axes);
        (Pixel[] PointAfter, int dataIndexLast) = GetLastPointY(axes);
        IndexRange visibleRange = new(dataIndexFirst, dataIndexLast);

        if (visibleRange.IsValid && NumericConversion.GenericToDouble(Xs, dataIndexFirst) > NumericConversion.GenericToDouble(Xs, dataIndexLast))
        {
            throw new InvalidDataException("Xs must contain only ascending values. "
                + $"The value at index {dataIndexFirst} ({Xs[dataIndexFirst]}) is greater than the value at index {dataIndexLast} ({Xs[dataIndexLast]})");
        }

        // get all points in view
        List<Pixel> visiblePixelList = [];
        if (visibleRange.Length > 0)
        {
            int pixelRowCount = (int)Math.Ceiling(rp.DataRect.Height);
            for (int pixelRowIndex = 0; pixelRowIndex < pixelRowCount; pixelRowIndex++)
            {
                visiblePixelList.AddRange(GetColumnPixelsY(pixelRowIndex, visibleRange, rp, axes));
            }
        }

        Pixel[] visiblePoints = [.. visiblePixelList];

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

    /// <summary>Returns the Y range covered by the specified data indices.</summary>
    /// <param name="index1">The first data index.</param>
    /// <param name="index2">The last data index.</param>
    /// <returns>The minimum and maximum Y coordinates.</returns>
    public CoordinateRange GetRangeY(int index1, int index2)
    {
        double min = NumericConversion.GenericToDouble(Ys, index1);
        double max = NumericConversion.GenericToDouble(Ys, index1);

        var minIndex = Math.Min(index1, index2);
        var maxIndex = Math.Max(index1, index2);

        for (int i = minIndex; i <= maxIndex; i++)
        {
            double value = NumericConversion.GenericToDouble(Ys, i);
            min = Math.Min(value, min);
            max = Math.Max(value, max);
        }

        return new((min * YScale) + YOffset, (max * YScale) + YOffset);
    }

    /// <summary>Gets the index associated with the given X position.</summary>
    /// <param name="x">The X position.</param>
    /// <returns>The associated data index.</returns>
    public int GetIndex(double x)
    {
        IndexRange range = new(MinimumIndex, MaximumIndex);
        return GetIndex(x, range);
    }

    /// <summary>Gets the index associated with the given X position within a range.</summary>
    /// <param name="x">The X position.</param>
    /// <param name="indexRange">The data index range to search.</param>
    /// <returns>The associated data index.</returns>
    public int GetIndex(double x, IndexRange indexRange)
    {
        var (_, index) = SearchIndex(x, indexRange);
        return index;
    }

    /// <summary>Returns line pixels for data within the specified horizontal pixel column.</summary>
    /// <param name="pixelColumnIndex">The horizontal pixel column.</param>
    /// <param name="rng">The visible data index range.</param>
    /// <param name="rp">The render pack containing the data rectangle.</param>
    /// <param name="axes">The axes used to transform coordinates to pixels.</param>
    /// <returns>Zero pixels for an empty column, one pixel for a single point, or entry/extrema/exit pixels otherwise.</returns>
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
        double startCoordinateY = (NumericConversion.GenericToDouble(Ys, firstIndex) * YScale) + YOffset;
        double endCoordinateY = (NumericConversion.GenericToDouble(Ys, lastIndex) * YScale) + YOffset;

        yield return new Pixel(pixelX, axes.GetPixelY(startCoordinateY)); // enter

        if (pointsInRange > MinimumPointCountForExtrema)
        {
            CoordinateRange verticalRange = GetRangeY(firstIndex, lastIndex); // YOffset is added in GetRangeY

            if (startCoordinateY > endCoordinateY)
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

        yield return new Pixel(pixelX, axes.GetPixelY(endCoordinateY)); // exit
    }

    /// <summary>Returns line pixels for data within the specified vertical pixel row.</summary>
    /// <param name="pixelColumnIndex">The vertical pixel row.</param>
    /// <param name="rng">The visible data index range.</param>
    /// <param name="rp">The render pack containing the data rectangle.</param>
    /// <param name="axes">The axes used to transform coordinates to pixels.</param>
    /// <returns>Zero pixels for an empty row, one pixel for a single point, or entry/extrema/exit pixels otherwise.</returns>
    public IEnumerable<Pixel> GetColumnPixelsY(int pixelColumnIndex, IndexRange rng, RenderPack rp, IAxes axes)
    {
        // here rowColumnIndex will count upwards from the bottom, but pixels are measured from the top of the plot
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
        double startCoordinateY = (NumericConversion.GenericToDouble(Ys, firstIndex) * YScale) + YOffset;
        double endCoordinateY = (NumericConversion.GenericToDouble(Ys, lastIndex) * YScale) + YOffset;

        yield return new Pixel(axes.GetPixelX(startCoordinateY), pixelY); // enter

        if (pointsInRange > MinimumPointCountForExtrema)
        {
            CoordinateRange verticalRange = GetRangeY(firstIndex, lastIndex); // YOffset is added in GetRangeY

            if (startCoordinateY > endCoordinateY)
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

        yield return new Pixel(axes.GetPixelX(endCoordinateY), pixelY); // exit
    }

    /// <summary>Gets the nearest data point to the specified location.</summary>
    /// <param name="location">The coordinate to search.</param>
    /// <param name="renderInfo">Information about the rendered data.</param>
    /// <param name="maxDistance">The maximum distance from the location.</param>
    /// <returns>The nearest data point.</returns>
    public DataPoint GetNearest(Coordinates location, RenderDetails renderInfo, float maxDistance = 15) =>
        DataSourceUtilities.GetNearestFast(this, location, renderInfo, maxDistance);

    /// <summary>Gets the nearest data point by X coordinate to the specified location.</summary>
    /// <param name="location">The coordinate to search.</param>
    /// <param name="renderInfo">Information about the rendered data.</param>
    /// <param name="maxDistance">The maximum distance from the location.</param>
    /// <returns>The nearest data point.</returns>
    public DataPoint GetNearestX(Coordinates location, RenderDetails renderInfo, float maxDistance = 15) =>
        DataSourceUtilities.GetNearestXFast(this, location, renderInfo, maxDistance);

    /// <summary>Searches a sorted array and returns the matching or insertion index.</summary>
    /// <param name="array">The sorted array to search.</param>
    /// <param name="index">The first index in the search range.</param>
    /// <param name="length">The number of elements in the search range.</param>
    /// <param name="value">The value to locate.</param>
    /// <returns>The matching index, or the bitwise complement of the insertion index.</returns>
    internal static int BinarySearch(TX[] array, int index, int length, double value)
    {
        int lo = index;
        int hi = index + length - 1;
        while (lo <= hi)
        {
            int i = lo + ((hi - lo) >> 1);
            int order = NumericConversion.GenericToDouble(array, i).CompareTo(value);

            if (order == 0)
            {
                return i;
            }

            if (order < 0)
            {
                lo = i + 1;
            }
            else
            {
                hi = i - 1;
            }
        }

        return ~lo;
    }

    /// <summary>Gets the data point immediately before the visible horizontal range, if one exists.</summary>
    /// <param name="axes">The axes used to locate the visible range.</param>
    /// <returns>The off-screen point and its following data index, or an empty array and minimum index.</returns>
    private (Pixel[] pointBefore, int firstIndex) GetFirstPointX(IAxes axes)
    {
        if (Xs.Length == 1)
        {
            return ([], MinimumIndex);
        }

        var (firstPointIndex, _) = SearchIndex(axes.XAxis.Range.Span > 0 ? axes.XAxis.Min : axes.XAxis.Max); // if axis is reversed first index will on the right limit of the plot

        if (firstPointIndex > MinimumIndex)
        {
            double x = (NumericConversion.GenericToDouble(Xs, firstPointIndex - 1) * XScale) + XOffset;
            double y = (NumericConversion.GenericToDouble(Ys, firstPointIndex - 1) * YScale) + YOffset;
            float beforeX = axes.GetPixelX(x);
            float beforeY = axes.GetPixelY(y);
            Pixel beforePoint = new(beforeX, beforeY);
            return ([beforePoint], firstPointIndex);
        }

        return ([], MinimumIndex);
    }

    /// <summary>Gets the data point immediately before the visible vertical range, if one exists.</summary>
    /// <param name="axes">The axes used to locate the visible range.</param>
    /// <returns>The off-screen point and its following data index, or an empty array and minimum index.</returns>
    private (Pixel[] pointBefore, int firstIndex) GetFirstPointY(IAxes axes)
    {
        if (Xs.Length == 1)
        {
            return ([], MinimumIndex);
        }

        var (firstPointIndex, _) = SearchIndex(axes.YAxis.Range.Span > 0 ? axes.YAxis.Min : axes.YAxis.Max); // if axis is reversed first index will on the top limit of the plot

        if (firstPointIndex > MinimumIndex)
        {
            double x = (NumericConversion.GenericToDouble(Xs, firstPointIndex - 1) * XScale) + XOffset;
            double y = (NumericConversion.GenericToDouble(Ys, firstPointIndex - 1) * YScale) + YOffset;
            float beforeX = axes.GetPixelX(y);
            float beforeY = axes.GetPixelY(x);
            Pixel beforePoint = new(beforeX, beforeY);
            return ([beforePoint], firstPointIndex);
        }

        return ([], MinimumIndex);
    }

    /// <summary>Gets the data point immediately after the visible horizontal range, if one exists.</summary>
    /// <param name="axes">The axes used to locate the visible range.</param>
    /// <returns>The off-screen point and its preceding data index, or an empty array and maximum index.</returns>
    private (Pixel[] pointAfter, int lastIndex) GetLastPointX(IAxes axes)
    {
        if (Xs.Length == 1)
        {
            return ([], MaximumIndex);
        }

        var (lastPointIndex, _) = SearchIndex(axes.XAxis.Range.Span > 0 ? axes.XAxis.Max : axes.XAxis.Min); // if axis is reversed last index will on the left limit of the plot

        if (lastPointIndex <= MaximumIndex)
        {
            double x = (NumericConversion.GenericToDouble(Xs, lastPointIndex) * XScale) + XOffset;
            double y = (NumericConversion.GenericToDouble(Ys, lastPointIndex) * YScale) + YOffset;
            float afterX = axes.GetPixelX(x);
            float afterY = axes.GetPixelY(y);
            Pixel afterPoint = new(afterX, afterY);
            return ([afterPoint], lastPointIndex - 1);
        }

        return ([], MaximumIndex);
    }

    /// <summary>Gets the data point immediately after the visible vertical range, if one exists.</summary>
    /// <param name="axes">The axes used to locate the visible range.</param>
    /// <returns>The off-screen point and its preceding data index, or an empty array and maximum index.</returns>
    private (Pixel[] pointAfter, int lastIndex) GetLastPointY(IAxes axes)
    {
        if (Xs.Length == 1)
        {
            return ([], MaximumIndex);
        }

        var (lastPointIndex, _) = SearchIndex(axes.YAxis.Range.Span > 0 ? axes.YAxis.Max : axes.YAxis.Min); // if axis is reversed last index will on the bottom limit of the plot

        if (lastPointIndex <= MaximumIndex)
        {
            double x = (NumericConversion.GenericToDouble(Xs, lastPointIndex) * XScale) + XOffset;
            double y = (NumericConversion.GenericToDouble(Ys, lastPointIndex) * YScale) + YOffset;
            float afterX = axes.GetPixelX(y);
            float afterY = axes.GetPixelY(x);
            Pixel afterPoint = new(afterX, afterY);
            return ([afterPoint], lastPointIndex - 1);
        }

        return ([], MaximumIndex);
    }

    /// <summary>Finds the index associated with the specified X position.</summary>
    /// <param name="x">The X position to search.</param>
    /// <returns>The insertion index and its value limited to the configured data range.</returns>
    private (int SearchedPosition, int LimitedIndex) SearchIndex(double x)
    {
        IndexRange range = new(MinimumIndex, MaximumIndex);
        return SearchIndex(x, range);
    }

    /// <summary>Finds the index associated with the specified X position within a given range.</summary>
    /// <param name="x">The X position to search.</param>
    /// <param name="indexRange">The inclusive index range to search.</param>
    /// <returns>The insertion index and its value limited to the specified range.</returns>
    private (int SearchedPosition, int LimitedIndex) SearchIndex(double x, IndexRange indexRange)
    {
        int index = BinarySearch(Xs, indexRange.Min, indexRange.Length, (x - XOffset) / XScale);

        // A negative result is the bitwise complement of the insertion position.
        // Clamp insertions above the configured range to its final index.
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
        double x = NumericConversion.GenericToDouble(Xs, index);
        double y = NumericConversion.GenericToDouble(Ys, index);
        return new(x, y);
    }

    Coordinates IDataSource.GetCoordinateScaled(int index)
    {
        double x = DataSourceUtilities.ScaleXY(Xs, index, XScale, XOffset);
        double y = DataSourceUtilities.ScaleXY(Ys, index, YScale, YOffset);
        return new(x, y);
    }

    double IDataSource.GetX(int index) =>
        Rotated
            ? NumericConversion.GenericToDouble(Ys, index)
            : NumericConversion.GenericToDouble(Xs, index);

    double IDataSource.GetXScaled(int index) =>
        Rotated
            ? DataSourceUtilities.ScaleXY(Ys, index, YScale, YOffset)
            : DataSourceUtilities.ScaleXY(Xs, index, XScale, XOffset);

    double IDataSource.GetY(int index) =>
        Rotated
            ? NumericConversion.GenericToDouble(Xs, index)
            : NumericConversion.GenericToDouble(Ys, index);

    double IDataSource.GetYScaled(int index) =>
        Rotated
            ? DataSourceUtilities.ScaleXY(Xs, index, XScale, XOffset)
            : DataSourceUtilities.ScaleXY(Ys, index, YScale, YOffset);

    bool IDataSource.IsSorted() => true;
}
