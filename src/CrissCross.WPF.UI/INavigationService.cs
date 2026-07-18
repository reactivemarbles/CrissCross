// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>
/// Represents a contract with a <see cref="FrameworkElement"/> that contains <see cref="INavigationView"/>.
/// Through defined <see cref="IPageService"/> service allows you to use the Dependency Injection pattern in navigation.
/// </summary>
public interface INavigationService
{
    /// <summary>Lets you navigate to the selected page based on it's type. Should be used with IPageService.</summary>
    /// <param name="pageType">The pageType value.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool Navigate(Type pageType);

    /// <summary>Lets you navigate to the selected page based on it's type, Should be used with IPageService.</summary>
    /// <param name="pageType">The pageType value.</param>
    /// <param name="dataContext">The dataContext value.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool Navigate(Type pageType, object? dataContext);

    /// <summary>Lets you navigate to the selected page based on it's tag. Should be used with IPageService.</summary>
    /// <param name="pageIdOrTargetTag">The pageIdOrTargetTag value.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool Navigate(string pageIdOrTargetTag);

    /// <summary>Lets you navigate to the selected page based on it's tag. Should be used with IPageService.</summary>
    /// <param name="pageIdOrTargetTag">The pageIdOrTargetTag value.</param>
    /// <param name="dataContext">The dataContext value.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool Navigate(string pageIdOrTargetTag, object? dataContext);

    /// <summary>Synchronously adds an element to the navigation stack and navigates current navigation Frame to the
    /// page represented by the element.</summary>
    /// <param name="pageType">The pageType value.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool NavigateWithHierarchy(Type pageType);

    /// <summary>Synchronously adds an element to the navigation stack and navigates current navigation Frame to the
    /// page represented by the element.</summary>
    /// <param name="pageType">The pageType value.</param>
    /// <param name="dataContext">The dataContext value.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool NavigateWithHierarchy(Type pageType, object? dataContext);

    /// <summary>Provides direct access to the control responsible for navigation.</summary>
    /// <returns>Instance of the <see cref="INavigationView"/> control.</returns>
    INavigationView GetNavigationControl();

    /// <summary>Lets you attach the control that represents the <see cref="INavigationView"/>.</summary>
    /// <param name="navigation">The navigation value.</param>
    void SetNavigationControl(INavigationView navigation);

    /// <summary>Lets you attach the service that delivers page instances to <see cref="INavigationView"/>.</summary>
    /// <param name="pageService">The pageService value.</param>
    void SetPageService(IPageService pageService);

    /// <summary>Navigates the NavigationView to the previous journal entry.</summary>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool GoBack();

    /// <summary>Navigates the NavigationView to the next journal entry.</summary>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool GoForward();
}
