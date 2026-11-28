// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering.RenderActions;

/// <summary>Renders the background within the data area.</summary>
public class RenderDataBackground : IRenderAction
{
    /// <inheritdoc/>
    public void Render(RenderPack rp)
    {
        rp.CanvasState.Clip(rp.DataRect);
        rp.Plot.DataBackground?.Render(rp, rp.DataRect);
        rp.CanvasState.DisableClipping();
    }
}
