// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering.RenderActions;

/// <summary>Restores state for the first OpenGL plottable in the plot.</summary>
public class RestoreGLState : IRenderAction
{
    /// <inheritdoc/>
    public void Render(RenderPack rp)
    {
        foreach (IPlottable plottable in rp.Plot.PlottableList)
        {
            if (plottable is IPlottableGL glPlottable)
            {
                glPlottable.RestoreGLState();
                break;
            }
        }
    }
}
