// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Threading;
using ScottPlot.Plottables;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.Plot;
using ReactiveUI.Reactive;
using ReactiveUI.Reactive.Builder;
#else
using CrissCross.WPF.Plot;
using ReactiveUI;
using ReactiveUI.Builder;
#endif

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Verifies editable settings through the public WPF reactive binding surface.</summary>
[NotInParallel]
public sealed class ReactivePlotSeriesSettingsTests
{
    /// <summary>The stable series name.</summary>
    private const string SeriesName = "series";

    /// <summary>The editable series label.</summary>
    private const string EditedLabel = "edited";

    /// <summary>The second sample.</summary>
    private const double SecondValue = 2;

    /// <summary>The third sample.</summary>
    private const double ThirdValue = 3;

    /// <summary>The fourth sample.</summary>
    private const double FourthValue = 4;

    /// <summary>The default marker size.</summary>
    private const double DefaultMarkerSize = 5;

    /// <summary>The edited width.</summary>
    private const float EditedLineWidth = 7;

    /// <summary>The edited marker size.</summary>
    private const float EditedMarkerSize = 9;

    /// <summary>The custom baseline.</summary>
    private const double CustomBaseline = -5;

    /// <summary>The paused sample.</summary>
    private const double PausedValue = 99;

    /// <summary>The second sequence.</summary>
    private const long SecondSequence = 2;

    /// <summary>The third sequence.</summary>
    private const long ThirdSequence = 3;

