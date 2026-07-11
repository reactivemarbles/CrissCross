// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using BenchmarkDotNet.Attributes;
using ReactiveUI;

namespace CrissCross.WinForms.Benchmarks;

    /// <summary>Benchmarks for ViewModelRoutedViewHost navigation and history operations.</summary>
    public class ViewModelRoutedViewHostBenchmark : IDisposable
    {
        /// <summary>Provides the _host member.</summary>
        private ViewModelRoutedViewHost? _host;

        /// <summary>Provides the _dummyViewModel member.</summary>
        private IRxObject? _dummyViewModel;

        /// <summary>Provides the _disposedValue member.</summary>
        private bool _disposedValue;

        /// <summary>Initializes the benchmark host and dummy view model.</summary>
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

        /// <summary>Benchmarks navigation to a new view model type.</summary>
        [Benchmark]
        public void Navigate() => _host?.Navigate<DummyRxObject>();

        /// <summary>Benchmarks navigation with a view model instance.</summary>
        [Benchmark]
        public void NavigateWithInstance() => _host?.Navigate(_dummyViewModel!);

        /// <summary>Benchmarks navigation and reset to a new view model type.</summary>
        [Benchmark]
        public void NavigateAndReset() => _host?.NavigateAndReset<DummyRxObject>();

        /// <summary>Benchmarks navigating back in the navigation stack.</summary>
        [Benchmark]
        public void NavigateBack() => _host?.NavigateBack();

        /// <summary>Benchmarks clearing the navigation history.</summary>
        [Benchmark]
        public void ClearHistory() => _host?.ClearHistory();

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
            {
                return;
            }

            if (disposing)
            {
                _host?.Dispose();
                (_dummyViewModel as IDisposable)?.Dispose();
            }

            _disposedValue = true;
        }

        /// <summary>Dummy implementation of IRxObject for benchmarking.</summary>
        private sealed class DummyRxObject : IRxObject
        {
            /// <summary>Provides the PropertyChanged member.</summary>
            public event PropertyChangedEventHandler? PropertyChanged
            {
                add { }
                remove { }
            }

            /// <summary>Gets or sets the value.</summary>
            public event PropertyChangingEventHandler? PropertyChanging
            {
                add { }
                remove { }
            }

            /// <summary>Gets or sets the display name.</summary>
            public string? DisplayName { get; set; }

            /// <summary>Gets or sets the name.</summary>
            public string? Name { get; set; }

            /// <summary>Gets a value indicating whether this object has been disposed.</summary>
            public bool IsDisposed { get; private set; }

            /// <summary>Gets the value.</summary>
            public IObservable<Exception> ThrownExceptions => Observable.Empty<Exception>();

            /// <summary>Gets the value.</summary>
            public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing => Observable.Empty<IReactivePropertyChangedEventArgs<IReactiveObject>>();

            /// <summary>Gets changed notifications.</summary>
            public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed => Observable.Empty<IReactivePropertyChangedEventArgs<IReactiveObject>>();

            /// <summary>Provides the Dispose member.</summary>
            public void Dispose()
            {
                IsDisposed = true;
            }

            /// <summary>Provides the SuppressChangeNotifications member.</summary>
            /// <returns>The result.</returns>
            public IDisposable SuppressChangeNotifications() => EmptyDisposable.Instance;

            /// <summary>Provides the RaisePropertyChanging member.</summary>
            /// <param name="args">The args value.</param>
            public void RaisePropertyChanging(PropertyChangingEventArgs args)
            {
            }

            /// <summary>Provides the RaisePropertyChanged member.</summary>
            /// <param name="args">The args value.</param>
            public void RaisePropertyChanged(PropertyChangedEventArgs args)
            {
            }

            /// <summary>Provides the WhenNavigatedFrom member.</summary>
            /// <param name="e">The args value.</param>
            public void WhenNavigatedFrom(IViewModelNavigationEventArgs e)
            {
            }

            /// <summary>Provides the WhenNavigatedTo member.</summary>
            /// <param name="e">The args value.</param>
            /// <param name="disposables">The disposables value.</param>
            public void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables)
            {
            }

            /// <summary>Provides the WhenNavigating member.</summary>
            /// <param name="e">The args value.</param>
            public void WhenNavigating(IViewModelNavigatingEventArgs e)
            {
            }
        }
}
