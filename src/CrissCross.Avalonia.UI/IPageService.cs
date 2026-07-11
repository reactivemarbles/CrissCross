// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;

namespace CrissCross.Avalonia.UI;

/// <summary>Represents a contract with the service that provides the pages for <see cref="Controls.INavigationView"/>.</summary>
public interface IPageService
{
    /// <summary>Takes a page of the given type.</summary>
    /// <typeparam name="T">Page type.</typeparam>
    /// <returns>Instance of the registered page service.</returns>
    T? GetPage<T>()
        where T : class;

    /// <summary>Takes a page of the given type.</summary>
    /// <param name="pageType">The page type.</param>
    /// <returns>Instance of the registered page service.</returns>
    Control? GetPage(Type pageType);
}
