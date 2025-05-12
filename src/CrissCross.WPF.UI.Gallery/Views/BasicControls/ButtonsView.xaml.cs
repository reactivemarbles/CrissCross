// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>
/// Interaction logic for ButtonsView.xaml.
/// </summary>
[IViewFor<ButtonsViewModel>]
public partial class ButtonsView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ButtonsView"/> class.
    /// </summary>
    public ButtonsView()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            BezelButton1.Events()
                .Click
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => BezelToggleButton1.IsChecked = !BezelToggleButton1.IsChecked)
                .DisposeWith(disposables);
        });
    }
}
