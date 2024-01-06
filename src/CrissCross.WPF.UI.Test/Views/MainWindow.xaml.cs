// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Test.ViewModels;

namespace CrissCross.WPF.UI.Test.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
public partial class MainWindow : INavigationWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="pageService">The page service.</param>
    /// <param name="navigationService">The navigation service.</param>
    public MainWindow(
        MainWindowViewModel viewModel,
        IPageService pageService,
        INavigationService navigationService)
    {
        ViewModel = viewModel;
        DataContext = this;

        Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);

        InitializeComponent();
        SetPageService(pageService);

        navigationService?.SetNavigationControl(RootNavigation);
        Loaded += (s, e) => RootNavigation.IsPaneOpen = false;
    }

    /// <summary>
    /// Gets the view model.
    /// </summary>
    /// <value>
    /// The view model.
    /// </value>
    public MainWindowViewModel ViewModel { get; }

    /// <summary>
    /// Provides direct access to the control responsible for navigation.
    /// </summary>
    /// <returns>
    /// Instance of the <see cref="T:Wpf.Ui.Controls.INavigationView" /> control.
    /// </returns>
    public INavigationView GetNavigation() => RootNavigation;

    /// <summary>
    /// Lets you navigate to the selected page based on it's type. Should be used with <see cref="T:Wpf.Ui.IPageService" />.
    /// </summary>
    /// <param name="pageType"><see langword="Type" /> of the page.</param>
    /// <returns>
    ///   <see langword="true" /> if the operation succeeds. <see langword="false" /> otherwise.
    /// </returns>
    public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

    /// <summary>
    /// Lets you attach the service that delivers page instances to <see cref="T:Wpf.Ui.Controls.INavigationView" />.
    /// </summary>
    /// <param name="pageService">Instance of the <see cref="T:Wpf.Ui.IPageService" /> with attached service provider.</param>
    public void SetPageService(IPageService pageService) => RootNavigation.SetPageService(pageService);

    /// <summary>
    /// Triggers the command to open a window.
    /// </summary>
    public void ShowWindow() => Show();

    /// <summary>
    /// Triggers the command to close a window.
    /// </summary>
    public void CloseWindow() => Close();

    /// <summary>
    /// Provides direct access to the control responsible for navigation.
    /// </summary>
    /// <returns>
    /// Instance of the <see cref="T:Wpf.Ui.Controls.INavigationView" /> control.
    /// </returns>
    INavigationView INavigationWindow.GetNavigation() => throw new NotImplementedException();

    /// <summary>
    /// Lets you attach the service provider that delivers page instances to <see cref="T:Wpf.Ui.Controls.INavigationView" />.
    /// </summary>
    /// <param name="serviceProvider">Instance of the <see cref="T:System.IServiceProvider" />.</param>
    public void SetServiceProvider(IServiceProvider serviceProvider) => throw new NotImplementedException();

    /// <summary>
    /// Raises the closed event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Make sure that closing this window will begin the process of closing the application.
        Application.Current.Shutdown();
    }
}
