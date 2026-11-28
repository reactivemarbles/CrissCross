// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.LayoutEngines;

/// <summary>Generates a layout using a fixed rectangle for the data area.</summary>
/// <param name="dataRect">The fixed rectangle used for the data area.</param>
public class FixedDataArea(PixelRect dataRect) : LayoutEngineBase, ILayoutEngine
{
    /// <summary>Gets the fixed data-area rectangle.</summary>
    private PixelRect DataRect { get; } = dataRect;

    /// <summary>Calculates a layout using the configured data-area rectangle.</summary>
    /// <param name="figureRect">The rectangle available for the full figure.</param>
    /// <param name="plot">The plot whose panels are measured.</param>
    /// <param name="paint">The paint used to measure the panels.</param>
    /// <returns>The calculated layout.</returns>
    public Layout GetLayout(PixelRect figureRect, Plot plot, Paint paint)
    {
        IEnumerable<IPanel> panels = plot.Axes.GetPanels();
        Dictionary<IPanel, float> panelSizes = LayoutEngineBase.MeasurePanels(panels, paint);
        Dictionary<IPanel, float> panelOffsets = GetPanelOffsets(panels, panelSizes);
        return new(figureRect, DataRect, panelSizes, panelOffsets);
    }
}
