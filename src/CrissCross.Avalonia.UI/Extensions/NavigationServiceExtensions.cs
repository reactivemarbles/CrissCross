// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Extensions;

/// <summary>Extensions for <see cref="INavigationService"/>.</summary>
public static class NavigationServiceExtensions
{
    /// <summary>Provides extension members for <see cref="INavigationService"/>.</summary>
    /// <param name="navigationService">The navigation service.</param>
    extension(INavigationService navigationService)
    {
        /// <summary>Lets you navigate to the selected page based on it's type.</summary>
        /// <typeparam name="T">Type of the page.</typeparam>
        /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
        public bool Navigate<T>()
        {
            ArgumentNullException.ThrowIfNull(navigationService);
            return navigationService.Navigate(typeof(T));
        }

        /// <summary>Lets you navigate to the selected page based on it's type.</summary>
        /// <typeparam name="T">Type of the page.</typeparam>
        /// <param name="dataContext">DataContext <see cref="object"/>.</param>
        /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
        public bool Navigate<T>(object? dataContext)
        {
            ArgumentNullException.ThrowIfNull(navigationService);
            return navigationService.Navigate(typeof(T), dataContext);
        }

        /// <summary>Synchronously adds an element to the navigation stack and navigates current navigation Frame to the page represented by the element.</summary>
        /// <typeparam name="T">Type of control to be synchronously added to the navigation stack.</typeparam>
        /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
        public bool NavigateWithHierarchy<T>()
        {
            ArgumentNullException.ThrowIfNull(navigationService);
            return navigationService.NavigateWithHierarchy(typeof(T));
        }

        /// <summary>Synchronously adds an element to the navigation stack and navigates current navigation Frame to the page represented by the element.</summary>
        /// <typeparam name="T">Type of control to be synchronously added to the navigation stack.</typeparam>
        /// <param name="dataContext">DataContext <see cref="object"/>.</param>
        /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
        public bool NavigateWithHierarchy<T>(object? dataContext)
        {
            ArgumentNullException.ThrowIfNull(navigationService);
            return navigationService.NavigateWithHierarchy(typeof(T), dataContext);
        }
    }
}
