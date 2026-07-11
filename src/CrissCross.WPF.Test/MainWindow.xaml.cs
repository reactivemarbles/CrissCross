// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.WPF.Test;

/// <summary>Interaction logic for MainWindow.xaml.</summary>
public partial class MainWindow
{
    /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
    public MainWindow()
    {
        InitializeComponent();
        _ = this.WhenActivated(d =>
        {
            var navigateBack = ReactiveCommand.Create(() => this.NavigateBack(), this.CanNavigateBack());
            NavBack.Command = navigateBack;
            _ = navigateBack.DisposeWith(d);
            this.NavigateToView<BrowserViewModel>();
        });
    }
}
