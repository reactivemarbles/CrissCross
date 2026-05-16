# CrissCross TUnit/MTP Test Strategy

Scope: repository-wide test placement, quality gates, leak checks, and performance gates for `src/CrissCross.slnx`. This is a checklist for follow-up implementation cards; it does not introduce product changes.

## Current test and benchmark inventory

- Main solution: `src/CrissCross.slnx`.
- MTP runner: `src/global.json` sets `"runner": "Microsoft.Testing.Platform"`.
- Shared MTP config: `src/testconfig.json` disables test parallelism and configures Cobertura coverage for `CrissCross\\..*` modules.
- Test package wiring: `src/Directory.Build.props` treats projects whose MSBuild project name ends with `.Tests` as test projects and adds TUnit, Microsoft.NET.Test.Sdk, Microsoft.Testing.Extensions.CodeCoverage, PublicApiGenerator, Verify.TUnit, and copied `testconfig.json`.
- Existing TUnit project: `src/Tests/CrissCross.Tests/CrissCross.Tests.csproj`.
  - Current coverage: core `RxObject`, `RxObjectMixins`, navigation event args, and `ViewModelRoutedViewHostMixins`.
- Existing BenchmarkDotNet projects:
  - `src/CrissCross.WPF.Benchmarks/CrissCross.WPF.Benchmarks.csproj`
  - `src/CrissCross.Avalonia.Benchmarks/CrissCross.Avalonia.Benchmarks.csproj`
  - `src/CrissCross.MAUI.Benchmarks/CrissCross.MAUI.Benchmarks.csproj`
  - `src/CrissCross.WinForms.Benchmarks/CrissCross.WinForms.Benchmarks.csproj`

## Test project placement

| Area | Production paths | Test location | Gate focus |
| --- | --- | --- | --- |
| Core navigation and reactive base | `src/CrissCross/`, especially `ViewModelRoutedViewHostMixins.cs`, `IViewModelRoutedViewHost.cs`, `NavigationEvents/`, `MagicInterfaces/`, `RxObject.cs`, `RxObjectMixins.cs` | Continue in `src/Tests/CrissCross.Tests/` | Navigation stack behavior, host registration isolation, event args, activation/deactivation signals, disposal of `CompositeDisposable`, and scheduler-independent observable behavior. |
| WPF shell/UI | `src/CrissCross.WPF/`, `src/CrissCross.WPF.UI/` | Add `src/Tests/CrissCross.WPF.UI.Tests/CrissCross.WPF.UI.Tests.csproj` when WPF.UI behavior is changed | `NavigationService`, `PageService`, `SnackbarService`, `ThemeService`, dialogs, taskbar, app host services, markup extensions, and WPF control activation/disposal. Target Windows TFMs only. |
| Avalonia shell/UI | `src/CrissCross.Avalonia/`, `src/CrissCross.Avalonia.UI/` | Add `src/Tests/CrissCross.Avalonia.UI.Tests/CrissCross.Avalonia.UI.Tests.csproj` when Avalonia.UI behavior is changed | Navigation/window services, dialogs/snackbars, theme/resource behavior, converters, virtualizing controls, and activation cleanup. Prefer headless service/model tests before UI automation. |
| MAUI shell/UI | `src/CrissCross.MAUI/NavigationShell.cs`, `src/CrissCross.Maui.UI/` | Add `src/Tests/CrissCross.MAUI.Tests/CrissCross.MAUI.Tests.csproj` when MAUI shell behavior is changed; add MAUI.UI-focused tests when reusable control behavior moves beyond gallery smoke coverage | Shell route registration, navigation command outcomes, MAUI UI control-state projections, weak event/subscription cleanup, and Windows MAUI build/test smoke. Repository now includes both `CrissCross.MAUI` and `CrissCross.Maui.UI`. |
| Galleries/examples | `src/CrissCross.WPF.UI.Gallery/`, `src/CrissCross.Avalonia.UI.Gallery/`, `src/CrissCross.WPF.UI.Test/`, `src/CrissCross.WPF.UI.CC_Nav.Test/`, `src/CrissCross.Avalonia.Test*/`, `src/CrissCross.MAUI.Test/`, `src/CrissCross.WPF.Plot.Test/` | Keep gallery tests as smoke/verification projects unless reusable behavior is extracted into library-specific `.Tests` projects | Build and startup smoke, sample route coverage, view-model construction, resource dictionary loading, and prevention of gallery-only regressions. Avoid putting core assertions only in gallery apps. |
| WPF.Plot | `src/CrissCross.WPF.Plot/Views/`, `src/CrissCross.WPF.Plot/ViewModels/`, `src/CrissCross.WPF.Plot/Controls/` | Continue in `src/Tests/CrissCross.WPF.Plot.Tests/CrissCross.WPF.Plot.Tests.csproj` | `LiveChartViewModel` state transitions, reactive plot binding/source/connection behavior, plottable model updates, axis/legend/crosshair options, chart mode switching, disposal, and allocation-sensitive update paths. No dedicated WPF.Plot BenchmarkDotNet project exists yet. |

New test projects should use a project name ending in `.Tests` so `src/Directory.Build.props` applies the repository's TUnit/MTP settings. Add each new test project to `src/CrissCross.slnx` under `/Tests/` when implemented.

## Required commands

Run commands from `src/` and use Windows dotnet from WSL:

