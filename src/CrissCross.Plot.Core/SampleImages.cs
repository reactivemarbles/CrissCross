// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot;

/// <summary>Provides sample images for demonstrations and testing.</summary>
public static class SampleImages
{
    /// <summary>The default width and height of a logo image.</summary>
    private const int DefaultImageSize = 256;

    /// <summary>The default deterministic seed for generated noise.</summary>
    private const int DefaultNoiseSeed = 0;

    /// <summary>The font size used for sample text.</summary>
    private const int NoisyTextFontSize = 36;

    /// <summary>The lower background's left-edge height ratio.</summary>
    private const double LowerBackgroundEdgeYRatio = 0.8;

    /// <summary>The first lower-background vertex's horizontal ratio.</summary>
    private const double LowerBackgroundPeakXRatio = 0.3;

    /// <summary>The first lower-background vertex's vertical ratio.</summary>
    private const double LowerBackgroundPeakYRatio = 0.5;

    /// <summary>The second lower-background vertex's horizontal ratio.</summary>
    private const double LowerBackgroundNextXRatio = 0.5;

    /// <summary>The second lower-background vertex's vertical ratio.</summary>
    private const double LowerBackgroundNextYRatio = 0.65;

    /// <summary>The lower background's right-edge height ratio.</summary>
    private const double LowerBackgroundEndYRatio = 0.16;

    /// <summary>The first white zigzag vertex's vertical ratio.</summary>
    private const double ZigzagStartYRatio = 0.7;

    /// <summary>The second white zigzag vertex's vertical ratio.</summary>
    private const double ZigzagRisingYRatio = 0.4;

    /// <summary>The third white zigzag vertex's vertical ratio.</summary>
    private const double ZigzagPeakYRatio = 0.55;

    /// <summary>The fourth white zigzag vertex's vertical ratio.</summary>
    private const double ZigzagEndYRatio = 0.06;

    /// <summary>The fifth white zigzag vertex's vertical ratio.</summary>
    private const double ZigzagEndPointYRatio = 0.27;

    /// <summary>The sixth white zigzag vertex's vertical ratio.</summary>
    private const double ZigzagDescendingYRatio = 0.75;

    /// <summary>The seventh white zigzag vertex's vertical ratio.</summary>
    private const double ZigzagLowerYRatio = 0.6;

    /// <summary>The final white zigzag vertex's vertical ratio.</summary>
    private const double ZigzagBottomYRatio = 0.9;

    /// <summary>The alpha applied to the outline of the noisy text image.</summary>
    private const double HalfAlpha = 0.5;

