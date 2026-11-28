// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CrissCross.Plot.Collections;

namespace CrissCross.Plot;

/// <summary>Provides utilities for searching, scaling, and locating values in plot data sources.</summary>
[SuppressMessage("Style", "SST1704", Justification = "DataSourceUtilities is the established public API name and renaming it would be a breaking change.")]
public static class DataSourceUtilities
{
    /// <summary>Extension members for <c>IDataSource</c>.</summary>
    /// <param name="dataSource">The data source to evaluate.</param>
    extension(IDataSource dataSource)
    {
        /// <summary>Creates a new <see cref="IndexRange"/></summary>
        /// <returns>an IndexRange representing a the minimum and maximum indexes to render.</returns>
        [MethodImpl(NumericConversion.ImplOptions)]
        public IndexRange GetRenderIndexRange()
        {
            if (dataSource is null || dataSource.Length == 0)
            {
                return IndexRange.None;
            }

            int min = Math.Min(dataSource.Length - 1, dataSource.MinRenderIndex);
            int max = Math.Min(dataSource.Length - 1, dataSource.MaxRenderIndex);
            return min > max || min < 0 || max < 0 ? IndexRange.None : new IndexRange(min, max);
        }
    }

    /// <summary>Extension members for <c>IEnumerable&lt;T&gt;</c>.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="values">The collection to evaluate.</param>
    extension<T>(IEnumerable<T> values)
    {
        /// <summary>Check if a collection is in ascending order.</summary>
        /// <param name="comparer">The comparer to use. If not specified, uses Comparer{T}.Default (which is preferred)</param>
        /// <returns>True if the collection is ascending (and can therefore be used with BinarySearch and GetClosest). Otherwise false.</returns>
        public bool IsAscending(IComparer<T>? comparer)
        {
            using var enu = values?.GetEnumerator() ?? throw new ArgumentNullException(nameof(values));
            if (!enu.MoveNext())
            {
                return false;
            }

            comparer ??= GenericComparer<T>.Default;
            T prev = enu.Current;
            while (enu.MoveNext())
            {
                if (comparer.Compare(enu.Current, prev) < 0) // current >= prev = OK, current < prev Not OK
                {
                    return false;
                }

                prev = enu.Current;
            }

            return true;
        }
    }

    /// <summary>Extension members for <c>IReadOnlyList&lt;T&gt;</c>.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="sortedList">The sorted list to search.</param>
    extension<T>(IReadOnlyList<T> sortedList)
    {

        /// <summary>Searches a range of elements in a sorted <see cref="IReadOnlyList{T}"/> for a value using a binary search algorithm.</summary>
        /// <param name="index">The starting index of the range to search.</param>
        /// <param name="length">The number of elements in the range to search.</param>
        /// <param name="value">The value to locate.</param>
        /// <param name="comparer">
        /// The comparer implementation to use when comparing elements, or <see langword="null"/> to use the default comparer.
        /// </param>
        /// <returns>
        /// The zero-based index of the value in the list, if found;
        /// otherwise, a negative number that is the bitwise complement of the index
        /// of the next element that is larger than <paramref name="value"/>, or, if
        /// there is no larger element, the bitwise complement of <paramref name="index"/> + <paramref name="length"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sortedList"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> or <paramref name="length"/> are invalid for the list.</exception>
        [SuppressMessage("Style", "SST2309", Justification = "The optional comparer parameter is retained for source compatibility.")]
        public int BinarySearch(
            int index,
            int length,
            T value,
            IComparer<T>? comparer = null)
        {
            ValidateNotNull(sortedList, nameof(sortedList));

            if (index < 0 || length < 0 || index + length > sortedList.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            comparer ??= Comparer<T>.Default;
            int low = index;
            int high = index + length - 1;

            while (low <= high)
            {
                int mid = low + ((high - low) >> 1);
                int cmp = comparer.Compare(sortedList[mid], value);
                if (cmp == 0)
                {
                    return mid;
                }

                if (cmp < 0)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }

            // Same semantics as Array.BinarySearch
            return ~low;
        }
    }

