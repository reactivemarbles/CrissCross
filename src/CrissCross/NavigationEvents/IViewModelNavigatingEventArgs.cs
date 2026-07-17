// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>IView Model Navigating EventArgs.</summary>
public interface IViewModelNavigatingEventArgs : IViewModelNavigationEventArgs
{
    /// <summary>Gets or sets whether navigation is canceled.</summary>
    /// <value>
    ///   <c>true</c> if cancel; otherwise, <c>false</c>.
    /// </value>
    bool Cancel { get; set; }
}
