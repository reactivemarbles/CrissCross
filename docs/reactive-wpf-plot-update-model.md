# Reactive WPF.Plot chart update model discovery/spec

## Scope

This document is the discovery and implementation specification for adding an IObservable-first chart update model to `CrissCross.WPF.Plot`. It covers the existing WPF plot control surface, current chart types, update behavior, missing functionality, threading/dispatcher constraints, allocation hot spots, proposed public API shape, and a failing TUnit test plan for the implementation card.

This card intentionally does not implement production plot changes. The API and tests below are intended to drive the next implementation slice with RED-GREEN-REFACTOR.

## Current chart type inventory

`PlotType` exposes five runtime chart categories:

| Chart type | Current UI type | ScottPlot plottable | Primary current update path |
|---|---|---|---|
| `Signal` | `SignalUI` | `DataLogger` | `UpdateSignal(IObservable<(Name, Value, DateTime, Axis)>)` |
| `Scatter` | `ScatterUI` | `Scatter` | `UpdateScatter(IObservable<(Name, X, Y, Axis)>)` |
| `DataLogger` | `DataLoggerUI` | `DataLogger` | `UpdateDataLogger(IObservable<(Name, Value, Axis, nPoints)>)` |
| `Streamer` | `StreamerUI` | `DataStreamer` | `UpdateStreamerFixedPoints(IObservable<(Name, Y, X, Axis)>)` |
| `SignalXY` | `SignalXY_UI` | `SignalXY` | constructor snapshot only; no observable update path |

`UserPlotType` currently defines eight user-facing shapes:

- `SignalEnumObsTicks`
- `DataLoggerEnumObsPoints`
- `SignalXYTimestamp`
- `SignalXYPoints`
- `SignalXYEnumPoints`
- `StreamerEnumObsPoints`
- `ScatterEnumObsPoints`
- `ScatterPoints`

The current public data surface is split across dependency properties on `LiveChart`:

- `ScatterObservablesWithTimeStamp`
- `SignalObservablesWithTimeStamp`
- `DataLoggerObservablesWithPoints`
- `DataWithTimeStamp`
- `SignalWithPoints`
- `SignalsWithPoints`
- `SignalObservablesWithPoints`
- `ScatterWithPoints`

`LiveChart.AssignLiveChartData(object source, UserPlotType type)` dispatches to these shapes with runtime type checks.

## Current implementation observations

### Chart creation and reinitialization

- `LiveChartViewModel.InitializeGenericPlotLines<T>()` clears current content, creates up to 16 plot lines, configures X-axis mode, selects Y-axis assignment, and repopulates `ChartObjectsCollection`.
- Observable Y-axis assignment is supported by `input.Select(x => x.Axis)`, but the subscription is not explicitly marshalled to the UI scheduler.
- Static `SignalXY_UI` and `ScatterUI` snapshot paths re-create or replace plot data immediately from constructor/property assignment.
- `ClearContent()` owns disposal of current plot lines; individual chart update subscriptions are disposed through each UI object's inherited `Disposables`.

### Threading and scheduler behavior

- `SignalUI.UpdateSignal()` does background projection on `RxSchedulers.TaskpoolScheduler` then marshals mutation/refresh to `RxSchedulers.MainThreadScheduler`.
- `DataLoggerUI.UpdateDataLogger()` follows the same taskpool-to-main pattern.
- `StreamerUI.UpdateStreamerFixedPoints()` follows the same taskpool-to-main pattern.
- `ScatterUI.UpdateScatter()` only observes on `RxSchedulers.MainThreadScheduler`; validation and potential list enumeration happen on the caller thread until the observe boundary.
- `SignalXY_UI` has no observable update API; mouse coordinate subscription is not scheduler-marshalled.
- Constructor subscriptions for name extraction use `Take(1).ObserveOn(RxSchedulers.MainThreadScheduler)`, but they duplicate the source subscription unless the source is shared upstream.
- Several subscriptions use `.Retry()` without a retry policy, which can hide source failures and resubscribe indefinitely.

