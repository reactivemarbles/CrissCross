// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;

namespace CrissCross;

/// <summary>
/// Helper methods for bidirectional navigation resolution.
/// </summary>
internal static class BidirectionalNavigationResolverHelpers
{
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

        var knownContracts = registrations.Keys
            .Where(candidate => candidate.SourceKind == sourceKind && candidate.ServiceType == sourceKey)
            .Select(candidate => candidate.Contract)
            .ToArray();

        throw new NavigationResolutionException(sourceKind, sourceKey, normalizedContract, knownContracts);
    }

    public static NavigationResolution<TViewModel, TView> ToTyped<TViewModel, TView>(NavigationResolution resolution)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel> => new((TViewModel)resolution.ViewModel, (TView)resolution.View, resolution.Contract, resolution.Parameter, resolution.NavigationType);
}
