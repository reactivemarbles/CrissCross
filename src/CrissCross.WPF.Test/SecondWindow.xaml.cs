// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using ReactiveUI;

namespace CrissCross.WPF.Test;

/// <summary>Interaction logic for SecondWindow.xaml.</summary>
public partial class SecondWindow : IUseNavigation
{
    /// <summary>Initializes a new instance of the <see cref="SecondWindow"/> class.</summary>
    public SecondWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <summary>Registers activation when the fully constructed view is loaded.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event data.</param>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        _ = this.WhenActivated(d =>
        {
            this.NavigateToView(new NavigationKeyRequest<FirstViewModel>());
            var navigateBack = ReactiveCommand.Create(() => this.NavigateBack(), this.CanNavigateBack());
            NavBack.Command = navigateBack;
            _ = navigateBack.DisposeWith(d);
        });
    }
}
