// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.AxisRules;

/// <summary>Adjusts the horizontal range to preserve square data units on screen.</summary>
/// <param name="xAxis">The horizontal axis whose range is adjusted.</param>
/// <param name="yAxis">The vertical axis used to calculate scale.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST1305",
    Justification = "The constructor parameter names are retained for existing named-argument callers.")]
public class SquarePreserveY(IXAxis xAxis, IYAxis yAxis) : IAxisRule
{
    /// <summary>Divisor used to calculate half of the plot width.</summary>
    private const double HalfSpanDivisor = 2;

    /// <summary>The horizontal axis whose range is adjusted.</summary>
    private readonly IXAxis _horizontalAxis = xAxis;

    /// <summary>The vertical axis used to calculate scale.</summary>
    private readonly IYAxis _verticalAxis = yAxis;

    /// <summary>Applies this rule after layout to preserve square data units.</summary>
    /// <param name="rp">The render pack containing the data rectangle.</param>
    /// <param name="beforeLayout">Whether the rule runs before layout.</param>
    public void Apply(RenderPack rp, bool beforeLayout)
    {
        if (beforeLayout)
        {
            return;
        }

        double coordinateUnitsPerPixelY = _verticalAxis.Height / rp.DataRect.Height;
        double halfWidth = rp.DataRect.Width / HalfSpanDivisor * coordinateUnitsPerPixelY;
        double minimumHorizontalCoordinate = _horizontalAxis.Range.Center - halfWidth;
        double maximumHorizontalCoordinate = _horizontalAxis.Range.Center + halfWidth;
        _horizontalAxis.Range.Set(minimumHorizontalCoordinate, maximumHorizontalCoordinate);
    }
}
