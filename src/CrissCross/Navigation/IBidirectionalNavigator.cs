// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Threading;
using ReactiveUI;

namespace CrissCross;

/// <summary>Resolves bidirectional ViewModel/View navigation requests without mutating a platform host.</summary>
public interface IBidirectionalNavigator
{
    /// <summary>Resolves a ViewModel-first navigation request using a supplied view model instance.</summary>
    /// <typeparam name="TViewModel">The view model key and result type.</typeparam>
    /// <typeparam name="TView">The resolved view type.</typeparam>
    /// <param name="viewModel">The view model instance.</param>
    /// <param name="contract">The navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An observable typed navigation resolution.</returns>
    IObservable<NavigationResolution<TViewModel, TView>> NavigateViewModel<TViewModel, TView>(
        TViewModel viewModel,
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel>;

    /// <summary>Resolves a ViewModel-first navigation request by creating the registered view model.</summary>
    /// <typeparam name="TViewModel">The view model key and result type.</typeparam>
    /// <typeparam name="TView">The resolved view type.</typeparam>
    /// <param name="contract">The navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An observable typed navigation resolution.</returns>
    IObservable<NavigationResolution<TViewModel, TView>> NavigateViewModel<TViewModel, TView>(
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel>;

    /// <summary>Resolves a ViewModel-first navigation request using an interface or base-class key.</summary>
    /// <typeparam name="TViewModelKey">The caller-facing view model lookup key.</typeparam>
    /// <param name="contract">The navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An observable navigation resolution.</returns>
    IObservable<NavigationResolution> NavigateViewModel<TViewModelKey>(
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default)
        where TViewModelKey : class;

    /// <summary>Resolves a ViewModel-first navigation request using a runtime key.</summary>
    /// <param name="viewModelKey">The view model lookup key.</param>
    /// <param name="contract">The navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An observable navigation resolution.</returns>
    IObservable<NavigationResolution> NavigateViewModel(
        Type viewModelKey,
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default);

    /// <summary>Resolves a View-first navigation request using a supplied view instance.</summary>
    /// <typeparam name="TViewModel">The view model result type.</typeparam>
    /// <typeparam name="TView">The view key and result type.</typeparam>
    /// <param name="view">The view instance.</param>
    /// <param name="contract">The navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An observable typed navigation resolution.</returns>
    IObservable<NavigationResolution<TViewModel, TView>> NavigateView<TViewModel, TView>(
        TView view,
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel>;

    /// <summary>Resolves a View-first navigation request by creating the registered view.</summary>
    /// <typeparam name="TViewModel">The view model result type.</typeparam>
    /// <typeparam name="TView">The view key and result type.</typeparam>
    /// <param name="contract">The navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An observable typed navigation resolution.</returns>
    IObservable<NavigationResolution<TViewModel, TView>> NavigateView<TViewModel, TView>(
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel>;

    /// <summary>Resolves a View-first navigation request using an interface or base-class key.</summary>
    /// <typeparam name="TViewKey">The caller-facing view lookup key.</typeparam>
    /// <param name="contract">The navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An observable navigation resolution.</returns>
    IObservable<NavigationResolution> NavigateView<TViewKey>(
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default)
        where TViewKey : class;

    /// <summary>Resolves a View-first navigation request using a runtime key.</summary>
    /// <param name="viewKey">The view lookup key.</param>
    /// <param name="contract">The navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An observable navigation resolution.</returns>
    IObservable<NavigationResolution> NavigateView(
        Type viewKey,
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default);
}
