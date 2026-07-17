// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Window = System.Windows.Window;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Represents a UI application.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UiApplication" /> class.
/// </remarks>
public class UiApplication
{
    /// <summary>Stores the _uiApplication value.</summary>
    [ThreadStatic]
    private static UiApplication? _uiApplication;

    /// <summary>Stores the _application value.</summary>
    private readonly Application? _application;

    /// <summary>Stores the _mainWindow value.</summary>
    private Window? _mainWindow;

    /// <summary>Initializes a new instance of the <see cref="UiApplication" /> class.</summary>
    /// <param name="application">The application.</param>
    public UiApplication(Application application)
    {
        if (application is null)
        {
            return;
        }

        if (!ApplicationHasResources(application))
        {
            return;
        }

        _application = application;

        System.Diagnostics.Debug.WriteLine(
            $"INFO | {typeof(UiApplication)} application is {_application}",
            "CrissCross.WPF.UI");
    }

    /// <summary>Gets the current application.</summary>
    public static UiApplication Current
    {
        get
        {
            _uiApplication ??= new UiApplication(Application.Current);

            return _uiApplication;
        }
    }

    /// <summary>Gets whether the application is running outside of the desktop app context.</summary>
    public bool IsApplication => _application is not null;

    /// <summary>Gets or sets the application's main window.</summary>
    public Window? MainWindow
    {
        get => _application?.MainWindow ?? _mainWindow;
        set
        {
            _application?.MainWindow = value;

            _mainWindow = value;
        }
    }

    /// <summary>Gets the application's resources.</summary>
    public ResourceDictionary Resources
    {
        get
        {
            if (field is null)
            {
                field = [];

                try
                {
                    ApplicationAccentColorManager.ApplySystemAccent();
                    var themesDictionary = new Markup.ThemesDictionary();
                    var controlsDictionary = new Markup.ControlsDictionary();
                    field.MergedDictionaries.Add(themesDictionary);
                    field.MergedDictionaries.Add(controlsDictionary);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }
            }

            return _application?.Resources ?? field;
        }
    }

    /// <summary>Gets or sets the application's main window.</summary>
    /// <param name="resourceKey">The resource key.</param>
    /// <returns>An object representing the resource.</returns>
    public object TryFindResource(object resourceKey) => Resources[resourceKey];

    /// <summary>Turns the application's into shutdown mode.</summary>
    public void Shutdown() => _application?.Shutdown();

    /// <summary>Provides the ApplicationHasResources member.</summary>
    /// <param name="application">The application value.</param>
    /// <returns>The result.</returns>
    private static bool ApplicationHasResources(Application application) =>
        application.Resources.MergedDictionaries.Any(e =>
            e.Source?.ToString()
                .Contains(Appearance.ApplicationThemeManager.LibraryNamespace, StringComparison.OrdinalIgnoreCase)
            == true);
}
