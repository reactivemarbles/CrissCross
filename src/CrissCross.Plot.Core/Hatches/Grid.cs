// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using SkiaSharp;

namespace CrissCross.Plot.Hatches;

/// <summary>Creates a grid hatch pattern.</summary>
/// <param name="rotate">Whether to rotate the grid pattern by 45 degrees.</param>
public class Grid(bool rotate) : IHatch
{
    /// <summary>The width and height of the grid bitmap in pixels.</summary>
    private const int TileSize = 20;

    /// <summary>The grid shader scale.</summary>
    private const float ShaderScale = 0.5F;

    /// <summary>The grid pattern rotation angle in degrees.</summary>
    private const float RotationDegrees = 45;

    /// <summary>The grid line width in pixels.</summary>
    private const int GridLineWidth = 3;

    /// <summary>The reusable grid bitmap.</summary>
    private static readonly SKBitmap _bmp = CreateBitmap();

    /// <summary>Initializes a new instance of the <see cref="Grid"/> class without rotation.</summary>
    public Grid()
        : this(false)
    {
    }

    /// <summary>Gets or sets whether to rotate the grid pattern by 45 degrees.</summary>
    public bool Rotate { get; set; } = rotate;

    /// <summary>Creates the shader for this hatch pattern.</summary>
    /// <param name="backgroundColor">The background color.</param>
    /// <param name="hatchColor">The hatch color.</param>
    /// <param name="rect">The plot rectangle.</param>
    /// <returns>The tiled grid shader.</returns>
    public SKShader GetShader(Color backgroundColor, Color hatchColor, PixelRect rect)
    {
        var rotationMatrix = Rotate ? SKMatrix.CreateRotationDegrees(RotationDegrees) : SKMatrix.Identity;

        return SKShader.CreateBitmap(
            _bmp,
            SKShaderTileMode.Repeat,
            SKShaderTileMode.Repeat,
            SKMatrix.CreateScale(ShaderScale, ShaderScale)
                .PostConcat(rotationMatrix))
                .WithColorFilter(Drawing.GetMaskColorFilter(hatchColor, backgroundColor));
    }

    /// <summary>Creates the bitmap containing the grid pattern.</summary>
    /// <returns>The grid bitmap.</returns>
    private static SKBitmap CreateBitmap()
    {
        var bmp = new SKBitmap(TileSize, TileSize);
        using var paint = new SKPaint() { Color = Colors.White.ToSKColor(), IsStroke = true, StrokeWidth = GridLineWidth };
        using var canvas = new SKCanvas(bmp);

        canvas.Clear(Colors.Black.ToSKColor());
        canvas.DrawRect(0, 0, TileSize, TileSize, paint);

        return bmp;
    }
}
