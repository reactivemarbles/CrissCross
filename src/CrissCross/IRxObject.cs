// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Provides the base contract for CrissCross reactive view models.</summary>
/// <seealso cref="System.IDisposable"/>
public interface IRxObject
    : IReactiveNotifyPropertyChanged<IReactiveObject>,
        IHandleObservableErrors,
        INotifiyRoutableViewModel,
        IDisposable,
        IAmBuilt
{
    /// <summary>Gets the navigation host or component name.</summary>
    new string? Name { get; }

    /// <summary>Gets or sets the display name.</summary>
    /// <value>
    /// The display name.
    /// </value>
    string? DisplayName { get; set; }
}
