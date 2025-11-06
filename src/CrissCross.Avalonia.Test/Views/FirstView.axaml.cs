// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;

namespace CrissCross.Avalonia.Test.Views;

/// <summary>
/// MainView.
/// </summary>
/// <seealso cref="UserControl" />
public partial class FirstView : ReactiveUserControl<FirstViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FirstView"/> class.
    /// </summary>
    public FirstView()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            ViewModel ??= Locator.Current.GetService<FirstViewModel>();
            this.BindCommand(ViewModel, vm => vm.GotoMain, v => v.GotoSecond).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.GotoFirst, v => v.GotoFirst).DisposeWith(d);
        });
    }
}
