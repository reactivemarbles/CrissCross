// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using ReactiveUI.Winforms;
using Splat;

namespace CrissCross.WinForms.Test.Views;

/// <summary>FirstView member.</summary>
public partial class FirstView : ReactiveUserControl<FirstViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="FirstView"/> class.</summary>
    public FirstView()
    {
        InitializeComponent();
        _ = this.WhenActivated(d =>
        {
            ViewModel ??= AppLocator.Current.GetService<FirstViewModel>();
            _ = this.BindCommand(ViewModel, vm => vm.GotoFirst, v => v.GotoFirst).DisposeWith(d);
            _ = this.BindCommand(ViewModel, vm => vm.GotoMain, v => v.GotoMain).DisposeWith(d);
        });
    }
}
