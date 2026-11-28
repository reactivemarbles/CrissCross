// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity;

/// <summary>Provides commonly used keys as a safer alternative to creating them directly.</summary>
public static class StandardKeys
{
    /// <summary>The Alt modifier key.</summary>
    public static readonly Key Alt = new("alt");

    /// <summary>The Control modifier key.</summary>
    public static readonly Key Control = new("ctrl");

    /// <summary>The Shift modifier key.</summary>
    public static readonly Key Shift = new("shift");

    /// <summary>The Down arrow key.</summary>
    public static readonly Key Down = new("down");

    /// <summary>The Up arrow key.</summary>
    public static readonly Key Up = new("up");

    /// <summary>The Left arrow key.</summary>
    public static readonly Key Left = new("left");

    /// <summary>The Right arrow key.</summary>
    public static readonly Key Right = new("right");

    /// <summary>An unknown key.</summary>
    public static readonly Key Unknown = new("unknown");

    /// <summary>The A key.</summary>
    public static readonly Key A = new("a");

    /// <summary>Determines whether the specified key is an arrow key.</summary>
    /// <param name="key">The key to check.</param>
    /// <returns><see langword="true"/> when the key is an arrow key.</returns>
    public static bool IsArrowKey(Key key) => key == Left || key == Right || key == Down || key == Up;
}
