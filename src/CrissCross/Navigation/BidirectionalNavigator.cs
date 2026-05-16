// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using ReactiveUI;

namespace CrissCross;

/// <summary>
/// Default bidirectional navigation resolver over an immutable registration snapshot.
/// </summary>
internal sealed class BidirectionalNavigator : IBidirectionalNavigator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<NavigationLookupKey, NavigationRegistrationDescriptor> _viewModelRegistrations;
    private readonly Dictionary<NavigationLookupKey, NavigationRegistrationDescriptor> _viewRegistrations;

    public BidirectionalNavigator(IEnumerable<NavigationRegistrationDescriptor> registrations, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        var descriptors = registrations.ToArray();
        _viewModelRegistrations = descriptors.ToDictionary(
            descriptor => new NavigationLookupKey(NavigationSourceKind.ViewModel, descriptor.ViewModelKey, descriptor.Contract));
        _viewRegistrations = descriptors.ToDictionary(
            descriptor => new NavigationLookupKey(NavigationSourceKind.View, descriptor.ViewKey, descriptor.Contract));
    }

    public IObservable<NavigationResolution<TViewModel, TView>> NavigateViewModel<TViewModel, TView>(
        TViewModel viewModel,
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel>
    {
        if (viewModel == null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        return ResolveViewModel(typeof(TViewModel), viewModel, contract, parameter, NavigationType.New, cancellationToken)
            .Select(BidirectionalNavigationResolverHelpers.ToTyped<TViewModel, TView>);
    }

    public IObservable<NavigationResolution<TViewModel, TView>> NavigateViewModel<TViewModel, TView>(
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel> => ResolveViewModel(typeof(TViewModel), null, contract, parameter, NavigationType.New, cancellationToken)
            .Select(BidirectionalNavigationResolverHelpers.ToTyped<TViewModel, TView>);

    public IObservable<NavigationResolution> NavigateViewModel<TViewModelKey>(
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default)
        where TViewModelKey : class => NavigateViewModel(typeof(TViewModelKey), contract, parameter, cancellationToken);

    public IObservable<NavigationResolution> NavigateViewModel(
        Type viewModelKey,
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default) => ResolveViewModel(viewModelKey, null, contract, parameter, NavigationType.New, cancellationToken);

    public IObservable<NavigationResolution<TViewModel, TView>> NavigateView<TViewModel, TView>(
        TView view,
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel>
    {
        if (view == null)
        {
            throw new ArgumentNullException(nameof(view));
        }

        return ResolveView(typeof(TView), view, contract, parameter, NavigationType.New, cancellationToken)
            .Select(BidirectionalNavigationResolverHelpers.ToTyped<TViewModel, TView>);
    }

    public IObservable<NavigationResolution<TViewModel, TView>> NavigateView<TViewModel, TView>(
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel> => ResolveView(typeof(TView), null, contract, parameter, NavigationType.New, cancellationToken)
            .Select(BidirectionalNavigationResolverHelpers.ToTyped<TViewModel, TView>);

    public IObservable<NavigationResolution> NavigateView<TViewKey>(
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default)
        where TViewKey : class => NavigateView(typeof(TViewKey), contract, parameter, cancellationToken);

    public IObservable<NavigationResolution> NavigateView(
        Type viewKey,
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default) => ResolveView(viewKey, null, contract, parameter, NavigationType.New, cancellationToken);

    private IObservable<NavigationResolution> ResolveViewModel(
        Type viewModelKey,
        IRxObject? suppliedViewModel,
        string? contract,
        object? parameter,
        NavigationType navigationType,
        CancellationToken cancellationToken) => Observable.Defer(() =>
        {
            if (viewModelKey == null)
            {
                throw new ArgumentNullException(nameof(viewModelKey));
            }

            cancellationToken.ThrowIfCancellationRequested();
            var descriptor = BidirectionalNavigationResolverHelpers.GetDescriptor(_viewModelRegistrations, NavigationSourceKind.ViewModel, viewModelKey, contract);
            var viewModel = suppliedViewModel ?? descriptor.CreateViewModel(_serviceProvider);
            cancellationToken.ThrowIfCancellationRequested();
            var view = descriptor.CreateView(_serviceProvider);
            view.ViewModel = viewModel;
            cancellationToken.ThrowIfCancellationRequested();
            return Observable.Return(new NavigationResolution(viewModel, view, descriptor.Contract, parameter, navigationType));
        });

    private IObservable<NavigationResolution> ResolveView(
        Type viewKey,
        IViewFor? suppliedView,
        string? contract,
        object? parameter,
        NavigationType navigationType,
        CancellationToken cancellationToken) => Observable.Defer(() =>
        {
            if (viewKey == null)
            {
                throw new ArgumentNullException(nameof(viewKey));
            }

            cancellationToken.ThrowIfCancellationRequested();
            var descriptor = BidirectionalNavigationResolverHelpers.GetDescriptor(_viewRegistrations, NavigationSourceKind.View, viewKey, contract);
            var view = suppliedView ?? descriptor.CreateView(_serviceProvider);
            cancellationToken.ThrowIfCancellationRequested();
            var viewModel = descriptor.ViewModelImplementation.IsInstanceOfType(view.ViewModel)
                ? (IRxObject)view.ViewModel!
                : descriptor.CreateViewModel(_serviceProvider);
            view.ViewModel = viewModel;
            cancellationToken.ThrowIfCancellationRequested();
            return Observable.Return(new NavigationResolution(viewModel, view, descriptor.Contract, parameter, navigationType));
        });
}