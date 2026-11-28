// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot;
using CrissCross.Plot.NamedColors;

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies that built-in named-color tables expose all their colors.</summary>
public sealed class NamedColorsBaseTests
{
    /// <summary>Checks that built-in color tables enumerate each declared color property.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAllColors_ReturnsEveryBuiltInNamedColor()
    {
        const int ExpectedXkcdColorCount = 949;
        const int ExpectedWebColorCount = 148;
        Color[] xkcdColors = new XkcdColors().GetAllColors();
        Color[] webColors = new WebColors().GetAllColors();

        await Assert.That(xkcdColors.Length).IsEqualTo(ExpectedXkcdColorCount);
        await Assert.That(webColors.Length).IsEqualTo(ExpectedWebColorCount);
        await Assert.That(Array.Exists(xkcdColors, static color => color.Equals(XkcdColors.AcidGreen))).IsTrue();
        await Assert.That(Array.Exists(webColors, static color => color.Equals(WebColors.AliceBlue))).IsTrue();
        await Assert.That(Array.TrueForAll(xkcdColors, static color => color.Alpha == byte.MaxValue)).IsTrue();
        await Assert.That(Array.TrueForAll(webColors, static color => color.Alpha == byte.MaxValue)).IsTrue();
    }
}
