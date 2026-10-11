// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Text;
using ScottPlot;
using ScottPlot.Plottables;

namespace CrissCross.Maui.Plot.Example;

/// <summary>Runs an opt-in showcase against the native chart and saves its actual rendering artifacts.</summary>
public sealed partial class MainPage
{
    /// <summary>The default demo retention budget.</summary>
    private const int DefaultPointBudget = 600;

    /// <summary>The fixed-window retention budget exercised by the showcase.</summary>
    private const int BoundedPointBudget = 12;

    /// <summary>The expected all-types series count.</summary>
    private const int AllTypeSeriesCount = 11;

    /// <summary>The expected live series count.</summary>
    private const int LiveSeriesCount = 5;

    /// <summary>The expected price and indicator series count.</summary>
    private const int IndicatorSeriesCount = 15;

    /// <summary>The time allowed for native live samples to arrive.</summary>
    private const int ScenarioWaitSeconds = 2;

    /// <summary>The historic reduction budget.</summary>
    private const int HistoricPointBudget = 1500;

    /// <summary>The rendered showcase image width.</summary>
    private const int PreviewWidth = 1200;

    /// <summary>The rendered showcase image height.</summary>
    private const int PreviewHeight = 700;

    /// <summary>Cycles chart scenarios on the UI thread and exports each native chart model's rendered output.</summary>
    /// <param name="outputDirectory">The directory in which to write PNGs and the verification report.</param>
    /// <returns>A task that completes after all showcase scenarios have rendered and passed their checks.</returns>
    public async Task VerifyScenariosAsync(string outputDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);
        _ = Directory.CreateDirectory(outputDirectory);
        var report = new StringBuilder("CrissCross native MAUI plot showcase\n");
        _ = report.AppendLine(_timeProvider.GetUtcNow().ToString("O", CultureInfo.InvariantCulture));
        await File.WriteAllTextAsync(Path.Combine(outputDirectory, "showcase.txt"), "RUNNING: verification has not completed.\n");
        var originalSources = _viewModel.ActiveSources;
        var originalScenario = _viewModel.ActiveScenario;
        var originalLightTheme = _viewModel.IsLightTheme;
        var originalFixedWindow = _chart.UseFixedNumberOfPoints;
        var originalPointBudget = _chart.NumberPointsPlotted;
        try
        {
            _viewModel.IsLightTheme = false;
            _chart.ViewModel.ApplyTheme(ReactivePlotTheme.Dark);
            _chart.UseFixedNumberOfPoints = false;
            _chart.NumberPointsPlotted = DefaultPointBudget;
            _chart.OpenPlotSettings();
            await CaptureScenarioAsync("all-types", _viewModel.ShowAllChartTypesCommand, outputDirectory, report, AllTypeSeriesCount);
            _chart.OpenPlotSettings();
            await CaptureScenarioAsync("live", _viewModel.ShowLiveCommand, outputDirectory, report, LiveSeriesCount);
            _chart.UseFixedNumberOfPoints = true;
            _chart.NumberPointsPlotted = BoundedPointBudget;
            await CaptureScenarioAsync("live-12-points", _viewModel.ShowLiveCommand, outputDirectory, report, LiveSeriesCount);
            _chart.UseFixedNumberOfPoints = false;
            _chart.NumberPointsPlotted = DefaultPointBudget;
            await CaptureScenarioAsync("historic-datetime", _viewModel.ShowHistoricCommand, outputDirectory, report, 1);
            await CaptureScenarioAsync("indicators-dark", _viewModel.ShowIndicatorsCommand, outputDirectory, report, IndicatorSeriesCount);
            using var themeExecution = _viewModel.ToggleThemeCommand.Execute().Subscribe();
            await CaptureScenarioAsync("indicators-light", _viewModel.ShowIndicatorsCommand, outputDirectory, report, IndicatorSeriesCount);
            _ = report.AppendLine("PASS: every scenario passed series, data, theme, axis and retention checks.");
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "showcase.txt"), report.ToString());
        }
        finally
        {
            _chart.UseFixedNumberOfPoints = originalFixedWindow;
            _chart.NumberPointsPlotted = originalPointBudget;
            _viewModel.IsLightTheme = originalLightTheme;
            _viewModel.ThemeButtonText = originalLightTheme ? "Use dark theme" : "Use light theme";
            _viewModel.ActiveScenario = originalScenario;
            _viewModel.ActiveSources = originalSources;
            _chart.ViewModel.ApplyTheme(originalLightTheme ? ReactivePlotTheme.Light : ReactivePlotTheme.Dark);
        }

        _scenario.Text = $"Showcase passed; saved to {outputDirectory}";
    }

    /// <summary>Reads the opt-in desktop verification argument or environment variable.</summary>
    /// <returns>The requested output directory and an optional argument validation error.</returns>
    private static (string? Directory, string? Error) GetVerificationOutputDirectory()
    {
        var arguments = Environment.GetCommandLineArgs();
        var optionIndex = Array.IndexOf(arguments, "--verify-directory");
        if (optionIndex < 0)
        {
            return (Environment.GetEnvironmentVariable("CRISSCROSS_PLOT_VERIFY_DIR"), null);
        }

        return optionIndex + 1 >= arguments.Length || string.IsNullOrWhiteSpace(arguments[optionIndex + 1]) || arguments[optionIndex + 1].StartsWith("--", StringComparison.Ordinal)
            ? (null, "Showcase needs a directory after --verify-directory.")
            : (arguments[optionIndex + 1], null);
    }

    /// <summary>Returns the number of data points held by a native rendered series.</summary>
    /// <param name="plottable">The rendered plot object.</param>
    /// <returns>The sample count, or null for non-data decorations.</returns>
    private static int? GetPointCount(IPlottable plottable) => plottable switch
    {
        Scatter scatter => scatter.Data.GetScatterPoints().Count,
        ScottPlot.Plottables.Signal signal => signal.Data.GetYs().Count,
        SignalXY signal => signal.Data.Count,
        DataLogger logger => logger.Data.Coordinates.Count,
        DataStreamer streamer => Math.Min(streamer.CountTotal, streamer.Count),
        BarPlot bars => bars.Bars.Count,
        LollipopPlot stem => CountStemPoints(stem),
        _ => null,
    };

    /// <summary>Counts a stem series without materializing its coordinates.</summary>
    /// <param name="stem">The native stem series.</param>
    /// <returns>The point count.</returns>
    private static int CountStemPoints(LollipopPlot stem)
    {
        var count = 0;
        foreach (var coordinate in stem.Coordinates)
        {
            count++;
        }

        return count;
    }

    /// <summary>Collects native data counts, excluding plot decorations.</summary>
    /// <param name="plot">The native rendered plot.</param>
    /// <returns>The data point counts.</returns>
    private static List<int> CollectPointCounts(ScottPlot.Plot plot)
    {
        List<int> samples = [];
        foreach (var plottable in plot.GetPlottables())
        {
            if (GetPointCount(plottable) is { } count)
            {
                samples.Add(count);
            }
        }

        return samples;
    }

    /// <summary>Renders and verifies one scenario after native chart subscriptions have received data.</summary>
    /// <param name="name">The artifact name.</param>
    /// <param name="command">The scenario command.</param>
    /// <param name="outputDirectory">The artifact directory.</param>
    /// <param name="report">The verification report.</param>
    /// <param name="expectedSeriesCount">The independently specified number of series.</param>
    /// <returns>A task that completes after the scenario has rendered.</returns>
    private async Task CaptureScenarioAsync(
        string name,
        ReactiveUI.ReactiveCommand<Unit, Unit> command,
        string outputDirectory,
        StringBuilder report,
        int expectedSeriesCount)
    {
        using var execution = command.Execute().Subscribe();
        await Task.Delay(TimeSpan.FromSeconds(ScenarioWaitSeconds));
        var chart = _chart.ViewModel ?? throw new InvalidOperationException("The native chart has no view model.");
        var nativePlot = chart.MauiPlot1vm ?? throw new InvalidOperationException("The native plot host is unavailable.");
        var samples = CollectPointCounts(nativePlot.Plot);
        ValidateSeries(name, samples, expectedSeriesCount);
        ValidateAxisAndTheme(name, samples);

        var imagePath = Path.Combine(outputDirectory, $"{name}.png");
        _ = nativePlot.Plot.SavePng(imagePath, PreviewWidth, PreviewHeight);
        if (new FileInfo(imagePath).Length < 1000)
        {
            throw new InvalidOperationException($"{name}: the chart rendering artifact is empty.");
        }

        _ = report.Append(name).Append(": PASS; series=")
            .Append(chart.SeriesSettings.Count.ToString(CultureInfo.InvariantCulture))
            .Append("; points=[").AppendJoin(',', samples).Append(']')
            .Append("; fixed-window=").Append(_chart.UseFixedNumberOfPoints)
            .Append("; point-limit=").Append(_chart.NumberPointsPlotted.ToString(CultureInfo.InvariantCulture))
            .Append("; datetime-axis=").Append(chart.IsXAxisDateTime)
            .Append("; background=").Append(chart.CurrentTheme.FigureBackground)
            .AppendLine();
    }

    /// <summary>Verifies source counts, nonempty data, chart-type coverage and bounded live retention.</summary>
    /// <param name="name">The scenario name.</param>
    /// <param name="samples">The native data counts.</param>
    /// <param name="expectedSeriesCount">The independent expected series count.</param>
    private void ValidateSeries(string name, List<int> samples, int expectedSeriesCount)
    {
        var chart = _chart.ViewModel;
        if (chart.SeriesSettings.Count != expectedSeriesCount || samples.Count != expectedSeriesCount)
        {
            throw new InvalidOperationException($"{name}: expected {expectedSeriesCount} nonempty series, found {chart.SeriesSettings.Count} settings and {samples.Count} data plottables.");
        }

        foreach (var count in samples)
        {
            if (count < 1 || (_chart.UseFixedNumberOfPoints && count > _chart.NumberPointsPlotted))
            {
                throw new InvalidOperationException($"{name}: invalid sample count {count} for the configured retention budget.");
            }
        }

        if (name != "all-types")
        {
            return;
        }

        ValidateChartTypes();
    }

    /// <summary>Verifies that the actual settings cover every supported plot type.</summary>
    private void ValidateChartTypes()
    {
        HashSet<PlotType> types = [];
        foreach (var settings in _chart.ViewModel.SeriesSettings)
        {
            _ = types.Add(settings.PlotType);
        }

        foreach (var type in Enum.GetValues<PlotType>())
        {
            if (!types.Contains(type))
            {
                throw new InvalidOperationException($"The all-types scenario is missing {type}.");
            }
        }
    }

    /// <summary>Verifies the actual native axis mode, historic reduction and theme.</summary>
    /// <param name="name">The scenario name.</param>
    /// <param name="samples">The native data counts.</param>
    private void ValidateAxisAndTheme(string name, List<int> samples)
    {
        var chart = _chart.ViewModel;
        var historic = name == "historic-datetime";
        if (chart.IsXAxisDateTime != historic || (historic && samples[0] > HistoricPointBudget))
        {
            throw new InvalidOperationException($"{name}: the X-axis mode or historic reduction budget is incorrect.");
        }

        var expectedTheme = name == "indicators-light" ? ReactivePlotTheme.Light : ReactivePlotTheme.Dark;
        if (chart.CurrentTheme == expectedTheme)
        {
            return;
        }

        throw new InvalidOperationException($"{name}: the native chart theme is incorrect.");
    }
}
