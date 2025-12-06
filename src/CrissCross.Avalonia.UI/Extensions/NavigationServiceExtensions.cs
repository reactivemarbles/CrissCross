// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Extensions;

/// <summary>
/// Extensions for <see cref="INavigationService"/>.
/// </summary>
public static class NavigationServiceExtensions
{
    /// <summary>
    /// Lets you navigate to the selected page based on it's type.
    /// </summary>
    /// <typeparam name="T">Type of the page.</typeparam>
    /// <param name="navigationService">The navigation service.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    public static bool Navigate<T>(this INavigationService navigationService)
    {
        ArgumentNullException.ThrowIfNull(navigationService);
        return navigationService.Navigate(typeof(T));
    }

    /// <summary>
    /// Lets you navigate to the selected page based on it's type.
    /// </summary>
    /// <typeparam name="T">Type of the page.</typeparam>
    /// <param name="navigationService">The navigation service.</param>
    /// <param name="dataContext">DataContext <see cref="object"/>.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    public static bool Navigate<T>(this INavigationService navigationService, object? dataContext)
    {
        ArgumentNullException.ThrowIfNull(navigationService);
        return navigationService.Navigate(typeof(T), dataContext);
    }

    /// <summary>
    /// Synchronously adds an element to the navigation stack and navigates current navigation Frame to the page represented by the element.
    /// </summary>
    /// <typeparam name="T">Type of control to be synchronously added to the navigation stack.</typeparam>
    /// <param name="navigationService">The navigation service.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    public static bool NavigateWithHierarchy<T>(this INavigationService navigationService)
    {
        ArgumentNullException.ThrowIfNull(navigationService);
        return navigationService.NavigateWithHierarchy(typeof(T));
    }

    /// <summary>
    /// Synchronously adds an element to the navigation stack and navigates current navigation Frame to the page represented by the element.
    /// </summary>
    /// <typeparam name="T">Type of control to be synchronously added to the navigation stack.</typeparam>
    /// <param name="navigationService">The navigation service.</param>
    /// <param name="dataContext">DataContext <see cref="object"/>.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    public static bool NavigateWithHierarchy<T>(this INavigationService navigationService, object? dataContext)
    {
        ArgumentNullException.ThrowIfNull(navigationService);
        return navigationService.NavigateWithHierarchy(typeof(T), dataContext);
    }
}
