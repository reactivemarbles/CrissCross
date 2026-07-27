// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Gallery.WinUI;

/// <summary>Provides the Windows application entry point.</summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>Initializes a new instance of the <see cref="App"/> class.</summary>
    public App() => InitializeComponent();

    /// <inheritdoc />
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
