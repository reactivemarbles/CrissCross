// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Test.ViewModels;

namespace CrissCross.WPF.UI.Test.Views.Pages;

/// <summary>
/// Interaction logic for SettingsPage.xaml.
/// </summary>
public partial class SettingsPage : INavigableView<SettingsViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPage"/> class.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    public SettingsPage(SettingsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    /// <summary>
    /// Gets viewModel used by the view.
    /// Optionally, it may implement <see cref="T:CrissCross.WPF.UI.Controls.INavigationAware" /> and be navigated by <see cref="T:CrissCross.WPF.UI.Controls.INavigationView" />.
    /// </summary>
    public SettingsViewModel ViewModel { get; }
}
