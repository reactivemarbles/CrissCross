// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Describes the geometry of a color gradient used to fill an area.</summary>
public enum GradientType
{
    /// <summary>Transitions colors along a straight line.</summary>
    Linear,

    /// <summary>Transitions colors outward from a center point.</summary>
    Radial,

    /// <summary>Transitions colors around a center point by angle.</summary>
    Sweep,

    /// <summary>Transitions colors between two circles.</summary>
    TwoPointConical
}
