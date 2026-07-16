// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Security.Cryptography;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.Plot.Test.ViewModels;

/// <summary>MainViewModel member.</summary>
/// <seealso cref="RxObject" />
public partial class MainViewModel : RxObject
{
    /// <summary>Provides the number of points retained by the demo streams.</summary>
    private const int DemoMaxPoints = 600;

    /// <summary>Provides the byte count required to read a random unsigned long.</summary>
    private const int RandomUInt64ByteCount = sizeof(ulong);

    /// <summary>Provides the byte offset used when reading the random unsigned long.</summary>
    private const int RandomUInt64StartIndex = 0;

    /// <summary>Provides the index of the oldest demo point in a rolling buffer.</summary>
    private const int OldestDemoPointIndex = 0;

    /// <summary>Provides the maximum value emitted by the random logger demo.</summary>
    private const double RandomLoggerMaximumValue = 100.0;

    /// <summary>Provides the interval between demo stream updates.</summary>
    private const int DemoStreamIntervalMilliseconds = 250;

    /// <summary>Provides the X-axis scale used by the demo data.</summary>
    private const double DemoPlotXScale = 10.0;

    /// <summary>Provides the common baseline used by the demo wave data.</summary>
    private const double DemoWaveBaseline = 50.0;

    /// <summary>Provides the signal wave period divisor.</summary>
    private const double SignalWavePeriod = 8.0;

    /// <summary>Provides the scatter wave period divisor.</summary>
    private const double ScatterWavePeriod = 9.0;

    /// <summary>Provides the streamer wave period divisor.</summary>
    private const double StreamerWavePeriod = 5.0;

    /// <summary>Provides the signal XY wave period divisor.</summary>
    private const double SignalXyWavePeriod = 12.0;

    /// <summary>Provides the common amplitude used by most demo wave data.</summary>
    private const double DemoWaveAmplitude = 35.0;

    /// <summary>Provides the amplitude used by the streamer demo wave.</summary>
    private const double StreamerWaveAmplitude = 45.0;

    /// <summary>Provides the signal axis index.</summary>
    private const int SignalAxisIndex = 0;

    /// <summary>Provides the scatter axis index.</summary>
    private const int ScatterAxisIndex = 1;

    /// <summary>Provides the logger axis index.</summary>
    private const int LoggerAxisIndex = 2;

    /// <summary>Provides the streamer axis index.</summary>
    private const int StreamerAxisIndex = 3;

    /// <summary>Provides the signal XY axis index.</summary>
    private const int SignalXyAxisIndex = 4;

    /// <summary>Provides signal demo points.</summary>
    private readonly Signal<(string? Name, IList<double>? Value, IList<double> X, int Axis)> _signalPoints = new();

    /// <summary>Provides scatter demo points.</summary>
    private readonly Signal<(string? Name, IList<double>? X, IList<double> Y, int Axis)> _scatterPoints = new();

