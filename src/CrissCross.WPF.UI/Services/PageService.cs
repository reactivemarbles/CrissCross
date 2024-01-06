// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using Wpf.Ui;

namespace CrissCross.WPF.UI;
#pragma warning disable CA1812
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
        where T : class => (T?)serviceProvider.GetService(CheckIsFrameworkElement(typeof(T)));

    /// <inheritdoc />
    public FrameworkElement? GetPage(Type pageType) =>
        serviceProvider.GetService(CheckIsFrameworkElement(pageType)) as FrameworkElement;

    private static Type CheckIsFrameworkElement(Type pageType)
    {
        if (!typeof(FrameworkElement).IsAssignableFrom(pageType))
        {
            throw new InvalidOperationException("The page should be a WPF control.");
        }

        return pageType;
    }
}
#pragma warning restore CA1812
