// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>
/// Interaction logic for PetView.xaml.
/// </summary>
[IViewFor<Pet>]
public partial class PetView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PetView"/> class.
    /// </summary>
    public PetView()
    {
        InitializeComponent();
        this.WhenActivated(d => this.OneWayBind(ViewModel, vm => vm.Name, v => v.PetName.Text).DisposeWith(d));
    }
}
