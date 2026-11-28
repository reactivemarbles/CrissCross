// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a contour path at a specified z-value.</summary>
/// <param name="Path">The path of the contour line.</param>
/// <param name="Z">The z-value of the contour line.</param>
public readonly record struct ContourLine(CoordinatePath Path, double Z);
