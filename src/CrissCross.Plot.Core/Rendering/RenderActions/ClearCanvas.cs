// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering.RenderActions;

/// <summary>Clears the canvas when the plot is configured to clear before rendering.</summary>
public class ClearCanvas : IRenderAction
{
    /// <inheritdoc/>
    public void Render(RenderPack rp)
    {
        if (!rp.Plot.RenderManager.ClearCanvasBeforeEachRender)
        {
            return;
        }

        rp.Canvas.Clear();
    }
}
