// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI;
using Splat;
using static ReactiveUI.Binding.ReactiveUIBindingExtensions;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Interaction logic for TreeViewView.xaml.</summary>
public partial class TreeViewView : IViewFor<TreeViewViewModel>
{
    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(TreeViewViewModel),
        typeof(TreeViewView),
        new(null));

    /// <summary>Tracks whether reactive bindings have been configured.</summary>
    private bool _bindingsConfigured;

    /// <summary>Initializes a new instance of the <see cref="TreeViewView"/> class.</summary>
    public TreeViewView()
    {
        InitializeComponent();
        ViewModel = new();

        // The gallery supplies its own root collection through ItemsSource.
        FamilyTree.ViewModel = null;

        // Register treeview elements
        AppLocator.CurrentMutable.Register(static () => new PersonView(), typeof(IViewFor<Person>));
        AppLocator.CurrentMutable.Register(static () => new PetView(), typeof(IViewFor<Pet>));

        Loaded += OnLoaded;
    }

    /// <summary>Gets the binding root view model.</summary>
    public TreeViewViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public TreeViewViewModel? ViewModel
    {
        get => (TreeViewViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TreeViewViewModel?)value;
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
            // Bind viewmodel to Treeview
            _ = this.OneWayBind(ViewModel, vm => vm.Family, v => v.FamilyTree.ItemsSource).DisposeWith(d);
            _ = this.WhenAnyValue(x => x.FamilyTree.SelectedItem)
                .BindTo(this, x => x.ViewModel!.SelectedItem)
                .DisposeWith(d);
            _ = this.Bind(ViewModel, vm => vm.NewName, v => v.NewName.Text).DisposeWith(d);
            _ = this.Bind(ViewModel, vm => vm.PetName, v => v.PetName.Text).DisposeWith(d);
            _ = this.Bind(ViewModel, vm => vm.SelectedElement, v => v.Selected.Text).DisposeWith(d);
            _ = this.Bind(ViewModel, vm => vm.LastSelectedElement, v => v.LastSelected.Text).DisposeWith(d);
            _ = this.BindCommand(ViewModel, vm => vm.AddPerson, v => v.AddPerson);
            _ = this.BindCommand(ViewModel, vm => vm.AddPet, v => v.AddPet);
            _ = this.BindCommand(ViewModel, vm => vm.Collapse, v => v.Collapse);
            _ = this.BindCommand(ViewModel, vm => vm.Expand, v => v.Expand);
            _ = this.BindCommand(ViewModel, vm => vm.Remove, v => v.Remove);
        });
    }
}
