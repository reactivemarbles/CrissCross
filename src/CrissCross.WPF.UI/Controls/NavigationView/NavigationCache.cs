// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides the NavigationCache member.</summary>
internal sealed class NavigationCache
{
    /// <summary>Stores the _entires value.</summary>
    private readonly Dictionary<Type, object?> _entires = [];

    /// <summary>Provides the Remember member.</summary>
    /// <param name="entryType">The entryType value.</param>
    /// <param name="cacheMode">The cacheMode value.</param>
    /// <param name="generate">The generate value.</param>
    /// <returns>The result.</returns>
    public object? Remember(Type? entryType, NavigationCacheMode cacheMode, Func<object?> generate)
    {
        if (entryType is null)
        {
            return null;
        }

        if (cacheMode == NavigationCacheMode.Disabled)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(
                $"Cache for {entryType} is disabled. Generating instance using action...");
#endif

            return generate.Invoke();
        }

        if (!_entires.TryGetValue(entryType, out var value))
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"{entryType} not found in cache, generating instance using action...");
#endif

            value = generate.Invoke();

            _entires.Add(entryType, value);
        }

#if DEBUG
        System.Diagnostics.Debug.WriteLine($"{entryType} found in cache.");
#endif

        return value;
    }
}
