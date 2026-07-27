// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Binding = System.Windows.Data.Binding;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using ReactiveBackButtonVisibility = CrissCross.Reactive.WPF.UI.Controls.NavigationViewBackButtonVisible;
using ReactiveConverters = CrissCross.Reactive.WPF.UI.Converters;
using ReactivePickerType = CrissCross.Reactive.WPF.UI.PickerType;
using StandardBackButtonVisibility = CrissCross.WPF.UI.Controls.NavigationViewBackButtonVisible;
using StandardConverters = CrissCross.WPF.UI.Converters;
using StandardPickerType = CrissCross.WPF.UI.PickerType;

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Verifies converter parity between the standard and reactive WPF UI packages.</summary>
public sealed class ConverterSurfaceTests
{
    /// <summary>The zero value used by numeric converter tests.</summary>
    private const double Zero = 0D;

    /// <summary>The one value used by numeric converter tests.</summary>
    private const double One = 1D;

    /// <summary>The two value used by numeric converter tests.</summary>
    private const double Two = 2D;

    /// <summary>The three value used by numeric converter tests.</summary>
    private const double Three = 3D;

    /// <summary>The four value used by numeric converter tests.</summary>
    private const double Four = 4D;

    /// <summary>The half value used by numeric converter tests.</summary>
    private const double Half = 0.5D;

    /// <summary>The range upper bound used by numeric converter tests.</summary>
    private const double Ten = 10D;

    /// <summary>The expected mid-range parsed value.</summary>
    private const double FourPointFive = 4.5D;

    /// <summary>The expected negative animation output.</summary>
    private const double NegativeFive = -5D;

    /// <summary>The expected animation output.</summary>
    private const double Five = 5D;

    /// <summary>The standard progress input.</summary>
    private const double ProgressInput = 80D;

    /// <summary>The expected progress thickness.</summary>
    private const double ExpectedProgressThickness = 10D;

    /// <summary>The fallback progress thickness.</summary>
    private const double FallbackProgressThickness = 12D;

    /// <summary>The divisor used for divided-size conversion.</summary>
    private const double SizeDivisor = 4D;

    /// <summary>The expected divided-size conversion result.</summary>
    private const double ExpectedDividedSize = 20D;

    /// <summary>The first proportional input.</summary>
    private const double ProportionalInput = 10D;

    /// <summary>The second proportional input.</summary>
    private const double ProportionalMultiplier = 8D;

    /// <summary>The third proportional input.</summary>
    private const double ProportionalDivisor = 2D;

    /// <summary>The fully opaque alpha component.</summary>
    private const byte FullAlpha = 255;

    /// <summary>The red component of the short hexadecimal color form.</summary>
    private const byte ShortHexRed = 170;

    /// <summary>The green component of the short hexadecimal color form.</summary>
    private const byte ShortHexGreen = 187;

    /// <summary>The blue component of the short hexadecimal color form.</summary>
    private const byte ShortHexBlue = 204;

    /// <summary>The alpha component of the four-digit hexadecimal color form.</summary>
    private const byte ShortHexAlpha = 17;

    /// <summary>The expected number of alpha-change events.</summary>
    private const int ExpectedAlphaChangeEventCount = 1;

    /// <summary>The year used by date conversion tests.</summary>
    private const int TestYear = 2026;

    /// <summary>The month used by date conversion tests.</summary>
    private const int TestMonth = 7;

    /// <summary>The day used by date conversion tests.</summary>
    private const int TestDay = 26;

    /// <summary>The hour used by date conversion tests.</summary>
    private const int TestHour = 12;

    /// <summary>The zero integer used by integer converter tests.</summary>
    private const int IntegerZero = 0;

    /// <summary>The one integer used by integer converter tests.</summary>
    private const int IntegerOne = 1;

    /// <summary>The two integer used by integer converter tests.</summary>
    private const int IntegerTwo = 2;

    /// <summary>The three integer used by integer converter tests.</summary>
    private const int IntegerThree = 3;

    /// <summary>The standard invalid converter input.</summary>
    private const string InvalidValue = "invalid";