    /// <inheritdoc cref="Array.BinarySearch{T}(T[], int, int, T, IComparer{T})"/>
    [MethodImpl(NumericConversion.ImplOptions)]
    [SuppressMessage("Style", "SST1703", Justification = "The classic extension method shape is retained for existing source and binary API compatibility.")]
    [SuppressMessage("Style", "SST1705", Justification = "Keeping this generic overload alongside the specialized overloads preserves the established public API.")]
    public static int BinarySearch<T, TList>(this TList sortedList, int index, int length, T value, IComparer<T>? comparer)
        where TList : IEnumerable<T>
    {
        ValidateNotNull(sortedList, nameof(sortedList));
        comparer ??= BinarySearchComparer.GetComparer<T>();

        return sortedList switch
        {
            List<T> listT => listT.BinarySearch(index, length, value, comparer),
            T[] arrayT => Array.BinarySearch(arrayT, index, length, value, comparer),
            CircularBuffer<T> circularBufferT => circularBufferT.BinarySearch(index, length, value, comparer),
            IReadOnlyList<T> readOnlyListT => readOnlyListT.BinarySearch(index, length, value, comparer),
            _ => throw new NotSupportedException($"unsupported {nameof(IList<>)}: {sortedList.GetType().Name}")
        };
    }

    /// <inheritdoc cref="GetClosestIndex{TValue, TList}(TList, TValue, IndexRange, IComparer{TValue}?)"/>
    [MethodImpl(NumericConversion.ImplOptions)]
    public static int GetClosestIndex(double[] sortedList, double value, IndexRange indexRange)
    {
        int index = Array.BinarySearch(sortedList, indexRange.Min, indexRange.Length, value);
        if (index < 0)
        {
            index = ~index;
        }

        return index > indexRange.Max ? indexRange.Max : index;
    }

    /// <inheritdoc cref="GetClosestIndex{TValue, TList}(TList, TValue, IndexRange, IComparer{TValue}?)"/>
    [MethodImpl(NumericConversion.ImplOptions)]
    public static int GetClosestIndex(List<double> sortedList, double value, IndexRange indexRange)
    {
        ValidateNotNull(sortedList, nameof(sortedList));

        int index = sortedList.BinarySearch(indexRange.Min, indexRange.Length, value, BinarySearchComparer.Instance);
        if (index < 0)
        {
            index = ~index;
        }

        return index > indexRange.Max ? indexRange.Max : index;
    }

    /// <inheritdoc cref="GetClosestIndex{TValue, TList}(TList, TValue, IndexRange, IComparer{TValue}?)"/>
    [MethodImpl(NumericConversion.ImplOptions)]
    public static int GetClosestIndex(Coordinates[] sortedList, Coordinates value, IndexRange indexRange)
    {
        int index = Array.BinarySearch(sortedList, indexRange.Min, indexRange.Length, value, BinarySearchComparer.Instance);
        if (index < 0)
        {
            index = ~index;
        }

        return index > indexRange.Max ? indexRange.Max : index;
    }

    /// <inheritdoc cref="GetClosestIndex{TValue, TList}(TList, TValue, IndexRange, IComparer{TValue}?)"/>
    [MethodImpl(NumericConversion.ImplOptions)]
    public static int GetClosestIndex(List<Coordinates> sortedList, Coordinates value, IndexRange indexRange)
    {
        ValidateNotNull(sortedList, nameof(sortedList));

        int index = sortedList.BinarySearch(indexRange.Min, indexRange.Length, value, BinarySearchComparer.Instance);
        if (index < 0)
        {
            index = ~index;
        }

        return index > indexRange.Max ? indexRange.Max : index;
    }

    /// <summary>Gets the closest index to a specified <paramref name="value"/> from the <paramref name="sortedList"/></summary>
    /// <typeparam name="TValue">The type of values in the collection</typeparam>
    /// <typeparam name="TList">Must be an object that implements IList or IReadOnlyList. List{T} | T[] are preferred.</typeparam>
    /// <param name="sortedList">The collection to search. Collection must be in ascending order.</param>
    /// <param name="value">The value to search for in the collection</param>
    /// <param name="indexRange">Provides details about the range of indexes to search</param>
    /// <param name="comparer">A comparer used to determine equality - recommend default comparer or <see cref="GenericComparer{T}.Default"/></param>
    /// <returns>The index of the item that is closest to the <paramref name="value"/></returns>
    [SuppressMessage("Style", "SST1703", Justification = "The generic classic extension method is retained for established public API compatibility.")]
    [SuppressMessage("Style", "SST1705", Justification = "The generic overload is kept alongside specialized overloads for source compatibility.")]
    public static int GetClosestIndex<TValue, TList>(this TList sortedList, TValue value, IndexRange indexRange, IComparer<TValue>? comparer)
        where TList : IEnumerable<TValue> // expects IList & IReadOnlyList
    {
        int index = BinarySearch(sortedList, indexRange.Min, indexRange.Length, value, comparer);

        // If x is not exactly matched to any value in Xs, BinarySearch returns a negative number.
        // The bitwise complement gives the position where x would be inserted.
        // If x is below the minimum, the result is the first index.
        // If x is above the maximum, the result is clamped to the final index.
        if (index < 0)
        {
            index = ~index; // read BinarySearch() docs
        }

        return index > indexRange.Max ? indexRange.Max : index;
    }

