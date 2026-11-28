// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity.UserActions;

/// <summary>Provides the default timestamp assigned to user actions.</summary>
internal static class UserActionTimestamp
{
    /// <summary>Gets the current UTC time for a newly created user action.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2010",
        Justification = "User-action record constructors preserve their existing API by capturing creation time without requiring a TimeProvider parameter.")]
    internal static DateTime UtcNow => DateTime.UtcNow;
}
