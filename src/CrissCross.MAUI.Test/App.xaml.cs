// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using Splat;

namespace CrissCross.MAUI.Test;

/// <summary>
/// App.
/// </summary>
/// <seealso cref="Application" />
public partial class App : Application
{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    /// <remarks>
    /// To be added.
    /// </remarks>
    public App()
    {
        InitializeComponent();
        Locator.CurrentMutable.RegisterConstant<MainViewModel>(new());
        Locator.CurrentMutable.Register<IViewFor<MainViewModel>>(() => new MainView());

        Locator.CurrentMutable.RegisterConstant<FirstViewModel>(new());
        Locator.CurrentMutable.Register<IViewFor<FirstViewModel>>(() => new FirstView());
        MainPage = new AppShell();
    }
}
