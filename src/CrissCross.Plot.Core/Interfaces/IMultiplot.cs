// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a collection of plots rendered together.</summary>
public interface IMultiplot
{
    /// <summary>Gets the collection of subplots.</summary>
    SubplotCollection Subplots { get; }

    /// <summary>Gets or sets the layout used to place subplots within the figure.</summary>
    IMultiplotLayout Layout { get; set; }

    /// <summary>Gets the state of the most recent render for plot hit-testing.</summary>
    MultiplotLayoutSnapshot LastRender { get; }

    /// <summary>Gets the manager for linking subplot axis limits.</summary>
    MultiplotSharedAxisManager SharedAxes { get; }

    /// <summary>Gets actions invoked before each render to coordinate subplot state.</summary>
    List<IMultiplotPreRenderAction> PreRenderActions { get; }

    /// <summary>Renders the multiplot to a canvas using the specified figure rectangle.</summary>
    /// <param name="canvas">The canvas to draw into.</param>
    /// <param name="figureRect">The rectangle containing the multiplot figure.</param>
    void Render(SKCanvas canvas, PixelRect figureRect);
}
