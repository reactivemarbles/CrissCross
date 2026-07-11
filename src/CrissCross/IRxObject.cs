// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI;

namespace CrissCross;

/// <summary>Provides the base contract for CrissCross reactive view models.</summary>
/// <seealso cref="System.IDisposable"/>
public interface IRxObject : IReactiveNotifyPropertyChanged<IReactiveObject>, IHandleObservableErrors, INotifiyRoutableViewModel, IDisposable, IAmBuilt
{
    /// <summary>Gets or sets the display name.</summary>
    /// <value>
    /// The display name.
    /// </value>
    string? DisplayName { get; set; }
}
