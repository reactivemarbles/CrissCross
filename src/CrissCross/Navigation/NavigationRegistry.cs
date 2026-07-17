// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;

namespace CrissCross;

/// <summary>Default in-memory, AOT-friendly bidirectional navigation registry.</summary>
public sealed class NavigationRegistry : INavigationRegistry
{
    /// <summary>Stores registrations keyed by view model type and contract.</summary>
    private readonly Dictionary<NavigationLookupKey, NavigationRegistrationDescriptor> _viewModelRegistrations = [];

    /// <summary>Stores registrations keyed by view type and contract.</summary>
    private readonly Dictionary<NavigationLookupKey, NavigationRegistrationDescriptor> _viewRegistrations = [];

    /// <inheritdoc/>
    public INavigationRegistry Register<TViewModel, TView>(
        Func<IServiceProvider, TViewModel> createViewModel,
        Func<IServiceProvider, TView> createView,
        string? contract)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel> =>
        Register(
            new NavigationRegistration<TViewModel, TViewModel, TView, TView>(createViewModel, createView)
            {
                Contract = contract,
            });

    /// <inheritdoc/>
    public INavigationRegistry Register<TViewModelKey, TViewModel, TViewKey, TView>(
        NavigationRegistration<TViewModelKey, TViewModel, TViewKey, TView> registration)
        where TViewModelKey : class
        where TViewModel : class, TViewModelKey, IRxObject
        where TViewKey : class
        where TView : class, TViewKey, IViewFor<TViewModel>
    {
        ThrowHelper.ThrowIfNull(registration, nameof(registration));
        ThrowHelper.ThrowIfNull(registration.CreateViewModel, nameof(registration.CreateViewModel));
        ThrowHelper.ThrowIfNull(registration.CreateView, nameof(registration.CreateView));

        var normalizedContract = NavigationContract.Normalize(registration.Contract);
        var viewModelKey = new NavigationLookupKey(
            NavigationSourceKind.ViewModel,
            typeof(TViewModelKey),
            normalizedContract);
        var viewKey = new NavigationLookupKey(NavigationSourceKind.View, typeof(TViewKey), normalizedContract);

        EnsureAvailable(_viewModelRegistrations, viewModelKey);
        EnsureAvailable(_viewRegistrations, viewKey);

        var descriptor = new NavigationRegistrationDescriptor(
            typeof(TViewModelKey),
            typeof(TViewModel),
            typeof(TViewKey),
            typeof(TView),
            normalizedContract,
            serviceProvider => registration.CreateViewModel(serviceProvider),
            serviceProvider => registration.CreateView(serviceProvider));

        _viewModelRegistrations.Add(viewModelKey, descriptor);
        _viewRegistrations.Add(viewKey, descriptor);
        return this;
    }

    /// <inheritdoc/>
    public IBidirectionalNavigator CreateNavigator(IServiceProvider? serviceProvider) =>
        new BidirectionalNavigator(
            _viewModelRegistrations.Values.ToArray(),
            serviceProvider ?? EmptyServiceProvider.Instance);

    /// <summary>Throws when a lookup key is already registered.</summary>
    /// <param name="registrations">The existing registration map.</param>
    /// <param name="key">The lookup key to validate.</param>
    private static void EnsureAvailable(
        Dictionary<NavigationLookupKey, NavigationRegistrationDescriptor> registrations,
        NavigationLookupKey key)
    {
        if (!registrations.ContainsKey(key))
        {
            return;
        }

        throw new NavigationRegistrationException(key.SourceKind, key.ServiceType, key.Contract);
    }
}
