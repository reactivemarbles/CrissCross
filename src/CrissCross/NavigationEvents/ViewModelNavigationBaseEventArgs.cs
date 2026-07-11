// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;

namespace CrissCross;

/// <summary>View Model Navigation Base Event Args.</summary>
/// <seealso cref="EventArgs" />
[DataContract]
public abstract class ViewModelNavigationBaseEventArgs
                : EventArgs, IViewModelNavigationBaseEventArgs
{
    /// <summary>Gets where navigation starts.</summary>
    /// <value>From.</value>
    [DataMember]
    public IRxObject? From { get; protected set; }

    /// <summary>Gets the navigation parameter.</summary>
    /// <value>The navigation parameter.</value>
    [DataMember]
    public object? NavigationParameter { get; protected set; }

    /// <summary>Gets where navigation ends.</summary>
    /// <value>To.</value>
    [DataMember]
    public IRxObject? To { get; protected set; }
}
