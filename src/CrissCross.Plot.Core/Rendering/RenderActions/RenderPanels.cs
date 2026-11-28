// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering.RenderActions;

/// <summary>Renders the plot's panels.</summary>
public class RenderPanels : IRenderAction
{
    /// <inheritdoc/>
    public void Render(RenderPack rp)
    {
        foreach (IPanel panel in rp.Plot.Axes.GetPanels())
        {
            float size = rp.Layout.PanelSizes[panel];
            float offset = rp.Layout.PanelOffsets[panel];
            panel.Render(rp, size, offset);
        }
    }
}
