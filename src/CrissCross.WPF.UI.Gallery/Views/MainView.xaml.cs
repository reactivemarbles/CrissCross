// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Splat;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>
/// Interaction logic for MainView.xaml.
/// </summary>
[IViewFor<MainViewModel>]
public partial class MainView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainView"/> class.
    /// </summary>
    public MainView()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<MainViewModel>()!;
        this.WhenActivated(d =>
        {
            // Bind the view model
            this.Bind(ViewModel, vm => vm.AppXamlSetup, v => v.AppXamlSetup.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.MainWindowXamlSetup, v => v.MainWindowXamlSetup.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.MainWindowXamlCsSetup, v => v.MainWindowXamlCsSetup.Text).DisposeWith(d);
        });
    }
}
