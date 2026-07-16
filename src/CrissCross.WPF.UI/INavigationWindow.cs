// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>Represents a contract with a Window that contains INavigationView.</summary>
public interface INavigationWindow
{
    /// <summary>Provides direct access to the control responsible for navigation.</summary>
    /// <returns>Instance of the <see cref="INavigationView"/> control.</returns>
    INavigationView GetNavigation();

    /// <summary>Lets you navigate to the selected page based on it's type. Should be used with IPageService.</summary>
    /// <param name="pageType">The pageType value.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool Navigate(Type pageType);

    /// <summary>Lets you attach the service provider that delivers page instances to INavigationView.</summary>
    /// <param name="serviceProvider">The serviceProvider value.</param>
    void SetServiceProvider(IServiceProvider serviceProvider);

    /// <summary>Lets you attach the service that delivers page instances to <see cref="INavigationView"/>.</summary>
    /// <param name="pageService">The pageService value.</param>
    void SetPageService(IPageService pageService);

    /// <summary>Triggers the command to open a window.</summary>
    void ShowWindow();

    /// <summary>Triggers the command to close a window.</summary>
    void CloseWindow();
}
