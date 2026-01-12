// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Runtime.Versioning;
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Provides a user interface component for displaying and interacting with a crosshair overlay on a plot, supporting
/// features such as autoscaling, manual scaling, and dynamic coordinate tracking.
/// </summary>
/// <remarks>This class is intended for use with Windows platforms and integrates with WpfPlot to visualize
/// crosshair markers and coordinate information. It supports both numeric and date/time axes, and can subscribe to
/// coordinate updates for interactive crosshair movement. Thread safety is not guaranteed; ensure that interactions
/// with UI elements occur on the appropriate thread.</remarks>
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
    [Reactive]
    private bool _useFixedNumberOfPoints;

    /// <summary>
    /// Initializes a new instance of the <see cref="Crosshair_UI"/> class, configuring crosshair display and marker behavior for the.
    /// specified plot with customizable appearance and scaling options.
    /// </summary>
    /// <remarks>If coordinatesObs is provided, the crosshair and marker label will update dynamically as new
    /// coordinates are received. When isXAxisDateTime is <see langword="true"/>, X axis values are displayed as
    /// formatted time strings. Autoscale and manualscale options control how the plot view responds to crosshair
    /// changes.</remarks>
    /// <param name="plot">The WpfPlot instance to which the crosshair and marker will be attached.</param>
    /// <param name="data">A tuple containing the name and axis index for the data series associated with the crosshair.</param>
    /// <param name="color">The color used for the crosshair and marker appearance.</param>
    /// <param name="isXAxisDateTime">Indicates whether the X axis represents DateTime values. If <see langword="true"/>, X axis values will be
    /// formatted as time strings.</param>
    /// <param name="autoscale">Specifies whether the plot should automatically scale to fit the crosshair and marker. Set to <see
    /// langword="true"/> to enable autoscaling.</param>
    /// <param name="manualscale">Specifies whether manual scaling is enabled for the crosshair and marker. Set to <see langword="true"/> to allow
    /// manual adjustment.</param>
    /// <param name="coordinatesObs">An optional observable sequence of Coordinates used to update the crosshair position and marker label in
    /// response to mouse movement. If null, the crosshair is initialized without dynamic updates.</param>
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
            CreateCrosshair(color: color);
        }
        else if (coordinatesObs != null && isXAxisDateTime)
        {
            MouseCoordinatesObs = coordinatesObs.Subscribe(x =>
            {
                var xtext = DateTime.FromOADate(x.X).ToLongTimeString();
                ChartSettings.MarkerText.LabelText = $"{x.Y:0.##}\n{xtext}";
                Plot?.Refresh();
            }).DisposeWith(Disposables);
            CreateCrosshair(color: color, DateTime.Now.ToOADate());
        }
        else
        {
            CreateCrosshair(color: color);
        }
    }

    /// <summary>
    /// Gets or sets the WpfPlot control used for rendering interactive plots within the application.
    /// </summary>
    public WpfPlot Plot { get; set; }

    /// <summary>
    /// Gets or sets the subscription used to observe mouse coordinate changes.
    /// </summary>
    /// <remarks>Dispose the returned object to stop receiving mouse coordinate updates and release resources
    /// associated with the observation.</remarks>
    public IDisposable? MouseCoordinatesObs { get; set; }

    /// <summary>
    /// Gets or sets the crosshair line to display on the plot, if any.
    /// </summary>
    public Crosshair? PlotLine { get; set; }

    /// <summary>
    /// Adds a draggable crosshair to the plot at the specified position and color.
    /// </summary>
    /// <remarks>The crosshair consists of horizontal and vertical lines that can be dragged by the user. Both
    /// lines display a label with a customizable background color. If an invalid color name is provided, the crosshair
    /// may not display as intended.</remarks>
    /// <param name="color">The name of the color to use for the crosshair lines and labels. Must be a valid system color name.</param>
    /// <param name="position">The horizontal position, in plot coordinates, where the crosshair is initially placed. Defaults to 0.0.</param>
    public void CreateCrosshair(string color, double position = 0.0)
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
        PlotLine.HorizontalLine.LabelFontColor = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(color));
        PlotLine.LineColor = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(color));
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
