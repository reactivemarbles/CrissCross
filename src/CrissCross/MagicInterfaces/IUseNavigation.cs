// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
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
