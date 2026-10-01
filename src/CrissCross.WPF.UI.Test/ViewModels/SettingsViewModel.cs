// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.WPF.UI.Test.ViewModels;

/// <summary>SettingsViewModel member.</summary>
/// <seealso cref="RxObject" />
/// <seealso cref="INavigationAware" />
public class SettingsViewModel : RxObject, INavigationAware
{
    /// <summary>Tracks whether application settings have been loaded.</summary>
    private bool _isInitialized;

    /// <summary>Initializes a new instance of the <see cref="SettingsViewModel"/> class.</summary>
    public SettingsViewModel() => ChangeThemeCommand = ReactiveCommand.Create<string>(OnChangeTheme);

    /// <summary>Gets the change theme command.</summary>
    /// <value>
    /// The change theme command.
    /// </value>
    public ReactiveCommand<string, Unit> ChangeThemeCommand { get; }

    /// <summary>Gets or sets the application version.</summary>
    /// <value>
    /// The application version.
    /// </value>
    public string AppVersion
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets or sets the current application theme.</summary>
    /// <value>
    /// The current application theme.
    /// </value>
    public CrissCross.WPF.UI.Appearance.ApplicationTheme CurrentApplicationTheme
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = CrissCross.WPF.UI.Appearance.ApplicationTheme.Unknown;

    /// <summary>Method triggered when the class is navigated.</summary>
    public void OnNavigatedTo()
    {
        if (_isInitialized)
        {
            return;
        }

        InitializeViewModel();
    }

    /// <summary>Method triggered when the navigation leaves the current class.</summary>
    public void OnNavigatedFrom() { }

    /// <summary>Gets the executing assembly version.</summary>
    /// <returns>The formatted assembly version.</returns>
    private static string GetAssemblyVersion() =>
        typeof(SettingsViewModel).Assembly.GetName().Version?.ToString() ?? string.Empty;

    /// <summary>Loads the current application settings.</summary>
    private void InitializeViewModel()
    {
        CurrentApplicationTheme = CrissCross.WPF.UI.Appearance.ApplicationThemeManager.GetAppTheme();
        AppVersion = $"CrissCross.WPF.UI.Test - {GetAssemblyVersion()}";

        _isInitialized = true;
    }

    /// <summary>Applies the requested application theme.</summary>
    /// <param name="parameter">The requested theme key.</param>
    private void OnChangeTheme(string parameter)
    {
        var theme = parameter == "theme_light"
            ? Appearance.ApplicationTheme.Light
            : Appearance.ApplicationTheme.Dark;
        if (CurrentApplicationTheme == theme)
        {
            return;
        }

        Appearance.ApplicationThemeManager.Apply(theme);
        CurrentApplicationTheme = theme;
    }
}
