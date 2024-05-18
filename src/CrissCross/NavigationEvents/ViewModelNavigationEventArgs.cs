// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;
using ReactiveUI;

namespace CrissCross;

/// <summary>
/// View Model Navigation EventArgs.
/// </summary>
[DataContract]
public class ViewModelNavigationEventArgs : ViewModelNavigationBaseEventArgs, IViewModelNavigationEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="ViewModelNavigationEventArgs" /> class.</summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <param name="navType">Type of the nav.</param>
    /// <param name="view">The view.</param>
    /// <param name="hostName">The Hostname.</param>
    /// <param name="parmeter">The parmeter.</param>
    public ViewModelNavigationEventArgs(IRxObject? from, IRxObject? to, NavigationType navType, IViewFor? view, string? hostName, object? parmeter = null)
    {
        From = from;
        To = to;
        View = view;
        NavigationType = navType;
        NavigationParameter = parmeter;
        HostName = hostName;
    }

    /// <summary>
    /// Gets or sets the name of the host.
    /// </summary>
    /// <value>
    /// The name of the host.
    /// </value>
    [DataMember]
    public string? HostName { get; set; }

    /// <summary>
    /// Gets or sets the type of the navigation.
    /// </summary>
    /// <value>The type of the navigation.</value>
    [DataMember]
    public NavigationType NavigationType { get; protected set; }

    /// <summary>
    /// Gets or sets the view.
    /// </summary>
    /// <value>The view.</value>
    [DataMember]
    public IViewFor? View { get; set; }
}
