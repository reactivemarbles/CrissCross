// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot;
using SkiaSharp;

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies the rendered geometry of financial bars.</summary>
public sealed class OhlcPlotTests
{
    /// <summary>Checks rising and falling bars use their own paths and styles in sequential mode.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Render_SequentialBars_DrawsRisingAndFallingStrokes()
    {
        const int width = 400;
        const int height = 300;
        const double high = 4;
        const double close = 3;
        const double open = 2;
        using CrissCross.Plot.Plot plot = new();
        var bars = plot.Add.OHLC([new(open, high, 1, close), new(close, high, 1, open)]);
        bars.Sequential = true;
        bars.RisingStyle.Color = Colors.Red;
        bars.FallingStyle.Color = Colors.Blue;
        plot.Axes.SetLimits(-1, 2, 0, 5);

        using SKBitmap bitmap = SKBitmap.Decode(plot.GetImageBytes(width, height, ImageFormat.Png));
        Pixel rising = plot.GetPixel(new(0, high));
        Pixel falling = plot.GetPixel(new(1, high));
        SKColor risingPixel = bitmap.GetPixel((int)rising.X, (int)rising.Y + 1);
        SKColor fallingPixel = bitmap.GetPixel((int)falling.X, (int)falling.Y + 1);

        await Assert.That(risingPixel.Red).IsGreaterThan(risingPixel.Blue);
        await Assert.That(fallingPixel.Blue).IsGreaterThan(fallingPixel.Red);
    }
}