    /// <summary>The unchanged geometry fallback value.</summary>
    private const string UnchangedValue = "unchanged";

    /// <summary>Provides the expected proportional conversion result.</summary>
    private const double ExpectedProportionalValue = 40D;

    /// <summary>Provides the color represented by the short hexadecimal form.</summary>
    private static readonly Color ShortHexColor =
        Color.FromArgb(FullAlpha, ShortHexRed, ShortHexGreen, ShortHexBlue);

    /// <summary>Provides the color represented by the alpha short hexadecimal form.</summary>
    private static readonly Color ShortHexAlphaColor =
        Color.FromArgb(ShortHexAlpha, ShortHexRed, ShortHexGreen, ShortHexBlue);

    /// <summary>Verifies boolean, text, progress, and divided-size converters across both variants.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ScalarConverters_ProjectExpectedValuesAndFallbacks()
    {
        IValueConverter[] inverted =
        [
            new StandardConverters.BoolToInvertedBoolConverter(),
            new ReactiveConverters.BoolToInvertedBoolConverter(),
        ];
        IValueConverter[] text =
        [
            new StandardConverters.TextToAsteriskConverter(),
            new ReactiveConverters.TextToAsteriskConverter(),
        ];
        IValueConverter[] progress =
        [
            new StandardConverters.ProgressThicknessConverter(),
            new ReactiveConverters.ProgressThicknessConverter(),
        ];
        IValueConverter[] divided =
        [
            new StandardConverters.DividedSizeConverter(),
            new ReactiveConverters.DividedSizeConverter(),
        ];

        foreach (var converter in inverted)
        {
            await Assert.That(RequireBoolean(converter.Convert(true, typeof(bool), null!, CultureInfo.InvariantCulture))).IsFalse();
            await Assert.That(RequireBoolean(converter.Convert(false, typeof(bool), null!, CultureInfo.InvariantCulture))).IsTrue();
            await Assert.That(RequireBoolean(converter.Convert(InvalidValue, typeof(bool), null!, CultureInfo.InvariantCulture))).IsFalse();
            await Assert.That(converter.ConvertBack(true, typeof(bool), null!, CultureInfo.InvariantCulture)).IsEqualTo(Binding.DoNothing);
        }

        foreach (var converter in text)
        {
            await Assert.That(converter.Convert("secret", typeof(string), null!, CultureInfo.InvariantCulture)).IsEqualTo("******");
            await Assert.That(converter.Convert(null!, typeof(string), null!, CultureInfo.InvariantCulture)).IsEqualTo(string.Empty);
            await Assert.That(converter.ConvertBack("ignored", typeof(string), null!, CultureInfo.InvariantCulture)).IsEqualTo(Binding.DoNothing);
        }

        foreach (var converter in progress)
        {
            await Assert.That(converter.Convert(ProgressInput, typeof(double), null!, CultureInfo.InvariantCulture)).IsEqualTo(ExpectedProgressThickness);
            await Assert.That(converter.Convert(InvalidValue, typeof(double), null!, CultureInfo.InvariantCulture)).IsEqualTo(FallbackProgressThickness);
            await Assert.That(converter.ConvertBack(One, typeof(double), null!, CultureInfo.InvariantCulture)).IsEqualTo(Binding.DoNothing);
        }

        foreach (var converter in divided)
        {
            await Assert.That(converter.Convert(ProgressInput, typeof(double), SizeDivisor, CultureInfo.InvariantCulture)).IsEqualTo(ExpectedDividedSize);
            await Assert.That(converter.ConvertBack(One, typeof(double), null!, CultureInfo.InvariantCulture)).IsEqualTo(Binding.DoNothing);
        }
    }

