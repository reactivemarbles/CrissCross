// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Contains settings and logic for how to draw an arrow once the base and tip pixels have been determined.</summary>
public interface IArrowShape
{
    /// <summary>Renders an arrow using the supplied line and style.</summary>
    /// <param name="rp">The render pack containing the canvas and paint.</param>
    /// <param name="pxLine">The line representing the arrow body.</param>
    /// <param name="arrowStyle">The style used to render the arrow.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp.Naming",
        "SST1305",
        Justification = "The parameter name is established in the public interface and used by named arguments.")]
    void Render(RenderPack rp, PixelLine pxLine, ArrowStyle arrowStyle);
}
