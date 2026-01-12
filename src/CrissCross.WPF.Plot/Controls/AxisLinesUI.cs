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
/// Provides UI functionality for displaying and managing axis lines on a WPF plot, supporting both horizontal and
/// vertical orientations with customizable appearance and reactive updates.
/// </summary>
/// <remarks>AxisLinesUI enables dynamic creation and updating of axis lines on a WpfPlot, allowing integration
/// with observable data sources for real-time position changes. The class supports customization of line style, color,
/// label text, and axis selection. It is intended for use on Windows platforms and manages resources appropriately
/// through disposal. Thread safety is considered when updating UI elements via observables.</remarks>
[SupportedOSPlatform("windows")]
public partial class AxisLinesUI : RxObject
{
    [Reactive]
    private ChartObjects _chartSettings = new();

    [Reactive]
    private string _lineOrientation;

    [Reactive]
    private int _axis;

    [Reactive]
    private string _labelText;

    [Reactive]
    private LinePattern _linePattern1;

    /// <summary>
    /// Initializes a new instance of the <see cref="AxisLinesUI"/> class, configuring axis lines on the specified plot with.
    /// customizable orientation, appearance, and label text. Subscribes to an observable to update the axis line's
    /// position and name dynamically.
    /// </summary>
    /// <remarks>If the orientation is set to "Horizontal", a horizontal axis line is created; if set to
    /// "Vertical", a vertical axis line is created. The observable parameter allows the axis line's position and name
    /// to be updated in real time as new values are emitted. This constructor is intended for use in interactive or
    /// dynamic plotting scenarios where axis lines need to reflect changing data.</remarks>
    /// <param name="plot">The WpfPlot control on which the axis lines will be rendered.</param>
    /// <param name="observable">An observable sequence that provides updates for the axis line's name and position. Each tuple contains an
    /// optional name and an optional position value.</param>
    /// <param name="linePattern">The line pattern to use for rendering the axis line, such as solid or dashed.</param>
    /// <param name="orientation">The orientation of the axis line. Specify "Horizontal" or "Vertical". Defaults to "Horizontal".</param>
    /// <param name="axis">The index of the axis to which the line is associated. Defaults to 0.</param>
    /// <param name="color">The color of the axis line. Specify a color name or value. Defaults to "Blue".</param>
    /// <param name="text">The label text to display alongside the axis line. Defaults to "---".</param>
    public AxisLinesUI(
        WpfPlot plot,
        IObservable<(string? Name, double? Position)> observable,
        LinePattern linePattern,
        string orientation = "Horizontal",
        int axis = 0,
        string color = "Blue",
        string text = "---")
    {
        LineOrientation = orientation;
        LinePattern1 = linePattern;
        Plot = plot;
        Axis = axis;
        ChartSettings.Color = color;
        LabelText = text;

        if (orientation == "Horizontal")
        {
            CreateHorizontalLine();
            UpdateAxisLineSubscription(observable);
        }
        else if (orientation == "Vertical")
        {
            CreateVerticalLine();
            UpdateAxisLineSubscription(observable);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AxisLinesUI"/> class, adding a horizontal or vertical axis line to the specified.
    /// plot at the given position with customizable appearance and label.
    /// </summary>
    /// <remarks>If the type parameter is set to "Horizontal", a horizontal axis line is created; if set to
    /// "Vertical", a vertical axis line is created. The appearance and label of the line can be customized using the
    /// provided parameters.</remarks>
    /// <param name="plot">The WpfPlot control to which the axis line will be added. Cannot be null.</param>
    /// <param name="position">The position, in plot coordinates, where the axis line will be drawn.</param>
    /// <param name="linePattern">The line pattern to apply to the axis line, such as solid or dashed.</param>
    /// <param name="type">The orientation of the axis line. Specify "Horizontal" to create a horizontal line or "Vertical" for a vertical
    /// line. Defaults to "Horizontal".</param>
    /// <param name="axis">The index of the axis to which the line is associated. Defaults to 0.</param>
    /// <param name="color">The color of the axis line. Specify a color name or value. Defaults to "Blue".</param>
    /// <param name="text">The label text to display with the axis line. Defaults to "---".</param>
    public AxisLinesUI(
        WpfPlot plot,
        double position,
        in LinePattern linePattern,
        string type = "Horizontal",
        int axis = 0,
        string color = "Blue",
        string text = "---")
    {
        LineOrientation = type;
        LinePattern1 = linePattern;
        Plot = plot;
        Axis = axis;
        ChartSettings.Color = color;
        LabelText = text;

        if (type == "Horizontal")
        {
            CreateHorizontalLine(position);
        }
        else if (type == "Vertical")
        {
            CreateVerticalLine(position);
        }
    }

    /// <summary>
    /// Gets or sets the WPF plot control used to display graphical data within the application.
    /// </summary>
    /// <remarks>Assigning a new value to this property replaces the current plot control instance. This
    /// property is typically used to embed interactive plots in WPF user interfaces.</remarks>
    public WpfPlot Plot { get; set; }

    /// <summary>
    /// Gets or sets the visual properties of the axis line, such as color, thickness, and style.
    /// </summary>
    /// <remarks>Set this property to customize the appearance of the axis line in the chart. If the value is
    /// <see langword="null"/>, the axis line will not be displayed.</remarks>
    public AxisLine? AxisLine { get; set; }

    /// <summary>
    /// Adds a vertical line to the plot at the specified position.
    /// </summary>
    /// <remarks>The line's appearance, including color and width, is determined by the current chart
    /// settings. The line will display the label text specified by the LabelText property.</remarks>
    /// <param name="position">The x-coordinate at which to place the vertical line. The default value is 0.0.</param>
    public void CreateVerticalLine(double position = 0.0)
    {
        var color = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(ChartSettings.Color!));
        AxisLine = Plot.Plot.Add.VerticalLine(x: position, width: (float)ChartSettings.LineWidth, color: color);
        AxisLine.LabelText = LabelText;
    }

    /// <summary>
    /// Adds a horizontal line to the plot at the specified vertical position.
    /// </summary>
    /// <remarks>The line's appearance, including color, width, label text, and alignment, is determined by
    /// the current chart settings. Use this method to highlight a specific value or threshold on the plot.</remarks>
    /// <param name="position">The vertical position, in plot coordinates, where the horizontal line will be drawn. Defaults to 0.0.</param>
    public void CreateHorizontalLine(double position = 0.0)
    {
        var color = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(ChartSettings.Color!));
        AxisLine = Plot.Plot.Add.HorizontalLine(y: position, width: (float)ChartSettings.LineWidth, color: color);
        AxisLine.LabelText = LabelText;
        AxisLine.LabelAlignment = Alignment.MiddleCenter;
        AxisLine.LinePattern = LinePattern1;
    }

