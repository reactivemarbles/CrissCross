// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>Normalizes optional navigation contracts.</summary>
internal static class NavigationContract
{
    /// <summary>Normalizes empty navigation contracts to null.</summary>
    /// <param name="contract">The requested contract.</param>
    /// <returns>The normalized contract.</returns>
    public static string? Normalize(string? contract) => string.IsNullOrWhiteSpace(contract) ? null : contract;

    /// <summary>Formats a contract for diagnostics.</summary>
    /// <param name="contract">The requested contract.</param>
    /// <returns>The diagnostic display value.</returns>
    public static string ToDisplay(string? contract) => Normalize(contract) ?? "<default>";
}
