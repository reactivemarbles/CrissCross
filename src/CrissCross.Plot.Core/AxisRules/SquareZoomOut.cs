// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.AxisRules;

/// <summary>Expands axis limits to preserve square pixels when zooming out.</summary>
/// <param name="xAxis">The horizontal axis to adjust.</param>
/// <param name="yAxis">The vertical axis to adjust.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST1305",
    Justification = "The constructor parameter names are retained for existing named-argument callers.")]
public class SquareZoomOut(IXAxis xAxis, IYAxis yAxis) : IAxisRule
{
    /// <summary>Divisor used to calculate half of a data rectangle dimension.</summary>
    private const double HalfDimensionDivisor = 2;

    /// <summary>The horizontal axis to adjust.</summary>
    private readonly IXAxis _horizontalAxis = xAxis;

    /// <summary>The vertical axis to adjust.</summary>
    private readonly IYAxis _verticalAxis = yAxis;

    /// <summary>Applies this rule after layout to expand the axis ranges symmetrically.</summary>
    /// <param name="rp">The render pack containing the data rectangle.</param>
    /// <param name="beforeLayout">Whether the rule runs before layout.</param>
    public void Apply(RenderPack rp, bool beforeLayout)
    {
        if (beforeLayout)
        {
            return;
        }

        double unitsPerPixelX = Math.Abs(_horizontalAxis.Width / rp.DataRect.Width);
        double unitsPerPixelY = Math.Abs(_verticalAxis.Height / rp.DataRect.Height);
        double maximumUnitsPerPixel = Math.Max(unitsPerPixelX, unitsPerPixelY);

        double halfHeight = rp.DataRect.Height / HalfDimensionDivisor * maximumUnitsPerPixel;
        double minimumVerticalCoordinate = _verticalAxis.Range.Center - halfHeight;
        double maximumVerticalCoordinate = _verticalAxis.Range.Center + halfHeight;
        bool verticalAxisInverted = _verticalAxis.Min > _verticalAxis.Max;
        if (verticalAxisInverted)
        {
            _verticalAxis.Range.Set(maximumVerticalCoordinate, minimumVerticalCoordinate);
        }
        else
        {
            _verticalAxis.Range.Set(minimumVerticalCoordinate, maximumVerticalCoordinate);
        }

        double halfWidth = rp.DataRect.Width / HalfDimensionDivisor * maximumUnitsPerPixel;
        double minimumHorizontalCoordinate = _horizontalAxis.Range.Center - halfWidth;
        double maximumHorizontalCoordinate = _horizontalAxis.Range.Center + halfWidth;
        bool horizontalAxisInverted = _horizontalAxis.Min > _horizontalAxis.Max;
        if (horizontalAxisInverted)
        {
            _horizontalAxis.Range.Set(maximumHorizontalCoordinate, minimumHorizontalCoordinate);
        }
        else
        {
            _horizontalAxis.Range.Set(minimumHorizontalCoordinate, maximumHorizontalCoordinate);
        }
    }
}
