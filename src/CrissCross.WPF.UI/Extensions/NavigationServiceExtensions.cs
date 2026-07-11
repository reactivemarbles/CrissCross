// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Extensions;

/// <summary>Extensions for the <see cref="INavigationService"/>.</summary>
public static class NavigationServiceExtensions
{
    /// <summary>Provides extension members.</summary>
    /// <param name="navigationService">The navigationService value.</param>
    extension(INavigationService navigationService)
    {
        /// <summary>Sets the pane display mode of the navigation service.</summary>
        /// <param name="paneDisplayMode">The pane display mode.</param>
        /// <returns>Same <see cref="INavigationService"/> so multiple calls can be chained.</returns>
        public INavigationService? SetPaneDisplayMode(
            NavigationViewPaneDisplayMode paneDisplayMode)
        {
            var navigationControl = navigationService?.GetNavigationControl();

            navigationControl?.PaneDisplayMode = paneDisplayMode;

            return navigationService;
        }
    }
}
