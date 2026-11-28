// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CrissCross.Plot.AutoScalers;

/// <summary>Scales axes using configurable fractional margins around plotted data.</summary>
public class FractionalAutoScaler : IAutoScaler
{
    /// <summary>The upper limit used when data has no valid horizontal or vertical range.</summary>
    private const double DefaultMaximum = 10;

    /// <summary>The lower limit used when data has no valid horizontal or vertical range.</summary>
    private const double DefaultMinimum = -10;

    /// <summary>The factor used to divide a total margin evenly between two sides.</summary>
    private const double FractionSplit = 0.5;

    /// <summary>The amount used to expand a range containing a single value.</summary>
    private const double SingleValuePadding = 1;

    /// <summary>Initializes a new instance of the <see cref="FractionalAutoScaler"/> class.</summary>
    /// <param name="horizontal">The total horizontal margin as a fraction of the data span.</param>
    /// <param name="vertical">The total vertical margin as a fraction of the data span.</param>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2309", Justification = "Optional defaults are part of the existing public API.")]
    public FractionalAutoScaler(double horizontal = .1, double vertical = .15)
    {
        LeftFraction = horizontal * FractionSplit;
        RightFraction = horizontal * FractionSplit;
        BottomFraction = vertical * FractionSplit;
        TopFraction = vertical * FractionSplit;
    }

    /// <summary>Initializes a new instance of the <see cref="FractionalAutoScaler"/> class with individual margins.</summary>
    /// <param name="left">The left margin fraction.</param>
    /// <param name="right">The right margin fraction.</param>
    /// <param name="bottom">The bottom margin fraction.</param>
    /// <param name="top">The top margin fraction.</param>
    public FractionalAutoScaler(double left, double right, double bottom, double top)
    {
        LeftFraction = left;
        RightFraction = right;
        BottomFraction = bottom;
        TopFraction = top;
    }

    /// <summary>Gets the fraction of the horizontal range reserved for the left margin.</summary>
    public double LeftFraction { get; private set; }

    /// <summary>Gets the fraction of the horizontal range reserved for the right margin.</summary>
    public double RightFraction { get; private set; }

    /// <summary>Gets the fraction of the vertical range reserved for the bottom margin.</summary>
    public double BottomFraction { get; private set; }

    /// <summary>Gets the fraction of the vertical range reserved for the top margin.</summary>
    public double TopFraction { get; private set; }

    /// <summary>Gets or sets a value indicating whether the horizontal axis is inverted.</summary>
    public bool InvertedX { get; set; }

    /// <summary>Gets or sets a value indicating whether the vertical axis is inverted.</summary>
    public bool InvertedY { get; set; }

    /// <summary>Sets equal left and right margin fractions.</summary>
    /// <param name="horizontal">The total horizontal margin as a fraction of the data span.</param>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2309", Justification = "The optional default is part of the existing public API.")]
    public void SetMarginsX(double horizontal = .1)
    {
        LeftFraction = horizontal * FractionSplit;
        RightFraction = horizontal * FractionSplit;
    }

    /// <summary>Sets equal bottom and top margin fractions.</summary>
    /// <param name="vertical">The total vertical margin as a fraction of the data span.</param>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2309", Justification = "The optional default is part of the existing public API.")]
    public void SetMarginsY(double vertical = .15)
    {
        BottomFraction = vertical * FractionSplit;
        TopFraction = vertical * FractionSplit;
    }

    /// <summary>Automatically scales every distinct axis used by the supplied plottables.</summary>
    /// <param name="plottables">The plottables whose visible data determines the axis ranges.</param>
    public void AutoScaleAll(IEnumerable<IPlottable> plottables)
    {
        List<IPlottable> plotItems = new(plottables);

        List<IXAxis> horizontalAxes = [];
        HashSet<IXAxis> seenHorizontalAxes = [];
        List<IYAxis> verticalAxes = [];
        HashSet<IYAxis> seenVerticalAxes = [];

        foreach (IPlottable plottable in plotItems)
        {
            IXAxis horizontalAxis = plottable.Axes.XAxis;
            if (seenHorizontalAxes.Add(horizontalAxis))
            {
                horizontalAxes.Add(horizontalAxis);
            }

            IYAxis verticalAxis = plottable.Axes.YAxis;
            if (seenVerticalAxes.Add(verticalAxis))
            {
                verticalAxes.Add(verticalAxis);
            }
        }

        ResetAxes(horizontalAxes);
        ResetAxes(verticalAxes);
        ExpandRanges(plotItems);

        foreach (IXAxis horizontalAxis in horizontalAxes)
        {
            ApplyHorizontalMargins(horizontalAxis);
        }

        foreach (IYAxis verticalAxis in verticalAxes)
        {
            ApplyVerticalMargins(verticalAxis);
        }
    }

    /// <summary>Gets the data limits for a plot, expanded by the configured margins.</summary>
    /// <param name="plot">The plot whose data limits are requested.</param>
    /// <param name="xAxis">The horizontal axis for which to calculate limits.</param>
    /// <param name="yAxis">The vertical axis for which to calculate limits.</param>
    /// <returns>The expanded axis limits, accounting for configured axis inversion.</returns>
    /// <exception cref="InvalidOperationException">The calculated limits are not valid or do not enclose an area.</exception>
    [SuppressMessage("RoslynCommonAnalyzers", "SST1305", Justification = "These parameter names are part of the existing public API.")]
    public AxisLimits GetAxisLimits(Plot plot, IXAxis xAxis, IYAxis yAxis)
    {
        AxisLimits dataLimits = plot.Axes.GetDataLimits(xAxis, yAxis);
        ExpandingAxisLimits limits = CreateExpandedLimits(in dataLimits);
        AxisLimits newLimits = new(
            left: limits.Left - (limits.HorizontalSpan * LeftFraction),
            right: limits.Right + (limits.HorizontalSpan * RightFraction),
            bottom: limits.Bottom - (limits.VerticalSpan * BottomFraction),
            top: limits.Top + (limits.VerticalSpan * TopFraction));

        if (!newLimits.IsReal || !newLimits.HasArea)
        {
            throw new InvalidOperationException("limits returned by the autoscaler must always be in a good state");
        }

        if (InvertedX)
        {
            newLimits = newLimits.InvertedHorizontally();
        }

        if (InvertedY)
        {
            newLimits = newLimits.InvertedVertically();
        }

        return newLimits;
    }