    /// <summary>Calculates the maximum count of indexes available to render in an <see cref="IDataSource"/>'s collection.</summary>
    /// <param name="dataSource">The datasource</param>
    /// <returns>The number of indexes that should be rendered. </returns>
    /// <remarks>Example : <br/>MaxIndex = Xs.Length - 1 <br/>MinIndex = 0, <br/>returns Xs.Length. </remarks>
    [MethodImpl(NumericConversion.ImplOptions)]
    [SuppressMessage("Style", "SST1703", Justification = "The classic extension method shape is retained for existing source and binary API compatibility.")]
    [SuppressMessage("Style", "SST1705", Justification = "This utility remains a static extension method for established public API compatibility.")]
    public static int GetRenderIndexCount(this IDataSource dataSource) =>
        Math.Max(0, Math.Min(dataSource.Length - 1, dataSource.MaxRenderIndex) - Math.Max(0, dataSource.MinRenderIndex) + 1);

    /// <summary>Scale the <paramref name="value"/> by applying the <paramref name="scalingFactor"/> and the <paramref name="offset"/></summary>
    /// <param name="value">the value to scale</param>
    /// <param name="scalingFactor">the scaling factor to apply.</param>
    /// <param name="offset">the offset to apply</param>
    /// <returns>value * <paramref name="scalingFactor"/> + <paramref name="offset"/></returns>
    [MethodImpl(NumericConversion.ImplOptions)]
    public static double ScaleXY(double value, double scalingFactor, double offset) =>
        (value * scalingFactor) + offset;

    /// <inheritdoc cref="ScaleXY(double, double, double)"/>
    /// <typeparam name="T">The type of value - will be converted to a double via <see cref="NumericConversion.GenericToDouble{T}(ref T)"/></typeparam>
    [MethodImpl(NumericConversion.ImplOptions)]
    public static double ScaleXY<T>(T value, double scalingFactor, double offset) =>
        (NumericConversion.GenericToDouble(ref value) * scalingFactor) + offset;

    /// <summary>Scales a value from a collection using the specified factor and offset.</summary>
    /// <param name="collection">The collection containing the value.</param>
    /// <param name="index">The index of the value within the collection.</param>
    /// <param name="scalingFactor">The factor applied to the value.</param>
    /// <param name="offset">The offset added after scaling.</param>
    /// <returns>The scaled value.</returns>
    /// <typeparam name="T">The element type.</typeparam>
    [MethodImpl(NumericConversion.ImplOptions)]
    public static double ScaleXY<T>(T[] collection, int index, double scalingFactor, double offset) =>
        (NumericConversion.GenericToDouble(collection, index) * scalingFactor) + offset;

    /// <inheritdoc cref="ScaleXY{T}(T[], int, double, double)"/>
    /// <typeparam name="T">The element type.</typeparam>
    [MethodImpl(NumericConversion.ImplOptions)]
    public static double ScaleXY<T>(IReadOnlyList<T> collection, int index, double scalingFactor, double offset) =>
        (NumericConversion.GenericToDouble(collection, index) * scalingFactor) + offset;

    /// <summary>Unscales a value by removing the specified offset and factor.</summary>
    /// <param name="value">The value to unscale.</param>
    /// <param name="scalingFactor">The scaling factor to remove.</param>
    /// <param name="offset">The offset to remove.</param>
    /// <returns>The unscaled value.</returns>
    public static double UnscaleXY(double value, double scalingFactor, double offset) =>
        (value - offset) / scalingFactor;

