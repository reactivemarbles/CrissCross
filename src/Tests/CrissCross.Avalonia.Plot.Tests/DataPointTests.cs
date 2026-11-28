// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot;

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies data-point value and sentinel behavior.</summary>
public sealed class DataPointTests
{
    /// <summary>Checks that coordinate construction preserves values and value equality.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Construction_FromCoordinatesPreservesDataAndEquality()
    {
        const double x = 1;
        const double y = 2;
        const int index = 3;
        const int differentIndex = 4;
        DataPoint point = new(new Coordinates(x, y), index);
        DataPoint same = new(x, y, index);

        await Assert.That(point.X).IsEqualTo(x);
        await Assert.That(point.Y).IsEqualTo(y);
        await Assert.That(point.Index).IsEqualTo(index);
        await Assert.That(point.Coordinates.X).IsEqualTo(x);
        await Assert.That(point.Coordinates.Y).IsEqualTo(y);
        await Assert.That(point.IsReal).IsTrue();
        await Assert.That(point.Equals(same)).IsTrue();
        await Assert.That(point.Equals((object)same)).IsTrue();
        await Assert.That(point == same).IsTrue();
        await Assert.That(point != new DataPoint(x, y, differentIndex)).IsTrue();
        await Assert.That(point.GetHashCode()).IsEqualTo(same.GetHashCode());
        await Assert.That(point.Equals((object?)null)).IsFalse();
    }

    /// <summary>Checks that the sentinel point uses NaN coordinates and is not real data.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task None_UsesNonRealSentinel()
    {
        DataPoint point = DataPoint.None;

        await Assert.That(double.IsNaN(point.X)).IsTrue();
        await Assert.That(double.IsNaN(point.Y)).IsTrue();
        await Assert.That(point.Index).IsEqualTo(-1);
        await Assert.That(point.IsReal).IsFalse();
    }
}
