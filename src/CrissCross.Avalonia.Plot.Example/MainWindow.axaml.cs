// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;
using CrissCross.Avalonia.Plot.Example.ViewModels;

namespace CrissCross.Avalonia.Plot.Example;

/// <summary>Hosts the plotting example and its navigation controls.</summary>
public sealed partial class MainWindow : Window, IDisposable
{
    /// <summary>Provides the collapsed navigation width.</summary>
    private const double CollapsedNavigationWidth = 48;

    /// <summary>Provides the expanded navigation width.</summary>
    private const double ExpandedNavigationWidth = 160;

    /// <summary>Owns the example's data streams and commands.</summary>
    private readonly MainViewModel _viewModel = new();

    /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
    public MainWindow()
    {
        InitializeComponent();
        PlotView.ViewModel = _viewModel;
        ToggleNavigationButton.Click += (_, _) =>
        {
            var expanded = NavigationPanel.Width > CollapsedNavigationWidth;
            NavigationPanel.Width = expanded ? CollapsedNavigationWidth : ExpandedNavigationWidth;
            NavigationSearch.IsVisible = !expanded;
            MainNavigationButton.Content = expanded ? "⌂" : "⌂  Main";
        };
        NavigationSearch.TextChanged += (_, _) => MainNavigationButton.IsVisible =
            "Main".Contains(NavigationSearch.Text ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        MainNavigationButton.Click += (_, _) => PlotView.IsVisible = true;
        Closed += (_, _) => Dispose();
    }

    /// <summary>Releases the view bindings and live demo streams.</summary>
    public void Dispose()
    {
        PlotView.Dispose();
        _viewModel.Dispose();
    }
}