```powershell
cd /mnt/c/Projects/GitHub/ReactiveMarbles/CrissCross/src
"/mnt/c/Program Files/dotnet/dotnet.exe" restore CrissCross.slnx
"/mnt/c/Program Files/dotnet/dotnet.exe" build CrissCross.slnx -c Release
"/mnt/c/Program Files/dotnet/dotnet.exe" test --solution CrissCross.slnx -c Release
```

Targeted test examples:

```powershell
"/mnt/c/Program Files/dotnet/dotnet.exe" test --project Tests/CrissCross.Tests/CrissCross.Tests.csproj -c Release
"/mnt/c/Program Files/dotnet/dotnet.exe" test --project Tests/CrissCross.Tests/CrissCross.Tests.csproj -c Release -- --list-tests
"/mnt/c/Program Files/dotnet/dotnet.exe" test --project Tests/CrissCross.Tests/CrissCross.Tests.csproj -c Release -- --treenode-filter "/*/*/ViewModelRoutedViewHostMixinsTests/*"
"/mnt/c/Program Files/dotnet/dotnet.exe" test --solution CrissCross.slnx -c Release --coverage --coverage-output-format cobertura -- --report-trx --output Detailed
```

MTP/TUnit rules:

- Do not use `--no-build`; stale binaries can hide failures.
- TUnit/MTP flags go after `--`.
- Keep the repository default non-parallel execution unless a test project is proven isolated.
- Do not combine multiple `--treenode-filter` expressions with shell `|`; run separate targeted commands.

## Leak and lifecycle checklist

Apply these gates to every change that touches activation, navigation, view composition, commands, or chart updates:

- Activation: every `WhenActivated` path returns all subscriptions, bindings, commands, event handlers, timers, and observable bridges through the provided `CompositeDisposable`.
- Deactivation: repeated activate/deactivate cycles are idempotent and do not duplicate command subscriptions or routed navigation handlers.
- Navigation: navigating away disposes the prior view/view-model cleanup scope where ownership is expected; retained navigation history is intentional and documented by test assertions.
- Weak references: long-lived services, static host registries, global message buses, and platform event sources must not keep views/view-models alive after cleanup. Prefer weak references or explicit unregister/dispose APIs where ownership crosses lifetimes.
- Reactive commands: async commands are cancellation-aware where possible; `ThrownExceptions` is observed or intentionally exposed to callers.
- Schedulers: tests should inject/test with repository scheduler abstractions where available; do not introduce `RxApp`.
- AOT/hot paths: avoid reflection-heavy assertions or runtime reflection plumbing in library hot paths; tests may verify public API shape with PublicApiGenerator/Verify, but production fixes should stay AOT-friendly.

Suggested TUnit leak pattern for follow-up cards:

1. Create the view/service/view-model under test in a local method.
2. Activate, exercise, deactivate/dispose.
3. Store only `WeakReference` to the object under test.
4. Force full GC twice with `GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();`.
5. Assert `weakReference.IsAlive` is false unless the tested contract intentionally retains the object.

## Performance and allocation gates

- Existing BenchmarkDotNet projects cover platform navigation host operations for WPF, Avalonia, MAUI, and WinForms. Use them when navigation/runtime changes could affect throughput or allocations:

```powershell
"/mnt/c/Program Files/dotnet/dotnet.exe" run --project CrissCross.WPF.Benchmarks/CrissCross.WPF.Benchmarks.csproj -c Release -- --filter "*ViewModelRoutedViewHostBenchmark*"
"/mnt/c/Program Files/dotnet/dotnet.exe" run --project CrissCross.Avalonia.Benchmarks/CrissCross.Avalonia.Benchmarks.csproj -c Release -- --filter "*ViewModelRoutedViewHostBenchmark*"
"/mnt/c/Program Files/dotnet/dotnet.exe" run --project CrissCross.MAUI.Benchmarks/CrissCross.MAUI.Benchmarks.csproj -c Release -- --filter "*NavigationShellBenchmark*"
```

- WPF.Plot changes need allocation-sensitive tests around `LiveChartViewModel` and plottable update batches before UI rendering changes. If sustained chart-update throughput becomes a release gate, add a dedicated `src/CrissCross.WPF.Plot.Benchmarks/` project in a product-change card.
- Track benchmark outputs from `BenchmarkDotNet.Artifacts/` only as review evidence; do not commit generated artifacts unless the repository later defines a baseline process.
- Acceptance for performance-sensitive changes: no avoidable per-point allocations in chart update loops, no unbounded retained series/history from transient updates, and benchmark deltas explained in the PR/card handoff.

## Acceptance gates for future implementation cards

- New behavior has a failing TUnit test first, then minimal implementation, then green targeted test.
- Relevant project/solution build succeeds in Release.
- Broad test gate succeeds: `"/mnt/c/Program Files/dotnet/dotnet.exe" test --solution CrissCross.slnx -c Release`.
- Coverage command runs for release candidates and produces Cobertura output.
- Leak-sensitive changes include at least one activation/disposal or weak-reference retention test.
- Performance-sensitive navigation/chart changes run the matching BenchmarkDotNet project or document why it is not applicable.
- Public APIs have XML docs; production code introduces no `#pragma warning disable`.
- Residual risks are stated explicitly when platform workloads, UI automation, or device-specific MAUI/Avalonia targets cannot be run locally.
