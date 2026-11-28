// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using SkiaSharp;

namespace CrissCross.Plot.Hatches;

/// <summary>Creates a square hatch pattern.</summary>
public class Square : IHatch
{
    /// <summary>The square bitmap width and height in pixels.</summary>
    private const int TileSize = 20;

    /// <summary>The size of the square in pixels.</summary>
    private const int SquareSize = TileSize / 2;

    /// <summary>The scale applied to the square shader.</summary>
    private const float ShaderScale = 0.5F;

    /// <summary>The reusable square bitmap.</summary>
    private static readonly SKBitmap _bmp = CreateBitmap();

    /// <summary>Creates the shader for this hatch pattern.</summary>
    /// <param name="backgroundColor">The background color.</param>
    /// <param name="hatchColor">The hatch color.</param>
    /// <param name="rect">The plot rectangle.</param>
    /// <returns>The tiled square shader.</returns>
    public SKShader GetShader(Color backgroundColor, Color hatchColor, PixelRect rect) => SKShader.CreateBitmap(
            _bmp,
            SKShaderTileMode.Repeat,
            SKShaderTileMode.Repeat,
            SKMatrix.CreateScale(ShaderScale, ShaderScale))
                .WithColorFilter(Drawing.GetMaskColorFilter(hatchColor, backgroundColor));

    /// <summary>Creates the bitmap containing the square pattern.</summary>
    /// <returns>The square bitmap.</returns>
    private static SKBitmap CreateBitmap()
    {
        var bitmap = new SKBitmap(TileSize, TileSize);
        using var paint = new SKPaint() { Color = Colors.White.ToSKColor() };
        using var canvas = new SKCanvas(bitmap);

        canvas.Clear(Colors.Black.ToSKColor());
        canvas.DrawRect(new(0, 0, SquareSize, SquareSize), paint);

        return bitmap;
    }
}
