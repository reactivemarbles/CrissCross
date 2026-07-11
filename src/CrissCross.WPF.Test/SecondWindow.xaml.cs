// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.WPF.Test;

/// <summary>Interaction logic for SecondWindow.xaml.</summary>
public partial class SecondWindow : IUseNavigation
{
    /// <summary>Initializes a new instance of the <see cref="SecondWindow"/> class.</summary>
    public SecondWindow()
    {
        InitializeComponent();
        _ = this.WhenActivated(d =>
        {
            this.NavigateToView<FirstViewModel>();
            var navigateBack = ReactiveCommand.Create(() => this.NavigateBack(), this.CanNavigateBack());
            NavBack.Command = navigateBack;
            _ = navigateBack.DisposeWith(d);
        });
    }
}
