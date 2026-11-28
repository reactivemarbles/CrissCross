// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Provides common axis-limit calculations for OHLC data sources.</summary>
public abstract class OHLCSourceBase : IOHLCSource
{
    /// <summary>Gets the number of OHLC values in the source.</summary>
    public abstract int Count { get; }

    /// <summary>Gets the OHLC values in this source.</summary>
    /// <returns>The OHLC values.</returns>
    public abstract IReadOnlyList<OHLC> GetOHLCs();

    /// <summary>Gets the horizontal and vertical limits of the source.</summary>
    /// <returns>The axis limits, or <see cref="AxisLimits.NoLimits"/> when the source is empty.</returns>
    public AxisLimits GetLimits() => Count > 0
        ? new AxisLimits(GetLimitsX(), GetLimitsY())
        : AxisLimits.NoLimits;

    /// <summary>Gets the horizontal range of the source timestamps.</summary>
    /// <returns>The earliest and latest timestamp coordinates.</returns>
    public CoordinateRange GetLimitsX()
    {
        IReadOnlyList<OHLC> values = GetOHLCs();
        if (values.Count == 0)
        {
            throw new InvalidOperationException("Sequence contains no elements");
        }

        DateTime minimumDate = values[0].DateTime;
        DateTime maximumDate = minimumDate;
        for (int i = 1; i < values.Count; i++)
        {
            DateTime date = values[i].DateTime;
            if (date < minimumDate)
            {
                minimumDate = date;
            }

            if (date > maximumDate)
            {
                maximumDate = date;
            }
        }

        return new(NumericConversion.ToNumber(minimumDate), NumericConversion.ToNumber(maximumDate));
    }

    /// <summary>Gets the vertical range spanning all low and high prices.</summary>
    /// <returns>The minimum low and maximum high prices.</returns>
    public CoordinateRange GetLimitsY()
    {
        IReadOnlyList<OHLC> values = GetOHLCs();
        if (values.Count == 0)
        {
            throw new InvalidOperationException("Sequence contains no elements");
        }

        double minimumPrice = values[0].Low;
        double maximumPrice = values[0].High;
        for (int i = 1; i < values.Count; i++)
        {
            minimumPrice = Math.Min(minimumPrice, values[i].Low);
            maximumPrice = Math.Max(maximumPrice, values[i].High);
        }

        return new(minimumPrice, maximumPrice);
    }

    /// <summary>Gets the vertical price range for an inclusive index range.</summary>
    /// <param name="index1">The first OHLC index, inclusive.</param>
    /// <param name="index2">The last OHLC index, inclusive.</param>
    /// <returns>The minimum low and maximum high price, or <see cref="CoordinateRange.NotSet"/> for an empty range.</returns>
    public CoordinateRange GetPriceRange(int index1, int index2)
    {
        if (index1 > index2)
        {
            return CoordinateRange.NotSet;
        }

        IReadOnlyList<OHLC> values = GetOHLCs();
        double minimumPrice = values[index1].Low;
        double maximumPrice = values[index1].High;
        for (int i = index1 + 1; i <= index2; i++)
        {
            minimumPrice = Math.Min(minimumPrice, values[i].Low);
            maximumPrice = Math.Max(maximumPrice, values[i].High);
        }

        return new(minimumPrice, maximumPrice);
    }
}
