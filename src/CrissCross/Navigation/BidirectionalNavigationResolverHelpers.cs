// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
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
    public static NavigationRegistrationDescriptor GetDescriptor(
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

        var knownContracts = registrations
            .Keys.Where(candidate => candidate.SourceKind == sourceKind && candidate.ServiceType == sourceKey)
            .Select(candidate => candidate.Contract)
            .ToArray();

        throw new NavigationResolutionException(sourceKind, sourceKey, normalizedContract, knownContracts);
    }

    /// <summary>Converts an untyped navigation resolution to a typed resolution.</summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="resolution">The untyped resolution.</param>
    /// <returns>The typed resolution.</returns>
    public static NavigationResolution<TViewModel, TView> ToTyped<TViewModel, TView>(NavigationResolution resolution)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel> =>
        new(
            (TViewModel)resolution.ViewModel,
            (TView)resolution.View,
            resolution.Contract,
            resolution.Parameter,
            resolution.NavigationType);
}
