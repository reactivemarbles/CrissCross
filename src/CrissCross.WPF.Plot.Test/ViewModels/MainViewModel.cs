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

    /// <summary>Tracks whether the reactive demo streams have started.</summary>
    private bool _demoStreamsStarted;

    /// <summary>Provides the live chart view model.</summary>
    [Reactive]
    private LiveChartViewModel? _liveChartViewModel;

    /// <summary>Initializes a new instance of the <see cref="MainViewModel"/> class.</summary>
    public MainViewModel()
    {
        YAxisNames = (["[Signal]", "[Scatter]", "[Logger]", "[Streamer]", "[SignalXY]"], ["#377eb8", "#ff7f00", "#4daf4a", "#984ea3", "#e41a1c"]);
        ReactivePlotSources = Observable.Return<IReadOnlyList<IReactivePlotSource>>(
        [
            ReactivePlotSource.FromSignalPoints(_signalPoints),
            ReactivePlotSource.FromScatterPoints(_scatterPoints),
            ReactivePlotSource.FromDataLoggerPoints(_dataLoggerPoints),
            ReactivePlotSource.FromStreamerPoints(_streamerPoints),
            ReactivePlotSource.FromSignalXyPoints(_signalXyPoints),
        ]);

        LiveChartSubject = [_signalPoints];
        _ = this.WhenAnyValue(vm => vm.LiveChartViewModel)
            .Where(static liveChartViewModel => liveChartViewModel is not null)
            .Select(static liveChartViewModel => liveChartViewModel!)
            .Subscribe(_ => StartReactiveDemoStreams());
    }

    /// <summary>Gets a one-shot observable that provides all live reactive plot sources used by the example chart.</summary>
    public IObservable<IReadOnlyList<IReactivePlotSource>> ReactivePlotSources { get; }

    /// <summary>Gets the legacy live chart subject retained for backwards-compatible examples.</summary>
    public IEnumerable<Signal<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>> LiveChartSubject { get; private set; }

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
#if NET472 || NET481
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

    /// <summary>Starts the reactive demo data streams.</summary>
    private void StartReactiveDemoStreams()
    {
        if (_demoStreamsStarted)
        {
            return;
        }

        _demoStreamsStarted = true;
        _demoStream.Disposable = Observable.Interval(TimeSpan.FromMilliseconds(DemoStreamIntervalMilliseconds))
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
                _scatterPoints.OnNext(("Scatter points", _scatterXHistory.ToArray(), _scatterYHistory.ToArray(), ScatterAxisIndex));
                _dataLoggerPoints.OnNext(("Data logger", [logger], LoggerAxisIndex, DemoMaxPoints));
                _streamerPoints.OnNext(("Streamer", [streamer], [plotX], StreamerAxisIndex));
                _signalXyPoints.OnNext(("SignalXY", _signalXyYHistory.ToArray(), _signalXyXHistory.ToArray(), SignalXyAxisIndex));
            });
    }
}
