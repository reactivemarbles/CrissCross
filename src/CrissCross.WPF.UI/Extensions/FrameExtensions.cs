// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace CrissCross.WPF.UI.Extensions;

/// <summary>
/// Set of extensions for <see cref="Frame"/>.
/// </summary>
public static class FrameExtensions
{
    /// <summary>
    /// Gets <see cref="FrameworkElement.DataContext"/> from <see cref="Frame"/>.
    /// </summary>
    /// <param name="frame">Selected frame.</param>
    /// <returns>DataContext of currently active element, otherwise <see langword="null"/>.</returns>
    public static object? GetDataContext(this Frame frame) => frame?.Content is not FrameworkElement element ? null : element.DataContext;

    /// <summary>
    /// Cleans <see cref="Frame"/> journal.
    /// </summary>
    /// <param name="frame">Selected frame.</param>
    public static void CleanNavigation(this Frame frame)
    {
        while (frame?.CanGoBack == true)
        {
            frame.RemoveBackEntry();
        }
    }
}
