// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using ReactiveUI;

namespace CrissCross.WPF.UI.Test.ViewModels;

/// <summary>
/// MainWindowViewModel.
/// </summary>
/// <seealso cref="RxObject" />
public class MainWindowViewModel : RxObject
{
    private bool _isInitialized;
    private string _applicationTitle = string.Empty;
    private ObservableCollection<object> _navigationItems = [];
    private ObservableCollection<object> _navigationFooter = [];
    private ObservableCollection<MenuItem> _trayMenuItems = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        if (!_isInitialized)
        {
            InitializeViewModel();
        }
    }

    /// <summary>
    /// Gets or sets the application title.
    /// </summary>
    /// <value>
    /// The application title.
    /// </value>
    public string ApplicationTitle
    {
        get => _applicationTitle;
        set => this.RaiseAndSetIfChanged(ref _applicationTitle, value);
    }

    /// <summary>
    /// Gets or sets the navigation items.
    /// </summary>
    /// <value>
    /// The navigation items.
    /// </value>
    public ObservableCollection<object> NavigationItems
    {
        get => _navigationItems;
        set => this.RaiseAndSetIfChanged(ref _navigationItems, value);
    }

    /// <summary>
    /// Gets or sets the navigation footer.
    /// </summary>
    /// <value>
    /// The navigation footer.
    /// </value>
    public ObservableCollection<object> NavigationFooter
    {
        get => _navigationFooter;
        set => this.RaiseAndSetIfChanged(ref _navigationFooter, value);
    }

    /// <summary>
    /// Gets or sets the tray menu items.
    /// </summary>
    /// <value>
    /// The tray menu items.
    /// </value>
    public ObservableCollection<MenuItem> TrayMenuItems
    {
        get => _trayMenuItems;
        set => this.RaiseAndSetIfChanged(ref _trayMenuItems, value);
    }

    private void InitializeViewModel()
    {
        ApplicationTitle = "CrissCross.WPF.UI Demo";

        NavigationItems =
        [
            new NavigationViewItem()
            {
                Content = "Home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },
            new NavigationViewItem()
            {
                Content = "Data",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                TargetPageType = typeof(Views.Pages.DataPage)
            }
        ];

        NavigationFooter =
        [
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        ];

        TrayMenuItems =
        [
            new MenuItem { Header = "Home", Tag = "tray_home" }
        ];

        _isInitialized = true;
    }
}
