// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI;

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Tests the exact color equality and hashing contract.</summary>
public class ColorStateTests
{
    /// <summary>The sample channel value.</summary>
    private const double SampleChannel = 0.5D;

    /// <summary>The difference used to verify exact channel equality.</summary>
    private const double ChannelDifference = 1E-11;

    /// <summary>Verifies nearby channel values retain distinct value identities.</summary>
    /// <returns>A task representing the asynchronous assertion.</returns>
    [Test]
    public async Task Equals_NearbyChannels_RemainsExact()
    {
        ColorState left = new(new(SampleChannel, 0D, 0D), 1D, new(0D, 0D, 0D), new(0D, 0D, 0D));
        ColorState right = new(new(SampleChannel + ChannelDifference, 0D, 0D), 1D, new(0D, 0D, 0D), new(0D, 0D, 0D));

        await Assert.That(left.Equals(right)).IsFalse();
    }

    /// <summary>Verifies NaN channels and signed zero preserve double value equality.</summary>
    /// <returns>A task representing the asynchronous assertion.</returns>
    [Test]
    public async Task Equals_NaNAndSignedZero_PreservesHashContract()
    {
        ColorState left = new(new(double.NaN, 0D, 0D), 1D, new(0D, 0D, 0D), new(0D, 0D, 0D));
        ColorState right = new(new(double.NaN, -0D, 0D), 1D, new(0D, 0D, 0D), new(0D, 0D, 0D));

        await Assert.That(left.Equals(right)).IsTrue();
        await Assert.That(left.GetHashCode()).IsEqualTo(right.GetHashCode());
    }
}
