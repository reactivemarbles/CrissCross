// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Xml.Linq;
using CrissCross.Maui.UI;
using CrissCross.Maui.UI.Resources.Styles;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace CrissCross.Tests;

/// <summary>Verifies the MAUI high-contrast theme resource path.</summary>
public sealed class MauiHighContrastThemeTests
{
    /// <summary>Provides the primary text contrast threshold required by WCAG 2.1 AA.</summary>
    private const double PrimaryTextMinimumContrast = 4.5;

    /// <summary>Provides the maximum color byte value.</summary>
    private const byte ByteMaximum = byte.MaxValue;

    /// <summary>Provides the RGB hex string length.</summary>
    private const int RgbHexLength = 6;

    /// <summary>Provides the ARGB hex string length.</summary>
    private const int ArgbHexLength = 8;

    /// <summary>Provides the length of one hexadecimal byte.</summary>
    private const int HexByteLength = 2;

    /// <summary>Provides the red byte offset in a hexadecimal color.</summary>
    private const int RedOffset = 0;

    /// <summary>Provides the green byte offset in a hexadecimal color.</summary>
    private const int GreenOffset = 2;

    /// <summary>Provides the blue byte offset in a hexadecimal color.</summary>
    private const int BlueOffset = 4;

    /// <summary>Provides the alpha byte offset in an ARGB hexadecimal color.</summary>
    private const int AlphaOffset = 0;

    /// <summary>Provides the red byte offset in an ARGB hexadecimal color.</summary>
    private const int ArgbRedOffset = 2;

    /// <summary>Provides the green byte offset in an ARGB hexadecimal color.</summary>
    private const int ArgbGreenOffset = 4;

    /// <summary>Provides the blue byte offset in an ARGB hexadecimal color.</summary>
    private const int ArgbBlueOffset = 6;

    /// <summary>Provides the WCAG contrast luminance offset.</summary>
    private const double ContrastOffset = 0.05;

    /// <summary>Provides the MAUI UI project directory name.</summary>
    private const string MauiUiProject = "CrissCross.Maui.UI";

    /// <summary>Provides the shared resource directory name.</summary>
    private const string ResourcesDirectory = "Resources";

    /// <summary>Provides the MAUI styles directory name.</summary>
    private const string StylesDirectory = "Styles";

    /// <summary>Provides the x namespace used by MAUI resource dictionaries.</summary>
    private static readonly XNamespace MauiXamlNamespace = "http://schemas.microsoft.com/winfx/2009/xaml";

    /// <summary>Provides the source root used to locate platform resource dictionaries.</summary>
    private static readonly string SourceRoot = LocateSourceRoot();

    /// <summary>Verifies high contrast can be activated and removed through the MAUI resource extension.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task UseCrissCrossMauiUiResources_HighContrastTogglesOverrideDictionary()
    {
        var resources = new ResourceDictionary();
        var highContrast = new ThemePreferenceState(ThemeChoice.HighContrast, ThemeChoice.Light, supportsHighContrast: true);

        _ = resources.UseCrissCrossMauiUiResources(highContrast);
        _ = resources.UseCrissCrossMauiUiResources(highContrast);

        await Assert.That(CountDictionaries<CrissCrossMauiUi>(resources)).IsEqualTo(1);
        await Assert.That(CountDictionaries<CrissCrossMauiHighContrastTheme>(resources)).IsEqualTo(1);
        await Assert.That(CountDictionaries<CrissCrossMauiLightTheme>(resources)).IsEqualTo(0);

        _ = resources.UseCrissCrossMauiUiResources(ThemeChoice.Dark);

        await Assert.That(CountDictionaries<CrissCrossMauiUi>(resources)).IsEqualTo(1);
        await Assert.That(CountDictionaries<CrissCrossMauiHighContrastTheme>(resources)).IsEqualTo(0);
        await Assert.That(CountDictionaries<CrissCrossMauiDarkTheme>(resources)).IsEqualTo(1);
    }

