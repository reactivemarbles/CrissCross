// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering.RenderActions;

/// <summary>Renders the zoom selection rectangle when visible.</summary>
public class RenderZoomRectangle : IRenderAction
{
    /// <inheritdoc/>
    public void Render(RenderPack rp)
    {
        if (!rp.Plot.ZoomRectangle.IsVisible)
        {
            return;
        }

        rp.Plot.ZoomRectangle.Render(rp);
    }
}
