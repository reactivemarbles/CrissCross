// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ReactiveUI;

[assembly: InternalsVisibleTo("CrissCross.Avalonia")]
[assembly: InternalsVisibleTo("CrissCross.MAUI")]
[assembly: InternalsVisibleTo("CrissCross.WinForms")]
[assembly: InternalsVisibleTo("CrissCross.WPF")]
[assembly: InternalsVisibleTo("CrissCross.XamForms")]
[assembly: InternalsVisibleTo("CrissCross.Tests")]

namespace CrissCross;

/// <summary>Provides navigation helpers for routed view hosts.</summary>
public static partial class ViewModelRoutedViewHostMixins
{
    /// <summary>Coordinates access to shared navigation host state.</summary>
    private static readonly object _lockObject = new();

    /// <summary>Gets the signal that at least one navigation host setup has completed.</summary>
    internal static ReplaySignal<Unit> ASetupCompleted { get; } = new(1);

    /// <summary>Gets the view-scoped disposables by navigation host name.</summary>
    internal static Dictionary<string, CompositeDisposable> CurrentViewDisposable { get; } = [];

    /// <summary>Gets the registered navigation hosts by name.</summary>
    internal static Dictionary<string, IViewModelRoutedViewHost> NavigationHost { get; } = [];

    /// <summary>Gets the navigating event signals by host name.</summary>
    internal static Dictionary<string, Signal<IViewModelNavigatingEventArgs>> ResultNavigating { get; } = [];

    /// <summary>Gets the completed navigation event signal.</summary>
    internal static Signal<IViewModelNavigationEventArgs> SetWhenNavigated { get; } = new();

    /// <summary>Gets the pending navigation event signal.</summary>
    internal static Signal<IViewModelNavigatingEventArgs> SetWhenNavigating { get; } = new();

    /// <summary>Gets the setup-completion signals by host name.</summary>
    internal static Dictionary<string, ReplaySignal<bool>> WhenSetupSubjects { get; } = [];

    /// <summary>Provides navigation event helpers.</summary>
    /// <param name="navigation">The navigation notification owner.</param>
    extension(INotifiyNavigation navigation)
    {
        /// <summary>Registers a handler for navigation-from notifications.</summary>
        /// <param name="handler">The navigation handler.</param>
        public void WhenNavigatedFrom(Action<IViewModelNavigationEventArgs> handler)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            ThrowHelper.ThrowIfNull(handler, nameof(handler));

            navigation.ISetupNavigatedFrom = true;
            var vm = (navigation as IViewFor)?.ViewModel as INotifiyRoutableViewModel;
            _ = SetWhenNavigated
                .Where(x => x.From is not null && x.From.Name == vm?.Name)
                .Subscribe(args =>
                {
                    handler(args);
                    args.From?.WhenNavigatedFrom(args);
                })
                .DisposeWith(navigation.CleanUp);
        }

        /// <summary>Registers a handler for navigation-to notifications.</summary>
        /// <param name="handler">The navigation handler.</param>
        public void WhenNavigatedTo(Action<IViewModelNavigationEventArgs, CompositeDisposable> handler)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            ThrowHelper.ThrowIfNull(handler, nameof(handler));

