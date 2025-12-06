// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.Avalonia.UI;

/// <summary>
/// Represents a contract with a <see cref="Avalonia.Controls.Window"/> that contains <see cref="INavigationView"/>.
/// Through defined <see cref="IPageService"/> service allows you to use the MVVM model in navigation.
/// </summary>
public interface INavigationWindow
{
    /// <summary>
    /// Provides direct access to the control responsible for navigation.
    /// </summary>
    /// <returns>Instance of the <see cref="INavigationView"/> control.</returns>
    INavigationView GetNavigation();

    /// <summary>
    /// Lets you navigate to the selected page based on it's type. Should be used with <see cref="IPageService"/>.
    /// </summary>
    /// <param name="pageType"><see langword="Type"/> of the page.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool Navigate(Type pageType);

    /// <summary>
    /// Lets you attach the service provider that delivers page instances to <see cref="INavigationView"/>.
    /// </summary>
    /// <param name="serviceProvider">Instance of the <see cref="IServiceProvider"/>.</param>
    void SetServiceProvider(IServiceProvider serviceProvider);

    /// <summary>
    /// Lets you attach the service that delivers page instances to <see cref="INavigationView"/>.
    /// </summary>
    /// <param name="pageService">Instance of the <see cref="IPageService"/> with attached service provider.</param>
    void SetPageService(IPageService pageService);

    /// <summary>
    /// Triggers the command to open a window.
    /// </summary>
    void ShowWindow();

    /// <summary>
    /// Triggers the command to close a window.
    /// </summary>
    void CloseWindow();
}
