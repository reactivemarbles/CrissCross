// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System;
using SkiaSharp;

namespace CrissCross.Plot.Hatches;

/// <summary>Creates a striped hatch pattern.</summary>
/// <param name="stripeDirection">The orientation of the stripes.</param>
public class Striped(StripeDirection stripeDirection) : IHatch
{
    /// <summary>The width of the stripe bitmap in pixels.</summary>
    private const int BitmapWidth = 20;

    /// <summary>The height of the stripe bitmap in pixels.</summary>
    private const int BitmapHeight = 50;

    /// <summary>The height of each stripe in the bitmap.</summary>
    private const int StripeHeight = BitmapWidth;

    /// <summary>The horizontal shader scale.</summary>
    private const float HorizontalScale = 0.1F;

    /// <summary>The vertical shader scale.</summary>
    private const float VerticalScale = 0.15F;

    /// <summary>The angle used for rising diagonal stripes.</summary>
    private const float DiagonalUpAngle = -45;

    /// <summary>The angle used for falling diagonal stripes.</summary>
    private const float DiagonalDownAngle = 45;

    /// <summary>The angle used for vertical stripes.</summary>
    private const float VerticalAngle = 90;

    /// <summary>The reusable stripe bitmap.</summary>
    private static readonly SKBitmap _bmp = CreateBitmap();

    /// <summary>Initializes a new instance of the <see cref="Striped"/> class with diagonal-up stripes.</summary>
    public Striped()
        : this(StripeDirection.DiagonalUp)
    {
    }

    /// <summary>Gets or sets the stripe orientation without rebuilding the bitmap.</summary>
    public StripeDirection StripeDirection { get; set; } = stripeDirection;

    /// <summary>Creates the shader for this hatch pattern.</summary>
    /// <param name="backgroundColor">The background color.</param>
    /// <param name="hatchColor">The hatch color.</param>
    /// <param name="rect">The plot rectangle.</param>
    /// <returns>The tiled stripe shader.</returns>
    public SKShader GetShader(Color backgroundColor, Color hatchColor, PixelRect rect)
    {
        var rotationMatrix = StripeDirection switch
        {
            StripeDirection.DiagonalUp => SKMatrix.CreateRotationDegrees(DiagonalUpAngle),
            StripeDirection.DiagonalDown => SKMatrix.CreateRotationDegrees(DiagonalDownAngle),
            StripeDirection.Horizontal => SKMatrix.Identity,
            StripeDirection.Vertical => SKMatrix.CreateRotationDegrees(VerticalAngle),
            _ => throw new ArgumentOutOfRangeException(nameof(StripeDirection), StripeDirection, "Unsupported stripe direction.")
        };

        return SKShader.CreateBitmap(
            _bmp,
            SKShaderTileMode.Repeat,
            SKShaderTileMode.Repeat,
            SKMatrix.CreateScale(HorizontalScale, VerticalScale)
                .PostConcat(rotationMatrix))
                .WithColorFilter(Drawing.GetMaskColorFilter(hatchColor, backgroundColor));
    }

    /// <summary>Creates the bitmap containing the stripe pattern.</summary>
    /// <returns>The stripe bitmap.</returns>
    private static SKBitmap CreateBitmap()
    {
        var bitmap = new SKBitmap(BitmapWidth, BitmapHeight);

        using var paint = new SKPaint() { Color = Colors.White.ToSKColor() };
        using var canvas = new SKCanvas(bitmap);

        canvas.Clear(Colors.Black.ToSKColor());
        canvas.DrawRect(new(0, 0, BitmapWidth, StripeHeight), paint);

        return bitmap;
    }
}
