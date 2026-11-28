// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity.UserActionResponses;

/// <summary>Autoscale the plot when the specified key is pressed.</summary>
/// <param name="key">The key that triggers autoscaling.</param>
public class KeyboardAutoscale(Key key) : KeyPressResponse(key, AutoScale)
{
    /// <summary>Autoscale the plot beneath the supplied pixel.</summary>
    /// <param name="plotControl">The control containing the plot.</param>
    /// <param name="pixel">The pixel used to locate the plot.</param>
    public static void AutoScale(IPlotControl plotControl, Pixel pixel)
    {
        Plot? plot = plotControl.GetPlotAtPixel(pixel);
        if (plot is null)
        {
            return;
        }

        plot.Axes.AutoScale();
    }
}
