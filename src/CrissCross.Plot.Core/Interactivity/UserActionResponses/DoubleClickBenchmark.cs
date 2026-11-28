// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity.UserActionResponses;

/// <summary>Toggles benchmark visibility in response to a double click.</summary>
/// <param name="button">The mouse button that triggers the action.</param>
public class DoubleClickBenchmark(MouseButton button) : DoubleClickResponse(button, ToggleBenchmarkVisibility)
{
    /// <summary>Toggles benchmark visibility for the plot beneath the clicked pixel.</summary>
    /// <param name="plotControl">The control containing the plot.</param>
    /// <param name="pixel">The clicked pixel position.</param>
    public static void ToggleBenchmarkVisibility(IPlotControl plotControl, Pixel pixel)
    {
        Plot? plot = plotControl.GetPlotAtPixel(pixel);
        if (plot is null)
        {
            return;
        }

        plot.Benchmark.IsVisible = !plot.Benchmark.IsVisible;
    }
}
