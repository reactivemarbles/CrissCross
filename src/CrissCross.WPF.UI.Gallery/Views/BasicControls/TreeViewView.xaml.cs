// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Splat;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Interaction logic for TreeViewView.xaml.</summary>
[IViewFor<TreeViewViewModel>]
public partial class TreeViewView
{
    /// <summary>Initializes a new instance of the <see cref="TreeViewView"/> class.</summary>
    public TreeViewView()
    {
        InitializeComponent();
        ViewModel = new();

        // Register treeview elements
        AppLocator.CurrentMutable.Register(() => new PersonView(), typeof(IViewFor<Person>));
        AppLocator.CurrentMutable.Register(() => new PetView(), typeof(IViewFor<Pet>));

        _ = this.WhenActivated(d =>
        {
            // Bind viewmodel to Treeview
            _ = this.OneWayBind(ViewModel, vm => vm.Family, v => v.FamilyTree.ViewModel!.Children).DisposeWith(d);
            _ = this.WhenAnyValue(x => x.FamilyTree.SelectedItem).BindTo(this, x => x.ViewModel!.SelectedItem).DisposeWith(d);
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
