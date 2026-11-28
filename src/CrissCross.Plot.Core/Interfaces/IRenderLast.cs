// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides a second rendering pass after plot axes are drawn.</summary>
public interface IRenderLast
{
    /// <summary>Renders content on top of the axes.</summary>
    /// <param name="rp">The render pack containing the canvas and paint.</param>
    void RenderLast(RenderPack rp);
}
