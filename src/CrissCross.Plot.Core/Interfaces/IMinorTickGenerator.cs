// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Generates minor tick positions between major ticks.</summary>
public interface IMinorTickGenerator
{
    /// <summary>Gets the minor ticks within the visible range.</summary>
    /// <param name="majorTicks">The positions of the major ticks.</param>
    /// <param name="visibleRange">The visible coordinate range.</param>
    /// <returns>The positions of the minor ticks.</returns>
    IEnumerable<double> GetMinorTicks(double[] majorTicks, CoordinateRange visibleRange);
}
