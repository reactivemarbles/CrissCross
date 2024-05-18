// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>
/// Lets you set the app theme.
/// </summary>
public class ThemeService : IThemeService
{
    /// <inheritdoc />
    public virtual ApplicationTheme GetTheme() => ApplicationThemeManager.GetAppTheme();

    /// <inheritdoc />
    public virtual SystemTheme GetNativeSystemTheme() => ApplicationThemeManager.GetSystemTheme();

    /// <inheritdoc />
    public virtual ApplicationTheme GetSystemTheme()
    {
        var systemTheme = ApplicationThemeManager.GetSystemTheme();

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
    public bool SetSystemAccent()
    {
        ApplicationAccentColorManager.ApplySystemAccent();

        return true;
    }

    /// <inheritdoc />
    public bool SetAccent(Color accentColor)
    {
        ApplicationAccentColorManager.Apply(accentColor);

        return true;
    }

    /// <inheritdoc />
    public bool SetAccent(SolidColorBrush accentSolidBrush)
    {
        if (accentSolidBrush == null)
        {
            throw new ArgumentNullException(nameof(accentSolidBrush));
        }

        var color = accentSolidBrush.Color;
        color.A = (byte)Math.Round(accentSolidBrush.Opacity * byte.MaxValue);

        ApplicationAccentColorManager.Apply(color);

        return true;
    }
}
