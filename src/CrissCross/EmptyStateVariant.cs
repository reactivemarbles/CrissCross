// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>
/// Describes the reason an empty-state surface is being shown.
/// </summary>
public enum EmptyStateVariant
{
    /// <summary>
    /// There is no data to show yet.
    /// </summary>
    NoData,

    /// <summary>
    /// Search or filter criteria produced no matching results.
    /// </summary>
    NoResults,

    /// <summary>
    /// An error prevented content from loading.
    /// </summary>
    Error,

    /// <summary>
    /// Content is unavailable because the application is offline.
    /// </summary>
    Offline,

    /// <summary>
    /// Content requires permissions the current user does not have.
    /// </summary>
    PermissionRequired
}
