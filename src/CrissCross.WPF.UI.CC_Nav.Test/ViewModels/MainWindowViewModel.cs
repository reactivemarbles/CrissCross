// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
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
        Locator.CurrentMutable.RegisterConstant<MainViewModel>(new());
        Locator.CurrentMutable.Register<IViewFor<MainViewModel>>(() => new MainView());

        Locator.CurrentMutable.RegisterConstant<FirstViewModel>(new());
        Locator.CurrentMutable.Register<IViewFor<FirstViewModel>>(() => new FirstView());

        Locator.CurrentMutable.RegisterConstant<BrowserViewModel>(new());
        Locator.CurrentMutable.Register<IViewFor<BrowserViewModel>>(() => new BrowserView());
        Locator.CurrentMutable.SetupComplete();
    }
}
