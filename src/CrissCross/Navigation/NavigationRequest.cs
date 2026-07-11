// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Threading;

namespace CrissCross;

/// <summary>Describes a platform-neutral navigation request before host mutation.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NavigationRequest"/> class.
/// </remarks>
/// <param name="sourceKind">The source side used to resolve the navigation pair.</param>
/// <param name="sourceInstance">The optional source instance supplied by the caller.</param>
/// <param name="sourceKey">The caller-facing source key.</param>
/// <param name="contract">The optional navigation contract.</param>
/// <param name="parameter">The optional navigation parameter.</param>
/// <param name="navigationType">The requested navigation operation type.</param>
/// <param name="cancellationToken">The cancellation token observed before resolution.</param>
public sealed class NavigationRequest(
    NavigationSourceKind sourceKind,
    object? sourceInstance,
    Type sourceKey,
    string? contract,
    object? parameter,
    NavigationType navigationType,
    CancellationToken cancellationToken)
{
    /// <summary>Gets the source side used to resolve the navigation pair.</summary>
    public NavigationSourceKind SourceKind { get; } = sourceKind;

    /// <summary>Gets the optional source instance supplied by the caller.</summary>
    public object? SourceInstance { get; } = sourceInstance;

    /// <summary>Gets the caller-facing source key.</summary>
    public Type SourceKey { get; } = sourceKey ?? throw new ArgumentNullException(nameof(sourceKey));

    /// <summary>Gets the normalized navigation contract.</summary>
    public string? Contract { get; } = NavigationContract.Normalize(contract);

    /// <summary>Gets the optional navigation parameter.</summary>
    public object? Parameter { get; } = parameter;

    /// <summary>Gets the requested navigation operation type.</summary>
    public NavigationType NavigationType { get; } = navigationType;

    /// <summary>Gets the cancellation token observed before resolution.</summary>
    public CancellationToken CancellationToken { get; } = cancellationToken;
}
