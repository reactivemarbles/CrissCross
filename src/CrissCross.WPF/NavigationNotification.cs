// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF;
#else
namespace CrissCross.WPF;
#endif

/// <summary>Dispatches completed-navigation notifications.</summary>
internal static class NavigationNotification
{
    /// <summary>Notifies participating views about completed navigation.</summary>
    /// <param name="eventArgs">The completed navigation event.</param>
    /// <param name="toView">The target view.</param>
    /// <param name="fromView">The source view.</param>
    internal static void NotifyViews(
        ViewModelNavigationEventArgs eventArgs,
        INotifiyNavigation? toView,
        INotifiyNavigation? fromView)
    {
        if (toView?.ISetupNavigatedTo != true && fromView?.ISetupNavigatedFrom != true)
        {
            return;
        }

        ViewModelRoutedViewHostMixins.SetWhenNavigated.OnNext(eventArgs);
    }

    /// <summary>Notifies the source view model when the source view does not handle navigation.</summary>
    /// <param name="eventArgs">The completed navigation event.</param>
    /// <param name="fromView">The source view.</param>
    /// <param name="currentViewModel">The source view model.</param>
    internal static void NotifyFrom(
        ViewModelNavigationEventArgs eventArgs,
        INotifiyNavigation? fromView,
        IRxObject? currentViewModel)
    {
        if (fromView?.ISetupNavigatedTo == true)
        {
            return;
        }

        currentViewModel?.WhenNavigatedFrom(eventArgs);
    }
}
