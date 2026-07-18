// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Notifies class about being navigated.</summary>
public interface INavigationAware
{
    /// <summary>Method triggered when the class is navigated.</summary>
    void OnNavigatedTo();

    /// <summary>Method triggered when the navigation leaves the current class.</summary>
    void OnNavigatedFrom();
}
