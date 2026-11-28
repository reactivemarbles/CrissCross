// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>
/// Logic that manages a collection of subplots with logic that can perform
/// custom actions when subplots are added or removed.
/// </summary>
public class SubplotCollection
{
    /// <summary>Gets or sets the action invoked after a plot is added.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public Action<List<Plot>> PlotAddedAction = CopyStyleOntoLastPlot;

    /// <summary>The plots managed by this collection.</summary>
    private readonly List<Plot> _plots = [];

    /// <summary>Gets the number of plots in the collection.</summary>
    public int Count => _plots.Count;

    /// <summary>Copies the figure and data background colors from the first plot onto the last plot.</summary>
    /// <param name="plots">The plots whose styles are copied.</param>
    public static void CopyStyleOntoLastPlot(List<Plot> plots)
    {
        plots[plots.Count - 1].FigureBackground.Color = plots[0].FigureBackground.Color;
        plots[plots.Count - 1].DataBackground.Color = plots[0].DataBackground.Color;
    }

    /// <summary>Adds a plot to the collection and invokes the configured callback.</summary>
    /// <param name="plot">The plot to add.</param>
    public void Add(Plot plot)
    {
        _plots.Add(plot);

        if (_plots.Count > 0)
        {
            plot.PlotControl = _plots[0].PlotControl;
        }

        PlotAddedAction.Invoke(_plots);
    }

    /// <summary>Removes the specified plot from the collection.</summary>
    /// <param name="plot">The plot to remove.</param>
    public void Remove(Plot plot) => _plots.Remove(plot);

    /// <summary>Removes the plot at the specified index.</summary>
    /// <param name="index">The zero-based index of the plot to remove.</param>
    public void RemoveAt(int index) => _plots.RemoveAt(index);

    /// <summary>Gets the plot at the specified index.</summary>
    /// <param name="index">The zero-based index of the plot to retrieve.</param>
    /// <returns>The plot at the specified index.</returns>
    public Plot GetPlot(int index) => _plots[index];

    /// <summary>Gets a copy of the plots in the collection.</summary>
    /// <returns>An array containing the plots.</returns>
    public Plot[] GetPlots() => [.. _plots];
}
