// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Resolves bidirectional ViewModel/View navigation requests without mutating a platform host.</summary>
public interface IBidirectionalNavigator
{
    /// <summary>Resolves a typed ViewModel-first navigation request.</summary>
    /// <typeparam name="TViewModel">The view model key and result type.</typeparam>
    /// <typeparam name="TView">The resolved view type.</typeparam>
    /// <param name="request">The typed navigation request.</param>
    /// <returns>An observable typed navigation resolution.</returns>
    IObservable<NavigationResolution<TViewModel, TView>> NavigateViewModel<TViewModel, TView>(
        ViewModelNavigationRequest<TViewModel, TView> request)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel>;

    /// <summary>Resolves a ViewModel-first navigation request using a runtime key.</summary>
    /// <param name="viewModelKey">The view model lookup key.</param>
    /// <param name="options">The navigation request options.</param>
    /// <returns>An observable navigation resolution.</returns>
    IObservable<NavigationResolution> NavigateViewModel(
        Type viewModelKey,
        NavigationRequestOptions options);

    /// <summary>Resolves a typed View-first navigation request.</summary>
    /// <typeparam name="TViewModel">The view model result type.</typeparam>
    /// <typeparam name="TView">The view key and result type.</typeparam>
    /// <param name="request">The typed navigation request.</param>
    /// <returns>An observable typed navigation resolution.</returns>
    IObservable<NavigationResolution<TViewModel, TView>> NavigateView<TViewModel, TView>(
        ViewNavigationRequest<TViewModel, TView> request)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel>;

    /// <summary>Resolves a View-first navigation request using a runtime key.</summary>
    /// <param name="viewKey">The view lookup key.</param>
    /// <param name="options">The navigation request options.</param>
    /// <returns>An observable navigation resolution.</returns>
    IObservable<NavigationResolution> NavigateView(
        Type viewKey,
        NavigationRequestOptions options);
}
