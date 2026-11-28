// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Markers;

/// <summary>Renders a filled square marker.</summary>
internal sealed class FilledSquare : IMarker
{
    /// <summary>The divisor used to calculate half of a marker size.</summary>
    private const float HalfSizeDivisor = 2;

    /// <inheritdoc/>
    public void Render(SKCanvas canvas, Paint paint, Pixel center, float size, MarkerStyle markerStyle)
    {
        PixelRect rect = new(center, size / HalfSizeDivisor);

        Drawing.FillRectangle(canvas, rect, paint, markerStyle.FillStyle);
        Drawing.DrawRectangle(canvas, rect, paint, markerStyle.OutlineStyle);
    }
}