    /// <summary>Verifies invalid numeric edits preserve the last accepted value.</summary>
    /// <param name="value">The invalid size.</param>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [Arguments(-1D)]
    [Arguments(double.NaN)]
    [Arguments(double.PositiveInfinity)]
    [Arguments(double.MaxValue)]
    public async Task Size_InvalidValue_PreservesSettings(double value)
    {
        using var settings = new ReactivePlotSeriesSettings(new(SeriesName, 0), PlotType.Line, "Red");
        await Assert.That(() => settings.LineWidth = value).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => settings.MarkerSize = value).Throws<ArgumentOutOfRangeException>();
        await Assert.That(settings.LineWidth).IsEqualTo(SecondValue);
        await Assert.That(settings.MarkerSize).IsEqualTo(DefaultMarkerSize);
    }

    /// <summary>Verifies invalid color, period, baseline, and retention edits are rejected.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Settings_InvalidInputs_AreRejected()
    {
        using var settings = new ReactivePlotSeriesSettings(new(SeriesName, 0), PlotType.Points, "Red");
        await Assert.That(() => settings.Color = "not-a-color").Throws<ArgumentException>();
        await Assert.That(() => settings.Color = null!).Throws<ArgumentNullException>();
        await Assert.That(() => settings.SeriesLabel = null!).Throws<ArgumentNullException>();
        await Assert.That(() => settings.SamplePeriod = 0).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => settings.SamplePeriod = double.NaN).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => settings.Baseline = double.PositiveInfinity).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => settings.MaxPoints = 0).Throws<ArgumentOutOfRangeException>();
        await Assert.That(settings.LineMode).IsEqualTo(PlotLineMode.MarkersOnly);
        await Assert.That(settings.Color).IsEqualTo("Red");
    }

    /// <summary>Verifies presentation edits survive source defaults and hiding retains new data.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Bind_StyleEditsAndVisibility_PersistAcrossUpdates()
    {
        var observed = await OnSta(static () =>
        {
            using var vm = new LiveChartViewModel(new Grid());
            using var updates = new Signal<ReactivePlotUpdate>();
            using var connection = Bind(vm, updates, PlotType.Line);
            updates.OnNext(Update(PlotType.Line, 1, [1D], [SecondValue]));
            var settings = vm.SeriesSettings[0];
            settings.Color = "Blue";
            settings.LineWidth = EditedLineWidth;
            settings.MarkerSize = EditedMarkerSize;
            settings.LineMode = PlotLineMode.LineAndMarkers;
            settings.SeriesLabel = EditedLabel;
            settings.ShowInLegend = false;
            settings.IsVisible = false;
            updates.OnNext(Update(PlotType.Line, SecondSequence, [SecondValue], [ThirdValue]) with { Style = new() { Color = "Red", LineWidth = 1 } });
            var scatter = GetScatter(vm);
            var hidden = !scatter.IsVisible;
            settings.IsVisible = true;
            return (hidden, scatter.IsVisible, scatter.LineWidth, scatter.MarkerSize, scatter.LineColor, scatter.GetAxisLimits().Top, settings.SeriesLabel, settings.ShowInLegend);
        });
        await Assert.That(observed.hidden).IsTrue();
        await Assert.That(observed.IsVisible).IsTrue();
        await Assert.That(observed.LineWidth).IsEqualTo(EditedLineWidth);
        await Assert.That(observed.MarkerSize).IsEqualTo(EditedMarkerSize);
        await Assert.That(observed.LineColor).IsEqualTo(ScottPlot.Color.FromColor(System.Drawing.Color.Blue));
        await Assert.That(observed.Top).IsEqualTo(ThirdValue);
        await Assert.That(observed.SeriesLabel).IsEqualTo(EditedLabel);
        await Assert.That(observed.ShowInLegend).IsFalse();
    }

    /// <summary>Verifies pause ignores incoming values and resume accepts the next update.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Bind_PauseAndResume_ControlDataUpdates()
    {
        var observed = await OnSta(static () =>
        {
            using var vm = new LiveChartViewModel(new Grid());
            using var updates = new Signal<ReactivePlotUpdate>();
            using var connection = Bind(vm, updates, PlotType.Line);
            updates.OnNext(Update(PlotType.Line, 1, [1D], [SecondValue]) with { Style = new() });
            vm.SeriesSettings[0].IsPaused = true;
            updates.OnNext(Update(PlotType.Line, SecondSequence, [SecondValue], [PausedValue]));
            var paused = GetScatter(vm).GetAxisLimits().Top;
            vm.SeriesSettings[0].IsPaused = false;
            updates.OnNext(Update(PlotType.Line, ThirdSequence, [ThirdValue], [FourthValue]));
            return (paused, GetScatter(vm).GetAxisLimits().Top);
        });
        await Assert.That(observed.paused).IsEqualTo(SecondValue);
        await Assert.That(observed.Top).IsEqualTo(FourthValue);
    }

    /// <summary>Verifies area baseline edits apply immediately and survive new source values.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Bind_AreaBaseline_UpdatesRenderedFill()
    {
        var observed = await OnSta(static () =>
        {
            using var vm = new LiveChartViewModel(new Grid());
            using var updates = new Signal<ReactivePlotUpdate>();
            using var connection = Bind(vm, updates, PlotType.Area);
            updates.OnNext(Update(PlotType.Area, 1, [1D], [SecondValue]));
            var defaultFill = GetScatter(vm).FillY;
            vm.SeriesSettings[0].Baseline = CustomBaseline;
            vm.SeriesSettings[0].BaselineMode = PlotBaselineMode.Custom;
            updates.OnNext(Update(PlotType.Area, SecondSequence, [SecondValue], [ThirdValue]));
            var area = GetScatter(vm);
            var custom = (area.FillY, area.FillYValue);
            vm.SeriesSettings[0].BaselineMode = PlotBaselineMode.None;
            return (defaultFill, custom.FillY, custom.FillYValue, Disabled: !area.FillY);
        });
        await Assert.That(observed.defaultFill).IsTrue();
        await Assert.That(observed.FillY).IsTrue();
        await Assert.That(observed.FillYValue).IsEqualTo(-DefaultMarkerSize);
        await Assert.That(observed.Disabled).IsTrue();
    }

    /// <summary>Verifies static display-window edits redraw and restore without another source notification.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Bind_StaticDisplayWindowEdit_RedrawsImmediately()
    {
        var observed = await OnSta(static () =>
        {
            using var vm = new LiveChartViewModel(new Grid());
            using var updates = new Signal<ReactivePlotUpdate>();
            using var connection = Bind(vm, updates, PlotType.Line);
            updates.OnNext(Update(PlotType.Line, 1, [1D, SecondValue, ThirdValue], [1D, SecondValue, ThirdValue]) with { Kind = ReactivePlotUpdateKind.Replace });
            vm.NumberPointsPlotted = 1;
            vm.UseFixedNumberOfPoints = true;
            var trimmed = GetScatter(vm).GetAxisLimits();
            vm.UseFixedNumberOfPoints = false;
            return (trimmed.Left, trimmed.Right, RestoredLeft: GetScatter(vm).GetAxisLimits().Left);
        });
        await Assert.That(observed.Left).IsEqualTo(ThirdValue);
        await Assert.That(observed.Right).IsEqualTo(ThirdValue);
        await Assert.That(observed.RestoredLeft).IsEqualTo(1D);
    }

    /// <summary>Verifies editable retention trims snapshots immediately and keeps only the newest incoming points.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Bind_RetentionEdit_TrimsCurrentAndFutureData()
    {
        var observed = await OnSta(static () =>
        {
            using var vm = new LiveChartViewModel(new Grid());
            using var updates = new Signal<ReactivePlotUpdate>();
            double immediate;
            ScottPlot.AxisLimits final;
            using (var connection = Bind(vm, updates, PlotType.Line))
            {
                updates.OnNext(Update(PlotType.Line, 1, [1D, SecondValue, ThirdValue], [1D, SecondValue, ThirdValue]));
                vm.SeriesSettings[0].MaxPoints = 1;
                immediate = GetScatter(vm).GetAxisLimits().Left;
                updates.OnNext(Update(PlotType.Line, SecondSequence, [FourthValue], [FourthValue]));
                final = GetScatter(vm).GetAxisLimits();
            }

            return (immediate, final.Left, final.Right, vm.SeriesSettings.Count, Removed: !updates.HasObservers);
        });
        await Assert.That(observed.immediate).IsEqualTo(ThirdValue);
        await Assert.That(observed.Left).IsEqualTo(FourthValue);
        await Assert.That(observed.Right).IsEqualTo(FourthValue);
        await Assert.That(observed.Count).IsEqualTo(0);
        await Assert.That(observed.Removed).IsTrue();
    }

    /// <summary>Verifies the first real update supplies defaults even when a clear arrives first.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Bind_ClearBeforeData_InitializesFirstStyle()
    {
        var observed = await OnSta(static () =>
        {
            using var vm = new LiveChartViewModel(new Grid());
            using var updates = new Signal<ReactivePlotUpdate>();
            using var connection = Bind(vm, updates, PlotType.Area);
            updates.OnNext(Update(PlotType.Area, 1, [], []) with { Kind = ReactivePlotUpdateKind.Clear });
            updates.OnNext(Update(PlotType.Area, SecondSequence, [1D], [SecondValue]) with
            {
                MaxPoints = 1,
                Style = new()
                {
                    Color = "Blue",
                    LineWidth = EditedLineWidth,
                    MarkerSize = EditedMarkerSize,
                    LineMode = PlotLineMode.Hidden,
                    BaselineMode = PlotBaselineMode.Custom,
                    Baseline = CustomBaseline,
                    ShowInLegend = false,
                },
            });
            var settings = vm.SeriesSettings[0];
            settings.IsCrossHairVisible = true;
            var area = GetScatter(vm);
            return (settings.SeriesName, settings.IsCrossHairVisible, settings.MaxPoints, area.IsVisible, area.LineWidth, area.MarkerSize, area.FillYValue, settings.ShowInLegend);
        });
        await Assert.That(observed.SeriesName).IsEqualTo(SeriesName);
        await Assert.That(observed.IsCrossHairVisible).IsTrue();
        await Assert.That(observed.MaxPoints).IsEqualTo(1);
        await Assert.That(observed.IsVisible).IsFalse();
        await Assert.That(observed.LineWidth).IsEqualTo(EditedLineWidth);
        await Assert.That(observed.MarkerSize).IsEqualTo(EditedMarkerSize);
        await Assert.That(observed.FillYValue).IsEqualTo(CustomBaseline);
        await Assert.That(observed.ShowInLegend).IsFalse();
    }

    /// <summary>Verifies UI-backed series keep presentation edits and ignore data while paused.</summary>
    /// <param name="type">The UI-backed chart type.</param>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [Arguments(PlotType.Signal)]
    [Arguments(PlotType.Scatter)]
    [Arguments(PlotType.DataLogger)]
    [Arguments(PlotType.SignalXY)]
    public async Task Bind_UiSeries_PreservesPresentationAndPause(PlotType type)
    {
        var observed = await OnSta(() =>
        {
            using var vm = new LiveChartViewModel(new Grid());
            using var updates = new Signal<ReactivePlotUpdate>();
            using var connection = Bind(vm, updates, type);
            updates.OnNext(Update(type, 1, [1D], [SecondValue]));
            var settings = vm.SeriesSettings[0];
            settings.Color = "Blue";
            settings.LineWidth = EditedLineWidth;
            settings.SeriesLabel = EditedLabel;
            settings.IsCrossHairVisible = true;
            settings.IsVisible = false;
            updates.OnNext(Update(type, SecondSequence, [SecondValue], [ThirdValue]));
            var ui = vm.PlotLinesCollectionUI[0];
            var plottable = ui switch
            {
                SignalUI signal => (ScottPlot.IPlottable)signal.PlotLine!,
                ScatterUI scatter => scatter.PlotLine!,
                DataLoggerUI logger => logger.PlotLine!,
                SignalXY_UI signalXy => signalXy.PlotLine!,
                _ => throw new InvalidOperationException("Expected UI series."),
            };
            var hidden = !plottable.IsVisible;
            settings.IsVisible = true;
            settings.IsPaused = true;
            updates.OnNext(Update(type, ThirdSequence, [ThirdValue], [PausedValue]));
            return (hidden, plottable.IsVisible, ui.ChartSettings.ItemName, ui.ChartSettings.IsCrossHairVisible, ((ScottPlot.IHasLine)plottable).LineStyle.Width, plottable.GetAxisLimits().Top);
        });
        await Assert.That(observed.hidden).IsTrue();
        await Assert.That(observed.IsVisible).IsTrue();
        await Assert.That(observed.ItemName).IsEqualTo(EditedLabel);
        await Assert.That(observed.IsCrossHairVisible).IsTrue();
        await Assert.That(observed.Width).IsEqualTo(EditedLineWidth);
        await Assert.That(observed.Top).IsEqualTo(ThirdValue);
    }

    /// <summary>Verifies fixed-buffer sample intervals and capacities survive incoming updates in every mode.</summary>
    /// <param name="mode">The selected scrolling mode.</param>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [Arguments(StreamerViewMode.ScrollLeft)]
    [Arguments(StreamerViewMode.ScrollRight)]
    [Arguments(StreamerViewMode.WipeLeft)]
    [Arguments(StreamerViewMode.WipeRight)]
    public async Task Bind_StreamerSettings_PersistAcrossUpdates(StreamerViewMode mode)
    {
        var observed = await OnSta(() =>
        {
            using var vm = new LiveChartViewModel(new Grid());
            vm.NumberPointsPlotted = 1;
            using var updates = new Signal<ReactivePlotUpdate>();
            using var connection = Bind(vm, updates, PlotType.Streamer);
            updates.OnNext(Update(PlotType.Streamer, 1, [1D], [SecondValue]));
            var settings = vm.SeriesSettings[0];
            settings.MaxPoints = (int)ThirdValue;
            settings.SamplePeriod = SecondValue;
            settings.StreamerViewMode = mode;
            updates.OnNext(Update(PlotType.Streamer, SecondSequence, [SecondValue, ThirdValue, FourthValue], [SecondValue, ThirdValue, FourthValue]));
            var streamer = ((StreamerUI)vm.PlotLinesCollectionUI[0]).PlotLine!;
            return (streamer.Period, streamer.Data.Length, streamer.Data.NewestPoint, settings.StreamerViewMode);
        });
        await Assert.That(observed.Period).IsEqualTo(SecondValue);
        await Assert.That(observed.Length).IsEqualTo((int)ThirdValue);
        await Assert.That(observed.NewestPoint).IsEqualTo(FourthValue);
        await Assert.That(observed.StreamerViewMode).IsEqualTo(mode);
    }

    /// <summary>Gets the rendered scatter series.</summary>
    /// <param name="vm">The chart.</param>
    /// <returns>The series.</returns>
    private static Scatter GetScatter(LiveChartViewModel vm)
    {
        foreach (var item in vm.WpfPlot1vm!.Plot.PlottableList)
        {
            if (item is Scatter scatter)
            {
                return scatter;
            }
        }

        throw new InvalidOperationException("Expected rendered scatter.");
    }

    /// <summary>Creates a public binding with deterministic notification dispatch.</summary>
    /// <param name="vm">The chart.</param>
    /// <param name="updates">The update stream.</param>
    /// <param name="type">The chart type.</param>
    /// <returns>The owned connection.</returns>
    private static IReactivePlotConnection Bind(LiveChartViewModel vm, Signal<ReactivePlotUpdate> updates, PlotType type) =>
        new ReactivePlotBinder().Bind(vm, [ReactivePlotSource.FromUpdates(new(SeriesName, 0), type, updates)], new() { UiScheduler = ImmediateScheduler.Instance });

    /// <summary>Creates a numeric append update.</summary>
    /// <param name="type">The chart type.</param>
    /// <param name="sequence">The update sequence.</param>
    /// <param name="x">The X values.</param>
    /// <param name="y">The Y values.</param>
    /// <returns>The update.</returns>
    private static ReactivePlotUpdate Update(PlotType type, long sequence, double[] x, double[] y) =>
        new(new(SeriesName, 0), type, ReactivePlotUpdateKind.Append, x, y, PlotXAxisKind.Numeric, sequence);

    /// <summary>Runs chart work on an STA thread with immediate reactive notifications.</summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="action">The chart work.</param>
    /// <returns>The chart result.</returns>
    private static Task<T> OnSta<T>(Func<T> action)
    {
        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(() =>
        {
            _ = Dispatcher.CurrentDispatcher;
            _ = RxAppBuilder.CreateReactiveUIBuilder().WithWpf().BuildApp();
            var previous = RxSchedulers.MainThreadScheduler;
            var previousTaskpool = RxSchedulers.TaskpoolScheduler;
            RxSchedulers.MainThreadScheduler = ImmediateScheduler.Instance;
            RxSchedulers.TaskpoolScheduler = ImmediateScheduler.Instance;
            try
            {
                completion.SetResult(action());
            }
            catch (Exception exception)
            {
                completion.SetException(exception);
            }
            finally
            {
                RxSchedulers.MainThreadScheduler = previous;
                RxSchedulers.TaskpoolScheduler = previousTaskpool;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return completion.Task;
    }
}
