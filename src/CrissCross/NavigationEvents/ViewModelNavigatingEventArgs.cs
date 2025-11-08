// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;
using ReactiveUI;

namespace CrissCross;

/// <summary>
/// View Model Navigating Event Args.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ViewModelNavigatingEventArgs" /> class.
/// </remarks>
/// <param name="from">From.</param>
/// <param name="to">To.</param>
/// <param name="navType">Type of the nav.</param>
/// <param name="view">The view.</param>
/// <param name="hostName">The hostName.</param>
/// <param name="parameter">The parameter.</param>
[DataContract]
public class ViewModelNavigatingEventArgs(IRxObject? from, IRxObject? to, NavigationType navType, IViewFor? view, string? hostName, object? parameter = null) : ViewModelNavigationEventArgs(from, to, navType, view, hostName, parameter), IViewModelNavigatingEventArgs
{
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="ViewModelNavigatingEventArgs"/>
    /// is canceled.
    /// </summary>
    /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
    [DataMember]
    public bool Cancel { get; set; }
}