### Error/completion behavior

Current update paths do not expose a connection-level error or completion state.

- Data stream errors are often swallowed by `.Retry()`.
- Per-update `try/catch` blocks in `SignalUI`, `DataLoggerUI`, and `StreamerUI` swallow plot exceptions and continue silently.
- Observable completion is not surfaced to callers or to the chart state model.
- Disposal is implicit through object disposal; callers cannot observe whether a binding completed, failed, or was disposed.

### Allocation and hot-path behavior

Current hot paths are already partially optimized, but still allocate under common update scenarios:

- `SignalUI.UpdateSignal()` allocates a `double[]` for converted timestamps per emission, then creates arrays from `_uniqueTimeBuffer` and `_uniqueDataBuffer` with collection expressions before `DataLogger.Add(...)`.
- `ScatterUI.InsertData()` removes the existing scatter plottable and creates a new scatter plottable for every update; it reuses buffers only when the incoming count equals the current buffer length, and otherwise allocates correctly sized arrays.
- `DataLoggerUI.UpdateDataLogger()` reuses `_valueBuffer` when possible, but allocates an exact-size array when the incoming count is smaller than the current buffer length.
- `StreamerUI.UpdateStreamerFixedPoints()` reuses `_valueBuffer`, but creates an `ArraySegment<double>` on each update.
- `ChartObjects.AppearanceSubsriptions()` refreshes the plot on every appearance tuple emission and does not coalesce rapid appearance changes.

## Missing functionality

1. No single IObservable-first source abstraction that covers all chart types.
2. No connection object for subscription lifecycle, status, error, completion, and explicit disposal.
3. No deterministic backpressure or batching policy for high-frequency updates.
4. No unified UI scheduler marshalling contract; each control chooses its own ObserveOn behavior.
5. No explicit invalid-sample policy for null, empty, mismatched, or out-of-range axis data.
6. No observable update path for `SignalXY_UI` after initial construction.
7. No test seam that can verify subscription disposal and scheduler use without constructing WPF/ScottPlot controls.
8. Existing tuple shapes encode transport and rendering details together, making downstream evolution hard.
9. Current `.Retry()` behavior hides errors and makes failure/completion semantics untestable.
10. Current initialization re-subscribes to each source for metadata/name and for data; cold observables can repeat work or emit twice.

## Proposed design

### Design principles

- Keep source data transport independent from ScottPlot/WPF mutation.
- Make observable subscription behavior testable without WPF rendering.
- Marshal exactly once at the binder boundary using `RxSchedulers.MainThreadScheduler` by default.
- Normalize all chart types to one envelope while preserving chart-specific options.
- Batch and sample before the UI scheduler when update frequency exceeds render frequency.
- Return an owned connection whose disposal terminates every source subscription.
- Surface `OnError` and `OnCompleted` as state, not hidden retry loops.
- Keep public APIs strongly typed, XML documented, and AOT-friendly.

### Public API sketch

