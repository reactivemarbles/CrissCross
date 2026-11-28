// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.LayoutEngines;

/// <summary>Measures panels and adds the padding needed to fit them around the data area.</summary>
public class Automatic : LayoutEngineBase, ILayoutEngine
{
    /// <summary>Gets or sets the size used for axis panels when they have no data.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "The public field is retained to preserve the existing API for consumers.")]
    public float SizeForAxisPanelsWithoutData = 10;

    /// <summary>Calculates a layout by measuring the plot panels.</summary>
    /// <param name="figureRect">The rectangle available for the plot.</param>
    /// <param name="plot">The plot whose panels are measured.</param>
    /// <param name="paint">The paint object used to measure panel contents.</param>
    /// <returns>The calculated layout.</returns>
    public Layout GetLayout(PixelRect figureRect, Plot plot, Paint paint)
    {
        // Regenerate ticks using the figure area to estimate the space required by axis panels.
        List<IPanel> panels = [.. plot.Axes.GetPanels()];

        foreach (IPanel panel in panels)
        {
            if (panel is IXAxis xAxis)
            {
                xAxis.RegenerateTicks(figureRect.Width, paint);
            }

            if (panel is IYAxis yAxis)
            {
                yAxis.RegenerateTicks(figureRect.Height, paint);
            }
        }

        Dictionary<IPanel, float> panelSizes = LayoutEngineBase.MeasurePanels(panels, paint);
        Dictionary<IPanel, float> panelOffsets = GetPanelOffsets(panels, panelSizes);
        float left = 0;
        float right = 0;
        float bottom = 0;
        float top = 0;

        foreach (IPanel panel in panels)
        {
            (left, right, bottom, top) = panel.Edge switch
            {
                Edge.Left => (left + panelSizes[panel], right, bottom, top),
                Edge.Right => (left, right + panelSizes[panel], bottom, top),
                Edge.Bottom => (left, right, bottom + panelSizes[panel], top),
                Edge.Top => (left, right, bottom, top + panelSizes[panel]),
                _ => (left, right, bottom, top),
            };
        }

        PixelPadding paddingNeededForPanels = new(left, right, bottom, top);
        float dataRectWidth = Math.Max(0, figureRect.Width - paddingNeededForPanels.Horizontal);
        float dataRectHeight = Math.Max(0, figureRect.Height - paddingNeededForPanels.Vertical);
        PixelSize dataRectSize = new(dataRectWidth, dataRectHeight);
        Pixel dataRectLocation = new(paddingNeededForPanels.Left, paddingNeededForPanels.Top);
        PixelRect dataRect = new PixelRect(dataRectLocation, dataRectSize)
            .WithPan(figureRect.Left, figureRect.Top);

        return new(figureRect, dataRect, panelSizes, panelOffsets);
    }
}
