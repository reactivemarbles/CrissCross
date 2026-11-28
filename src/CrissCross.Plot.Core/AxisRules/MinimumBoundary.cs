// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.AxisRules;

/// <summary>Constrains axes to at least the specified boundary limits.</summary>
/// <param name="xAxis">The horizontal axis to constrain.</param>
/// <param name="yAxis">The vertical axis to constrain.</param>
/// <param name="limits">The minimum axis limits.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST1305",
    Justification = "The constructor parameter names are retained for existing named-argument callers.")]
public class MinimumBoundary(IXAxis xAxis, IYAxis yAxis, AxisLimits limits) : IAxisRule
{
    /// <summary>The horizontal axis constrained by this rule.</summary>
    private readonly IXAxis _horizontalAxis = xAxis;

    /// <summary>The vertical axis constrained by this rule.</summary>
    private readonly IYAxis _verticalAxis = yAxis;

    /// <summary>Gets or sets the minimum limits for the axes.</summary>
    public AxisLimits Limits { get; set; } = limits;

    /// <summary>Applies the minimum boundary to both configured axes.</summary>
    /// <param name="rp">The render pack containing the plot state.</param>
    /// <param name="beforeLayout">Whether the rule runs before layout.</param>
    public void Apply(RenderPack rp, bool beforeLayout)
    {
        CoordinateRange horizontalLimits = Limits.XRange;
        CoordinateRange verticalLimits = Limits.YRange;
        AdjustAxis(_horizontalAxis, in horizontalLimits);
        AdjustAxis(_verticalAxis, in verticalLimits);
    }

    /// <summary>Adjusts an axis to remain within its configured limits without reducing its span.</summary>
    /// <param name="axis">The axis to adjust.</param>
    /// <param name="limits">The minimum coordinate range.</param>
    private static void AdjustAxis(IAxis axis, in CoordinateRange limits)
    {
        double span = Math.Max(Math.Abs(axis.Range.Span), limits.Span);
        if (axis.IsInverted())
        {
            AdjustInvertedAxis(axis.Range, in limits, span);
        }
        else
        {
            AdjustNormalAxis(axis.Range, in limits, span);
        }
    }

    /// <summary>Adjusts an inverted axis to remain within its configured limits.</summary>
    /// <param name="range">The mutable axis range.</param>
    /// <param name="limits">The minimum coordinate range.</param>
    /// <param name="span">The span to preserve.</param>
    private static void AdjustInvertedAxis(CoordinateRangeMutable range, in CoordinateRange limits, double span)
    {
        AdjustInvertedMinimum(range, limits.Max, span);
        AdjustInvertedMaximum(range, limits.Min, span);
    }

    /// <summary>Moves the inverted-axis minimum to its allowed boundary when necessary.</summary>
    /// <param name="range">The mutable axis range.</param>
    /// <param name="boundary">The maximum allowed minimum coordinate.</param>
    /// <param name="span">The span to preserve.</param>
    private static void AdjustInvertedMinimum(CoordinateRangeMutable range, double boundary, double span)
    {
        if (!(range.Min < boundary))
        {
            return;
        }

        range.Min = boundary;
        range.Max = boundary - span;
    }

    /// <summary>Moves the inverted-axis maximum to its allowed boundary when necessary.</summary>
    /// <param name="range">The mutable axis range.</param>
    /// <param name="boundary">The minimum allowed maximum coordinate.</param>
    /// <param name="span">The span to preserve.</param>
    private static void AdjustInvertedMaximum(CoordinateRangeMutable range, double boundary, double span)
    {
        if (!(range.Max > boundary))
        {
            return;
        }

        range.Max = boundary;
        range.Min = boundary + span;
    }

    /// <summary>Adjusts a normal axis to remain within its configured limits.</summary>
    /// <param name="range">The mutable axis range.</param>
    /// <param name="limits">The minimum coordinate range.</param>
    /// <param name="span">The span to preserve.</param>
    private static void AdjustNormalAxis(CoordinateRangeMutable range, in CoordinateRange limits, double span)
    {
        AdjustNormalMaximum(range, limits.Max, span);
        AdjustNormalMinimum(range, limits.Min, span);
    }

    /// <summary>Moves the normal-axis maximum to its allowed boundary when necessary.</summary>
    /// <param name="range">The mutable axis range.</param>
    /// <param name="boundary">The minimum allowed maximum coordinate.</param>
    /// <param name="span">The span to preserve.</param>
    private static void AdjustNormalMaximum(CoordinateRangeMutable range, double boundary, double span)
    {
        if (!(range.Max < boundary))
        {
            return;
        }

        range.Max = boundary;
        range.Min = boundary - span;
    }

    /// <summary>Moves the normal-axis minimum to its allowed boundary when necessary.</summary>
    /// <param name="range">The mutable axis range.</param>
    /// <param name="boundary">The maximum allowed minimum coordinate.</param>
    /// <param name="span">The span to preserve.</param>
    private static void AdjustNormalMinimum(CoordinateRangeMutable range, double boundary, double span)
    {
        if (!(range.Min > boundary))
        {
            return;
        }

        range.Min = boundary;
        range.Max = boundary + span;
    }
}
