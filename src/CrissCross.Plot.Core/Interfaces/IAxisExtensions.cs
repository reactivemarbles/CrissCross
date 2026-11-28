// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;

namespace CrissCross.Plot;

/// <summary>Provides extension methods for <see cref="IAxis"/>.</summary>
public static class IAxisExtensions
{
    /// <summary>Extension members for <c>IAxis</c>.</summary>
    /// <param name="axis">The axis to extend.</param>
    extension(IAxis axis)
    {
        /// <summary>Gets the immutable coordinate range of the axis.</summary>
        /// <returns>The minimum and maximum axis coordinates.</returns>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1024:UsePropertiesWhereAppropriate",
            Justification = "GetRange is an established public extension method; changing it to a property would break the API.")]
        public CoordinateRange GetRange() => new(axis.Min, axis.Max);

        /// <summary>Gets whether the axis coordinates are inverted.</summary>
        /// <returns><see langword="true"/> when the minimum exceeds the maximum.</returns>
        public bool IsInverted() => axis.Min > axis.Max;

        /// <summary>Removes the current tick generator from the axis.</summary>
        public void RemoveTickGenerator() => axis.TickGenerator = new TickGenerators.EmptyTickGenerator();

        /// <summary>
        /// Sets the axis size to zero and disables tick generation.
        /// This method allows the data area to rest against the edge of the figure
        /// (e.g., when collapsing space between stacked plots in a multiplot).
        /// </summary>
        public void Collapse()
        {
            axis.MaximumSize = 0;
            axis.RemoveTickGenerator();
        }
    }
}
