// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Service that provides pages for navigation.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PageService" /> class.
/// Creates new instance and attaches the <see cref="IServiceProvider" />.
/// </remarks>
/// <param name="serviceProvider">The service provider.</param>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class PageService(IServiceProvider serviceProvider) : IPageService
{
    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <inheritdoc />
    public T? GetPage<T>(PageNavigationRequest<T> request)
        where T : class => (T?)serviceProvider.GetService(CheckIsFrameworkElement(request.PageType));

    /// <inheritdoc />
    public FrameworkElement? GetPage(Type pageType) =>
        serviceProvider.GetService(CheckIsFrameworkElement(pageType)) as FrameworkElement;

    /// <summary>Provides the CheckIsFrameworkElement member.</summary>
    /// <param name="pageType">The pageType value.</param>
    /// <returns>The result.</returns>
    private static Type CheckIsFrameworkElement(Type pageType)
    {
        if (!typeof(FrameworkElement).IsAssignableFrom(pageType))
        {
            throw new InvalidOperationException("The page should be a WPF control.");
        }

        return pageType;
    }
}
