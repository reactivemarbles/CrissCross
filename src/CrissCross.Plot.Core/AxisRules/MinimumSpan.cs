// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.AxisRules;

/// <summary>Constrains the spans of horizontal and vertical axes to minimum sizes.</summary>
/// <param name="xAxis">The horizontal axis to constrain.</param>
/// <param name="yAxis">The vertical axis to constrain.</param>
/// <param name="xSpan">The minimum horizontal span.</param>
/// <param name="ySpan">The minimum vertical span.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST1305",
    Justification = "The constructor parameter names are retained for existing named-argument callers.")]
public class MinimumSpan(IXAxis xAxis, IYAxis yAxis, double xSpan, double ySpan) : IAxisRule
{
    /// <summary>Gets or sets the minimum horizontal span.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Changing this public field to a property would break the existing binary API.")]
    public double XSpan = xSpan;

    /// <summary>Gets or sets the minimum vertical span.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Changing this public field to a property would break the existing binary API.")]
    public double YSpan = ySpan;

    /// <summary>Divisor used to calculate half of an axis span.</summary>
    private const double HalfSpanDivisor = 2;

    /// <summary>The horizontal axis constrained by this rule.</summary>
    private readonly IXAxis _horizontalAxis = xAxis;

    /// <summary>The vertical axis constrained by this rule.</summary>
    private readonly IYAxis _verticalAxis = yAxis;

    /// <summary>Applies this rule by expanding each axis to its configured minimum span.</summary>
    /// <param name="rp">The render pack containing the plot state.</param>
    /// <param name="beforeLayout">Whether the rule runs before layout.</param>
    public void Apply(RenderPack rp, bool beforeLayout)
    {
        if (_horizontalAxis.IsInverted())
        {
            if (Math.Abs(_horizontalAxis.Range.Span) < XSpan)
            {
                double minimumX = _horizontalAxis.Range.Center - (XSpan / HalfSpanDivisor);
                double maximumX = _horizontalAxis.Range.Center + (XSpan / HalfSpanDivisor);
                _horizontalAxis.Range.Set(maximumX, minimumX);
            }
        }
        else if (_horizontalAxis.Range.Span < XSpan)
        {
            double minimumX = _horizontalAxis.Range.Center - (XSpan / HalfSpanDivisor);
            double maximumX = _horizontalAxis.Range.Center + (XSpan / HalfSpanDivisor);
            _horizontalAxis.Range.Set(minimumX, maximumX);
        }

        if (_verticalAxis.IsInverted())
        {
            if (Math.Abs(_verticalAxis.Range.Span) < YSpan)
            {
                double minimumY = _verticalAxis.Range.Center - (YSpan / HalfSpanDivisor);
                double maximumY = _verticalAxis.Range.Center + (YSpan / HalfSpanDivisor);
                _verticalAxis.Range.Set(maximumY, minimumY);
            }
        }
        else if (_verticalAxis.Range.Span < YSpan)
        {
            double minimumY = _verticalAxis.Range.Center - (YSpan / HalfSpanDivisor);
            double maximumY = _verticalAxis.Range.Center + (YSpan / HalfSpanDivisor);
            _verticalAxis.Range.Set(minimumY, maximumY);
        }
    }
}
