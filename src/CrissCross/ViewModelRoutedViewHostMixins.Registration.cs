// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Provides navigation helpers for routed view hosts.</summary>
public static partial class ViewModelRoutedViewHostMixins
{
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
}
