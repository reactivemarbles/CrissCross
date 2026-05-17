# Reactive WPF.Plot streams

`CrissCross.WPF.Plot` supports an observable-first binding surface for live charts. The API normalizes existing tuple-based chart streams into `IReactivePlotSource` instances and binds them through `ReactivePlotBinder`, which owns subscription lifetime, UI-scheduler marshalling, validation, batching, visible-point retention, completion, and error state.

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

The WPF plot example at `src/CrissCross.WPF.Plot.Test` demonstrates all five chart types through `MainViewModel.ReactivePlotSources`. It uses one numeric X-axis scale for the live demo so all series render together, starts live demo subjects when the chart view model is attached, and disposes the interval subscription with the view model.
