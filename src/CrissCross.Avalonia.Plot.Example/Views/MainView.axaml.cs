// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CrissCross.Avalonia.Plot.Example.ViewModels;
using ReactiveUI;

using static ReactiveUI.Binding.ReactiveUIBindingExtensions;

namespace CrissCross.Avalonia.Plot.Example.Views;

/// <summary>Interaction logic for MainView.axaml.</summary>
public partial class MainView : UserControl, IDisposable, IViewFor<MainViewModel>
{
    /// <summary>Identifies the view model dependency property.</summary>
    public static readonly StyledProperty<MainViewModel?> ViewModelProperty =
        AvaloniaProperty.Register<MainView, MainViewModel?>(nameof(ViewModel));

    /// <summary>Stores the bindings owned by the current visual-tree lifetime.</summary>
    private CompositeDisposable? _viewBindings;

    /// <summary>Initializes a new instance of the <see cref="MainView"/> class.</summary>
    public MainView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>Gets the binding root view model.</summary>
    public MainViewModel? BindingRoot => ViewModel;

    /// <summary>Gets or sets the view model.</summary>
    public MainViewModel? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (MainViewModel?)value;
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

    /// <summary>Registers bindings after Avalonia has completed view construction.</summary>
    /// <param name="sender">The loaded view.</param>
    /// <param name="e">The routed event data.</param>
    private void OnLoaded(object? sender, RoutedEventArgs e)
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
    private void OnUnloaded(object? sender, RoutedEventArgs e)
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
        var viewModel = ViewModel;
        if (viewModel is null)
        {
            return;
        }

        _ = viewModel
            .WhenAnyValue(static vm => vm.NumberPointsPlotted)
            .Where(static numberOfPoints => numberOfPoints.HasValue)
            .Select(static numberOfPoints => Math.Max(1, Convert.ToInt32(numberOfPoints.GetValueOrDefault())))
            .BindTo(this, static v => v.Chart.NumberPointsPlotted)
            .DisposeWith(disposables);
        _ = viewModel
            .ReactivePlotSources.ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(sources => Chart.ReactivePlotSources = sources)
            .DisposeWith(disposables);
        _ = this.WhenAnyValue(static x => x.Chart.ViewModel)
            .BindTo(viewModel, static vm => vm.LiveChartViewModel)
            .DisposeWith(disposables);
    }
}
