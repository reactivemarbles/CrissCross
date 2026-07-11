# CrissCross baseline solution map and validation constraints

Scope: discovery only for `/mnt/c/Projects/GitHub/ReactiveMarbles/CrissCross`.

## Executive summary

- The repository is a ReactiveUI-centric navigation/UI framework across Core, WPF, WPF UI, WPF WebView2, WPF Plot, WinForms, Avalonia, Avalonia UI, and MAUI. README confirms the supported platform/package list at `README.md:18-28` and the ViewModel-first navigation model at `README.md:51-63`.
- Main solution is `src/CrissCross.slnx`, an XML SLNX file. `agent.md` states SLNX is the required format and works with `dotnet build/test` at `agent.md:25-32`.
- The solution includes 29 project entries. Several example/mobile/browser projects are present but explicitly excluded from Release|Any CPU solution build (`Build Project="false"`) in `src/CrissCross.slnx:17-65`; the broad Release build currently builds 17 projects.
- Full validation is Windows-bound because WPF, WinForms, MAUI Windows, WebView2, and .NET Framework targets are in the graph. `agent.md:69-71` calls this out directly.
- On this machine, Windows dotnet is `/mnt/c/Program Files/dotnet/dotnet.exe`; SDK 10.0.204/MSBuild 18.3.3 is installed. Initially `restore` failed due missing `wasm-tools`; `dotnet workload restore` installed/updated `android`, `ios`, `maccatalyst`, `macos`, `maui-windows`, `tvos`, and `wasm-tools`, after which restore/build proceeded.
- Current broad build passes Release with warnings: `dotnet build CrissCross.slnx -c Release` exits 0 with 248 warnings. A `-warnaserror` validation is not currently feasible without fixing existing warnings.
- Current broad tests do not pass: `dotnet test --solution CrissCross.slnx -c Release` discovers 78 tests, with 68 passed, 6 skipped, 4 failed.

## Repository/agent constraints

Canonical agent guidance is `agent.md`; `AGENTS.md` delegates to it.

Important rules from `agent.md`:

- Work from `src` for build/test and use `src/CrissCross.slnx` (`agent.md:11-14`, `agent.md:54-67`).
- Required SDKs are documented as .NET 8.0, 9.0, and 10.0 (`agent.md:36-41`), although this machine currently reports only .NET SDK 10.0.203 and 10.0.204.
- Run `dotnet workload restore` from `src` before full validation (`agent.md:42-52`).
- Testing uses Microsoft Testing Platform + TUnit; MTP flags go after `--`, do not use `--no-build`, and tests are configured non-parallel (`agent.md:75-117`).
- Prefer `RxSchedulers`; `RxApp` has been removed (`agent.md:175-180`, `README.md:45-49`).
- Public APIs require XML docs, analyzers are enabled, and production `#pragma warning disable` is disallowed (`agent.md:214-250`).

## Solution and project map

Source of truth: `src/CrissCross.slnx:10-90` plus project files.

### Core/package projects