    /// <summary>Verifies brush and color converters preserve color values and documented fallbacks.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BrushConverters_PreserveColorsAndUseRedFallback()
    {
        IValueConverter[] brushToColor =
        [
            new StandardConverters.BrushToColorConverter(),
            new ReactiveConverters.BrushToColorConverter(),
        ];
        IValueConverter[] colorToBrush =
        [
            new StandardConverters.ColorToBrushConverter(),
            new ReactiveConverters.ColorToBrushConverter(),
        ];
        IValueConverter[] fallbackBrush =
        [
            new StandardConverters.FallbackBrushConverter(),
            new ReactiveConverters.FallbackBrushConverter(),
        ];
        var blueBrush = new SolidColorBrush(Colors.CornflowerBlue);

        foreach (var converter in brushToColor)
        {
            await Assert.That(converter.Convert(blueBrush, typeof(Color), null!, CultureInfo.InvariantCulture)).IsEqualTo(Colors.CornflowerBlue);
            await Assert.That(converter.Convert(Colors.Green, typeof(Color), null!, CultureInfo.InvariantCulture)).IsEqualTo(Colors.Green);
            await Assert.That(converter.Convert(InvalidValue, typeof(Color), null!, CultureInfo.InvariantCulture)).IsEqualTo(Colors.Red);
            await Assert.That(converter.ConvertBack(Colors.Red, typeof(Brush), null!, CultureInfo.InvariantCulture)).IsEqualTo(Binding.DoNothing);
        }

        foreach (var converter in colorToBrush)
        {
            var converted = (SolidColorBrush)converter.Convert(
                Colors.CornflowerBlue,
                typeof(Brush),
                null!,
                CultureInfo.InvariantCulture);
            var convertedBack = (Color)converter.ConvertBack(
                converted,
                typeof(Color),
                null!,
                CultureInfo.InvariantCulture);

            await Assert.That(converted.Color).IsEqualTo(Colors.CornflowerBlue);
            await Assert.That(convertedBack).IsEqualTo(Colors.CornflowerBlue);
        }

        foreach (var converter in fallbackBrush)
        {
            await Assert.That(converter.Convert(blueBrush, typeof(Brush), null!, CultureInfo.InvariantCulture)).IsSameReferenceAs(blueBrush);
            await Assert
                .That(((SolidColorBrush)converter.Convert(Colors.Green, typeof(Brush), null!, CultureInfo.InvariantCulture)).Color)
                .IsEqualTo(Colors.Green);
            await Assert
                .That(((SolidColorBrush)converter.Convert(InvalidValue, typeof(Brush), null!, CultureInfo.InvariantCulture)).Color)
                .IsEqualTo(Colors.Red);
            await Assert.That(converter.ConvertBack(null, typeof(Color), null, CultureInfo.InvariantCulture)).IsEqualTo(Binding.DoNothing);
        }
    }

