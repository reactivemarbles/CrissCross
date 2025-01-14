// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>
/// Represents a contract with the service that provides the pages for <see cref="INavigationView"/>.
/// </summary>
public interface IPageService
{
    /// <summary>
    /// Takes a page of the given type.
    /// </summary>
    /// <typeparam name="T">Page type.</typeparam>
    /// <returns>Instance of the registered page service.</returns>
    public T? GetPage<T>()
        where T : class;

    /// <summary>
    /// Takes a page of the given type.
    /// </summary>
    /// <param name="pageType">Page type.</param>
    /// <returns>Instance of the registered page service.</returns>
    public FrameworkElement? GetPage(Type pageType);
}
