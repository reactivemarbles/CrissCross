// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using Color = ScottPlot.Color;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Provides a user interface component for visualizing and interacting with data streams using a plot, supporting
/// real-time updates and scaling options. Nice for continuous live data.
/// </summary>
/// <remarks>This class is intended for use on Windows platforms and integrates with reactive observables to
/// display streaming data. It supports automatic and manual scaling of the plot, and can be configured to display a
/// fixed number of data points. The UI is designed to work with ScottPlot's WpfPlot and DataLogger for efficient data
/// visualization. Thread safety is managed internally for observable subscriptions and UI updates. Dispose of instances
/// to release resources when no longer needed.</remarks>
[SupportedOSPlatform("windows")]
public partial class DataLoggerUI : RxObject, IPlottableUI
{
    /// <summary>The maximum number of points retained by the data logger.</summary>
    private const int MaximumLoggedPointCount = 100_000_000;

    /// <summary>Stores the value buffer value.</summary>
    private double[]? _valueBuffer;

    /// <summary>Stores the chart settings value.</summary>
    [Reactive]
    private ChartObjects _chartSettings;

    /// <summary>Stores the auto scale value.</summary>
    [Reactive]
    private bool _autoScale;

    /// <summary>Stores the manual scale value.</summary>
    [Reactive]
    private bool _manualScale;

    /// <summary>Stores the mode value.</summary>
    [Reactive]
    private int _mode;

    /// <summary>Stores the number points plotted value.</summary>
    [Reactive]
    private int _numberPointsPlotted;

    /// <summary>Stores the use fixed number of points value.</summary>
    [Reactive]
    private bool _useFixedNumberOfPoints;

    /// <summary>Initializes a new instance of the <see cref="DataLoggerUI"/> class, configuring a data logger visualization for a WpfPlot. using data from an observable sequence.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="observable">The observable value.</param>
    /// <param name="color">The color value.</param>
    /// <param name="autoscale">The autoscale value.</param>
    /// <param name="manualscale">The manualscale value.</param>
    /// <param name="points">The points value.</param>
    public DataLoggerUI(WpfPlot plot, IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable, string color, bool autoscale = true, bool manualscale = false, bool points = false)
    {
        ChartSettings = new(color: color);
        ManualScale = manualscale;
        AutoScale = autoscale;

        Plot = plot;
        CreateDataLogger(color);

        // Set name from first emission of the observable
        _ = observable
            .Take(1)
            .Where(d => !string.IsNullOrEmpty(d.Name))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(data => ChartSettings.ItemName = data.Name!)
            .DisposeWith(Disposables);

        ChartSettings.AppearanceSubsriptions(Plot, PlotLine!);
    }

    /// <summary>Initializes a new instance of the <see cref="DataLoggerUI"/> class, configuring the plot and data logger to visualize. observable data streams with customizable appearance and scaling options.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="observable">The observable value.</param>
    /// <param name="color">The color value.</param>
    /// <param name="autoscale">The autoscale value.</param>
    /// <param name="manualscale">The manualscale value.</param>
    /// <param name="points">The points value.</param>
    public DataLoggerUI(WpfPlot plot, IObservable<(string? Name, IList<double>? X, int Axis, int nPoints)> observable, string color, bool autoscale = true, bool manualscale = false, bool points = false)
    {
        ChartSettings = new(color: color);
        ManualScale = manualscale;
        AutoScale = autoscale;

        Plot = plot;
        CreateDataLogger(color);

        // Set name from first emission of the observable
        _ = observable
            .Take(1)
            .Where(d => !string.IsNullOrEmpty(d.Name))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(data => ChartSettings.ItemName = data.Name!)
            .DisposeWith(Disposables);

        UpdateDataLogger(observable);
        ChartSettings.AppearanceSubsriptions(Plot, PlotLine!);
    }

    /// <summary>Gets or sets the WPF plot control used to display graphical data within the application.</summary>
    public WpfPlot Plot { get; set; }

    /// <summary>Gets or sets the data logger used for recording plot line information.</summary>
    public DataLogger? PlotLine { get; set; }

    /// <summary>Initializes a new data logger line on the plot and sets its color using the specified hex value.</summary>
    /// <param name="color">The color value.</param>
    public void CreateDataLogger(string color)
    {
        PlotLine = Plot.Plot.Add.DataLogger();
        PlotLine.ViewSlide();
        PlotLine.ManageAxisLimits = false;
        PlotLine.LineStyle.Width = 1;
        PlotLine.Color = Color.FromHex(color);
    }

    /// <summary>Subscribes to an observable sequence of data points and updates the data logger plot with incoming values.</summary>
    /// <param name="observable">The observable value.</param>
    public void UpdateDataLogger(IObservable<(string? Name, IList<double>? Value, int Axis, int nPoints)> observable) => observable
        .ObserveOn(RxSchedulers.TaskpoolScheduler)
        .Where(d => !string.IsNullOrEmpty(d.Name) && d.Value?.Count > 0 && d.nPoints > 0)
        .Select(data => (data.Value!, Math.Min(data.nPoints, MaximumLoggedPointCount)))
        .Retry(int.MaxValue)
        .ObserveOn(RxSchedulers.MainThreadScheduler)
        .Subscribe(d =>
        {
            var (valueList, nPoints) = d;
            var count = valueList.Count;

            // Reuse or grow buffer to avoid allocations
            if (_valueBuffer is null || _valueBuffer.Length < count)
            {
                _valueBuffer = new double[count];
            }

            // Copy values to buffer
            for (var i = 0; i < count; i++)
            {
                _valueBuffer[i] = valueList[i];
            }

            // Use ArraySegment since ScottPlot DataLogger doesn't support Span
            if (count == _valueBuffer.Length)
            {
                PlotLine!.Add(_valueBuffer);
            }
            else
            {
                var values = new double[count];
                Array.Copy(_valueBuffer, values, count);
                PlotLine!.Add(values);
            }

            if (PlotLine!.Data.Coordinates.Count > nPoints)
            {
                PlotLine.Data.Coordinates.RemoveRange(0, PlotLine.Data.Coordinates.Count - nPoints);
            }

            PlotLine.ManageAxisLimits = false;

            if (ChartSettings.IsPaused)
            {
                return;
            }

            Plot.Refresh();
        }).DisposeWith(Disposables);

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    /// <param name="disposing">The disposing value.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ChartSettings.IsCheckedCmd?.Dispose();
            ChartSettings.Dispose();
            _valueBuffer = null;
        }

        base.Dispose(disposing);
    }
}