    /// <summary>Verifies hexadecimal color conversion supports short, long, alpha, and invalid forms.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task HexConverters_HandleAllSupportedShapesAndInvalidValues()
    {
        var standard = new StandardConverters.ColorToHexConverter();
        var reactive = new ReactiveConverters.ColorToHexConverter();
        var standardEventCount = IntegerZero;
        var reactiveEventCount = IntegerZero;
        standard.OnShowAlphaChange += (_, _) => standardEventCount++;
        reactive.OnShowAlphaChange += (_, _) => reactiveEventCount++;

        foreach (var converter in new IValueConverter[] { standard, reactive })
        {
            await Assert.That(converter.Convert(null!, typeof(string), null!, CultureInfo.InvariantCulture)).IsEqualTo(DependencyProperty.UnsetValue);
            await Assert.That(converter.Convert(Colors.CornflowerBlue, typeof(string), null!, CultureInfo.InvariantCulture)).IsEqualTo("#FF6495ED");
            await Assert.That(converter.ConvertBack("#abc", typeof(Color), null!, CultureInfo.InvariantCulture)).IsEqualTo(ShortHexColor);
            await Assert.That(converter.ConvertBack("#1abc", typeof(Color), null!, CultureInfo.InvariantCulture)).IsEqualTo(ShortHexAlphaColor);
            await Assert.That(converter.ConvertBack("6495ED", typeof(Color), null!, CultureInfo.InvariantCulture)).IsEqualTo(Colors.CornflowerBlue);
            var invalidColor = converter.ConvertBack("not-a-color", typeof(Color), null!, CultureInfo.InvariantCulture);
            await Assert.That(invalidColor).IsEqualTo(DependencyProperty.UnsetValue);
            await Assert.That(converter.ConvertBack(null!, typeof(Color), null!, CultureInfo.InvariantCulture)).IsEqualTo(DependencyProperty.UnsetValue);
        }

        standard.ShowAlpha = false;
        reactive.ShowAlpha = false;
        await Assert.That(standardEventCount).IsEqualTo(ExpectedAlphaChangeEventCount);
        await Assert.That(reactiveEventCount).IsEqualTo(ExpectedAlphaChangeEventCount);
        await Assert.That(standard.Convert(Colors.CornflowerBlue, typeof(string), null!, CultureInfo.InvariantCulture)).IsEqualTo("#6495ED");
        await Assert.That(reactive.Convert(Colors.CornflowerBlue, typeof(string), null!, CultureInfo.InvariantCulture)).IsEqualTo("#6495ED");
        await Assert.That(StandardConverters.ColorToHexConverter.ConvertBackNoAlpha("#abc")).IsEqualTo(ShortHexColor);
        await Assert.That(ReactiveConverters.ColorToHexConverter.ConvertBackNoAlpha("#abc")).IsEqualTo(ShortHexColor);
        await Assert.That(StandardConverters.ColorToHexConverter.ConvertBackNoAlpha("#abcd")).IsEqualTo(DependencyProperty.UnsetValue);
        await Assert.That(ReactiveConverters.ColorToHexConverter.ConvertBackNoAlpha("#1234567")).IsEqualTo(DependencyProperty.UnsetValue);
        await Assert.That(static () => StandardConverters.ColorToHexConverter.ConvertNoAlpha(null!)).Throws<ArgumentNullException>();
        await Assert.That(static () => ReactiveConverters.ColorToHexConverter.ConvertBackNoAlpha(null!)).Throws<ArgumentNullException>();
    }

    /// <summary>Verifies multi-value converters cover valid, fallback, negative, and invalid-input branches.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task MultiValueConverters_HandleValidAndFallbackInputs()
    {
        IMultiValueConverter[] animation =
        [
            new StandardConverters.AnimationFactorToValueConverter(),
            new ReactiveConverters.AnimationFactorToValueConverter(),
        ];
        IMultiValueConverter[] minimum =
        [
            new StandardConverters.MinConverter(),
            new ReactiveConverters.MinConverter(),
        ];
        IMultiValueConverter[] proportional =
        [
            new StandardConverters.ProportialConverter(),
            new ReactiveConverters.ProportialConverter(),
        ];

        foreach (var converter in animation)
        {
            await Assert.That(converter.Convert([ProportionalInput, Half], typeof(double), string.Empty, CultureInfo.InvariantCulture)).IsEqualTo(Five);
            await Assert.That(converter.Convert([ProportionalInput, Half], typeof(double), "negative", CultureInfo.InvariantCulture)).IsEqualTo(NegativeFive);
            await Assert.That(converter.Convert([InvalidValue, Half], typeof(double), string.Empty, CultureInfo.InvariantCulture)).IsEqualTo(Zero);
            await Assert.That(converter.Convert([ProportionalInput, InvalidValue], typeof(double), string.Empty, CultureInfo.InvariantCulture)).IsEqualTo(Zero);
            await Assert.That(converter.ConvertBack(Zero, [typeof(double)], null!, CultureInfo.InvariantCulture)).Contains(Binding.DoNothing);
        }

        foreach (var converter in minimum)
        {
            var minimumValue = converter.Convert([IntegerThree, IntegerOne, IntegerTwo], typeof(int), null!, CultureInfo.InvariantCulture);
            await Assert.That(minimumValue).IsEqualTo(IntegerOne);
            await Assert.That(() => converter.Convert([], typeof(int), null!, CultureInfo.InvariantCulture)).Throws<InvalidOperationException>();
            await Assert.That(converter.ConvertBack(IntegerZero, [typeof(int)], null!, CultureInfo.InvariantCulture)).Contains(Binding.DoNothing);
        }

        foreach (var converter in proportional)
        {
            var proportionalValue = converter.Convert([ProportionalInput, ProportionalMultiplier, ProportionalDivisor], typeof(double), null!, CultureInfo.InvariantCulture);
            var invalidFirstValue = converter.Convert([InvalidValue, ProportionalMultiplier, ProportionalDivisor], typeof(double), null!, CultureInfo.InvariantCulture);
            var invalidSecondValue = converter.Convert([ProportionalInput, InvalidValue, ProportionalDivisor], typeof(double), null!, CultureInfo.InvariantCulture);
            await Assert.That(proportionalValue).IsEqualTo(ExpectedProportionalValue);
            await Assert.That(invalidFirstValue).IsEqualTo(Zero);
            await Assert.That(invalidSecondValue).IsEqualTo(Zero);
            await Assert.That(converter.ConvertBack(IntegerZero, [typeof(double)], null!, CultureInfo.InvariantCulture)).Contains(Binding.DoNothing);
        }
    }

