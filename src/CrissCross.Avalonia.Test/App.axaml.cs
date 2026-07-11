// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CrissCross.Avalonia.Test.Views;

namespace CrissCross.Avalonia.Test;

/// <summary>App member.</summary>
/// <seealso cref="Application" />
public class App : Application
{
    /// <summary>Initializes the application by loading XAML etc.</summary>
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    /// <summary>Called when [framework initialization completed].</summary>
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime activity)
        {
            activity.MainViewFactory = () => new MainUserControl();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainUserControl();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
