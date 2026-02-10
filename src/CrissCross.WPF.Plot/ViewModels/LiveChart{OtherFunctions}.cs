// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.Versioning;
using ScottPlot.Plottables;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Represents the view model for a live chart, providing data binding and interaction logic for chart visualization
/// components.
/// </summary>
/// <remarks>This class is intended for use with Windows 10 version 19041 or later. It exposes methods and
/// properties to support interactive chart features, such as detecting axis lines under the mouse cursor and managing
/// crosshair collections. Thread safety and platform compatibility should be considered when integrating with UI
/// components.</remarks>
[SupportedOSPlatform("windows")]
public partial class LiveChartViewModel : RxObject
{
    /// <summary>
    /// Gets the axis line located under the specified mouse coordinates, if any.
    /// </summary>
    /// <remarks>This method checks for both horizontal and vertical axis lines near the given coordinates.
    /// The search uses a small radius to determine proximity. If multiple axis lines overlap, the first one found is
    /// returned.</remarks>
    /// <param name="x">The x-coordinate of the mouse position, in plot units.</param>
    /// <param name="y">The y-coordinate of the mouse position, in plot units.</param>
    /// <returns>An <see cref="AxisLine"/> representing the axis line under the mouse position, or <see langword="null"/> if no
    /// axis line is found.</returns>
    public AxisLine? GetLineUnderMouse(float x, float y)
    {
        var rect = WpfPlot1vm!.Plot.GetCoordinateRect(x, y, radius: 5, XAxis1, YAxisList[0]);

        foreach (var axLine in CrosshairCollection)
        {
            var plotLine = axLine.PlotLine;
            if (plotLine?.HorizontalLine.IsUnderMouse(rect) == true)
            {
                return plotLine.HorizontalLine;
            }

            if (plotLine?.VerticalLine.IsUnderMouse(rect) == true)
            {
                return plotLine.VerticalLine;
            }
        }

        return null;
    }
}
