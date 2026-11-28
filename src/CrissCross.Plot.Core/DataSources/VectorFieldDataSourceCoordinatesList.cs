// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Provides vector-field data from a list of rooted coordinate vectors.</summary>
/// <param name="rootedVectors">The rooted vectors exposed by this source.</param>
public class VectorFieldDataSourceCoordinatesList(IList<RootedCoordinateVector> rootedVectors) : IVectorFieldSource, IDataSource, IGetNearest
{
    /// <summary>The rooted vectors exposed by this source.</summary>
    private readonly IList<RootedCoordinateVector> _rootedVectors = rootedVectors;

    /// <summary>Gets or sets the first vector index included in rendering.</summary>
    public int MinRenderIndex { get; set; }

    /// <summary>Gets or sets the last vector index included in rendering.</summary>
    public int MaxRenderIndex { get; set; } = int.MaxValue;

    int IDataSource.Length => _rootedVectors.Count;

    bool IDataSource.PreferCoordinates => true;

    /// <summary>Gets the rooted vectors currently included in rendering.</summary>
    /// <returns>A snapshot of the visible rooted vectors.</returns>
    public IReadOnlyList<RootedCoordinateVector> GetRootedVectors()
    {
        int firstIndex = Math.Max(0, MinRenderIndex);
        int visibleCount = Math.Max(0, Math.Min(this.GetRenderIndexCount(), _rootedVectors.Count - firstIndex));
        List<RootedCoordinateVector> vectors = [with(capacity: visibleCount)];
        for (int i = 0; i < visibleCount; i++)
        {
            vectors.Add(_rootedVectors[firstIndex + i]);
        }

        return vectors;
    }

    /// <summary>Gets the axis limits of the visible rooted vectors.</summary>
    /// <returns>The axis limits.</returns>
    public AxisLimits GetLimits()
    {
        ExpandingAxisLimits limits = new();
        foreach (RootedCoordinateVector vector in GetRootedVectors())
        {
            limits.Expand(vector.Point);
        }

        return limits.AxisLimits;
    }

    /// <summary>Gets the horizontal range of the visible rooted vectors.</summary>
    /// <returns>The horizontal coordinate range.</returns>
    public CoordinateRange GetLimitsX() => GetLimits().Rect.XRange;

    /// <summary>Gets the vertical range of the visible rooted vectors.</summary>
    /// <returns>The vertical coordinate range.</returns>
    public CoordinateRange GetLimitsY() => GetLimits().Rect.YRange;

    /// <summary>Gets the data point nearest to a location.</summary>
    /// <param name="location">The location to compare against.</param>
    /// <param name="renderInfo">The current render details.</param>
    /// <param name="maxDistance">The maximum pixel distance allowed.</param>
    /// <returns>The nearest data point.</returns>
    public DataPoint GetNearest(Coordinates location, RenderDetails renderInfo, float maxDistance = 15) =>
        DataSourceUtilities.GetNearest(this, location, renderInfo, maxDistance);

    /// <summary>Gets the data point with the nearest horizontal coordinate.</summary>
    /// <param name="mouseLocation">The location to compare against.</param>
    /// <param name="renderInfo">The current render details.</param>
    /// <param name="maxDistance">The maximum pixel distance allowed.</param>
    /// <returns>The nearest data point by horizontal coordinate.</returns>
    public DataPoint GetNearestX(Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance = 15) =>
        DataSourceUtilities.GetNearestX(this, mouseLocation, renderInfo, maxDistance);

    /// <summary>Gets the index of the rooted vector closest to the specified location.</summary>
    /// <param name="mouseLocation">The location to compare against.</param>
    /// <returns>The index of the closest rooted vector.</returns>
    int IDataSource.GetXClosestIndex(Coordinates mouseLocation) =>
        _rootedVectors.GetClosestIndex(
            new RootedCoordinateVector(mouseLocation, default),
            this.GetRenderIndexRange(),
            BinarySearchComparer.Instance);

    Coordinates IDataSource.GetCoordinate(int index) => _rootedVectors[index].Point;

    Coordinates IDataSource.GetCoordinateScaled(int index) => ((IDataSource)this).GetCoordinate(index);

    double IDataSource.GetX(int index) => _rootedVectors[index].Point.X;

    double IDataSource.GetXScaled(int index) => ((IDataSource)this).GetX(index);

    double IDataSource.GetY(int index) => _rootedVectors[index].Point.Y;

    double IDataSource.GetYScaled(int index) => ((IDataSource)this).GetY(index);

    bool IDataSource.IsSorted() => _rootedVectors.IsAscending(BinarySearchComparer.Instance);
}
