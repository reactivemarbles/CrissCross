// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
