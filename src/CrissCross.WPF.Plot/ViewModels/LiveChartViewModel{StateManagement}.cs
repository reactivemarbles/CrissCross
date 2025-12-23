// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using CP.Reactive;

namespace CrissCross.WPF.Plot;

/// <summary>
/// LiveChartViewModel - Chart Objects Access.
/// Provides external access to chart line settings.
/// </summary>
[SupportedOSPlatform("windows10.0.19041")]
public partial class LiveChartViewModel
{
    /// <summary>
    /// Gets the collection of chart objects representing line settings.
    /// This collection is automatically populated when plot lines are created.
    /// </summary>
    /// <remarks>
    /// <para><b>IMPORTANT NOTES:</b></para>
    /// <list type="bullet">
    /// <item><description>Items are LIVE references - changes immediately affect the chart</description></item>
    /// <item><description>Collection is CLEARED and REPOPULATED on reinitialization</description></item>
    /// <item><description>Do not hold long-term references to items - they may be disposed</description></item>
    /// <item><description>ItemName may initially be "---" until first observable emission</description></item>
    /// <item><description>Access must occur on the UI thread (not thread-safe)</description></item>
    /// <item><description>Items are NOT disposed when collection is cleared - they remain owned by PlotLinesCollectionUI</description></item>
    /// </list>
    /// <para><b>Lifecycle:</b></para>
    /// <list type="bullet">
    /// <item><description>Updated via UpdateChartObjectsCollection() after InitializeGenericPlotLines</description></item>
    /// <item><description>Items disposed only when ClearContent() is called on parent ViewModel</description></item>
    /// <item><description>Collection cleared but items remain alive during reinitialization</description></item>
    /// </list>
    /// <para><b>Usage Pattern:</b></para>
    /// <code>
    /// // Save settings before reinit
    /// var saved = Chart1.ViewModel.ChartObjectsCollection.Items
    ///     .Select(x => new { x.ItemName, x.LineWidth, x.Color }).ToList();
    ///
    /// // Reinitialize
    /// Chart1.SignalObservablesWithPoints = newData;
    ///
    /// // Restore settings
    /// foreach (var obj in Chart1.ViewModel.ChartObjectsCollection.Items)
    /// {
    ///     var s = saved.FirstOrDefault(x => x.ItemName == obj.ItemName);
    ///     if (s != null) {
    ///         obj.LineWidth = s.LineWidth;
    ///         obj.Color = s.Color;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Access settings
    /// foreach (var obj in Chart1.ViewModel.ChartObjectsCollection.Items)
    /// {
    ///     Console.WriteLine($"{obj.ItemName}: {obj.Color}");
    ///     obj.LineWidth = 5.0; // Modify settings
    /// }
    /// </code>
    /// </example>
    public ReactiveList<ChartObjects> ChartObjectsCollection { get; private set; } = [];

    /// <summary>
    /// Updates the ChartObjectsCollection from current plot lines.
    /// Called automatically after plot initialization.
    /// </summary>
    /// <remarks>
    /// Uses a snapshot of PlotLinesCollectionUI to avoid concurrent modification exceptions.
    /// Items are not disposed when cleared as they remain owned by PlotLinesCollectionUI.
    /// </remarks>
    private void UpdateChartObjectsCollection()
    {
        // Clear existing collection (items remain alive, owned by PlotLinesCollectionUI)
        ChartObjectsCollection.Clear();

        // Populate from current plot lines (snapshot to avoid concurrent modification)
        if (PlotLinesCollectionUI != null)
        {
            foreach (var plotLine in PlotLinesCollectionUI.Items.ToList())
            {
                if (plotLine?.ChartSettings != null)
                {
                    ChartObjectsCollection.Add(plotLine.ChartSettings);
                }
            }
        }
    }
}
