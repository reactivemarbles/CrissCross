// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.CC_Nav.Test.Views;
using ReactiveUI;
using Splat;

namespace CrissCross.WPF.UI.CC_Nav.Test;

/// <summary>
/// MainWindowViewModel.
/// </summary>
/// <seealso cref="RxObject" />
public class MainWindowViewModel : RxObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        AppLocator.CurrentMutable.RegisterConstant<MainViewModel>(new());
        AppLocator.CurrentMutable.Register<IViewFor<MainViewModel>>(() => new MainView());

        AppLocator.CurrentMutable.RegisterConstant<FirstViewModel>(new());
        AppLocator.CurrentMutable.Register<IViewFor<FirstViewModel>>(() => new FirstView());

        ////AppLocator.CurrentMutable.RegisterConstant<BrowserViewModel>(new());
        ////AppLocator.CurrentMutable.Register<IViewFor<BrowserViewModel>>(() => new BrowserView());
        AppLocator.CurrentMutable.SetupComplete();
    }
}
