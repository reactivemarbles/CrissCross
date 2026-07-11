// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI;

namespace CrissCross;

/// <summary>Registers bidirectional ViewModel/View navigation pairs.</summary>
public interface INavigationRegistry
{
    /// <summary>Registers a concrete ViewModel/View pair for bidirectional navigation.</summary>
    /// <typeparam name="TViewModel">The concrete view model key and implementation.</typeparam>
    /// <typeparam name="TView">The concrete view key and implementation.</typeparam>
    /// <param name="createViewModel">The view model factory.</param>
    /// <param name="createView">The view factory.</param>
    /// <param name="contract">The optional navigation contract.</param>
    /// <returns>The registry for chained registrations.</returns>
    INavigationRegistry Register<TViewModel, TView>(
        Func<IServiceProvider, TViewModel> createViewModel,
        Func<IServiceProvider, TView> createView,
        string? contract = null)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel>;

    /// <summary>Registers a ViewModel/View pair with explicit interface or base-class lookup keys.</summary>
    /// <typeparam name="TViewModelKey">The caller-facing view model lookup key.</typeparam>
    /// <typeparam name="TViewModel">The concrete view model implementation type.</typeparam>
    /// <typeparam name="TViewKey">The caller-facing view lookup key.</typeparam>
    /// <typeparam name="TView">The concrete view implementation type.</typeparam>
    /// <param name="createViewModel">The view model factory.</param>
    /// <param name="createView">The view factory.</param>
    /// <param name="contract">The optional navigation contract.</param>
    /// <returns>The registry for chained registrations.</returns>
    INavigationRegistry Register<TViewModelKey, TViewModel, TViewKey, TView>(
        Func<IServiceProvider, TViewModel> createViewModel,
        Func<IServiceProvider, TView> createView,
        string? contract = null)
        where TViewModelKey : class
        where TViewModel : class, TViewModelKey, IRxObject
        where TViewKey : class
        where TView : class, TViewKey, IViewFor<TViewModel>;

    /// <summary>Creates a navigator over the current registrations.</summary>
    /// <param name="serviceProvider">The optional service provider for navigation factories.</param>
    /// <returns>A bidirectional navigator.</returns>
    IBidirectionalNavigator CreateNavigator(IServiceProvider? serviceProvider = null);
}
