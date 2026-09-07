// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using CrissCross.Avalonia.UI.Appearance;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies cross-theme resource parity for platform UI theme dictionaries.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class ThemeParityTests
{
    /// <summary>Provides the Avalonia UI project directory name.</summary>
    private const string AvaloniaUiProject = "CrissCross.Avalonia.UI";

    /// <summary>Provides the WPF UI project directory name.</summary>
    private const string WpfUiProject = "CrissCross.WPF.UI";

    /// <summary>Provides the shared resources directory name.</summary>
    private const string ResourcesDirectory = "Resources";

    /// <summary>Provides the shared theme directory name.</summary>
    private const string ThemeDirectory = "Theme";

    /// <summary>Provides the high contrast theme name.</summary>
    private const string HighContrastTheme = "HighContrast";

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

    /// <summary>Provides the x namespace used by platform resource dictionaries.</summary>
    private static readonly XNamespace XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

    /// <summary>Provides the source root used to locate platform resource dictionaries.</summary>
    private static readonly string SourceRoot = LocateSourceRoot();

    /// <summary>Verifies Avalonia exposes a dedicated high contrast theme variant.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task AvaloniaHighContrastThemeVariant_UsesDedicatedKeyAndDarkInheritance()
    {
        await Assert.That(ApplicationThemeManager.HighContrastThemeVariant.Key).IsEqualTo(HighContrastTheme);
        await Assert.That(ApplicationThemeManager.HighContrastThemeVariant.InheritVariant).IsSameReferenceAs(ThemeVariant.Dark);
    }

    /// <summary>Verifies Avalonia's fluent resource dictionary includes all supported theme dictionaries.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task AvaloniaFluentTheme_WhenLoaded_IncludesHighContrastThemeResources()
    {
        var styles = (Styles)AvaloniaXamlLoader.Load(new("avares://CrissCross.Avalonia.UI/Resources/FluentTheme.axaml"));

        await Assert.That(styles.Resources.ThemeDictionaries.ContainsKey(ThemeVariant.Dark)).IsTrue();
        await Assert.That(styles.Resources.ThemeDictionaries.ContainsKey(ThemeVariant.Light)).IsTrue();
        await Assert.That(styles.Resources.ThemeDictionaries.ContainsKey(ApplicationThemeManager.HighContrastThemeVariant)).IsTrue();

        await Assert.That(File.Exists(GetAvaloniaThemePath(HighContrastTheme))).IsTrue();
    }

    /// <summary>Verifies Avalonia high contrast keeps the Light and Dark semantic resource key contract.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task AvaloniaHighContrastTheme_KeepsLightAndDarkResourceKeysInParity()
    {
        var lightKeys = ReadResourceKeys(GetAvaloniaThemePath("Light"));
        var darkKeys = ReadResourceKeys(GetAvaloniaThemePath("Dark"));
        var highContrastKeys = ReadResourceKeys(GetAvaloniaThemePath(HighContrastTheme));

        await Assert.That(GetMissingKeys(lightKeys, highContrastKeys)).IsEmpty();
        await Assert.That(GetMissingKeys(darkKeys, highContrastKeys)).IsEmpty();
        await Assert.That(GetMissingKeys(highContrastKeys, lightKeys)).IsEmpty();
        await Assert.That(GetMissingKeys(highContrastKeys, darkKeys)).IsEmpty();
    }

    /// <summary>Verifies high contrast dictionaries keep primary text readable on their application surface.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task HighContrastThemes_KeepPrimaryTextReadableOnTheApplicationSurface()
    {
        var avaloniaColors = ReadColorResources(GetAvaloniaThemePath(HighContrastTheme));
        var wpfColors = ReadColorResources(GetWpfThemePath("HCBlack"));

        await Assert.That(GetContrast(avaloniaColors["TextFillColorPrimary"], avaloniaColors["ApplicationBackgroundColor"]))
            .IsGreaterThanOrEqualTo(PrimaryTextMinimumContrast);
        await Assert.That(GetContrast(wpfColors["SystemColorWindowTextColor"], wpfColors["ApplicationBackgroundColor"]))
            .IsGreaterThanOrEqualTo(PrimaryTextMinimumContrast);
    }

    /// <summary>Reads all keyed entries from a XAML resource dictionary.</summary>
    /// <param name="path">The resource dictionary path.</param>
    /// <returns>The resource keys.</returns>
    private static HashSet<string> ReadResourceKeys(string path)
    {
        var keys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var element in XDocument.Load(path).Descendants())
        {
            var key = GetXamlKey(element);
            if (key is not null)
            {
                _ = keys.Add(key);
            }
        }

        return keys;
    }

    /// <summary>Reads keyed literal colors from a desktop resource dictionary.</summary>
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
            if (key is not null && element.Value.StartsWith('#'))
            {
                colors.Add(key, ParseColor(element.Value));
            }
        }

        return colors;
    }

    /// <summary>Gets a resource key from the XAML namespace.</summary>
    /// <param name="element">The resource dictionary element.</param>
    /// <returns>The resource key when present.</returns>
    private static string? GetXamlKey(XElement element) => (string?)element.Attribute(XamlNamespace + "Key");

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

    /// <summary>Gets an Avalonia UI theme resource path.</summary>
    /// <param name="themeName">The theme name.</param>
    /// <returns>The theme resource file path.</returns>
    private static string GetAvaloniaThemePath(string themeName) =>
        Path.Combine(SourceRoot, AvaloniaUiProject, ResourcesDirectory, ThemeDirectory, $"{themeName}.axaml");

    /// <summary>Gets a WPF UI theme resource path.</summary>
    /// <param name="themeName">The theme name.</param>
    /// <returns>The theme resource file path.</returns>
    private static string GetWpfThemePath(string themeName) =>
        Path.Combine(SourceRoot, WpfUiProject, ResourcesDirectory, ThemeDirectory, $"{themeName}.xaml");

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
