// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Provides data-source behavior for a coordinate list.</summary>
/// <param name="readOnlyList">The coordinates exposed by this data source.</param>
public class CoordinateDataSource(IReadOnlyList<Coordinates> readOnlyList) : IDataSource
{
    /// <summary>The coordinates exposed by this data source.</summary>
    private readonly IReadOnlyList<Coordinates> _coordinates = readOnlyList;

    /// <summary>Gets or sets the horizontal offset applied to coordinates.</summary>
    public double XOffset { get; set; }

    /// <summary>Gets or sets the vertical offset applied to coordinates.</summary>
    public double YOffset { get; set; }

    /// <summary>Gets or sets the horizontal scale applied to coordinates.</summary>
    public double XScale { get; set; } = 1;

    /// <summary>Gets or sets the vertical scale applied to coordinates.</summary>
    public double YScale { get; set; } = 1;

    /// <summary>Gets a value indicating that this data source prefers coordinate access.</summary>
    public bool PreferCoordinates => true;

    /// <summary>Gets the number of coordinates in the source.</summary>
    public int Length => _coordinates.Count;

    /// <summary>Gets or sets the minimum index rendered from this source.</summary>
    public int MinRenderIndex { get; set; }

    /// <summary>Gets or sets the maximum index rendered from this source.</summary>
    public int MaxRenderIndex { get; set; } = readOnlyList.Count - 1;

    /// <summary>Gets the coordinate at the specified index.</summary>
    /// <param name="index">The coordinate index.</param>
    /// <returns>The coordinate at that index.</returns>
    public Coordinates GetCoordinate(int index) => _coordinates[index];

    /// <summary>Gets the scaled coordinate at the specified index.</summary>
    /// <param name="index">The coordinate index.</param>
    /// <returns>The scaled coordinate.</returns>
    public Coordinates GetCoordinateScaled(int index) => DataSourceUtilities.ScaleCoordinate(_coordinates[index], XScale, XOffset, YScale, YOffset);

    /// <summary>Gets the horizontal coordinate at the specified index.</summary>
    /// <param name="index">The coordinate index.</param>
    /// <returns>The horizontal coordinate.</returns>
    public double GetX(int index) => _coordinates[index].X;

    /// <summary>Gets the index of the coordinate closest to the specified location.</summary>
    /// <param name="mouseLocation">The location to compare against.</param>
    /// <returns>The index of the closest coordinate.</returns>
    public int GetXClosestIndex(Coordinates mouseLocation) =>
        DataSourceUtilities.GetClosestIndex(
            _coordinates is Coordinates[] coordinateArray ? coordinateArray : [.. _coordinates],
            DataSourceUtilities.UnScaleCoordinate(mouseLocation, XScale, XOffset, YScale, YOffset),
            this.GetRenderIndexRange());

    /// <summary>Gets the scaled horizontal coordinate at the specified index.</summary>
    /// <param name="index">The coordinate index.</param>
    /// <returns>The scaled horizontal coordinate.</returns>
    public double GetXScaled(int index) => DataSourceUtilities.ScaleXY(_coordinates[index].X, XScale, XOffset);

    /// <summary>Gets the vertical coordinate at the specified index.</summary>
    /// <param name="index">The coordinate index.</param>
    /// <returns>The vertical coordinate.</returns>
    public double GetY(int index) => _coordinates[index].Y;

    /// <summary>Gets the scaled vertical coordinate at the specified index.</summary>
    /// <param name="index">The coordinate index.</param>
    /// <returns>The scaled vertical coordinate.</returns>
    public double GetYScaled(int index) => DataSourceUtilities.ScaleXY(_coordinates[index].Y, YScale, YOffset);

    /// <summary>Gets a value indicating whether the coordinates are sorted by horizontal position.</summary>
    /// <returns><see langword="true"/> when the coordinates are sorted.</returns>
    public bool IsSorted() => _coordinates.IsAscending(BinarySearchComparer.Instance);
}
