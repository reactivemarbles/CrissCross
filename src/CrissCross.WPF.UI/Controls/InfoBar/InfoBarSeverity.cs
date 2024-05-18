// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// InfoBarSeverity.
/// </summary>
public enum InfoBarSeverity
{
    /// <summary>
    /// Communicates that the InfoBar is displaying general information that requires the user's attention.
    /// </summary>
    Informational = 0,

    /// <summary>
    /// Communicates that the InfoBar is displaying information regarding a long-running and/or background task
    /// that has completed successfully.
    /// </summary>
    Success = 1,

    /// <summary>
    /// Communicates that the InfoBar is displaying information regarding a condition that might cause a problem in
    /// the future.
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Communicates that the InfoBar is displaying information regarding an error or problem that has occurred.
    /// </summary>
    Error = 3
}
