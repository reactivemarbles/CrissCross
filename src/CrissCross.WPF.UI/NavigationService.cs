// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

namespace CrissCross.WPF.UI;

/// <summary>
/// A service that provides methods related to navigation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NavigationService"/> class.
/// </remarks>
/// <param name="serviceProvider">Service provider providing page instances.</param>
public class NavigationService(IServiceProvider serviceProvider) : INavigationService
{
    /// <summary>
    /// Control representing navigation.
    /// </summary>
#pragma warning disable SA1401 // Fields should be private
    protected INavigationView? NavigationControl;

    /// <summary>
    /// Locally attached page service.
    /// </summary>
    private IPageService? _pageService;

    /// <inheritdoc />
    public INavigationView GetNavigationControl() =>
        NavigationControl ?? throw new ArgumentNullException(nameof(NavigationControl));

    /// <inheritdoc />
    public void SetNavigationControl(INavigationView navigation)
    {
        NavigationControl = navigation;

        if (_pageService != null)
        {
            NavigationControl.SetPageService(_pageService);

            return;
        }

        NavigationControl.SetServiceProvider(serviceProvider);
    }

    /// <inheritdoc />
    public void SetPageService(IPageService pageService)
    {
        if (NavigationControl == null)
        {
            _pageService = pageService;

            return;
        }

        ThrowIfPageServiceIsNull();

        NavigationControl.SetPageService(_pageService!);
    }

    /// <inheritdoc />
    public bool Navigate(Type pageType)
    {
        ThrowIfNavigationControlIsNull();

        return NavigationControl!.Navigate(pageType);
    }

    /// <inheritdoc />
    public bool Navigate(Type pageType, object? dataContext)
    {
        ThrowIfNavigationControlIsNull();

        return NavigationControl!.Navigate(pageType, dataContext);
    }

    /// <inheritdoc />
    public bool Navigate(string pageIdOrTargetTag)
    {
        ThrowIfNavigationControlIsNull();

        return NavigationControl!.Navigate(pageIdOrTargetTag);
    }

    /// <inheritdoc />
    public bool Navigate(string pageIdOrTargetTag, object? dataContext)
    {
        ThrowIfNavigationControlIsNull();

        return NavigationControl!.Navigate(pageIdOrTargetTag, dataContext);
    }

    /// <inheritdoc />
    public bool GoBack()
    {
        ThrowIfNavigationControlIsNull();

        return NavigationControl!.GoBack();
    }

    /// <inheritdoc />
    public bool NavigateWithHierarchy(Type pageType)
    {
        ThrowIfNavigationControlIsNull();

        return NavigationControl!.NavigateWithHierarchy(pageType);
    }

    /// <inheritdoc />
    public bool NavigateWithHierarchy(Type pageType, object? dataContext)
    {
        ThrowIfNavigationControlIsNull();

        return NavigationControl!.NavigateWithHierarchy(pageType, dataContext);
    }

    /// <summary>
    /// Throws if navigation control is null.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">NavigationControl.</exception>
    protected void ThrowIfNavigationControlIsNull()
    {
        if (NavigationControl is null)
        {
            throw new ArgumentNullException(nameof(NavigationControl));
        }
    }

    /// <summary>
    /// Throws if page service is null.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">_pageService.</exception>
    protected void ThrowIfPageServiceIsNull()
    {
        if (_pageService is null)
        {
            throw new ArgumentNullException(nameof(_pageService));
        }
    }
}
