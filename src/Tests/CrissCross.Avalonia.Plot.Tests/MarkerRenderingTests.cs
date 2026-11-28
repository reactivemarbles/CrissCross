// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot;
using SkiaSharp;

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies raster geometry for markers built from paths.</summary>
public sealed class MarkerRenderingTests
{
    /// <summary>Verifies that a custom renderer still runs when the standard shape is None.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task DrawMarkers_NoneShapeWithCustomRenderer_RendersCustomShape()
    {
        const int dimension = 32;
        const int center = 16;
        const int markerSize = 10;
        Pixel[] pixels = [new(center, center)];
        using SKBitmap bitmap = new(dimension, dimension);
        using SKCanvas canvas = new(bitmap);
        using Paint paint = Paint.NewDisposablePaint();
        canvas.Clear(SKColors.Transparent);
        MarkerStyle style = new(MarkerShape.None, markerSize, Colors.Blue) { CustomRenderer = MarkerShape.FilledCircle.GetMarker() };

        Drawing.DrawMarkers(canvas, paint, pixels, style);

        await Assert.That(bitmap.GetPixel(center, center).Alpha).IsGreaterThan((byte)0);
    }

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

    /// <summary>Checks that all standard marker shapes render pixels except the no-marker value.</summary>
    /// <param name="shape">The standard marker shape to render.</param>
    /// <param name="expectedVisible">Whether the marker is expected to draw visible pixels.</param>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [Arguments(MarkerShape.None, false)]
    [Arguments(MarkerShape.FilledCircle, true)]
    [Arguments(MarkerShape.OpenCircle, true)]
    [Arguments(MarkerShape.FilledSquare, true)]
    [Arguments(MarkerShape.OpenSquare, true)]
    [Arguments(MarkerShape.FilledTriangleUp, true)]
    [Arguments(MarkerShape.OpenTriangleUp, true)]
    [Arguments(MarkerShape.FilledTriangleDown, true)]
    [Arguments(MarkerShape.OpenTriangleDown, true)]
    [Arguments(MarkerShape.FilledDiamond, true)]
    [Arguments(MarkerShape.OpenDiamond, true)]
    [Arguments(MarkerShape.Eks, true)]
    [Arguments(MarkerShape.Cross, true)]
    [Arguments(MarkerShape.VerticalBar, true)]
    [Arguments(MarkerShape.HorizontalBar, true)]
    [Arguments(MarkerShape.TriUp, true)]
    [Arguments(MarkerShape.TriDown, true)]
    [Arguments(MarkerShape.Asterisk, true)]
    [Arguments(MarkerShape.HashTag, true)]
    [Arguments(MarkerShape.OpenCircleWithDot, true)]
    [Arguments(MarkerShape.OpenCircleWithCross, true)]
    [Arguments(MarkerShape.OpenCircleWithEks, true)]
    [Arguments(MarkerShape.CircleWithLineLeft, true)]
    [Arguments(MarkerShape.CircleWithLineRight, true)]
    [Arguments(MarkerShape.TriangleWithLineLeft, true)]
    [Arguments(MarkerShape.TriangleWithLineRight, true)]
    public async Task Render_BuiltInShape_ProducesExpectedVisibility(MarkerShape shape, bool expectedVisible)
    {
        const int dimension = 64;
        const int center = 32;
        const int size = 24;
        using SKBitmap bitmap = new(dimension, dimension);
        using SKCanvas canvas = new(bitmap);
        using Paint paint = Paint.NewDisposablePaint();
        canvas.Clear(SKColors.Transparent);
        MarkerStyle style = new(shape, size, Colors.Black);

        style.Render(canvas, new(center, center), paint);

        bool hasVisiblePixel = false;
        for (int y = 0; y < dimension && !hasVisiblePixel; y++)
        {
            for (int x = 0; x < dimension; x++)
            {
                if (bitmap.GetPixel(x, y).Alpha != 0)
                {
                    hasVisiblePixel = true;
                    break;
                }
            }
        }

        await Assert.That(hasVisiblePixel).IsEqualTo(expectedVisible);
    }
}