    /// <summary>Scale a coordinate by applying the offsets and the scaling factors.</summary>
    /// <inheritdoc cref="UnScaleCoordinate(Coordinates, RenderDetails, double, double, double, double, IXAxis?, IYAxis?)"/>
    [MethodImpl(NumericConversion.ImplOptions)]
    [SuppressMessage("StyleSharp.Naming", "SST1305", Justification = "The X/Y parameter names are established public named-argument identifiers.")]
    public static Coordinates ScaleCoordinate(
        Coordinates coordinate,
        double xScalingFactor,
        double xOffset,
        double yScalingFactor,
        double yOffset) => new(
        x: ScaleXY(coordinate.X, xScalingFactor, xOffset),
        y: ScaleXY(coordinate.Y, yScalingFactor, yOffset));

    /// <inheritdoc cref="UnScaleCoordinate(Coordinates, RenderDetails, double, double, double, double, IXAxis?, IYAxis?)"/>
    [MethodImpl(NumericConversion.ImplOptions)]
    [SuppressMessage("StyleSharp.Naming", "SST1305", Justification = "The X/Y parameter names are established public named-argument identifiers.")]
    public static Coordinates UnScaleCoordinate(
        Coordinates coordinate,
        double xScalingFactor,
        double xOffset,
        double yScalingFactor,
        double yOffset) => new(
        x: (coordinate.X - xOffset) / xScalingFactor,
        y: (coordinate.Y - yOffset) / yScalingFactor);

    /// <summary>Unscale a coordinate.</summary>
    /// <param name="pixelCoordinate">the coordinate to unscale</param>
    /// <param name="renderInfo">RenderDetails used to unscale the coordinate by the pixels/axis</param>
    /// <param name="xScalingFactor">The plottable's X Scaling factor</param>
    /// <param name="xOffset">The plottable's X Offset</param>
    /// <param name="yScalingFactor">The plottable's Y Scaling factor</param>
    /// <param name="yOffset">The plottable's Y OFfset</param>
    /// <param name="xaxis">The X-Axis to used to get the pixel scaling from the <paramref name="renderInfo"/>. If not found uses the default value from the renderInfo.</param>
    /// <param name="yaxis">The Y-Axis to used to get the pixel scaling from the <paramref name="renderInfo"/>. If not found uses the default value from the renderInfo.</param>
    /// <returns>A new <see cref="Coordinates"/> value.</returns>
    [MethodImpl(NumericConversion.ImplOptions)]
    [SuppressMessage("Style", "SST1472", Justification = "The eight-parameter overload is part of the established public API and is retained for compatibility.")]
    [SuppressMessage("Style", "SST2309", Justification = "All optional parameters are retained for existing public API source compatibility.")]
    [SuppressMessage("StyleSharp.Naming", "SST1305", Justification = "The X/Y parameter names are established public named-argument identifiers.")]
    public static Coordinates UnScaleCoordinate(
        Coordinates pixelCoordinate,
        RenderDetails renderInfo,
        double xScalingFactor = 1,
        double xOffset = 0,
        double yScalingFactor = 1,
        double yOffset = 0,
        IXAxis? xaxis = null,
        IYAxis? yaxis = null)
    {
        _ = renderInfo.TryGetPixelPerUnitX(xaxis, out var pixelScaleX);
        _ = renderInfo.TryGetPixelPerUnitY(yaxis, out var pixelScaleY);

        return new(
            x: UnScale(pixelCoordinate.X, pixelScaleX, xScalingFactor, xOffset),
            y: UnScale(pixelCoordinate.Y, pixelScaleY, yScalingFactor, yOffset));
        static double UnScale(double value, double pixelScaling, double axisScaling, double offset) => ((value / pixelScaling) - offset) / axisScaling;
    }

    /// <remarks> Checks <see cref="IDataSource.IsSorted"/> and decides to call the Fast or Original method accordingly</remarks>
    /// <inheritdoc cref="GetNearestFast"/>
    [MethodImpl(NumericConversion.ImplOptions)]
    [SuppressMessage("Style", "SST2309", Justification = "The optional axis parameters preserve the established public API.")]
    [SuppressMessage("Naming", "SST1305", Justification = "Axis parameter names are established public named-argument identifiers.")]
    public static DataPoint GetNearestSmart(
        IDataSource dataSource,
        Coordinates mouseLocation,
        RenderDetails renderInfo,
        float maxDistance,
        IXAxis? xAxis = null,
        IYAxis? yAxis = null) =>
        dataSource.IsSorted()
            ? GetNearestFast(dataSource, mouseLocation, renderInfo, maxDistance, xAxis, yAxis)
            : GetNearest(dataSource, mouseLocation, renderInfo, maxDistance, xAxis, yAxis);

