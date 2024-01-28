// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Markup;
using Wpf.Ui.Appearance;

namespace CrissCross.WPF.UI;

/// <summary>
/// Provides a dictionary implementation that contains <c>WPF UI</c> theme resources used by components and other elements of a WPF application.
/// </summary>
[Localizability(LocalizationCategory.Ignore)]
[Ambient]
[UsableDuringInitialization(true)]
public class ThemesDictionary : ResourceDictionary
{
    private const string DictionaryUri = "pack://application:,,,/CrissCross.WPF.UI;component/Resources/Themes/";

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemesDictionary"/> class.
    /// </summary>
    public ThemesDictionary() => SetSourceBasedOnSelectedTheme(ApplicationTheme.Light);

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
            _ => "Light"
        };

        Source = new Uri($"{DictionaryUri}{themeName}.xaml", UriKind.Absolute);
    }
}
