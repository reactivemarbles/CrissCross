// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;

namespace CrissCross.Avalonia.UI.Appearance;

/// <summary>
/// Allows to manage the application theme.
/// </summary>
public static class ApplicationThemeManager
{
    private static ApplicationTheme _cachedApplicationTheme = ApplicationTheme.Unknown;
    private static Color _accentColor = Colors.CornflowerBlue;

    /// <summary>
    /// Event triggered when the application's theme is changed.
    /// </summary>
    public static event ThemeChangedEvent? Changed;

    /// <summary>
    /// Gets a value that indicates whether the application is currently using the high contrast theme.
    /// </summary>
    /// <returns><see langword="true"/> if application uses high contrast theme.</returns>
    public static bool IsHighContrast() => _cachedApplicationTheme == ApplicationTheme.HighContrast;

    /// <summary>
    /// Gets the current application theme.
    /// </summary>
    /// <returns>The current <see cref="ApplicationTheme"/>.</returns>
    public static ApplicationTheme GetAppTheme()
    {
        if (_cachedApplicationTheme == ApplicationTheme.Unknown)
        {
            FetchApplicationTheme();
        }

        return _cachedApplicationTheme;
    }

    /// <summary>
    /// Gets the current accent color.
    /// </summary>
    /// <returns>The current accent <see cref="Color"/>.</returns>
    public static Color GetAccentColor() => _accentColor;

    /// <summary>
    /// Changes the current application theme.
    /// </summary>
    /// <param name="applicationTheme">Theme to set.</param>
    /// <param name="accentColor">Optional accent color.</param>
    public static void Apply(ApplicationTheme applicationTheme, Color? accentColor = null)
    {
        if (applicationTheme == ApplicationTheme.Unknown)
        {
            return;
        }

        _cachedApplicationTheme = applicationTheme;

        if (accentColor.HasValue)
        {
            _accentColor = accentColor.Value;
        }

        // Set the theme variant in Avalonia
        if (Application.Current is not null)
        {
            Application.Current.RequestedThemeVariant = applicationTheme switch
            {
                ApplicationTheme.Dark => ThemeVariant.Dark,
                ApplicationTheme.Light => ThemeVariant.Light,
                _ => ThemeVariant.Default
            };
        }

        Changed?.Invoke(_cachedApplicationTheme, _accentColor);
    }

    /// <summary>
    /// Applies the system theme to the application.
    /// </summary>
    public static void ApplySystemTheme()
    {
        var systemTheme = SystemThemeManager.GetSystemTheme();

        var themeToSet = systemTheme switch
        {
            SystemTheme.Dark or SystemTheme.CapturedMotion or SystemTheme.Glow => ApplicationTheme.Dark,
            SystemTheme.HC1 or SystemTheme.HC2 or SystemTheme.HCBlack or SystemTheme.HCWhite => ApplicationTheme.HighContrast,
            _ => ApplicationTheme.Light
        };

        Apply(themeToSet);
    }

    /// <summary>
    /// Gets a value that indicates whether the application is matching the system theme.
    /// </summary>
    /// <returns><see langword="true"/> if the application has the same theme as the system.</returns>
    public static bool IsAppMatchesSystem()
    {
        var appTheme = GetAppTheme();
        var sysTheme = SystemThemeManager.GetSystemTheme();

        return appTheme switch
        {
            ApplicationTheme.Dark => sysTheme is SystemTheme.Dark or SystemTheme.CapturedMotion or SystemTheme.Glow,
            ApplicationTheme.Light => sysTheme is SystemTheme.Light or SystemTheme.Flow or SystemTheme.Sunrise,
            ApplicationTheme.HighContrast => sysTheme is SystemTheme.HC1 or SystemTheme.HC2 or SystemTheme.HCBlack or SystemTheme.HCWhite,
            _ => false
        };
    }

    /// <summary>
    /// Checks if the application is currently using a dark theme.
    /// </summary>
    /// <returns><see langword="true"/> if using dark theme.</returns>
    public static bool IsDark() => _cachedApplicationTheme == ApplicationTheme.Dark;

    /// <summary>
    /// Checks if the application is currently using a light theme.
    /// </summary>
    /// <returns><see langword="true"/> if using light theme.</returns>
    public static bool IsLight() => _cachedApplicationTheme == ApplicationTheme.Light;

    /// <summary>
    /// Sets the accent color.
    /// </summary>
    /// <param name="color">The accent color to set.</param>
    public static void SetAccentColor(Color color)
    {
        _accentColor = color;
        Changed?.Invoke(_cachedApplicationTheme, _accentColor);
    }

    private static void FetchApplicationTheme()
    {
        if (Application.Current is null)
        {
            _cachedApplicationTheme = ApplicationTheme.Light;
            return;
        }

        var actualTheme = Application.Current.ActualThemeVariant;
        _cachedApplicationTheme = actualTheme == ThemeVariant.Dark
            ? ApplicationTheme.Dark
            : ApplicationTheme.Light;
    }
}
