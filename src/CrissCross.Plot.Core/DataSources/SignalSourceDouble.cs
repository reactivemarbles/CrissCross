// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Provides signal-source behavior for double-precision values.</summary>
public class SignalSourceDouble : SignalSourceBase, ISignalSource, IDataSource
{
    /// <summary>The signal values indexed by this source.</summary>
    private readonly IReadOnlyList<double> _ys;

    /// <summary>Initializes a new instance of the <see cref="SignalSourceDouble"/> class.</summary>
    /// <param name="ys">The signal values.</param>
    /// <param name="period">The horizontal coordinate distance between values.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2241",
        Justification = "The constructor assigns the inherited Period property, which cannot be initialized in a storage initializer.")]
    public SignalSourceDouble(IReadOnlyList<double> ys, double period)
    {
        _ys = ys;
        Period = period;
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
    /// <returns>The minimum and maximum signal values in the range.</returns>
    public override SignalRangeY GetLimitsY(int firstIndex, int lastIndex)
    {
        double min = double.PositiveInfinity;
        double max = double.NegativeInfinity;

        for (int i = firstIndex; i <= lastIndex; i++)
        {
            min = Math.Min(min, _ys[i]);
            max = Math.Max(max, _ys[i]);
        }

        return new(min, max);
    }

    /// <summary>Gets the signal bounds represented by a horizontal pixel column.</summary>
    /// <param name="axes">The axes used to transform coordinates and pixels.</param>
    /// <param name="xPixelIndex">The horizontal pixel index within the data rectangle.</param>
    /// <returns>The pixel column and its vertical signal bounds.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1305",
        Justification = "The parameter name matches the implemented interface contract.")]
    public PixelColumn GetPixelColumn(IAxes axes, int xPixelIndex)
    {
        const double pixelOverlapFraction = 0.01;
        float pixelXCoordinate = axes.DataRect.Left + xPixelIndex;
        double minimumHorizontalCoordinate = axes.GetCoordinateX(pixelXCoordinate);
        float horizontalUnitsPerPixel = (float)(axes.XAxis.Width / axes.DataRect.Width);
        double maximumHorizontalCoordinate = minimumHorizontalCoordinate + Math.Abs(horizontalUnitsPerPixel);

        if (UsePixelOverlap)
        {
            maximumHorizontalCoordinate += horizontalUnitsPerPixel * pixelOverlapFraction;
        }

        if (!RangeContainsSignal(minimumHorizontalCoordinate, maximumHorizontalCoordinate))
        {
            return PixelColumn.WithoutData(pixelXCoordinate);
        }

        int firstIndex = GetIndex(minimumHorizontalCoordinate, true);
        int lastIndex = GetIndex(maximumHorizontalCoordinate, true);
        float entryPixelY = axes.GetPixelY((_ys[firstIndex] * YScale) + YOffset);
        float exitPixelY = axes.GetPixelY((_ys[lastIndex] * YScale) + YOffset);
        SignalRangeY verticalRange = GetLimitsY(firstIndex, lastIndex);
        float bottomPixelY = axes.GetPixelY((verticalRange.Min * YScale) + YOffset);
        float topPixelY = axes.GetPixelY((verticalRange.Max * YScale) + YOffset);

        return new(pixelXCoordinate, entryPixelY, exitPixelY, bottomPixelY, topPixelY);
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
