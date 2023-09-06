// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CrissCross.MAUI.Test.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App() => InitializeComponent();

    /// <summary>
    /// Creates the maui application.
    /// </summary>
    /// <returns>A MauiApp.</returns>
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
