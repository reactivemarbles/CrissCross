// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides extension methods for axis limit managers.</summary>
public static class IAxisLimitManagerExtensions
{
    /// <summary>Creates axis limits recommended by the manager for the current view and data.</summary>
    /// <param name="manager">The manager that calculates the recommended ranges.</param>
    /// <returns>The recommended horizontal and vertical axis limits.</returns>
    extension(IAxisLimitManager manager)
    {
        /// <summary>Returns recommended axis limits given the current view and size of the data.</summary>
        /// <param name="viewLimits">The current axis limits.</param>
        /// <param name="dataLimits">The axis limits occupied by the data.</param>
        /// <returns>The recommended axis limits.</returns>
        public AxisLimits GetAxisLimits(AxisLimits viewLimits, AxisLimits dataLimits) =>
            new(manager.GetRangeX(viewLimits.XRange, dataLimits.XRange), manager.GetRangeY(viewLimits.YRange, dataLimits.YRange));
    }
}
