// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>Identifies a typed navigation lookup key and its request options.</summary>
/// <typeparam name="TKey">The navigation lookup key type.</typeparam>
public sealed class NavigationKeyRequest<TKey>
    where TKey : class
{
    /// <summary>Gets the runtime navigation lookup key.</summary>
    public System.Type Key => typeof(TKey);

    /// <summary>Gets or sets the navigation request options.</summary>
    public NavigationRequestOptions Options { get; set; } = new();
}
