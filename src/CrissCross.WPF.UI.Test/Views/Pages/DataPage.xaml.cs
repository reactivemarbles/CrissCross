// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Test.ViewModels;
using ReactiveMarbles.ObservableEvents;

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

    /// <summary>
    /// Gets the owner.
    /// </summary>
    /// <value>
    /// The owner.
    /// </value>
    string ICanShowMessages.Owner => "mainWindow";
}
