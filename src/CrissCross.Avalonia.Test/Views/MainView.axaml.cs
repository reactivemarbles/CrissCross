// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;

namespace CrissCross.Avalonia.Test.Views;

/// <summary>
/// MainView.
/// </summary>
/// <seealso cref="UserControl" />
public partial class MainView : ReactiveUserControl<MainViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainView"/> class.
    /// </summary>
    public MainView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            ViewModel ??= Locator.Current.GetService<MainViewModel>();
            this.BindCommand(ViewModel, vm => vm.GotoFirst, v => v.GotoFirst).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.GotoMain, v => v.GotoSecond).DisposeWith(d);
        });
    }
}