    /// <summary>Creates finite limits and expands any range that contains only one value.</summary>
    /// <param name="dataLimits">The original data limits.</param>
    /// <returns>Finite limits with non-zero spans.</returns>
    private static ExpandingAxisLimits CreateExpandedLimits(in AxisLimits dataLimits)
    {
        ExpandingAxisLimits limits = new(dataLimits);

        if (!limits.IsRealX)
        {
            limits.SetX(DefaultMinimum, DefaultMaximum);
        }

        if (!limits.IsRealY)
        {
            limits.SetY(DefaultMinimum, DefaultMaximum);
        }

        ExpandDegenerateRanges(limits);
        return limits;
    }

    /// <summary>Expands zero-width horizontal and vertical ranges.</summary>
    /// <param name="limits">The limits to expand.</param>
    private static void ExpandDegenerateRanges(ExpandingAxisLimits limits)
    {
        ExpandDegenerateHorizontalRange(limits);
        ExpandDegenerateVerticalRange(limits);
    }

    /// <summary>Expands a horizontal range that contains only one value.</summary>
    /// <param name="limits">The limits to expand.</param>
    private static void ExpandDegenerateHorizontalRange(ExpandingAxisLimits limits)
    {
        if (!AreEqual(limits.Left, limits.Right))
        {
            return;
        }

        limits.SetX(limits.Left - SingleValuePadding, limits.Right + SingleValuePadding);
        if (!AreEqual(limits.Left, limits.Right))
        {
            return;
        }

        limits.Left = NumericConversion.DecrementLargeDouble(limits.Left);
        limits.Right = NumericConversion.IncrementLargeDouble(limits.Right);
    }

    /// <summary>Expands a vertical range that contains only one value.</summary>
    /// <param name="limits">The limits to expand.</param>
    private static void ExpandDegenerateVerticalRange(ExpandingAxisLimits limits)
    {
        if (!AreEqual(limits.Bottom, limits.Top))
        {
            return;
        }

        limits.SetY(limits.Bottom - SingleValuePadding, limits.Top + SingleValuePadding);
        if (!AreEqual(limits.Bottom, limits.Top))
        {
            return;
        }

        limits.Bottom = NumericConversion.DecrementLargeDouble(limits.Bottom);
        limits.Top = NumericConversion.IncrementLargeDouble(limits.Top);
    }

    /// <summary>Compares finite values for exact equality without applying a range tolerance.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the values are equal.</returns>
    private static bool AreEqual(double left, double right) => left.CompareTo(right) is 0;

    /// <summary>Resets each axis to its default range.</summary>
    /// <typeparam name="TAxis">The axis type.</typeparam>
    /// <param name="axes">The axes to reset.</param>
    private static void ResetAxes<TAxis>(List<TAxis> axes)
        where TAxis : IAxis
    {
        foreach (TAxis axis in axes)
        {
            axis.Range.Reset();
        }
    }

    /// <summary>Expands axis ranges to include each visible plottable.</summary>
    /// <param name="plotItems">The plottables to include.</param>
    private static void ExpandRanges(List<IPlottable> plotItems)
    {
        foreach (IPlottable plottable in plotItems)
        {
            if (!plottable.IsVisible)
            {
                continue;
            }

            AxisLimits limits = plottable.GetAxisLimits();
            plottable.Axes.XAxis.Range.Expand(limits.XRange);
            plottable.Axes.YAxis.Range.Expand(limits.YRange);
        }
    }

    /// <summary>Applies horizontal margins and inversion to an axis range.</summary>
    /// <param name="horizontalAxis">The axis to update.</param>
    private void ApplyHorizontalMargins(IXAxis horizontalAxis)
    {
        double left = horizontalAxis.Range.Min - (horizontalAxis.Range.Span * LeftFraction);
        double right = horizontalAxis.Range.Max + (horizontalAxis.Range.Span * RightFraction);
        if (!NumericConversion.IsReal(left) || !NumericConversion.IsReal(right))
        {
            return;
        }

        if (AreEqual(left, right))
        {
            left--;
            right++;
        }

        if (InvertedX)
        {
            horizontalAxis.Range.Set(right, left);
        }
        else
        {
            horizontalAxis.Range.Set(left, right);
        }
    }

    /// <summary>Applies vertical margins and inversion to an axis range.</summary>
    /// <param name="verticalAxis">The axis to update.</param>
    private void ApplyVerticalMargins(IYAxis verticalAxis)
    {
        double bottom = verticalAxis.Range.Min - (verticalAxis.Range.Span * BottomFraction);
        double top = verticalAxis.Range.Max + (verticalAxis.Range.Span * TopFraction);
        if (!NumericConversion.IsReal(bottom) || !NumericConversion.IsReal(top))
        {
            return;
        }

        if (AreEqual(bottom, top))
        {
            bottom--;
            top++;
        }

        if (InvertedY)
        {
            verticalAxis.Range.Set(top, bottom);
        }
        else
        {
            verticalAxis.Range.Set(bottom, top);
        }
    }
}
