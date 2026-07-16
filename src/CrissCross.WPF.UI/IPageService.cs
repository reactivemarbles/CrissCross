// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>Represents a contract with the service that provides the pages for <see cref="INavigationView"/>.</summary>
public interface IPageService
{
    /// <summary>Takes a page of the given type.</summary>
    /// <typeparam name="T">Page type.</typeparam>
    /// <param name="request">The typed page request.</param>
    /// <returns>Instance of the registered page service.</returns>
    T? GetPage<T>(PageNavigationRequest<T> request)
        where T : class;

    /// <summary>Takes a page of the given type.</summary>
    /// <param name="pageType">Page type.</param>
    /// <returns>Instance of the registered page service.</returns>
    FrameworkElement? GetPage(Type pageType);
}
