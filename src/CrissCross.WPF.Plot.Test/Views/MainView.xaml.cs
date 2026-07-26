// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.Plot.Test.ViewModels;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Splat;

namespace CrissCross.WPF.Plot.Test.Views;

/// <summary>Interaction logic for MainView.xaml.</summary>
[IViewFor<MainViewModel>]
public partial class MainView : IDisposable
{
    /// <summary>Stores the bindings owned by the current visual-tree lifetime.</summary>
    private CompositeDisposable? _viewBindings;

    /// <summary>Initializes a new instance of the <see cref="MainView"/> class.</summary>
    public MainView()
    {
        InitializeComponent();
        ViewModel = AppLocator.Current.GetService<MainViewModel>()!;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>Releases the view's event handlers and active bindings.</summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases resources owned by the view.</summary>
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

    /// <summary>Registers bindings after WPF has completed view construction.</summary>
    /// <param name="sender">The loaded view.</param>
    /// <param name="e">The routed event data.</param>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_viewBindings is not null)
        {
            return;
        }

        _viewBindings = new();
        BindChartProperties(_viewBindings);
        BindCommands(_viewBindings);
        BindPlotData(_viewBindings);
    }

    /// <summary>Releases view bindings when navigation removes the view from the visual tree.</summary>
    /// <param name="sender">The unloaded view.</param>
    /// <param name="e">The routed event data.</param>
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _viewBindings?.Dispose();
        _viewBindings = null;
    }

    /// <summary>Binds chart settings and status text.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void BindChartProperties(CompositeDisposable disposables)
    {
        _ = this.OneWayBind(ViewModel, static vm => vm.YAxisNames, static v => v.Chart.YAxisName)
            .DisposeWith(disposables);
        _ = this.OneWayBind(ViewModel, static vm => vm.ActiveScenario, static v => v.ActiveScenarioText.Text)
            .DisposeWith(disposables);
        _ = this.OneWayBind(ViewModel, static vm => vm.ThemeButtonText, static v => v.ToggleThemeButton.Content)
            .DisposeWith(disposables);
        _ = this.Bind(
                ViewModel,
                static vm => vm.UseFixedNumberOfPoints,
                static v => v.CheckBoxUseFixedNumberOfPoints.IsChecked)
            .DisposeWith(disposables);
        _ = this.Bind(ViewModel, static vm => vm.UseFixedNumberOfPoints, static v => v.Chart.UseFixedNumberOfPoints)
            .DisposeWith(disposables);
        _ = this.Bind(ViewModel, static vm => vm.NumberPointsPlotted, static v => v.NumberPointsPlotted.Value)
            .DisposeWith(disposables);
    }

    /// <summary>Binds scenario and theme commands.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void BindCommands(CompositeDisposable disposables)
    {
        _ = this.BindCommand(
                ViewModel,
                static viewModel => viewModel.ShowAllChartTypesCommand,
                static view => view.ShowAllChartTypesButton)
            .DisposeWith(disposables);
        _ = this.BindCommand(
                ViewModel,
                static viewModel => viewModel.ShowLiveCommand,
                static view => view.ShowLiveButton)
            .DisposeWith(disposables);
        _ = this.BindCommand(
                ViewModel,
                static viewModel => viewModel.ShowHistoricCommand,
                static view => view.ShowHistoricButton)
            .DisposeWith(disposables);
        _ = this.BindCommand(
                ViewModel,
                static viewModel => viewModel.ShowIndicatorsCommand,
                static view => view.ShowIndicatorsButton)
            .DisposeWith(disposables);
        _ = this.BindCommand(
                ViewModel,
                static viewModel => viewModel.ToggleThemeCommand,
                static view => view.ToggleThemeButton)
            .DisposeWith(disposables);
    }

    /// <summary>Binds plot point limits, sources, and the live chart view model.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void BindPlotData(CompositeDisposable disposables)
    {
        _ = ViewModel
            .WhenAnyValue(static vm => vm.NumberPointsPlotted)
            .Where(static numberOfPoints => numberOfPoints.HasValue)
            .Select(static numberOfPoints => Math.Max(1, Convert.ToInt32(numberOfPoints.GetValueOrDefault())))
            .BindTo(this, static v => v.Chart.NumberPointsPlotted)
            .DisposeWith(disposables);
        _ = ViewModel
            .ReactivePlotSources.ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(sources => Chart.ReactivePlotSources = sources)
            .DisposeWith(disposables);
        _ = this.WhenAnyValue(static x => x.Chart.ViewModel)
            .BindTo(ViewModel, static vm => vm.LiveChartViewModel)
            .DisposeWith(disposables);
    }
}
