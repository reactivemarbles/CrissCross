// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Markers;

/// <summary>Renders a hash-shaped marker.</summary>
internal sealed class HashTag : IMarker
{
    /// <summary>The divisor used to calculate one sixth of a marker size.</summary>
    private const float SixthSizeDivisor = 6;

    /// <summary>The divisor used to calculate half of a marker size.</summary>
    private const float HalfSizeDivisor = 2;

    /// <inheritdoc/>
    public void Render(SKCanvas canvas, Paint paint, Pixel center, float size, MarkerStyle markerStyle)
    {
        float sixthOffset = size / SixthSizeDivisor;
        float halfOffset = size / HalfSizeDivisor;

        using SKPathBuilder builder = new();

        // Vertical Lines
        builder.MoveTo(center.X + sixthOffset, center.Y + halfOffset);
        builder.LineTo(center.X + sixthOffset, center.Y - halfOffset);
        builder.MoveTo(center.X - sixthOffset, center.Y + halfOffset);
        builder.LineTo(center.X - sixthOffset, center.Y - halfOffset);

        // Horizontal Lines
        builder.MoveTo(center.X + halfOffset, center.Y + sixthOffset);
        builder.LineTo(center.X - halfOffset, center.Y + sixthOffset);
        builder.MoveTo(center.X + halfOffset, center.Y - sixthOffset);
        builder.LineTo(center.X - halfOffset, center.Y - sixthOffset);

        using SKPath path = builder.Detach();
        Drawing.DrawPath(canvas, paint, path, markerStyle.LineStyle);
    }
}
