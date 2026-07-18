// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Win32;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Appearance;
#else
namespace CrissCross.WPF.UI.Appearance;
#endif

/// <summary>Provides information about Windows system themes.</summary>
/// <example>
/// <code lang="csharp">
/// var currentWindowTheme = SystemThemeManager.GetCachedSystemTheme();
/// </code>
/// <code lang="csharp">
/// SystemThemeManager.UpdateSystemThemeCache();
/// var currentWindowTheme = SystemThemeManager.GetCachedSystemTheme();
/// </code>
/// </example>
public static class SystemThemeManager
{
    /// <summary>Maps Windows theme file names to system themes.</summary>
    private static readonly (string ThemeFile, SystemTheme Theme)[] _themeFileMappings =
    [
        ("basic.theme", SystemTheme.Light),
        ("aero.theme", SystemTheme.Light),
        ("dark.theme", SystemTheme.Dark),
        ("hcblack.theme", SystemTheme.HCBlack),
        ("hcwhite.theme", SystemTheme.HCWhite),
        ("hc1.theme", SystemTheme.HC1),
        ("hc2.theme", SystemTheme.HC2),
        ("themea.theme", SystemTheme.Glow),
        ("themeb.theme", SystemTheme.CapturedMotion),
        ("themec.theme", SystemTheme.Sunrise),
        ("themed.theme", SystemTheme.Flow),];

    /// <summary>Stores the _cachedTheme value.</summary>
    private static SystemTheme _cachedTheme = SystemTheme.Unknown;

    /// <summary>Gets the Windows glass color.</summary>
    public static Color GlassColor => SystemParameters.WindowGlassColor;

    /// <summary>Gets a value indicating whether the system is currently using the high contrast theme.</summary>
    public static bool HighContrast => SystemParameters.HighContrast;

    /// <summary>Returns the Windows theme retrieved from the registry. If it has not been cached before, invokes the
    /// <see cref="UpdateSystemThemeCache"/> and then returns the currently obtained theme.</summary>
    /// <returns>Currently cached Windows theme.</returns>
    public static SystemTheme GetCachedSystemTheme()
    {
        if (_cachedTheme != SystemTheme.Unknown)
        {
            return _cachedTheme;
        }

        UpdateSystemThemeCache();

        return _cachedTheme;
    }

    /// <summary>Refreshes the currently saved system theme.</summary>
    public static void UpdateSystemThemeCache() => _cachedTheme = GetCurrentSystemTheme();

    /// <summary>Provides the GetCurrentSystemTheme member.</summary>
    /// <returns>The result.</returns>
    private static SystemTheme GetCurrentSystemTheme()
    {
        var currentTheme =
            (
                Registry.GetValue(
                    "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes",
                    "CurrentTheme",
                    "aero.theme") as string) ?? string.Empty;

        if (!string.IsNullOrEmpty(currentTheme))
        {
            var mappedTheme = GetThemeFromPath(currentTheme);
            if (mappedTheme != SystemTheme.Unknown)
            {
                return mappedTheme;
            }
        }

        return GetThemeFromPersonalization();
    }

    /// <summary>Gets a system theme from a Windows theme path.</summary>
    /// <param name="currentTheme">The current theme path.</param>
    /// <returns>The mapped system theme, or <see cref="SystemTheme.Unknown"/>.</returns>
    private static SystemTheme GetThemeFromPath(string currentTheme)
    {
        var normalizedTheme = currentTheme.ToLower().Trim();
        var mapping = _themeFileMappings.FirstOrDefault(item => normalizedTheme.Contains(item.ThemeFile));
        return mapping.Theme;
    }

    /// <summary>Gets a system theme from personalization registry values.</summary>
    /// <returns>The system theme.</returns>
    private static SystemTheme GetThemeFromPersonalization()
    {
        var rawAppsUseLightTheme = Registry.GetValue(
            "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
            "AppsUseLightTheme",
            1);

        if (rawAppsUseLightTheme is 0 or 1)
        {
            return rawAppsUseLightTheme is 0 ? SystemTheme.Dark : SystemTheme.Light;
        }

        var rawSystemUsesLightTheme =
            Registry.GetValue(
                "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                "SystemUsesLightTheme",
                1) ?? 1;

        return rawSystemUsesLightTheme is 0 ? SystemTheme.Dark : SystemTheme.Light;
    }
}
