// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Navigation cache for storing page instances.
/// </summary>
internal class NavigationCache
{
    private readonly Dictionary<Type, object?> _entries = [];

    /// <summary>
    /// Remembers the specified entry type.
    /// </summary>
    /// <param name="entryType">Type of the entry.</param>
    /// <param name="cacheMode">The cache mode.</param>
    /// <param name="generate">The generate function.</param>
    /// <returns>The cached or generated instance.</returns>
    public object? Remember(Type? entryType, NavigationCacheMode cacheMode, Func<object?> generate)
    {
        if (entryType == null)
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

        if (!_entries.TryGetValue(entryType, out var value))
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(
                $"{entryType} not found in cache, generating instance using action...");
#endif

            value = generate.Invoke();

            _entries.Add(entryType, value);
        }

#if DEBUG
        System.Diagnostics.Debug.WriteLine($"{entryType} found in cache.");
#endif

        return value;
    }

    /// <summary>
    /// Clears the cache.
    /// </summary>
    public void Clear() => _entries.Clear();
}