| Project | Target frameworks | Role/dependencies |
|---|---|---|
| `CrissCross/CrissCross.csproj` | `$(CrissCrossCoreTargetFrameworks)` | Core reactive/navigation abstractions. References `ReactiveUI`. AOT-compatible for net8+/net9+/net10+ (`CrissCross.csproj:3-12`). |
| `CrissCross.WPF/CrissCross.WPF.csproj` | `$(CrissCrossWinTargetFrameworks)` | WPF host package. Uses WPF, references `ReactiveUI.WPF`, `Microsoft.Web.WebView2`, and core (`CrissCross.WPF.csproj:3-19`). |
| `CrissCross.WinForms/CrissCross.WinForms.csproj` | `$(CrissCrossWinTargetFrameworks)` | WinForms host package. Uses Windows Forms, references `ReactiveUI.WinForms` and core (`CrissCross.WinForms.csproj:3-18`). |
| `CrissCross.WPF.WebView2/CrissCross.WPF.WebView2.csproj` | `$(CrissCrossWebviewTargetFrameworks)` | WPF/WebView2 overlay package. Uses WPF and `Microsoft.Web.WebView2` (`CrissCross.WPF.WebView2.csproj:3-14`). |
| `CrissCross.WPF.UI/CrissCross.WPF.UI.csproj` | `$(CrissCrossWinTargetFrameworks)` | WPF UI control library. Uses WPF, resources/fonts, hosting/logging packages, ReactiveList, WPF UI packages, source generators, and references `CrissCross.WPF` (`CrissCross.WPF.UI.csproj:3-67`). |
| `CrissCross.WPF.Plot/CrissCross.WPF.Plot.csproj` | `$(CrissCrossWinTargetFrameworks)` | WPF plot control suite. Uses WPF, ScottPlot.WPF, source generators, and references `CrissCross.WPF.UI` (`CrissCross.WPF.Plot.csproj:3-28`). |
| `CrissCross.Avalonia/CrissCross.Avalonia.csproj` | `$(CrissCrossAvaloniaTargetFrameworks)` | Avalonia host package. References `Avalonia`, `ReactiveUI.Avalonia`, `ReactiveUI`, and core (`CrissCross.Avalonia.csproj:3-20`). |
| `CrissCross.Avalonia.UI/CrissCross.Avalonia.UI.csproj` | `$(CrissCrossAvaloniaTargetFrameworks)` | Avalonia UI control library. Uses Avalonia, Fluent theme, CP hosting, logging, ReactiveList, AngleSharp/Markdig, source generators, and references `CrissCross.Avalonia` (`CrissCross.Avalonia.UI.csproj:3-55`). |
| `CrissCross.MAUI/CrissCross.MAUI.csproj` | net9/net10 plus iOS, tvOS, macOS, MacCatalyst, Android, and Windows targets | MAUI helper package. Uses `UseMaui`, `SingleProject`, ReactiveUI.Maui, MAUI controls/compatibility, extensions packages, Android annotation exclusion, and core (`CrissCross.MAUI.csproj:3-35`). |

### Framework property expansion

Defined in `src/Directory.Build.props:32-38`:

- `CrissCrossCoreTargetFrameworks`: `net8.0;net9.0;net10.0`, plus Windows target frameworks when building on Windows.
- `CrissCrossAvaloniaTargetFrameworks`: `net8.0;net9.0;net10.0`.
- `CrissCrossWinTargetFrameworks`: `net472;net481;net8.0-windows10.0.19041.0;net9.0-windows10.0.19041.0;net10.0-windows10.0.19041.0`.
- `CrissCrossWebviewTargetFrameworks`: `net472;net481;net8.0-windows;net9.0-windows;net10.0-windows`.

### Examples, galleries, and benchmarks

Solution folders in `src/CrissCross.slnx`:

- Benchmarks: Avalonia, MAUI, WinForms, WPF (`CrissCross.slnx:10-15`). These are included in the solution and not marked `Build Project="false"`.
- Examples: Avalonia base/Android/Browser/Desktop/iOS, MAUI, WinForms, WPF, WPF UI, WPF UI CC_Nav, WPF WebView2, WPF Plot, plus WPF and Avalonia UI galleries (`CrissCross.slnx:16-66`). Most examples are marked not to build for `Release|Any CPU`; WPF Plot example and both gallery projects are not excluded.
- `../build/_build.csproj` is listed but excluded from solution build (`CrissCross.slnx:78-80`).

## UI platform map

