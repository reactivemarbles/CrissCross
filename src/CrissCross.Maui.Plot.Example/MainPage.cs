// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using CrissCross.Maui.Plot.Example.ViewModels;
using ReactiveUI;

using static ReactiveUI.Binding.ReactiveUIBindingExtensions;

namespace CrissCross.Maui.Plot.Example;

/// <summary>Provides native scenario navigation and controls around the MAUI live chart.</summary>
public sealed partial class MainPage : ContentPage, IDisposable
{
    /// <summary>The spacing between native toolbar controls.</summary>
    private const int ControlSpacing = 6;

    /// <summary>The page padding.</summary>
    private const int PagePadding = 12;

    /// <summary>The vertical page spacing.</summary>
    private const int PageSpacing = 8;

    /// <summary>The numeric entry width.</summary>
    private const int PointEntryWidth = 100;

    /// <summary>The chart layout row.</summary>
    private const int ChartRow = 2;

    /// <summary>Owns the generated chart data and reactive commands.</summary>
    private readonly MainViewModel _viewModel = new();

    /// <summary>Owns subscriptions to the chart scenario state.</summary>
    private readonly CompositeDisposable _subscriptions = new();

    /// <summary>Displays the active scenario.</summary>
    private readonly Label _scenario = new() { FontSize = 20, FontAttributes = FontAttributes.Bold };

    /// <summary>Hosts the native ScottPlot chart and settings controls.</summary>
    private readonly LiveChart _chart = new() { AutomationId = "LiveChart" };

    /// <summary>Provides the verification artifact clock.</summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>Tracks whether the page resources have been released.</summary>
    private bool _disposed;

    /// <summary>Initializes a new instance of the <see cref="MainPage"/> class.</summary>
    public MainPage()
        : this(TimeProvider.System)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="MainPage"/> class with a supplied clock.</summary>
    /// <param name="timeProvider">The clock used to timestamp verification reports.</param>
    public MainPage(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        _timeProvider = timeProvider;
        Title = "MAUI plotting showcase";
        Content = CreateLayout(CreateControls());
        _chart.YAxisName = _viewModel.YAxisNames;
        _viewModel.LiveChartViewModel = _chart.ViewModel;
        _ = _viewModel.ReactivePlotSources.Subscribe(sources => _chart.ReactivePlotSources = sources)
            .DisposeWith(_subscriptions);
        _ = _viewModel.WhenAnyValue(static model => model.ActiveScenario)
            .Subscribe(value => _scenario.Text = value).DisposeWith(_subscriptions);
    }

