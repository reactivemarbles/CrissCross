// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls.Presenters;
using CrissCross.Avalonia.UI.Controls;
using CrissCross.Avalonia.UI.Gallery.ViewModels;
using ReactiveUI;
using Splat;

namespace CrissCross.Avalonia.UI.Gallery.Views;

/// <summary>
/// Main window for the gallery application.
/// </summary>
public partial class MainWindow : NavigationWindow<MainViewModel>
{
    private const string NavHostName = "mainNavHost";

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        // Set the window name for navigation registration
        Name = NavHostName;

        InitializeComponent();

        // Set the DataContext to the MainViewModel
        DataContext = AppLocator.Current.GetService<MainViewModel>();

        // Use WhenActivated instead of BuildComplete to ensure navigation host is registered
        this.WhenActivated(_ =>
        {
            // Navigate to home page initially
            this.NavigateToView<HomePageViewModel>();
        });
    }

    /// <summary>
    /// Registers the content presenter.
    /// </summary>
    /// <param name="presenter">The presenter.</param>
    /// <returns>
    /// A bool.
    /// </returns>
    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter == null)
        {
            return false;
        }

        // Override the default content presenter with a grid containing a back button and the navigation frame
        if (presenter.Name == "PART_ContentPresenter" && presenter.Content == null)
        {
            var grid = new Grid();
            presenter.Content = grid;
        }

        return base.RegisterContentPresenter(presenter);
    }
}
