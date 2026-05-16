// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace CrissCross;

/// <summary>
/// Represents a deterministic bidirectional navigation resolution failure.
/// </summary>
public sealed class NavigationResolutionException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationResolutionException"/> class.
    /// </summary>
    public NavigationResolutionException()
    {
        SourceKind = NavigationSourceKind.ViewModel;
        SourceKey = typeof(object);
        KnownContracts = Array.Empty<string?>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationResolutionException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public NavigationResolutionException(string? message)
        : base(message)
    {
        SourceKind = NavigationSourceKind.ViewModel;
        SourceKey = typeof(object);
        KnownContracts = Array.Empty<string?>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationResolutionException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NavigationResolutionException(string? message, Exception? innerException)
        : base(message, innerException)
    {
        SourceKind = NavigationSourceKind.ViewModel;
        SourceKey = typeof(object);
        KnownContracts = Array.Empty<string?>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationResolutionException"/> class.
    /// </summary>
    /// <param name="sourceKind">The source side used for lookup.</param>
    /// <param name="sourceKey">The caller-facing source key.</param>
    /// <param name="contract">The requested navigation contract.</param>
    /// <param name="knownContracts">The contracts known for the source key.</param>
    public NavigationResolutionException(NavigationSourceKind sourceKind, Type sourceKey, string? contract, IEnumerable<string?> knownContracts)
        : base($"No {sourceKind} navigation registration exists for '{sourceKey?.FullName}' with contract '{NavigationContract.ToDisplay(contract)}'.")
    {
        SourceKind = sourceKind;
        SourceKey = sourceKey ?? throw new ArgumentNullException(nameof(sourceKey));
        Contract = NavigationContract.Normalize(contract);
        KnownContracts = new ReadOnlyCollection<string?>(knownContracts.Select(NavigationContract.Normalize).Distinct(StringComparer.Ordinal).ToArray());
    }

    /// <summary>
    /// Gets the source side used for lookup.
    /// </summary>
    public NavigationSourceKind SourceKind { get; }

    /// <summary>
    /// Gets the caller-facing source key.
    /// </summary>
    public Type SourceKey { get; }

    /// <summary>
    /// Gets the requested normalized navigation contract.
    /// </summary>
    public string? Contract { get; }

    /// <summary>
    /// Gets the contracts known for the source key.
    /// </summary>
    public IReadOnlyList<string?> KnownContracts { get; }
}
