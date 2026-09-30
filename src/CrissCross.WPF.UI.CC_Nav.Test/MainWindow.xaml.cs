// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Controls;
using ReactiveUI;

namespace CrissCross.WPF.UI.CC_Nav.Test;

/// <summary>Interaction logic for MainWindow.xaml.</summary>
public partial class MainWindow
{
    /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
    public MainWindow()
    {
        InitializeComponent();

        Breadcrumb.SetupNavigation(nameof(mainWindow));
        Navigation = Breadcrumb;

        Loaded += OnLoaded;
    }

    /// <summary>Gets the navigation.</summary>
    /// <value>
    /// The navigation.
    /// </value>
    public static BreadcrumbBar? Navigation { get; private set; }

    /// <summary>Initializes window services after construction.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event data.</param>
    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        Appearance.SystemThemeWatcher.Watch(this);
        _ = this.WhenActivated(Activate);
    }

    /// <summary>Activates navigation bindings for the window.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void Activate(CompositeDisposable disposables)
    {
        var navigation =
            Navigation
            ?? throw new InvalidOperationException("The navigation control must be initialized before activation.");
        var navigateBack = ReactiveCommand.Create(() => navigation.NavigateBack(), this.CanNavigateBack());
        NavBack.Command = navigateBack;
        _ = navigateBack.DisposeWith(disposables);
        navigation.NavigateTo(new NavigationKeyRequest<MainViewModel>(), "Main View");
    }
}
