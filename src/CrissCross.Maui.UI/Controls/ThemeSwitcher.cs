// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>
/// Displays and changes a shared theme preference snapshot.
/// </summary>
public class ThemeSwitcher : ContentView
{
    /// <summary>
    /// Bindable property for <see cref="ThemeState"/>.
    /// </summary>
    public static readonly BindableProperty ThemeStateProperty = BindableProperty.Create(
        nameof(ThemeState),
        typeof(ThemePreferenceState),
        typeof(ThemeSwitcher));

    /// <summary>
    /// Bindable property for <see cref="ChangeThemeCommand"/>.
    /// </summary>
    public static readonly BindableProperty ChangeThemeCommandProperty = BindableProperty.Create(
        nameof(ChangeThemeCommand),
        typeof(ICommand),
        typeof(ThemeSwitcher));

    /// <summary>
    /// Gets or sets the shared CrissCross state projected by this control.
    /// </summary>
    public ThemePreferenceState? ThemeState
    {
        get => (ThemePreferenceState?)GetValue(ThemeStateProperty);
        set => SetValue(ThemeStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked by the control surface.
    /// </summary>
    public ICommand? ChangeThemeCommand
    {
        get => (ICommand?)GetValue(ChangeThemeCommandProperty);
        set => SetValue(ChangeThemeCommandProperty, value);
    }
}
