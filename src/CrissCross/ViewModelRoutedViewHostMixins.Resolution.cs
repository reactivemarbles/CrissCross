// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Splat;

namespace CrissCross;

/// <summary>Provides navigation helpers for routed view hosts.</summary>
public static partial class ViewModelRoutedViewHostMixins
{
    /// <summary>Gets the active disposable collection for a navigation event.</summary>
    /// <param name="args">The navigation event args.</param>
    /// <returns>The active disposable collection.</returns>
    private static CompositeDisposable GetCurrentViewDisposable(IViewModelNavigationEventArgs args)
    {
        if (
            args.NavigationType == NavigationType.New
            && CurrentViewDisposable.TryGetValue(args.HostName!, out var cleanupCompositeDisposable))
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
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(
        "Resolving a view from a runtime view model type requires runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Resolving a view from a runtime view model type may require members removed by trimming.")]
#endif
    private static void NavigateResolvedView(
        IViewModelRoutedViewHost viewHost,
        Type rxObject,
        string? contract,
        object? parameter)
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
    private static void NavigateResolvedNavigationKey(
        IViewModelRoutedViewHost viewHost,
        Type navigationKey,
        string? contract,
        object? parameter)
    {
        if (viewHost is not IResolvedViewModelRoutedViewHost resolvedViewHost)
        {
            throw new InvalidOperationException(
                "The registered navigation host does not support resolved ViewModel/View navigation.");
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
            return navigator
                .NavigateViewModel(
                    navigationKey,
                    new NavigationRequestOptions { Contract = contract, Parameter = parameter })
                .FirstAsync()
                .GetAwaiter()
                .GetResult();
        }
        catch (NavigationResolutionException)
        {
            return navigator
                .NavigateView(
                    navigationKey,
                    new NavigationRequestOptions { Contract = contract, Parameter = parameter })
                .FirstAsync()
                .GetAwaiter()
                .GetResult();
        }
    }

    /// <summary>Gets the registered bidirectional navigator.</summary>
    /// <returns>The registered navigator.</returns>
    private static IBidirectionalNavigator GetRequiredNavigator() =>
        AppLocator.Current.GetService<IBidirectionalNavigator>()
        ?? AppLocator.Current.GetService<INavigationRegistry>()?.CreateNavigator()
        ?? throw new InvalidOperationException("No bidirectional navigation registry has been registered.");

    /// <summary>Throws when no navigation hosts are registered.</summary>
    private static void EnsureNavigationHostAvailable()
    {
        if (NavigationHost.Count != 0)
        {
            return;
        }

        throw new InvalidOperationException(
            "No navigation host registered, please ensure that the NavigationShell has a Name.");
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
