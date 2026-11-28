// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Renders and manages multiple plots in a shared figure.</summary>
public class Multiplot : IMultiplot
{
    /// <summary>Initializes a new instance of the <see cref="Multiplot"/> class with a new plot.</summary>
    public Multiplot()
        : this(new Plot())
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Multiplot"/> class containing the specified plot.</summary>
    /// <param name="plot">The plot to add.</param>
    public Multiplot(Plot plot)
    {
        this.AddPlot(plot);
    }

    /// <summary>Gets the manager for axes shared between subplots.</summary>
    public MultiplotSharedAxisManager SharedAxes { get; } = new();

    /// <summary>Gets the collection of plots in the multiplot.</summary>
    public SubplotCollection Subplots { get; } = new();

    /// <summary>Gets the snapshot of the most recently rendered layout.</summary>
    public MultiplotLayoutSnapshot LastRender { get; } = new();

    /// <summary>Gets or sets the actions invoked before rendering each subplot.</summary>
    public List<IMultiplotPreRenderAction> PreRenderActions { get; set; } = [];

    /// <summary>Gets or sets the layout used to arrange the subplots.</summary>
    public IMultiplotLayout Layout { get; set; } = new MultiplotLayouts.Rows();

    /// <summary>Renders all subplots onto the specified canvas.</summary>
    /// <param name="canvas">The canvas to render onto.</param>
    /// <param name="figureRect">The rectangle available for the multiplot.</param>
    public void Render(SKCanvas canvas, PixelRect figureRect)
    {
        SharedAxes.UpdateSharedPlotAxisLimits();

        foreach (IMultiplotPreRenderAction preRenderAction in PreRenderActions)
        {
            preRenderAction.Invoke();
        }

        canvas.Clear();
        PixelRect[] subplotRectangles = Layout.GetSubplotRectangles(Subplots, figureRect);
        for (int i = 0; i < Subplots.Count; i++)
        {
            Plot plot = Subplots.GetPlot(i);
            plot.RenderManager.ClearCanvasBeforeEachRender = false;
            plot.Render(canvas, subplotRectangles[i]);
            LastRender.Remember(plot, subplotRectangles[i]);
        }
    }
}
