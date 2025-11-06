// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if !DESIGN
using System.Reactive.Disposables.Fluent;
using ReactiveUI;
using ReactiveUI.Winforms;
using Splat;
#endif

namespace CrissCross.WinForms.Test.Views;

/// <summary>
/// FirstView.
/// </summary>
#if DESIGN
public partial class FirstView : UserControl
#else
public partial class FirstView : ReactiveUserControl<FirstViewModel>
#endif
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FirstView"/> class.
    /// </summary>
    public FirstView()
    {
        InitializeComponent();
#if !DESIGN
        this.WhenActivated(d =>
        {
            ViewModel ??= Locator.Current.GetService<FirstViewModel>();
            this.BindCommand(ViewModel, vm => vm.GotoFirst, v => v.GotoFirst).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.GotoMain, v => v.GotoMain).DisposeWith(d);
        });
#endif
    }
}
