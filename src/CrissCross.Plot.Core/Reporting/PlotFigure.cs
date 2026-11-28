// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Reporting;

/// <summary>Contains a plot and its reporting metadata.</summary>
/// <param name="Plot">The plot represented by this figure.</param>
/// <param name="Title">The title of the figure.</param>
/// <param name="Description">The description of the figure.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST1800",
    Justification = "PlotFigure remains inheritable for existing consumers that extend reporting metadata.")]
public record PlotFigure(Plot Plot, string Title, string Description);
