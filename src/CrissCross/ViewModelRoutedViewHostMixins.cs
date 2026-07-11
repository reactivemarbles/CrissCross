// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ReactiveUI;
using Splat;

[assembly: InternalsVisibleTo("CrissCross.Avalonia")]
[assembly: InternalsVisibleTo("CrissCross.MAUI")]
[assembly: InternalsVisibleTo("CrissCross.WinForms")]
[assembly: InternalsVisibleTo("CrissCross.WPF")]
[assembly: InternalsVisibleTo("CrissCross.XamForms")]
[assembly: InternalsVisibleTo("CrissCross.Tests")]

namespace CrissCross;

/// <summary>Provides navigation helpers for routed view hosts.</summary>
public static class ViewModelRoutedViewHostMixins
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
        /// <param name="hostName">Name of the host.</param>
        /// <returns>An observable back-navigation state.</returns>
        public IObservable<bool> CanNavigateBack(string hostName = "")
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            return Observable.Create<bool>(observer =>
            {
                var disposable = new CompositeDisposable();
                _ = navigation.WhenSetup(hostName).Subscribe(unused =>
                {
                    if (NavigationHost.Count == 0 || hostName is null || !TryGetNavigationHost(hostName, out var host) || host is null)
                    {
                        return;
                    }

                    _ = host.CanNavigateBackObservable
                        .DistinctUntilChanged()
                        .Subscribe(x => observer.OnNext(x == true))
                        .DisposeWith(disposable);
                }).DisposeWith(disposable);

                observer.OnNext(false);
                return disposable;
            });
        }

        /// <summary>Clears the history for the named navigation host.</summary>
        /// <param name="hostName">Name of the host.</param>
        public void ClearHistory(string hostName = "")
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            GetRequiredNavigationHost(hostName).ClearHistory();
        }

        /// <summary>Navigates backward on the named host.</summary>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="parameter">The navigation parameter.</param>
        /// <returns>The target view model.</returns>
        public IRxObject? NavigateBack(string? hostName = "", object? parameter = null)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            return GetRequiredNavigationHost(hostName).NavigateBack(parameter);
        }

        /// <summary>Navigates the named host to the requested view model type.</summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="contract">The contract.</param>
        /// <param name="parameter">The navigation parameter.</param>
        public void NavigateToView<T>(string? hostName = "", string? contract = null, object? parameter = null)
            where T : class, IRxObject
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            GetRequiredNavigationHost(hostName).Navigate<T>(contract, parameter);
        }

        /// <summary>Navigates the named host to the requested view model type.</summary>
        /// <param name="rxObject">The view model type.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="contract">The contract.</param>
        /// <param name="parameter">The navigation parameter.</param>
#if NET8_0_OR_GREATER
        [System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Resolving a view from a runtime view model type requires runtime type inspection.")]
        [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Resolving a view from a runtime view model type may require members removed by trimming.")]
#endif
        public void NavigateToView(Type rxObject, string? hostName = "", string? contract = null, object? parameter = null)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            NavigateResolvedView(GetRequiredNavigationHost(hostName), rxObject, contract, parameter);
        }

        /// <summary>Navigates the named host to the registered navigation key.</summary>
        /// <typeparam name="TNavigationKey">The caller-facing view model or view lookup key.</typeparam>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="contract">The contract.</param>
        /// <param name="parameter">The navigation parameter.</param>
        public void NavigateTo<TNavigationKey>(string? hostName = "", string? contract = null, object? parameter = null)
            where TNavigationKey : class
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            NavigateResolvedNavigationKey(GetRequiredNavigationHost(hostName), typeof(TNavigationKey), contract, parameter);
        }

        /// <summary>Navigates the named host to the registered navigation key.</summary>
        /// <param name="navigationKey">The caller-facing view model or view lookup key.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="contract">The contract.</param>
        /// <param name="parameter">The navigation parameter.</param>
        public void NavigateTo(Type navigationKey, string? hostName = "", string? contract = null, object? parameter = null)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            NavigateResolvedNavigationKey(GetRequiredNavigationHost(hostName), navigationKey, contract, parameter);
        }

        /// <summary>Navigates the named host to the requested view model type and clears history.</summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="contract">The contract.</param>
        /// <param name="parameter">The navigation parameter.</param>
        public void NavigateToViewAndClearHistory<T>(string hostName = "", string? contract = null, object? parameter = null)
            where T : class, IRxObject
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            GetRequiredNavigationHost(hostName).NavigateAndReset<T>(contract, parameter);
        }

        /// <summary>Notifies when the named host is setup.</summary>
        /// <param name="hostName">Name of the host.</param>
        /// <returns>An observable setup state.</returns>
        public IObservable<bool> WhenSetup(string? hostName = "") =>
            Observable.Create<bool>(observer =>
            {
                ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
                var disposable = new CompositeDisposable();
                _ = ASetupCompleted.Subscribe(unused =>
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
                }).DisposeWith(disposable);
                return disposable;
            });
    }

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
                _ = navigation.WhenSetup().Subscribe(unused =>
                {
                    if (NavigationHost.Count == 0 || navigation.Name is null || !TryGetNavigationHost(navigation.Name, out var host) || host is null)
                    {
                        return;
                    }

                    _ = host.CanNavigateBackObservable
                        .DistinctUntilChanged()
                        .Subscribe(x => observer.OnNext(x == true))
                        .DisposeWith(disposable);
                }).DisposeWith(disposable);

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
        /// <param name="parameter">The navigation parameter.</param>
        public void NavigateBack(object? parameter = null)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            _ = GetRequiredNavigationHost(navigation.Name).NavigateBack(parameter);
        }

        /// <summary>Navigates the primary host to the requested view model type.</summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="contract">The contract.</param>
        /// <param name="parameter">The navigation parameter.</param>
        public void NavigateToView<T>(string? contract = null, object? parameter = null)
            where T : class, IRxObject
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            GetRequiredNavigationHost(navigation.Name).Navigate<T>(contract, parameter);
        }

        /// <summary>Navigates the primary host to the requested view model type.</summary>
        /// <param name="rxObject">The view model type.</param>
        /// <param name="contract">The contract.</param>
        /// <param name="parameter">The navigation parameter.</param>
