// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides legend text for a plottable.</summary>
public interface IHasLegendText
{
    /// <summary>Gets or sets the text displayed in the legend.</summary>
    string LegendText { get; set; }
}
