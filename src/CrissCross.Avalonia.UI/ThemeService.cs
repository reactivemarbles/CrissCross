// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Media;
using CrissCross.Avalonia.UI.Appearance;

namespace CrissCross.Avalonia.UI;

/// <summary>Lets you set the Avalonia application theme.</summary>
public class ThemeService : IThemeService
{
    /// <inheritdoc />
    public virtual ApplicationTheme GetTheme() => ApplicationThemeManager.GetAppTheme();

    /// <inheritdoc />
    public virtual SystemTheme GetNativeSystemTheme() => SystemThemeManager.GetSystemTheme();

    /// <inheritdoc />
    public virtual ApplicationTheme GetSystemTheme()
    {
        var systemTheme = SystemThemeManager.GetSystemTheme();

        return systemTheme switch
        {
            SystemTheme.Light => ApplicationTheme.Light,
            SystemTheme.Dark => ApplicationTheme.Dark,
            SystemTheme.Glow => ApplicationTheme.Dark,
            SystemTheme.CapturedMotion => ApplicationTheme.Dark,
            SystemTheme.Sunrise => ApplicationTheme.Light,
            SystemTheme.Flow => ApplicationTheme.Light,
            SystemTheme.HCBlack => ApplicationTheme.HighContrast,
            SystemTheme.HC1 => ApplicationTheme.HighContrast,
            SystemTheme.HC2 => ApplicationTheme.HighContrast,
            SystemTheme.HCWhite => ApplicationTheme.HighContrast,
            _ => ApplicationTheme.Unknown
        };
    }

    /// <inheritdoc />
    public virtual bool SetTheme(ApplicationTheme applicationTheme)
    {
        if (ApplicationThemeManager.GetAppTheme() == applicationTheme)
        {
            return false;
        }

        ApplicationThemeManager.Apply(applicationTheme);

        return true;
    }

    /// <inheritdoc />
    public bool SetSystemAccent() => false;

    /// <inheritdoc />
    public bool SetAccent(Color accentColor)
    {
        ApplicationThemeManager.SetAccentColor(accentColor);

        return true;
    }

    /// <inheritdoc />
    public bool SetAccent(SolidColorBrush accentSolidBrush)
    {
        if (accentSolidBrush is null)
        {
            throw new ArgumentNullException(nameof(accentSolidBrush));
        }

        ApplicationThemeManager.SetAccentColor(accentSolidBrush.Color);

        return true;
    }
}
