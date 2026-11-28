// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.LayoutEngines;

/// <summary>Generates layouts where the data area has fixed padding from the figure edges.</summary>
/// <param name="padding">The padding around the data area.</param>
public class FixedPadding(PixelPadding padding) : LayoutEngineBase, ILayoutEngine
{
    /// <summary>Gets the padding around the data area.</summary>
    private PixelPadding Padding { get; } = padding;

    /// <summary>Calculates the layout using the configured fixed padding.</summary>
    /// <param name="figureRect">The rectangle available to the plot.</param>
    /// <param name="plot">The plot whose panels are measured.</param>
    /// <param name="paint">The paint used to measure labels.</param>
    /// <returns>The calculated layout.</returns>
    public Layout GetLayout(PixelRect figureRect, Plot plot, Paint paint)
    {
        List<IPanel> panels = [.. plot.Axes.GetPanels()];

        // must recalculate ticks before measuring panels
        // NOTE: the actual ticks will be regenerated later, after the layout is determined
        foreach (IPanel panel in panels)
        {
            if (panel is IXAxis xAxis)
            {
                xAxis.RegenerateTicks(figureRect.Width, paint);
            }
        }

        foreach (IPanel panel in panels)
        {
            if (panel is IYAxis yAxis)
            {
                yAxis.RegenerateTicks(figureRect.Height, paint);
            }
        }

        Dictionary<IPanel, float> panelSizes = LayoutEngineBase.MeasurePanels(panels, paint);
        Dictionary<IPanel, float> panelOffsets = GetPanelOffsets(panels, panelSizes);

        PixelRect dataRect = new(
            left: figureRect.Left + Padding.Left,
            right: figureRect.Left + figureRect.Width - Padding.Right,
            bottom: figureRect.Top + figureRect.Height - Padding.Bottom,
            top: figureRect.Top + Padding.Top);

        return new(figureRect, dataRect, panelSizes, panelOffsets);
    }
}
