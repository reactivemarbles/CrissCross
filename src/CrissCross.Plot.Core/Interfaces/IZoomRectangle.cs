// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a zoom rectangle drawn over a plot.</summary>
public interface IZoomRectangle
{
    /// <summary>Gets or sets whether the rectangle spans the full vertical axis.</summary>
    bool VerticalSpan { get; set; }

    /// <summary>Gets or sets whether the rectangle spans the full horizontal axis.</summary>
    bool HorizontalSpan { get; set; }

    /// <summary>Gets or sets whether the zoom rectangle is visible.</summary>
    bool IsVisible { get; set; }

    /// <summary>Gets or sets the pixel where the drag began.</summary>
    Pixel MouseDown { get; set; }

    /// <summary>Gets or sets the current pixel where the drag ends.</summary>
    Pixel MouseUp { get; set; }

    /// <summary>Applies the rectangle to a horizontal axis.</summary>
    /// <param name="xAxes">The horizontal axis to update.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Parameter name is retained for existing named-argument callers.")]
    void Apply(IXAxis xAxes);

    /// <summary>Applies the rectangle to a vertical axis.</summary>
    /// <param name="yAxes">The vertical axis to update.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Parameter name is retained for existing named-argument callers.")]
    void Apply(IYAxis yAxes);

    /// <summary>Renders the zoom rectangle.</summary>
    /// <param name="rp">The render pack to draw into.</param>
    void Render(RenderPack rp);
}
