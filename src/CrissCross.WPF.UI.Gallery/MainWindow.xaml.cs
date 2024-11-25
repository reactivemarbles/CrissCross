// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Windows;
using CrissCross.WPF.UI.Appearance;
using CrissCross.WPF.UI.Controls;
using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI;
using Splat;

namespace CrissCross.WPF.UI.Gallery;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
public partial class MainWindow : IAmBuilt
{
    /// <summary>
    /// The tracker property.
    /// </summary>
    public static readonly DependencyProperty TrackerProperty = DependencyProperty.Register(
        nameof(Tracker),
        typeof(Tracker),
        typeof(MainWindow),
        new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        // Watch for system theme changes
        SystemThemeWatcher.Watch(this);
        InitializeComponent();
        Navigation = NavBreadcrumb;

        // Set the data context
        DataContext = ViewModel = new();
        this.WhenActivated(d =>
        {
            // Set the tracker
            var tracker = Locator.Current.GetService<Tracker>();
            tracker?.Track(this);
            SetCurrentValue(TrackerProperty, tracker);

            // Bind the view model
            this.OneWayBind(ViewModel, vm => vm.ApplicationTitle, v => v.Title).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.NavigationModels, v => v.NavigationLeft.ItemsSource).DisposeWith(d);

            NavBreadcrumb.SetupNavigation("mainWindow");

            // Navigate to the main view
            NavBreadcrumb.NavigateTo<MainViewModel>(breadcrumbItemContent: "Main");

            ////this.NavigateToView<MainViewModel>();
        });

        // Dispose the view model on close
        Closing += (s, e) => ViewModel.Dispose();
    }

    /// <summary>
    /// Gets the nav breadcrumb.
    /// </summary>
    /// <value>
    /// The nav breadcrumb.
    /// </value>
    public static BreadcrumbBar? Navigation { get; private set; }
}
