// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Markers;

/// <summary>Renders an outlined circle with a filled center dot.</summary>
internal sealed class OpenCircleWithDot : IMarker
{
    /// <summary>The divisor used to calculate half of a marker size.</summary>
    private const float HalfSizeDivisor = 2;

    /// <inheritdoc/>
    public void Render(SKCanvas canvas, Paint paint, Pixel center, float size, MarkerStyle markerStyle)
    {
        float radius = size / HalfSizeDivisor;
        Drawing.DrawCircle(canvas, center, radius, markerStyle.LineStyle, paint);

        float dotRadius = markerStyle.LineStyle.Width > 0
            ? markerStyle.LineStyle.Width
            : 1;

        Drawing.FillCircle(canvas, center, dotRadius, markerStyle.FillStyle, paint);
    }
}