```csharp
namespace CrissCross.WPF.Plot;

/// <summary>
/// Describes an observable source of chart updates for one logical series.
/// </summary>
public interface IReactivePlotSource
{
    /// <summary>
    /// Gets the stable key used to preserve series identity across updates.
    /// </summary>
    PlotSeriesKey Key { get; }

    /// <summary>
    /// Gets the plot type that determines the ScottPlot plottable adapter.
    /// </summary>
    PlotType PlotType { get; }

    /// <summary>
    /// Gets the observable update stream for this series.
    /// </summary>
    IObservable<ReactivePlotUpdate> Updates { get; }
}

/// <summary>
/// Represents an active chart-source binding.
/// </summary>
public interface IReactivePlotConnection : IDisposable
{
    /// <summary>
    /// Gets lifecycle state transitions for the binding.
    /// </summary>
    IObservable<ReactivePlotConnectionState> State { get; }

    /// <summary>
    /// Gets recoverable update errors observed after binding.
    /// </summary>
    IObservable<Exception> Errors { get; }

    /// <summary>
    /// Gets a value indicating whether all source streams completed.
    /// </summary>
    bool IsCompleted { get; }
}

/// <summary>
/// Immutable identity for a chart series.
/// </summary>
public readonly record struct PlotSeriesKey(string Name, int Axis);

/// <summary>
/// Immutable update envelope used by all reactive chart sources.
/// </summary>
public sealed record ReactivePlotUpdate(
    PlotSeriesKey Key,
    PlotType PlotType,
    ReactivePlotUpdateKind Kind,
    IReadOnlyList<double> X,
    IReadOnlyList<double> Y,
    PlotXAxisKind XAxisKind,
    long Sequence);

/// <summary>
/// Binding options for reactive chart updates.
/// </summary>
public sealed record ReactivePlotBindingOptions
{
    public IScheduler? SourceScheduler { get; init; }

    public IScheduler? UiScheduler { get; init; }

    public TimeSpan? BatchWindow { get; init; }

    public int MaxBatchSize { get; init; } = 1024;

    public int? MaxVisiblePoints { get; init; }

    public ReactivePlotOverflowStrategy OverflowStrategy { get; init; } = ReactivePlotOverflowStrategy.DropOldest;

    public ReactivePlotErrorMode ErrorMode { get; init; } = ReactivePlotErrorMode.SurfaceAndStopSeries;
}
```

Supporting enums:

```csharp
public enum ReactivePlotUpdateKind
{
    Append,
    Replace,
    Clear
}

public enum PlotXAxisKind
{
    Numeric,
    OADate,
    Ticks
}

public enum ReactivePlotOverflowStrategy
{
    DropOldest,
    DropNewest,
    KeepLatest,
    BlockSource
}

public enum ReactivePlotErrorMode
{
    SurfaceAndStopSeries,
    SurfaceAndContinueWithRetry,
    IgnoreInvalidUpdates
}

public enum ReactivePlotConnectionState
{
    Connecting,
    Active,
    Completed,
    Faulted,
    Disposed
}
```

### Binder responsibilities

Add a binder/service layer instead of placing all behavior inside WPF controls:

```csharp
public interface IReactivePlotBinder
{
    IReactivePlotConnection Bind(
        LiveChartViewModel chart,
        IEnumerable<IReactivePlotSource> sources,
        ReactivePlotBindingOptions? options = null);
}
```

The binder should:

1. Convert legacy tuple inputs to `IReactivePlotSource` adapters.
2. Share each source stream once to avoid duplicate cold observable subscriptions.
3. Validate update shape before UI marshalling.
4. Apply batching/backpressure before UI marshalling.
5. Observe mutations on `options.UiScheduler ?? RxSchedulers.MainThreadScheduler`.
6. Create or reuse one plottable adapter per `PlotSeriesKey`.
7. Call chart-specific adapter operations (`Append`, `Replace`, `Clear`) without reinitializing unrelated series.
8. Emit connection state transitions and errors.
9. Dispose all source subscriptions and chart adapter subscriptions deterministically.

### Adapter responsibilities by chart type

| Plot type | Adapter behavior |
|---|---|
| `Signal` | Append time-series Y values using X ticks/OADate/numeric conversion policy; enforce unique or monotonic X policy explicitly. |
| `Scatter` | Replace full X/Y snapshot or append batches depending on `ReactivePlotUpdateKind`; avoid removing/recreating plottable when ScottPlot data source replacement is sufficient. |
| `DataLogger` | Append Y values; enforce `MaxVisiblePoints`; use bounded buffer strategy. |
| `Streamer` | Append latest Y samples; treat X values as optional metadata only unless a future streamer mode requires period derivation. |
| `SignalXY` | Support both initial snapshot and future updates through the same `Replace`/`Append` envelope. |

### Legacy compatibility path

Do not remove existing dependency properties in the first implementation slice. Add adapters:

- `ReactivePlotSource.FromSignalTicks(...)`
- `ReactivePlotSource.FromScatterPoints(...)`
- `ReactivePlotSource.FromDataLoggerPoints(...)`
- `ReactivePlotSource.FromStreamerPoints(...)`
- `ReactivePlotSource.FromSignalXySnapshot(...)`

