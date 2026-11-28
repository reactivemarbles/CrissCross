// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Reporting;

/// <summary>Collects plots with optional titles and descriptions for reporting.</summary>
public class PlotCollection
{
    /// <summary>Gets the figures in this collection.</summary>
    public List<PlotFigure> Figures { get; } = [];

    /// <summary>Adds a plot and its optional reporting metadata.</summary>
    /// <param name="plot">The plot to add.</param>
    /// <param name="title">The optional title for the plot.</param>
    /// <param name="description">The optional description for the plot.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional metadata defaults are part of the existing public API.")]
    public void Add(Plot plot, string? title = null, string? description = null) => Figures.Add(new(plot, title ?? string.Empty, description ?? string.Empty));
}