    /// <summary>Creates a colorized sample image of the Mona Lisa.</summary>
    /// <returns>The generated sample image.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "PerformanceSharp",
        "PSH1020",
        Justification = "SampleData.MonaLisa exposes a rectangular array as public API; preserving that shape avoids a breaking API change and an extra conversion allocation.")]
    public static Image MonaLisa()
    {
        double[, ] data = SampleData.MonaLisa();
        int height = data.GetLength(0);
        int width = data.GetLength(1);
        Range range = Range.GetRange(data);

        Colormaps.Viridis colormap = new();

        uint[] argb = new uint[data.Length];
        for (int y = 0; y < height; y++)
        {
            int rowOffset = y * width;
            for (int x = 0; x < width; x++)
            {
                argb[rowOffset + x] = colormap.GetColor(data[y, x], range).ARGB;
            }
        }

        SKBitmap bmp = Drawing.BitmapFromArgbs(argb, width, height);
        return new(bmp);
    }

    /// <summary>Creates a logo image using the default dimensions.</summary>
    /// <returns>The generated logo image.</returns>
    public static Image ScottPlotLogo() => ScottPlotLogo(DefaultImageSize, DefaultImageSize);

    /// <summary>Creates a logo image with the specified width and default height.</summary>
    /// <param name="width">The image width in pixels.</param>
    /// <returns>The generated logo image.</returns>
    public static Image ScottPlotLogo(int width) => ScottPlotLogo(width, DefaultImageSize);

    /// <summary>Creates a logo image with the specified dimensions.</summary>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <returns>The generated logo image.</returns>
    public static Image ScottPlotLogo(int width, int height)
    {
        using SKSurface surface = Drawing.CreateSurface(width, height);
        using SKCanvas canvas = surface.Canvas;
        using Paint paint = Paint.NewDisposablePaint();
        PixelRect canvasRect = new(0, width, height, 0);

        FillStyle fillStyle = new();

        // Purple upper background.
        fillStyle.Color = Color.FromHex("#67217a");
        Drawing.FillRectangle(canvas, canvasRect, paint, fillStyle);

        // pink lower background
        Pixel[] pointsLowerBackground =
        [
            new(0, height),
            new(0, LowerBackgroundEdgeYRatio * height),
            new(LowerBackgroundPeakXRatio * width, LowerBackgroundPeakYRatio * height),
            new(LowerBackgroundNextXRatio * width, LowerBackgroundNextYRatio * height),
            new(width, LowerBackgroundEndYRatio * height),
            new(width, height),
        ];

        // Pink lower background.
        fillStyle.Color = Color.FromHex("#9a4993");
        FillPolygon(canvas, canvasRect, paint, fillStyle, pointsLowerBackground);

        // white ziggy
        Pixel[] pointsZiggy =
        [
            new(0, ZigzagStartYRatio * height),
            new(LowerBackgroundPeakXRatio * width, ZigzagRisingYRatio * height),
            new(LowerBackgroundNextXRatio * width, ZigzagPeakYRatio * height),
            new(width, ZigzagEndYRatio * height),
            new(width, ZigzagEndPointYRatio * height),
            new(LowerBackgroundNextXRatio * width, ZigzagDescendingYRatio * height),
            new(LowerBackgroundPeakXRatio * width, ZigzagLowerYRatio * height),
            new(0, ZigzagBottomYRatio * height),
        ];

        fillStyle.Color = Colors.White;
        FillPolygon(canvas, canvasRect, paint, fillStyle, pointsZiggy);

        return new(surface);
    }

    /// <summary>Creates a grayscale noise image with the default seed.</summary>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <returns>The generated noise image.</returns>
    public static Image NoiseGrayscale(int width, int height) => NoiseGrayscale(width, height, DefaultNoiseSeed);

    /// <summary>Creates a grayscale noise image using the specified seed.</summary>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <param name="seed">The seed used to generate repeatable noise.</param>
    /// <returns>The generated noise image.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "PerformanceSharp",
        "PSH1020",
        Justification = "The Image API consumes rectangular pixel arrays; retaining this representation avoids a full-array conversion allocation.")]
    public static Image NoiseGrayscale(int width, int height, int seed)
    {
        RandomDataGenerator gen = new(seed);

        byte[, ] noise = new byte[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noise[y, x] = gen.RandomByte();
            }
        }

        return new(noise);
    }

    /// <summary>Creates a text image over a grayscale noise background.</summary>
    /// <param name="text">The text to draw.</param>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <returns>The generated image.</returns>
    public static Image NoisyText(string text, int width, int height)
    {
        PixelRect rect = new(0, width, height, 0);
        using SKSurface surface = Drawing.CreateSurface((int)rect.Width, (int)rect.Height);
        using SKCanvas canvas = surface.Canvas;
        using Paint paint = Paint.NewDisposablePaint();

        Image noiseImage = NoiseGrayscale(width, height);
        noiseImage.Render(canvas, rect, paint, false);

        LineStyle ls = new(1, Colors.LightBlue.WithAlpha(HalfAlpha));
        Drawing.DrawRectangle(canvas, rect, paint, ls);
        Drawing.DrawDebugRectangle(canvas, paint, rect);

        LabelStyle label = new() { Alignment = Alignment.MiddleCenter, ForeColor = Colors.Navy, FontSize = NoisyTextFontSize, Bold = true, };

        label.Render(canvas, rect.Center, paint, text);

        return new(surface);
    }

    /// <summary>Fills a polygon clipped to the specified canvas rectangle.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="canvasRect">The rectangle used to clip the polygon.</param>
    /// <param name="paint">The paint to configure and use.</param>
    /// <param name="fillStyle">The fill style for the polygon.</param>
    /// <param name="pixels">The polygon vertices in order.</param>
    private static void FillPolygon(SKCanvas canvas, PixelRect canvasRect, Paint paint, FillStyle fillStyle, Pixel[] pixels)
    {
        using SKPathBuilder builder = new();
        builder.MoveTo(pixels[0].ToSKPoint());
        for (int i = 1; i < pixels.Length; i++)
        {
            builder.LineTo(pixels[i].ToSKPoint());
        }

        builder.Close();

        using SKPath path = builder.Detach();
        Drawing.FillPath(canvas, paint, path, fillStyle, canvasRect);
    }
}
