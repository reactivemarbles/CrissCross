// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using BenchmarkDotNet.Attributes;
using ReactiveUI;

namespace CrissCross.WinForms.Benchmarks
{
    /// <summary>
    /// Benchmarks for ViewModelRoutedViewHost navigation and history operations.
    /// </summary>
    public partial class ViewModelRoutedViewHostBenchmark : IDisposable
    {
        private ViewModelRoutedViewHost? _host;
        private IRxObject? _dummyViewModel;
        private bool _disposedValue;

        /// <summary>
        /// Initializes the benchmark host and dummy view model.
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            _host = new ViewModelRoutedViewHost
            {
                HostName = "BenchmarkHost"
            };
            _host.Setup();
            _dummyViewModel = new DummyRxObject();
        }

        /// <summary>
        /// Benchmarks navigation to a new view model type.
        /// </summary>
        [Benchmark]
        public void Navigate() => _host?.Navigate<DummyRxObject>();

        /// <summary>
        /// Benchmarks navigation with a view model instance.
        /// </summary>
        [Benchmark]
        public void NavigateWithInstance() => _host?.Navigate(_dummyViewModel!);

        /// <summary>
        /// Benchmarks navigation and reset to a new view model type.
        /// </summary>
        [Benchmark]
        public void NavigateAndReset() => _host?.NavigateAndReset<DummyRxObject>();

        /// <summary>
        /// Benchmarks navigating back in the navigation stack.
        /// </summary>
        [Benchmark]
        public void NavigateBack() => _host?.NavigateBack();

        /// <summary>
        /// Benchmarks clearing the navigation history.
        /// </summary>
        [Benchmark]
        public void ClearHistory() => _host?.ClearHistory();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _host?.Dispose();
                    (_dummyViewModel as IDisposable)?.Dispose();
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Dummy implementation of IRxObject for benchmarking.
        /// </summary>
        private class DummyRxObject : IRxObject
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            public event PropertyChangingEventHandler? PropertyChanging;

            public string? DisplayName { get; set; }

            public string? Name { get; set; }

            public bool IsDisposed => false;

            public IObservable<Exception> ThrownExceptions => Observable.Empty<Exception>();

            public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing => Observable.Empty<IReactivePropertyChangedEventArgs<IReactiveObject>>();

            public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed => Observable.Empty<IReactivePropertyChangedEventArgs<IReactiveObject>>();

            public void Dispose()
            {
            }

            public IDisposable SuppressChangeNotifications() => Disposable.Empty;

            public void RaisePropertyChanging(PropertyChangingEventArgs args)
            {
            }

            public void RaisePropertyChanged(PropertyChangedEventArgs args)
            {
            }

            public void WhenNavigatedFrom(IViewModelNavigationEventArgs args)
            {
            }

            public void WhenNavigatedTo(IViewModelNavigationEventArgs args, CompositeDisposable disposables)
            {
            }

            public void WhenNavigating(IViewModelNavigatingEventArgs args)
            {
            }
        }
    }
}
