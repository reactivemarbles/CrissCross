// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.LayoutEngines;

/// <summary>Generates a layout that matches the data area of another plot.</summary>
/// <param name="referencePlot">The plot whose data-area rectangle is matched.</param>
public class MatchedDataRect(Plot referencePlot) : LayoutEngineBase, ILayoutEngine
{
    /// <summary>Gets the plot whose data-area rectangle is matched.</summary>
    private Plot ReferencePlot { get; } = referencePlot;

    /// <summary>Calculates a layout using the reference plot's data-area rectangle.</summary>
    /// <param name="figureRect">The rectangle available for the full figure.</param>
    /// <param name="plot">The plot whose panels are measured.</param>
    /// <param name="paint">The paint used to measure the panels.</param>
    /// <returns>The calculated layout.</returns>
    public Layout GetLayout(PixelRect figureRect, Plot plot, Paint paint)
    {
        IEnumerable<IPanel> panels = plot.Axes.GetPanels();
        Dictionary<IPanel, float> panelSizes = LayoutEngineBase.MeasurePanels(panels, paint);
        Dictionary<IPanel, float> panelOffsets = GetPanelOffsets(panels, panelSizes);

        PixelRect dataRect = ReferencePlot.RenderManager.LastRender.DataRect;
        return new(figureRect, dataRect, panelSizes, panelOffsets);
    }
}
