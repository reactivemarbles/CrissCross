// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace CrissCross.Avalonia.UI.Appearance;

/// <summary>
/// Provides methods for detecting the system theme.
/// </summary>
public static class SystemThemeManager
{
    private static SystemTheme _cachedSystemTheme = SystemTheme.Unknown;

    /// <summary>
    /// Gets the current system theme.
    /// </summary>
    /// <returns>The current <see cref="SystemTheme"/>.</returns>
    public static SystemTheme GetSystemTheme()
    {
        if (_cachedSystemTheme == SystemTheme.Unknown)
        {
            UpdateSystemThemeCache();
        }

        return _cachedSystemTheme;
    }

    /// <summary>
    /// Updates the cached system theme.
    /// </summary>
    public static void UpdateSystemThemeCache()
    {
        _cachedSystemTheme = DetectSystemTheme();
    }

    /// <summary>
    /// Gets a value indicating whether the system is using high contrast.
    /// </summary>
    /// <returns><see langword="true"/> if high contrast is enabled.</returns>
    public static bool IsHighContrast()
    {
        var theme = GetSystemTheme();
        return theme is SystemTheme.HC1 or SystemTheme.HC2 or SystemTheme.HCBlack or SystemTheme.HCWhite;
    }

    /// <summary>
    /// Gets a value indicating whether the system is using a dark theme.
    /// </summary>
    /// <returns><see langword="true"/> if using dark theme.</returns>
    public static bool IsDark()
    {
        var theme = GetSystemTheme();
        return theme is SystemTheme.Dark or SystemTheme.CapturedMotion or SystemTheme.Glow;
    }

    /// <summary>
    /// Gets a value indicating whether the system is using a light theme.
    /// </summary>
    /// <returns><see langword="true"/> if using light theme.</returns>
    public static bool IsLight()
    {
        var theme = GetSystemTheme();
        return theme is SystemTheme.Light or SystemTheme.Flow or SystemTheme.Sunrise;
    }

    private static SystemTheme DetectSystemTheme()
    {
        // Cross-platform detection
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return DetectWindowsTheme();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return DetectMacOSTheme();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return DetectLinuxTheme();
        }

        return SystemTheme.Light;
    }

    private static SystemTheme DetectWindowsTheme()
    {
#if WINDOWS
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key?.GetValue("AppsUseLightTheme") is int useLightTheme)
            {
                return useLightTheme == 0 ? SystemTheme.Dark : SystemTheme.Light;
            }
        }
        catch
        {
            // Registry access might fail on some systems
        }
#endif
        return SystemTheme.Light;
    }

    private static SystemTheme DetectMacOSTheme()
    {
        // On macOS, we'd typically use NSAppearance
        // For now, default to Light
        return SystemTheme.Light;
    }

    private static SystemTheme DetectLinuxTheme()
    {
        // On Linux, theme detection varies by desktop environment
        // For now, default to Light
        try
        {
            var gtkTheme = Environment.GetEnvironmentVariable("GTK_THEME");
            if (!string.IsNullOrEmpty(gtkTheme) && gtkTheme.Contains("dark", StringComparison.OrdinalIgnoreCase))
            {
                return SystemTheme.Dark;
            }
        }
        catch
        {
            // Environment variable access might fail
        }

        return SystemTheme.Light;
    }
}
