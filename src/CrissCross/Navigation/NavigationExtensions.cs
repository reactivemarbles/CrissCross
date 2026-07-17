// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Splat;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Provides strongly typed convenience methods over runtime navigation keys.</summary>
public static class NavigationExtensions
{
    /// <summary>Provides strongly typed navigation methods over runtime navigation keys.</summary>
    /// <param name="navigator">The bidirectional navigator.</param>
    extension(IBidirectionalNavigator navigator)
    {
        /// <summary>Resolves a ViewModel-first request using an interface or base-class key.</summary>
        /// <typeparam name="TViewModelKey">The caller-facing view model lookup key.</typeparam>
        /// <param name="request">The typed navigation key request.</param>
        /// <returns>An observable navigation resolution.</returns>
        public IObservable<NavigationResolution> NavigateViewModel<TViewModelKey>(
            NavigationKeyRequest<TViewModelKey> request)
            where TViewModelKey : class
        {
            ThrowHelper.ThrowIfNull(navigator, nameof(navigator));
            ThrowHelper.ThrowIfNull(request, nameof(request));
            return navigator.NavigateViewModel(typeof(TViewModelKey), request.Options);
        }

        /// <summary>Resolves a View-first request using an interface or base-class key.</summary>
        /// <typeparam name="TViewKey">The caller-facing view lookup key.</typeparam>
        /// <param name="request">The typed navigation key request.</param>
        /// <returns>An observable navigation resolution.</returns>
        public IObservable<NavigationResolution> NavigateView<TViewKey>(NavigationKeyRequest<TViewKey> request)
            where TViewKey : class
        {
            ThrowHelper.ThrowIfNull(navigator, nameof(navigator));
            ThrowHelper.ThrowIfNull(request, nameof(request));
            return navigator.NavigateView(typeof(TViewKey), request.Options);
        }
    }

    /// <summary>Provides strongly typed navigation methods for routed view hosts.</summary>
    /// <param name="viewHost">The navigation host.</param>
    extension(IViewModelRoutedViewHost viewHost)
    {
        /// <summary>Navigates a host to the supplied view model.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="viewModel">The view model.</param>
        public void Navigate<TViewModel>(TViewModel viewModel)
            where TViewModel : class, IRxObject => viewHost.Navigate(viewModel, null, null);

        /// <summary>Navigates a host to the supplied view model and contract.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="viewModel">The view model.</param>
        /// <param name="contract">The navigation contract.</param>
        public void Navigate<TViewModel>(TViewModel viewModel, string? contract)
            where TViewModel : class, IRxObject => viewHost.Navigate(viewModel, contract, null);

        /// <summary>Navigates a host to the supplied view model and clears history.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="viewModel">The view model.</param>
        public void NavigateAndReset<TViewModel>(TViewModel viewModel)
            where TViewModel : class, IRxObject => viewHost.NavigateAndReset(viewModel, null, null);

        /// <summary>Navigates a host to the supplied view model and contract, then clears history.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="viewModel">The view model.</param>
        /// <param name="contract">The navigation contract.</param>
        public void NavigateAndReset<TViewModel>(TViewModel viewModel, string? contract)
            where TViewModel : class, IRxObject => viewHost.NavigateAndReset(viewModel, contract, null);

        /// <summary>Navigates back without a parameter.</summary>
        /// <returns>The target view model.</returns>
        public IRxObject? NavigateBack() => viewHost.NavigateBack(null);

        /// <summary>Navigates a host to a resolved view model type.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="request">The typed navigation key request.</param>
        public void Navigate<TViewModel>(NavigationKeyRequest<TViewModel> request)
            where TViewModel : class, IRxObject
        {
            ThrowHelper.ThrowIfNull(viewHost, nameof(viewHost));
            ThrowHelper.ThrowIfNull(request, nameof(request));
            var options = request.Options;
            var viewModel =
                AppLocator.Current.GetService<TViewModel>(options.Contract)
                ?? throw new InvalidOperationException(
                    $"No view model is registered for '{typeof(TViewModel).FullName}'.");
            viewHost.Navigate(viewModel, options.Contract, options.Parameter);
        }

        /// <summary>Navigates a host to a resolved view model type and clears its history.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="request">The typed navigation key request.</param>
        public void NavigateAndReset<TViewModel>(NavigationKeyRequest<TViewModel> request)
            where TViewModel : class, IRxObject
        {
            ThrowHelper.ThrowIfNull(viewHost, nameof(viewHost));
            ThrowHelper.ThrowIfNull(request, nameof(request));
            var options = request.Options;
            var viewModel =
                AppLocator.Current.GetService<TViewModel>(options.Contract)
                ?? throw new InvalidOperationException(
                    $"No view model is registered for '{typeof(TViewModel).FullName}'.");
            viewHost.NavigateAndReset(viewModel, options.Contract, options.Parameter);
        }
    }
}
