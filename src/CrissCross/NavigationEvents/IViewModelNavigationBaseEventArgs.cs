// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>IView Model Navigation Base Event Args.</summary>
public interface IViewModelNavigationBaseEventArgs
{
    /// <summary>Gets from.</summary>
    /// <value>
    /// From.
    /// </value>
    IRxObject? From { get; }

    /// <summary>Gets the navigation parameter.</summary>
    /// <value>
    /// The navigation parameter.
    /// </value>
    object? NavigationParameter { get; }

    /// <summary>Gets to.</summary>
    /// <value>
    /// To.
    /// </value>
    IRxObject? To { get; }
}
