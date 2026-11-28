// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot;
using SkiaSharp;

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies raster geometry for markers built from paths.</summary>
public sealed class MarkerRenderingTests
{
    /// <summary>Checks that the hashtag has four separate strokes and an empty center.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task HashTag_RendersStrokesWithoutFillingCenter()
    {
        const int dimension = 64;
        const int center = 32;
        const int offset = 4;
        const int size = 24;
        using SKBitmap bitmap = new(dimension, dimension);
        using SKCanvas canvas = new(bitmap);
        using Paint paint = Paint.NewDisposablePaint();
        canvas.Clear(SKColors.Transparent);
        MarkerStyle style = new(MarkerShape.HashTag, size, Colors.Black);

        style.Render(canvas, new(center, center), paint);

        await Assert.That(bitmap.GetPixel(center, center).Alpha).IsEqualTo((byte)0);
        await Assert.That(bitmap.GetPixel(center + offset, center).Alpha).IsGreaterThan((byte)0);
        await Assert.That(bitmap.GetPixel(center - offset, center).Alpha).IsGreaterThan((byte)0);
        await Assert.That(bitmap.GetPixel(center, center + offset).Alpha).IsGreaterThan((byte)0);
        await Assert.That(bitmap.GetPixel(center, center - offset).Alpha).IsGreaterThan((byte)0);
    }

    /// <summary>Checks the opposite vertical spokes of the two triangular line markers.</summary>
    /// <param name="shape">The marker shape to render.</param>
    /// <param name="direction">The vertical direction of its central spoke.</param>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [Arguments(MarkerShape.TriUp, 1)]
    [Arguments(MarkerShape.TriDown, -1)]
    public async Task TriMarker_RendersCentralSpokeInExpectedDirection(MarkerShape shape, int direction)
    {
        const int dimension = 64;
        const int center = 32;
        const int offset = 8;
        const int size = 24;
        using SKBitmap bitmap = new(dimension, dimension);
        using SKCanvas canvas = new(bitmap);
        using Paint paint = Paint.NewDisposablePaint();
        canvas.Clear(SKColors.Transparent);
        MarkerStyle style = new(shape, size, Colors.Black);

        style.Render(canvas, new(center, center), paint);

        await Assert.That(bitmap.GetPixel(center, center + (direction * offset)).Alpha).IsGreaterThan((byte)0);
        await Assert.That(bitmap.GetPixel(center, center - (direction * offset)).Alpha).IsEqualTo((byte)0);
    }
}
