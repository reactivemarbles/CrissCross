// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Extensions;

namespace CrissCross.WPF.UI.Appearance;

/// <summary>Allows updating the accents used by controls in the application by swapping dynamic resources.</summary>
/// <example>
/// <code lang="csharp">
/// ApplicationAccentColorManager.Apply(
///     Color.FromArgb(0xFF, 0xEE, 0x00, 0xBB),
///     ApplicationTheme.Dark,
///     false
/// );
/// </code>
/// <code lang="csharp">
/// ApplicationAccentColorManager.Apply(
///     ApplicationAccentColorManager.GetColorizationColor(),
///     ApplicationTheme.Dark,
///     false
/// );
/// </code>
/// </example>
public static class ApplicationAccentColorManager
{
    /// <summary>Provides the BackgroundBrightnessThresholdValue member.</summary>
    private const double BackgroundBrightnessThresholdValue = 80D;

    /// <summary>Provides the brightness correction applied to the system glass color.</summary>
    private const float SystemGlassBrightnessAdjustment = 6F;

    /// <summary>Provides the dark-theme primary accent brightness adjustment.</summary>
    private const float DarkPrimaryBrightnessAdjustment = 15F;

    /// <summary>Provides the dark-theme primary accent saturation adjustment.</summary>
    private const float DarkPrimarySaturationAdjustment = -12F;

    /// <summary>Provides the dark-theme secondary accent brightness adjustment.</summary>
    private const float DarkSecondaryBrightnessAdjustment = 30F;

    /// <summary>Provides the dark-theme secondary accent saturation adjustment.</summary>
    private const float DarkSecondarySaturationAdjustment = -24F;

    /// <summary>Provides the dark-theme tertiary accent brightness adjustment.</summary>
    private const float DarkTertiaryBrightnessAdjustment = 45F;

    /// <summary>Provides the dark-theme tertiary accent saturation adjustment.</summary>
    private const float DarkTertiarySaturationAdjustment = -36F;

    /// <summary>Provides the light-theme primary accent brightness adjustment.</summary>
    private const float LightPrimaryBrightnessAdjustment = -5F;

    /// <summary>Provides the light-theme secondary accent brightness adjustment.</summary>
    private const float LightSecondaryBrightnessAdjustment = -10F;

    /// <summary>Provides the light-theme tertiary accent brightness adjustment.</summary>
    private const float LightTertiaryBrightnessAdjustment = -15F;

    /// <summary>Provides the secondary accent brush opacity.</summary>
    private const double SecondaryAccentBrushOpacity = 0.9D;

    /// <summary>Provides the tertiary accent brush opacity.</summary>
    private const double TertiaryAccentBrushOpacity = 0.8D;

    /// <summary>Gets the SystemAccentColor.</summary>
    public static Color SystemAccent
    {
        get
        {
            var resource = UiApplication.Current.Resources["SystemAccentColor"];

            return resource is Color color ? color : Colors.Transparent;
        }
    }

    /// <summary>Gets the <see cref="Brush"/> of the SystemAccentColor.</summary>
    public static Brush SystemAccentBrush => new SolidColorBrush(SystemAccent);

    /// <summary>Gets the SystemAccentColorPrimary.</summary>
    public static Color PrimaryAccent
    {
        get
        {
            var resource = UiApplication.Current.Resources["SystemAccentColorPrimary"];

            return resource is Color color ? color : Colors.Transparent;
        }
    }

    /// <summary>Gets the <see cref="Brush"/> of the SystemAccentColorPrimary.</summary>
    public static Brush PrimaryAccentBrush => new SolidColorBrush(PrimaryAccent);

    /// <summary>Gets the SystemAccentColorSecondary.</summary>
    public static Color SecondaryAccent
    {
        get
        {
            var resource = UiApplication.Current.Resources["SystemAccentColorSecondary"];

            return resource is Color color ? color : Colors.Transparent;
        }
    }

    /// <summary>Gets the <see cref="Brush"/> of the SystemAccentColorSecondary.</summary>
    public static Brush SecondaryAccentBrush => new SolidColorBrush(SecondaryAccent);