Then update `LiveChart.AssignLiveChartData(...)` and existing property setters to use the binder internally. This preserves the old API while making all behavior testable through `IReactivePlotSource` and `IReactivePlotBinder`.

## Failing TUnit test plan

Create a new test project rather than using the WPF example app:

- Project path: `src/Tests/CrissCross.WPF.Plot.Tests/CrissCross.WPF.Plot.Tests.csproj`
- Target framework: `net10.0-windows10.0.19041.0`
- `UseWPF`: `true`
- References: `CrissCross.WPF.Plot.csproj`
- Test style: TUnit + Microsoft Testing Platform; no `--no-build`

Keep the first tests at the binder/source level so they do not need a visible WPF window. Use fake adapter sinks where possible. Only add Dispatcher/WPF integration tests after binder behavior is covered.

### RED test group 1: source adapters

1. `SignalTicksAdapter_EmitsAppendUpdateWithTicksXAxisKind`
   - Arrange legacy `IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>`.
   - Act through `ReactivePlotSource.FromSignalTicks(...)`.
   - Assert first `ReactivePlotUpdate` has `PlotType.Signal`, `Kind.Append`, `XAxisKind.Ticks`, matching X/Y counts, name, and axis.
   - Initial failure: no `ReactivePlotSource` API exists.

2. `ScatterPointsAdapter_RejectsMismatchedXAndYCounts`
   - Arrange an update where `X.Count != Y.Count`.
   - Assert binder/source reports a validation error instead of mutating the sink.
   - Initial failure: no validation result/error contract exists.

3. `SignalXyAdapter_SupportsObservableReplaceUpdates`
   - Arrange multiple SignalXY snapshots.
   - Assert each snapshot produces `PlotType.SignalXY` and `Kind.Replace`.
   - Initial failure: current `SignalXY_UI` is constructor snapshot only.

### RED test group 2: binder lifecycle

4. `Bind_ReturnsConnectionThatDisposesSourceSubscription`
   - Arrange a source observable created with `Observable.Create` that records disposal.
   - Act: bind, then dispose returned `IReactivePlotConnection`.
   - Assert source disposal callback ran and connection state emits `Disposed`.
   - Initial failure: no connection object exists.

5. `Bind_UsesSingleSubscriptionPerSource`
   - Arrange a cold source that increments a subscription counter.
   - Act: bind one source and emit a name plus data update.
   - Assert subscription count is `1`.
   - Initial failure: existing name/data subscriptions can duplicate subscriptions.

6. `Bind_CompletionEmitsCompletedStateWithoutDisposingChart`
   - Arrange a finite source.
   - Assert `ReactivePlotConnectionState.Completed` is emitted and chart adapters remain inspectable.
   - Initial failure: completion is not surfaced.

7. `Bind_SourceErrorEmitsFaultedStateAndError`
   - Arrange a source that emits one valid update then `OnError`.
   - Assert connection `Errors` receives the exception and state becomes `Faulted` when `ErrorMode.SurfaceAndStopSeries` is used.
   - Initial failure: current `.Retry()` hides error semantics.

### RED test group 3: scheduler and backpressure

8. `Bind_MarshalsAdapterMutationToConfiguredUiScheduler`
   - Arrange a `TestScheduler`/recording scheduler as `UiScheduler` and a fake adapter that records scheduler thread/virtual tick.
   - Act: emit from a background/source scheduler.
   - Assert no mutation occurs before advancing UI scheduler; mutation occurs after advancing it.
   - Initial failure: no injectable scheduler boundary exists.

9. `Bind_BatchesHighFrequencyUpdatesBeforeUiScheduler`
   - Arrange 1,000 updates and `BatchWindow = TimeSpan.FromMilliseconds(16)` with `MaxBatchSize = 128`.
   - Assert fake adapter receives fewer batched calls than source emissions and preserves last sequence.
   - Initial failure: current controls refresh per emission.

