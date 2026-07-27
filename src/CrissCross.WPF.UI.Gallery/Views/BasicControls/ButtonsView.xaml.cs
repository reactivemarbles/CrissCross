// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Interaction logic for ButtonsView.xaml.</summary>
[IViewFor<ButtonsViewModel>]
public partial class ButtonsView
{
    /// <summary>Initializes a new instance of the <see cref="ButtonsView"/> class.</summary>
    public ButtonsView()
    {
        InitializeComponent();
        _ = this.WhenActivated(disposables =>
        {
            _ = EventSignal
                .From<RoutedEventHandler, RoutedEventArgs>(
                    static handler => handler.Invoke,
                    handler => BezelButton1.Click += handler,
                    handler => BezelButton1.Click -= handler)
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Subscribe(_ => BezelToggleButton1.IsChecked = !BezelToggleButton1.IsChecked)
                .DisposeWith(disposables);
        });
    }
}