    /// <summary>Gets the SystemAccentColorTertiary.</summary>
    public static Color TertiaryAccent
    {
        get
        {
            var resource = UiApplication.Current.Resources["SystemAccentColorTertiary"];

            return resource is Color color ? color : Colors.Transparent;
        }
    }

    /// <summary>Gets the <see cref="Brush"/> of the SystemAccentColorTertiary.</summary>
    public static Brush TertiaryAccentBrush => new SolidColorBrush(TertiaryAccent);

    /// <summary>Changes the color accents of the application based on the color entered.</summary>
    /// <param name="systemAccent">Primary accent color.</param>
    public static void Apply(Color systemAccent) => Apply(systemAccent, ApplicationTheme.Light, false);

    /// <summary>Applies an accent color for the specified application theme.</summary>
    /// <param name="systemAccent">Primary accent color.</param>
    /// <param name="applicationTheme">The application theme.</param>
    public static void Apply(Color systemAccent, ApplicationTheme applicationTheme) =>
        Apply(systemAccent, applicationTheme, false);

    /// <summary>Applies an accent color for the specified application theme.</summary>
    /// <param name="systemAccent">Primary accent color.</param>
    /// <param name="applicationTheme">If <see cref="ApplicationTheme.Dark"/>, the colors will be different.</param>
    /// <param name="systemGlassColor">If the color comes from the system glass color.</param>
    public static void Apply(Color systemAccent, ApplicationTheme applicationTheme, bool systemGlassColor)
    {
        if (systemGlassColor)
        {
            // WindowGlassColor is little darker than accent color
            systemAccent = systemAccent.UpdateBrightness(SystemGlassBrightnessAdjustment);
        }

        Color primaryAccent;
        Color secondaryAccent;
        Color tertiaryAccent;

        if (applicationTheme == ApplicationTheme.Dark)
        {
            primaryAccent = systemAccent.Update(DarkPrimaryBrightnessAdjustment, DarkPrimarySaturationAdjustment);
            secondaryAccent = systemAccent.Update(DarkSecondaryBrightnessAdjustment, DarkSecondarySaturationAdjustment);
            tertiaryAccent = systemAccent.Update(DarkTertiaryBrightnessAdjustment, DarkTertiarySaturationAdjustment);
        }
        else
        {
            primaryAccent = systemAccent.UpdateBrightness(LightPrimaryBrightnessAdjustment);
            secondaryAccent = systemAccent.UpdateBrightness(LightSecondaryBrightnessAdjustment);
            tertiaryAccent = systemAccent.UpdateBrightness(LightTertiaryBrightnessAdjustment);
        }

        UpdateColorResources(systemAccent, primaryAccent, secondaryAccent, tertiaryAccent);
    }

    /// <summary>Changes the color accents of the application based on the entered colors.</summary>
    /// <param name="systemAccent">Primary color.</param>
    /// <param name="primaryAccent">Alternative light or dark color.</param>
    /// <param name="secondaryAccent">Second alternative light or dark color (most used).</param>
    /// <param name="tertiaryAccent">Third alternative light or dark color.</param>
    public static void Apply(Color systemAccent, Color primaryAccent, Color secondaryAccent, Color tertiaryAccent) =>
        UpdateColorResources(systemAccent, primaryAccent, secondaryAccent, tertiaryAccent);

    /// <summary>Applies system accent color to the application.</summary>
    public static void ApplySystemAccent() => Apply(GetColorizationColor(), ApplicationThemeManager.GetAppTheme());

    /// <summary>Gets current Desktop Window Manager colorization color.</summary>
    /// <returns>A color.</returns>
    public static Color GetColorizationColor() => UnsafeNativeMethods.GetDwmColor();

