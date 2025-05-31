// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.Versioning;
using ScottPlot.Plottables;

namespace CrissCross.WPF.Plot;

/// <summary>
/// AICSLiveChart.
/// </summary>
[SupportedOSPlatform("windows10.0.19041")]
public partial class LiveChartViewModel : RxObject
{
    /// <summary>
    /// Gets the line under mouse.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns>AxisLine.</returns>
    public AxisLine? GetLineUnderMouse(float x, float y)
    {
        var rect = WpfPlot1vm!.Plot.GetCoordinateRect(x, y, radius: 5, XAxis1, YAxisList[0]);
        Trace.WriteLine($"rect : Right {rect.Right:0.00} - Left {rect.Left:0.00}");

        foreach (var axLine in CrosshairCollection)
        {
            Trace.WriteLine($"cross : X {axLine!.PlotLine!.HorizontalLine.Position:0.00} - Y {axLine!.PlotLine!.VerticalLine.Position:0.00}");
            if (axLine.PlotLine!.HorizontalLine.IsUnderMouse(rect))
            {
                return axLine.PlotLine.HorizontalLine;
            }
            else if (axLine.PlotLine!.VerticalLine.IsUnderMouse(rect))
            {
                return axLine.PlotLine.VerticalLine;
            }
        }

        return null;
    }
}
