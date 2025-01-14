// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>
/// MainViewModel.
/// </summary>
/// <seealso cref="CrissCross.RxObject" />
public partial class MainViewModel : RxObject
{
    /// <summary>
    /// Gets the application xaml setup.
    /// </summary>
    /// <value>
    /// The application xaml setup.
    /// </value>
    [Reactive]
    private string _appXamlSetup = """
        <Application
            x:Class="CrissCross.WPF.UI.Gallery.App"
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:local="clr-namespace:CrissCross.WPF.UI.Gallery"
            xmlns:ui="https://github.com/reactivemarbles/CrissCross.ui"
            StartupUri="MainWindow.xaml">
            <Application.Resources>
                <ResourceDictionary>
                    <ResourceDictionary.MergedDictionaries>
                        <ui:ControlsDictionary />
                        <ui:ThemesDictionary Theme="Light" />
                    </ResourceDictionary.MergedDictionaries>
                </ResourceDictionary>
            </Application.Resources>
        </Application>
        """
;

    /// <summary>
    /// Gets the main window xaml setup.
    /// </summary>
    /// <value>
    /// The main window xaml setup.
    /// </value>
    [Reactive]
    private string _mainWindowXamlSetup = """
        <ui:FluentNavigationWindow
            x:Class="CrissCross.WPF.UI.Gallery.MainWindow"
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
            xmlns:local="clr-namespace:CrissCross.WPF.UI.Gallery.ViewModels"
            xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
            xmlns:ui="https://github.com/reactivemarbles/CrissCross.ui"
            x:Name="mainWindow"
            Title="MainWindow"
            Width="800"
            Height="450"
            d:DataContext="{d:DesignInstance Type=local:MainWindowViewModel}"
            x:TypeArguments="local:MainWindowViewModel"
            mc:Ignorable="d">
            <ui:FluentNavigationWindow.LeftContent>
                <StackPanel>
                    <ui:TextBox Margin="3,0,0,0" Text="{Binding Filter, ElementName=NavigationLeft, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}">
                        <ui:TextBox.Icon>
                            <ui:SymbolIcon Symbol="Search20" />
                        </ui:TextBox.Icon>
                    </ui:TextBox>
                    <ui:NavigationVMLeft x:Name="NavigationLeft" />
                </StackPanel>
            </ui:FluentNavigationWindow.LeftContent>
            <Grid>
                <!-- Initial content goes here -->
            </Grid>
        </ui:FluentNavigationWindow>
        """;

    /// <summary>
    /// Gets the main window xaml cs setup.
    /// </summary>
    /// <value>
    /// The main window xaml cs setup.
    /// </value>
    [Reactive]
    private string _mainWindowXamlCsSetup = """
        using System.Reactive.Disposables;
        using System.Windows;
        using CrissCross.WPF.UI.Appearance;
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

                    // Navigate to the main view
                    this.NavigateToView<MainViewModel>();
                });

                // Dispose the view model on close
                Closing += (s, e) => ViewModel.Dispose();
            }
        }
        """;
}
