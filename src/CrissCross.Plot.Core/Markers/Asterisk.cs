// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Markers;

/// <summary>Renders a marker with horizontal, vertical, and diagonal strokes.</summary>
internal sealed class Asterisk : IMarker
{
    /// <summary>The divisor used to calculate half of a marker size.</summary>
    private const float HalfSizeDivisor = 2;

    /// <summary>The divisor used to calculate the diagonal stroke offset.</summary>
    private const double DiagonalOffsetDivisor = 2.828;

    /// <inheritdoc/>
    public void Render(SKCanvas canvas, Paint paint, Pixel center, float size, MarkerStyle markerStyle)
    {
        float crossOffset = size / HalfSizeDivisor;
        float eksOffset = (float)(size / DiagonalOffsetDivisor);

        using SKPathBuilder builder = new();

        // Same as cross marker
        builder.MoveTo(center.X + crossOffset, center.Y);
        builder.LineTo(center.X - crossOffset, center.Y);
        builder.MoveTo(center.X, center.Y + crossOffset);
        builder.LineTo(center.X, center.Y - crossOffset);

        // Similar to eks (X) marker, but with length = size / 2
        builder.MoveTo(center.X + eksOffset, center.Y + eksOffset);
        builder.LineTo(center.X - eksOffset, center.Y - eksOffset);
        builder.MoveTo(center.X - eksOffset, center.Y + eksOffset);
        builder.LineTo(center.X + eksOffset, center.Y - eksOffset);

        using SKPath path = builder.Detach();
        Drawing.DrawPath(canvas, paint, path, markerStyle.LineStyle);
    }
}
