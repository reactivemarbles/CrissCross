// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;
using CrissCross.Plot.Colormaps;

namespace CrissCross.Plot;

/// <summary>Provides built-in colormap access and image generation.</summary>
public static class Colormap
{
    /// <summary>Gets or sets the default colormap.</summary>
    public static IColormap Default { get; set; } = new MellowRainbow();

    /// <summary>Gets every available colormap in the assembly.</summary>
    /// <returns>The available colormap instances.</returns>
    [RequiresUnreferencedCode("Discovers colormaps by reflecting over the assembly types.")]
    public static IColormap[] GetColormaps() => TypeDiscovery.GetImplementations<IColormap>(typeof(Colormap).Assembly);

    /// <summary>Creates an image showing the colors from a colormap.</summary>
    /// <param name="colormap">The colormap to render.</param>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <returns>The generated image.</returns>
    public static Image GetImage(IColormap colormap, int width, int height)
    {
        using SKBitmap bmp = new(width, height);
        using SKCanvas canvas = new(bmp);

        using Paint paint = Paint.NewDisposablePaint();
        paint.IsAntialias = false;
        paint.IsStroke = true;

        for (int i = 0; i < width; i++)
        {
            paint.Color = colormap.GetColor(i / (width - 1.0));
            PixelLine line = new(i, 0, i, height);
            Drawing.DrawLine(canvas, paint, line);
        }

        using MemoryStream ms = new();
        _ = bmp.Encode(ms, SKEncodedImageFormat.Jpeg, quality: 85);
        byte[] bytes = ms.ToArray();
        return new(bytes);
    }

    /// <summary>Creates a custom interpolated colormap from the specified colors.</summary>
    /// <param name="colors">The colors in the colormap.</param>
    /// <returns>The interpolated colormap.</returns>
    public static IColormap FromColors(Color[] colors) => FromColors(colors, true);

    /// <summary>Creates a custom colormap from the specified colors.</summary>
    /// <param name="colors">The colors in the colormap.</param>
    /// <param name="smooth">Whether to interpolate between adjacent colors.</param>
    /// <returns>The custom colormap.</returns>
    public static IColormap FromColors(Color[] colors, bool smooth) => smooth
        ? new CustomInterpolated(colors)
        : new CustomPalette(colors);
}
