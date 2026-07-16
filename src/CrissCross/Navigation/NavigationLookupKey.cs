// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace CrissCross;

/// <summary>Identifies one side of a navigation registration by source kind, service type, and contract.</summary>
/// <param name="sourceKind">The navigation source kind.</param>
/// <param name="serviceType">The registered service type.</param>
/// <param name="contract">The optional navigation contract.</param>
internal readonly struct NavigationLookupKey(NavigationSourceKind sourceKind, Type serviceType, string? contract)
    : IEquatable<NavigationLookupKey>
{
    /// <summary>Gets the navigation source kind.</summary>
    public NavigationSourceKind SourceKind { get; } = sourceKind;

    /// <summary>Gets the registered service type.</summary>
    public Type ServiceType { get; } = serviceType;

    /// <summary>Gets the optional navigation contract.</summary>
    public string? Contract { get; } = contract;

    /// <summary>Compares two lookup keys for equality.</summary>
    /// <param name="left">The left lookup key.</param>
    /// <param name="right">The right lookup key.</param>
    /// <returns><c>true</c> when the keys are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(NavigationLookupKey left, NavigationLookupKey right) => left.Equals(right);

    /// <summary>Compares two lookup keys for inequality.</summary>
    /// <param name="left">The left lookup key.</param>
    /// <param name="right">The right lookup key.</param>
    /// <returns><c>true</c> when the keys are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(NavigationLookupKey left, NavigationLookupKey right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(NavigationLookupKey other) =>
        SourceKind == other.SourceKind
        && ServiceType == other.ServiceType
        && string.Equals(Contract, other.Contract, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is NavigationLookupKey other && Equals(other);

    /// <inheritdoc/>
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
