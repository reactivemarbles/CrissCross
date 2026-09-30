// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.UI.Controls;
using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI;
using Splat;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Interaction logic for the curated WPF control catalog.</summary>
public partial class ControlCatalogView : IViewFor<ControlCatalogViewModel>
{
    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(ControlCatalogViewModel),
        typeof(ControlCatalogView),
        new(null));

    /// <summary>Initializes a new instance of the <see cref="ControlCatalogView"/> class.</summary>
    public ControlCatalogView()
    {
        InitializeComponent();
        ViewModel = AppLocator.Current.GetService<ControlCatalogViewModel>()!;
        DataContext = ViewModel;
        _ = this.WhenActivated((CompositeDisposable _) => ShowSnackbar());
    }

    /// <summary>Gets the binding root view model.</summary>
    public ControlCatalogViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public ControlCatalogViewModel? ViewModel
    {
        get => (ControlCatalogViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (ControlCatalogViewModel?)value;
    }

    /// <summary>Shows the composed Snackbar example through its required presenter.</summary>
    private void ShowSnackbar()
    {
        var snackbar = new Snackbar(CatalogSnackbarPresenter) { Title = "Snackbar", Content = "Reactive composition active.", Timeout = TimeSpan.FromMinutes(1) };

        snackbar.Show(immediately: true);
    }
}
