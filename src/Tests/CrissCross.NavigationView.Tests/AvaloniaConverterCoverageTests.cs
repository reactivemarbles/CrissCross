// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia.Media;
using CrissCross.Avalonia.UI.Appearance;
using CrissCross.Avalonia.UI.Converters;
using CrissCross.Avalonia.UI.Extensions;

namespace CrissCross.NavigationView.Tests;

/// <summary>Exercises converter success and fallback behavior without a visual host.</summary>
public sealed class AvaloniaConverterCoverageTests
{
    /// <summary>The fallback value supplied to converters.</summary>
    private const string InvalidValue = "invalid";

    /// <summary>The expected masked password length.</summary>
    private const int PasswordLength = 6;

    /// <summary>The numeric value supplied to the minimum converter.</summary>
    private const double InputValue = 4D;

    /// <summary>The numeric minimum supplied directly to the converter.</summary>
    private const double MinimumValue = 2D;

    /// <summary>The numeric minimum supplied as text to the converter.</summary>
    private const double ParsedMinimum = 3D;

    /// <summary>The partial opacity used to test brush conversion.</summary>
    private const double HalfOpacity = 0.5D;

    /// <summary>The positive color adjustment percentage.</summary>
    private const float PositiveColorAdjustment = 10F;

    /// <summary>The negative color adjustment percentage.</summary>
    private const float NegativeColorAdjustment = -10F;

    /// <summary>Verifies basic boolean, brush, color, text, and numeric conversion paths.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Converters_WhenGivenValidAndFallbackValues_ReturnExpectedResults()
    {
        var culture = CultureInfo.InvariantCulture;
        var inverted = new BoolToInvertedBoolConverter();
        var brushToColor = new BrushToColorConverter();
        var colorToBrush = new ColorToBrushConverter();
        var asterisk = new TextToAsteriskConverter();
        var enumToBool = new EnumToBoolConverter();
        var hex = new ColorToHexConverter();
        var minimum = new MinConverter();
        var brush = new SolidColorBrush(Colors.CornflowerBlue);

        await Assert.That(RequireBoolean(inverted.Convert(true, typeof(bool), null, culture))).IsFalse();
        await Assert.That(RequireBoolean(inverted.Convert(InvalidValue, typeof(bool), null, culture))).IsFalse();
        await Assert.That(brushToColor.Convert(brush, typeof(Color), null, culture)).IsEqualTo(Colors.CornflowerBlue);
        await Assert.That(brushToColor.ConvertBack(Colors.CornflowerBlue, typeof(IBrush), null, culture)).IsTypeOf<SolidColorBrush>();
        await Assert.That(colorToBrush.Convert(Colors.CornflowerBlue, typeof(IBrush), null, culture)).IsTypeOf<SolidColorBrush>();
        await Assert.That(colorToBrush.ConvertBack(brush, typeof(Color), null, culture)).IsEqualTo(Colors.CornflowerBlue);
        await Assert.That((asterisk.Convert("secret", typeof(string), null, culture) as string)?.Length).IsEqualTo(PasswordLength);
        await Assert.That(asterisk.Convert(null, typeof(string), null, culture)).IsEqualTo(string.Empty);
        await Assert.That(minimum.Convert(InputValue, typeof(double), MinimumValue, culture)).IsEqualTo(MinimumValue);
        await Assert.That(minimum.Convert(InputValue, typeof(double), "3", culture)).IsEqualTo(ParsedMinimum);
        await Assert.That(minimum.Convert("fallback", typeof(string), null, culture)).IsEqualTo("fallback");
        await Assert.That(RequireBoolean(enumToBool.Convert(ApplicationTheme.Dark, typeof(bool), ApplicationTheme.Dark, culture))).IsTrue();
        await Assert.That(RequireBoolean(enumToBool.Convert(null, typeof(bool), ApplicationTheme.Dark, culture))).IsFalse();
        await Assert.That(hex.Convert(Colors.CornflowerBlue, typeof(string), null, culture)).IsEqualTo(Colors.CornflowerBlue.ToString());
        await Assert.That(hex.ConvertBack("#FF6495ED", typeof(Color), null, culture)).IsEqualTo(Colors.CornflowerBlue);
        await Assert.That(hex.ConvertBack(InvalidValue, typeof(Color), null, culture)).IsEqualTo(Colors.Transparent);

        var themeChanged = new ThemeChangedEventArgs(ApplicationTheme.Dark, Colors.CornflowerBlue);
        await Assert.That(themeChanged.CurrentTheme).IsEqualTo(ApplicationTheme.Dark);
        await Assert.That(themeChanged.AccentColor).IsEqualTo(Colors.CornflowerBlue);

        var color = Colors.CornflowerBlue;
        await Assert.That(color.ToBrush().Color).IsEqualTo(color);
        await Assert.That(color.ToBrush(HalfOpacity).Opacity).IsEqualTo(HalfOpacity);
        await Assert.That(color.ToHexWithoutAlpha()).IsEqualTo("#6495ED");
        await Assert.That(ColorExtensions.FromHex("#FF6495ED")).IsEqualTo(Colors.CornflowerBlue);
        await Assert.That(ColorExtensions.FromHex(InvalidValue)).IsNull();
        await Assert.That(color.UpdateBrightness(PositiveColorAdjustment)).IsNotEqualTo(color);
        await Assert.That(color.UpdateLuminance(NegativeColorAdjustment)).IsNotEqualTo(color);
        await Assert.That(color.UpdateSaturation(PositiveColorAdjustment)).IsNotEqualTo(color);
    }

    /// <summary>Returns a converter result as a required boolean value.</summary>
    /// <param name="value">The converter result.</param>
    /// <returns>The boolean result.</returns>
    private static bool RequireBoolean(object? value) => value as bool? ?? throw new InvalidOperationException();
}
