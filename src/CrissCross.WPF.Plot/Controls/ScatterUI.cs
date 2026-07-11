// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Provides a user interface component for displaying and interacting with scatter plots in a WPF application. Supports
/// real-time data streaming and customizable scaling options. Nice for plotting XY random values.
/// </summary>
/// <remarks>ScatterUI is designed for use on Windows platforms and integrates with reactive data sources to
/// update scatter plots dynamically. It supports both automatic and manual axis scaling, and exposes properties for
/// plot configuration and appearance. Thread safety is ensured for UI updates by observing data on the main thread.
/// Dispose of instances when no longer needed to release resources associated with plot elements and
/// subscriptions.</remarks>
[SupportedOSPlatform("windows")]
public partial class ScatterUI : RxObject, IPlottableUI
{
    /// <summary>Stores the horizontal buffer value.</summary>
    private double[]? _horizontalBuffer;

    /// <summary>Stores the vertical buffer value.</summary>
    private double[]? _verticalBuffer;

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

    /// <summary>Initializes a new instance of the <see cref="ScatterUI"/> class, configuring a scatter plot visualization and subscribing to. observable data updates.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="observable">The observable value.</param>
    /// <param name="color">The color value.</param>
    /// <param name="autoscale">The autoscale value.</param>
    /// <param name="manualscale">The manualscale value.</param>
    public ScatterUI(WpfPlot plot, IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable, string color, bool autoscale = true, bool manualscale = false)
    {
        ChartSettings = new(color: color);
        ManualScale = manualscale;
        AutoScale = autoscale;

        Plot = plot;

        CreateScatter(color);

        // Set name from first emission of the observable
        _ = observable
            .Take(1)
            .Where(d => !string.IsNullOrEmpty(d.Name))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(data => ChartSettings.ItemName = data.Name!)
            .DisposeWith(Disposables);

        UpdateScatter(observable);
        ChartSettings.AppearanceSubsriptions(Plot, PlotLine!);
    }

    /// <summary>Initializes a new instance of the <see cref="ScatterUI"/> class to display a scatter plot with the specified data and. appearance settings.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="data">The data value.</param>
    /// <param name="color">The color value.</param>
    /// <param name="autoscale">The autoscale value.</param>
    /// <param name="manualscale">The manualscale value.</param>
    public ScatterUI(WpfPlot plot, (string? Name, IList<double> X, IList<double> Y, int Axis) data, string color, bool autoscale = true, bool manualscale = false)
    {
        ChartSettings = new(color: color);
        ManualScale = manualscale;
        AutoScale = autoscale;

        Plot = plot;
        ChartSettings.DisplayedValue = 0;

        // Set name from data parameter (not observable)
        if (!string.IsNullOrEmpty(data.Name))
        {
            ChartSettings.ItemName = data.Name;
        }

        CreateScatter(color);
        InsertData(data.X, data.Y);
        ChartSettings.AppearanceSubsriptions(Plot, PlotLine!);
    }

    /// <summary>Gets or sets the WpfPlot control used for rendering interactive plots within the application.</summary>
    /// <remarks>Assigning a new WpfPlot instance replaces the current plot displayed in the user interface.
    /// This property is typically used to embed or update graphical data visualizations in WPF applications.</remarks>
    public WpfPlot Plot { get; set; }

    /// <summary>Gets or sets the plot line to be displayed on the scatter chart.</summary>
    public Scatter? PlotLine { get; set; }

    /// <summary>Gets or sets the collection of axes used for chart rendering and data visualization.</summary>
    /// <remarks>Use this property to configure the axes for the chart, such as setting axis types, ranges, or
    /// labels. Modifying the axes affects how data is displayed and interpreted within the chart.</remarks>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Creates a scatter plot with the specified color.</summary>
    /// <remarks>The scatter plot is initialized with a single data point at (0, 0). The line and marker sizes
    /// are set to minimal values for a simple visual representation. The color must be a valid hexadecimal color code;
    /// otherwise, an error may occur when parsing the color.</remarks>
    /// <param name="color">A string representing the color of the scatter plot, specified in hexadecimal format (e.g., "#FF0000" for red).</param>
    public void CreateScatter(string color)
    {
        double[] y = [0];
        double[] x = [0];
        PlotLine = Plot.Plot.Add.Scatter(x, y);
        PlotLine.LineStyle.Width = 1f;
        PlotLine.MarkerSize = 1f;
        PlotLine.Color = Color.FromHex(color);
    }

