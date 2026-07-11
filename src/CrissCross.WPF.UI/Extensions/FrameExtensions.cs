// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace CrissCross.WPF.UI.Extensions;

/// <summary>Set of extensions for <see cref="Frame"/>.</summary>
public static class FrameExtensions
{
    /// <summary>Provides extension members.</summary>
    /// <param name="frame">The frame value.</param>
    extension(Frame frame)
    {
        /// <summary>Gets <see cref="FrameworkElement.DataContext"/> from <see cref="Frame"/>.</summary>
        /// <returns>DataContext of currently active element, otherwise <see langword="null"/>.</returns>
        public object? DataContext => frame?.Content is not FrameworkElement element ? null : element.DataContext;

        /// <summary>Cleans <see cref="Frame"/> journal.</summary>
        public void CleanNavigation()
        {
            while (frame?.CanGoBack == true)
            {
                _ = frame.RemoveBackEntry();
            }
        }
    }
}
