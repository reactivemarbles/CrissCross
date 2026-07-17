// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>I Use Navigation.</summary>
public interface IUseNavigation : IAmBuilt
{
    /// <summary>Gets the name.</summary>
    /// <value>
    /// The name.
    /// </value>
    new string? Name { get; }
}
