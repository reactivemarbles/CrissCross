// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Provides signal-source behavior for values stored in a read-only list.</summary>
/// <typeparam name="T">The type of the signal values.</typeparam>
public class SignalSourceGenericList<T> : SignalSourceBase, ISignalSource, IDataSource
{
    /// <summary>The signal values indexed by this source.</summary>
    private readonly IReadOnlyList<T> _ys;

    /// <summary>Initializes a new instance of the <see cref="SignalSourceGenericList{T}"/> class.</summary>
    /// <param name="ys">The signal values.</param>
    /// <param name="period">The horizontal coordinate distance between values.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2241",
        Justification = "The constructor assigns the inherited Period property, which cannot be initialized in a storage initializer.")]
    public SignalSourceGenericList(IReadOnlyList<T> ys, double period)
    {
        _ys = ys;
        Period = period;
    }

    /// <summary>Gets the number of signal values.</summary>
    public override int Length => _ys.Count;

    bool IDataSource.PreferCoordinates => false;

    /// <summary>Gets signal values converted to double precision.</summary>
    /// <returns>The converted values.</returns>
    public IReadOnlyList<double> GetYs() => NumericConversion.GenericToDoubleArray(_ys);

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
            T genericValue = _ys[i];
            yield return NumericConversion.GenericToDouble(ref genericValue);
        }
    }

    /// <summary>Gets the signal value at the specified index.</summary>
    /// <param name="index">The signal index.</param>
    /// <returns>The signal value converted to double precision.</returns>
    public double GetY(int index)
    {
        T genericValue = _ys[index];
        return NumericConversion.GenericToDouble(ref genericValue);
    }

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
            T genericValue = _ys[i];
            double value = NumericConversion.GenericToDouble(ref genericValue);
            min = Math.Min(min, value);
            max = Math.Max(max, value);
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
        double maximumHorizontalCoordinate = minimumHorizontalCoordinate + horizontalUnitsPerPixel;

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
        float entryPixelY = axes.GetPixelY((NumericConversion.GenericToDouble(_ys, firstIndex) * YScale) + YOffset);
        float exitPixelY = axes.GetPixelY((NumericConversion.GenericToDouble(_ys, lastIndex) * YScale) + YOffset);
        double minimumVerticalValue = double.PositiveInfinity;
        double maximumVerticalValue = double.NegativeInfinity;
        for (int i = firstIndex; i <= lastIndex; i++)
        {
            double value = NumericConversion.GenericToDouble(_ys, i);
            minimumVerticalValue = Math.Min(minimumVerticalValue, value);
            maximumVerticalValue = Math.Max(maximumVerticalValue, value);
        }

        minimumVerticalValue = (minimumVerticalValue * YScale) + YOffset;
        maximumVerticalValue = (maximumVerticalValue * YScale) + YOffset;
        float bottomPixelY = axes.GetPixelY(minimumVerticalValue);
        float topPixelY = axes.GetPixelY(maximumVerticalValue);

        return new(pixelXCoordinate, entryPixelY, exitPixelY, bottomPixelY, topPixelY);
    }

    int IDataSource.GetXClosestIndex(Coordinates mouseLocation) => GetIndex(mouseLocation.X, true);

    Coordinates IDataSource.GetCoordinate(int index) => new(index * Period, NumericConversion.GenericToDouble(_ys, index));

    Coordinates IDataSource.GetCoordinateScaled(int index) => new((index * Period) + XOffset, DataSourceUtilities.ScaleXY(_ys, index, YScale, YOffset));

    double IDataSource.GetX(int index) => index * Period;

    double IDataSource.GetXScaled(int index) => (index * Period) + XOffset;

    double IDataSource.GetY(int index) => GetY(index);

    double IDataSource.GetYScaled(int index) => DataSourceUtilities.ScaleXY(_ys, index, YScale, YOffset);

    bool IDataSource.IsSorted() => true;
}
