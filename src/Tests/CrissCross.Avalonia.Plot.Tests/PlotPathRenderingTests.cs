// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot;
using SkiaSharp;

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Exercises filled plot paths through real raster rendering.</summary>
public sealed class PlotPathRenderingTests
{
    /// <summary>Checks that a tooltip tail is rendered when its tip moves away from its body.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Tooltip_MovingTip_RendersTailWithoutChangingBody()
    {
        const int width = 400;
        const int height = 300;
        const double sampleX = 0.4;
        const double tipX = 0.8;
        using CrissCross.Plot.Plot plot = new();
        plot.Axes.SetLimits(-1, 1, -1, 1);
        var tooltip = plot.Add.Tooltip(new(0, 0), "Label", new(0, 0));
        tooltip.FillColor = Colors.Red;
        using SKBitmap withoutTail = SKBitmap.Decode(plot.GetImageBytes(width, height, ImageFormat.Png));
        Pixel sample = plot.GetPixel(new(sampleX, 0));

        tooltip.TipLocation = new(tipX, 0);
        using SKBitmap withTail = SKBitmap.Decode(plot.GetImageBytes(width, height, ImageFormat.Png));
        SKColor before = withoutTail.GetPixel((int)sample.X, (int)sample.Y);
        SKColor after = withTail.GetPixel((int)sample.X, (int)sample.Y);

        await Assert.That(before).IsNotEqualTo(SKColors.Red);
        await Assert.That(after).IsEqualTo(SKColors.Red);
    }

    /// <summary>Checks that independently constructed coxcomb slices retain their fill colors.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Coxcomb_TwoSlices_RendersBothColors()
    {
        const int width = 400;
        const int height = 300;
        const int colorThreshold = 200;
        const int lowThreshold = 40;
        using CrissCross.Plot.Plot plot = new();
        _ = plot.Add.Coxcomb(new List<PieSlice> { new(1, Colors.Red), new(1, Colors.Blue) });
        plot.Axes.AutoScale();
        using SKBitmap bitmap = SKBitmap.Decode(plot.GetImageBytes(width, height, ImageFormat.Png));
        int redPixels = 0;
        int bluePixels = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                SKColor pixel = bitmap.GetPixel(x, y);
                if (pixel.Red > colorThreshold && pixel.Green < lowThreshold && pixel.Blue < lowThreshold)
                {
                    redPixels++;
                }
                else if (pixel.Blue > colorThreshold && pixel.Green < lowThreshold && pixel.Red < lowThreshold)
                {
                    bluePixels++;
                }
            }
        }

        await Assert.That(redPixels).IsGreaterThan(0);
        await Assert.That(bluePixels).IsGreaterThan(0);
    }
}
