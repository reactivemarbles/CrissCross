// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using ReactiveUI;

namespace CrissCross.WPF.UI.Test.ViewModels;

/// <summary>
/// SettingsViewModel.
/// </summary>
/// <seealso cref="RxObject" />
/// <seealso cref="INavigationAware" />
public class SettingsViewModel : RxObject, INavigationAware
{
    private bool _isInitialized;
    private string _appVersion = string.Empty;
    private CrissCross.WPF.UI.Appearance.ApplicationTheme _currentApplicationTheme = CrissCross.WPF.UI
        .Appearance
        .ApplicationTheme
        .Unknown;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    public SettingsViewModel() => ChangeThemeCommand = ReactiveCommand.Create<string>(OnChangeTheme);

    /// <summary>
    /// Gets the change theme command.
    /// </summary>
    /// <value>
    /// The change theme command.
    /// </value>
    public ReactiveCommand<string, Unit> ChangeThemeCommand { get; }

    /// <summary>
    /// Gets or sets the application version.
    /// </summary>
    /// <value>
    /// The application version.
    /// </value>
    public string AppVersion
    {
        get => _appVersion;
        set => this.RaiseAndSetIfChanged(ref _appVersion, value);
    }

    /// <summary>
    /// Gets or sets the current application theme.
    /// </summary>
    /// <value>
    /// The current application theme.
    /// </value>
    public CrissCross.WPF.UI.Appearance.ApplicationTheme CurrentApplicationTheme
    {
        get => _currentApplicationTheme;
        set => this.RaiseAndSetIfChanged(ref _currentApplicationTheme, value);
    }

    /// <summary>
    /// Method triggered when the class is navigated.
    /// </summary>
    public void OnNavigatedTo()
    {
        if (!_isInitialized)
        {
            InitializeViewModel();
        }
    }

    /// <summary>
    /// Method triggered when the navigation leaves the current class.
    /// </summary>
    public void OnNavigatedFrom()
    {
    }

    private static string GetAssemblyVersion() =>
        System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;

    private void InitializeViewModel()
    {
        CurrentApplicationTheme = CrissCross.WPF.UI.Appearance.ApplicationThemeManager.GetAppTheme();
        AppVersion = $"CrissCross.WPF.UI.Test - {GetAssemblyVersion()}";

        _isInitialized = true;
    }

    private void OnChangeTheme(string parameter)
    {
        switch (parameter)
        {
            case "theme_light":
                if (CurrentApplicationTheme == CrissCross.WPF.UI.Appearance.ApplicationTheme.Light)
                {
                    break;
                }

                CrissCross.WPF.UI.Appearance.ApplicationThemeManager.Apply(CrissCross.WPF.UI.Appearance.ApplicationTheme.Light);
                CurrentApplicationTheme = CrissCross.WPF.UI.Appearance.ApplicationTheme.Light;

                break;

            default:
                if (CurrentApplicationTheme == CrissCross.WPF.UI.Appearance.ApplicationTheme.Dark)
                {
                    break;
                }

                CrissCross.WPF.UI.Appearance.ApplicationThemeManager.Apply(CrissCross.WPF.UI.Appearance.ApplicationTheme.Dark);
                CurrentApplicationTheme = CrissCross.WPF.UI.Appearance.ApplicationTheme.Dark;

                break;
        }
    }
}
