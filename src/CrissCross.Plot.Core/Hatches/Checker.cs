// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using SkiaSharp;

namespace CrissCross.Plot.Hatches;

/// <summary>Creates a checkerboard hatch pattern.</summary>
public class Checker : IHatch
{
    /// <summary>The checkerboard bitmap width and height in pixels.</summary>
    private const int TileSize = 20;

    /// <summary>The size of each checkerboard square in pixels.</summary>
    private const int SquareSize = TileSize / 2;

    /// <summary>The scale applied to the checkerboard shader.</summary>
    private const float ShaderScale = 0.5F;

    /// <summary>The reusable checkerboard bitmap.</summary>
    private static readonly SKBitmap _bmp = CreateBitmap();

    /// <summary>Creates the shader for this hatch pattern.</summary>
    /// <param name="backgroundColor">The background color.</param>
    /// <param name="hatchColor">The hatch color.</param>
    /// <param name="rect">The plot rectangle.</param>
    /// <returns>The tiled checkerboard shader.</returns>
    public SKShader GetShader(Color backgroundColor, Color hatchColor, PixelRect rect) => SKShader.CreateBitmap(
            _bmp,
            SKShaderTileMode.Repeat,
            SKShaderTileMode.Repeat,
            SKMatrix.CreateScale(ShaderScale, ShaderScale))
                .WithColorFilter(Drawing.GetMaskColorFilter(hatchColor, backgroundColor));

    /// <summary>Creates the bitmap containing the checkerboard pattern.</summary>
    /// <returns>The checkerboard bitmap.</returns>
    private static SKBitmap CreateBitmap()
    {
        var bmp = new SKBitmap(TileSize, TileSize);
        using var paint = new SKPaint() { Color = Colors.White.ToSKColor() };
        using var canvas = new SKCanvas(bmp);

        canvas.Clear(Colors.Black.ToSKColor());
        canvas.DrawRect(new(0, 0, SquareSize, SquareSize), paint);
        canvas.DrawRect(new(SquareSize, SquareSize, TileSize, TileSize), paint);

        return bmp;
    }
}
