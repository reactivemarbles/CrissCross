// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
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
    private double[]? _xBuffer;
    private double[]? _yBuffer;

    [Reactive]
    private ChartObjects _chartSettings;
    [Reactive]
    private bool _autoScale;
    [Reactive]
    private bool _manualScale;
    [Reactive]
    private int _mode;
    [Reactive]
    private int _numberPointsPlotted;
    [Reactive]
    private bool _useFixedNumberOfPoints;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScatterUI"/> class, configuring a scatter plot visualization and subscribing to.
    /// observable data updates.
    /// </summary>
    /// <remarks>The series name is set from the first emission of the observable sequence. Subsequent updates
    /// to the plot are handled automatically as new data arrives. Appearance settings are applied to the plot and its
    /// line after initialization.</remarks>
    /// <param name="plot">The WpfPlot control that displays the scatter plot.</param>
    /// <param name="observable">An observable sequence providing data updates for the scatter plot, including the series name, X and Y values,
    /// and axis assignment.</param>
    /// <param name="color">The color used to render the scatter plot line.</param>
    /// <param name="autoscale">Indicates whether the plot axes should automatically scale to fit the data. Defaults to <see langword="true"/>.</param>
    /// <param name="manualscale">Indicates whether manual axis scaling is enabled. Defaults to <see langword="false"/>.</param>
    public ScatterUI(WpfPlot plot, IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable, string color, bool autoscale = true, bool manualscale = false)
    {
        ChartSettings = new(color: color);
        ManualScale = manualscale;
        AutoScale = autoscale;

        Plot = plot;

        CreateScatter(color);

        // Set name from first emission of the observable
        observable
            .Take(1)
            .Where(d => !string.IsNullOrEmpty(d.Name))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(data => ChartSettings.ItemName = data.Name!)
            .DisposeWith(Disposables);

        UpdateScatter(observable);
        ChartSettings.AppearanceSubsriptions(Plot, PlotLine!);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScatterUI"/> class to display a scatter plot with the specified data and.
    /// appearance settings.
    /// </summary>
    /// <remarks>If both <paramref name="autoscale"/> and <paramref name="manualscale"/> are set to <see
    /// langword="true"/>, autoscaling will take precedence. The series name from <paramref name="data"/> is used for
    /// display purposes if provided.</remarks>
    /// <param name="plot">The WpfPlot control where the scatter plot will be rendered.</param>
    /// <param name="data">A tuple containing the series name, X values, Y values, and axis index for the scatter plot. The series name may
    /// be null.</param>
    /// <param name="color">The color used to display the scatter plot line.</param>
    /// <param name="autoscale">Indicates whether the plot axes should automatically scale to fit the data. The default is <see
    /// langword="true"/>.</param>
    /// <param name="manualscale">Indicates whether manual axis scaling is enabled. The default is <see langword="false"/>.</param>
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

    /// <summary>
    /// Gets or sets the WpfPlot control used for rendering interactive plots within the application.
    /// </summary>
    /// <remarks>Assigning a new WpfPlot instance replaces the current plot displayed in the user interface.
    /// This property is typically used to embed or update graphical data visualizations in WPF applications.</remarks>
    public WpfPlot Plot { get; set; }

    /// <summary>
    /// Gets or sets the plot line to be displayed on the scatter chart.
    /// </summary>
    public Scatter? PlotLine { get; set; }

    /// <summary>
    /// Gets or sets the collection of axes used for chart rendering and data visualization.
    /// </summary>
    /// <remarks>Use this property to configure the axes for the chart, such as setting axis types, ranges, or
    /// labels. Modifying the axes affects how data is displayed and interpreted within the chart.</remarks>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>
    /// Creates a scatter plot with the specified color.
    /// </summary>
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

    /// <summary>
    /// Replaces the current scatter plot data with the specified X and Y coordinate values.
    /// </summary>
    /// <remarks>Both <paramref name="x"/> and <paramref name="y"/> must contain the same number of elements.
    /// Existing plot data will be replaced with the new values.</remarks>
    /// <param name="x">The collection of X values to plot. The number of elements must match the number of elements in <paramref
    /// name="y"/>. Cannot be null.</param>
    /// <param name="y">The collection of Y values to plot. The number of elements must match the number of elements in <paramref
    /// name="x"/>. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="x"/> or <paramref name="y"/> is null.</exception>
    public void InsertData(IList<double> x, IList<double> y)
    {
        if (x == null)
        {
            throw new ArgumentNullException(nameof(x));
        }

        if (y == null)
        {
            throw new ArgumentNullException(nameof(y));
        }

        var count = x.Count;
        Axes = PlotLine!.Axes;
        Plot.Plot.Remove(PlotLine!);

        // Reuse or grow buffers to avoid allocations
        if (_xBuffer == null || _xBuffer.Length < count)
        {
            _xBuffer = new double[count];
            _yBuffer = new double[count];
        }

        // Copy data to buffers
        for (var i = 0; i < count; i++)
        {
            _xBuffer[i] = x[i];
            _yBuffer![i] = y[i];
        }

        // Create correctly sized arrays for scatter plot
        double[] xs;
        double[] ys;
        if (count == _xBuffer.Length)
        {
            xs = _xBuffer;
            ys = _yBuffer!;
        }
        else
        {
            xs = new double[count];
            ys = new double[count];
            Array.Copy(_xBuffer, xs, count);
            Array.Copy(_yBuffer!, ys, count);
        }

        PlotLine = Plot.Plot.Add.Scatter(xs, ys);
        PlotLine.LineStyle.Width = 1f;
        PlotLine.LineStyle.IsVisible = false;
        PlotLine.MarkerSize = 1f;
        PlotLine!.Axes = Axes;
    }

    /// <summary>
    /// Subscribes to an observable sequence of scatter plot data and updates the plot with new values as they arrive.
    /// </summary>
    /// <remarks>The plot is updated only when valid data is received and the chart is not paused. Axis limits
    /// are automatically adjusted if manual or automatic scaling is enabled. The subscription is disposed of with the
    /// object's disposables to manage resources appropriately.</remarks>
    /// <param name="observable">An observable sequence that provides tuples containing the series name, X values, Y values, and axis index. Each
    /// tuple must have non-null Name, X, and Y values, with X and Y lists of equal, non-zero length.</param>
    public void UpdateScatter(IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)> observable) =>
        observable
            .Where(data => data.Name != null && data.X != null && data.Y != null && data.X.Count > 0 && data.Y.Count > 0 && data.X.Count == data.Y.Count)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(data =>
            {
                InsertData(data.X!, data.Y);

                // UPDATE X AXIS
                if (ManualScale || AutoScale)
                {
                    var xList = data.X!;
                    var yList = data.Y;

                    // Calculate min/max without LINQ allocations
                    var (xMin, xMax) = GetMinMax(xList);
                    var (yMin, yMax) = GetMinMax(yList);

                    Plot.Plot.Axes.SetLimitsX(xMin - 1, xMax + 1, Plot!.Plot.Axes.Bottom);
                    Plot.Plot.Axes.SetLimitsY(yMin - 1, yMax + 1, Plot!.Plot.Axes.Left);
                }

                // UPDATE IF IS NOT PAUSED
                if (!ChartSettings.IsPaused)
                {
                    Plot.Refresh();
                }
            }).DisposeWith(Disposables);

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
    /// unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ChartSettings.IsCheckedCmd?.Dispose();
            ChartSettings.Dispose();
            _chartSettings.Dispose();
            _xBuffer = null;
            _yBuffer = null;
        }
    }

    /// <summary>
    /// Returns the minimum and maximum values from the specified collection of doubles.
    /// </summary>
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
