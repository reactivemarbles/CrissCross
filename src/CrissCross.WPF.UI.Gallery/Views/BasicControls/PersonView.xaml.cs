// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>
/// Interaction logic for PersonView.xaml.
/// </summary>
[IViewFor<Person>]
public partial class PersonView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PersonView"/> class.
    /// </summary>
    public PersonView()
    {
        InitializeComponent();
        this.WhenActivated(d => this.OneWayBind(ViewModel, vm => vm.Name, v => v.PersonName.Text).DisposeWith(d));
    }
}
