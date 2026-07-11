// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using ReactiveUI;
using Splat;

namespace CrissCross;

/// <summary>IViewModel Routed ViewHost.</summary>
public interface IViewModelRoutedViewHost : IActivatableView, IEnableLogger
{
    /// <summary>Gets the navigation stack.</summary>
    /// <value>
    /// The navigation stack.
    /// </value>
    ObservableCollection<Type?> NavigationStack { get; }

    /// <summary>Gets the current view model.</summary>
    /// <value>
    /// The current view model.
    /// </value>
    IObservable<INotifiyRoutableViewModel> CurrentViewModel { get; }

    /// <summary>Gets or sets a value indicating whether [navigate back is enabled].</summary>
    /// <value>
    ///   <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
    /// </value>
    bool? CanNavigateBack { get; set; }

    /// <summary>Gets the can navigate back observable.</summary>
    /// <value>
    /// The can navigate back observable.
    /// </value>
    IObservable<bool?> CanNavigateBackObservable { get; }

    /// <summary>Gets or sets a value indicating whether [navigate back is enabled].</summary>
    /// <value>
    ///   <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
    /// </value>
    bool? NavigateBackIsEnabled { get; set; }

    /// <summary>Gets or sets the name of the host.</summary>
    /// <value>
    /// The name of the host.
    /// </value>
    string Name { get; set; }

    /// <summary>Gets or sets the host name used for navigation.</summary>
    /// <value>
    /// The host name used for navigation.
    /// </value>
    string HostName { get; set; }

    /// <summary>Gets a value indicating whether [requires setup].</summary>
    /// <value>
    ///   <c>true</c> if [requires setup]; otherwise, <c>false</c>.
    /// </value>
    bool RequiresSetup { get; }

    /// <summary>Clears the history.</summary>
    void ClearHistory();

    /// <summary>Setups this instance.</summary>
    void Setup();

    /// <summary>Navigates to a view model type known at compile time.</summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    void Navigate<TViewModel>(TViewModel viewModel, string? contract = null, object? parameter = null)
        where TViewModel : class, IRxObject;

    /// <summary>Navigates the specified contract.</summary>
    /// <param name="viewModel">The view model to navigate to.</param>
    /// <param name="contract">The navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
#if NET8_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Resolving a view from a runtime view model instance requires runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Resolving a view from a runtime view model instance may require members removed by trimming.")]
#endif
    void Navigate(IRxObject viewModel, string? contract = null, object? parameter = null);

    /// <summary>Navigates to a view model type known at compile time and clears history.</summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    void NavigateAndReset<TViewModel>(TViewModel viewModel, string? contract = null, object? parameter = null)
        where TViewModel : class, IRxObject;

    /// <summary>Navigates the and reset.</summary>
    /// <param name="viewModel">The view model to navigate to.</param>
    /// <param name="contract">The navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
#if NET8_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Resolving a view from a runtime view model instance requires runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Resolving a view from a runtime view model instance may require members removed by trimming.")]
#endif
    void NavigateAndReset(IRxObject viewModel, string? contract = null, object? parameter = null);

    /// <summary>Navigates the back.</summary>
    /// <param name="parameter">The navigation parameter.</param>
    /// <returns>The target ViewModel.</returns>
    IRxObject? NavigateBack(object? parameter = null);

    /// <summary>Refreshes this instance.</summary>
    void Refresh();
}
