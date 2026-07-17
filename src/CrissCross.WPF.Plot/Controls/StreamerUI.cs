// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
#if !REACTIVE_SHIM
using ReactiveUI;
#endif
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>
/// Provides a user interface component for streaming and visualizing real-time data on a plot, supporting dynamic
/// scaling and configurable display options.
/// </summary>
/// <remarks>This class is intended for use on Windows platforms and integrates with reactive data sources to
/// update plots in real time. It supports automatic and manual scaling, fixed or variable point display, and
/// customizable appearance through chart settings. Thread safety is managed internally for UI updates. Dispose of
/// instances when no longer needed to release resources associated with chart settings and subscriptions.</remarks>
[SupportedOSPlatform("windows")]
public partial class StreamerUI : RxObject, IPlottableUI
{
    /// <summary>Stores the value buffer value.</summary>
    private readonly double[] _valueBuffer;

    /// <summary>Stores the sample count value.</summary>
    private readonly uint _sampleCount = 1;

    /// <summary>Stores the fs value.</summary>
    private readonly int _fs = 1;

    /// <summary>Stores the number points plotted saved value.</summary>
    private readonly int _numberPointsPlottedSaved;

    /// <summary>Stores the chart settings value.</summary>
    [Reactive]
    private ChartObjects _chartSettings = new();

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

    /// <summary>Initializes a new instance of the <see cref="StreamerUI"/> class.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="observable">The observable value.</param>
    /// <param name="fs">The sampling frequency, in Hz, used for interpreting the data stream.</param>
    /// <param name="sampleCount">The number of samples to buffer for the streaming plot. Must be greater than
    /// 0.</param>
    /// <param name="plottedPointCount">The initial number of data points to display on the plot.</param>
    /// <param name="color">The color used for the plot line and chart appearance.</param>
    public StreamerUI(
        WpfPlot plot,
        IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> observable,
        int fs,
        uint sampleCount,
        int plottedPointCount,
        string color)
        : this(plot, observable, fs, sampleCount, plottedPointCount, color, new StreamerUIOptions())
    {
    }

    /// <summary>Initializes a new instance of the <see cref="StreamerUI"/> class.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="observable">The observable value.</param>
    /// <param name="fs">The sampling frequency.</param>
    /// <param name="sampleCount">The sample count.</param>
    /// <param name="plottedPointCount">The plotted point count.</param>
    /// <param name="color">The plot color.</param>
    /// <param name="autoscale">A value indicating whether automatic scaling is enabled.</param>
    public StreamerUI(
        WpfPlot plot,
        IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> observable,
        int fs,
        uint sampleCount,
        int plottedPointCount,
        string color,
        bool autoscale)
        : this(
            plot,
            observable,
            fs,
            sampleCount,
            plottedPointCount,
            color,
            new StreamerUIOptions { AutoScale = autoscale })
    {
    }

    /// <summary>Initializes a new instance of the <see cref="StreamerUI"/> class.</summary>
    /// <param name="plot">The plot value.</param>
    /// <param name="observable">The observable value.</param>
    /// <param name="fs">The sampling frequency.</param>
    /// <param name="sampleCount">The sample count.</param>
    /// <param name="plottedPointCount">The plotted point count.</param>
    /// <param name="color">The plot color.</param>
    /// <param name="options">The streamer configuration.</param>
    public StreamerUI(
        WpfPlot plot,
        IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> observable,
        int fs,
        uint sampleCount,
        int plottedPointCount,
        string color,
        StreamerUIOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        ChartSettings = new("---", color);
        ManualScale = options.ManualScale;
        AutoScale = options.AutoScale;
        UseFixedNumberOfPoints = options.FixedPoints;

        if (sampleCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sampleCount), "sampleCount must be greater than 0");
        }

        _sampleCount = sampleCount;
        _fs = fs;
        _numberPointsPlottedSaved = plottedPointCount;
        _valueBuffer = new double[plottedPointCount];

        Plot = plot;

        CreateStreamer(color);

        // Set name from first emission of the observable
        _ = observable
            .Take(1)
            .Where(d => !string.IsNullOrEmpty(d.Name))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(data => ChartSettings.ItemName = data.Name!)
            .DisposeWith(Disposables);

        UpdateStreamerFixedPoints(observable);
        ChartSettings.CreateCursorValues(Plot, color);
        ChartSettings.AppearanceSubsriptions(Plot, PlotLine!);
    }

    /// <summary>Gets or sets the WPF plot control used to display graphical data within the application.</summary>
    /// <remarks>Assign this property to embed or update the plot visualization in a WPF user interface. The
    /// property should be set before interacting with plot-specific features or rendering data.</remarks>
    public WpfPlot Plot { get; set; }

    /// <summary>Gets or sets the data streamer used to plot line data in the visualization.</summary>
    public DataStreamer? PlotLine { get; set; }

    /// <summary>Initializes a new data streamer line on the plot using the specified color.</summary>
    /// <remarks>The method configures the streamer line with default width and period settings, and scrolls
    /// the view to the left to display the most recent data points. The color parameter must be a valid hexadecimal
    /// color code; otherwise, an exception may be thrown by the color parsing logic.</remarks>
    /// <param name="color">A string representing the color of the streamer line, specified in hexadecimal format (e.g.,
    /// "#FF0000" for red).</param>
    public void CreateStreamer(string color)
    {
        PlotLine = Plot.Plot.Add.DataStreamer(_numberPointsPlottedSaved);
        var darray = new double[_numberPointsPlottedSaved];
        PlotLine.Data = new(darray);
        PlotLine.ViewScrollLeft();
        PlotLine.Period = (double)_fs / _sampleCount;
        PlotLine.LineStyle.Width = 1F;
        PlotLine.Color = Color.FromHex(color);
    }

    /// <summary>Provides the UpdateStreamerFixedPoints member.</summary>
    /// <param name="observable">The observable value.</param>
    public void UpdateStreamerFixedPoints(
        IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)> observable) =>
        observable
            .ObserveOn(RxSchedulers.TaskpoolScheduler)
            .Where(d =>
                !string.IsNullOrEmpty(d.Name)
                && d.Y is not null
                && d.X is not null
                && d.Y.Count > 0
                && d.X.Count > 0
                && d.Y.Count == d.X.Count)
            .Retry(int.MaxValue)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(d =>
            {
                var sourceList = d.Y!;
                var count = Math.Min(sourceList.Count, _numberPointsPlottedSaved);

                // Copy to pre-allocated buffer to avoid allocations
                for (var i = 0; i < count; i++)
                {
                    _valueBuffer[i] = sourceList[i];
                }

                // For DataStreamer, we need to provide an IEnumerable<double>
                // Use ArraySegment which implements IEnumerable without allocation
                var segment = new ArraySegment<double>(_valueBuffer, 0, count);
                PlotLine!.AddRange(segment);
                PlotLine!.ManageAxisLimits = false;

                if (ChartSettings.IsPaused)
                {
                    return;
                }

                Plot.Refresh();
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

        ChartSettings.IsCheckedCmd?.Dispose();
        _chartSettings.Dispose();
        base.Dispose(disposing);
    }
}