    /// <summary>Updates application resources.</summary>
    /// <param name="systemAccent">The systemAccent value.</param>
    /// <param name="primaryAccent">The primaryAccent value.</param>
    /// <param name="secondaryAccent">The secondaryAccent value.</param>
    /// <param name="tertiaryAccent">The tertiaryAccent value.</param>
    private static void UpdateColorResources(
        Color systemAccent,
        Color primaryAccent,
        Color secondaryAccent,
        Color tertiaryAccent)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine("INFO | SystemAccentColor: " + systemAccent, "CrissCross.WPF.UI.Accent");
        System.Diagnostics.Debug.WriteLine(
            "INFO | SystemAccentColorPrimary: " + primaryAccent,
            "CrissCross.WPF.UI.Accent");
        System.Diagnostics.Debug.WriteLine(
            "INFO | SystemAccentColorSecondary: " + secondaryAccent,
            "CrissCross.WPF.UI.Accent");
        System.Diagnostics.Debug.WriteLine(
            "INFO | SystemAccentColorTertiary: " + tertiaryAccent,
            "CrissCross.WPF.UI.Accent");
#endif

        if (secondaryAccent.GetBrightness() > BackgroundBrightnessThresholdValue)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("INFO | Text on accent is DARK", "CrissCross.WPF.UI.Accent");
#endif
            UiApplication.Current.Resources["TextOnAccentFillColorPrimary"] = Color.FromArgb(0xFF, 0x00, 0x00, 0x00);
            UiApplication.Current.Resources["TextOnAccentFillColorSecondary"] = Color.FromArgb(0x80, 0x00, 0x00, 0x00);
            UiApplication.Current.Resources["TextOnAccentFillColorDisabled"] = Color.FromArgb(0x77, 0x00, 0x00, 0x00);
            UiApplication.Current.Resources["TextOnAccentFillColorSelectedText"] = Color.FromArgb(
                0x00,
                0x00,
                0x00,
                0x00);
            UiApplication.Current.Resources["AccentTextFillColorDisabled"] = Color.FromArgb(0x5D, 0x00, 0x00, 0x00);
        }
        else
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("INFO | Text on accent is LIGHT", "CrissCross.WPF.UI.Accent");
#endif
            UiApplication.Current.Resources["TextOnAccentFillColorPrimary"] = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            UiApplication.Current.Resources["TextOnAccentFillColorSecondary"] = Color.FromArgb(0x80, 0xFF, 0xFF, 0xFF);
            UiApplication.Current.Resources["TextOnAccentFillColorDisabled"] = Color.FromArgb(0x87, 0xFF, 0xFF, 0xFF);
            UiApplication.Current.Resources["TextOnAccentFillColorSelectedText"] = Color.FromArgb(
                0xFF,
                0xFF,
                0xFF,
                0xFF);
            UiApplication.Current.Resources["AccentTextFillColorDisabled"] = Color.FromArgb(0x5D, 0xFF, 0xFF, 0xFF);
        }

        UiApplication.Current.Resources["SystemAccentColor"] = systemAccent;
        UiApplication.Current.Resources["SystemAccentColorPrimary"] = primaryAccent;
        UiApplication.Current.Resources["SystemAccentColorSecondary"] = secondaryAccent;
        UiApplication.Current.Resources["SystemAccentColorTertiary"] = tertiaryAccent;

        UiApplication.Current.Resources["SystemAccentBrush"] = secondaryAccent.ToBrush();
        UiApplication.Current.Resources["SystemFillColorAttentionBrush"] = secondaryAccent.ToBrush();
        UiApplication.Current.Resources["AccentTextFillColorPrimaryBrush"] = tertiaryAccent.ToBrush();
        UiApplication.Current.Resources["AccentTextFillColorSecondaryBrush"] = tertiaryAccent.ToBrush();
        UiApplication.Current.Resources["AccentTextFillColorTertiaryBrush"] = secondaryAccent.ToBrush();
        UiApplication.Current.Resources["AccentFillColorSelectedTextBackgroundBrush"] = systemAccent.ToBrush();
        UiApplication.Current.Resources["AccentFillColorDefaultBrush"] = secondaryAccent.ToBrush();

        UiApplication.Current.Resources["AccentFillColorSecondaryBrush"] = secondaryAccent.ToBrush(
            SecondaryAccentBrushOpacity);
        UiApplication.Current.Resources["AccentFillColorTertiaryBrush"] = secondaryAccent.ToBrush(
            TertiaryAccentBrushOpacity);
    }
}
