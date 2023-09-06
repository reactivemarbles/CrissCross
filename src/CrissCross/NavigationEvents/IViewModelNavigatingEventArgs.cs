// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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