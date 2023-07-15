// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !DESIGN
using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.Winforms;
using Splat;
#endif

namespace CrissCross.WinForms.Test.Views;

/// <summary>
/// MainView.
/// </summary>
#if DESIGN
public partial class MainView : UserControl
#else
public partial class MainView : ReactiveUserControl<MainViewModel>
#endif
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainView"/> class.
    /// </summary>
    public MainView()
    {
        InitializeComponent();
#if !DESIGN
        this.WhenActivated(d =>
        {
            ViewModel ??= Locator.Current.GetService<MainViewModel>();
            this.BindCommand(ViewModel, vm => vm.GotoFirst, v => v.GotoFirst).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.GotoMain, v => v.GotoMain).DisposeWith(d);
        });
#endif
    }
}
