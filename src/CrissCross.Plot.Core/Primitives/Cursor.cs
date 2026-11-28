// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Identifies a pointer cursor shape.</summary>
public enum Cursor
{
    /// <summary>The standard arrow pointer.</summary>
    Arrow,

    /// <summary>A prohibited-action cursor.</summary>
    No,

    /// <summary>A wait or busy cursor.</summary>
    Wait,

    /// <summary>A hand pointer used for clickable items.</summary>
    Hand,

    /// <summary>A crosshair cursor.</summary>
    Cross,

    /// <summary>A cursor indicating movement in all directions.</summary>
    SizeAll,

    /// <summary>A cursor indicating vertical resizing.</summary>
    SizeNorthSouth,

    /// <summary>A cursor indicating horizontal resizing.</summary>
    SizeWestEast,
}