    /// <remarks> Checks <see cref="IDataSource.IsSorted"/> and decides to call the Fast or Original method accordingly</remarks>
    /// <inheritdoc cref="GetNearestXFast"/>
    [MethodImpl(NumericConversion.ImplOptions)]
    [SuppressMessage("Style", "SST2309", Justification = "The optional axis parameter preserves the established public API.")]
    [SuppressMessage("Naming", "SST1305", Justification = "Axis parameter names are established public named-argument identifiers.")]
    public static DataPoint GetNearestXSmart(IDataSource dataSource, Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance, IXAxis? xAxis = null) =>
        dataSource.IsSorted() ? GetNearestXFast(dataSource, mouseLocation, renderInfo, maxDistance, xAxis) : GetNearestX(dataSource, mouseLocation, renderInfo, maxDistance, xAxis);

    /// <summary>Get the nearest datapoint on the plot from the <paramref name="dataSource"/>, based on the <paramref name="mouseLocation"/> and the <paramref name="renderInfo"/></summary>
    /// <remarks>This is the original way to locate the nearest DataPoint from the collection, and is safe for unsorted collections (such as Scatter).</remarks>
    /// <param name="dataSource">The data source</param>
    /// <param name="mouseLocation">the mouse coordinates from the plot. <see cref="Plot.GetCoordinates(Pixel, IXAxis?, IYAxis?)"/></param>
    /// <param name="renderInfo">The render details used to convert plot units to pixels.</param>
    /// <param name="maxDistance">The maximum distance to search</param>
    /// <param name="xAxis">The X-Axis of assigned to the datasource. If not specified, uses the bottom axis.</param>
    /// <param name="yAxis">The Y axis assigned to the data source. If not specified, the left axis is used.</param>
    /// <returns>The nearest data point, or <see cref="DataPoint.None"/> when no point is within range.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dataSource"/> is <see langword="null"/>.</exception>
    [SuppressMessage("Style", "SST2309", Justification = "The optional axis parameters preserve the established public API.")]
    [SuppressMessage("Naming", "SST1305", Justification = "Axis parameter names are established public named-argument identifiers.")]
    public static DataPoint GetNearest(IDataSource dataSource, Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance, IXAxis? xAxis = null, IYAxis? yAxis = null)
    {
        ValidateNotNull(dataSource, nameof(dataSource));

        double maxDistanceSquared = maxDistance * maxDistance;
        double closestDistanceSquared = double.PositiveInfinity;
        int closestIndex = 0;

        _ = renderInfo.TryGetPixelPerUnitX(xAxis, out double pxPerUnitX);
        _ = renderInfo.TryGetPixelPerUnitY(yAxis, out double pxPerUnitY);

        int indexLength = Math.Max(0, dataSource.MinRenderIndex) + dataSource.GetRenderIndexCount();
        bool preferCoordinates = dataSource.PreferCoordinates;
        double x;
        double y;

        for (int i = Math.Max(0, dataSource.MinRenderIndex); i < indexLength; i++)
        {
            if (preferCoordinates)
            {
                (x, y) = dataSource.GetCoordinateScaled(i).Deconstruct();
            }
            else
            {
                x = dataSource.GetXScaled(i);
                y = dataSource.GetYScaled(i);
            }

            double deltaX = (x - mouseLocation.X) * pxPerUnitX;
            double deltaY = (y - mouseLocation.Y) * pxPerUnitY;
            double distanceSquared = (deltaX * deltaX) + (deltaY * deltaY);

            if (distanceSquared < closestDistanceSquared)
            {
                closestDistanceSquared = distanceSquared;
                closestIndex = i;
            }
        }

        if (closestDistanceSquared < maxDistanceSquared)
        {
            return preferCoordinates
                ? new DataPoint(dataSource.GetCoordinateScaled(closestIndex), closestIndex)
                : new DataPoint(dataSource.GetXScaled(closestIndex), dataSource.GetYScaled(closestIndex), closestIndex);
        }

        return DataPoint.None;
    }

