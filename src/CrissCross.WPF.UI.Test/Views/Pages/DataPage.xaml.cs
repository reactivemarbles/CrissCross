// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Test.ViewModels;
using ReactiveMarbles.ObservableEvents;
using MessageBoxButton = System.Windows.MessageBoxButton;

namespace CrissCross.WPF.UI.Test.Views.Pages;

/// <summary>
/// Interaction logic for DataView.xaml.
/// </summary>
public partial class DataPage : INavigableView<DataViewModel>, ICanShowMessages
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataPage"/> class.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    public DataPage(DataViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
        this.Events().Loaded.Subscribe(async _ => await this.MessageBoxShow("I am a Custom message box", "Custom Message Box", "Custom Button"));
    }

    /// <summary>
    /// Gets viewModel used by the view.
    /// Optionally, it may implement <see cref="T:CrissCross.WPF.UI.Controls.INavigationAware" /> and be navigated by <see cref="T:CrissCross.WPF.UI.Controls.INavigationView" />.
    /// </summary>
    public DataViewModel ViewModel { get; }
}