    /// <summary>Provides data logger demo points.</summary>
    private readonly Signal<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)> _dataLoggerPoints = new();

    /// <summary>Provides streamer demo points.</summary>
    private readonly Signal<(string? Name, IList<double>? Y, IList<double> X, int Axis)> _streamerPoints = new();

    /// <summary>Provides signal XY demo points.</summary>
    private readonly Signal<(string? Name, IList<double>? Y, IList<double> X, int Axis)> _signalXyPoints = new();

    /// <summary>Stores scatter X history for the rolling demo window.</summary>
    private readonly List<double> _scatterXHistory = [];

    /// <summary>Stores scatter Y history for the rolling demo window.</summary>
    private readonly List<double> _scatterYHistory = [];

    /// <summary>Stores signal XY X history for the rolling demo window.</summary>
    private readonly List<double> _signalXyXHistory = [];

    /// <summary>Stores signal XY Y history for the rolling demo window.</summary>
    private readonly List<double> _signalXyYHistory = [];

    /// <summary>Provides the active demo stream subscription.</summary>
    private readonly SerialDisposable _demoStream = new();

    /// <summary>Stores all live example sources.</summary>
    private readonly IReadOnlyList<IReactivePlotSource> _liveSources;

    /// <summary>Stores sources that demonstrate every supported chart type.</summary>
    private readonly IReadOnlyList<IReactivePlotSource> _allChartSources;

    /// <summary>Stores the reduced long-span DateTime example sources.</summary>
    private readonly IReadOnlyList<IReactivePlotSource> _historicSources;

    /// <summary>Stores all technical indicator example sources.</summary>
    private readonly IReadOnlyList<IReactivePlotSource> _indicatorSources;

    /// <summary>Tracks whether the reactive demo streams have started.</summary>
    private bool _demoStreamsStarted;

    /// <summary>Provides the live chart view model.</summary>
    [Reactive]
    private LiveChartViewModel? _liveChartViewModel;

    /// <summary>Stores the currently selected example sources.</summary>
    [Reactive]
    private IReadOnlyList<IReactivePlotSource> _activeSources = [];

    /// <summary>Stores the active example title.</summary>
    [Reactive]
    private string _activeScenario = "All chart types";

    /// <summary>Stores the theme command caption.</summary>
    [Reactive]
    private string _themeButtonText = "Use light theme";

    /// <summary>Stores whether the plot is using its light theme.</summary>
    [Reactive]
    private bool _isLightTheme;

    /// <summary>Stores whether the example keeps a fixed rolling point window.</summary>
    [Reactive]
    private bool _useFixedNumberOfPoints;

    /// <summary>Stores the maximum number of points displayed by the example.</summary>
    [Reactive]
    private double? _numberPointsPlotted = DemoMaxPoints;

    /// <summary>Initializes a new instance of the <see cref="MainViewModel"/> class.</summary>
    public MainViewModel()
    {
        YAxisNames = (
            ["[Live]", "[Static XY]", "[Historic]", "[Studies]", "[Oscillator]", "[Bars]"],
            ["#377eb8", "#ff7f00", "#4daf4a", "#984ea3", "#e41a1c", "#a65628"]);
        _liveSources =
        [
            ReactivePlotSource.FromSignalPoints(_signalPoints),
            ReactivePlotSource.FromScatterPoints(_scatterPoints),
            ReactivePlotSource.FromDataLoggerPoints(_dataLoggerPoints),
            ReactivePlotSource.FromStreamerPoints(_streamerPoints),
            ReactivePlotSource.FromSignalXyPoints(_signalXyPoints),];
        _allChartSources = CreateAllChartTypeSources(_liveSources);
        _historicSources = CreateHistoricSources();
        _indicatorSources = CreateIndicatorSources();
        ActiveSources = _allChartSources;
        ReactivePlotSources = this.WhenAnyValue(viewModel => viewModel.ActiveSources).Select(static sources => sources);

        ShowAllChartTypesCommand = ReactiveCommand.Create(() => SelectScenario("All chart types", _allChartSources));
        ShowLiveCommand = ReactiveCommand.Create(() => SelectScenario("Live reactive streams", _liveSources));
        ShowHistoricCommand = ReactiveCommand.Create(() =>
            SelectScenario("Large-span DateTime history", _historicSources));
        ShowIndicatorsCommand = ReactiveCommand.Create(() => SelectScenario("Technical indicators", _indicatorSources));
        ToggleThemeCommand = ReactiveCommand.Create(ToggleTheme);
        _ = ShowAllChartTypesCommand.DisposeWith(Disposables);
        _ = ShowLiveCommand.DisposeWith(Disposables);
        _ = ShowHistoricCommand.DisposeWith(Disposables);
        _ = ShowIndicatorsCommand.DisposeWith(Disposables);
        _ = ToggleThemeCommand.DisposeWith(Disposables);

        LiveChartSubject = [_signalPoints];
        _ = this.WhenAnyValue(vm => vm.LiveChartViewModel)
            .Where(static liveChartViewModel => liveChartViewModel is not null)
            .Select(static liveChartViewModel => liveChartViewModel!)
            .Subscribe(_ => StartReactiveDemoStreams());
    }

    /// <summary>Gets all live reactive plot sources used by the example chart.</summary>
    public IObservable<IReadOnlyList<IReactivePlotSource>> ReactivePlotSources { get; }

    /// <summary>Gets the command that displays every supported chart type.</summary>
    public ReactiveCommand<Unit, Unit> ShowAllChartTypesCommand { get; }

    /// <summary>Gets the command that displays rolling observable streams.</summary>
    public ReactiveCommand<Unit, Unit> ShowLiveCommand { get; }

    /// <summary>Gets the command that displays reduced long-span DateTime history.</summary>
    public ReactiveCommand<Unit, Unit> ShowHistoricCommand { get; }

    /// <summary>Gets the command that displays technical indicator overlays.</summary>
    public ReactiveCommand<Unit, Unit> ShowIndicatorsCommand { get; }

    /// <summary>Gets the command that switches the plot between its dark and light themes.</summary>
    public ReactiveCommand<Unit, Unit> ToggleThemeCommand { get; }

    /// <summary>Gets the legacy live chart subject retained for backwards-compatible examples.</summary>
    public IEnumerable<Signal<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>> LiveChartSubject
    {
        get;
        private set;
    }

    /// <summary>Gets the y axis names.</summary>
    /// <value>
    /// The y axis names.
    /// </value>
    public (IList<string> yNames, IList<string> hexColors) YAxisNames { get; private set; }

    /// <summary>Releases managed demo subjects.</summary>
    /// <param name="disposing">true to release managed resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _demoStream.Dispose();
            _signalPoints.Dispose();
            _scatterPoints.Dispose();
            _dataLoggerPoints.Dispose();
            _streamerPoints.Dispose();
            _signalXyPoints.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>Gets the next random value for the demo logger.</summary>
    /// <returns>The next random value.</returns>
    private static double NextRandomValue()
    {
        var bytes = new byte[RandomUInt64ByteCount];
#if NET472 || NET48 || NET481
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(bytes);
#else
        RandomNumberGenerator.Fill(bytes);
#endif
        return BitConverter.ToUInt64(bytes, RandomUInt64StartIndex) / (double)ulong.MaxValue * RandomLoggerMaximumValue;
    }

    /// <summary>Adds a value to a rolling demo point buffer.</summary>
    /// <param name="values">The rolling values.</param>
    /// <param name="value">The value to append.</param>
    private static void AddDemoPoint(List<double> values, double value)
    {
        values.Add(value);
        if (values.Count <= DemoMaxPoints)
        {
            return;
        }

        values.RemoveAt(OldestDemoPointIndex);
    }

    /// <summary>Creates a scenario containing all supported normalized plot types.</summary>
    /// <param name="liveSources">The existing live source adapters.</param>
    /// <returns>The complete source collection.</returns>
    private static IReadOnlyList<IReactivePlotSource> CreateAllChartTypeSources(
        IReadOnlyList<IReactivePlotSource> liveSources)
    {
        var samples = CreateChartTypeSamples();
        return [.. liveSources, .. CreateContinuousChartTypeSources(samples), .. CreateSparseChartTypeSources(samples)];
    }

    /// <summary>Creates the continuous line, step, and area examples.</summary>
    /// <param name="samples">The shared static sample data.</param>
    /// <returns>The continuous plot sources.</returns>
    private static IReadOnlyList<IReactivePlotSource> CreateContinuousChartTypeSources(ChartTypeSamples samples)
    {
        const int primaryAxis = 1;
        const float lineWidth = 2;
        const double areaOffset = 20;
        return
        [
            ReactivePlotSource.FromStatic(
                PlotSeriesData.Numeric("Line", primaryAxis, samples.X, samples.Wave),
                PlotType.Line,
                new() { Color = "#42A5F5", LineWidth = lineWidth }),
            ReactivePlotSource.FromStatic(
                PlotSeriesData.Numeric("Step", primaryAxis, samples.X, samples.Step),
                PlotType.StepLine,
                new() { Color = "#FFCA28" }),
            ReactivePlotSource.FromStatic(
                PlotSeriesData.Numeric(
                    "Area",
                    primaryAxis,
                    samples.X,
                    samples.Wave.Select(static value => value - areaOffset).ToArray()),
                PlotType.Area,
                new()
                {
                    Color = "#66BB6A",
                    BaselineMode = PlotBaselineMode.Custom,
                    Baseline = areaOffset,
                }),];
    }

    /// <summary>Creates the bar, stem, and points examples.</summary>
    /// <param name="samples">The shared static sample data.</param>
    /// <returns>The sparse plot sources.</returns>
    private static IReadOnlyList<IReactivePlotSource> CreateSparseChartTypeSources(ChartTypeSamples samples)
    {
        const int sparseAxis = 5;
        const double stemOffset = 20;
        const double pointOffset = 50;
        const float markerSize = 8;
        return
        [
            ReactivePlotSource.FromStatic(
                PlotSeriesData.Numeric("Bars", sparseAxis, samples.SparseX, samples.SparseY),
                PlotType.Bar,
                new() { Color = "#AB47BC" }),
            ReactivePlotSource.FromStatic(
                PlotSeriesData.Numeric(
                    "Stem",
                    sparseAxis,
                    samples.SparseX,
                    samples.SparseY.Select(static value => value + stemOffset).ToArray()),
                PlotType.Stem,
                new() { Color = "#EF5350", LineMode = PlotLineMode.LineAndMarkers }),
            ReactivePlotSource.FromStatic(
                PlotSeriesData.Numeric(
                    "Points",
                    sparseAxis,
                    samples.SparseX,
                    samples.SparseY.Select(static value => value + pointOffset).ToArray()),
                PlotType.Points,
                new()
                {
                    Color = "#26C6DA",
                    LineMode = PlotLineMode.MarkersOnly,
                    MarkerSize = markerSize,
                }),];
    }

    /// <summary>Creates the shared samples for the static chart type examples.</summary>
    /// <returns>The generated chart type samples.</returns>
    private static ChartTypeSamples CreateChartTypeSamples()
    {
        const int sampleCount = 80;
        const double waveBaseline = 50;
        const double wavePeriod = 8;
        const double waveAmplitude = 30;
        const double stepWidth = 10;
        const int sparseSampleCount = 12;
        const double sparseSpacing = 6;
        const double sparseBaseline = 20;
        const double sparsePeriod = 7;
        const double sparseAmplitude = 15;
        var x = Enumerable.Range(0, sampleCount).Select(static value => (double)value).ToArray();
        var wave = x.Select(static value => waveBaseline + (Math.Sin(value / wavePeriod) * waveAmplitude)).ToArray();
        var step = x.Select(static value => Math.Floor(value / stepWidth) * stepWidth).ToArray();
        var sparseX = Enumerable.Range(0, sparseSampleCount).Select(static value => value * sparseSpacing).ToArray();
        var sparseY = sparseX
            .Select(static value => sparseBaseline + (Math.Sin(value / sparsePeriod) * sparseAmplitude))
            .ToArray();
        return new(x, wave, step, sparseX, sparseY);
    }

    /// <summary>Creates a reduced DateTime series representing decades of dense historic samples.</summary>
    /// <returns>The historic example sources.</returns>
    private static IReadOnlyList<IReactivePlotSource> CreateHistoricSources()
    {
        const int historicPointCount = 150_000;
        const int renderedPointBudget = 1_500;
        const double minutesPerSample = 120;
        const double historicBaseline = 55;
        const double shortPeriod = 140;
        const double shortAmplitude = 15;
        const double longPeriod = 4_000;
        const double longAmplitude = 25;
        const int historicAxis = 2;
        const float lineWidth = 2;
        var start = new DateTime(1995, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var timestamps = new DateTime[historicPointCount];
        var values = new double[historicPointCount];
        for (var i = 0; i < historicPointCount; i++)
        {
            timestamps[i] = start.AddMinutes(i * minutesPerSample);
            values[i] =
                historicBaseline
                + (Math.Sin(i / shortPeriod) * shortAmplitude)
                + (Math.Sin(i / longPeriod) * longAmplitude);
        }

        var history = PlotSeriesData.DateTime("Thirty-year trend", historicAxis, timestamps, values);
        return
        [
            ReactivePlotSource.FromHistoric(
                history,
                renderedPointBudget,
                PlotType.Line,
                new() { Color = "#4CAF50", LineWidth = lineWidth }),];
    }

    /// <summary>Creates static price data with all supported technical studies.</summary>
    /// <returns>The indicator example sources.</returns>
    private static IReadOnlyList<IReactivePlotSource> CreateIndicatorSources()
    {
        const int sampleCount = 320;
        const double priceBaseline = 100;
        const double trendPerSample = 0.08;
        const double shortPeriod = 9;
        const double shortAmplitude = 8;
        const double longPeriod = 27;
        const double longAmplitude = 5;
        const int priceAxis = 3;
        const int oscillatorAxis = 4;
        const float lineWidth = 2;
        const int simpleMovingAveragePeriod = 20;
        const int exponentialMovingAveragePeriod = 12;
        const int relativeStrengthIndexPeriod = 14;
        var x = Enumerable.Range(0, sampleCount).Select(static value => (double)value).ToArray();
        var close = x.Select(static value =>
                priceBaseline
                + (value * trendPerSample)
                + (Math.Sin(value / shortPeriod) * shortAmplitude)
                + (Math.Cos(value / longPeriod) * longAmplitude))
            .ToArray();
        var price = PlotSeriesData.Numeric("Price", priceAxis, x, close);
        var priceSource = ReactivePlotSource.FromStatic(
            price,
            PlotType.Line,
            new() { Color = "#ECEFF1", LineWidth = lineWidth });
        var bollinger = ReactivePlotStudies.BollingerBands(priceSource, axis: priceAxis);
        var ichimoku = ReactivePlotStudies.Ichimoku(priceSource, axis: priceAxis);
        var macd = ReactivePlotStudies.MovingAverageConvergenceDivergence(priceSource, axis: oscillatorAxis);
        return
        [
            priceSource,
            ReactivePlotStudies.SimpleMovingAverage(
                priceSource,
                simpleMovingAveragePeriod,
                priceAxis,
                new() { Color = "#FFCA28" }),
            ReactivePlotStudies.ExponentialMovingAverage(
                priceSource,
                exponentialMovingAveragePeriod,
                priceAxis,
                new() { Color = "#FF7043" }),
            .. bollinger,
            .. ichimoku,
            ReactivePlotStudies.RelativeStrengthIndex(
                priceSource,
                relativeStrengthIndexPeriod,
                oscillatorAxis,
                new() { Color = "#26A69A" }),
            .. macd,];
    }

    /// <summary>Starts the reactive demo data streams.</summary>
    private void StartReactiveDemoStreams()
    {
        if (_demoStreamsStarted)
        {
            return;
        }

        _demoStreamsStarted = true;
        _demoStream.Disposable = Observable
            .Interval(TimeSpan.FromMilliseconds(DemoStreamIntervalMilliseconds))
            .Subscribe(index =>
            {
                var x = (double)index;
                var plotX = x * DemoPlotXScale;
                var signal = DemoWaveBaseline + (Math.Sin(index / SignalWavePeriod) * DemoWaveAmplitude);
                var scatter = DemoWaveBaseline + (Math.Cos(index / ScatterWavePeriod) * DemoWaveAmplitude);
                var logger = NextRandomValue();
                var streamer = DemoWaveBaseline + (Math.Sin(index / StreamerWavePeriod) * StreamerWaveAmplitude);
                var signalXy = DemoWaveBaseline + (Math.Sin(index / SignalXyWavePeriod) * DemoWaveAmplitude);

                AddDemoPoint(_scatterXHistory, plotX);
                AddDemoPoint(_scatterYHistory, scatter);
                AddDemoPoint(_signalXyXHistory, plotX);
                AddDemoPoint(_signalXyYHistory, signalXy);

                _signalPoints.OnNext(("Signal points", [signal], [plotX], SignalAxisIndex));
                _scatterPoints.OnNext(
                    ("Scatter points", _scatterXHistory.ToArray(), _scatterYHistory.ToArray(), ScatterAxisIndex));
                _dataLoggerPoints.OnNext(("Data logger", [logger], LoggerAxisIndex, DemoMaxPoints));
                _streamerPoints.OnNext(("Streamer", [streamer], [plotX], StreamerAxisIndex));
                _signalXyPoints.OnNext(
                    ("SignalXY", _signalXyYHistory.ToArray(), _signalXyXHistory.ToArray(), SignalXyAxisIndex));
            });
    }

    /// <summary>Selects one chart example scenario.</summary>
    /// <param name="name">The scenario name.</param>
    /// <param name="sources">The scenario sources.</param>
    private void SelectScenario(string name, IReadOnlyList<IReactivePlotSource> sources)
    {
        ActiveScenario = name;
        ActiveSources = sources;
    }

    /// <summary>Switches the rendered plot theme.</summary>
    private void ToggleTheme()
    {
        IsLightTheme = !IsLightTheme;
        ThemeButtonText = IsLightTheme ? "Use dark theme" : "Use light theme";
        LiveChartViewModel?.ApplyTheme(IsLightTheme ? ReactivePlotTheme.Light : ReactivePlotTheme.Dark);
    }

    /// <summary>Contains shared samples used by the static chart type examples.</summary>
    /// <param name="X">The dense X coordinates.</param>
    /// <param name="Wave">The dense wave values.</param>
    /// <param name="Step">The dense step values.</param>
    /// <param name="SparseX">The sparse X coordinates.</param>
    /// <param name="SparseY">The sparse Y coordinates.</param>
    private sealed record ChartTypeSamples(
        double[] X,
        double[] Wave,
        double[] Step,
        double[] SparseX,
        double[] SparseY);
}
