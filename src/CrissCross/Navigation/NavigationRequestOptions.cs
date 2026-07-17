// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Threading;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Configures contract, parameter, and cancellation behavior for a navigation request.</summary>
public sealed class NavigationRequestOptions
{
    /// <summary>Gets or sets the optional navigation host name.</summary>
    public string? HostName { get; set; }

    /// <summary>Gets or sets the optional navigation contract.</summary>
    public string? Contract { get; set; }

    /// <summary>Gets or sets the optional navigation parameter.</summary>
    public object? Parameter { get; set; }

    /// <summary>Gets or sets the cancellation token.</summary>
    public CancellationToken CancellationToken { get; set; }
}
