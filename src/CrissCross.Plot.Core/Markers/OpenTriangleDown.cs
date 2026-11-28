// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Markers;

/// <summary>Renders an outlined downward-pointing triangle marker.</summary>
internal sealed class OpenTriangleDown : IMarker
{
    /// <summary>The divisor used to calculate the triangle's radius from its side length.</summary>
    private const double TriangleRadiusDivisor = 1.732D;

    /// <summary>The factor used to calculate the horizontal vertex offset.</summary>
    private const double HorizontalOffsetFactor = 0.866D;

    /// <summary>The divisor used to calculate half of the triangle's radius.</summary>
    private const float HalfSizeDivisor = 2F;

    /// <inheritdoc/>
    public void Render(SKCanvas canvas, Paint paint, Pixel center, float size, MarkerStyle markerStyle)
    {
        // Length of each side of triangle = size
        float radius = (float)(size / TriangleRadiusDivisor); // size / sqrt(3)
        float horizontalOffset = (float)(radius * HorizontalOffsetFactor); // r * sqrt(3)/2
        float verticalOffset = radius / HalfSizeDivisor;

        // Bottom, right, and left vertices
        SKPoint[] pointsList =
        {
            new (center.X, center.Y + radius),
            new (center.X + horizontalOffset, center.Y - verticalOffset),
            new (center.X - horizontalOffset, center.Y - verticalOffset),
        };

        using SKPathBuilder builder = new();
        builder.AddPoly(pointsList);
        using SKPath path = builder.Detach();

        Drawing.DrawPath(canvas, paint, path, markerStyle.LineStyle);
    }
}