    /// <summary>Get the nearest datapoint from the <paramref name="dataSource"/>, based on the <paramref name="mouseLocation"/> X location and the <paramref name="renderInfo"/></summary>
    /// <inheritdoc cref="GetNearest"/>
    [SuppressMessage("Style", "SST2309", Justification = "The optional axis parameter preserves the established public API.")]
    [SuppressMessage("Naming", "SST1305", Justification = "Axis parameter names are established public named-argument identifiers.")]
    public static DataPoint GetNearestX(IDataSource dataSource, Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance, IXAxis? xAxis = null)
    {
        ValidateNotNull(dataSource, nameof(dataSource));

        _ = renderInfo.TryGetPixelPerUnitX(xAxis, out double pxPerUnitX);

        int indexLength = Math.Max(0, dataSource.MinRenderIndex) + dataSource.GetRenderIndexCount();
        double closestDistance = double.PositiveInfinity;

        int closestIndex = 0;

        for (int i = Math.Max(0, dataSource.MinRenderIndex); i < indexLength; i++)
        {
            double deltaX = Math.Abs(dataSource.GetXScaled(i) - mouseLocation.X) * pxPerUnitX;
            if (deltaX < closestDistance)
            {
                closestDistance = deltaX;
                closestIndex = i;
            }
        }

        if (closestDistance <= maxDistance)
        {
            return dataSource.PreferCoordinates
                ? new DataPoint(dataSource.GetCoordinateScaled(closestIndex), closestIndex)
                : new DataPoint(dataSource.GetXScaled(closestIndex), dataSource.GetYScaled(closestIndex), closestIndex);
        }

        return DataPoint.None;
    }

    /// <remarks>This is a faster way to locate the nearest DataPoint from the collection, but it requires the collection to be sorted in ascending order. ( Signal | SignalXY )</remarks>
    /// <inheritdoc cref="GetNearest"/>
    [SuppressMessage("Style", "SST2309", Justification = "The optional axis parameters preserve the established public API.")]
    [SuppressMessage("Naming", "SST1305", Justification = "Axis parameter names are established public named-argument identifiers.")]
    public static DataPoint GetNearestFast(IDataSource dataSource, Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance, IXAxis? xAxis = null, IYAxis? yAxis = null)
    {
        // To-DO : GetXCLosestIndex accepts the mouse coordinate, which means its fully scalled and offset. This must be accounted for in the IDataSource.GetXClosestIndex
        int closestIndex = dataSource.GetXClosestIndex(mouseLocation);

        _ = renderInfo.TryGetPixelPerUnitX(xAxis, out double pxPerUnitX);
        _ = renderInfo.TryGetPixelPerUnitY(yAxis, out double pxPerUnitY);

        int searchedLeft = closestIndex;
        int searchedRight = closestIndex;
        int nextPoint = closestIndex;
        double maxDistanceSquared = maxDistance * maxDistance;
        double maxDistanceToSearch = maxDistanceSquared;

        double closestDistanceSquared = double.PositiveInfinity;

        bool preferCoordinates = dataSource.PreferCoordinates;
        double x;
        double y;
        while (nextPoint != -1)
        {
            if (preferCoordinates)
            {
                (x, y) = dataSource.GetCoordinateScaled(nextPoint).Deconstruct();
            }
            else
            {
                x = dataSource.GetXScaled(nextPoint);
                y = dataSource.GetYScaled(nextPoint);
            }

            double deltaX = (x - mouseLocation.X) * pxPerUnitX;
            double deltaY = (y - mouseLocation.Y) * pxPerUnitY;
            double distanceSquared = (deltaX * deltaX) + (deltaY * deltaY);

            if (distanceSquared < maxDistanceToSearch)
            {
                maxDistanceToSearch = distanceSquared;
                closestIndex = nextPoint;
                closestDistanceSquared = distanceSquared;
            }

            nextPoint = GetNextPointNearestSearch(dataSource, mouseLocation.X, searchedLeft, searchedRight, maxDistanceToSearch, pxPerUnitX);

            if (nextPoint < searchedLeft)
            {
                searchedLeft = nextPoint;
            }
            else
            {
                searchedRight = nextPoint;
            }
        }

        if (closestDistanceSquared <= maxDistanceSquared)
        {
            return preferCoordinates
                ? new DataPoint(dataSource.GetCoordinateScaled(closestIndex), closestIndex)
                : new DataPoint(dataSource.GetXScaled(closestIndex), dataSource.GetYScaled(closestIndex), closestIndex);
        }

        return DataPoint.None;
    }

