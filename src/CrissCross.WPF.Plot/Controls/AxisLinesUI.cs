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
    /// <summary>The default axis-line orientation.</summary>
    private const string DefaultOrientation = HorizontalOrientation;

    /// <summary>The horizontal axis-line orientation.</summary>
    private const string HorizontalOrientation = "Horizontal";

    /// <summary>The vertical axis-line orientation.</summary>
    private const string VerticalOrientation = "Vertical";

    /// <summary>The default axis-line color.</summary>
    private const string DefaultColor = "Blue";

    /// <summary>The default axis-line label.</summary>
    private const string DefaultText = "---";

    /// <summary>Stores the chart settings value.</summary>
    [Reactive]
    private ChartObjects _chartSettings = new();

    /// <summary>Stores the line orientation value.</summary>
    [Reactive]
    private string _lineOrientation;

    /// <summary>Stores the axis value.</summary>
    [Reactive]
    private int _axis;

    /// <summary>Stores the label text value.</summary>
    [Reactive]
    private string _labelText;

    /// <summary>Stores the line pattern1 value.</summary>
    [Reactive]
    private LinePattern _linePattern1;

    /// <summary>Initializes a new instance of the <see cref="AxisLinesUI"/> class, configuring axis lines on the
    /// specified plot with. customizable orientation, appearance, and label text. Subscribes to an observable to update
    /// the axis line's position and name dynamically.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="observable">The observable value.</param>
    /// <param name="linePattern">The linePattern value.</param>
    public AxisLinesUI(
        WpfPlot plot,
        IObservable<(string? Name, double? Position)> observable,
        LinePattern linePattern)
        : this(plot, observable, linePattern, DefaultOrientation)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AxisLinesUI"/> class.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="observable">The observable value.</param>
    /// <param name="linePattern">The line pattern value.</param>
    /// <param name="orientation">The orientation value.</param>
    public AxisLinesUI(
        WpfPlot plot,
        IObservable<(string? Name, double? Position)> observable,
        LinePattern linePattern,
        string orientation)
        : this(plot, observable, linePattern, orientation, 0)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AxisLinesUI"/> class.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="observable">The observable value.</param>
    /// <param name="linePattern">The line pattern value.</param>
    /// <param name="orientation">The orientation value.</param>
    /// <param name="axis">The axis value.</param>
    public AxisLinesUI(
        WpfPlot plot,
        IObservable<(string? Name, double? Position)> observable,
        LinePattern linePattern,
        string orientation,
        int axis)
        : this(plot, observable, linePattern, orientation, axis, DefaultColor)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AxisLinesUI"/> class.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="observable">The observable value.</param>
    /// <param name="linePattern">The line pattern value.</param>
    /// <param name="orientation">The orientation value.</param>
    /// <param name="axis">The axis value.</param>
    /// <param name="color">The color value.</param>
    public AxisLinesUI(
        WpfPlot plot,
        IObservable<(string? Name, double? Position)> observable,
        LinePattern linePattern,
        string orientation,
        int axis,
        string color)
        : this(plot, observable, linePattern, orientation, axis, color, DefaultText)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AxisLinesUI"/> class.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="observable">The observable value.</param>
    /// <param name="linePattern">The linePattern value.</param>
    /// <param name="orientation">The orientation value.</param>
    /// <param name="axis">The axis value.</param>
    /// <param name="color">The color value.</param>
    /// <param name="text">The text value.</param>
    public AxisLinesUI(
        WpfPlot plot,
        IObservable<(string? Name, double? Position)> observable,
        LinePattern linePattern,
        string orientation,
        int axis,
        string color,
        string text)
    {
        LineOrientation = orientation;
        LinePattern1 = linePattern;
        Plot = plot;
        Axis = axis;
        ChartSettings.Color = color;
        LabelText = text;

        if (orientation == HorizontalOrientation)
        {
            CreateHorizontalLine();
            UpdateAxisLineSubscription(observable);
        }
        else if (orientation == VerticalOrientation)
        {
            CreateVerticalLine();
            UpdateAxisLineSubscription(observable);
        }
    }

    /// <summary>Initializes a new instance of the <see cref="AxisLinesUI"/> class, adding a horizontal or vertical axis
    /// line to the specified. plot at the given position with customizable appearance and label.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="position">The position value.</param>
    /// <param name="linePattern">The linePattern value.</param>
    public AxisLinesUI(WpfPlot plot, double position, in LinePattern linePattern)
        : this(plot, position, in linePattern, DefaultOrientation)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AxisLinesUI"/> class.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="position">The position value.</param>
    /// <param name="linePattern">The linePattern value.</param>
    /// <param name="type">The type value.</param>
    public AxisLinesUI(WpfPlot plot, double position, in LinePattern linePattern, string type)
        : this(plot, position, in linePattern, type, 0)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AxisLinesUI"/> class.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="position">The position value.</param>
    /// <param name="linePattern">The linePattern value.</param>
    /// <param name="type">The type value.</param>
    /// <param name="axis">The axis value.</param>
    public AxisLinesUI(WpfPlot plot, double position, in LinePattern linePattern, string type, int axis)
        : this(plot, position, in linePattern, type, axis, DefaultColor)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AxisLinesUI"/> class.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="position">The position value.</param>
    /// <param name="linePattern">The linePattern value.</param>
    /// <param name="type">The type value.</param>
    /// <param name="axis">The axis value.</param>
    /// <param name="color">The color value.</param>
    public AxisLinesUI(WpfPlot plot, double position, in LinePattern linePattern, string type, int axis, string color)
        : this(plot, position, in linePattern, type, axis, color, DefaultText)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AxisLinesUI"/> class.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="position">The position value.</param>
    /// <param name="linePattern">The linePattern value.</param>
    /// <param name="type">The type value.</param>
    /// <param name="axis">The axis value.</param>
    /// <param name="color">The color value.</param>
    /// <param name="text">The text value.</param>
    public AxisLinesUI(
        WpfPlot plot,
        double position,
        in LinePattern linePattern,
        string type,
        int axis,
        string color,
        string text)
    {
        LineOrientation = type;
        LinePattern1 = linePattern;
        Plot = plot;
        Axis = axis;
        ChartSettings.Color = color;
        LabelText = text;

        if (type == HorizontalOrientation)
        {
            CreateHorizontalLine(position);
        }
        else if (type == VerticalOrientation)
        {
            CreateVerticalLine(position);
        }
    }

    /// <summary>Gets or sets the WPF plot control used to display graphical data within the application.</summary>
    /// <remarks>Assigning a new value to this property replaces the current plot control instance. This
    /// property is typically used to embed interactive plots in WPF user interfaces.</remarks>
    public WpfPlot Plot { get; set; }

    /// <summary>Gets or sets the visual properties of the axis line, such as color, thickness, and style.</summary>
    /// <remarks>Set this property to customize the appearance of the axis line in the chart. If the value is
    /// <see langword="null"/>, the axis line will not be displayed.</remarks>
    public AxisLine? AxisLine { get; set; }

    /// <summary>Adds a vertical line to the plot at the specified position.</summary>
    /// <remarks>The line's appearance, including color and width, is determined by the current chart
    /// settings. The line will display the label text specified by the LabelText property.</remarks>
    public void CreateVerticalLine() => CreateVerticalLine(0.0);

    /// <summary>Adds a vertical line to the plot at the specified position.</summary>
    /// <param name="position">The x-coordinate at which to place the vertical line.</param>
    public void CreateVerticalLine(double position)
    {
        var color = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(ChartSettings.Color!));
        AxisLine = Plot.Plot.Add.VerticalLine(x: position, width: (float)ChartSettings.LineWidth, color: color);
        AxisLine.LabelText = LabelText;
    }

    /// <summary>Adds a horizontal line to the plot at the specified vertical position.</summary>
    /// <remarks>The line's appearance, including color, width, label text, and alignment, is determined by
    /// the current chart settings. Use this method to highlight a specific value or threshold on the plot.</remarks>
    public void CreateHorizontalLine() => CreateHorizontalLine(0.0);

    /// <summary>Adds a horizontal line to the plot at the specified vertical position.</summary>
    /// <param name="position">The vertical position, in plot coordinates, where the horizontal line will be
    /// drawn.</param>
    public void CreateHorizontalLine(double position)
    {
        var color = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(ChartSettings.Color!));
        AxisLine = Plot.Plot.Add.HorizontalLine(y: position, width: (float)ChartSettings.LineWidth, color: color);
        AxisLine.LabelText = LabelText;
        AxisLine.LabelAlignment = Alignment.MiddleCenter;
        AxisLine.LinePattern = LinePattern1;
    }

    /// <summary>Provides the UpdateAxisLineSubscription member.</summary>
    /// <param name="observable">The observable value.</param>
    public void UpdateAxisLineSubscription(IObservable<(string? Name, double? Position)> observable) =>
        observable
            .SubscribeOn(RxSchedulers.TaskpoolScheduler) // Procesa en un hilo de fondo
            .ObserveOn(RxSchedulers.MainThreadScheduler) // Actualiza la UI en el hilo principal
            .Subscribe(data =>
            {
                // CHECKS
                if (string.IsNullOrEmpty(data.Name) || data.Position is null)
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
            })
            .DisposeWith(Disposables);

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    /// <param name="disposing">The disposing value.</param>
    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            base.Dispose(disposing);
            return;
        }

        _chartSettings.Dispose();
        base.Dispose(disposing);
    }
}
