// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI;
using static ReactiveUI.Binding.ReactiveUIBindingExtensions;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Interaction logic for PersonView.xaml.</summary>
public partial class PersonView : IViewFor<Person>
{
    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(Person),
        typeof(PersonView),
        new(null));

    /// <summary>Initializes a new instance of the <see cref="PersonView"/> class.</summary>
    public PersonView()
    {
        InitializeComponent();
        _ = this.WhenActivated(BindViewModel);
    }

    /// <summary>Gets the binding root view model.</summary>
    public Person? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public Person? ViewModel
    {
        get => (Person?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Person?)value;
    }

    /// <summary>Binds the active view model to the view.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void BindViewModel(CompositeDisposable disposables) =>
        this.OneWayBind(ViewModel, vm => vm.DisplayName, v => v.PersonName.Text).DisposeWith(disposables);
}
