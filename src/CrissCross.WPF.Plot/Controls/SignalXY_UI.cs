// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.Versioning;
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.DataSources;
using ScottPlot.Plottables;
using ScottPlot.WPF;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Nice for historical data (performance).
/// </summary>
/// <seealso cref="SignalXY_UI" />
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
    /// Initializes a new instance of the <see cref="SignalXY_UI"/> class.
    /// </summary>
    /// <param name="plot">The plot.</param>
    /// <param name="data">The data.</param>
    /// <param name="color">The color.</param>
    /// <param name="autoscale">if set to <c>true</c> [autoscale].</param>
    /// <param name="manualscale">if set to <c>true</c> [manualscale].</param>
    /// <param name="coordinatesObs">The coordinates obs.</param>
    /// <exception cref="System.ArgumentNullException">data.</exception>
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
    /// Gets or sets the plot.
    /// </summary>
    /// <value>
    /// The plot.
    /// </value>
    public WpfPlot Plot { get; set; }

    /// <summary>
    /// Gets or sets the streamer.
    /// </summary>
    /// <value>
    /// The streamer.
    /// </value>
    public SignalXY? PlotLine { get; set; }

    /// <summary>
    /// Gets or sets the mouse coordinates.
    /// </summary>
    /// <value>
    /// The mouse coordinates.
    /// </value>
    public IDisposable? MouseCoordinatesObs { get; set; }

    /// <summary>
    /// Creates the stream.
    /// </summary>
    /// <param name="color">color.</param>
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
