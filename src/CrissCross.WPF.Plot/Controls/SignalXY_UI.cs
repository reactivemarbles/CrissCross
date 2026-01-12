// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Runtime.Versioning;
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.DataSources;
using ScottPlot.Plottables;
using ScottPlot.WPF;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Provides a user interface component for plotting and interacting with XY signal data on a Windows platform. Supports
/// visualization, autoscaling, manual scaling, and real-time mouse coordinate tracking within a WPF plot.
/// </summary>
/// <remarks>This class is intended for use in WPF applications and requires Windows. It enables dynamic plotting
/// of XY data, including features such as autoscale, manual scale, and interactive crosshair updates based on mouse
/// movement. The plotted data and appearance can be customized via constructor parameters and exposed properties.
/// Thread safety is not guaranteed; all interactions should occur on the UI thread.</remarks>
[SupportedOSPlatform("windows")]
public partial class SignalXY_UI : RxObject, IPlottableUI
{
    [Reactive]
    private ChartObjects? _chartSettings;
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
    /// Initializes a new instance of the <see cref="SignalXY_UI"/> class to display an XY signal on the specified WpfPlot using the.
    /// provided data and appearance settings.
    /// </summary>
    /// <remarks>If a coordinates observable is supplied, the plot will display a crosshair and marker at the
    /// nearest data point to the mouse position, updating in real time. The autoscale and manualscale parameters
    /// control axis scaling behavior and can be used together to customize how the plot responds to data
    /// changes.</remarks>
    /// <param name="plot">The WpfPlot control on which the signal will be rendered.</param>
    /// <param name="data">A tuple containing the signal's name, Y-axis values, corresponding X-axis DateTime values, and the axis index to
    /// plot against. The Value and DateTime lists must not be null.</param>
    /// <param name="color">The color used to render the signal line and related chart elements.</param>
    /// <param name="autoscale">Indicates whether the plot should automatically scale its axes to fit the signal data. The default is <see
    /// langword="true"/>.</param>
    /// <param name="manualscale">Indicates whether manual axis scaling is enabled for the plot. The default is <see langword="false"/>.</param>
    /// <param name="coordinatesObs">An optional observable sequence of mouse coordinates. If provided, the plot will update crosshair and marker
    /// positions in response to mouse movements.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="data.Value"/> or <paramref name="data.DateTime"/> is null.</exception>
    public SignalXY_UI(
        WpfPlot plot,
        (string? Name, IList<double>? Value, IList<double> DateTime, int Axis) data,
        string color,
        bool autoscale = true,
        bool manualscale = false,
        IObservable<Coordinates>? coordinatesObs = null)
    {
        if (data.Value == null || data.DateTime == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        ChartSettings = new(itemName: data.Name!, color: color);
        ChartSettings.DisposeWith(Disposables);
        ManualScale = manualscale;
        AutoScale = autoscale;

        Plot = plot;
        CreateSignal(color);
        PlotLine!.Data = new SignalXYSourceDoubleArray([.. data.DateTime], [.. data.Value]);
        ChartSettings.AppearanceSubsriptions(Plot, PlotLine);
        ChartSettings.CreateCursorValues(Plot, color);

        if (coordinatesObs != null)
        {
            MouseCoordinatesObs = coordinatesObs.Subscribe(x =>
            {
                if (PlotLine!.Data.Count <= 0)
                {
                    return;
                }

                var closestCoordinate = PlotLine.GetNearestX(x, Plot.Plot.LastRender);

                ChartSettings.Crosshair!.Position = closestCoordinate.Coordinates;
                ChartSettings.Marker!.Position = closestCoordinate.Coordinates;
                ChartSettings.MarkerText!.Location = closestCoordinate.Coordinates;
                ChartSettings.MarkerText!.LabelText = $"{closestCoordinate.Y:0.##}\n{closestCoordinate.X:0.##}";

                Plot?.Refresh();
            }).DisposeWith(Disposables);
        }
    }

    /// <summary>
    /// Gets or sets the WpfPlot control used for rendering interactive plots within the application.
    /// </summary>
    /// <remarks>Assigning a new WpfPlot instance replaces the current plot displayed in the control. This
    /// property is typically used to configure or update the plot shown to users.</remarks>
    public WpfPlot Plot { get; set; }

    /// <summary>
    /// Gets or sets the plot line to be displayed on the chart as an XY signal.
    /// </summary>
    public SignalXY? PlotLine { get; set; }

    /// <summary>
    /// Gets or sets the subscription used to observe mouse coordinate changes.
    /// </summary>
    /// <remarks>Dispose the returned object to stop receiving mouse coordinate updates and release associated
    /// resources. Assigning a new value may replace an existing subscription; ensure previous subscriptions are
    /// disposed if no longer needed.</remarks>
    public IDisposable? MouseCoordinatesObs { get; set; }

    /// <summary>
    /// Creates a new signal plot line with the specified color.
    /// </summary>
    /// <remarks>The signal is initialized with a single data point at the origin. The line width is set to 1
    /// pixel. If the color string is not a valid hex code, an exception may be thrown by the underlying color
    /// conversion method.</remarks>
    /// <param name="color">A hexadecimal color string that determines the color of the signal line. Must be a valid hex color code.</param>
    public void CreateSignal(string color)
    {
        double[] y = [0];
        double[] x = [0];
        PlotLine = Plot.Plot.Add.SignalXY(x, y);
        PlotLine.LineStyle.Width = 1f;
        PlotLine.Color = Color.FromHex(color);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
    /// unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            MouseCoordinatesObs?.Dispose();
        }

        base.Dispose(disposing);
    }
}
