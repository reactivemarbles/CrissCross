// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.Avalonia.UI;

/// <summary>
/// Defines a contract for a window that supports navigation between pages using a navigation control and associated
/// services.
/// </summary>
/// <remarks>Implementations of this interface provide mechanisms to navigate between pages, manage navigation
/// services, and control the window's visibility. The interface is typically used in applications that require dynamic
/// page navigation, such as multi-page desktop or UI applications.</remarks>
public interface INavigationWindow
{
    /// <summary>Provides direct access to the control responsible for navigation.</summary>
    /// <returns>Instance of the <see cref="INavigationView"/> control.</returns>
    INavigationView GetNavigation();

    /// <summary>Lets you navigate to the selected page based on it's type. Should be used with <see cref="IPageService"/>.</summary>
    /// <param name="pageType">The page type.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool Navigate(Type pageType);

    /// <summary>Lets you attach the service provider that delivers page instances to <see cref="INavigationView"/>.</summary>
    /// <param name="serviceProvider">The service provider.</param>
    void SetServiceProvider(IServiceProvider serviceProvider);

    /// <summary>Lets you attach the service that delivers page instances to <see cref="INavigationView"/>.</summary>
    /// <param name="pageService">The page service.</param>
    void SetPageService(IPageService pageService);

    /// <summary>Triggers the command to open a window.</summary>
    void ShowWindow();

    /// <summary>Triggers the command to close a window.</summary>
    void CloseWindow();
}