    /// <summary>Releases chart subscriptions and demo streams when the application window closes.</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _verificationLifetime.Cancel();
        _verificationLifetime.Dispose();
        _subscriptions.Dispose();
        _chart.Dispose();
        _viewModel.Dispose();
    }

    /// <summary>Creates an accessible native scenario button.</summary>
    /// <param name="text">The button caption.</param>
    /// <param name="automationId">The stable native automation identifier.</param>
    /// <param name="command">The command invoked by the button.</param>
    /// <returns>The native button.</returns>
    private static Button CreateButton(string text, string automationId, ReactiveCommand<Unit, Unit> command) => new()
    {
        Text = text,
        AutomationId = automationId,
        Command = command,
        Margin = new(0, 0, ControlSpacing, ControlSpacing),
    };

    /// <summary>Creates the page's accessible scenario controls.</summary>
    /// <returns>The wrapping native toolbar.</returns>
    private FlexLayout CreateControls()
    {
        var controls = new FlexLayout { Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap, AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center };
        controls.Children.Add(CreateButton("All chart types", "AllChartTypes", _viewModel.ShowAllChartTypesCommand));
        controls.Children.Add(CreateButton("Live streams", "LiveStreams", _viewModel.ShowLiveCommand));
        controls.Children.Add(CreateButton("Historic DateTime", "HistoricDateTime", _viewModel.ShowHistoricCommand));
        controls.Children.Add(CreateButton("Indicators", "Indicators", _viewModel.ShowIndicatorsCommand));
        var theme = CreateButton("Use light theme", "ToggleTheme", _viewModel.ToggleThemeCommand);
        controls.Children.Add(theme);
        _ = _viewModel.WhenAnyValue(static model => model.ThemeButtonText)
            .Subscribe(value => theme.Text = value).DisposeWith(_subscriptions);
        AddVerificationControl(controls);
        AddPointControls(controls);
        return controls;
    }

    /// <summary>Creates the native page layout.</summary>
    /// <param name="controls">The wrapping toolbar.</param>
    /// <returns>The layout containing title, toolbar and native plot.</returns>
    private Grid CreateLayout(FlexLayout controls)
    {
        var grid = new Grid { Padding = PagePadding, RowSpacing = PageSpacing, RowDefinitions = { new(GridLength.Auto), new(GridLength.Auto), new(GridLength.Star) } };
        grid.Add(_scenario, 0, 0);
        grid.Add(controls, 0, 1);
        grid.Add(_chart, 0, ChartRow);
        return grid;
    }

    /// <summary>Adds the native rolling-window and retention controls.</summary>
    /// <param name="controls">The toolbar receiving the controls.</param>
    private void AddPointControls(FlexLayout controls)
    {
        var fixedWindow = new Switch { AutomationId = "FixedPointWindow" };
        fixedWindow.Toggled += (_, e) =>
        {
            _viewModel.UseFixedNumberOfPoints = e.Value;
            _chart.UseFixedNumberOfPoints = e.Value;
        };
        controls.Children.Add(new Label { Text = "Fixed point window", VerticalOptions = LayoutOptions.Center, Margin = PageSpacing });
        controls.Children.Add(fixedWindow);
        var pointLimit = new Entry { Text = "600", Keyboard = Keyboard.Numeric, WidthRequest = PointEntryWidth, AutomationId = "PointLimit" };
        pointLimit.TextChanged += (_, e) =>
        {
            if (!int.TryParse(e.NewTextValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count) || count <= 0)
            {
                return;
            }

            _viewModel.NumberPointsPlotted = count;
            _chart.NumberPointsPlotted = count;
        };
        controls.Children.Add(pointLimit);
    }

    /// <summary>Adds the native showcase button and opt-in desktop launch behavior.</summary>
    /// <param name="controls">The toolbar receiving the showcase control.</param>
    private void AddVerificationControl(FlexLayout controls)
    {
        var verify = new Button { Text = "Run showcase", AutomationId = "RunShowcase", Margin = new(0, 0, ControlSpacing, ControlSpacing) };
        verify.Clicked += async (_, _) => await RunVerificationAsync(verify, Path.Combine(FileSystem.AppDataDirectory, "PlotVerification"));
        controls.Children.Add(verify);
        Loaded += async (_, _) =>
        {
            var (requestedOutput, optionError) = GetVerificationOutputDirectory();
            if (optionError is not null)
            {
                _scenario.Text = optionError;
                return;
            }

            if (string.IsNullOrWhiteSpace(requestedOutput) || !verify.IsEnabled || _automaticVerificationStarted)
            {
                return;
            }

            _automaticVerificationStarted = true;
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "--verify-responsiveness") >= 0)
            {
                await RunVerificationAsync(verify, requestedOutput, responsiveness: true);
                return;
            }

            await RunVerificationAsync(verify, requestedOutput);
        };
    }

    /// <summary>Runs the opt-in showcase and records any failure without ending the interactive application.</summary>
    /// <param name="button">The initiating native button.</param>
    /// <param name="outputDirectory">The requested artifact directory.</param>
    /// <param name="responsiveness">Whether to run the dispatcher soak instead of the rendered showcase.</param>
    /// <returns>The asynchronous showcase.</returns>
    private async Task RunVerificationAsync(Button button, string outputDirectory, bool responsiveness = false)
    {
        button.IsEnabled = false;
        try
        {
            if (responsiveness)
            {
                await VerifyResponsivenessAsync(outputDirectory);
                _scenario.Text = $"Responsiveness verification completed; see {outputDirectory}";
            }
            else
            {
                await VerifyScenariosAsync(outputDirectory);
            }
        }
        catch (Exception error)
        {
            _scenario.Text = $"Showcase failed: {error.Message}";
            _ = Directory.CreateDirectory(outputDirectory);
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "showcase-failed.txt"), error.ToString());
        }
        finally
        {
            button.IsEnabled = true;
        }
    }
}
