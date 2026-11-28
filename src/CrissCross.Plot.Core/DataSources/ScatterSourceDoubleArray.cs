// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>This data source manages X/Y points as separate X and Y collections.</summary>
/// <param name="xs">The horizontal coordinates.</param>
/// <param name="ys">The vertical coordinates.</param>
public class ScatterSourceDoubleArray(double[] xs, double[] ys) : IScatterSource, IDataSource, IGetNearest
{
    /// <summary>The horizontal coordinates.</summary>
    private readonly double[] _xs = xs;

    /// <summary>The vertical coordinates.</summary>
    private readonly double[] _ys = ys;

    /// <summary>Gets or sets the first point index included in rendering.</summary>
    public int MinRenderIndex { get; set; }

    /// <summary>Gets or sets the last point index included in rendering.</summary>
    public int MaxRenderIndex { get; set; } = Math.Min(xs.Length, ys.Length) - 1;

    bool IDataSource.PreferCoordinates => false;

    int IDataSource.Length => Math.Min(_xs.Length, _ys.Length);

    /// <summary>Gets the coordinates currently included in rendering.</summary>
    /// <returns>A read-only view of the visible coordinates.</returns>
    public IReadOnlyList<Coordinates> GetScatterPoints() => _xs.ZipView(_ys, static (x, y) => new Coordinates(x, y))
            .SkipView(MinRenderIndex)
            .TakeView(this.GetRenderIndexCount());

    /// <summary>Gets the axis limits of the visible coordinates.</summary>
    /// <returns>The axis limits.</returns>
    public AxisLimits GetLimits() => new(GetLimitsX(), GetLimitsY());

    /// <summary>Gets the horizontal range of the visible coordinates.</summary>
    /// <returns>The horizontal coordinate range.</returns>
    public CoordinateRange GetLimitsX() => CoordinateRange.Extrema(_xs.SkipView(MinRenderIndex).TakeView(this.GetRenderIndexCount()));

    /// <summary>Gets the vertical range of the visible coordinates.</summary>
    /// <returns>The vertical coordinate range.</returns>
    public CoordinateRange GetLimitsY() => CoordinateRange.Extrema(_ys.SkipView(MinRenderIndex).TakeView(this.GetRenderIndexCount()));

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

    int IDataSource.GetXClosestIndex(Coordinates mouseLocation) => DataSourceUtilities.GetClosestIndex(_xs, mouseLocation.X, this.GetRenderIndexRange());

    Coordinates IDataSource.GetCoordinate(int index) => new(_xs[index], _ys[index]);

    Coordinates IDataSource.GetCoordinateScaled(int index) => ((IDataSource)this).GetCoordinate(index);

    double IDataSource.GetX(int index) => _xs[index];

    double IDataSource.GetXScaled(int index) => ((IDataSource)this).GetX(index);

    double IDataSource.GetY(int index) => _ys[index];

    double IDataSource.GetYScaled(int index) => ((IDataSource)this).GetY(index);

    bool IDataSource.IsSorted() => _xs.IsAscending(BinarySearchComparer.Instance);
}
