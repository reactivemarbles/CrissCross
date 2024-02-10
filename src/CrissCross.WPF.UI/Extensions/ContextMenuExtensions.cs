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
/// Extensions for the <see cref="ContextMenu"/>.
/// </summary>
internal static class ContextMenuExtensions
{
    /// <summary>
    /// Tries to apply Mica effect to the <see cref="ContextMenu"/>.
    /// </summary>
    public static void ApplyMica(this ContextMenu contextMenu) => contextMenu.Opened += ContextMenuOnOpened;

    private static void ContextMenuOnOpened(object sender, RoutedEventArgs e)
    {
        if (sender is not ContextMenu contextMenu)
        {
            return;
        }

        if (PresentationSource.FromVisual(contextMenu) is not HwndSource source)
        {
            return;
        }

        if (ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark)
        {
            UnsafeNativeMethods.ApplyWindowDarkMode(source.Handle);
        }
    }
}
