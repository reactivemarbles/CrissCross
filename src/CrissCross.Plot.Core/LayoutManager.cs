// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.LayoutEngines;

namespace CrissCross.Plot;

/// <summary>A collection of methods for making common adjustments to plot layouts.</summary>
/// <param name="plot">The plot whose layout is managed.</param>
public class LayoutManager(Plot plot)
{
    /// <summary>The plot whose layout is managed.</summary>
    private readonly Plot _plot = plot;

    /// <summary>Gets or sets the layout engine used to position plot elements.</summary>
    public ILayoutEngine LayoutEngine { get; set; } = new Automatic();

    /// <summary>Automatically resize the layout on each render to achieve the best fit.</summary>
    public void Default() => LayoutEngine = new Automatic();

    /// <summary>Applies a fixed layout using the given rectangle to define the data area.</summary>
    /// <param name="dataRect">The rectangle used for the data area.</param>
    public void Fixed(PixelRect dataRect) => LayoutEngine = new FixedDataArea(dataRect);

    /// <summary>Applies a fixed layout using the given padding to define space around the data area.</summary>
    /// <param name="padding">The padding between the data area and the figure edges.</param>
    public void Fixed(PixelPadding padding) => LayoutEngine = new FixedPadding(padding);

    /// <summary>
    /// Helper method to set visibility of all axes and title panels.
    /// Hiding all panels allows the data area to the extend to the edge of the figure.
    /// This method hides the title, but call <c>Plot.Title()</c> to re-enable it.
    /// </summary>
    /// <param name="hideAllPanels">Whether to hide every axis and title panel.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional default is part of the existing public API.")]
    public void Frameless(bool hideAllPanels = true) => _plot.Axes.Frameless(hideAllPanels);
}