#if NET8_0_OR_GREATER
        [System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Resolving a view from a runtime view model type requires runtime type inspection.")]
        [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Resolving a view from a runtime view model type may require members removed by trimming.")]
#endif
        public void NavigateToView(Type rxObject, string? contract = null, object? parameter = null)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            NavigateResolvedView(GetRequiredNavigationHost(navigation.Name), rxObject, contract, parameter);
        }

        /// <summary>Navigates the primary host to the registered navigation key.</summary>
        /// <typeparam name="TNavigationKey">The caller-facing view model or view lookup key.</typeparam>
        /// <param name="contract">The contract.</param>
        /// <param name="parameter">The navigation parameter.</param>
        public void NavigateTo<TNavigationKey>(string? contract = null, object? parameter = null)
            where TNavigationKey : class
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            NavigateResolvedNavigationKey(GetRequiredNavigationHost(navigation.Name), typeof(TNavigationKey), contract, parameter);
        }

        /// <summary>Navigates the primary host to the registered navigation key.</summary>
        /// <param name="navigationKey">The caller-facing view model or view lookup key.</param>
        /// <param name="contract">The contract.</param>
        /// <param name="parameter">The navigation parameter.</param>
        public void NavigateTo(Type navigationKey, string? contract = null, object? parameter = null)
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            NavigateResolvedNavigationKey(GetRequiredNavigationHost(navigation.Name), navigationKey, contract, parameter);
        }

        /// <summary>Navigates the primary host to the requested view model type and clears history.</summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="contract">The contract.</param>
        /// <param name="parameter">The navigation parameter.</param>
        public void NavigateToViewAndClearHistory<T>(string? contract = null, object? parameter = null)
            where T : class, IRxObject
        {
            ThrowHelper.ThrowIfNull(navigation, nameof(navigation));
            EnsureNavigationHostAvailable();
            GetRequiredNavigationHost(navigation.Name).NavigateAndReset<T>(contract, parameter);
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
                    _ = ASetupCompleted.Subscribe(unused =>
                    {
                        if (navigation.Name is null || !TryGetNavigationHost(navigation.Name, out var host) || host is null)
                        {
                            return;
                        }

                        if (!WhenSetupSubjects.TryGetValue(host.Name, out var whenSetup))
                        {
                            return;
                        }

                        _ = whenSetup.Where(x => x).Subscribe(observer).DisposeWith(disposable);
                    }).DisposeWith(disposable);
                });
                return disposable;
            });
        }
    }

    /// <summary>Creates the host key list for a navigation host registration.</summary>
    /// <param name="navigation">The navigation owner.</param>
    /// <param name="viewHost">The view host.</param>
    /// <returns>The ordered host keys.</returns>
    private static List<string> CreateHostKeys(ISetNavigation navigation, IViewModelRoutedViewHost viewHost)
    {
        var hostKeys = new List<string>();
        AddHostKey(hostKeys, navigation.Name);
        AddHostKey(hostKeys, viewHost.Name);

        if (hostKeys.Count == 0)
        {
            hostKeys.Add($"__crisscross_host_{RuntimeHelpers.GetHashCode(navigation)}");
        }

        return hostKeys;
    }

    /// <summary>Adds a non-empty host key when it is not already present.</summary>
    /// <param name="hostKeys">The host key list.</param>
    /// <param name="hostName">The host name.</param>
    private static void AddHostKey(List<string> hostKeys, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(hostName) || hostKeys.Contains(hostName!))
        {
            return;
        }

        hostKeys.Add(hostName!);
    }

    /// <summary>Adds a dictionary value only when the key is absent.</summary>
    /// <typeparam name="TValue">The dictionary value type.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The dictionary key.</param>
    /// <param name="value">The value to add.</param>
    private static void AddIfMissing<TValue>(Dictionary<string, TValue> dictionary, string key, TValue value)
    {
#if NET8_0_OR_GREATER
        _ = dictionary.TryAdd(key, value);
#else
        if (dictionary.ContainsKey(key))
        {
            return;
        }

        dictionary.Add(key, value);
#endif
    }

    /// <summary>Ensures the registered view host has a stable name.</summary>
    /// <param name="viewHost">The view host.</param>
    /// <param name="hostKey">The primary host key.</param>
    /// <param name="hostKeys">The host key list.</param>
    private static void EnsureViewHostName(IViewModelRoutedViewHost viewHost, string hostKey, List<string> hostKeys)
    {
        if (!string.IsNullOrWhiteSpace(viewHost.Name))
        {
            return;
        }

        viewHost.Name = hostKey;
        AddHostKey(hostKeys, viewHost.Name);
    }

    /// <summary>Registers all aliases for a navigation host.</summary>
    /// <param name="hostKeys">The host keys.</param>
    /// <param name="viewHost">The view host.</param>
    private static void RegisterNavigationHostAliases(List<string> hostKeys, IViewModelRoutedViewHost viewHost)
    {
        lock (_lockObject)
        {
            foreach (var key in hostKeys)
            {
                NavigationHost[key] = viewHost;
                AddIfMissing(WhenSetupSubjects, key, new(1));
                AddIfMissing(CurrentViewDisposable, key, []);
                AddIfMissing(ResultNavigating, key, new Signal<IViewModelNavigatingEventArgs>());
            }
        }
    }

    /// <summary>Runs setup on the host when required.</summary>
    /// <param name="viewHost">The view host.</param>
    private static void SetupViewHostIfRequired(IViewModelRoutedViewHost viewHost)
    {
        if (!viewHost.RequiresSetup)
        {
            return;
        }

        viewHost.Setup();
    }

    /// <summary>Publishes setup completion for a host.</summary>
    /// <param name="hostKey">The host key.</param>
    private static void PublishHostSetup(string hostKey)
    {
        lock (_lockObject)
        {
            if (WhenSetupSubjects.TryGetValue(hostKey, out var hostSetup))
            {
                hostSetup.OnNext(true);
            }
        }
    }

    /// <summary>Gets the active disposable collection for a navigation event.</summary>
    /// <param name="args">The navigation event args.</param>
    /// <returns>The active disposable collection.</returns>
    private static CompositeDisposable GetCurrentViewDisposable(IViewModelNavigationEventArgs args)
    {
        if (args.NavigationType == NavigationType.New && CurrentViewDisposable.TryGetValue(args.HostName!, out var cleanupCompositeDisposable))
        {
            cleanupCompositeDisposable.Dispose();
            CurrentViewDisposable[args.HostName!] = [];
        }

        if (!CurrentViewDisposable.TryGetValue(args.HostName!, out var disposable))
        {
            disposable = [];
            CurrentViewDisposable[args.HostName!] = disposable;
        }

        return disposable;
    }

    /// <summary>Navigates to a resolved view model service when one is available.</summary>
    /// <param name="viewHost">The view host.</param>
    /// <param name="rxObject">The view model type.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
