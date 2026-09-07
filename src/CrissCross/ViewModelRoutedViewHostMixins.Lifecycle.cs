// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Provides navigation host lifecycle helpers.</summary>
public static partial class ViewModelRoutedViewHostMixins
{
    /// <summary>Unregisters every navigation host alias still owned by the specified host.</summary>
    /// <param name="viewHost">The view host being disposed.</param>
    internal static void UnregisterNavigationHost(IViewModelRoutedViewHost viewHost)
    {
        ThrowHelper.ThrowIfNull(viewHost, nameof(viewHost));
        var hostKeys = new List<string>();

        lock (_lockObject)
        {
            foreach (var pair in NavigationHost)
            {
                if (ReferenceEquals(pair.Value, viewHost))
                {
                    hostKeys.Add(pair.Key);
                }
            }

            foreach (var hostKey in hostKeys)
            {
                _ = NavigationHost.Remove(hostKey);
                DisposeAndRemove(CurrentViewDisposable, hostKey);
                DisposeAndRemove(ResultNavigating, hostKey);
                DisposeAndRemove(WhenSetupSubjects, hostKey);
            }
        }
    }

    /// <summary>Disposes and removes a disposable dictionary value.</summary>
    /// <typeparam name="TDisposable">The disposable value type.</typeparam>
    /// <param name="dictionary">The dictionary to update.</param>
    /// <param name="key">The key to remove.</param>
    private static void DisposeAndRemove<TDisposable>(Dictionary<string, TDisposable> dictionary, string key)
        where TDisposable : IDisposable
    {
        if (!dictionary.TryGetValue(key, out var disposable))
        {
            return;
        }

        _ = dictionary.Remove(key);
        disposable.Dispose();
    }
}
