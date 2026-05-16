// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace CrissCross;

/// <summary>
/// Identifies one side of a navigation registration by source kind, service type, and contract.
/// </summary>
internal readonly struct NavigationLookupKey(NavigationSourceKind sourceKind, Type serviceType, string? contract) : IEquatable<NavigationLookupKey>
{
    public NavigationSourceKind SourceKind { get; } = sourceKind;

    public Type ServiceType { get; } = serviceType;

    public string? Contract { get; } = contract;

    public static bool operator ==(NavigationLookupKey left, NavigationLookupKey right) => left.Equals(right);

    public static bool operator !=(NavigationLookupKey left, NavigationLookupKey right) => !left.Equals(right);

    public bool Equals(NavigationLookupKey other) =>
        SourceKind == other.SourceKind &&
        ServiceType == other.ServiceType &&
        string.Equals(Contract, other.Contract, StringComparison.Ordinal);

    public override bool Equals(object? obj) => obj is NavigationLookupKey other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = (hash * 23) + SourceKind.GetHashCode();
            hash = (hash * 23) + ServiceType.GetHashCode();
            hash = (hash * 23) + (Contract is null ? 0 : StringComparer.Ordinal.GetHashCode(Contract));
            return hash;
        }
    }
}
