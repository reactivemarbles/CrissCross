// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Extensions;
#else
namespace CrissCross.WPF.UI.Extensions;
#endif

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
        public INavigationService? SetPaneDisplayMode(NavigationViewPaneDisplayMode paneDisplayMode)
        {
            var navigationControl = navigationService?.GetNavigationControl();

            navigationControl?.PaneDisplayMode = paneDisplayMode;

            return navigationService;
        }
    }
}
