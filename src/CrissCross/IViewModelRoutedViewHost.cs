// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using ReactiveUI;
using Splat;

namespace CrissCross
{
    /// <summary>
    /// IViewModel Routed ViewHost.
    /// </summary>
    public interface IViewModelRoutedViewHost : IActivatableView, IEnableLogger
    {
        /// <summary>
        /// Gets the navigation stack.
        /// </summary>
        /// <value>
        /// The navigation stack.
        /// </value>
        ObservableCollection<Type?> NavigationStack { get; }

        /// <summary>
        /// Gets the current view model.
        /// </summary>
        /// <value>
        /// The current view model.
        /// </value>
        IObservable<INotifiyRoutableViewModel> CurrentViewModel { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [navigate back is enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
        /// </value>
        bool CanNavigateBack { get; set; }

        /// <summary>
        /// Gets the can navigate back observable.
        /// </summary>
        /// <value>
        /// The can navigate back observable.
        /// </value>
        IObservable<bool> CanNavigateBackObservable { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [navigate back is enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
        /// </value>
        bool NavigateBackIsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the name of the host.
        /// </summary>
        /// <value>
        /// The name of the host.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether [requires setup].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [requires setup]; otherwise, <c>false</c>.
        /// </value>
        bool RequiresSetup { get; }

        /// <summary>
        /// Clears the history.
        /// </summary>
        void ClearHistory();

        /// <summary>
        /// Setups this instance.
        /// </summary>
        void Setup();

        /// <summary>
        /// Navigates the specified contract.
        /// </summary>
        /// <typeparam name="T">The Type.</typeparam>
        /// <param name="contract">The contract.</param>
        /// <param name="parameter">The parameter.</param>
        void Navigate<T>(string? contract = null, object? parameter = null)
            where T : class, IRxObject;

        /// <summary>
        /// Navigates the and reset.
        /// </summary>
        /// <typeparam name="T">The Type.</typeparam>
        /// <param name="contract">The contract.</param>
        /// <param name="parameter">The parameter.</param>
        void NavigateAndReset<T>(string? contract = null, object? parameter = null)
            where T : class, IRxObject;

        /// <summary>
        /// Navigates the back.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        void NavigateBack(object? parameter = null);

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        void Refresh();
    }
}