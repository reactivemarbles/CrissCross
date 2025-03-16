// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using Color = ScottPlot.Color;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Nice for continuous live data.
/// </summary>
/// <seealso cref="AxisLinesUI" />
public partial class AxisLinesUI : RxObject
{
    [Reactive]
    private Settings _chartSettings = new Settings();

    [Reactive]
    private string _lineOrientation;

    [Reactive]
    private int _axis;

    [Reactive]
    private string _labelText;

    [Reactive]
    private LinePattern _linePattern1;

    /// <summary>
    /// Initializes a new instance of the <see cref="AxisLinesUI" /> class.
    /// </summary>
    /// <param name="plot">if set to <c>true</c> [paused].</param>
    /// <param name="observable">The observable.</param>
    /// <param name="orientation">The orientation ["Horizontal"] or ["Vertical"].</param>
    /// <param name="axis">The axis.</param>
    /// <param name="color">The color.</param>
    /// <param name="text">The text.</param>
    /// <param name="linePattern">The line pattern.</param>
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
            AppearanceSubsriptions();
        }
        else if (orientation == "Vertical")
        {
            CreateVerticalLine();
            UpdateAxisLineSubscription(observable);
            AppearanceSubsriptions();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AxisLinesUI" /> class.
    /// </summary>
    /// <param name="plot">if set to <c>true</c> [paused].</param>
    /// <param name="position">The position.</param>
    /// <param name="linePattern">The line pattern.</param>
    /// <param name="type">The type.</param>
    /// <param name="axis">The axis.</param>
    /// <param name="color">The color.</param>
    /// <param name="text">The text.</param>
    public AxisLinesUI(
        WpfPlot plot,
        double position,
        LinePattern linePattern,
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
            AppearanceSubsriptions();
        }
        else if (type == "Vertical")
        {
            CreateVerticalLine(position);
            AppearanceSubsriptions();
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
    public AxisLine? AxisLine { get; set; }

    /// <summary>
    /// Creates the stream.
    /// </summary>
    /// <param name="position">The position.</param>
    public void CreateVerticalLine(double position = 0.0)
    {
        if (string.IsNullOrWhiteSpace(ChartSettings.Color))
        {
            return;
        }

        var color = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(ChartSettings.Color));
        AxisLine = Plot.Plot.Add.VerticalLine(x: position, width: (float)ChartSettings.LineWidth, color: color);
        ////AxisLine.Text = LabelText;
        AxisLine.LabelText = LabelText;
        ////AxisLine.LabelText = LabelText;
        ////AxisLine.LabelBackgroundColor = color;
    }

    /// <summary>
    /// Creates the stream.
    /// </summary>
    /// <param name="position">The position.</param>
    public void CreateHorizontalLine(double position = 0.0)
    {
        if (string.IsNullOrWhiteSpace(ChartSettings.Color))
        {
            return;
        }

        var color = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(ChartSettings.Color));
        AxisLine = Plot.Plot.Add.HorizontalLine(y: position, width: (float)ChartSettings.LineWidth, color: color);
        ////AxisLine.Text = LabelText;
        AxisLine.LabelText = LabelText;
        ////AxisLine.LabelText = LabelText;
        AxisLine.LabelAlignment = Alignment.MiddleCenter;
        AxisLine.LinePattern = LinePattern1;
        ////AxisLine.LabelBackgroundColor = color;
    }

    /// <summary>
    /// Updates the stream.
    /// </summary>
    /// <param name="observable">The observable.</param>
    public void UpdateAxisLineSubscription(IObservable<(string? Name, double? Position)> observable) => observable
        .SubscribeOn(RxApp.TaskpoolScheduler) // Procesa en un hilo de fondo
        .ObserveOn(RxApp.MainThreadScheduler) // Actualiza la UI en el hilo principal
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

    /// <summary>
    /// Appearances the subsriptions.
    /// </summary>
    private void AppearanceSubsriptions()
    {
        this.WhenAnyValue(x => x.ChartSettings.LineWidth, x => x.ChartSettings.Color, x => x.ChartSettings.Visibility).Subscribe(x =>
        {
            AxisLine!.LineStyle.Width = (float)x.Item1;
            AxisLine!.Color = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(x.Item2!));
            ChartSettings.IsChecked = x.Item3 == "Invisible";
            AxisLine.IsVisible = x.Item3 != "Invisible";
            Plot.Refresh();
        }).DisposeWith(Disposables);
        this.WhenAnyValue(x => x.ChartSettings.IsChecked)
            .Subscribe(x => ChartSettings.Visibility = x == true ? "Invisible" : "Visible")
            .DisposeWith(Disposables);
    }
}
