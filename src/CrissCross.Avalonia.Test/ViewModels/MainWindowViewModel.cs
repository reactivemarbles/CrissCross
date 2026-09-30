// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.Test.Views;
using Splat;

namespace CrissCross.Avalonia.Test;

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
        AppLocator.CurrentMutable.Register<IViewFor<MainViewModel>>(static () => MainView.Create());

        var firstViewModel = new FirstViewModel();
        firstViewModel.InitializeCommands();
        AppLocator.CurrentMutable.RegisterConstant(firstViewModel);
        AppLocator.CurrentMutable.Register<IViewFor<FirstViewModel>>(static () => FirstView.Create());
        AppLocator.CurrentMutable.SetupComplete();
    }
}