    /// <summary>
    /// Subscribes to an observable sequence that provides axis line updates and applies changes to the chart
    /// accordingly.
    /// </summary>
    /// <remarks>Updates to the axis line position and chart name are processed on a background thread and
    /// applied to the UI thread. The chart is refreshed only if it is not paused. The subscription is disposed
    /// automatically with the object's disposables.</remarks>
    /// <param name="observable">An observable sequence emitting tuples containing the axis line name and its position. The name may be null or
    /// empty, and the position may be null; updates are only applied when both are valid.</param>
    public void UpdateAxisLineSubscription(IObservable<(string? Name, double? Position)> observable) =>
        observable
        .SubscribeOn(RxSchedulers.TaskpoolScheduler) // Procesa en un hilo de fondo
        .ObserveOn(RxSchedulers.MainThreadScheduler) // Actualiza la UI en el hilo principal
        .Subscribe(data =>
        {
            // CHECKS
            if (string.IsNullOrEmpty(data.Name) || data.Position == null)
            {
                return;
            }

            AxisLine!.Position = (double)data.Position;

            // UPDATE IF IS NOT PAUSED
            if (!ChartSettings.IsPaused)
            {
                Plot.Refresh();
            }

            // UPDATE NAME
            ChartSettings.ItemName = data.Name;
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
            ChartSettings.Dispose();
            _chartSettings.Dispose();
        }
    }
}