| Platform area | Projects | Validation implications |
|---|---|---|
| WPF navigation | `CrissCross.WPF`, WPF examples/benchmarks | Windows-only; multi-targets .NET Framework and net8/9/10 Windows. |
| WPF UI controls | `CrissCross.WPF.UI`, `CrissCross.WPF.UI.Gallery`, examples | Windows-only; includes a broad resource/control surface. Existing build warnings include nullability/member-hiding and XML cref warnings. |
| WPF Plot | `CrissCross.WPF.Plot`, `CrissCross.WPF.Plot.Test` | Windows-only; depends on WPF UI and ScottPlot.WPF. |
| WPF WebView2 | `CrissCross.WPF.WebView2`, example | Windows-only; targets net472/net481 and net8/9/10-windows. |
| WinForms | `CrissCross.WinForms`, example, benchmark | Windows-only; targets `CrissCrossWinTargetFrameworks`. |
| Avalonia | `CrissCross.Avalonia`, examples Desktop/Browser/Android/iOS, benchmark | Library targets net8/9/10; examples require Avalonia desktop/browser/mobile workloads, including wasm-tools for browser. |
| Avalonia UI | `CrissCross.Avalonia.UI`, `CrissCross.Avalonia.UI.Gallery` | Library/gallery build on Windows after workload restore; current warnings include Avalonia obsolete Watermark usage and member hiding. |
| MAUI | `CrissCross.MAUI`, MAUI example, benchmark | Requires MAUI/mobile workloads; targets net9/net10 mobile/Desktop/Windows frameworks. |

## Package/version baseline

Central package management is enabled in `src/Directory.Packages.props:3-5`.

Key versions from `src/Directory.Packages.props`:

- Avalonia: `12.0.2`; ReactiveUI: `23.2.27`; ReactiveUI.Avalonia: `12.0.1`; Splat variable is defined as `19.3.1` but no central `Splat` package version entry was found (`Directory.Packages.props:6-28`).
- WebView2: `1.0.3912.50`; Microsoft.Extensions.*: `10.0.7`; MAUI Controls/Compatibility: `10.0.60` for net10, `9.0.120` otherwise (`Directory.Packages.props:30-40`).
- Reactive primitives/source generators: `ReactiveUI.Primitives*` `6.0.0`, `ReactiveUI.SourceGenerators` `3.1.0` (`Directory.Packages.props:25-57`).
- UI/support packages: CP.Extensions.Hosting.ReactiveUI WPF/Avalonia `3.1.26`, ReactiveList `4.0.35`, ScottPlot.WPF `5.1.58`, AngleSharp `1.4.0`, Markdig `1.1.3` (`Directory.Packages.props:49-60`).
- Testing packages: TUnit `1.43.11`, Verify.TUnit `31.16.2`, Microsoft.NET.Test.Sdk `18.5.1`, Microsoft.Testing.Extensions.CodeCoverage `18.6.2`, PublicApiGenerator `11.5.4` (`Directory.Packages.props:64-69`).
- Build/analyzers: Nerdbank.GitVersioning `3.9.50`, StyleSharp.Analyzers `3.14.0`, Roslynator.Analyzers `4.15.0` (`Directory.Packages.props:82-84`).

## Test coverage shape

- One real test project exists: `src/Tests/CrissCross.Tests/CrissCross.Tests.csproj`.
- It targets `net10.0`, outputs an executable, references only `CrissCross`, and imports TUnit usings (`CrissCross.Tests.csproj:3-17`). Test package references are injected centrally through `Directory.Build.props:43-63` because the project name ends with `.Tests`.
- Test files found: `RxObjectMixinsTests.cs`, `RxObjectTests.cs`, `ViewModelRoutedViewHostMixinsTests.cs`, `NavigationEventArgsTests.cs`.
- Static search found 78 `[Test]` attributes across those files.
- Current broad test result: 78 total, 68 succeeded, 6 skipped, 4 failed.
- Failing tests in the current run:
  - `SetMainNavigationHost_DoesNotRegisterTwice` at `ViewModelRoutedViewHostMixinsTests.cs:229`, equality assertion differs at `CurrentViewModel` observable member.
  - `ClearHistory_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered` at `ViewModelRoutedViewHostMixinsTests.cs:259`, expected exception but no exception was thrown.
  - `ClearHistory_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered` at `ViewModelRoutedViewHostMixinsTests.cs:298`, expected exception but no exception was thrown.
  - `NavigateToView_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered` at `ViewModelRoutedViewHostMixinsTests.cs:421`, expected `KeyNotFoundException` but no exception was thrown.