    /// <summary>Replaces the current scatter plot data with the specified X and Y coordinate values.</summary>
    /// <param name="x">The x value.</param>
    /// <param name="y">The y value.</param>
    public void InsertData(IList<double> x, IList<double> y)
    {
        if (x is null)
        {
            throw new ArgumentNullException(nameof(x));
        }

        if (y is null)
        {
            throw new ArgumentNullException(nameof(y));
        }

        var count = x.Count;
        Axes = PlotLine!.Axes;
        Plot.Plot.Remove(PlotLine!);

        // Reuse or grow buffers to avoid allocations
        if (_horizontalBuffer is null || _horizontalBuffer.Length < count)
        {
            _horizontalBuffer = new double[count];
            _verticalBuffer = new double[count];
        }

        // Copy data to buffers
        for (var i = 0; i < count; i++)
        {
            _horizontalBuffer[i] = x[i];
            _verticalBuffer![i] = y[i];
        }

        // Create correctly sized arrays for scatter plot
        double[] xs;
        double[] ys;
        if (count == _horizontalBuffer.Length)
        {
            xs = _horizontalBuffer;
            ys = _verticalBuffer!;
        }
        else
        {
            xs = new double[count];
            ys = new double[count];
            Array.Copy(_horizontalBuffer, xs, count);
            Array.Copy(_verticalBuffer!, ys, count);
        }

        PlotLine = Plot.Plot.Add.Scatter(xs, ys);
        PlotLine.LineStyle.Width = 1f;
        PlotLine.LineStyle.IsVisible = false;
        PlotLine.MarkerSize = 1f;
        PlotLine!.Axes = Axes;
    }

    /// <summary>Subscribes to an observable sequence of scatter plot data and updates the plot with new values as they arrive.</summary>
    /// <param name="observable">The observable value.</param>
    public void UpdateScatter(IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable) =>
        observable
            .Where(data => data.Name is not null && data.X is not null && data.Y is not null && data.X.Count > 0 && data.Y.Count > 0 && data.X.Count == data.Y.Count)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(data =>
            {
                InsertData(data.X!, data.Y);

                // UPDATE X AXIS
                if (ManualScale || AutoScale)
                {
                    var horizontalValues = data.X!;
                    var verticalValues = data.Y;

                    // Calculate min/max without LINQ allocations
                    var (xMin, xMax) = GetMinMax(horizontalValues);
                    var (yMin, yMax) = GetMinMax(verticalValues);

                    Plot.Plot.Axes.SetLimitsX(xMin - 1, xMax + 1, Plot!.Plot.Axes.Bottom);
                    Plot.Plot.Axes.SetLimitsY(yMin - 1, yMax + 1, Plot!.Plot.Axes.Left);
                }

                // UPDATE IF IS NOT PAUSED
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
        if (!disposing)
        {
            base.Dispose(disposing);
            return;
        }

        ChartSettings.IsCheckedCmd?.Dispose();
        ChartSettings.Dispose();
        _horizontalBuffer = null;
        _verticalBuffer = null;
        base.Dispose(disposing);
    }

    /// <summary>Returns the minimum and maximum values from the specified collection of doubles.</summary>
    /// <param name="values">A non-empty list of double values from which to determine the minimum and maximum. Cannot be null or empty.</param>
    /// <returns>A tuple containing the smallest value as Min and the largest value as Max from the input list.</returns>
    private static (double Min, double Max) GetMinMax(IList<double> values)
    {
        var min = values[0];
        var max = values[0];

        for (var i = 1; i < values.Count; i++)
        {
            var value = values[i];
            if (value < min)
            {
                min = value;
            }

            if (value > max)
            {
                max = value;
            }
        }

        return (min, max);
    }
}
