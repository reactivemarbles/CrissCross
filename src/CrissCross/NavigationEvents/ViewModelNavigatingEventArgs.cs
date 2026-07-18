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

/// <summary>View Model Navigating Event Args.</summary>
[DataContract]
public class ViewModelNavigatingEventArgs : ViewModelNavigationEventArgs, IViewModelNavigatingEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="ViewModelNavigatingEventArgs"/> class.</summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <param name="navType">Type of the nav.</param>
    /// <param name="view">The view.</param>
    /// <param name="hostName">The host name.</param>
    public ViewModelNavigatingEventArgs(
        IRxObject? from,
        IRxObject? to,
        NavigationType navType,
        IViewFor? view,
        string? hostName)
        : this(from, to, navType, view, hostName, null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ViewModelNavigatingEventArgs"/> class.</summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <param name="navType">Type of the nav.</param>
    /// <param name="view">The view.</param>
    /// <param name="hostName">The host name.</param>
    /// <param name="parameter">The parameter.</param>
    public ViewModelNavigatingEventArgs(
        IRxObject? from,
        IRxObject? to,
        NavigationType navType,
        IViewFor? view,
        string? hostName,
        object? parameter)
        : base(from, to, navType, view, hostName, parameter)
    {
    }

    /// <summary>Gets or sets whether navigation is canceled.</summary>
    /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
    [DataMember]
    public bool Cancel { get; set; }
}
