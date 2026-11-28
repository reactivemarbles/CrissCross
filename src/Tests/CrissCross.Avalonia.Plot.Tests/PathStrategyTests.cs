// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot;
using CrissCross.Plot.PathStrategies;
using SkiaSharp;

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies the geometry returned by path strategies.</summary>
public sealed class PathStrategyTests
{
    /// <summary>Verifies straight paths retain separate segments around missing data.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Straight_MissingPoint_SeparatesSegments()
    {
        const int expectedPoints = 4;
        const int lastIndex = 3;
        Pixel first = new(0, 0);
        Pixel last = new(1, 1);
        using SKPath path = new Straight().GetPath([first, last, Pixel.NaN, first, last]);

        await Assert.That(path.PointCount).IsEqualTo(expectedPoints);
        await Assert.That(path.VerbCount).IsEqualTo(expectedPoints);
        await Assert.That(path.GetPoint(0)).IsEqualTo(first.ToSKPoint());
        await Assert.That(path.GetPoint(lastIndex)).IsEqualTo(last.ToSKPoint());
    }

    /// <summary>Verifies quadratic paths preserve control points and endpoints.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task QuadHalfPoint_TwoPoints_CreatesExpectedCurves()
    {
        const int expectedPoints = 5;
        const int expectedVerbs = 3;
        const int secondPointIndex = 2;
        const int thirdPointIndex = 3;
        const int lastPointIndex = 4;
        const float width = 4;
        const float height = 2;
        const float midpoint = 2;
        using SKPath path = new QuadHalfPoint().GetPath([new(0, 0), new(width, height)]);

        await Assert.That(path.PointCount).IsEqualTo(expectedPoints);
        await Assert.That(path.VerbCount).IsEqualTo(expectedVerbs);
        await Assert.That(path.GetPoint(1)).IsEqualTo(new(midpoint, 0));
        await Assert.That(path.GetPoint(secondPointIndex)).IsEqualTo(new(midpoint, 1));
        await Assert.That(path.GetPoint(thirdPointIndex)).IsEqualTo(new(midpoint, height));
        await Assert.That(path.GetPoint(lastPointIndex)).IsEqualTo(new(width, height));
    }

    /// <summary>Verifies missing points separate quadratic segments without contaminating their coordinates.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task QuadHalfPoint_MissingPoints_SeparatesSegments()
    {
        const int expectedPoints = 10;
        const int expectedVerbs = 6;
        const int secondSegmentStart = 5;
        const int lastPointIndex = 9;
        const int secondCoordinate = 2;
        const int thirdX = 4;
        const int fourthX = 6;
        const int fourthY = 3;
        Pixel first = new(0, 0);
        Pixel second = new(secondCoordinate, secondCoordinate);
        Pixel third = new(thirdX, 1);
        Pixel fourth = new(fourthX, fourthY);
        using SKPath path = new QuadHalfPoint().GetPath([Pixel.NaN, first, second, Pixel.NaN, third, fourth, Pixel.NaN]);

        await Assert.That(path.PointCount).IsEqualTo(expectedPoints);
        await Assert.That(path.VerbCount).IsEqualTo(expectedVerbs);
        await Assert.That(path.GetPoint(0)).IsEqualTo(first.ToSKPoint());
        await Assert.That(path.GetPoint(secondSegmentStart)).IsEqualTo(third.ToSKPoint());
        await Assert.That(path.GetPoint(lastPointIndex)).IsEqualTo(fourth.ToSKPoint());
        for (int i = 0; i < path.PointCount; i++)
        {
            await Assert.That(float.IsNaN(path.GetPoint(i).X) || float.IsNaN(path.GetPoint(i).Y)).IsFalse();
        }
    }
}
