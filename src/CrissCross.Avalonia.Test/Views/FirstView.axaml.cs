// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
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
    }

    /// <summary>Creates a view and registers activation after construction.</summary>
    /// <returns>The initialized view.</returns>
    internal static FirstView Create()
    {
        var view = new FirstView();
        view.InitializeActivation();
        return view;
    }

    /// <summary>Registers the view activation bindings.</summary>
    private void InitializeActivation() =>
        _ = this.WhenActivated(d =>
        {
            ViewModel ??= AppLocator.Current.GetService<FirstViewModel>();
            _ = this.BindCommand(ViewModel, static vm => vm.GotoMain, static v => v.GotoSecond).DisposeWith(d);
            _ = this.BindCommand(ViewModel, static vm => vm.GotoFirst, static v => v.GotoFirst).DisposeWith(d);
        });
}
