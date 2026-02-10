// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if !DESIGN
using System.Runtime.Versioning;
using CrissCross.WinForms.Test.Views;
using ReactiveUI;
using ReactiveUI.Builder;
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
    [RequiresPreviewFeatures]
    public MainWindowViewModel()
    {
        AppLocator.CurrentMutable.RegisterConstant<MainViewModel>(new());
        AppLocator.CurrentMutable.Register<IViewFor<MainViewModel>>(() => new MainView());

        AppLocator.CurrentMutable.RegisterConstant<FirstViewModel>(new());
        AppLocator.CurrentMutable.Register<IViewFor<FirstViewModel>>(() => new FirstView());

        AppLocator.CurrentMutable.SetupComplete();
        var s = new SecondForm();
        s.Show();
    }
#endif
}
