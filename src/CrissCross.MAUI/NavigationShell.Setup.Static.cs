// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace CrissCross.MAUI;

/// <summary>Hosts MAUI shell navigation for routed view models.</summary>
public partial class NavigationShell
{
    /// <summary>Writes shell navigation locations to the debug output.</summary>
    /// <param name="eventArgs">The event arguments.</param>
    private static void TraceShellNavigation(ShellNavigatedEventArgs eventArgs)
    {
        Debug.WriteLine($"Current {eventArgs.Current.Location.OriginalString}");
        if (eventArgs.Previous is null)
        {
            return;
        }

        Debug.WriteLine($"Previous {eventArgs.Previous.Location.OriginalString}");
    }

    /// <summary>Determines whether a shell navigation source represents backward navigation.</summary>
    /// <param name="source">The navigation source.</param>
    /// <returns><c>true</c> when the source navigates backward; otherwise, <c>false</c>.</returns>
    private static bool IsBackNavigation(ShellNavigationSource source) =>
        source == ShellNavigationSource.Pop || source == ShellNavigationSource.PopToRoot;

    /// <summary>Publishes a navigation event to views that requested notifications.</summary>
    /// <param name="navigationEvent">The navigation event.</param>
    /// <param name="toView">The destination view.</param>
    /// <param name="fromView">The source view.</param>
    private static void PublishViewNavigationEvent(
        IViewModelNavigationEventArgs navigationEvent,
        INotifiyNavigation? toView,
        INotifiyNavigation? fromView)
    {
        if (toView?.ISetupNavigatedTo != true && fromView?.ISetupNavigatedFrom != true)
        {
            return;
        }

        ViewModelRoutedViewHostMixins.SetWhenNavigated.OnNext(navigationEvent);
    }
}
