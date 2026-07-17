// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ReactiveUI;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Default bidirectional navigation resolver over an immutable registration snapshot.</summary>
internal sealed class BidirectionalNavigator : IBidirectionalNavigator
{
    /// <summary>Provides services used to create registered views and view models.</summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>Stores registrations keyed by view model type and contract.</summary>
    private readonly Dictionary<NavigationLookupKey, NavigationRegistrationDescriptor> _viewModelRegistrations;

    /// <summary>Stores registrations keyed by view type and contract.</summary>
    private readonly Dictionary<NavigationLookupKey, NavigationRegistrationDescriptor> _viewRegistrations;

    /// <summary>Initializes a new instance of the <see cref="BidirectionalNavigator"/> class.</summary>
    /// <param name="registrations">The navigation registrations.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public BidirectionalNavigator(
        IEnumerable<NavigationRegistrationDescriptor> registrations,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        var descriptors = registrations.ToArray();
        _viewModelRegistrations = descriptors.ToDictionary(descriptor => new NavigationLookupKey(
            NavigationSourceKind.ViewModel,
            descriptor.ViewModelKey,
            descriptor.Contract));
        _viewRegistrations = descriptors.ToDictionary(descriptor => new NavigationLookupKey(
            NavigationSourceKind.View,
            descriptor.ViewKey,
            descriptor.Contract));
    }

    /// <inheritdoc/>
    public IObservable<NavigationResolution<TViewModel, TView>> NavigateViewModel<TViewModel, TView>(
        ViewModelNavigationRequest<TViewModel, TView> request)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel>
    {
        ThrowHelper.ThrowIfNull(request, nameof(request));
        ThrowHelper.ThrowIfNull(request.Options, nameof(request.Options));

        return ResolveViewModel(
                typeof(TViewModel),
                request.ViewModel,
                request.Options.Contract,
                request.Options.Parameter,
                NavigationType.New,
                request.Options.CancellationToken)
            .Select(BidirectionalNavigationResolverHelpers.ToTyped<TViewModel, TView>);
    }

    /// <inheritdoc/>
    public IObservable<NavigationResolution> NavigateViewModel(Type viewModelKey, NavigationRequestOptions options)
    {
        ThrowHelper.ThrowIfNull(options, nameof(options));
        return ResolveViewModel(
            viewModelKey,
            null,
            options.Contract,
            options.Parameter,
            NavigationType.New,
            options.CancellationToken);
    }

    /// <inheritdoc/>
    public IObservable<NavigationResolution<TViewModel, TView>> NavigateView<TViewModel, TView>(
        ViewNavigationRequest<TViewModel, TView> request)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel>
    {
        ThrowHelper.ThrowIfNull(request, nameof(request));
        ThrowHelper.ThrowIfNull(request.Options, nameof(request.Options));

        return ResolveView(
                typeof(TView),
                request.View,
                request.Options.Contract,
                request.Options.Parameter,
                NavigationType.New,
                request.Options.CancellationToken)
            .Select(BidirectionalNavigationResolverHelpers.ToTyped<TViewModel, TView>);
    }

    /// <inheritdoc/>
    public IObservable<NavigationResolution> NavigateView(Type viewKey, NavigationRequestOptions options)
    {
        ThrowHelper.ThrowIfNull(options, nameof(options));
        return ResolveView(
            viewKey,
            null,
            options.Contract,
            options.Parameter,
            NavigationType.New,
            options.CancellationToken);
    }

    /// <summary>Resolves a view model navigation request.</summary>
    /// <param name="viewModelKey">The view model lookup type.</param>
    /// <param name="suppliedViewModel">The optional supplied view model.</param>
    /// <param name="contract">The optional navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    /// <param name="navigationType">The navigation type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The resolved navigation result.</returns>
    private IObservable<NavigationResolution> ResolveViewModel(
        Type viewModelKey,
        IRxObject? suppliedViewModel,
        string? contract,
        object? parameter,
        NavigationType navigationType,
        CancellationToken cancellationToken) =>
        Observable.Defer(() =>
        {
            ThrowHelper.ThrowIfNull(viewModelKey, nameof(viewModelKey));

            cancellationToken.ThrowIfCancellationRequested();
            var descriptor = BidirectionalNavigationResolverHelpers.GetDescriptor(
                _viewModelRegistrations,
                NavigationSourceKind.ViewModel,
                viewModelKey,
                contract);
            var viewModel = suppliedViewModel ?? descriptor.CreateViewModel(_serviceProvider);
            cancellationToken.ThrowIfCancellationRequested();
            var view = descriptor.CreateView(_serviceProvider);
            view.ViewModel = viewModel;
            cancellationToken.ThrowIfCancellationRequested();
            return Observable.Return(
                new NavigationResolution(viewModel, view, descriptor.Contract, parameter, navigationType));
        });

    /// <summary>Resolves a view navigation request.</summary>
    /// <param name="viewKey">The view lookup type.</param>
    /// <param name="suppliedView">The optional supplied view.</param>
    /// <param name="contract">The optional navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    /// <param name="navigationType">The navigation type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The resolved navigation result.</returns>
    private IObservable<NavigationResolution> ResolveView(
        Type viewKey,
        IViewFor? suppliedView,
        string? contract,
        object? parameter,
        NavigationType navigationType,
        CancellationToken cancellationToken) =>
        Observable.Defer(() =>
        {
            ThrowHelper.ThrowIfNull(viewKey, nameof(viewKey));

            cancellationToken.ThrowIfCancellationRequested();
            var descriptor = BidirectionalNavigationResolverHelpers.GetDescriptor(
                _viewRegistrations,
                NavigationSourceKind.View,
                viewKey,
                contract);
            var view = suppliedView ?? descriptor.CreateView(_serviceProvider);
            cancellationToken.ThrowIfCancellationRequested();
            var viewModel = descriptor.ViewModelImplementation.IsInstanceOfType(view.ViewModel)
                ? (IRxObject)view.ViewModel!
                : descriptor.CreateViewModel(_serviceProvider);
            view.ViewModel = viewModel;
            cancellationToken.ThrowIfCancellationRequested();
            return Observable.Return(
                new NavigationResolution(viewModel, view, descriptor.Contract, parameter, navigationType));
        });
}
