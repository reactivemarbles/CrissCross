// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Interaction logic for PersonView.xaml.</summary>
[IViewFor<Person>]
public partial class PersonView
{
    /// <summary>Initializes a new instance of the <see cref="PersonView"/> class.</summary>
    public PersonView()
    {
        InitializeComponent();
        _ = this.WhenActivated(BindViewModel);
    }

    /// <summary>Binds the active view model to the view.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void BindViewModel(CompositeDisposable disposables) =>
        this.OneWayBind(ViewModel, vm => vm.DisplayName, v => v.PersonName.Text).DisposeWith(disposables);
}
