// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Markup;

namespace CrissCross.WPF.UI.Markup;

/// <summary>
/// Provides a dictionary implementation that contains theme resources used by components and other elements of a WPF application.
/// </summary>
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
    /// <summary>
    /// Initializes a new instance of the <see cref="ThemesDictionary"/> class.
    /// </summary>
    public ThemesDictionary() => SetSourceBasedOnSelectedTheme(ApplicationTheme.Dark);

    /// <summary>
    /// Sets the default application theme.
    /// </summary>
    public ApplicationTheme Theme
    {
        set => SetSourceBasedOnSelectedTheme(value);
    }

    private void SetSourceBasedOnSelectedTheme(ApplicationTheme? selectedApplicationTheme)
    {
        var themeName = selectedApplicationTheme switch
        {
            ApplicationTheme.Dark => "Dark",
            ApplicationTheme.HighContrast => "HighContrast",
            _ => "Light"
        };

        Source = new Uri($"{ApplicationThemeManager.ThemesDictionaryPath}{themeName}.xaml", UriKind.Absolute);
    }
}
