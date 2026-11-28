// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering.RenderActions;

/// <summary>Renders grids positioned beneath plottables.</summary>
public class RenderGridsBelowPlottables : IRenderAction
{
    /// <inheritdoc/>
    public void Render(RenderPack rp)
    {
        foreach (IGrid grid in rp.Plot.Axes.AllGrids)
        {
            if (grid.IsBeneathPlottables)
            {
                grid.Render(rp);
            }
        }
    }
}
