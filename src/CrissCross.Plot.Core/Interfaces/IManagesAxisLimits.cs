// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>
/// This interface is applied to plottables which modify axis limits at render time.
/// The update method is called at render time before the ticks are calculated.
/// </summary>
public interface IManagesAxisLimits
{
    /// <summary>Gets or sets whether this object updates axis limits during rendering.</summary>
    bool ManageAxisLimits { get; set; }

    /// <summary>Updates the plot's axis limits.</summary>
    /// <param name="plot">The plot whose axis limits are updated.</param>
    void UpdateAxisLimits(Plot plot);
}
