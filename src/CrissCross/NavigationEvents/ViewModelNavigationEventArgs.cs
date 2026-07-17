// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;
using ReactiveUI;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>View Model Navigation EventArgs.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ViewModelNavigationEventArgs" /> class.
/// </remarks>
/// <param name="from">From.</param>
/// <param name="to">To.</param>
/// <param name="navType">Type of the nav.</param>
/// <param name="view">The view.</param>
/// <param name="hostName">The Hostname.</param>
/// <param name="parmeter">The parmeter.</param>
[DataContract]
public class ViewModelNavigationEventArgs(
    IRxObject? from,
    IRxObject? to,
    NavigationType navType,
    IViewFor? view,
    string? hostName,
    object? parmeter = null) : ViewModelNavigationBaseEventArgs(from, to, parmeter), IViewModelNavigationEventArgs
{
    /// <summary>Gets or sets the name of the host.</summary>
    /// <value>
    /// The name of the host.
    /// </value>
    [DataMember]
    public string? HostName { get; set; } = hostName;

    /// <summary>Gets the type of the navigation.</summary>
    /// <value>The type of the navigation.</value>
    [DataMember]
    public NavigationType NavigationType { get; protected set; } = navType;

    /// <summary>Gets or sets the view.</summary>
    /// <value>The view.</value>
    [DataMember]
    public IViewFor? View { get; set; } = view;
}
