// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
        [Reactive]
        private LiveChartViewModel? _liveChartViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            YAxisNames = (new List<string> { "[Series 0]", "[Series 1]", "[Series 2]", "[Series 3]", "[Series 4]", "[Series 5]" }, new List<string> { "#377eb8", "#ff7f00", "#377eb8", "#ff7f00", "#ff7f00", "#377eb8" });

            var numberAxis = YAxisNames.yNames.Count;
            LiveChartSubject = [.. Enumerable.Range(0, numberAxis).Select(_ => new Subject<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>())];
            if (YAxisNames.yNames.Count != YAxisNames.hexColors.Count)
            {
                throw new InvalidOperationException("Y Axis number should match colors number");
            }

            this.WhenAnyValue(vm => vm.LiveChartViewModel)
                .Subscribe(liveChart =>
                {
                    liveChart?.ClearAxisLines();

                    // Generate some random data for the chart using observables
                    Observable.Interval(TimeSpan.FromMilliseconds(1000.0 / numberAxis))
                        .Select(_ =>
                        {
                            // Use RandomNumberGenerator for cryptographically secure random numbers
                            var bytes = new byte[8];
                            RandomNumberGenerator.Fill(bytes);
                            double randomValue = BitConverter.ToUInt64(bytes, 0) / (double)ulong.MaxValue * 100;
                            return (Name: $"Series {_ % numberAxis}", Value: new List<double> { randomValue }, DateTime: new List<double> { DateTime.Now.Ticks }, Axis: (int)(_ % numberAxis));
                        })
                        .Subscribe(data =>
                        {
                            var subject = LiveChartSubject.ElementAt(data.Axis);
                            if (subject is Subject<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> sub)
                            {
                                sub.OnNext(data);
                            }
                        });
                });
        }

        /// <summary>
        /// Gets the live chart subject.
        /// </summary>
        /// <value>
        /// The live chart subject.
        /// </value>
        public IEnumerable<Subject<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>> LiveChartSubject { get; private set; }

        /// <summary>
        /// Gets the y axis names.
        /// </summary>
        /// <value>
        /// The y axis names.
        /// </value>
        public (IList<string> yNames, IList<string> hexColors) YAxisNames { get; private set; }
    }
}
