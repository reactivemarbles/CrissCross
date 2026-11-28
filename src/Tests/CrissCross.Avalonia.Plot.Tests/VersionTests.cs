// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies Plot version formatting and compatibility checks.</summary>
public sealed class VersionTests
{
    /// <summary>The amount added to a version component for mismatch checks.</summary>
    private const int VersionIncrement = 1;

    /// <summary>Checks that the long version identifies CrissCross Plot.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task LongString_UsesCrissCrossPlotBrand() =>
        await Assert.That(CrissCross.Plot.Version.LongString).IsEqualTo($"CrissCross Plot {CrissCross.Plot.Version.VersionString}");

    /// <summary>Checks that matching major, minor, and build versions are accepted.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ShouldBe_MatchingVersion_DoesNotThrow()
    {
        CrissCross.Plot.Version.ShouldBe(CrissCross.Plot.Version.Major);
        CrissCross.Plot.Version.ShouldBe(CrissCross.Plot.Version.Major, CrissCross.Plot.Version.Minor);
        CrissCross.Plot.Version.ShouldBe(CrissCross.Plot.Version.Major, CrissCross.Plot.Version.Minor, CrissCross.Plot.Version.Build);

        await Assert.That(CrissCross.Plot.Version.LongString).IsNotEmpty();
    }

    /// <summary>Checks that mismatched major, minor, and build versions are rejected.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ShouldBe_MismatchedVersion_Throws()
    {
        await Assert.That(
            static () => CrissCross.Plot.Version.ShouldBe(CrissCross.Plot.Version.Major + VersionIncrement))
            .Throws<InvalidOperationException>();
        await Assert.That(
            static () => CrissCross.Plot.Version.ShouldBe(CrissCross.Plot.Version.Major, CrissCross.Plot.Version.Minor + VersionIncrement))
            .Throws<InvalidOperationException>();
        await Assert.That(
            static () => CrissCross.Plot.Version.ShouldBe(
                CrissCross.Plot.Version.Major,
                CrissCross.Plot.Version.Minor,
                CrissCross.Plot.Version.Build + VersionIncrement))
            .Throws<InvalidOperationException>();
    }
}
