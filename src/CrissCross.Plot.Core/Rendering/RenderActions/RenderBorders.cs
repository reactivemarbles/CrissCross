// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering.RenderActions;

/// <summary>Renders the figure and data borders.</summary>
public class RenderBorders : IRenderAction
{
    /// <inheritdoc/>
    public void Render(RenderPack rp)
    {
        rp.Plot.FigureBorder.Render(rp.Canvas, rp.FigureRect, rp.Paint, contract: true);
        rp.Plot.DataBorder.Render(rp.Canvas, rp.DataRect, rp.Paint);
    }
}
