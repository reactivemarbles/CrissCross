// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
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
        Appearance.SystemThemeWatcher.Watch(this);

        InitializeComponent();

        Breadcrumb.SetupNavigation(nameof(mainWindow));
        Navigation = Breadcrumb;

        _ = this.WhenActivated(Activate);
    }

    /// <summary>Gets the navigation.</summary>
    /// <value>
    /// The navigation.
    /// </value>
    public static BreadcrumbBar? Navigation { get; private set; }

    /// <summary>Activates navigation bindings for the window.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void Activate(CompositeDisposable disposables)
    {
        var navigation = Navigation ?? throw new InvalidOperationException("The navigation control must be initialized before activation.");
        var navigateBack = ReactiveCommand.Create(() => navigation.NavigateBack(), this.CanNavigateBack());
        NavBack.Command = navigateBack;
        _ = navigateBack.DisposeWith(disposables);
        navigation.NavigateTo<MainViewModel>(breadcrumbItemContent: "Main View");
    }
}
