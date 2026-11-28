// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering.RenderActions;

/// <summary>Updates axis limits for plottables that manage their own limits.</summary>
public class ExecutePlottableAxisManagers : IRenderAction
{
    /// <inheritdoc/>
    public void Render(RenderPack rp)
    {
        List<IManagesAxisLimits> axisLimitManagers = [];
        foreach (IPlottable plottable in rp.Plot.PlottableList)
        {
            if (plottable is IManagesAxisLimits axisLimitManager && axisLimitManager.ManageAxisLimits)
            {
                axisLimitManagers.Add(axisLimitManager);
            }
        }

        foreach (IManagesAxisLimits axisLimitManager in axisLimitManagers)
        {
            axisLimitManager.UpdateAxisLimits(rp.Plot);
        }
    }
}
