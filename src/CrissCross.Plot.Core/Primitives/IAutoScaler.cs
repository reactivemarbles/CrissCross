// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Contains logic for determining new axis limits when Autoscale() is called.</summary>
public interface IAutoScaler
{
    // TODO: axis-specific autoscaling can be moved out of the control class and placed somewhere in here
    // TODO: GetRecommendedAxisLimits() should return a dictionary of limits by axis,
    // then both functions can be collapsed into one.
    /// <summary>Gets or sets whether the X axis is inverted.</summary>
    bool InvertedX { get; set; }

    /// <summary>Gets or sets whether the Y axis is inverted.</summary>
    bool InvertedY { get; set; }

    // TODO: Add this so auto-scalers can provide scaling information without requiring plottable collections
    /// <summary>Returns the recommended axis limits for plottables that use the specified axes.</summary>
    /// <param name="plot">The plot containing the plottables.</param>
    /// <param name="xAxis">The X axis whose limits are calculated.</param>
    /// <param name="yAxis">The Y axis whose limits are calculated.</param>
    /// <returns>The recommended limits for the specified axes.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "The established public parameter names support named-argument callers.")]
    AxisLimits GetAxisLimits(Plot plot, IXAxis xAxis, IYAxis yAxis);

    /// <summary>Autoscales every unset axis used by the specified plottables.</summary>
    /// <param name="plottables">The plottables whose axes are autoscaled.</param>
    void AutoScaleAll(IEnumerable<IPlottable> plottables);
}
