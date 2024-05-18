// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Window = System.Windows.Window;

namespace CrissCross.WPF.UI;

/// <summary>
/// Represents a UI application.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UiApplication" /> class.
/// </remarks>
/// <param name="application">The application.</param>
public class UiApplication(Application application)
{
    private static UiApplication? _uiApplication;
    private ResourceDictionary? _resources;

    private Window? _mainWindow;

    /// <summary>
    /// Gets the current application.
    /// </summary>
    public static UiApplication Current => GetUiApplication();

    /// <summary>
    /// Gets a value indicating whether the application is running outside of the desktop app context.
    /// </summary>
    public bool IsApplication => application is not null;

    /// <summary>
    /// Gets or sets the application's main window.
    /// </summary>
    public Window? MainWindow
    {
        get => application?.MainWindow ?? _mainWindow;
        set
        {
            if (application is not null)
            {
                application.MainWindow = value;
            }

            _mainWindow = value;
        }
    }

    /// <summary>
    /// Gets or sets the application's resources.
    /// </summary>
    public ResourceDictionary Resources
    {
        get
        {
            if (_resources is null)
            {
                _resources = [];

                try
                {
                    ApplicationAccentColorManager.ApplySystemAccent();
                    var themesDictionary = new Markup.ThemesDictionary();
                    var controlsDictionary = new Markup.ControlsDictionary();
                    _resources.MergedDictionaries.Add(themesDictionary);
                    _resources.MergedDictionaries.Add(controlsDictionary);
                }
                catch
                {
                }
            }

            return application?.Resources ?? _resources;
        }

        set
        {
            if (application is not null)
            {
                application.Resources = value;
            }

            _resources = value;
        }
    }

    /// <summary>
    /// Gets or sets the application's main window.
    /// </summary>
    /// <param name="resourceKey">The resource key.</param>
    /// <returns>An object representing the resource.</returns>
    public object TryFindResource(object resourceKey) => Resources[resourceKey];

    /// <summary>
    /// Turns the application's into shutdown mode.
    /// </summary>
    public void Shutdown() => application?.Shutdown();

    private static UiApplication GetUiApplication() =>
        _uiApplication ??= new UiApplication(Application.Current);
}
