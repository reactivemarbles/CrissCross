// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI;
using static ReactiveUI.Binding.ReactiveUIBindingExtensions;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Interaction logic for PetView.xaml.</summary>
public partial class PetView : IViewFor<Pet>
{
    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(Pet),
        typeof(PetView),
        new(null));

    /// <summary>Initializes a new instance of the <see cref="PetView"/> class.</summary>
    public PetView()
    {
        InitializeComponent();
        _ = this.WhenActivated(BindViewModel);
    }

    /// <summary>Gets the binding root view model.</summary>
    public Pet? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public Pet? ViewModel
    {
        get => (Pet?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Pet?)value;
    }

    /// <summary>Binds the active view model to the view.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void BindViewModel(CompositeDisposable disposables) =>
        this.OneWayBind(ViewModel, vm => vm.DisplayName, v => v.PetName.Text).DisposeWith(disposables);
}
