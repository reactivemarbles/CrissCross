// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using BenchmarkDotNet.Attributes;
using ReactiveUI;

namespace CrissCross.MAUI.Benchmarks
{
    /// <summary>
    /// NavigationShellBenchmark.
    /// </summary>
    public sealed class NavigationShellBenchmark : IDisposable
    {
        private NavigationShell _host;
        private IRxObject _dummyViewModel;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            _host = new NavigationShell();
            _host.Name = "BenchmarkHost";
            _host.Setup();
            _dummyViewModel = new DummyRxObject();
        }

        /// <summary>
        /// Navigates this instance.
        /// </summary>
        [Benchmark]
        public void Navigate()
        {
            _host.Navigate<DummyRxObject>();
        }

        /// <summary>
        /// Navigates the with instance.
        /// </summary>
        [Benchmark]
        public void NavigateWithInstance()
        {
            _host.Navigate(_dummyViewModel);
        }

        /// <summary>
        /// Navigates the and reset.
        /// </summary>
        [Benchmark]
        public void NavigateAndReset()
        {
            _host.NavigateAndReset<DummyRxObject>();
        }

        /// <summary>
        /// Navigates the back.
        /// </summary>
        [Benchmark]
        public void NavigateBack()
        {
            _host.NavigateBack();
        }

        /// <summary>
        /// Clears the history.
        /// </summary>
        [Benchmark]
        public void ClearHistory()
        {
            _host.ClearHistory();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _host?.Dispose();
            (_dummyViewModel as IDisposable)?.Dispose();
        }

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
