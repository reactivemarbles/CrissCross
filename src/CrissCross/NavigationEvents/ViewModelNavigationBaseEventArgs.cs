// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;

namespace CrissCross;

/// <summary>View Model Navigation Base Event Args.</summary>
/// <remarks>Initializes shared navigation event state.</remarks>
/// <param name="from">The navigation source.</param>
/// <param name="to">The navigation destination.</param>
/// <param name="navigationParameter">The navigation parameter.</param>
/// <seealso cref="EventArgs" />
[DataContract]
public abstract class ViewModelNavigationBaseEventArgs(
    IRxObject? from = null,
    IRxObject? to = null,
    object? navigationParameter = null)
                : EventArgs, IViewModelNavigationBaseEventArgs
{
    /// <summary>Gets where navigation starts.</summary>
    /// <value>From.</value>
    [DataMember]
    public IRxObject? From { get; protected set; } = from;

    /// <summary>Gets the navigation parameter.</summary>
    /// <value>The navigation parameter.</value>
    [DataMember]
    public object? NavigationParameter { get; protected set; } = navigationParameter;

    /// <summary>Gets where navigation ends.</summary>
    /// <value>To.</value>
    [DataMember]
    public IRxObject? To { get; protected set; } = to;
}
