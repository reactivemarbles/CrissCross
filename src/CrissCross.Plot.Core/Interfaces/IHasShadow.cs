// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Defines shadow styling for a plot element.</summary>
public interface IHasShadow
{
    /// <summary>Gets the fill style used to render the shadow.</summary>
    FillStyle ShadowFillStyle { get; }

    /// <summary>Gets or sets the shadow offset.</summary>
    PixelOffset ShadowOffset { get; set; }

    /// <summary>Gets or sets the alignment of the shadow.</summary>
    Alignment ShadowAlignment { get; set; }

    /// <summary>Gets or sets the shadow color.</summary>
    Color ShadowColor { get; set; }
}
