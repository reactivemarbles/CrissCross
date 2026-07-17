// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Extensions;
#else
namespace CrissCross.WPF.UI.Extensions;
#endif

/// <summary>Extensions for the <see cref="ContextMenu"/>.</summary>
internal static class ContextMenuExtensions
{
    /// <summary>Provides extension members.</summary>
    /// <param name="contextMenu">The contextMenu value.</param>
    extension(ContextMenu contextMenu)
    {
        /// <summary>Tries to apply Mica effect to the <see cref="ContextMenu"/>.</summary>
        public void ApplyMica() => contextMenu.Opened += ContextMenuOnOpened;
    }

    /// <summary>Provides the ContextMenuOnOpened member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
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

        if (ApplicationThemeManager.GetAppTheme() != ApplicationTheme.Dark)
        {
            return;
        }

        _ = UnsafeNativeMethods.ApplyWindowDarkMode(source.Handle);
    }
}
