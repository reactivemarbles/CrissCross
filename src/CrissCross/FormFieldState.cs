// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>
/// Describes the visual and validation state of a form field container.
/// </summary>
public enum FormFieldState
{
    /// <summary>
    /// The field is in its default state.
    /// </summary>
    Normal,

    /// <summary>
    /// The field currently has input focus.
    /// </summary>
    Focused,

    /// <summary>
    /// The field value has passed validation.
    /// </summary>
    Valid,

    /// <summary>
    /// The field has non-blocking validation warnings.
    /// </summary>
    Warning,

    /// <summary>
    /// The field has blocking validation errors.
    /// </summary>
    Invalid,

    /// <summary>
    /// The field has validation work in progress.
    /// </summary>
    Pending
}
