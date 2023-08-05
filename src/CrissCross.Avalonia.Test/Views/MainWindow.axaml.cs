// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using Avalonia.Controls;
using ReactiveUI;

namespace CrissCross.Avalonia.Test.Views;

/// <summary>
/// MainWindow.
/// </summary>
/// <seealso cref="Window" />
public partial class MainWindow : NavigationWindow<MainWindowViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            this.NavigateToView<MainViewModel>();
            ////NavBack.Command = ReactiveCommand.Create(() => this.NavigateBack(), this.CanNavigateBack()).DisposeWith(d);
        });
    }
}
