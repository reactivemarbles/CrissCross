// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents InfoBarSeverity.</summary>
public enum InfoBarSeverity
{
    /// <summary>Communicates that the InfoBar is displaying general information that requires the user's attention.</summary>
    Informational = 0,

    /// <summary>
    /// Communicates that the InfoBar is displaying information regarding a long-running and/or background task
    /// that has completed successfully.
    /// </summary>
    Success = 1,

    /// <summary>Communicates that the InfoBar is displaying information regarding a condition that might cause a problem in the future.</summary>
    Warning = 2,

    /// <summary>Communicates that the InfoBar is displaying information regarding an error or problem that has occurred.</summary>
    Error = 3
}
