// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents the location of a point relative to a rectangle.</summary>
public enum Alignment
{
    /// <summary>The upper-left corner.</summary>
    UpperLeft,

    /// <summary>The upper edge's center.</summary>
    UpperCenter,

    /// <summary>The upper-right corner.</summary>
    UpperRight,

    /// <summary>The left edge's center.</summary>
    MiddleLeft,

    /// <summary>The center of the rectangle.</summary>
    MiddleCenter,

    /// <summary>The right edge's center.</summary>
    MiddleRight,

    /// <summary>The lower-left corner.</summary>
    LowerLeft,

    /// <summary>The lower edge's center.</summary>
    LowerCenter,

    /// <summary>The lower-right corner.</summary>
    LowerRight,
}
