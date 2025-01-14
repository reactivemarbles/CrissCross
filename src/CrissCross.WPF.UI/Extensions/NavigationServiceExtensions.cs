// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Extensions;

/// <summary>
/// Extensions for the <see cref="INavigationService"/>.
/// </summary>
public static class NavigationServiceExtensions
{
    /// <summary>
    /// Sets the pane display mode of the navigation service.
    /// </summary>
    /// <param name="navigationService">The navigation service.</param>
    /// <param name="paneDisplayMode">The pane display mode.</param>
    /// <returns>Same <see cref="INavigationService"/> so multiple calls can be chained.</returns>
    public static INavigationService? SetPaneDisplayMode(
        this INavigationService navigationService,
        NavigationViewPaneDisplayMode paneDisplayMode)
    {
        var navigationControl = navigationService?.GetNavigationControl();

        if (navigationControl is not null)
        {
            navigationControl.PaneDisplayMode = paneDisplayMode;
        }

        return navigationService;
    }
}
