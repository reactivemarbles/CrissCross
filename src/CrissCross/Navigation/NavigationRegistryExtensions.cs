// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI;

namespace CrissCross;

/// <summary>Provides convenience overloads for navigation registration.</summary>
public static class NavigationRegistryExtensions
{
    /// <summary>Provides convenience overloads for a navigation registry.</summary>
    /// <param name="registry">The navigation registry.</param>
    extension(INavigationRegistry registry)
    {
        /// <summary>Registers a concrete ViewModel/View pair without a contract.</summary>
        /// <typeparam name="TViewModel">The concrete view model type.</typeparam>
        /// <typeparam name="TView">The concrete view type.</typeparam>
        /// <param name="createViewModel">The view model factory.</param>
        /// <param name="createView">The view factory.</param>
        /// <returns>The registry for chained registrations.</returns>
        public INavigationRegistry Register<TViewModel, TView>(
            Func<IServiceProvider, TViewModel> createViewModel,
            Func<IServiceProvider, TView> createView)
            where TViewModel : class, IRxObject
            where TView : class, IViewFor<TViewModel> =>
            registry.Register(createViewModel, createView, null);

        /// <summary>Creates a navigator using the registry's default service provider.</summary>
        /// <returns>A bidirectional navigator.</returns>
        public IBidirectionalNavigator CreateNavigator() => registry.CreateNavigator(null);
    }
}
