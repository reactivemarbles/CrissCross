// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Decorates a scatter source with cached axis limits.</summary>
/// <param name="source">The source whose limits are cached.</param>
public class CacheScatterLimitsDecorator(IScatterSource source) : IScatterSource, IGetNearest
{
    /// <summary>The wrapped scatter source.</summary>
    private readonly IScatterSource _source = source;

    /// <summary>The cached complete axis limits.</summary>
    private AxisLimits? _axisLimits;

    /// <summary>The cached horizontal limits.</summary>
    private CoordinateRange _limitsX = CoordinateRange.NotSet;

    /// <summary>The cached vertical limits.</summary>
    private CoordinateRange _limitsY = CoordinateRange.NotSet;

    /// <summary>Gets or sets the first point index included in rendering.</summary>
    public int MinRenderIndex
    {
        get => _source.MinRenderIndex;
        set => _source.MinRenderIndex = value;
    }

    /// <summary>Gets or sets the last point index included in rendering.</summary>
    public int MaxRenderIndex
    {
        get => _source.MaxRenderIndex;
        set => _source.MaxRenderIndex = value;
    }

    /// <summary>Gets the cached axis limits.</summary>
    /// <returns>The axis limits.</returns>
    public AxisLimits GetLimits()
    {
        _axisLimits ??= _source.GetLimits();

        return _axisLimits.Value;
    }

    /// <summary>Gets the cached horizontal limits.</summary>
    /// <returns>The horizontal coordinate range.</returns>
    public CoordinateRange GetLimitsX()
    {
        if (_limitsX == CoordinateRange.NotSet)
        {
            _limitsX = _source.GetLimitsX();
        }

        return _limitsX;
    }

    /// <summary>Gets the cached vertical limits.</summary>
    /// <returns>The vertical coordinate range.</returns>
    public CoordinateRange GetLimitsY()
    {
        if (_limitsY == CoordinateRange.NotSet)
        {
            _limitsY = _source.GetLimitsY();
        }

        return _limitsY;
    }

    /// <summary>Gets the data point nearest to a location.</summary>
    /// <param name="location">The location to compare against.</param>
    /// <param name="renderInfo">The current render details.</param>
    /// <param name="maxDistance">The maximum pixel distance allowed.</param>
    /// <returns>The nearest data point.</returns>
    public DataPoint GetNearest(Coordinates location, RenderDetails renderInfo, float maxDistance = 15) => _source.GetNearest(location, renderInfo, maxDistance);

    /// <summary>Gets the data point with the nearest horizontal coordinate.</summary>
    /// <param name="location">The location to compare against.</param>
    /// <param name="renderInfo">The current render details.</param>
    /// <param name="maxDistance">The maximum pixel distance allowed.</param>
    /// <returns>The nearest data point by horizontal coordinate.</returns>
    public DataPoint GetNearestX(Coordinates location, RenderDetails renderInfo, float maxDistance = 15) => _source.GetNearestX(location, renderInfo, maxDistance);

    /// <summary>Gets the scatter points from the wrapped source.</summary>
    /// <returns>The scatter points.</returns>
    public IReadOnlyList<Coordinates> GetScatterPoints() => _source.GetScatterPoints();
}
