// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot;
using SkiaSharp;

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Tests font selection and glyph detection for plot rendering.</summary>
public sealed class FontsTests
{
    /// <summary>Verifies that missing glyphs are counted for the selected typeface.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task CountMissingGlyphs_DefaultFont_CountsMissingCodePoints()
    {
        const int missingCodePoint = 0x10FFFF;
        const int expectedMissingCount = 2;
        string family = SKTypeface.Default.FamilyName;
        using SKTypeface typeface = SKTypeface.FromFamilyName(family);
        using SKFont font = new(typeface);
        await Assert.That(font.ContainsGlyph('A')).IsTrue();
        await Assert.That(font.ContainsGlyph(missingCodePoint)).IsFalse();

        int missingCount = Fonts.CountMissingGlyphs(family, ['A', missingCodePoint, missingCodePoint]);

        await Assert.That(missingCount).IsEqualTo(expectedMissingCount);
    }

    /// <summary>Verifies that a new paint uses a typeface that can measure ordinary text.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NewDisposablePaint_DefaultFont_MeasuresText()
    {
        const int fontSize = 20;
        using Paint paint = Paint.NewDisposablePaint();
        paint.TextSize = fontSize;

        PixelRect bounds = paint.MeasureText("Plot");

        await Assert.That(bounds.Width).IsGreaterThan(0);
        await Assert.That(bounds.Height).IsGreaterThan(0);
    }
}
