// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering.RenderActions;

/// <summary>Synchronizes the first OpenGL plottable after a render.</summary>
public class SyncGLPlottables : IRenderAction
{
    /// <summary>Finishes the first OpenGL plottable in the plot.</summary>
    /// <param name="rp">The render pack containing the plot.</param>
    public void Render(RenderPack rp)
    {
        foreach (var plottable in rp.Plot.PlottableList)
        {
            if (plottable is IPlottableGL graphicsPlottable)
            {
                graphicsPlottable.GLFinish();
                break;
            }
        }
    }
}