    /// <summary>Get the nearest datapoint from the <paramref name="dataSource"/>, based on the <paramref name="mouseLocation"/> X location and the <paramref name="renderInfo"/></summary>
    /// <inheritdoc cref="GetNearestFast"/>
    [SuppressMessage("Style", "SST2309", Justification = "The optional axis parameter preserves the established public API.")]
    [SuppressMessage("Naming", "SST1305", Justification = "Axis parameter names are established public named-argument identifiers.")]
    public static DataPoint GetNearestXFast(IDataSource dataSource, Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance, IXAxis? xAxis = null)
    {
        // Nearest X will always be within 1 of the closest index of a sorted collection
        _ = renderInfo.TryGetPixelPerUnitX(xAxis, out double pxPerUnitX);
        int closestIndex = dataSource.GetXClosestIndex(mouseLocation);

        double distance1 = Math.Abs(dataSource.GetXScaled(closestIndex) - mouseLocation.X) * pxPerUnitX;
        if (closestIndex > 0)
        {
            double distance2 = Math.Abs(dataSource.GetXScaled(closestIndex - 1) - mouseLocation.X) * pxPerUnitX;
            if (distance1 <= distance2 && distance1 <= maxDistance)
            {
                // do nothing - heads to the return statements at bottom
            }
            else if (distance2 < maxDistance) // distance2 is known closer at this point, check if within range
            {
                closestIndex--;
            }
            else
            {
                return DataPoint.None;
            }
        }
        else if (distance1 > maxDistance)
        {
            return DataPoint.None;
        }

        return dataSource.PreferCoordinates
            ? new DataPoint(dataSource.GetCoordinateScaled(closestIndex), closestIndex)
            : new DataPoint(dataSource.GetXScaled(closestIndex), dataSource.GetYScaled(closestIndex), closestIndex);
    }

    /// <summary>Gets the next candidate point while expanding a nearest-point search.</summary>
    /// <param name="dataSource">The data source being searched.</param>
    /// <param name="locationX">The target X coordinate.</param>
    /// <param name="searchedLeft">The leftmost index already searched.</param>
    /// <param name="searchedRight">The rightmost index already searched.</param>
    /// <param name="maxDistanceSquared">The maximum allowed squared distance.</param>
    /// <param name="pixelsPerUnitX">The horizontal pixel scale.</param>
    /// <returns>The next candidate index, or -1 when no candidate is within range.</returns>
    private static int GetNextPointNearestSearch(IDataSource dataSource, double locationX, int searchedLeft, int searchedRight, double maxDistanceSquared, double pixelsPerUnitX)
    {
        int leftCandidate = searchedLeft - 1;
        int rightCandidate = searchedRight + 1;

        if (leftCandidate < 0 && rightCandidate >= dataSource.Length)
        {
            return -1;
        }

        if (leftCandidate < 0)
        {
            double distance = (dataSource.GetXScaled(rightCandidate) - locationX) * pixelsPerUnitX;
            return distance * distance > maxDistanceSquared ? -1 : rightCandidate;
        }

        if (rightCandidate >= dataSource.Length)
        {
            double distance = (dataSource.GetXScaled(leftCandidate) - locationX) * pixelsPerUnitX;
            return distance * distance > maxDistanceSquared ? -1 : leftCandidate;
        }

        double leftCandidateDistance = (dataSource.GetXScaled(leftCandidate) - locationX) * pixelsPerUnitX;
        double rightCandidateDistance = (dataSource.GetXScaled(rightCandidate) - locationX) * pixelsPerUnitX;

        double minDistanceSquared = Math.Min(leftCandidateDistance * leftCandidateDistance, rightCandidateDistance * rightCandidateDistance);
        if (minDistanceSquared > maxDistanceSquared)
        {
            return -1;
        }

        return Math.Abs(leftCandidateDistance) < Math.Abs(rightCandidateDistance) ? leftCandidate : rightCandidate;
    }

    /// <summary>Throws when a public method receives a null reference.</summary>
    /// <typeparam name="T">The validated reference type.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="parameterName">The public parameter name.</param>
    private static void ValidateNotNull<T>(T value, string parameterName)
    {
        if (value is not null)
        {
            return;
        }

        throw new ArgumentNullException(parameterName);
    }
}
