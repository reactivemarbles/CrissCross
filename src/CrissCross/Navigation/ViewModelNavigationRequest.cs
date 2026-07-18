// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Identifies a typed ViewModel-first navigation pair and request options.</summary>
/// <typeparam name="TViewModel">The view model key and result type.</typeparam>
/// <typeparam name="TView">The resolved view type.</typeparam>
public sealed class ViewModelNavigationRequest<TViewModel, TView>
    where TViewModel : class, IRxObject
    where TView : class, IViewFor<TViewModel>
{
    /// <summary>Gets the runtime resolved view type.</summary>
    public System.Type ViewType => typeof(TView);

    /// <summary>Gets or sets an optional supplied view model; the registered factory is used when absent.</summary>
    public TViewModel? ViewModel { get; set; }

    /// <summary>Gets or sets the navigation request options.</summary>
    public NavigationRequestOptions Options { get; set; } = new();
}