10. `Bind_DropOldestOverflowKeepsLatestVisiblePoints`
    - Arrange `MaxVisiblePoints = 100` and 500 append samples.
    - Assert adapter receives/retains the last 100 samples in order.
    - Initial failure: policies differ by chart type and are not testable as a common contract.

### RED test group 4: chart type coverage

11. `Bind_CreatesAdapterForEveryPlotType`
    - Data-drive over `PlotType.Signal`, `Scatter`, `DataLogger`, `Streamer`, and `SignalXY`.
    - Assert the adapter factory is invoked for each plot type and receives the expected initial update.
    - Initial failure: no common adapter factory exists.

12. `Bind_DoesNotReinitializeUnchangedSeries`
    - Arrange two sources with stable keys, update one source.
    - Assert only that series' adapter mutates; the other is not cleared/recreated.
    - Initial failure: current property-change path clears and reinitializes all content.

13. `Bind_ClearUpdateClearsOnlyTargetSeries`
    - Arrange two sources and emit `ReactivePlotUpdateKind.Clear` for one key.
    - Assert only the target adapter is cleared.
    - Initial failure: no `Clear` update envelope exists.

### RED test group 5: invalid data and legacy compatibility

14. `LegacyAssignLiveChartData_UsesReactiveBinderForObservableInputs`
    - Arrange `SignalEnumObsTicks`, `ScatterEnumObsPoints`, `DataLoggerEnumObsPoints`, and `StreamerEnumObsPoints`.
    - Assert each is adapted to `IReactivePlotSource` and bound through the binder.
    - Initial failure: `AssignLiveChartData` directly calls old change methods.

15. `InvalidAxisIndex_EmitsValidationErrorBeforeUiMutation`
    - Arrange an update with `Axis = -1` or an axis outside configured axes.
    - Assert no adapter mutation and one validation error.
    - Initial failure: some axis checks happen inside UI initialization subscriptions and can throw asynchronously.

16. `NullOrEmptySeriesData_IsIgnoredOrFaultedAccordingToOptions`
    - Arrange null/empty lists.
    - Assert `IgnoreInvalidUpdates` drops them and `SurfaceAndStopSeries` faults them deterministically.
    - Initial failure: current behavior varies by control and mostly filters silently.

## Suggested implementation sequence for next card

1. Add source/update model records and XML docs.
2. Add a pure binder core using fake adapter interfaces and TUnit tests first.
3. Add legacy tuple-to-source adapters.
4. Add chart-type adapter factory that wraps existing `SignalUI`, `ScatterUI`, `DataLoggerUI`, `StreamerUI`, and `SignalXY_UI` without changing old public properties.
5. Add `SignalXY` observable replace/append support.
6. Replace existing `LiveChart` property-setter internals with binder calls while preserving old public APIs.
7. Run targeted WPF.Plot tests, then full solution tests.

## Verification commands for implementation card

Run from `/mnt/c/Projects/GitHub/ReactiveMarbles/CrissCross/src` using Windows dotnet from WSL:

```powershell
"/mnt/c/Program Files/dotnet/dotnet.exe" restore CrissCross.slnx
"/mnt/c/Program Files/dotnet/dotnet.exe" test --project Tests/CrissCross.WPF.Plot.Tests/CrissCross.WPF.Plot.Tests.csproj -c Release -- --treenode-filter "/*/CrissCross.WPF.Plot.Tests/ReactivePlotBinderTests/*"
"/mnt/c/Program Files/dotnet/dotnet.exe" test --solution CrissCross.slnx -c Release
```

Do not use `--no-build`. Put MTP/TUnit flags after `--`.

## Residual risks

- Full WPF test execution may require Windows desktop/STA behavior even when launched through Windows dotnet from WSL.
- ScottPlot data-source APIs may constrain true zero-allocation updates; adapter design should isolate these constraints.
- Backpressure policy must avoid unbounded buffering when the UI scheduler is blocked.
- Existing public tuple APIs are broad; keep them as adapters until downstream consumers can migrate.
