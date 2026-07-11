// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>Describes the observable execution state projected by command execution controls.</summary>
public enum CommandButtonState
{
    /// <summary>The command is ready and has not recently completed.</summary>
    Idle,

    /// <summary>The command is currently executing.</summary>
    Executing,

    /// <summary>The command completed successfully.</summary>
    Succeeded,

    /// <summary>The command failed and has an associated error.</summary>
    Failed,

    /// <summary>The command was cancelled before normal completion.</summary>
    Cancelled
}
