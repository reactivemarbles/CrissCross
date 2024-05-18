// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using ReactiveUI;

namespace CrissCross.WPF.Test;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
public partial class MainWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            NavBack.Command = ReactiveCommand.Create(() => this.NavigateBack(), this.CanNavigateBack()).DisposeWith(d);
            this.NavigateToView<BrowserViewModel>();
        });
    }
}
