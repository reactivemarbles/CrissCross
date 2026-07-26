// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Normalizes optional navigation contracts.</summary>
internal static class NavigationContract
{
    /// <summary>Normalizes empty navigation contracts to null.</summary>
    /// <param name="contract">The requested contract.</param>
    /// <returns>The normalized contract.</returns>
    internal static string? Normalize(string? contract) => string.IsNullOrWhiteSpace(contract) ? null : contract;

    /// <summary>Formats a contract for diagnostics.</summary>
    /// <param name="contract">The requested contract.</param>
    /// <returns>The diagnostic display value.</returns>
    internal static string ToDisplay(string? contract) => Normalize(contract) ?? "<default>";
}
