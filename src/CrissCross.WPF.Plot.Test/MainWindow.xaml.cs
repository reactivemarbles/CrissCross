// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.Plot.Test.ViewModels;
using CrissCross.WPF.UI;
using CrissCross.WPF.UI.Appearance;
using CrissCross.WPF.UI.Controls;
using ReactiveUI;
using Splat;

namespace CrissCross.WPF.Plot.Test;

/// <summary>Interaction logic for MainWindow.xaml.</summary>
public partial class MainWindow : IAmBuilt, IDisposable
{
    /// <summary>The tracker property.</summary>
    public static readonly DependencyProperty TrackerProperty = DependencyProperty.Register(
        nameof(Tracker),
        typeof(Tracker),
        typeof(MainWindow),
        new PropertyMetadata(null));

    /// <summary>Stores bindings owned by the window's visual-tree lifetime.</summary>
    private CompositeDisposable? _viewBindings;

    /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
    public MainWindow()
    {
        InitializeComponent();
        Navigation = NavBreadcrumb;
        ViewModel = new();
        DataContext = ViewModel;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>Gets the nav breadcrumb.</summary>
    /// <value>
    /// The nav breadcrumb.
    /// </value>
    public static BreadcrumbBar? Navigation { get; private set; }

    /// <summary>Releases the window's event handlers and active bindings.</summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases resources owned by the window.</summary>
    /// <param name="disposing">Whether managed resources should be released.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;
        _viewBindings?.Dispose();
        _viewBindings = null;
    }

    /// <summary>Initializes theme tracking, bindings, and navigation after construction.</summary>
    /// <param name="sender">The loaded window.</param>
    /// <param name="e">The routed event data.</param>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_viewBindings is not null)
        {
            return;
        }

        SystemThemeWatcher.Watch(this);
        var tracker = AppLocator.Current.GetService<Tracker>();
        tracker?.Track(this);
        SetCurrentValue(TrackerProperty, tracker);
        _viewBindings = new();
        _ = this.OneWayBind(ViewModel, vm => vm.ApplicationTitle, v => v.Title).DisposeWith(_viewBindings);
        _ = this.OneWayBind(ViewModel, vm => vm.NavigationModels, v => v.NavigationLeft.ItemsSource)
            .DisposeWith(_viewBindings);
        NavBreadcrumb.SetupNavigation(nameof(mainWindow));
        NavBreadcrumb.NavigateTo(new NavigationKeyRequest<MainViewModel>(), "Main");
    }

    /// <summary>Releases view bindings when the window leaves the visual tree.</summary>
    /// <param name="sender">The unloaded window.</param>
    /// <param name="e">The routed event data.</param>
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _viewBindings?.Dispose();
        _viewBindings = null;
    }
}
