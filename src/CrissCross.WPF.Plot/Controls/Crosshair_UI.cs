// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.Versioning;
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Nice for historical data (performance).
/// </summary>
/// <seealso cref="Crosshair_UI" />
[SupportedOSPlatform("windows")]
public partial class Crosshair_UI : RxObject, IPlottableUI
{
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

    /// <summary>
    /// Initializes a new instance of the <see cref="Crosshair_UI" /> class.
    /// </summary>
    /// <param name="plot">if set to <c>true</c> [paused].</param>
    /// <param name="data">The data.</param>
    /// <param name="color">The color.</param>
    /// <param name="isXAxisDateTime">if set to <c>true</c> [is x axis date time].</param>
    /// <param name="autoscale">if set to <c>true</c> [autoscale].</param>
    /// <param name="manualscale">if set to <c>true</c> [manualscale].</param>
    /// <param name="coordinatesObs">The coordinates obs.</param>
    public Crosshair_UI(
        WpfPlot plot,
        (string? Name, int Axis) data,
        string color,
        bool isXAxisDateTime = false,
        bool autoscale = true,
        bool manualscale = false,
        IObservable<Coordinates>? coordinatesObs = null)
    {
        ChartSettings = new ChartObjects(itemName: data.Name!, color: color);
        ManualScale = manualscale;
        AutoScale = autoscale;
        Plot = plot;

        ChartSettings?.AppearanceSubsriptions(Plot);
        ChartSettings?.CreateCursorValues(Plot, color);
        ChartSettings!.Marker.IsVisible = false;

        if (coordinatesObs != null && !isXAxisDateTime)
        {
            MouseCoordinatesObs = coordinatesObs.Subscribe(x =>
            {
                ChartSettings.MarkerText.LabelText = $"{x.Y:0.##}\n{x.X:0.##}";
                Plot?.Refresh();
            }).DisposeWith(Disposables);
            CreateCrosshair();
        }
        else if (coordinatesObs != null && isXAxisDateTime)
        {
            MouseCoordinatesObs = coordinatesObs.Subscribe(x =>
            {
                var xtext = DateTime.FromOADate(x.X).ToLongTimeString();
                ChartSettings.MarkerText.LabelText = $"{x.Y:0.##}\n{xtext}";
                Plot?.Refresh();
            }).DisposeWith(Disposables);
            CreateCrosshair(DateTime.Now.ToOADate());
        }
        else
        {
            CreateCrosshair();
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
    /// Gets or sets the mouse coordinates.
    /// </summary>
    /// <value>
    /// The mouse coordinates.
    /// </value>
    public IDisposable? MouseCoordinatesObs { get; set; }

    /// <summary>
    /// Gets or sets the streamer.
    /// </summary>
    /// <value>
    /// The streamer.
    /// </value>
    public Crosshair? PlotLine { get; set; }

    /// <summary>
    /// Creates the stream.
    /// </summary>
    /// <param name="position">The position.</param>
    public void CreateCrosshair(double position = 0.0)
    {
        PlotLine = Plot.Plot.Add.Crosshair(position, 0);
        PlotLine.IsVisible = true;
        PlotLine.LineWidth = 3;
        PlotLine.HorizontalLine.IsDraggable = true;
        PlotLine.VerticalLine.IsDraggable = true;
        PlotLine!.HorizontalLine.LabelStyle.BackgroundColor = ScottPlot.Colors.White;
        PlotLine!.VerticalLine.LabelStyle.BackgroundColor = ScottPlot.Colors.White;
        PlotLine.HorizontalLine.Text = "Click and drag me";
        PlotLine.VerticalLine.Text = "Click and drag me";
        PlotLine.HorizontalLine.LabelStyle.IsVisible = true;
        PlotLine.VerticalLine.LabelStyle.IsVisible = true;
        PlotLine.HorizontalLine.LabelStyle.OffsetY = -10;
        PlotLine.HorizontalLine.LabelStyle.OffsetX = 80;
        PlotLine.HorizontalLine.LabelStyle.Rotation = 0;
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
            ChartSettings.IsCheckedCmd?.Dispose();
            ChartSettings.Dispose();
            MouseCoordinatesObs?.Dispose();
        }
    }
}