    /// <summary>Verifies a System theme snapshot can use a Dark fallback when no application is available.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task UseCrissCrossMauiUiResources_SystemThemeStateUsesDarkFallbackWhenHeadless()
    {
        var previousApplication = Application.Current;
        Application.Current = null;

        try
        {
            var resources = new ResourceDictionary();
            var systemDark = new ThemePreferenceState(ThemeChoice.System, ThemeChoice.Dark, supportsHighContrast: true);

            _ = resources.UseCrissCrossMauiUiResources(systemDark);

            await Assert.That(CountDictionaries<CrissCrossMauiUi>(resources)).IsEqualTo(1);
            await Assert.That(CountDictionaries<CrissCrossMauiDarkTheme>(resources)).IsEqualTo(1);
            await Assert.That(CountDictionaries<CrissCrossMauiLightTheme>(resources)).IsEqualTo(0);
            await Assert.That(CountDictionaries<CrissCrossMauiHighContrastTheme>(resources)).IsEqualTo(0);
        }
        finally
        {
            Application.Current = previousApplication;
        }
    }

    /// <summary>Verifies the default and System-choice paths follow runtime requested-theme changes.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task UseCrissCrossMauiUiResources_SystemPathsFollowRequestedThemeChanges()
    {
        var previousApplication = Application.Current;
        var application = new Application { UserAppTheme = AppTheme.Dark };
        var systemChoiceResources = new ResourceDictionary();
        Application.SetCurrentApplication(application);

        try
        {
            _ = application.Resources.UseCrissCrossMauiUiResources();
            _ = systemChoiceResources.UseCrissCrossMauiUiResources(ThemeChoice.System);

            await AssertHasTheme<CrissCrossMauiDarkTheme>(application.Resources);
            await AssertHasTheme<CrissCrossMauiDarkTheme>(systemChoiceResources);

            application.UserAppTheme = AppTheme.Light;

            await AssertHasTheme<CrissCrossMauiLightTheme>(application.Resources);
            await AssertHasTheme<CrissCrossMauiLightTheme>(systemChoiceResources);
        }
        finally
        {
            Application.Current = previousApplication;
        }
    }

    /// <summary>Verifies explicit high contrast remains fixed after runtime requested-theme changes.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task UseCrissCrossMauiUiResources_ExplicitHighContrastIgnoresRequestedThemeChanges()
    {
        var previousApplication = Application.Current;
        var application = new Application { UserAppTheme = AppTheme.Light };
        Application.SetCurrentApplication(application);

        try
        {
            _ = application.Resources.UseCrissCrossMauiUiResources();
            _ = application.Resources.UseCrissCrossMauiUiResources(ThemeChoice.HighContrast);

            await AssertHasTheme<CrissCrossMauiHighContrastTheme>(application.Resources);

            application.UserAppTheme = AppTheme.Dark;

            await AssertHasTheme<CrissCrossMauiHighContrastTheme>(application.Resources);
        }
        finally
        {
            Application.Current = previousApplication;
        }
    }

    /// <summary>Verifies MAUI control styles consume dynamic semantic tokens that high contrast can override.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ControlsStyles_UseDynamicSemanticTokensForHighContrastOverride()
    {
        var styles = await File.ReadAllTextAsync(GetMauiStylePath("Controls.xaml"));

        await Assert.That(styles).DoesNotContain("AppThemeBinding");
        await Assert.That(styles).Contains("{DynamicResource CrissCrossAccentColor}");
        await Assert.That(styles).Contains("{DynamicResource CrissCrossSurfaceColor}");
        await Assert.That(styles).Contains("{DynamicResource CrissCrossSuccessColor}");
        await Assert.That(styles).Contains("{DynamicResource CrissCrossDangerColor}");
        await Assert.That(styles).Contains("{DynamicResource CrissCrossCautionColor}");
        await Assert.That(styles).Contains("{DynamicResource CrissCrossNeutralSurfaceColor}");
    }

    /// <summary>Verifies high contrast defines every active MAUI semantic token with readable foregrounds.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task HighContrastTheme_OverridesAllActiveSemanticTokensAndKeepsTextReadable()
    {
        var lightColors = ReadColorResources(GetMauiStylePath("Light.xaml"));
        var darkColors = ReadColorResources(GetMauiStylePath("Dark.xaml"));
        var highContrastColors = ReadColorResources(GetMauiStylePath("HighContrast.xaml"));

        await Assert.That(GetMissingKeys(lightColors.Keys, highContrastColors.Keys)).IsEmpty();
        await Assert.That(GetMissingKeys(darkColors.Keys, highContrastColors.Keys)).IsEmpty();
        await Assert.That(GetContrast(highContrastColors["CrissCrossTextColor"], highContrastColors["CrissCrossSurfaceColor"]))
            .IsGreaterThanOrEqualTo(PrimaryTextMinimumContrast);
        await Assert.That(GetContrast(highContrastColors["CrissCrossAccentTextColor"], highContrastColors["CrissCrossAccentColor"]))
            .IsGreaterThanOrEqualTo(PrimaryTextMinimumContrast);
    }

