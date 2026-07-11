// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;

namespace CrissCross.Avalonia.UI;

/// <summary>Service that provides pages for navigation.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PageService" /> class.
/// Creates new instance and attaches the <see cref="IServiceProvider" />.
/// </remarks>
/// <param name="serviceProvider">The service provider.</param>
internal sealed class PageService(IServiceProvider serviceProvider) : IPageService
{
    /// <inheritdoc />
    public T? GetPage<T>()
        where T : class => (T?)serviceProvider.GetService(CheckIsControl(typeof(T)));

    /// <inheritdoc />
    public Control? GetPage(Type pageType) =>
        serviceProvider.GetService(CheckIsControl(pageType)) as Control;

    /// <summary>Provides the CheckIsControl member.</summary>
    /// <param name="pageType">The pageType value.</param>
    /// <returns>The result.</returns>
    private static Type CheckIsControl(Type pageType)
    {
        if (!typeof(Control).IsAssignableFrom(pageType))
        {
            throw new InvalidOperationException("The page should be an Avalonia control.");
        }

        return pageType;
    }
}
