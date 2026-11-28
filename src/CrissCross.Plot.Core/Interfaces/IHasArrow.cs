// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides configurable arrow styling for a plottable.</summary>
public interface IHasArrow
{
    /// <summary>Gets or sets the arrow rendering style.</summary>
    ArrowStyle ArrowStyle { get; set; }

    /// <summary>Gets or sets the arrow line color.</summary>
    Color ArrowLineColor { get; set; }

    /// <summary>Gets or sets the arrow line width.</summary>
    float ArrowLineWidth { get; set; }

    /// <summary>Gets or sets the arrow fill color.</summary>
    Color ArrowFillColor { get; set; }

    /// <summary>Gets or sets the minimum arrow length.</summary>
    float ArrowMinimumLength { get; set; }

    /// <summary>Gets or sets the maximum arrow length.</summary>
    float ArrowMaximumLength { get; set; }

    /// <summary>Gets or sets the arrow offset.</summary>
    float ArrowOffset { get; set; }

    /// <summary>Gets or sets the arrow anchor.</summary>
    ArrowAnchor ArrowAnchor { get; set; }

    /// <summary>Gets or sets the arrow width.</summary>
    float ArrowWidth { get; set; }

    /// <summary>Gets or sets the arrowhead axis length.</summary>
    float ArrowheadAxisLength { get; set; }

    /// <summary>Gets or sets the arrowhead length.</summary>
    float ArrowheadLength { get; set; }

    /// <summary>Gets or sets the arrowhead width.</summary>
    float ArrowheadWidth { get; set; }
}
