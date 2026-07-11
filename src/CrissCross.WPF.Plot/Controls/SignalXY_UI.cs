// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
    /// <summary>Stores the chart settings value.</summary>
    [Reactive]
    private ChartObjects? _chartSettings;

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

    /// <summary>Initializes a new instance of the <see cref="SignalXY_UI"/> class to display an XY signal on the specified WpfPlot using the. provided data and appearance settings.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="data">The data value.</param>
    /// <param name="color">The color value.</param>
    /// <param name="autoscale">The autoscale value.</param>
    /// <param name="manualscale">The manualscale value.</param>
    /// <param name="coordinatesObs">The coordinatesObs value.</param>
    public SignalXY_UI(
        WpfPlot plot,
        (string? Name, IList<double>? Value, IList<double> DateTime, int Axis) data,
        string color,
        bool autoscale = true,
        bool manualscale = false,
        IObservable<Coordinates>? coordinatesObs = null)
    {
        if (data.Value is null || data.DateTime is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        ChartSettings = new(itemName: data.Name!, color: color);
        _ = ChartSettings.DisposeWith(Disposables);
        ManualScale = manualscale;
        AutoScale = autoscale;

        Plot = plot;
        CreateSignal(color);
        PlotLine!.Data = new SignalXYSourceDoubleArray([.. data.DateTime], [.. data.Value]);
        ChartSettings.AppearanceSubsriptions(Plot, PlotLine);
        ChartSettings.CreateCursorValues(Plot, color);

        if (coordinatesObs is null)
        {
            return;
        }

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

    /// <summary>Gets or sets the WpfPlot control used for rendering interactive plots within the application.</summary>
    /// <remarks>Assigning a new WpfPlot instance replaces the current plot displayed in the control. This
    /// property is typically used to configure or update the plot shown to users.</remarks>
    public WpfPlot Plot { get; set; }

    /// <summary>Gets or sets the plot line to be displayed on the chart as an XY signal.</summary>
    public SignalXY? PlotLine { get; set; }

    /// <summary>Gets or sets the subscription used to observe mouse coordinate changes.</summary>
    /// <remarks>Dispose the returned object to stop receiving mouse coordinate updates and release associated
    /// resources. Assigning a new value may replace an existing subscription; ensure previous subscriptions are
    /// disposed if no longer needed.</remarks>
    public IDisposable? MouseCoordinatesObs { get; set; }

    /// <summary>Creates a new signal plot line with the specified color.</summary>
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

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    /// <param name="disposing">The disposing value.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            MouseCoordinatesObs?.Dispose();
        }

        base.Dispose(disposing);
    }
}