            navigation.ISetupNavigatedTo = true;
            var vm = (navigation as IViewFor)?.ViewModel as INotifiyRoutableViewModel;
            _ = SetWhenNavigated
                .Where(x => x?.To?.Name == vm?.Name)
                .Subscribe(args =>
                {
                    var disposable = GetCurrentViewDisposable(args);
                    handler(args, disposable);
                    args.To?.WhenNavigatedTo(args, disposable);
                })
                .DisposeWith(navigation.CleanUp);
        }

        /// <summary>Registers a handler for pending navigation notifications.</summary>
        /// <param name="handler">The navigation handler.</param>
        public void WhenNavigating(Func<IViewModelNavigatingEventArgs, IViewModelNavigatingEventArgs> handler)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            ThrowHelper.ThrowIfNull(handler, nameof(handler));

            navigation.ISetupNavigating = true;
            var vm = (navigation as IViewFor)?.ViewModel as INotifiyRoutableViewModel;
            _ = SetWhenNavigating
                .Where(x => x?.From is null || x.From.Name == vm?.Name)
                .Subscribe(args =>
                {
                    if (args is null)
                    {
                        return;
                    }

                    if (args.From is not null)
                    {
                        _ = handler(args);
                    }

                    args.From?.WhenNavigating(args);
                    if (!ResultNavigating.TryGetValue(args.HostName!, out var resultNavigating))
                    {
                        return;
                    }

                    resultNavigating.OnNext(args);
                })
                .DisposeWith(navigation.CleanUp);
        }
    }

    /// <summary>Provides setup helpers for navigation hosts.</summary>
    /// <param name="navigation">The navigation setup owner.</param>
    extension(ISetNavigation navigation)
    {
        /// <summary>Sets the main navigation host.</summary>
        /// <param name="viewHost">The view host.</param>
        public void SetMainNavigationHost(IViewModelRoutedViewHost viewHost)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            ThrowHelper.ThrowIfNull(viewHost, nameof(viewHost));

            var hostKeys = CreateHostKeys(navigation, viewHost);
            var hostKey = hostKeys[0];
            EnsureViewHostName(viewHost, hostKey, hostKeys);
            RegisterNavigationHostAliases(hostKeys, viewHost);
            SetupViewHostIfRequired(viewHost);
            ASetupCompleted.OnNext(Unit.Default);
            PublishHostSetup(hostKey);
        }
    }

    /// <summary>Provides host-name based navigation helpers.</summary>
    /// <param name="navigation">The hosted navigation owner.</param>
    extension(IUseHostedNavigation navigation)
    {
        /// <summary>Gets a value indicating whether the named host can navigate back.</summary>
        /// <returns>An observable back-navigation state.</returns>
        public IObservable<bool> CanNavigateBack() => navigation.CanNavigateBack(string.Empty);

        /// <summary>Gets a value indicating whether the named host can navigate back.</summary>
        /// <param name="hostName">Name of the host.</param>
        /// <returns>An observable back-navigation state.</returns>
        public IObservable<bool> CanNavigateBack(string hostName)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            return Observable.Create<bool>(observer =>
            {
                var disposable = new CompositeDisposable();
                _ = navigation
                    .WhenSetup(hostName)
                    .Subscribe(unused =>
                    {
                        if (
                            NavigationHost.Count == 0
                            || hostName is null
                            || !TryGetNavigationHost(hostName, out var host)
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

        /// <summary>Clears the history for the named navigation host.</summary>
        public void ClearHistory() => navigation.ClearHistory(string.Empty);

        /// <summary>Clears the history for the named navigation host.</summary>
        /// <param name="hostName">Name of the host.</param>
        public void ClearHistory(string hostName)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            GetRequiredNavigationHost(hostName).ClearHistory();
        }

        /// <summary>Navigates backward on the named host.</summary>
        /// <returns>The target view model.</returns>
        public IRxObject? NavigateBack() => navigation.NavigateBack(string.Empty, null);

        /// <summary>Navigates backward on the named host.</summary>
        /// <param name="hostName">Name of the host.</param>
        /// <returns>The target view model.</returns>
        public IRxObject? NavigateBack(string? hostName) => navigation.NavigateBack(hostName, null);

        /// <summary>Navigates backward on the named host.</summary>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="parameter">The navigation parameter.</param>
        /// <returns>The target view model.</returns>
        public IRxObject? NavigateBack(string? hostName, object? parameter)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            return GetRequiredNavigationHost(hostName).NavigateBack(parameter);
        }

        /// <summary>Navigates the named host to the requested view model type.</summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="request">The typed navigation request.</param>
        public void NavigateToView<T>(NavigationKeyRequest<T> request)
            where T : class, IRxObject
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            ThrowHelper.ThrowIfNull(request, nameof(request));
            EnsureNavigationHostAvailable();
            GetRequiredNavigationHost(request.Options.HostName).Navigate(request);
        }

        /// <summary>Navigates the named host to the requested view model type.</summary>
        /// <param name="rxObject">The view model type.</param>
#if NET8_0_OR_GREATER
        [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(
            "Resolving a view from a runtime view model type requires runtime type inspection.")]
        [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
            "Resolving a view from a runtime view model type may require members removed by trimming.")]
#endif
        public void NavigateToView(Type rxObject) =>
            navigation.NavigateToView(rxObject, new NavigationRequestOptions());

        /// <summary>Navigates the named host to the requested view model type.</summary>
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
                GetRequiredNavigationHost(options.HostName),
                rxObject,
                options.Contract,
                options.Parameter);
        }

        /// <summary>Navigates the named host to the registered navigation key.</summary>
        /// <typeparam name="TNavigationKey">The caller-facing view model or view lookup key.</typeparam>
        /// <param name="request">The typed navigation request.</param>
        public void NavigateTo<TNavigationKey>(NavigationKeyRequest<TNavigationKey> request)
            where TNavigationKey : class
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            ThrowHelper.ThrowIfNull(request, nameof(request));
            EnsureNavigationHostAvailable();
            NavigateResolvedNavigationKey(
                GetRequiredNavigationHost(request.Options.HostName),
                typeof(TNavigationKey),
                request.Options.Contract,
                request.Options.Parameter);
        }

        /// <summary>Navigates the named host to the registered navigation key.</summary>
        /// <param name="navigationKey">The caller-facing view model or view lookup key.</param>
        public void NavigateTo(Type navigationKey) =>
            navigation.NavigateTo(navigationKey, new NavigationRequestOptions());

        /// <summary>Navigates the named host to the registered navigation key.</summary>
        /// <param name="navigationKey">The caller-facing view model or view lookup key.</param>
        /// <param name="options">The navigation request options.</param>
        public void NavigateTo(Type navigationKey, NavigationRequestOptions options)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            ThrowHelper.ThrowIfNull(options, nameof(options));
            EnsureNavigationHostAvailable();
            NavigateResolvedNavigationKey(
                GetRequiredNavigationHost(options.HostName),
                navigationKey,
                options.Contract,
                options.Parameter);
        }

        /// <summary>Navigates the named host to the requested view model type and clears history.</summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="request">The typed navigation request.</param>
        public void NavigateToViewAndClearHistory<T>(NavigationKeyRequest<T> request)
            where T : class, IRxObject
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            ThrowHelper.ThrowIfNull(request, nameof(request));
            EnsureNavigationHostAvailable();
            GetRequiredNavigationHost(request.Options.HostName).NavigateAndReset(request);
        }

        /// <summary>Notifies when the named host is setup.</summary>
        /// <returns>An observable setup state.</returns>
        public IObservable<bool> WhenSetup() => navigation.WhenSetup(string.Empty);

        /// <summary>Notifies when the named host is setup.</summary>
        /// <param name="hostName">Name of the host.</param>
        /// <returns>An observable setup state.</returns>
        public IObservable<bool> WhenSetup(string? hostName) =>
            Observable.Create<bool>(observer =>
            {
                ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
                var disposable = new CompositeDisposable();
                _ = ASetupCompleted
                    .Subscribe(unused =>
                    {
                        if (!TryGetNavigationHost(hostName, out var host) || host is null)
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
                return disposable;
            });
    }
}
