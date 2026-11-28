// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators.Financial;

/// <summary>Disables financial tick generation.</summary>
public class None : IFinancialTickGenerator
{
    /// <summary>Returns no ticks.</summary>
    /// <param name="dateTimes">The date values represented by the axis.</param>
    /// <param name="minIndexInView">The first date index in view.</param>
    /// <param name="maxIndexInView">The last date index in view.</param>
    /// <returns>An empty list of tick positions and labels.</returns>
    public List<(int Index, string Label)> GetTicks(DateTime[] dateTimes, int minIndexInView, int maxIndexInView) => [];
}
