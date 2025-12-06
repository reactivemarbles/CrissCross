// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;

namespace CrissCross.Avalonia.UI;

/// <summary>
/// Service that provides pages for navigation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PageService" /> class.
/// Creates new instance and attaches the <see cref="IServiceProvider" />.
/// </remarks>
/// <param name="serviceProvider">The service provider.</param>
internal class PageService(IServiceProvider serviceProvider) : IPageService
{
    /// <inheritdoc />
    public T? GetPage<T>()
        where T : class => (T?)serviceProvider.GetService(CheckIsControl(typeof(T)));

    /// <inheritdoc />
    public Control? GetPage(Type pageType) =>
        serviceProvider.GetService(CheckIsControl(pageType)) as Control;

    private static Type CheckIsControl(Type pageType)
    {
        if (!typeof(Control).IsAssignableFrom(pageType))
        {
            throw new InvalidOperationException("The page should be an Avalonia control.");
        }

        return pageType;
    }
}
