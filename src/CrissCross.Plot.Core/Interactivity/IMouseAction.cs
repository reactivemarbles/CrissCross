// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity;

/// <summary>User actions that occur at a point in pixel space.</summary>
public interface IMouseAction : IUserAction
{
    /// <summary>Gets the pixel location associated with the mouse action.</summary>
    Pixel Pixel { get; }
}
