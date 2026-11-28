// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.LayoutEngines;

/// <summary>Provides shared measurement and offset calculations for layout engines.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST1432",
    Justification = "This public base type is inherited by public layout-engine implementations.")]
public class LayoutEngineBase
{
    /// <summary>Measures every panel and clamps its measured size to the panel limits.</summary>
    /// <param name="panels">The panels to measure.</param>
    /// <param name="paint">The paint object used for measurement.</param>
    /// <returns>The measured size of each panel.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1473",
        Justification = "Exact equality identifies fixed-size panels; a tolerance changes distinct bounds.")]
    internal static Dictionary<IPanel, float> MeasurePanels(IEnumerable<IPanel> panels, Paint paint)
    {
        Dictionary<IPanel, float> measuredPanels = new();

        foreach (IPanel panel in panels)
        {
            measuredPanels[panel] = panel.MinimumSize == panel.MaximumSize
                ? panel.MinimumSize
                : NumericConversion.Clamp(panel.Measure(paint), panel.MinimumSize, panel.MaximumSize);
        }

        return measuredPanels;
    }

    /// <summary>Calculates sequential offsets for panels in their original order.</summary>
    /// <param name="panels">The panels whose offsets are calculated.</param>
    /// <param name="sizes">The size of each panel.</param>
    /// <param name="offsets">The dictionary to populate with panel offsets.</param>
    internal static void CalculateOffsets(IEnumerable<IPanel> panels, Dictionary<IPanel, float> sizes, Dictionary<IPanel, float> offsets)
    {
        float offset = 0;
        foreach (IPanel panel in panels)
        {
            offsets[panel] = offset;
            offset += sizes[panel];
        }
    }

    /// <summary>Gets panel offsets grouped by their plot edge.</summary>
    /// <param name="panels">The panels whose offsets are calculated.</param>
    /// <param name="panelSizes">The measured size of each panel.</param>
    /// <returns>The offset of every panel.</returns>
    internal static Dictionary<IPanel, float> GetPanelOffsets(IEnumerable<IPanel> panels, Dictionary<IPanel, float> panelSizes)
    {
        List<IPanel> leftPanels = [];
        List<IPanel> rightPanels = [];
        List<IPanel> bottomPanels = [];
        List<IPanel> topPanels = [];
        foreach (IPanel panel in panels)
        {
            List<IPanel>? edgePanels = panel.Edge switch
            {
                Edge.Left => leftPanels,
                Edge.Right => rightPanels,
                Edge.Bottom => bottomPanels,
                Edge.Top => topPanels,
                _ => null,
            };
            edgePanels?.Add(panel);
        }

        Dictionary<IPanel, float> panelOffsets = new();
        CalculateOffsets(leftPanels, panelSizes, panelOffsets);
        CalculateOffsets(rightPanels, panelSizes, panelOffsets);
        CalculateOffsets(bottomPanels, panelSizes, panelOffsets);
        CalculateOffsets(topPanels, panelSizes, panelOffsets);
        return panelOffsets;
    }
}
