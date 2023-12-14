// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !DESIGN
using System.Reactive;
using CrissCross.WinForms.Test.Views;
using ReactiveUI;
using Splat;
#endif

namespace CrissCross.WinForms.Test;

/// <summary>
/// MainWindowViewModel.
/// </summary>
public class MainWindowViewModel : RxObject
{
#if !DESIGN
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        Locator.CurrentMutable.RegisterConstant<MainViewModel>(new());
        Locator.CurrentMutable.Register<IViewFor<MainViewModel>>(() => new MainView());

        Locator.CurrentMutable.RegisterConstant<FirstViewModel>(new());
        Locator.CurrentMutable.Register<IViewFor<FirstViewModel>>(() => new FirstView());

        Locator.CurrentMutable.SetupComplete();
        var s = new SecondForm();
        s.Show();
    }
#endif
}
