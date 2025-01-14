// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Notifies class about being navigated.
/// </summary>
public interface INavigationAware
{
    /// <summary>
    /// Method triggered when the class is navigated.
    /// </summary>
    void OnNavigatedTo();

    /// <summary>
    /// Method triggered when the navigation leaves the current class.
    /// </summary>
    void OnNavigatedFrom();
}