#if NET8_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Resolving a view from a runtime view model type requires runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Resolving a view from a runtime view model type may require members removed by trimming.")]
#endif
    private static void NavigateResolvedView(IViewModelRoutedViewHost viewHost, Type rxObject, string? contract, object? parameter)
    {
        ThrowHelper.ThrowIfNull(rxObject, nameof(rxObject));
        if (AppLocator.Current.GetService(rxObject, contract) is not IRxObject toViewModel)
        {
            return;
        }

        viewHost.Navigate(toViewModel, contract, parameter);
    }

    /// <summary>Navigates to the navigation pair resolved from a caller-facing key.</summary>
    /// <param name="viewHost">The view host.</param>
    /// <param name="navigationKey">The caller-facing view model or view lookup key.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    private static void NavigateResolvedNavigationKey(IViewModelRoutedViewHost viewHost, Type navigationKey, string? contract, object? parameter)
    {
        if (viewHost is not IResolvedViewModelRoutedViewHost resolvedViewHost)
        {
            throw new InvalidOperationException("The registered navigation host does not support resolved ViewModel/View navigation.");
        }

        var resolution = ResolveNavigationKey(navigationKey, contract, parameter);
        resolvedViewHost.Navigate(resolution);
    }

    /// <summary>Resolves a caller-facing navigation key through the registered bidirectional navigator.</summary>
    /// <param name="navigationKey">The caller-facing view model or view lookup key.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    /// <returns>The navigation resolution.</returns>
    private static NavigationResolution ResolveNavigationKey(Type navigationKey, string? contract, object? parameter)
    {
        ThrowHelper.ThrowIfNull(navigationKey, nameof(navigationKey));
        var navigator = GetRequiredNavigator();
        try
        {
            return navigator.NavigateViewModel(navigationKey, contract, parameter).FirstAsync().GetAwaiter().GetResult();
        }
        catch (NavigationResolutionException)
        {
            return navigator.NavigateView(navigationKey, contract, parameter).FirstAsync().GetAwaiter().GetResult();
        }
    }

    /// <summary>Gets the registered bidirectional navigator.</summary>
    /// <returns>The registered navigator.</returns>
    private static IBidirectionalNavigator GetRequiredNavigator() =>
        AppLocator.Current.GetService<IBidirectionalNavigator>() ??
            AppLocator.Current.GetService<INavigationRegistry>()?.CreateNavigator() ??
            throw new InvalidOperationException("No bidirectional navigation registry has been registered.");

    /// <summary>Throws when no navigation hosts are registered.</summary>
    private static void EnsureNavigationHostAvailable()
    {
        if (NavigationHost.Count != 0)
        {
            return;
        }

        throw new InvalidOperationException("No navigation host registered, please ensure that the NavigationShell has a Name.");
    }

    /// <summary>Attempts to resolve a navigation host by name.</summary>
    /// <param name="hostName">The requested host name.</param>
    /// <param name="host">The resolved host.</param>
    /// <returns><c>true</c> when a host was found; otherwise, <c>false</c>.</returns>
    private static bool TryGetNavigationHost(string? hostName, out IViewModelRoutedViewHost? host)
    {
        host = null;

        if (NavigationHost.Count == 0)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(hostName))
        {
            host = NavigationHost.First().Value;
            return true;
        }

        if (NavigationHost.TryGetValue(hostName!, out host))
        {
            return true;
        }

        foreach (var existingHost in NavigationHost.Values)
        {
            if (string.Equals(existingHost.Name, hostName, StringComparison.Ordinal))
            {
                host = existingHost;
                AliasNavigationHost(hostName!, host);
                return true;
            }
        }

        return false;
    }

    /// <summary>Gets a required navigation host or throws a descriptive exception.</summary>
    /// <param name="hostName">The requested host name.</param>
    /// <returns>The resolved host.</returns>
    private static IViewModelRoutedViewHost GetRequiredNavigationHost(string? hostName)
    {
        if (TryGetNavigationHost(hostName, out var host) && host is not null)
        {
            return host;
        }

        var requestedHostName = string.IsNullOrWhiteSpace(hostName) ? "<default>" : hostName;
        throw new KeyNotFoundException($"Navigation host '{requestedHostName}' has not been registered.");
    }

    /// <summary>Adds an alias for an existing navigation host.</summary>
    /// <param name="hostName">The alias host name.</param>
    /// <param name="host">The existing host.</param>
    private static void AliasNavigationHost(string hostName, IViewModelRoutedViewHost host)
    {
        if (NavigationHost.ContainsKey(hostName) || string.IsNullOrWhiteSpace(hostName))
        {
            return;
        }

        lock (_lockObject)
        {
            AddIfMissing(NavigationHost, hostName, host);

            if (!WhenSetupSubjects.TryGetValue(hostName, out var whenSetup))
            {
                whenSetup = new(1);
                WhenSetupSubjects.Add(hostName, whenSetup);
            }

            AddIfMissing(CurrentViewDisposable, hostName, []);
            AddIfMissing(ResultNavigating, hostName, new Signal<IViewModelNavigatingEventArgs>());
        }
    }
}
