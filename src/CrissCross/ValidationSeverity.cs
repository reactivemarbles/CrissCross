// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>
/// Describes the severity of a validation message.
/// </summary>
public enum ValidationSeverity
{
    /// <summary>
    /// Informational validation feedback that does not block submission.
    /// </summary>
    Information,

    /// <summary>
    /// Successful validation feedback.
    /// </summary>
    Success,

    /// <summary>
    /// Warning feedback that should be reviewed but does not block submission.
    /// </summary>
    Warning,

    /// <summary>
    /// Error feedback that blocks submission.
    /// </summary>
    Error,

    /// <summary>
    /// Validation feedback for an async check that is still running.
    /// </summary>
    Pending
}