- Skips are intentional/currently documented in test output around DisplayName property change behavior and unregistered-host navigation behavior.
- `src/testconfig.json` disables test parallelism and configures coverage module include/exclude patterns (`testconfig.json:2-23`).

## Validation commands and observed results

All commands below were run from `/mnt/c/Projects/GitHub/ReactiveMarbles/CrissCross/src` using Windows dotnet:

```bash
"/mnt/c/Program Files/dotnet/dotnet.exe" --info
```

Observed: SDK 10.0.204, MSBuild 18.3.3, Windows RID win-x64. After workload restore, installed workloads include `android`, `ios`, `maccatalyst`, `macos`, `maui-windows`, `tvos`, and `wasm-tools`.

```bash
"/mnt/c/Program Files/dotnet/dotnet.exe" restore CrissCross.slnx
```

Observed first run: failed with `NETSDK1147` requiring `wasm-tools` for `CrissCross.Avalonia.Test.Browser`.

```bash
"/mnt/c/Program Files/dotnet/dotnet.exe" workload restore
```

Observed: installed/updated the missing mobile/wasm workloads successfully.

```bash
"/mnt/c/Program Files/dotnet/dotnet.exe" restore CrissCross.slnx
```

Observed after workload restore: exit 0; 26 projects restored, 2 up-to-date.

```bash
"/mnt/c/Program Files/dotnet/dotnet.exe" build CrissCross.slnx -c Release
```

Observed: exit 0; 248 warnings, 0 errors. This is currently the broad build baseline.

```bash
"/mnt/c/Program Files/dotnet/dotnet.exe" test --solution CrissCross.slnx -c Release
```

Observed: exit 2; 78 total tests, 68 passed, 6 skipped, 4 failed. Do not use `--no-build` for future test passes.

Recommended future validation ladder:

1. `"/mnt/c/Program Files/dotnet/dotnet.exe" --info`
2. `"/mnt/c/Program Files/dotnet/dotnet.exe" workload restore`
3. `"/mnt/c/Program Files/dotnet/dotnet.exe" restore CrissCross.slnx`
4. `"/mnt/c/Program Files/dotnet/dotnet.exe" build CrissCross.slnx -c Release`
5. `"/mnt/c/Program Files/dotnet/dotnet.exe" test --solution CrissCross.slnx -c Release`
6. For focused TUnit diagnostics: `"/mnt/c/Program Files/dotnet/dotnet.exe" test --project Tests/CrissCross.Tests/CrissCross.Tests.csproj -c Release -- --treenode-filter "/*/*/ViewModelRoutedViewHostMixinsTests/*"`
7. Only after warning cleanup: `"/mnt/c/Program Files/dotnet/dotnet.exe" build CrissCross.slnx -c Release -warnaserror`

## Known constraints and risks for follow-on cards

- Workload state is part of the validation contract. A fresh Windows machine can fail restore before `dotnet workload restore`, specifically on Avalonia Browser/wasm (`NETSDK1147`).
- The documented SDK requirement says 8/9/10 SDKs, but this machine has only 10.0.203/10.0.204 SDKs installed. SDK 10 can build the current graph here after workload restore, but SDK mismatch remains a reproducibility risk.
- Broad build currently passes only without treating warnings as errors. Existing warning classes include core nullability warnings (`CS8602`, `CS8604`), Avalonia member-hiding warnings (`CS0108`), Avalonia/nullability warnings (`CS8620`, `CS8625`), unresolved XML cref warnings (`CS1574`), obsolete Avalonia Watermark warnings (`AVLN5001`), and unused field warnings (`CS0169`).
- Broad tests currently fail in navigation host behavior tests, so test status must be reported separately from build status.
- Solution build does not exercise every example project under Release|Any CPU because many are marked `Build Project="false"`; targeted project builds are required when changing those examples.
- Mobile/browser/MAUI projects add platform workload risk and can make broad restore/build slower or environment-sensitive.
- Public API and AOT-sensitive core changes should preserve the existing RxSchedulers/no-RxApp direction and avoid reflection-heavy hot paths.