    /// <summary>Verifies date and enum converters preserve values and reject mismatched enum inputs.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task DateAndEnumConverters_EnforceTypedContracts()
    {
        IValueConverter[] dates =
        [
            new StandardConverters.DateTimeOffsetToDateTimeConverter(),
            new ReactiveConverters.DateTimeOffsetToDateTimeConverter(),
        ];
        IValueConverter[] enums =
        [
            new StandardConverters.EnumToBoolConverter<DayOfWeek>(),
            new ReactiveConverters.EnumToBoolConverter<DayOfWeek>(),
        ];
        var date = new DateTimeOffset(TestYear, TestMonth, TestDay, TestHour, IntegerZero, IntegerZero, TimeSpan.Zero);

        foreach (var converter in dates)
        {
            var local = converter.Convert(date, typeof(DateTime), null, CultureInfo.InvariantCulture);
            await Assert.That(local is DateTime).IsTrue();
            await Assert.That(converter.Convert(null, typeof(DateTime), null, CultureInfo.InvariantCulture)).IsNull();
            await Assert.That(converter.Convert(InvalidValue, typeof(DateTime), null, CultureInfo.InvariantCulture)).IsEqualTo(Binding.DoNothing);
            await Assert.That(converter.ConvertBack(local, typeof(DateTimeOffset), null, CultureInfo.InvariantCulture) is DateTimeOffset).IsTrue();
            await Assert.That(converter.ConvertBack(null, typeof(DateTimeOffset), null, CultureInfo.InvariantCulture)).IsNull();
            await Assert.That(converter.ConvertBack(InvalidValue, typeof(DateTimeOffset), null, CultureInfo.InvariantCulture)).IsEqualTo(Binding.DoNothing);
        }

        foreach (var converter in enums)
        {
            await Assert.That(RequireBoolean(converter.Convert(DayOfWeek.Sunday, typeof(bool), DayOfWeek.Sunday, CultureInfo.InvariantCulture))).IsTrue();
            await Assert.That(RequireBoolean(converter.Convert(DayOfWeek.Monday, typeof(bool), DayOfWeek.Sunday, CultureInfo.InvariantCulture))).IsFalse();
            await Assert.That(() => converter.Convert(InvalidValue, typeof(bool), DayOfWeek.Sunday, CultureInfo.InvariantCulture)).Throws<ArgumentException>();
            await Assert.That(() => converter.Convert(DayOfWeek.Sunday, typeof(bool), InvalidValue, CultureInfo.InvariantCulture)).Throws<ArgumentException>();
            await Assert.That(converter.ConvertBack(true, typeof(DayOfWeek), DayOfWeek.Sunday, CultureInfo.InvariantCulture)).IsEqualTo(Binding.DoNothing);
        }
    }

    /// <summary>Verifies range, split-geometry, visibility, and picker converters preserve their contracts.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task LayoutConverters_ClampAndProjectTypedValues()
    {
        await AssertRangeConvertersAsync();
        await AssertSplitGeometryConvertersAsync();
        await AssertVisibilityConvertersAsync();
        await AssertPickerConvertersAsync();
    }

