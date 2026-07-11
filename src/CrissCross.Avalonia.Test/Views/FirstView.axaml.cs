// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Avalonia;
using Splat;

namespace CrissCross.Avalonia.Test.Views;

/// <summary>MainView member.</summary>
/// <seealso cref="UserControl" />
public partial class FirstView : ReactiveUserControl<FirstViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="FirstView"/> class.</summary>
    public FirstView()
    {
        InitializeComponent();
        _ = this.WhenActivated(d =>
        {
            ViewModel ??= AppLocator.Current.GetService<FirstViewModel>();
            _ = this.BindCommand(ViewModel, vm => vm.GotoMain, v => v.GotoSecond).DisposeWith(d);
            _ = this.BindCommand(ViewModel, vm => vm.GotoFirst, v => v.GotoFirst).DisposeWith(d);
        });
    }
}
