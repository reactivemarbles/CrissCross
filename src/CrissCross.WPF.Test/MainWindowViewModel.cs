// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.Test.Views;
using Splat;

namespace CrissCross.WPF.Test;

/// <summary>MainWindowViewModel member.</summary>
/// <seealso cref="RxObject" />
public class MainWindowViewModel : RxObject
{
    /// <summary>Initializes a new instance of the <see cref="MainWindowViewModel"/> class.</summary>
    public MainWindowViewModel()
    {
        var mainViewModel = new MainViewModel();
        mainViewModel.InitializeCommands();
        AppLocator.CurrentMutable.RegisterConstant(mainViewModel);
        AppLocator.CurrentMutable.Register<IViewFor<MainViewModel>>(static () => new MainView());

        var firstViewModel = new FirstViewModel();
        firstViewModel.InitializeCommands();
        AppLocator.CurrentMutable.RegisterConstant(firstViewModel);
        AppLocator.CurrentMutable.Register<IViewFor<FirstViewModel>>(static () => new FirstView());

        var browserViewModel = new BrowserViewModel();
        browserViewModel.InitializeCommands();
        AppLocator.CurrentMutable.RegisterConstant(browserViewModel);
        AppLocator.CurrentMutable.Register<IViewFor<BrowserViewModel>>(static () => new BrowserView());
        AppLocator.CurrentMutable.SetupComplete();
        var s = new SecondWindow();
        s.Show();
    }
}
