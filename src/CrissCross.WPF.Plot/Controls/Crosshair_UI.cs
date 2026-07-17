// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
    /// <summary>The crosshair stroke width.</summary>
    private const float CrosshairLineWidth = 3;

    /// <summary>The vertical offset for the horizontal-axis label.</summary>
    private const float HorizontalLabelOffsetY = -10;

    /// <summary>The horizontal offset for the horizontal-axis label.</summary>
    private const float HorizontalLabelOffsetX = 80;

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

    /// <summary>Initializes a new instance of the <see cref="Crosshair_UI"/> class, configuring crosshair display and
    /// marker behavior for the. specified plot with customizable appearance and scaling options.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="data">The data value.</param>
    /// <param name="color">The color value.</param>
    public Crosshair_UI(WpfPlot plot, (string? Name, int Axis) data, string color)
        : this(plot, data, color, false)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Crosshair_UI"/> class.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="data">The data value.</param>
    /// <param name="color">The color value.</param>
    /// <param name="isXAxisDateTime">The isXAxisDateTime value.</param>
    public Crosshair_UI(WpfPlot plot, (string? Name, int Axis) data, string color, bool isXAxisDateTime)
        : this(plot, data, color, isXAxisDateTime, true)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Crosshair_UI"/> class.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="data">The data value.</param>
    /// <param name="color">The color value.</param>
    /// <param name="isXAxisDateTime">The isXAxisDateTime value.</param>
    /// <param name="coordinatesObs">The coordinatesObs value.</param>
    public Crosshair_UI(
        WpfPlot plot,
        (string? Name, int Axis) data,
        string color,
        bool isXAxisDateTime,
        IObservable<Coordinates>? coordinatesObs)
        : this(plot, data, color, isXAxisDateTime, true, false, coordinatesObs)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Crosshair_UI"/> class.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="data">The data value.</param>
    /// <param name="color">The color value.</param>
    /// <param name="isXAxisDateTime">The isXAxisDateTime value.</param>
    /// <param name="autoscale">The autoscale value.</param>
    public Crosshair_UI(
        WpfPlot plot,
        (string? Name, int Axis) data,
        string color,
        bool isXAxisDateTime,
        bool autoscale)
        : this(plot, data, color, isXAxisDateTime, autoscale, false)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Crosshair_UI"/> class.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="data">The data value.</param>
    /// <param name="color">The color value.</param>
    /// <param name="isXAxisDateTime">The isXAxisDateTime value.</param>
    /// <param name="autoscale">The autoscale value.</param>
    /// <param name="manualscale">The manualscale value.</param>
    public Crosshair_UI(
        WpfPlot plot,
        (string? Name, int Axis) data,
        string color,
        bool isXAxisDateTime,
        bool autoscale,
        bool manualscale)
        : this(plot, data, color, isXAxisDateTime, autoscale, manualscale, null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Crosshair_UI"/> class.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="data">The data value.</param>
    /// <param name="color">The color value.</param>
    /// <param name="isXAxisDateTime">The isXAxisDateTime value.</param>
    /// <param name="autoscale">The autoscale value.</param>
    /// <param name="manualscale">The manualscale value.</param>
    /// <param name="coordinatesObs">The coordinatesObs value.</param>
    public Crosshair_UI(
        WpfPlot plot,
        (string? Name, int Axis) data,
        string color,
        bool isXAxisDateTime,
        bool autoscale,
        bool manualscale,
        IObservable<Coordinates>? coordinatesObs)
    {
        ChartSettings = new(itemName: data.Name!, color: color);
        ManualScale = manualscale;
        AutoScale = autoscale;
        Plot = plot;

        ChartSettings?.AppearanceSubsriptions(Plot);
        ChartSettings?.CreateCursorValues(Plot, color);
        ChartSettings!.Marker.IsVisible = false;

        if (coordinatesObs is not null && !isXAxisDateTime)
        {
            MouseCoordinatesObs = coordinatesObs
                .Subscribe(x =>
                {
                    ChartSettings.MarkerText.LabelText = $"{x.Y:0.##}\n{x.X:0.##}";
                    Plot?.Refresh();
                })
                .DisposeWith(Disposables);
            CreateCrosshair(color: color);
        }
        else if (coordinatesObs is not null && isXAxisDateTime)
        {
            MouseCoordinatesObs = coordinatesObs
                .Subscribe(x =>
                {
                    var xtext = DateTime.FromOADate(x.X).ToLongTimeString();
                    ChartSettings.MarkerText.LabelText = $"{x.Y:0.##}\n{xtext}";
                    Plot?.Refresh();
                })
                .DisposeWith(Disposables);
            CreateCrosshair(color: color, DateTime.Now.ToOADate());
        }
        else
        {
            CreateCrosshair(color: color);
        }
    }

    /// <summary>Gets or sets the WpfPlot control used for rendering interactive plots within the application.</summary>
    public WpfPlot Plot { get; set; }

    /// <summary>Gets or sets the subscription used to observe mouse coordinate changes.</summary>
    /// <remarks>Dispose the returned object to stop receiving mouse coordinate updates and release resources
    /// associated with the observation.</remarks>
    public IDisposable? MouseCoordinatesObs { get; set; }

    /// <summary>Gets or sets the crosshair line to display on the plot, if any.</summary>
    public Crosshair? PlotLine { get; set; }

    /// <summary>Adds a draggable crosshair to the plot at the specified position and color.</summary>
    /// <remarks>The crosshair consists of horizontal and vertical lines that can be dragged by the user. Both
    /// lines display a label with a customizable background color. If an invalid color name is provided, the crosshair
    /// may not display as intended.</remarks>
    /// <param name="color">The name of the color to use for the crosshair lines and labels. Must be a valid system
    /// color name.</param>
    public void CreateCrosshair(string color) => CreateCrosshair(color, 0.0);

    /// <summary>Adds a draggable crosshair to the plot at the specified position and color.</summary>
    /// <param name="color">The name of the color to use for the crosshair lines and labels.</param>
    /// <param name="position">The horizontal position, in plot coordinates.</param>
    public void CreateCrosshair(string color, double position)
    {
        PlotLine = Plot.Plot.Add.Crosshair(position, 0);
        PlotLine.IsVisible = true;
        PlotLine.LineWidth = CrosshairLineWidth;
        PlotLine.HorizontalLine.IsDraggable = true;
        PlotLine.VerticalLine.IsDraggable = true;
        PlotLine!.HorizontalLine.LabelStyle.BackgroundColor = ScottPlot.Colors.White;
        PlotLine!.VerticalLine.LabelStyle.BackgroundColor = ScottPlot.Colors.White;
        PlotLine.HorizontalLine.Text = "Click and drag me";
        PlotLine.VerticalLine.Text = "Click and drag me";
        PlotLine.HorizontalLine.LabelStyle.IsVisible = true;
        PlotLine.VerticalLine.LabelStyle.IsVisible = true;
        PlotLine.HorizontalLine.LabelStyle.OffsetY = HorizontalLabelOffsetY;
        PlotLine.HorizontalLine.LabelStyle.OffsetX = HorizontalLabelOffsetX;
        PlotLine.HorizontalLine.LabelStyle.Rotation = 0;
        PlotLine.HorizontalLine.LabelFontColor = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(color));
        PlotLine.LineColor = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(color));
    }

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
        MouseCoordinatesObs?.Dispose();
        base.Dispose(disposing);
    }
}
