// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using BenchmarkDotNet.Attributes;
using ReactiveUI;

namespace CrissCross.WPF.Benchmarks
{
    /// <summary>
    /// Benchmarks for ViewModelRoutedViewHost navigation and history operations.
    /// </summary>
    public sealed class ViewModelRoutedViewHostBenchmark : IDisposable
    {
        private ViewModelRoutedViewHost _host;
        private IRxObject _dummyViewModel;

        /// <summary>
        /// Initializes the benchmark host and dummy view model.
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            _host = new ViewModelRoutedViewHost();
            _host.HostName = "BenchmarkHost";
            _host.Setup();
            _dummyViewModel = new DummyRxObject();
        }

        /// <summary>
        /// Benchmarks navigation to a new view model type.
        /// </summary>
        [Benchmark]
        public void Navigate()
        {
            _host.Navigate<DummyRxObject>();
        }

        /// <summary>
        /// Benchmarks navigation with a view model instance.
        /// </summary>
        [Benchmark]
        public void NavigateWithInstance()
        {
            _host.Navigate(_dummyViewModel);
        }

        /// <summary>
        /// Benchmarks navigation and reset to a new view model type.
        /// </summary>
        [Benchmark]
        public void NavigateAndReset()
        {
            _host.NavigateAndReset<DummyRxObject>();
        }

        /// <summary>
        /// Benchmarks navigating back in the navigation stack.
        /// </summary>
        [Benchmark]
        public void NavigateBack()
        {
            _host.NavigateBack();
        }

        /// <summary>
        /// Benchmarks clearing the navigation history.
        /// </summary>
        [Benchmark]
        public void ClearHistory()
        {
            _host.ClearHistory();
        }

        /// <summary>
        /// Disposes the benchmark host and dummy view model.
        /// </summary>
        public void Dispose()
        {
            _host?.Dispose();
            (_dummyViewModel as IDisposable)?.Dispose();
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

            public IDisposable SuppressChangeNotifications()
            {
                return Disposable.Empty;
            }

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
