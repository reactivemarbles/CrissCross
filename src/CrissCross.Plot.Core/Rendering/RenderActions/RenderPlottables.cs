// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering.RenderActions;

/// <summary>Renders the visible plottables in the data area.</summary>
public class RenderPlottables : IRenderAction
{
    /// <inheritdoc/>
    public void Render(RenderPack rp)
    {
        List<IPlottable> visiblePlottables = [];
        foreach (IPlottable plottable in rp.Plot.PlottableList)
        {
            if (plottable.IsVisible)
            {
                visiblePlottables.Add(plottable);
            }
        }

        foreach (IPlottable plottable in visiblePlottables)
        {
            plottable.Axes.DataRect = rp.DataRect;

            rp.CanvasState.Save();

            if (plottable is IPlottableGL plottableGL)
            {
                plottableGL.Render(rp);
            }
            else
            {
                rp.CanvasState.Clip(rp.DataRect);
                plottable.Render(rp);
            }

            rp.CanvasState.DisableClipping();
        }
    }
}
