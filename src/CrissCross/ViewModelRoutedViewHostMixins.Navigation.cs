// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Provides navigation helpers for routed view hosts.</summary>
public static partial class ViewModelRoutedViewHostMixins
{
    /// <summary>Provides primary host navigation helpers.</summary>
    /// <param name="navigation">The primary navigation owner.</param>
    extension(IUseNavigation navigation)
    {
        /// <summary>Gets a value indicating whether the primary host can navigate back.</summary>
        /// <returns>An observable back-navigation state.</returns>
        public IObservable<bool> CanNavigateBack()
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            return Observable.Create<bool>(observer =>
            {
                var disposable = new CompositeDisposable();
                _ = navigation
                    .WhenSetup()
                    .Subscribe(unused =>
                    {
                        if (
                            NavigationHost.Count == 0
                            || navigation.Name is null
                            || !TryGetNavigationHost(navigation.Name, out var host)
                            || host is null)
                        {
                            return;
                        }

                        _ = host
                            .CanNavigateBackObservable.DistinctUntilChanged()
                            .Subscribe(x => observer.OnNext(x == true))
                            .DisposeWith(disposable);
                    })
                    .DisposeWith(disposable);

                observer.OnNext(false);
                return disposable;
            });
        }

        /// <summary>Clears the history for the primary navigation host.</summary>
        public void ClearHistory()
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            GetRequiredNavigationHost(navigation.Name).ClearHistory();
        }

        /// <summary>Navigates backward on the primary host.</summary>
        public void NavigateBack()
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            _ = GetRequiredNavigationHost(navigation.Name).NavigateBack(null);
        }

        /// <summary>Navigates backward on the primary host.</summary>
        /// <param name="parameter">The navigation parameter.</param>
        public void NavigateBack(object? parameter)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            _ = GetRequiredNavigationHost(navigation.Name).NavigateBack(parameter);
        }

        /// <summary>Navigates the primary host to the requested view model type.</summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="request">The typed navigation request.</param>
        public void NavigateToView<T>(NavigationKeyRequest<T> request)
            where T : class, IRxObject
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            ThrowHelper.ThrowIfNull(request, nameof(request));
            EnsureNavigationHostAvailable();
            GetRequiredNavigationHost(navigation.Name).Navigate(request);
        }

        /// <summary>Navigates the primary host to the requested view model type.</summary>
        /// <param name="rxObject">The view model type.</param>
#if NET8_0_OR_GREATER
        [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(
            "Resolving a view from a runtime view model type requires runtime type inspection.")]
        [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
            "Resolving a view from a runtime view model type may require members removed by trimming.")]
#endif
        public void NavigateToView(Type rxObject) =>
            navigation.NavigateToView(rxObject, new NavigationRequestOptions());

        /// <summary>Navigates the primary host to the requested view model type.</summary>
        /// <param name="rxObject">The view model type.</param>
        /// <param name="options">The navigation request options.</param>
#if NET8_0_OR_GREATER
        [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(
            "Resolving a view from a runtime view model type requires runtime type inspection.")]
        [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
            "Resolving a view from a runtime view model type may require members removed by trimming.")]
#endif
        public void NavigateToView(Type rxObject, NavigationRequestOptions options)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            ThrowHelper.ThrowIfNull(options, nameof(options));
            EnsureNavigationHostAvailable();
            NavigateResolvedView(
                GetRequiredNavigationHost(navigation.Name),
                rxObject,
                options.Contract,
                options.Parameter);
        }

        /// <summary>Navigates the primary host to the registered navigation key.</summary>
        /// <typeparam name="TNavigationKey">The caller-facing view model or view lookup key.</typeparam>
        /// <param name="request">The typed navigation request.</param>
        public void NavigateTo<TNavigationKey>(NavigationKeyRequest<TNavigationKey> request)
            where TNavigationKey : class
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            ThrowHelper.ThrowIfNull(request, nameof(request));
            EnsureNavigationHostAvailable();
            NavigateResolvedNavigationKey(
                GetRequiredNavigationHost(navigation.Name),
                typeof(TNavigationKey),
                request.Options.Contract,
                request.Options.Parameter);
        }

        /// <summary>Navigates the primary host to the registered navigation key.</summary>
        /// <param name="navigationKey">The caller-facing view model or view lookup key.</param>
        public void NavigateTo(Type navigationKey) =>
            navigation.NavigateTo(navigationKey, new NavigationRequestOptions());

        /// <summary>Navigates the primary host to the registered navigation key.</summary>
        /// <param name="navigationKey">The caller-facing view model or view lookup key.</param>
        /// <param name="options">The navigation request options.</param>
        public void NavigateTo(Type navigationKey, NavigationRequestOptions options)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            ThrowHelper.ThrowIfNull(options, nameof(options));
            EnsureNavigationHostAvailable();
            NavigateResolvedNavigationKey(
                GetRequiredNavigationHost(navigation.Name),
                navigationKey,
                options.Contract,
                options.Parameter);
        }

        /// <summary>Navigates the primary host to the requested view model type and clears history.</summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="request">The typed navigation request.</param>
        public void NavigateToViewAndClearHistory<T>(NavigationKeyRequest<T> request)
            where T : class, IRxObject
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            ThrowHelper.ThrowIfNull(request, nameof(request));
            EnsureNavigationHostAvailable();
            GetRequiredNavigationHost(navigation.Name).NavigateAndReset(request);
        }

        /// <summary>Notifies when the primary host is setup.</summary>
        /// <returns>An observable setup state.</returns>
        public IObservable<bool> WhenSetup()
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            return Observable.Create<bool>(observer =>
            {
                var disposable = new CompositeDisposable();
                navigation.BuildComplete(() =>
                {
                    _ = ASetupCompleted
                        .Subscribe(unused =>
                        {
                            if (
                                navigation.Name is null
                                || !TryGetNavigationHost(navigation.Name, out var host)
                                || host is null)
                            {
                                return;
                            }

                            if (!WhenSetupSubjects.TryGetValue(host.Name, out var whenSetup))
                            {
                                return;
                            }

                            _ = whenSetup.Where(x => x).Subscribe(observer).DisposeWith(disposable);
                        })
                        .DisposeWith(disposable);
                });
                return disposable;
            });
        }
    }
}