    /// <summary>Verifies exactly one concrete MAUI theme override is active.</summary>
    /// <typeparam name="T">The expected concrete MAUI theme resource dictionary type.</typeparam>
    /// <param name="resources">The resources to inspect.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private static async Task AssertHasTheme<T>(ResourceDictionary resources)
        where T : ResourceDictionary
    {
        await Assert.That(CountDictionaries<CrissCrossMauiUi>(resources)).IsEqualTo(1);
        await Assert.That(CountDictionaries<T>(resources)).IsEqualTo(1);
        await Assert.That(CountDictionaries<CrissCrossMauiLightTheme>(resources)
            + CountDictionaries<CrissCrossMauiDarkTheme>(resources)
            + CountDictionaries<CrissCrossMauiHighContrastTheme>(resources)).IsEqualTo(1);
    }

    /// <summary>Counts merged dictionaries assignable to a requested type.</summary>
    /// <typeparam name="T">The dictionary type to count.</typeparam>
    /// <param name="resources">The resources to inspect.</param>
    /// <returns>The matching dictionary count.</returns>
    private static int CountDictionaries<T>(ResourceDictionary resources)
        where T : ResourceDictionary
    {
        var count = 0;
        foreach (var dictionary in resources.MergedDictionaries)
        {
            if (dictionary is T)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>Gets the path for a MAUI shared style dictionary.</summary>
    /// <param name="fileName">The style dictionary file name.</param>
    /// <returns>The style dictionary path.</returns>
    private static string GetMauiStylePath(string fileName) =>
        Path.Combine(SourceRoot, MauiUiProject, ResourcesDirectory, StylesDirectory, fileName);

    /// <summary>Reads keyed literal colors from a MAUI resource dictionary.</summary>
    /// <param name="path">The resource dictionary path.</param>
    /// <returns>The keyed colors.</returns>
    private static Dictionary<string, RgbaColor> ReadColorResources(string path)
    {
        var colors = new Dictionary<string, RgbaColor>(StringComparer.Ordinal);
        foreach (var element in XDocument.Load(path).Descendants())
        {
            if (element.Name.LocalName != "Color")
            {
                continue;
            }

            var key = GetXamlKey(element);
            if (key is not null)
            {
                colors.Add(key, ParseColor(element.Value));
            }
        }

        return colors;
    }

    /// <summary>Gets a resource key from the MAUI x namespace.</summary>
    /// <param name="element">The resource dictionary element.</param>
    /// <returns>The resource key when present.</returns>
    private static string? GetXamlKey(XElement element) => (string?)element.Attribute(MauiXamlNamespace + "Key");

    /// <summary>Gets the required keys that are absent from the available resource keys.</summary>
    /// <param name="requiredKeys">The keys that must exist.</param>
    /// <param name="availableKeys">The keys currently supplied by a dictionary.</param>
    /// <returns>The missing resource keys.</returns>
    private static string[] GetMissingKeys(IEnumerable<string> requiredKeys, IEnumerable<string> availableKeys)
    {
        var available = new HashSet<string>(availableKeys, StringComparer.Ordinal);
        var missing = new List<string>();
        foreach (var key in requiredKeys)
        {
            if (!available.Contains(key))
            {
                missing.Add(key);
            }
        }

        return missing.ToArray();
    }

    /// <summary>Calculates WCAG contrast after compositing a foreground color on an opaque background.</summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The opaque background color.</param>
    /// <returns>The contrast ratio.</returns>
    private static double GetContrast(RgbaColor foreground, RgbaColor background)
    {
        var foregroundLuminance = foreground.CompositeOn(background).GetRelativeLuminance();
        var backgroundLuminance = background.GetRelativeLuminance();
        return (Math.Max(foregroundLuminance, backgroundLuminance) + ContrastOffset)
            / (Math.Min(foregroundLuminance, backgroundLuminance) + ContrastOffset);
    }

    /// <summary>Parses an RGB or ARGB hexadecimal resource color.</summary>
    /// <param name="value">The resource color value.</param>
    /// <returns>The parsed color.</returns>
    private static RgbaColor ParseColor(string value)
    {
        var hex = value.Trim().TrimStart('#');
        return hex.Length switch
        {
            RgbHexLength => new(ByteMaximum, ParseHexByte(hex, RedOffset), ParseHexByte(hex, GreenOffset), ParseHexByte(hex, BlueOffset)),
            ArgbHexLength => new(ParseHexByte(hex, AlphaOffset), ParseHexByte(hex, ArgbRedOffset), ParseHexByte(hex, ArgbGreenOffset), ParseHexByte(hex, ArgbBlueOffset)),
            _ => throw new FormatException($"Unsupported theme color '{value}'."),
        };
    }

    /// <summary>Parses one two-character hexadecimal byte.</summary>
    /// <param name="hex">The complete hexadecimal color.</param>
    /// <param name="offset">The byte offset.</param>
    /// <returns>The parsed byte.</returns>
    private static byte ParseHexByte(string hex, int offset) =>
        byte.Parse(hex.AsSpan(offset, HexByteLength), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

    /// <summary>Locates the source root while supporting MTP's test working directory.</summary>
    /// <returns>The source root path.</returns>
    private static string LocateSourceRoot()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "CrissCross.slnx")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate CrissCross.slnx from the current test working directory.");
    }

    /// <summary>Represents an RGBA color used by contrast calculations.</summary>
    /// <param name="Alpha">The alpha component.</param>
    /// <param name="Red">The red component.</param>
    /// <param name="Green">The green component.</param>
    /// <param name="Blue">The blue component.</param>
    private readonly record struct RgbaColor(byte Alpha, byte Red, byte Green, byte Blue)
    {
        /// <summary>Provides the red coefficient used by relative luminance.</summary>
        private const double RedLuminanceCoefficient = 0.2126;

        /// <summary>Provides the green coefficient used by relative luminance.</summary>
        private const double GreenLuminanceCoefficient = 0.7152;

        /// <summary>Provides the blue coefficient used by relative luminance.</summary>
        private const double BlueLuminanceCoefficient = 0.0722;

        /// <summary>Provides the sRGB linear conversion threshold.</summary>
        private const double SrgbLinearThreshold = 0.04045;

        /// <summary>Provides the sRGB linear divisor.</summary>
        private const double SrgbLinearDivisor = 12.92;

        /// <summary>Provides the sRGB gamma offset.</summary>
        private const double SrgbGammaOffset = 0.055;

        /// <summary>Provides the sRGB gamma divisor.</summary>
        private const double SrgbGammaDivisor = 1.055;

        /// <summary>Provides the sRGB gamma exponent.</summary>
        private const double SrgbGammaExponent = 2.4;

        /// <summary>Composites this color over an opaque background.</summary>
        /// <param name="background">The opaque background color.</param>
        /// <returns>The opaque composited color.</returns>
        public RgbaColor CompositeOn(RgbaColor background)
        {
            var opacity = Alpha / (double)ByteMaximum;
            return new(
                ByteMaximum,
                Blend(Red, background.Red, opacity),
                Blend(Green, background.Green, opacity),
                Blend(Blue, background.Blue, opacity));
        }

        /// <summary>Calculates the WCAG relative luminance of this color.</summary>
        /// <returns>The relative luminance.</returns>
        public double GetRelativeLuminance() =>
            (RedLuminanceCoefficient * ToLinear(Red))
            + (GreenLuminanceCoefficient * ToLinear(Green))
            + (BlueLuminanceCoefficient * ToLinear(Blue));

        /// <summary>Blends a foreground channel over a background channel.</summary>
        /// <param name="foreground">The foreground channel.</param>
        /// <param name="background">The background channel.</param>
        /// <param name="opacity">The foreground opacity.</param>
        /// <returns>The blended channel.</returns>
        private static byte Blend(byte foreground, byte background, double opacity) =>
            (byte)Math.Round((foreground * opacity) + (background * (1D - opacity)), MidpointRounding.AwayFromZero);

        /// <summary>Converts an sRGB byte channel to a linear channel.</summary>
        /// <param name="channel">The sRGB channel.</param>
        /// <returns>The linear channel.</returns>
        private static double ToLinear(byte channel)
        {
            var normalized = channel / (double)ByteMaximum;
            return normalized <= SrgbLinearThreshold
                ? normalized / SrgbLinearDivisor
                : Math.Pow((normalized + SrgbGammaOffset) / SrgbGammaDivisor, SrgbGammaExponent);
        }
    }
}
