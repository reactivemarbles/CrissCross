// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Provides navigation using a resolved ViewModel/View pair.</summary>
public interface IResolvedViewModelRoutedViewHost : IViewModelRoutedViewHost
{
    /// <summary>Navigates the specified resolved navigation pair.</summary>
    /// <param name="resolution">The resolved navigation pair.</param>
    void Navigate(NavigationResolution resolution);

    /// <summary>Navigates the specified resolved navigation pair and resets history.</summary>
    /// <param name="resolution">The resolved navigation pair.</param>
    void NavigateAndReset(NavigationResolution resolution);
}
