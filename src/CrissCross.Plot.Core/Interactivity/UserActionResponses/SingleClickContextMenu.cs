// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity.UserActionResponses;

/// <summary>Launches the context menu in response to a single click.</summary>
/// <param name="button">The mouse button that triggers the context menu.</param>
public class SingleClickContextMenu(MouseButton button) : SingleClickResponse(button, LaunchContextMenu)
{
    /// <summary>Launches the context menu for the plot beneath the clicked pixel.</summary>
    /// <param name="plotControl">The control containing the plot.</param>
    /// <param name="pixel">The clicked pixel position.</param>
    public static void LaunchContextMenu(IPlotControl plotControl, Pixel pixel)
    {
        Plot? plot = plotControl.GetPlotAtPixel(pixel);
        if (plot is null)
        {
            return;
        }

        plot.PlotControl?.ShowContextMenu(pixel);
    }
}
