// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>I View Model Navigation EventArgs.</summary>
public interface IViewModelNavigationEventArgs : IViewModelNavigationBaseEventArgs
{
    /// <summary>Gets or sets the name of the host.</summary>
    /// <value>
    /// The name of the host.
    /// </value>
    string? HostName { get; set; }

    /// <summary>Gets the type of the navigation.</summary>
    /// <value>
    /// The type of the navigation.
    /// </value>
    NavigationType NavigationType { get; }

    /// <summary>Gets or sets the view.</summary>
    /// <value>
    /// The view.
    /// </value>
    IViewFor? View { get; set; }
}
