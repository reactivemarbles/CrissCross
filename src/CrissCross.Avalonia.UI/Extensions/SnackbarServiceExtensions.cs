// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.Avalonia.UI.Controls;
#else
using CrissCross.Avalonia.UI.Controls;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Extensions;
#else
namespace CrissCross.Avalonia.UI.Extensions;
#endif

/// <summary>Extensions for the <see cref="ISnackbarService"/>.</summary>
public static class SnackbarServiceExtensions
{
    /// <summary>Provides extension members for the <see cref="ISnackbarService"/>.</summary>
    /// <param name="snackbarService">The snackbar service.</param>
    extension(ISnackbarService snackbarService)
    {
        /// <summary>Shows the snackbar. If it is already visible, firstly hides it for a moment, changes its content,
        /// and then shows it again.</summary>
        /// <param name="title">Name displayed on top of snackbar.</param>
        /// <param name="message">Message inside the snackbar.</param>
        public void Show(string title, string message) =>
            snackbarService?.Show(title, message, ControlAppearance.Secondary, null, snackbarService.DefaultTimeOut);

        /// <summary>Shows the snackbar. If it is already visible, firstly hides it for a moment, changes its content,
        /// and then shows it again.</summary>
        /// <param name="title">Name displayed on top of snackbar.</param>
        /// <param name="message">Message inside the snackbar.</param>
        /// <param name="appearance">Display style.</param>
        public void Show(string title, string message, ControlAppearance appearance) =>
            snackbarService?.Show(title, message, appearance, null, snackbarService.DefaultTimeOut);

        /// <summary>Shows the snackbar. If it is already visible, firstly hides it for a moment, changes its content,
        /// and then shows it again.</summary>
        /// <param name="title">Name displayed on top of snackbar.</param>
        /// <param name="message">Message inside the snackbar.</param>
        /// <param name="icon">Additional icon on the left.</param>
        public void Show(string title, string message, IconElement icon) =>
            snackbarService?.Show(title, message, ControlAppearance.Secondary, icon, snackbarService.DefaultTimeOut);

        /// <summary>Shows the snackbar. If it is already visible, firstly hides it for a moment, changes its content,
        /// and then shows it again.</summary>
        /// <param name="title">Name displayed on top of snackbar.</param>
        /// <param name="message">Message inside the snackbar.</param>
        /// <param name="timeout">The time after which the snackbar should disappear.</param>
        public void Show(string title, string message, TimeSpan timeout) =>
            snackbarService?.Show(title, message, ControlAppearance.Secondary, null, timeout);

        /// <summary>Shows the snackbar. If it is already visible, firstly hides it for a moment, changes its content,
        /// and then shows it again.</summary>
        /// <param name="title">Name displayed on top of snackbar.</param>
        /// <param name="message">Message inside the snackbar.</param>
        /// <param name="appearance">Display style.</param>
        /// <param name="timeout">The time after which the snackbar should disappear.</param>
        public void Show(string title, string message, ControlAppearance appearance, TimeSpan timeout) =>
            snackbarService?.Show(title, message, appearance, null, timeout);

        /// <summary>Shows the snackbar. If it is already visible, firstly hides it for a moment, changes its content,
        /// and then shows it again.</summary>
        /// <param name="title">Name displayed on top of snackbar.</param>
        /// <param name="message">Message inside the snackbar.</param>
        /// <param name="icon">Additional icon on the left.</param>
        /// <param name="timeout">The time after which the snackbar should disappear.</param>
        public void Show(string title, string message, IconElement icon, TimeSpan timeout) =>
            snackbarService?.Show(title, message, ControlAppearance.Secondary, icon, timeout);
    }
}