    /// <summary>Verifies standard and reactive range converters clamp parsed values to their configured bounds.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task AssertRangeConvertersAsync()
    {
        var standardRange = new StandardConverters.RangeConstrainedDoubleToDoubleConverter { Min = Zero, Max = Ten };
        var reactiveRange = new ReactiveConverters.RangeConstrainedDoubleToDoubleConverter { Min = Zero, Max = Ten };

        foreach (IValueConverter converter in new IValueConverter[] { standardRange, reactiveRange })
        {
            await Assert.That(converter.Convert(Four, typeof(double), null!, CultureInfo.InvariantCulture)).IsEqualTo(Four);
            await Assert.That(converter.ConvertBack("20", typeof(double), null!, CultureInfo.InvariantCulture)).IsEqualTo(Ten);
            await Assert.That(converter.ConvertBack("-2", typeof(double), null!, CultureInfo.InvariantCulture)).IsEqualTo(Zero);
            await Assert.That(converter.ConvertBack("4,5", typeof(double), null!, CultureInfo.InvariantCulture)).IsEqualTo(FourPointFive);
            await Assert.That(converter.ConvertBack(InvalidValue, typeof(double), null!, CultureInfo.InvariantCulture)).IsEqualTo(DependencyProperty.UnsetValue);
            await Assert.That(() => converter.ConvertBack(null!, typeof(double), null!, CultureInfo.InvariantCulture)).Throws<ArgumentNullException>();
        }
    }

    /// <summary>Verifies split converters project thickness and corner geometry for both UI variants.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task AssertSplitGeometryConvertersAsync()
    {
        IValueConverter[] leftThickness =
        [
            new StandardConverters.LeftSplitThicknessConverter(),
            new ReactiveConverters.LeftSplitThicknessConverter(),
        ];
        IValueConverter[] rightThickness =
        [
            new StandardConverters.RightSplitThicknessConverter(),
            new ReactiveConverters.RightSplitThicknessConverter(),
        ];
        IValueConverter[] leftCorner =
        [
            new StandardConverters.LeftSplitCornerRadiusConverter(),
            new ReactiveConverters.LeftSplitCornerRadiusConverter(),
        ];
        IValueConverter[] rightCorner =
        [
            new StandardConverters.RightSplitCornerRadiusConverter(),
            new ReactiveConverters.RightSplitCornerRadiusConverter(),
        ];
        var thickness = new Thickness(One, Two, Three, Four);
        var corner = new CornerRadius(One, Two, Three, Four);

        foreach (var converter in leftThickness)
        {
            var convertedThickness = converter.Convert(thickness, typeof(Thickness), null!, CultureInfo.InvariantCulture);
            await Assert.That(convertedThickness).IsEqualTo(new Thickness(One, Two, Zero, Four));
            await Assert.That(converter.Convert(UnchangedValue, typeof(Thickness), null!, CultureInfo.InvariantCulture)).IsEqualTo(UnchangedValue);
            await Assert.That(converter.ConvertBack(thickness, typeof(Thickness), null!, CultureInfo.InvariantCulture)).IsEqualTo(Binding.DoNothing);
        }

        foreach (var converter in rightThickness)
        {
            var convertedThickness = converter.Convert(thickness, typeof(Thickness), null!, CultureInfo.InvariantCulture);
            await Assert.That(convertedThickness).IsEqualTo(new Thickness(Zero, Two, Three, Four));
            await Assert.That(converter.Convert(UnchangedValue, typeof(Thickness), null!, CultureInfo.InvariantCulture)).IsEqualTo(UnchangedValue);
            await Assert.That(converter.ConvertBack(thickness, typeof(Thickness), null!, CultureInfo.InvariantCulture)).IsEqualTo(Binding.DoNothing);
        }

        foreach (var converter in leftCorner)
        {
            var convertedCorner = converter.Convert(corner, typeof(CornerRadius), null!, CultureInfo.InvariantCulture);
            await Assert.That(convertedCorner).IsEqualTo(new CornerRadius(One, Zero, Zero, Four));
            await Assert.That(converter.Convert(UnchangedValue, typeof(CornerRadius), null!, CultureInfo.InvariantCulture)).IsEqualTo(UnchangedValue);
            await Assert.That(converter.ConvertBack(corner, typeof(CornerRadius), null!, CultureInfo.InvariantCulture)).IsEqualTo(Binding.DoNothing);
        }

        foreach (var converter in rightCorner)
        {
            var convertedCorner = converter.Convert(corner, typeof(CornerRadius), null!, CultureInfo.InvariantCulture);
            await Assert.That(convertedCorner).IsEqualTo(new CornerRadius(Zero, Two, Three, Zero));
            await Assert.That(converter.Convert(UnchangedValue, typeof(CornerRadius), null!, CultureInfo.InvariantCulture)).IsEqualTo(UnchangedValue);
            await Assert.That(converter.ConvertBack(corner, typeof(CornerRadius), null!, CultureInfo.InvariantCulture)).IsEqualTo(Binding.DoNothing);
        }
    }

