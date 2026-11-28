// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Provides a fast signal source backed by double-precision Y values.</summary>
public class FastSignalSourceDouble : SignalSourceBase, ISignalSource, IDataSource
{
    /// <summary>The fractional overlap added to each pixel column to avoid missing values.</summary>
    private const double PixelOverlapFraction = 0.01;

    /// <summary>The signal values indexed by this source.</summary>
    private readonly IReadOnlyList<double> _ys;

    /// <summary>The cache used to retrieve signal extrema efficiently.</summary>
    private readonly MinMaxCache _minMaxCache;

    /// <summary>Initializes a new instance of the <see cref="FastSignalSourceDouble"/> class using regularly spaced double-precision Y values.</summary>
    /// <param name="ys">The signal values.</param>
    /// <param name="period">The horizontal coordinate distance between values.</param>
    /// <param name="cachePeriod">The number of values grouped into each min/max cache entry.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The optional cache size is retained to preserve the established source and binary API.")]
    public FastSignalSourceDouble(IReadOnlyList<double> ys, double period, int cachePeriod = 1000)
    {
        _ys = ys;
        Period = period;
        _minMaxCache = new(_ys, cachePeriod);
    }

    /// <summary>Gets the number of signal values.</summary>
    public override int Length => _ys.Count;

    bool IDataSource.PreferCoordinates => false;

    /// <summary>Gets all signal values.</summary>
    /// <returns>The underlying read-only signal values.</returns>
    public IReadOnlyList<double> GetYs() => _ys;

    /// <summary>Enumerates signal values within the specified index range.</summary>
    /// <param name="index1">The first index, inclusive.</param>
    /// <param name="index2">The last index, inclusive.</param>
    /// <returns>The values in the requested range.</returns>
    public IEnumerable<double> GetYs(int index1, int index2)
    {
        index1 = Math.Max(index1, MinRenderIndex);
        index2 = Math.Min(index2, MaxRenderIndex);
        for (int i = index1; i <= index2; i++)
        {
            yield return _ys[i];
        }
    }

    /// <summary>Gets the signal value at the specified index.</summary>
    /// <param name="index">The signal index.</param>
    /// <returns>The signal value.</returns>
    public double GetY(int index) => _ys[index];

    /// <summary>Gets the minimum and maximum signal values in an index range.</summary>
    /// <param name="firstIndex">The first index, inclusive.</param>
    /// <param name="lastIndex">The last index, inclusive.</param>
    /// <returns>The minimum and maximum values in the range.</returns>
    public override SignalRangeY GetLimitsY(int firstIndex, int lastIndex) => _minMaxCache.GetMinMax(firstIndex, lastIndex + 1);

    /// <summary>Gets the signal range represented by a horizontal pixel column.</summary>
    /// <param name="axes">The axes used to transform coordinates and pixels.</param>
    /// <param name="xPixelIndex">The horizontal pixel index within the data rectangle.</param>
    /// <returns>The pixel column and its vertical signal bounds.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1305",
        Justification = "The parameter name matches the implemented interface contract.")]
    public PixelColumn GetPixelColumn(IAxes axes, int xPixelIndex)
    {
        float pixelX = axes.DataRect.Left + xPixelIndex;
        double coordinateMinimum = axes.GetCoordinateX(pixelX);
        float unitsPerPixelX = (float)(axes.XAxis.Width / axes.DataRect.Width);
        double coordinateMaximum = coordinateMinimum + Math.Abs(unitsPerPixelX);

        // Add slight overlap to prevent floating-point errors from missing points.
        coordinateMaximum += unitsPerPixelX * PixelOverlapFraction;

        if (!RangeContainsSignal(coordinateMinimum, coordinateMaximum))
        {
            return PixelColumn.WithoutData(pixelX);
        }

        int firstIndex = GetIndex(coordinateMinimum, true);
        int lastIndex = GetIndex(coordinateMaximum, true);

        float pixelYEnter = axes.GetPixelY(_ys[firstIndex] + YOffset);
        float pixelYExit = axes.GetPixelY(_ys[lastIndex] + YOffset);
        SignalRangeY rangeY = GetLimitsY(firstIndex, lastIndex);
        float pixelYBottom = axes.GetPixelY(rangeY.Min + YOffset);
        float pixelYTop = axes.GetPixelY(rangeY.Max + YOffset);

        return new(pixelX, pixelYEnter, pixelYExit, pixelYBottom, pixelYTop);
    }

    int IDataSource.GetXClosestIndex(Coordinates mouseLocation) => GetIndex(mouseLocation.X, true);

    Coordinates IDataSource.GetCoordinate(int index) => new(index * Period, _ys[index]);

    Coordinates IDataSource.GetCoordinateScaled(int index) => new((index * Period) + XOffset, DataSourceUtilities.ScaleXY(_ys[index], YScale, YOffset));

    double IDataSource.GetX(int index) => index * Period;

    double IDataSource.GetXScaled(int index) => (index * Period) + XOffset;

    double IDataSource.GetY(int index) => GetY(index);

    double IDataSource.GetYScaled(int index) => DataSourceUtilities.ScaleXY(_ys[index], YScale, YOffset);

    bool IDataSource.IsSorted() => true;
}
