// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot;
using SkiaSharp;

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies filled streaming plots on both sides of their baseline.</summary>
public sealed class DataStreamerRenderingTests
{
    /// <summary>Checks that scrolling fills are clipped above and below the baseline.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Render_ScrollWithBaseline_FillsBothSides()
    {
        const int width = 400;
        const int height = 300;
        const int capacity = 4;
        const double magnitude = 2;
        const double sampleX = 0.5;
        using CrissCross.Plot.Plot plot = new();
        var streamer = plot.Add.DataStreamer(capacity);
        streamer.ManageAxisLimits = false;
        streamer.ViewScrollLeft();
        streamer.FillY = true;
        streamer.FillYAboveColor = Colors.Red;
        streamer.FillYBelowColor = Colors.Blue;
        streamer.Add([magnitude, magnitude, -magnitude, -magnitude]);
        plot.Axes.SetLimits(-1, capacity, -3, 3);

        using SKBitmap bitmap = SKBitmap.Decode(plot.GetImageBytes(width, height, ImageFormat.Png));
        Pixel above = plot.GetPixel(new(sampleX, 1));
        Pixel below = plot.GetPixel(new(capacity - 1 - sampleX, -1));
        SKColor aboveColor = bitmap.GetPixel((int)above.X, (int)above.Y);
        SKColor belowColor = bitmap.GetPixel((int)below.X, (int)below.Y);

        await Assert.That(aboveColor).IsEqualTo(SKColors.Red);
        await Assert.That(belowColor).IsEqualTo(SKColors.Blue);
    }
}
