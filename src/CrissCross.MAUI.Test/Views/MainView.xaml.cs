// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using ReactiveUI;
using Splat;

namespace CrissCross.MAUI.Test;

/// <summary>
/// MainView.
/// </summary>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MainView
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
            this.BindCommand(ViewModel, vm => vm.GotoMain, v => v.GotoMain).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.GotoFirst, v => v.GotoFirst).DisposeWith(d);
        });
    }
}
