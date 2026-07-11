// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI;

namespace CrissCross;

/// <summary>INotifiy Navigation.</summary>
/// <seealso cref="IActivatableView" />
/// <seealso cref="IDisposable" />
public interface INotifiyNavigation : IActivatableView, IDisposable
{
    /// <summary>Gets or sets a value indicating whether [i setup navigated to].</summary>
    /// <value>
    ///   <c>true</c> if [i setup navigated to]; otherwise, <c>false</c>.
    /// </value>
    bool ISetupNavigatedTo { get; set; }

    /// <summary>Gets or sets a value indicating whether [i setup navigated from].</summary>
    /// <value>
    ///   <c>true</c> if [i setup navigated from]; otherwise, <c>false</c>.
    /// </value>
    bool ISetupNavigatedFrom { get; set; }

    /// <summary>Gets or sets a value indicating whether [i setup navigating].</summary>
    /// <value>
    ///   <c>true</c> if [i setup navigating]; otherwise, <c>false</c>.
    /// </value>
    bool ISetupNavigating { get; set; }

    /// <summary>Gets the clean up.</summary>
    /// <value>
    /// The clean up.
    /// </value>
    CompositeDisposable CleanUp { get; }
}
