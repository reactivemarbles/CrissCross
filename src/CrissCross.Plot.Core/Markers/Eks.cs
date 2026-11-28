// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Markers;

/// <summary>Renders an X-shaped marker.</summary>
internal sealed class Eks : IMarker
{
    /// <summary>The divisor used to calculate half of a marker size.</summary>
    private const float HalfSizeDivisor = 2;

    /// <inheritdoc/>
    public void Render(SKCanvas canvas, Paint paint, Pixel center, float size, MarkerStyle markerStyle)
    {
        float radius = size / HalfSizeDivisor;

        using SKPathBuilder builder = new();
        builder.MoveTo(center.X + radius, center.Y + radius);
        builder.LineTo(center.X - radius, center.Y - radius);
        builder.MoveTo(center.X - radius, center.Y + radius);
        builder.LineTo(center.X + radius, center.Y - radius);

        using SKPath path = builder.Detach();
        Drawing.DrawPath(canvas, paint, path, markerStyle.LineStyle);
    }
}
