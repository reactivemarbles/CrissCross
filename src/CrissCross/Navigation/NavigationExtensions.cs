// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Splat;

namespace CrissCross;

/// <summary>Provides strongly typed convenience methods over runtime navigation keys.</summary>
public static class NavigationExtensions
{
    /// <summary>Provides strongly typed navigation methods over runtime navigation keys.</summary>
    /// <param name="navigator">The bidirectional navigator.</param>
    extension(IBidirectionalNavigator navigator)
    {
        /// <summary>Resolves a ViewModel-first request using an interface or base-class key.</summary>
        /// <typeparam name="TViewModelKey">The caller-facing view model lookup key.</typeparam>
        /// <param name="contract">The navigation contract.</param>
        /// <param name="parameter">The navigation parameter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An observable navigation resolution.</returns>
        public IObservable<NavigationResolution> NavigateViewModel<TViewModelKey>(
            string? contract = null,
            object? parameter = null,
            CancellationToken cancellationToken = default)
            where TViewModelKey : class
        {
            ThrowHelper.ThrowIfNull(navigator, nameof(navigator));
            return navigator.NavigateViewModel(typeof(TViewModelKey), contract, parameter, cancellationToken);
        }

        /// <summary>Resolves a View-first request using an interface or base-class key.</summary>
        /// <typeparam name="TViewKey">The caller-facing view lookup key.</typeparam>
        /// <param name="contract">The navigation contract.</param>
        /// <param name="parameter">The navigation parameter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An observable navigation resolution.</returns>
        public IObservable<NavigationResolution> NavigateView<TViewKey>(
            string? contract = null,
            object? parameter = null,
            CancellationToken cancellationToken = default)
            where TViewKey : class
        {
            ThrowHelper.ThrowIfNull(navigator, nameof(navigator));
            return navigator.NavigateView(typeof(TViewKey), contract, parameter, cancellationToken);
        }
    }

    /// <summary>Provides strongly typed navigation methods for routed view hosts.</summary>
    /// <param name="viewHost">The navigation host.</param>
    extension(IViewModelRoutedViewHost viewHost)
    {
        /// <summary>Navigates a host to a resolved view model type.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="contract">The navigation contract.</param>
        /// <param name="parameter">The navigation parameter.</param>
        public void Navigate<TViewModel>(string? contract = null, object? parameter = null)
            where TViewModel : class, IRxObject
        {
            ThrowHelper.ThrowIfNull(viewHost, nameof(viewHost));
            var viewModel = AppLocator.Current.GetService<TViewModel>(contract) ??
                throw new InvalidOperationException($"No view model is registered for '{typeof(TViewModel).FullName}'.");
            viewHost.Navigate(viewModel, contract, parameter);
        }

        /// <summary>Navigates a host to a resolved view model type and clears its history.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="contract">The navigation contract.</param>
        /// <param name="parameter">The navigation parameter.</param>
        public void NavigateAndReset<TViewModel>(string? contract = null, object? parameter = null)
            where TViewModel : class, IRxObject
        {
            ThrowHelper.ThrowIfNull(viewHost, nameof(viewHost));
            var viewModel = AppLocator.Current.GetService<TViewModel>(contract) ??
                throw new InvalidOperationException($"No view model is registered for '{typeof(TViewModel).FullName}'.");
            viewHost.NavigateAndReset(viewModel, contract, parameter);
        }
    }
}
