// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Implement this interface to create a custom grid.</summary>
public interface IGrid
{
    /// <summary>Gets or sets whether the grid is visible.</summary>
    bool IsVisible { get; set; }

    /// <summary>Gets or sets whether the grid is drawn beneath plottables.</summary>
    bool IsBeneathPlottables { get; set; }

    /// <summary>Gets or sets the horizontal axis displayed by the grid.</summary>
    IXAxis XAxis { get; set; }

    /// <summary>Gets or sets the vertical axis displayed by the grid.</summary>
    IYAxis YAxis { get; set; }

    /// <summary>Gets or sets the horizontal-axis grid style.</summary>
    GridStyle XAxisStyle { get; set; }

    /// <summary>Gets or sets the vertical-axis grid style.</summary>
    GridStyle YAxisStyle { get; set; }

    /// <summary>Renders the grid.</summary>
    /// <param name="rp">The render pack to draw into.</param>
    void Render(RenderPack rp);
}
