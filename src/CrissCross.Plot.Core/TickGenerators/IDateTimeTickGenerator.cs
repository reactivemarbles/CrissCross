// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators;

/// <summary>Generates ticks and converts dates to their coordinate-space values.</summary>
public interface IDateTimeTickGenerator : ITickGenerator
{
    /// <summary>Converts dates to coordinate-space values.</summary>
    /// <param name="dates">The dates to convert.</param>
    /// <returns>The coordinate-space values corresponding to the dates.</returns>
    IEnumerable<double> ConvertToCoordinateSpace(IEnumerable<DateTime> dates);
}
