// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.Plot.Test.ViewModels
{
    /// <summary>
    /// MainViewModel.
    /// </summary>
    /// <seealso cref="RxObject" />
    public partial class MainViewModel : RxObject
    {
        private const int DemoMaxPoints = 600;
        private readonly Subject<(string? Name, IList<double>? Value, IList<double> X, int Axis)> _signalPoints = new();
        private readonly Subject<(string? Name, IList<double>? X, IList<double> Y, int Axis)> _scatterPoints = new();
        private readonly Subject<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)> _dataLoggerPoints = new();
        private readonly Subject<(string? Name, IList<double>? Y, IList<double> X, int Axis)> _streamerPoints = new();
        private readonly Subject<(string? Name, IList<double>? Y, IList<double> X, int Axis)> _signalXyPoints = new();
        private readonly List<double> _scatterXHistory = [];
        private readonly List<double> _scatterYHistory = [];
        private readonly List<double> _signalXyXHistory = [];
        private readonly List<double> _signalXyYHistory = [];
        private readonly SerialDisposable _demoStream = new();
        private bool _demoStreamsStarted;

        [Reactive]
        private LiveChartViewModel? _liveChartViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            YAxisNames = (new List<string> { "[Signal]", "[Scatter]", "[Logger]", "[Streamer]", "[SignalXY]" }, new List<string> { "#377eb8", "#ff7f00", "#4daf4a", "#984ea3", "#e41a1c" });
            ReactivePlotSources = Observable.Return<IReadOnlyList<IReactivePlotSource>>(
            [
                ReactivePlotSource.FromSignalPoints(_signalPoints),
                ReactivePlotSource.FromScatterPoints(_scatterPoints),
                ReactivePlotSource.FromDataLoggerPoints(_dataLoggerPoints),
                ReactivePlotSource.FromStreamerPoints(_streamerPoints),
                ReactivePlotSource.FromSignalXyPoints(_signalXyPoints),
            ]);

            LiveChartSubject = [_signalPoints];
            this.WhenAnyValue(vm => vm.LiveChartViewModel)
                .WhereNotNull()
                .Subscribe(_ => StartReactiveDemoStreams());
        }

        /// <summary>
        /// Gets a one-shot observable that provides all live reactive plot sources used by the example chart.
        /// </summary>
        public IObservable<IReadOnlyList<IReactivePlotSource>> ReactivePlotSources { get; }

        /// <summary>
        /// Gets the legacy live chart subject retained for backwards-compatible examples.
        /// </summary>
        public IEnumerable<Subject<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>> LiveChartSubject { get; private set; }

        /// <summary>
        /// Gets the y axis names.
        /// </summary>
        /// <value>
        /// The y axis names.
        /// </value>
        public (IList<string> yNames, IList<string> hexColors) YAxisNames { get; private set; }

        /// <summary>
        /// Releases managed demo subjects.
        /// </summary>
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

        private static double NextRandomValue()
        {
            var bytes = new byte[8];
#if NET472 || NET481
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(bytes);
#else
            RandomNumberGenerator.Fill(bytes);
#endif
            return BitConverter.ToUInt64(bytes, 0) / (double)ulong.MaxValue * 100;
        }

        private static void AddDemoPoint(List<double> values, double value)
        {
            values.Add(value);
            if (values.Count > DemoMaxPoints)
            {
                values.RemoveAt(0);
            }
        }

        private void StartReactiveDemoStreams()
        {
            if (_demoStreamsStarted)
            {
                return;
            }

            _demoStreamsStarted = true;
            _demoStream.Disposable = Observable.Interval(TimeSpan.FromMilliseconds(250))
                .Subscribe(index =>
                {
                    var x = (double)index;
                    var plotX = x * 10.0;
                    var signal = 50.0 + (Math.Sin(index / 8.0) * 35.0);
                    var scatter = 50.0 + (Math.Cos(index / 9.0) * 35.0);
                    var logger = NextRandomValue();
                    var streamer = 50.0 + (Math.Sin(index / 5.0) * 45.0);
                    var signalXy = 50.0 + (Math.Sin(index / 12.0) * 35.0);

                    AddDemoPoint(_scatterXHistory, plotX);
                    AddDemoPoint(_scatterYHistory, scatter);
                    AddDemoPoint(_signalXyXHistory, plotX);
                    AddDemoPoint(_signalXyYHistory, signalXy);

                    _signalPoints.OnNext(("Signal points", [signal], [plotX], 0));
                    _scatterPoints.OnNext(("Scatter points", _scatterXHistory.ToArray(), _scatterYHistory.ToArray(), 1));
                    _dataLoggerPoints.OnNext(("Data logger", [logger], 2, DemoMaxPoints));
                    _streamerPoints.OnNext(("Streamer", [streamer], [plotX], 3));
                    _signalXyPoints.OnNext(("SignalXY", _signalXyYHistory.ToArray(), _signalXyXHistory.ToArray(), 4));
                });
        }
    }
}
