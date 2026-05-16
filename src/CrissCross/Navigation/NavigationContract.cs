// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>
/// Normalizes optional navigation contracts.
/// </summary>
internal static class NavigationContract
{
    public static string? Normalize(string? contract) => string.IsNullOrWhiteSpace(contract) ? null : contract;

    public static string ToDisplay(string? contract) => Normalize(contract) ?? "<default>";
}
