// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity;

/// <summary>Mouse actions that describe a button changing state.</summary>
public interface IMouseButtonAction : IMouseAction
{
    /// <summary>Gets the mouse button associated with this action.</summary>
    MouseButton Button { get; }

    /// <summary>Gets whether the button is pressed.</summary>
    bool IsPressed { get; }
}
