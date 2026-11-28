// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>This data source manages X/Y points as a collection of coordinates.</summary>
/// <param name="coordinates">The coordinates exposed by the source.</param>
public class ScatterSourceCoordinatesArray(Coordinates[] coordinates) : IScatterSource, IDataSource, IGetNearest
{
    /// <summary>The coordinates exposed by this source.</summary>
    private readonly Coordinates[] _coordinates = coordinates;

    /// <summary>Gets or sets the first coordinate index included in rendering.</summary>
    public int MinRenderIndex { get; set; }

    /// <summary>Gets or sets the last coordinate index included in rendering.</summary>
    public int MaxRenderIndex { get; set; } = coordinates.Length - 1;

    bool IDataSource.PreferCoordinates => true;

    int IDataSource.Length => _coordinates.Length;

    /// <summary>Gets the coordinates currently included in rendering.</summary>
    /// <returns>A read-only view of the visible coordinates.</returns>
    public IReadOnlyList<Coordinates> GetScatterPoints() => _coordinates
            .SkipView(MinRenderIndex)
            .TakeView(this.GetRenderIndexCount());

    /// <summary>Gets the axis limits of the visible coordinates.</summary>
    /// <returns>The axis limits.</returns>
    public AxisLimits GetLimits()
    {
        ExpandingAxisLimits limits = new();
        limits.Expand(_coordinates.SkipView(MinRenderIndex).TakeView(this.GetRenderIndexCount()));
        return limits.AxisLimits;
    }

    /// <summary>Gets the horizontal range of the visible coordinates.</summary>
    /// <returns>The horizontal coordinate range.</returns>
    public CoordinateRange GetLimitsX() => GetLimits().Rect.XRange;

    /// <summary>Gets the vertical range of the visible coordinates.</summary>
    /// <returns>The vertical coordinate range.</returns>
    public CoordinateRange GetLimitsY() => GetLimits().Rect.YRange;

    /// <summary>Gets the data point nearest to a location.</summary>
    /// <param name="location">The location to compare against.</param>
    /// <param name="renderInfo">The current render details.</param>
    /// <param name="maxDistance">The maximum pixel distance allowed.</param>
    /// <returns>The nearest data point.</returns>
    public DataPoint GetNearest(Coordinates location, RenderDetails renderInfo, float maxDistance = 15) =>
        DataSourceUtilities.GetNearestSmart(this, location, renderInfo, maxDistance);

    /// <summary>Gets the data point with the nearest horizontal coordinate.</summary>
    /// <param name="location">The location to compare against.</param>
    /// <param name="renderInfo">The current render details.</param>
    /// <param name="maxDistance">The maximum pixel distance allowed.</param>
    /// <returns>The nearest data point by horizontal coordinate.</returns>
    public DataPoint GetNearestX(Coordinates location, RenderDetails renderInfo, float maxDistance = 15) =>
        DataSourceUtilities.GetNearestXSmart(this, location, renderInfo, maxDistance);

    int IDataSource.GetXClosestIndex(Coordinates mouseLocation) => DataSourceUtilities.GetClosestIndex(_coordinates, mouseLocation, this.GetRenderIndexRange());

    Coordinates IDataSource.GetCoordinate(int index) => _coordinates[index];

    Coordinates IDataSource.GetCoordinateScaled(int index) => ((IDataSource)this).GetCoordinate(index);

    double IDataSource.GetX(int index) => _coordinates[index].X;

    double IDataSource.GetY(int index) => _coordinates[index].Y;

    double IDataSource.GetXScaled(int index) => ((IDataSource)this).GetX(index);

    double IDataSource.GetYScaled(int index) => ((IDataSource)this).GetY(index);

    bool IDataSource.IsSorted() => _coordinates.IsAscending(BinarySearchComparer.Instance);
}
