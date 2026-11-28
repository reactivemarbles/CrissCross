// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering.RenderActions;

/// <summary>Regenerates ticks on the distinct axes used by a plot's plottables.</summary>
public class RegenerateTicks : IRenderAction
{
    /// <summary>Regenerates horizontal and vertical ticks for the plot.</summary>
    /// <param name="rp">The render pack containing the plot and its data-area dimensions.</param>
    public void Render(RenderPack rp)
    {
        HashSet<IXAxis> regeneratedXAxes = [];
        foreach (var plottable in rp.Plot.PlottableList)
        {
            IXAxis horizontalAxis = plottable.Axes.XAxis;
            if (regeneratedXAxes.Add(horizontalAxis))
            {
                horizontalAxis.RegenerateTicks(rp.DataRect.Width, rp.Paint);
            }
        }

        HashSet<IYAxis> regeneratedYAxes = [];
        foreach (var plottable in rp.Plot.PlottableList)
        {
            IYAxis verticalAxis = plottable.Axes.YAxis;
            if (regeneratedYAxes.Add(verticalAxis))
            {
                verticalAxis.RegenerateTicks(rp.DataRect.Height, rp.Paint);
            }
        }
    }
}
