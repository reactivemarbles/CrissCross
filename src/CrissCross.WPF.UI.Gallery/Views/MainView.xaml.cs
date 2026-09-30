// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI;
using Splat;
using static ReactiveUI.Binding.ReactiveUIBindingExtensions;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Interaction logic for MainView.xaml.</summary>
public partial class MainView : IViewFor<MainViewModel>
{
    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(MainViewModel),
        typeof(MainView),
        new(null));

    /// <summary>Tracks whether reactive bindings have been configured.</summary>
    private bool _bindingsConfigured;

    /// <summary>Initializes a new instance of the <see cref="MainView"/> class.</summary>
    public MainView()
    {
        InitializeComponent();
        ViewModel = AppLocator.Current.GetService<MainViewModel>()!;
        Loaded += OnLoaded;
    }

    /// <summary>Gets the binding root view model.</summary>
    public MainViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public MainViewModel? ViewModel
    {
        get => (MainViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (MainViewModel?)value;
    }

    /// <summary>Configures reactive bindings after construction has completed.</summary>
    /// <param name="sender">The loaded view.</param>
    /// <param name="e">The routed event data.</param>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_bindingsConfigured)
        {
            return;
        }

        _bindingsConfigured = true;
        _ = this.WhenActivated(d =>
        {
            // Bind the view model
            _ = this.Bind(ViewModel, vm => vm.AppXamlSetup, v => v.AppXamlSetup.Text).DisposeWith(d);
            _ = this.Bind(ViewModel, vm => vm.MainWindowXamlSetup, v => v.MainWindowXamlSetup.Text).DisposeWith(d);
            _ = this.Bind(ViewModel, vm => vm.MainWindowXamlCsSetup, v => v.MainWindowXamlCsSetup.Text).DisposeWith(d);
        });
    }
}
