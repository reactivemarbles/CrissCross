// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.Plot.Test.Views;
using CrissCross.WPF.UI;
using CrissCross.WPF.UI.Controls;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Splat;

namespace CrissCross.WPF.Plot.Test.ViewModels;

/// <summary>MainWindowViewModel member.</summary>
/// <seealso cref="RxObject" />
public partial class MainWindowViewModel : RxObject
{
    /// <summary>Provides access to persisted window tracking.</summary>
    private readonly Tracker? _tracker;

    /// <summary>Provides the application title.</summary>
    [Reactive]
    private string _applicationTitle = "CrissCross UI Plot Test";

    /// <summary>Initializes a new instance of the <see cref="MainWindowViewModel"/> class.</summary>
    public MainWindowViewModel()
    {
        _tracker = new();
        SetupTracker();
        NavigationModels = [];
        var menuIcon = new SymbolIcon(SymbolRegular.LineHorizontal320);
        var homeIcon = new SymbolIcon(SymbolRegular.Home20);
        NavigationModels.AddRange([
            new(null, NavigationModels) { IsExpander = true, Icon = menuIcon },
            new(typeof(MainViewModel), NavigationModels) { Name = "Main", Icon = homeIcon, IsSelected = true },]);

        // Register ViewModels and Views
        AppLocator
            .CurrentMutable.RegisterLazySingletonAnd(static () => new MainViewModel())
            .Register<IViewFor<MainViewModel>>(static () => new MainView());
        AppLocator.CurrentMutable.SetupComplete();
    }

    /// <summary>Gets the navigation models.</summary>
    /// <value>
    /// The navigation models.
    /// </value>
    public List<NavigationModel> NavigationModels { get; }

    /// <summary>Configures persisted main window tracking.</summary>
    private void SetupTracker()
    {
        var windowId = $"[Width={SystemParameters.VirtualScreenWidth},Height={SystemParameters.VirtualScreenHeight}]";
        AppLocator.CurrentMutable.RegisterConstant(_tracker);
        _tracker
            ?.Configure(new TrackingRequest<MainWindow>())
            .Id(static w => w.Name, windowId)
            .Property(static w => w.Height)
            .Property(static w => w.Width)
            .Property(static w => w.Left)
            .Property(static w => w.Top)
            .Property(static w => w.WindowState)
            .PersistOn(static w => nameof(w.Closing))
            .StopTrackingOn(static w => nameof(w.Closing));
    }
}
