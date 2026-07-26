// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using ReactiveUI;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Helper methods for bidirectional navigation resolution.</summary>
internal static class BidirectionalNavigationResolverHelpers
{
    /// <summary>Gets the descriptor for a navigation lookup.</summary>
    /// <param name="registrations">The registration map.</param>
    /// <param name="sourceKind">The source kind.</param>
    /// <param name="sourceKey">The source key.</param>
    /// <param name="contract">The requested contract.</param>
    /// <returns>The matching descriptor.</returns>
    internal static NavigationRegistrationDescriptor GetDescriptor(
        IReadOnlyDictionary<NavigationLookupKey, NavigationRegistrationDescriptor> registrations,
        NavigationSourceKind sourceKind,
        Type sourceKey,
        string? contract)
    {
        var normalizedContract = NavigationContract.Normalize(contract);
        var key = new NavigationLookupKey(sourceKind, sourceKey, normalizedContract);
        if (registrations.TryGetValue(key, out var descriptor))
        {
            return descriptor;
        }

        var knownContracts = new List<string?>();
        foreach (var candidate in registrations.Keys)
        {
            if (candidate.SourceKind == sourceKind && candidate.ServiceType == sourceKey)
            {
                knownContracts.Add(candidate.Contract);
            }
        }

        throw new NavigationResolutionException(sourceKind, sourceKey, normalizedContract, knownContracts);
    }

    /// <summary>Converts an untyped navigation resolution to a typed resolution.</summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="resolution">The untyped resolution.</param>
    /// <returns>The typed resolution.</returns>
    internal static NavigationResolution<TViewModel, TView> ToTyped<TViewModel, TView>(NavigationResolution resolution)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel> =>
        new(
            (TViewModel)resolution.ViewModel,
            (TView)resolution.View,
            resolution.Contract,
            resolution.Parameter,
            resolution.NavigationType);
}
