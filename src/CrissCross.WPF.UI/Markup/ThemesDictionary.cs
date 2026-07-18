// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Markup;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Markup;
#else
namespace CrissCross.WPF.UI.Markup;
#endif

/// <summary>Provides a dictionary implementation that contains theme resources used by components and other elements of
/// a WPF application.</summary>
/// <example>
/// <code lang="xml">
/// &lt;Application
///     xmlns:ui="https://github.com/reactivemarbles/CrissCross.ui"&gt;
///     &lt;Application.Resources&gt;
///         &lt;ResourceDictionary&gt;
///             &lt;ResourceDictionary.MergedDictionaries&gt;
///                 &lt;ui:ThemesDictionary Theme = "Dark" /&gt;
///             &lt;/ResourceDictionary.MergedDictionaries&gt;
///         &lt;/ResourceDictionary&gt;
///     &lt;/Application.Resources&gt;
/// &lt;/Application&gt;
/// </code>
/// </example>
[Localizability(LocalizationCategory.Ignore)]
[Ambient]
[UsableDuringInitialization(true)]
public class ThemesDictionary : ResourceDictionary
{
    /// <summary>Initializes a new instance of the <see cref="ThemesDictionary"/> class.</summary>
    public ThemesDictionary() => SetSourceBasedOnSelectedTheme(GetSystemApplicationTheme());

    /// <summary>Gets or sets the default application theme.</summary>
    public ApplicationTheme Theme
    {
        get => GetSystemApplicationTheme();
        set => SetSourceBasedOnSelectedTheme(value);
    }

    /// <summary>Provides the GetSystemApplicationTheme member.</summary>
    /// <returns>The result.</returns>
    private static ApplicationTheme GetSystemApplicationTheme() =>
        ApplicationThemeManager.GetSystemTheme() switch
        {
            SystemTheme.Dark or SystemTheme.Glow or SystemTheme.CapturedMotion => ApplicationTheme.Dark,
            SystemTheme.HCBlack or SystemTheme.HC1 or SystemTheme.HC2 or SystemTheme.HCWhite =>
                ApplicationTheme.HighContrast,
            _ => ApplicationTheme.Light,
        };

    /// <summary>Provides the SetSourceBasedOnSelectedTheme member.</summary>
    /// <param name="selectedApplicationTheme">The selectedApplicationTheme value.</param>
    private void SetSourceBasedOnSelectedTheme(ApplicationTheme? selectedApplicationTheme)
    {
        var themeName = selectedApplicationTheme switch
        {
            ApplicationTheme.Dark => "Dark",
            ApplicationTheme.HighContrast => "HighContrast",
            _ => "Light",
        };

        Source = new($"{ApplicationThemeManager.ThemesDictionaryPath}{themeName}.xaml", UriKind.Absolute);
    }
}
