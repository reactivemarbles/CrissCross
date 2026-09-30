// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WinForms.Test.Views;
using Splat;

namespace CrissCross.WinForms.Test;

/// <summary>MainWindowViewModel member.</summary>
public class MainWindowViewModel : RxObject
{
    /// <summary>Initializes a new instance of the <see cref="MainWindowViewModel"/> class.</summary>
    public MainWindowViewModel()
    {
        AppLocator.CurrentMutable.RegisterConstant<MainViewModel>(new());
        AppLocator.CurrentMutable.Register<IViewFor<MainViewModel>>(static () => new MainView());

        AppLocator.CurrentMutable.RegisterConstant<FirstViewModel>(new());
        AppLocator.CurrentMutable.Register<IViewFor<FirstViewModel>>(static () => new FirstView());

        AppLocator.CurrentMutable.SetupComplete();
        var secondForm = new SecondForm().DisposeWith(Disposables);
        secondForm.Show();
    }
}
