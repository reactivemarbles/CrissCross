// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering.RenderActions;

/// <summary>Renders visible plottables that draw after the regular plot content.</summary>
public class RenderPlottablesLast : IRenderAction
{
    /// <inheritdoc/>
    public void Render(RenderPack rp)
    {
        List<IRenderLast> visibleLastPlottables = [];
        foreach (IPlottable plottable in rp.Plot.PlottableList)
        {
            if (plottable.IsVisible && plottable is IRenderLast renderLast)
            {
                visibleLastPlottables.Add(renderLast);
            }
        }

        foreach (IRenderLast plottable in visibleLastPlottables)
        {
            plottable.RenderLast(rp);
        }
    }
}
