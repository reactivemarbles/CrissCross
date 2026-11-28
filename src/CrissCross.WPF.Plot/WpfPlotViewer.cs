// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Displays an existing plot in a modal WPF window.</summary>
public static class WpfPlotViewer
{
    /// <summary>Opens a modal window for an existing plot.</summary>
    /// <param name="plot">The plot to display.</param>
    /// <param name="title">The window title.</param>
    /// <param name="width">The window width.</param>
    /// <param name="height">The window height.</param>
    [SuppressMessage("Design", "SST2309:Avoid optional parameters on public APIs", Justification = "Retain the existing public signature and its optional arguments.")]
    public static void Launch(CrissCross.Plot.Plot plot, string title = "", int width = 600, int height = 400)
    {
        ThrowHelper.ThrowIfNull(plot, nameof(plot));
        IPlotControl? originalControl = plot.PlotControl;
        WpfPlot wpfPlot = new();
        try
        {
            wpfPlot.Reset(plot);
            System.Windows.Window win = new() { WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen, Width = width, Height = height, Title = title, Content = wpfPlot };
            _ = win.ShowDialog();
        }
        finally
        {
            plot.PlotControl = originalControl;
        }
    }
}
