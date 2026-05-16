// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>
/// Represents lifecycle state of a reactive plot connection.
/// </summary>
public enum ReactivePlotConnectionState
{
    /// <summary>
    /// The binder is subscribing to sources.
    /// </summary>
    Connecting,

    /// <summary>
    /// At least one valid update has been applied.
    /// </summary>
    Active,

    /// <summary>
    /// All sources completed normally.
    /// </summary>
    Completed,

    /// <summary>
    /// A source or validation error faulted the connection.
    /// </summary>
    Faulted,

    /// <summary>
    /// The connection was explicitly disposed.
    /// </summary>
    Disposed,
}
