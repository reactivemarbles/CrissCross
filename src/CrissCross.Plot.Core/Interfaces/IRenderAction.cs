// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents an action performed during plot rendering.</summary>
public interface IRenderAction
{
    /// <summary>Performs this render action.</summary>
    /// <param name="rp">The render state used by this action.</param>
    void Render(RenderPack rp);
}
