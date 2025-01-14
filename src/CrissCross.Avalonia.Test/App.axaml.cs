// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CrissCross.Avalonia.Test.Views;

namespace CrissCross.Avalonia.Test;

/// <summary>
/// App.
/// </summary>
/// <seealso cref="Application" />
public partial class App : Application
{
    /// <summary>
    /// Initializes the application by loading XAML etc.
    /// </summary>
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    /// <summary>
    /// Called when [framework initialization completed].
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainUserControl();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
