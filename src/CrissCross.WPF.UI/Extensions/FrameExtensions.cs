// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

using System.Windows.Controls;

namespace CrissCross.WPF.UI.Extensions;

/// <summary>
/// Set of extensions for <see cref="System.Windows.Controls.Frame"/>.
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
