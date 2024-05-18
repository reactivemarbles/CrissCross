// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
