// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Specifies how consecutive data points are connected.</summary>
public enum ConnectStyle
{
    /// <summary>Connect points with straight lines.</summary>
    Straight,

    /// <summary>Connect points with a line horizontally, then vertically.</summary>
    StepHorizontal,

    /// <summary>Connect points with a line vertically, then horizontally.</summary>
    StepVertical,
}
