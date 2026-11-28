// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides configurable marker styling for a plottable.</summary>
public interface IHasMarker
{
    /// <summary>Gets or sets the complete marker style.</summary>
    MarkerStyle MarkerStyle { get; set; }

    /// <summary>Gets or sets the marker shape.</summary>
    MarkerShape MarkerShape { get; set; }

    /// <summary>Gets or sets the marker size in pixels.</summary>
    float MarkerSize { get; set; }

    /// <summary>Gets or sets the marker fill color.</summary>
    Color MarkerFillColor { get; set; }

    /// <summary>Gets or sets the marker line color.</summary>
    Color MarkerLineColor { get; set; }

    /// <summary>Gets or sets the marker line width.</summary>
    float MarkerLineWidth { get; set; }

    /// <summary>Gets or sets the marker color.</summary>
    Color MarkerColor { get; set; }
}