    /// <summary>Verifies back-button visibility conversion for both UI variants.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task AssertVisibilityConvertersAsync()
    {
        var standardVisibility = new StandardConverters.BackButtonVisibilityToVisibilityConverter();
        var reactiveVisibility = new ReactiveConverters.BackButtonVisibilityToVisibilityConverter();
        var standardCollapsed = standardVisibility.Convert(StandardBackButtonVisibility.Collapsed, typeof(Visibility), null!, CultureInfo.InvariantCulture);
        var standardVisible = standardVisibility.Convert(StandardBackButtonVisibility.Visible, typeof(Visibility), null!, CultureInfo.InvariantCulture);
        var reactiveCollapsed = reactiveVisibility.Convert(ReactiveBackButtonVisibility.Collapsed, typeof(Visibility), null!, CultureInfo.InvariantCulture);
        await Assert.That(standardCollapsed).IsEqualTo(Visibility.Collapsed);
        await Assert.That(standardVisible).IsEqualTo(Visibility.Visible);
        await Assert.That(reactiveCollapsed).IsEqualTo(Visibility.Collapsed);
        await Assert.That(reactiveVisibility.Convert(InvalidValue, typeof(Visibility), null!, CultureInfo.InvariantCulture)).IsEqualTo(Visibility.Collapsed);
        await Assert.That(standardVisibility.ConvertBack(Visibility.Visible, typeof(object), null!, CultureInfo.InvariantCulture)).IsEqualTo(Binding.DoNothing);
        await Assert.That(reactiveVisibility.ConvertBack(Visibility.Visible, typeof(object), null!, CultureInfo.InvariantCulture)).IsEqualTo(Binding.DoNothing);
    }

    /// <summary>Verifies picker type conversion for both UI variants.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task AssertPickerConvertersAsync()
    {
        var standardPicker = new StandardConverters.PickerTypeToIntConverter();
        var reactivePicker = new ReactiveConverters.PickerTypeToIntConverter();
        await Assert.That(standardPicker.Convert(StandardPickerType.HSL, typeof(int), null!, CultureInfo.InvariantCulture)).IsEqualTo(IntegerOne);
        var standardPickerType = standardPicker.ConvertBack(IntegerZero, typeof(StandardPickerType), null!, CultureInfo.InvariantCulture);
        await Assert.That(standardPickerType).IsEqualTo(StandardPickerType.HSV);
        await Assert.That(reactivePicker.Convert(ReactivePickerType.HSL, typeof(int), null!, CultureInfo.InvariantCulture)).IsEqualTo(IntegerOne);
        var reactivePickerType = reactivePicker.ConvertBack(IntegerZero, typeof(ReactivePickerType), null!, CultureInfo.InvariantCulture);
        await Assert.That(reactivePickerType).IsEqualTo(ReactivePickerType.HSV);
    }

    /// <summary>Returns a converter result as a required boolean value.</summary>
    /// <param name="value">The converter result.</param>
    /// <returns>The boolean result.</returns>
    private static bool RequireBoolean(object? value) => value as bool? ?? throw new InvalidOperationException();
}
