// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

// TODO: Performance of many data sources may be improved and code duplication reduced.
// Improvements should be made in concert with performance tests.
/// <summary>Internal interface used for utility functions within <see cref="DataSourceUtilities"/>.</summary>
public interface IDataSource
{
    /// <summary>Gets whether utilities should prefer paths that use <see cref="GetCoordinate"/>.</summary>
    bool PreferCoordinates { get; }

    /// <summary>Gets the length of the collection.</summary>
    int Length { get; }

    /// <summary>Gets the minimum index to render.</summary>
    int MinRenderIndex { get; }

    /// <summary>Gets the maximum index to render.</summary>
    int MaxRenderIndex { get; }

    /// <summary>Gets the closest index to a specified X coordinate.</summary>
    /// <param name="mouseLocation">The mouse location; use its X coordinate unless the plot is rotated.</param>
    /// <returns>The index with the X coordinate closest to the requested location.</returns>
    /// <remarks>For sorted data, a binary search such as <see cref="DataSourceUtilities.GetClosestIndex(double[], double, IndexRange)"/> is fastest.</remarks>
    int GetXClosestIndex(Coordinates mouseLocation);

    /// <summary>Gets the X-Y coordinate from the data source at the specified index.</summary>
    /// <param name="index">The data source index.</param>
    /// <returns>The coordinate at the specified index.</returns>
    Coordinates GetCoordinate(int index);

    /// <summary>Gets the X-Y coordinate at the specified index with offsets and scaling applied.</summary>
    /// <param name="index">The data source index.</param>
    /// <returns>The scaled coordinate at the specified index.</returns>
    Coordinates GetCoordinateScaled(int index);

    /// <summary>Gets the X value from the data source at the specified index.</summary>
    /// <param name="index">The data source index.</param>
    /// <returns>The X value at the specified index.</returns>
    double GetX(int index);

    /// <summary>Gets the X value at the specified index with offsets and scaling applied.</summary>
    /// <param name="index">The data source index.</param>
    /// <returns>The scaled X value at the specified index.</returns>
    double GetXScaled(int index);

    /// <summary>Gets the Y value from the data source at the specified index.</summary>
    /// <param name="index">The data source index.</param>
    /// <returns>The Y value at the specified index.</returns>
    double GetY(int index);

    /// <summary>Gets the Y value at the specified index with offsets and scaling applied.</summary>
    /// <param name="index">The data source index.</param>
    /// <returns>The scaled Y value at the specified index.</returns>
    double GetYScaled(int index);

    /// <summary>Gets whether the collection is sorted, allowing faster nearest-index searches.</summary>
    /// <returns><see langword="true"/> when the collection is sorted; otherwise, <see langword="false"/>.</returns>
    bool IsSorted();
}
