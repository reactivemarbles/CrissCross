// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>
/// IView Model Navigating EventArgs.
/// </summary>
public interface IViewModelNavigatingEventArgs : IViewModelNavigationEventArgs
{
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="IViewModelNavigatingEventArgs"/> is cancel.
    /// </summary>
    /// <value>
    ///   <c>true</c> if cancel; otherwise, <c>false</c>.
    /// </value>
    bool Cancel { get; set; }
}