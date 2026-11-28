// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Markers;

/// <summary>Renders a right-facing triangle marker with a horizontal line.</summary>
internal sealed class TriangleWithLineRight : IMarker
{
    /// <summary>The divisor used to calculate half dimensions.</summary>
    private const float HalfDimensionDivisor = 2F;

    /// <summary>The fraction of the marker size used for triangle height.</summary>
    private const float TriangleHeightFraction = 0.8F;

    /// <summary>The fraction of the marker size used for triangle width.</summary>
    private const float TriangleWidthFraction = 0.5F;

    /// <summary>Renders the marker at the specified pixel.</summary>
    /// <param name="canvas">The canvas used to draw the marker.</param>
    /// <param name="paint">The paint used for rendering.</param>
    /// <param name="center">The center point of the marker.</param>
    /// <param name="size">The marker size in pixels.</param>
    /// <param name="markerStyle">The style used to render the marker.</param>
    public void Render(SKCanvas canvas, Paint paint, Pixel center, float size, MarkerStyle markerStyle)
    {
        float halfWidth = size / HalfDimensionDivisor;

        // Horizontal line
        using SKPathBuilder lineBuilder = new();
        lineBuilder.MoveTo(center.X + halfWidth, center.Y);
        lineBuilder.LineTo(center.X - halfWidth, center.Y);
        using SKPath line = lineBuilder.Detach();
        Drawing.DrawPath(canvas, paint, line, markerStyle.LineStyle);

        // Triangle
        var triangleHeight = size * TriangleHeightFraction;
        var triangleWidth = size * TriangleWidthFraction;
        var centerOfTriangle = center.MovedRight(halfWidth - (triangleWidth / HalfDimensionDivisor));

        var left = centerOfTriangle.X - (triangleWidth / HalfDimensionDivisor);
        var right = centerOfTriangle.X + (triangleWidth / HalfDimensionDivisor);
        var top = centerOfTriangle.Y - (triangleHeight / HalfDimensionDivisor);
        var bottom = centerOfTriangle.Y + (triangleHeight / HalfDimensionDivisor);

        SKPoint[] pointsList =
        {
            new (right, top),
            new (left, centerOfTriangle.Y),
            new (right, bottom),
        };

        using SKPathBuilder builder = new();
        builder.AddPoly(pointsList);
        using SKPath path = builder.Detach();

        PixelRect rect = new(left, right, bottom, top);
        Drawing.FillPath(canvas, paint, path, markerStyle.FillStyle, rect);
        Drawing.DrawPath(canvas, paint, path, markerStyle.OutlineStyle);
    }
}
