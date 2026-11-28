// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Generates ticks for financial time axes.</summary>
public interface IFinancialTickGenerator
{
    /// <summary>Gets labeled tick positions within the specified date-index range.</summary>
    /// <param name="dateTimes">The date values represented by the axis.</param>
    /// <param name="minIndexInView">The first date index in view.</param>
    /// <param name="maxIndexInView">The last date index in view.</param>
    /// <returns>Tick positions paired with their labels.</returns>
    List<(int Index, string Label)> GetTicks(DateTime[] dateTimes, int minIndexInView, int maxIndexInView);
}
