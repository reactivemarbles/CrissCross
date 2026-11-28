// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if PLOT_MAUI && REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Maui.Plot;
#elif PLOT_MAUI
namespace CrissCross.Maui.Plot;
#elif PLOT_AVALONIA && REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.Plot;
#elif PLOT_AVALONIA
namespace CrissCross.Avalonia.Plot;
#elif REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Coordinates deferred plot mutations with rendering.</summary>
internal static class PlotSynchronizationExtensions
{
    /// <summary>Extends scheduled notification streams with render synchronization.</summary>
    /// <typeparam name="T">The notification type.</typeparam>
    /// <param name="source">The scheduled notifications.</param>
    extension<T>(IObservable<T> source)
    {
        /// <summary>Runs each notification under the destination plot's render lock.</summary>
        /// <param name="plot">The destination plot.</param>
        /// <param name="onNext">The complete plot mutation.</param>
        /// <returns>The owned subscription.</returns>
        internal IDisposable SubscribeSynchronized(CrissCross.Plot.Plot plot, Action<T> onNext)
        {
            var sync = plot.Sync;
            return source.Subscribe(value =>
            {
                lock (sync)
                {
                    onNext(value);
                }
            });
        }
    }

    /// <summary>Extends series controls with plottable lifecycle operations.</summary>
    /// <param name="ui">The series control.</param>
    extension(IPlottableUI ui)
    {
        /// <summary>Gets the current data plottable owned by a series control.</summary>
        /// <returns>The data plottable, or null before one has been created.</returns>
        internal CrissCross.Plot.IPlottable? GetPlottable() => ui switch
        {
            SignalUI signal => signal.PlotLine,
            ScatterUI scatter => scatter.PlotLine,
            DataLoggerUI logger => logger.PlotLine,
            StreamerUI streamer => streamer.PlotLine,
            SignalXY_UI signalXy => signalXy.PlotLine,
            _ => null,
        };

        /// <summary>Removes a series and its cursor overlays while the caller owns the render lock.</summary>
        /// <param name="plot">The destination plot.</param>
        internal void RemoveFromPlot(CrissCross.Plot.Plot plot)
        {
            if (ui.GetPlottable() is { } plottable)
            {
                plot.Remove(plottable);
            }

            plot.Remove(ui.ChartSettings.Crosshair);
            plot.Remove(ui.ChartSettings.Marker);
            plot.Remove(ui.ChartSettings.MarkerText);
        }
    }
}
