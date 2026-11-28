// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Markers;

/// <summary>Renders a filled diamond-shaped marker.</summary>
internal sealed class FilledDiamond : IMarker
{
    /// <summary>The divisor used to calculate half of a marker size.</summary>
    private const float HalfSizeDivisor = 2;

    /// <inheritdoc/>
    public void Render(SKCanvas canvas, Paint paint, Pixel center, float size, MarkerStyle markerStyle)
    {
        float radius = size / HalfSizeDivisor;

        PixelRect rect = new(center, radius);

        SKPoint[] pointsList =
        {
            new(center.X + radius, center.Y),
            new(center.X, center.Y + radius),
            new(center.X - radius, center.Y),
            new(center.X, center.Y - radius),
        };

        using SKPathBuilder builder = new();
        builder.AddPoly(pointsList);
        using SKPath path = builder.Detach();

        Drawing.FillPath(canvas, paint, path, markerStyle.FillStyle, rect);
        Drawing.DrawPath(canvas, paint, path, markerStyle.OutlineStyle);
    }
}
