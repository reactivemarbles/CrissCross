// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>
/// Defines the platform-neutral layout orientation for stepper and wizard progress controls.
/// </summary>
public enum StepperOrientation
{
    /// <summary>
    /// Steps are arranged from left to right.
    /// </summary>
    Horizontal,

    /// <summary>
    /// Steps are arranged from top to bottom.
    /// </summary>
    Vertical,

    /// <summary>
    /// Steps use a compact layout suitable for narrow regions.
    /// </summary>
    Compact
}
