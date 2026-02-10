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
/// Interaction logic for TreeViewView.xaml.
/// </summary>
[IViewFor<TreeViewViewModel>]
public partial class TreeViewView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TreeViewView"/> class.
    /// </summary>
    public TreeViewView()
    {
        InitializeComponent();
        ViewModel = new TreeViewViewModel();

        // Register treeview elements
        AppLocator.CurrentMutable.Register(() => new PersonView(), typeof(IViewFor<Person>));
        AppLocator.CurrentMutable.Register(() => new PetView(), typeof(IViewFor<Pet>));

        this.WhenActivated(d =>
        {
            // Bind viewmodel to Treeview
            this.OneWayBind(ViewModel, vm => vm.Family, v => v.FamilyTree.ViewModel!.Children).DisposeWith(d);
            this.WhenAnyValue(x => x.FamilyTree.SelectedItem).BindTo(this, x => x.ViewModel!.SelectedItem).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.NewName, v => v.NewName.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.PetName, v => v.PetName.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedElement, v => v.Selected.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.LastSelectedElement, v => v.LastSelected.Text).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.AddPerson, v => v.AddPerson);
            this.BindCommand(ViewModel, vm => vm.AddPet, v => v.AddPet);
            this.BindCommand(ViewModel, vm => vm.Collapse, v => v.Collapse);
            this.BindCommand(ViewModel, vm => vm.Expand, v => v.Expand);
            this.BindCommand(ViewModel, vm => vm.Remove, v => v.Remove);
        });
    }
}
