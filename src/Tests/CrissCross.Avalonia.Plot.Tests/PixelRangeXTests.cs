// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot;

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies horizontal pixel-range behavior.</summary>
public sealed class PixelRangeXTests
{
    /// <summary>Checks endpoint normalization, span, containment, and formatting.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Construction_NormalizesEndpointsAndContainsInclusivePositions()
    {
        const float left = 1;
        const float right = 4;
        const float span = 3;
        const float outside = 0;
        PixelRangeX range = new(right, left);

        await Assert.That(range.Left).IsEqualTo(left);
        await Assert.That(range.Right).IsEqualTo(right);
        await Assert.That(range.Span).IsEqualTo(span);
        await Assert.That(range.Contains(left)).IsTrue();
        await Assert.That(range.Contains(right)).IsTrue();
        await Assert.That(range.Contains(outside)).IsFalse();
        await Assert.That(range.ToString()).IsEqualTo("Left=1, Right=4, Span=3");
    }

    /// <summary>Checks that expansion updates either bound and equality remains value based.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Expand_UpdatesBoundsAndPreservesValueEquality()
    {
        const float initialLeft = 2;
        const float initialRight = 4;
        const float expandedLeft = 1;
        const float expandedRight = 5;
        const float interior = 3;
        const float differentLeft = 0;
        PixelRangeX range = new(initialLeft, initialRight);
        range.Expand(expandedLeft);
        range.Expand(expandedRight);
        range.Expand(interior);
        PixelRangeX equal = new(expandedLeft, expandedRight);

        await Assert.That(range).IsEqualTo(equal);
        await Assert.That(range.Equals((object)equal)).IsTrue();
        await Assert.That(range == equal).IsTrue();
        await Assert.That(range != new PixelRangeX(differentLeft, expandedRight)).IsTrue();
        await Assert.That(range.GetHashCode()).IsEqualTo(equal.GetHashCode());
        await Assert.That(range.Equals((object?)null)).IsFalse();
    }
}
