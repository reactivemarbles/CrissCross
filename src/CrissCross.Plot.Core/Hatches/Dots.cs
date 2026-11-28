// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using SkiaSharp;

namespace CrissCross.Plot.Hatches;

/// <summary>Creates a dotted hatch pattern.</summary>
public class Dots : IHatch
{
    /// <summary>The dotted bitmap width and height in pixels.</summary>
    private const int TileSize = 20;

    /// <summary>The center of the dot in the tile.</summary>
    private const int DotCenter = 5;

    /// <summary>The dot radius in pixels.</summary>
    private const int DotRadius = 5;

    /// <summary>The scale applied to the dotted shader.</summary>
    private const float ShaderScale = 0.5F;

    /// <summary>The reusable dotted bitmap.</summary>
    private static readonly SKBitmap _bmp = CreateBitmap();

    /// <summary>Creates the shader for this hatch pattern.</summary>
    /// <param name="backgroundColor">The background color.</param>
    /// <param name="hatchColor">The hatch color.</param>
    /// <param name="rect">The plot rectangle.</param>
    /// <returns>The tiled dotted shader.</returns>
    public SKShader GetShader(Color backgroundColor, Color hatchColor, PixelRect rect) => SKShader.CreateBitmap(
            _bmp,
            SKShaderTileMode.Repeat,
            SKShaderTileMode.Repeat,
            SKMatrix.CreateScale(ShaderScale, ShaderScale))
                .WithColorFilter(Drawing.GetMaskColorFilter(hatchColor, backgroundColor));

    /// <summary>Creates the bitmap containing the dotted pattern.</summary>
    /// <returns>The dotted bitmap.</returns>
    private static SKBitmap CreateBitmap()
    {
        var bmp = new SKBitmap(TileSize, TileSize);
        using var paint = new SKPaint() { Color = Colors.White.ToSKColor(), IsAntialias = true };
        using var canvas = new SKCanvas(bmp);

        canvas.Clear(Colors.Black.ToSKColor());
        canvas.DrawCircle(DotCenter, DotCenter, DotRadius, paint);

        return bmp;
    }
}
