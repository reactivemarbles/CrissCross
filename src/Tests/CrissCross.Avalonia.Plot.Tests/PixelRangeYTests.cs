// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot;

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies vertical pixel-range behavior.</summary>
public sealed class PixelRangeYTests
{
    /// <summary>Checks endpoint normalization, span, containment, and formatting.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Construction_NormalizesEndpointsAndContainsInclusivePositions()
    {
        const float top = 1;
        const float bottom = 4;
        const float span = 3;
        const float outside = 0;
        PixelRangeY range = new(bottom, top);

        await Assert.That(range.Top).IsEqualTo(top);
        await Assert.That(range.Bottom).IsEqualTo(bottom);
        await Assert.That(range.Span).IsEqualTo(span);
        await Assert.That(range.Contains(top)).IsTrue();
        await Assert.That(range.Contains(bottom)).IsTrue();
        await Assert.That(range.Contains(outside)).IsFalse();
        await Assert.That(range.ToString()).IsEqualTo("Top=1, Bottom=4, Span=3");
    }

    /// <summary>Checks that expansion updates either bound and equality remains value based.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Expand_UpdatesBoundsAndPreservesValueEquality()
    {
        const float initialTop = 2;
        const float initialBottom = 4;
        const float expandedTop = 1;
        const float expandedBottom = 5;
        const float interior = 3;
        const float differentTop = 0;
        PixelRangeY range = new(initialTop, initialBottom);
        range.Expand(expandedTop);
        range.Expand(expandedBottom);
        range.Expand(interior);
        PixelRangeY equal = new(expandedTop, expandedBottom);

        await Assert.That(range).IsEqualTo(equal);
        await Assert.That(range.Equals((object)equal)).IsTrue();
        await Assert.That(range == equal).IsTrue();
        await Assert.That(range != new PixelRangeY(differentTop, expandedBottom)).IsTrue();
        await Assert.That(range.GetHashCode()).IsEqualTo(equal.GetHashCode());
        await Assert.That(range.Equals((object?)null)).IsFalse();
    }
}
