// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI;

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Tests finite tolerance and special floating-point comparison behavior.</summary>
public class DoubleComparisonTests
{
    /// <summary>Verifies infinities only compare close to the same signed infinity.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <param name="expected">The expected comparison result.</param>
    /// <returns>A task representing the asynchronous assertion.</returns>
    [Test]
    [Arguments(double.PositiveInfinity, double.PositiveInfinity, true)]
    [Arguments(double.NegativeInfinity, double.NegativeInfinity, true)]
    [Arguments(double.PositiveInfinity, double.NegativeInfinity, false)]
    [Arguments(double.PositiveInfinity, 1D, false)]
    [Arguments(1D, double.NegativeInfinity, false)]
    [Arguments(double.NaN, double.NaN, true)]
    [Arguments(double.NaN, 1D, false)]
    [Arguments(1D, double.NaN, false)]
    [Arguments(0D, -0D, true)]
    [Arguments(0D, 5E-11, true)]
    [Arguments(0D, 1E-5, false)]
    [Arguments(1E10, 1E10 + 0.5D, true)]
    [Arguments(1E10, 1E10 + 2D, false)]
    public async Task AreClose_RespectsToleranceAndSpecialValues(double left, double right, bool expected) =>
        await Assert.That(DoubleComparison.AreClose(left, right)).IsEqualTo(expected);
}
