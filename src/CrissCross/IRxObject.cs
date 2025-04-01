﻿// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using ReactiveUI;

namespace CrissCross;

/// <summary>
/// interface for RxBase.
/// </summary>
/// <seealso cref="System.IDisposable"/>
public interface IRxObject : IReactiveNotifyPropertyChanged<IReactiveObject>, IHandleObservableErrors, INotifiyRoutableViewModel, ICancelable, IAmBuilt
{
    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    /// <value>
    /// The display name.
    /// </value>
    string? DisplayName { get; set; }
}
