// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.TaskBar;

/// <summary>
/// Specifies the state of the progress indicator in the Windows task bar.
/// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.windows.shell.taskbaritemprogressstate?view=windowsdesktop-5.0"/>.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1027:Mark enums with FlagsAttribute", Justification = "Not Required.")]
public enum TaskBarProgressState
{
    /// <summary>
    /// No progress indicator is displayed in the task bar area.
    /// </summary>
    None = 0x0,

    /// <summary>
    /// A pulsing green (W10) or gray (W11) indicator is displayed in the task bar area.
    /// </summary>
    Indeterminate = 0x1,

    /// <summary>
    /// A green progress indicator is displayed in the task bar area.
    /// </summary>
    Normal = 0x2,

    /// <summary>
    /// A red progress indicator is displayed in the task bar area.
    /// </summary>
    Error = 0x4,

    /// <summary>
    /// A yellow progress indicator is displayed in the task bar area.
    /// </summary>
    Paused = 0x8
}
