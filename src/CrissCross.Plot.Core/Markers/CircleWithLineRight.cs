// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Markers;

/// <summary>Renders a circle with a horizontal line extending from its center.</summary>
internal sealed class CircleWithLineRight : IMarker
{
    /// <summary>The divisor used to calculate half of a marker size.</summary>
    private const float HalfSizeDivisor = 2;

    /// <summary>The divisor used to calculate one quarter of a marker size.</summary>
    private const float QuarterSizeDivisor = 4;

    /// <inheritdoc/>
    public void Render(SKCanvas canvas, Paint paint, Pixel center, float size, MarkerStyle markerStyle)
    {
        float radius = size / HalfSizeDivisor;

        // Horizontal line
        using SKPathBuilder builder = new();
        builder.MoveTo(center.X + radius, center.Y);
        builder.LineTo(center.X - radius, center.Y);
        using SKPath path = builder.Detach();
        Drawing.DrawPath(canvas, paint, path, markerStyle.LineStyle);

        // Circle
        var circleCenter = center.MovedRight(radius);
        var circleRadius = size / QuarterSizeDivisor;
        Drawing.FillCircle(canvas, circleCenter, circleRadius, markerStyle.FillStyle, paint);
        Drawing.DrawCircle(canvas, circleCenter, circleRadius, markerStyle.OutlineStyle, paint);
    }
}
