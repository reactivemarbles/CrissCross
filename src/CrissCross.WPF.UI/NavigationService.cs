// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
    private INavigationView? _navigationControl;

    /// <summary>
    /// Locally attached page service.
    /// </summary>
    private IPageService? _pageService;

    /// <inheritdoc />
    public INavigationView GetNavigationControl() =>
        _navigationControl ?? throw new ArgumentNullException(nameof(_navigationControl));

    /// <inheritdoc />
    public void SetNavigationControl(INavigationView navigation)
    {
        _navigationControl = navigation;

        if (_pageService != null)
        {
            _navigationControl.SetPageService(_pageService);

            return;
        }

        _navigationControl.SetServiceProvider(serviceProvider);
    }

    /// <inheritdoc />
    public void SetPageService(IPageService pageService)
    {
        if (_navigationControl == null)
        {
            _pageService = pageService;

            return;
        }

        ThrowIfPageServiceIsNull();

        _navigationControl.SetPageService(_pageService!);
    }

    /// <inheritdoc />
    public bool Navigate(Type pageType)
    {
        ThrowIfNavigationControlIsNull();

        return _navigationControl!.Navigate(pageType);
    }

    /// <inheritdoc />
    public bool Navigate(Type pageType, object? dataContext)
    {
        ThrowIfNavigationControlIsNull();

        return _navigationControl!.Navigate(pageType, dataContext);
    }

    /// <inheritdoc />
    public bool Navigate(string pageIdOrTargetTag)
    {
        ThrowIfNavigationControlIsNull();

        return _navigationControl!.Navigate(pageIdOrTargetTag);
    }

    /// <inheritdoc />
    public bool Navigate(string pageIdOrTargetTag, object? dataContext)
    {
        ThrowIfNavigationControlIsNull();

        return _navigationControl!.Navigate(pageIdOrTargetTag, dataContext);
    }

    /// <inheritdoc />
    public bool GoBack()
    {
        ThrowIfNavigationControlIsNull();

        return _navigationControl!.GoBack();
    }

    /// <inheritdoc />
    public bool GoForward()
    {
        ThrowIfNavigationControlIsNull();

        return _navigationControl!.GoForward();
    }

    /// <inheritdoc />
    public bool NavigateWithHierarchy(Type pageType)
    {
        ThrowIfNavigationControlIsNull();

        return _navigationControl!.NavigateWithHierarchy(pageType);
    }

    /// <inheritdoc />
    public bool NavigateWithHierarchy(Type pageType, object? dataContext)
    {
        ThrowIfNavigationControlIsNull();

        return _navigationControl!.NavigateWithHierarchy(pageType, dataContext);
    }

    /// <summary>
    /// Throws if navigation control is null.
    /// </summary>
    /// <exception cref="ArgumentNullException">Navigation control.</exception>
    protected void ThrowIfNavigationControlIsNull()
    {
        if (_navigationControl is null)
        {
            throw new ArgumentNullException(nameof(_navigationControl));
        }
    }

    /// <summary>
    /// Throws if page service is null.
    /// </summary>
    /// <exception cref="ArgumentNullException">_pageService.</exception>
    protected void ThrowIfPageServiceIsNull()
    {
        if (_pageService is null)
        {
            throw new ArgumentNullException(nameof(_pageService));
        }
    }
}
