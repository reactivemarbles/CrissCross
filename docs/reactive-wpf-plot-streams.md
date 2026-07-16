# Reactive WPF.Plot streams

`CrissCross.WPF.Plot` supports one observable-first binding surface for static, historic, and live charts. The API normalizes data into `IReactivePlotSource` instances and binds them through `ReactivePlotBinder`, which owns subscription lifetime, UI-scheduler marshalling, validation, batching, visible-point retention, completion, and error state.

## Minimal usage

```csharp
var signalPoints = new Subject<(string? Name, IList<double>? Value, IList<double> X, int Axis)>();
var scatterPoints = new Subject<(string? Name, IList<double>? X, IList<double> Y, int Axis)>();
var loggerPoints = new Subject<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)>();
var streamerPoints = new Subject<(string? Name, IList<double>? Y, IList<double> X, int Axis)>();
var signalXyPoints = new Subject<(string? Name, IList<double>? Y, IList<double> X, int Axis)>();

Chart.ReactivePlotSources =
[
    ReactivePlotSource.FromSignalPoints(signalPoints),
    ReactivePlotSource.FromScatterPoints(scatterPoints),
    ReactivePlotSource.FromDataLoggerPoints(loggerPoints),
    ReactivePlotSource.FromStreamerPoints(streamerPoints),
    ReactivePlotSource.FromSignalXyPoints(signalXyPoints),
];
```

Each source emits one logical series. Series identity is derived from the emitted name and axis, so subsequent updates reuse the same adapter instead of rebuilding unrelated series.

## Update behavior by chart type

| Chart type | Factory | Update mode | X-axis kind |
|---|---|---|---|
| Signal | `ReactivePlotSource.FromSignalTicks(...)` | Append | Ticks |
| Signal | `ReactivePlotSource.FromSignalPoints(...)` | Append | Numeric |
| Scatter | `ReactivePlotSource.FromScatterPoints(...)` | Replace | Numeric |
| DataLogger | `ReactivePlotSource.FromDataLoggerPoints(...)` | Append | Numeric ordinal |
| Streamer | `ReactivePlotSource.FromStreamerPoints(...)` | Append | Numeric |
| SignalXY | `ReactivePlotSource.FromSignalXyPoints(...)` | Replace | Numeric |
| Line | `ReactivePlotSource.FromStatic(...)` | Replace | Numeric, OADate, or ticks |
| Step line | `ReactivePlotSource.FromStatic(...)` | Replace | Numeric, OADate, or ticks |
| Area | `ReactivePlotSource.FromStatic(...)` | Replace | Numeric, OADate, or ticks |
| Bar | `ReactivePlotSource.FromStatic(...)` | Replace | Numeric, OADate, or ticks |
| Stem | `ReactivePlotSource.FromStatic(...)` | Replace | Numeric, OADate, or ticks |
| Points | `ReactivePlotSource.FromStatic(...)` | Replace | Numeric, OADate, or ticks |

## Static, DateTime, and historic data

Use `PlotSeriesData.Numeric(...)` for double/double data and `PlotSeriesData.DateTime(...)` for DateTime/double data. DateTime values are normalized to ScottPlot-compatible OLE Automation dates.

```csharp
var staticSeries = PlotSeriesData.Numeric("Reference", 0, x, y);
var staticSource = ReactivePlotSource.FromStatic(staticSeries, PlotType.Area);

var history = PlotSeriesData.DateTime("Ten-year trend", 0, timestamps, values);
var historicSource = ReactivePlotSource.FromHistoric(
    history,
    targetPointCount: 2_000,
    plotType: PlotType.Line);
```

`FromHistoric` uses Largest-Triangle-Three-Buckets reduction to preserve extrema and overall shape while bounding the number of points sent to the renderer. The first and final timestamps are retained.

For typed live streams, use `ReactivePlotSource.FromLive(...)` with `PlotDataPoint` or `ReactivePlotSource.FromDateTimeLive(...)` with `(DateTime Timestamp, double Value)` values. Both factories support per-series rolling retention.

## Technical studies

`TechnicalIndicators` calculates pure static SMA, EMA, MACD, RSI, Ichimoku, and Bollinger Band results. `ReactivePlotStudies` composes the same studies over any `IReactivePlotSource` and returns ordinary sources that participate in the same lifecycle, scheduling, batching, styling, and axis assignment as primary series.

```csharp
var price = ReactivePlotSource.FromStatic(priceData);
var sources = new List<IReactivePlotSource>
{
    price,
    ReactivePlotStudies.SimpleMovingAverage(price, 20),
    ReactivePlotStudies.ExponentialMovingAverage(price, 12),
    ReactivePlotStudies.RelativeStrengthIndex(price, axis: 1),
};
sources.AddRange(ReactivePlotStudies.BollingerBands(price));
sources.AddRange(ReactivePlotStudies.MovingAverageConvergenceDivergence(price, axis: 1));
sources.AddRange(ReactivePlotStudies.Ichimoku(price));
```

## Styling and interaction

`ReactivePlotSeriesStyle` controls color, line width, marker size, line/marker visibility, legend inclusion, and zero/custom area baselines. `ReactivePlotTheme.Dark`, `ReactivePlotTheme.Light`, or an application-defined `ReactivePlotTheme` can be applied with `LiveChartViewModel.ApplyTheme(...)`.

ScottPlot's native mouse input supplies responsive zoom and pan. `LiveChart` adds nearest-point hover labels, markers, crosshairs, movable axis lines, automatic/manual scaling, per-series visibility, and multiple Y axes. These features remain active for reactive sources because the adapters assign every generated plottable to the chart's shared axes and interaction surface.

## Lifecycle and errors

`ReactivePlotBinder.Bind(...)` returns an `IReactivePlotConnection`:

- `State` emits `Connecting`, `Active`, `Completed`, `Faulted`, and `Disposed` transitions.
- `Errors` surfaces source exceptions and validation failures.
- `Dispose()` terminates source subscriptions and disposes chart adapters.
- `ReactivePlotErrorMode.SurfaceAndStopSeries` faults the connection and prevents later invalid-stream emissions from mutating the chart.
- `ReactivePlotErrorMode.IgnoreInvalidUpdates` skips invalid samples without creating adapters.

## Scheduling, batching, and retention

By default, updates are marshalled to `RxSchedulers.MainThreadScheduler` and are not batched, so hot live sources appear immediately. Configure batching only when needed:

```csharp
var options = new ReactivePlotBindingOptions
{
    BatchWindow = TimeSpan.FromMilliseconds(16),
    MaxBatchSize = 128,
    MaxVisiblePoints = 10_000,
    OverflowStrategy = ReactivePlotOverflowStrategy.DropOldest,
};
```

Use `UiScheduler` in tests or specialized hosts to control marshalling deterministically. Use `SourceScheduler` only when source subscription work needs to move off the caller thread.

## Example project

The WPF plot example at `src/CrissCross.WPF.Plot.Test` demonstrates all eleven chart types, rolling live streams, a reduced 150,000-point DateTime history, every technical study, and dark/light plot themes. Scenario and theme commands are bound with ReactiveUI; no imperative data binding is performed from the window.
