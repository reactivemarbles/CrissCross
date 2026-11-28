// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering.RenderActions;

/// <summary>Runs the continuous autoscale action when enabled.</summary>
public class ContinuouslyAutoscale : IRenderAction
{
    /// <inheritdoc/>
    public void Render(RenderPack rp)
    {
        if (!rp.Plot.Axes.ContinuouslyAutoscale)
        {
            return;
        }

        rp.Plot.Axes.ContinuousAutoscaleAction.Invoke(rp);
    }
}
