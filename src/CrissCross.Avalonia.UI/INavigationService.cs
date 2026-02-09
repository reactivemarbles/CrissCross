// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI;

/// <summary>
/// Defines a contract for navigation services that manage navigation between pages or views within an application.
/// Provides methods to navigate to pages by type or tag, manage the navigation stack, and interact with the navigation
/// control and page service.
/// </summary>
/// <remarks>Implementations of this interface enable decoupled navigation logic, allowing view models or other
/// components to initiate navigation without direct references to UI elements. The interface supports navigation with
/// or without associated data contexts and provides methods for hierarchical navigation scenarios. It is typically used
/// in conjunction with an IPageService to resolve page instances and an INavigationView control to present navigation
/// UI.</remarks>
public interface INavigationService
{
    /// <summary>
    /// Lets you navigate to the selected page based on it's type. Should be used with <see cref="IPageService"/>.
    /// </summary>
    /// <param name="pageType"><see langword="Type"/> of the page.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool Navigate(Type pageType);

    /// <summary>
    /// Lets you navigate to the selected page based on it's type, Should be used with <see cref="IPageService"/>.
    /// </summary>
    /// <param name="pageType"><see langword="Type"/> of the page.</param>
    /// <param name="dataContext">DataContext <see cref="object"/>.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool Navigate(Type pageType, object? dataContext);

    /// <summary>
    /// Lets you navigate to the selected page based on it's tag. Should be used with <see cref="IPageService"/>.
    /// </summary>
    /// <param name="pageIdOrTargetTag">Id or tag of the page.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool Navigate(string pageIdOrTargetTag);

    /// <summary>
    /// Lets you navigate to the selected page based on it's tag. Should be used with <see cref="IPageService"/>.
    /// </summary>
    /// <param name="pageIdOrTargetTag">Id or tag of the page.</param>
    /// <param name="dataContext">DataContext <see cref="object"/>.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool Navigate(string pageIdOrTargetTag, object? dataContext);

    /// <summary>
    /// Synchronously adds an element to the navigation stack and navigates current navigation Frame to the page represented by the element.
    /// </summary>
    /// <param name="pageType">Type of control to be synchronously added to the navigation stack.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool NavigateWithHierarchy(Type pageType);

    /// <summary>
    /// Synchronously adds an element to the navigation stack and navigates current navigation Frame to the page represented by the element.
    /// </summary>
    /// <param name="pageType">Type of control to be synchronously added to the navigation stack.</param>
    /// <param name="dataContext">DataContext <see cref="object"/>.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool NavigateWithHierarchy(Type pageType, object? dataContext);

    /// <summary>
    /// Provides direct access to the control responsible for navigation.
    /// </summary>
    /// <returns>Instance of the <see cref="Controls.INavigationView"/> control.</returns>
    Controls.INavigationView GetNavigationControl();

    /// <summary>
    /// Lets you attach the control that represents the <see cref="Controls.INavigationView"/>.
    /// </summary>
    /// <param name="navigation">Instance of the <see cref="Controls.INavigationView"/>.</param>
    void SetNavigationControl(Controls.INavigationView navigation);

    /// <summary>
    /// Lets you attach the service that delivers page instances to <see cref="Controls.INavigationView"/>.
    /// </summary>
    /// <param name="pageService">Instance of the <see cref="IPageService"/>.</param>
    void SetPageService(IPageService pageService);

    /// <summary>
    /// Navigates the NavigationView to the previous journal entry.
    /// </summary>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool GoBack();
}
