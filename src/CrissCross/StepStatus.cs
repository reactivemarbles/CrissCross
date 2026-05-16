// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>
/// Defines platform-neutral workflow step state for steppers and wizard progress controls.
/// </summary>
public enum StepStatus
{
    /// <summary>
    /// The step has not started.
    /// </summary>
    Pending,

    /// <summary>
    /// The step is the active/current step.
    /// </summary>
    Active,

    /// <summary>
    /// The step has completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// The optional step was skipped.
    /// </summary>
    Skipped,

    /// <summary>
    /// The step has a non-blocking warning.
    /// </summary>
    Warning,

    /// <summary>
    /// The step has a blocking error.
    /// </summary>
    Error
}
