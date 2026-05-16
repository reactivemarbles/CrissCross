# CrissCross gallery examples

The gallery/example projects are deterministic manual-QA surfaces and developer documentation for the shared CrissCross runtime plus platform UI packages.

## Projects

- `src/CrissCross.WPF.UI.Gallery/CrissCross.WPF.UI.Gallery.csproj`
  - Windows desktop WPF gallery.
  - Demonstrates ViewModel-based navigation through `NavigationVMLeft` and registered `IViewFor<TViewModel>` mappings.
  - Demonstrates View-based navigation through the breadcrumb/navigation window APIs already used by the gallery shell.
  - Includes the `FeaturePlaygroundView` page for new shared controls, async reactive commands, activation/disposal, and shared WPF styles/themes.
- `src/CrissCross.Avalonia.UI.Gallery/CrissCross.Avalonia.UI.Gallery.csproj`
  - Cross-platform Avalonia desktop gallery.
  - Demonstrates `NavigationWindow<MainViewModel>` hosting, ViewModel-based navigation commands, ReactiveUI view location, and the same feature playground concepts.
- `src/CrissCross.MAUI.Test/CrissCross.MAUI.Example.csproj`
  - MAUI example app extended with a `ControlsGalleryView` page.
  - Demonstrates MAUI UI controls, `CrissCrossMauiUi.xaml` shared resources, touch-first layout, platform-specific behavior notes, and navigation from the existing main page.

## Feature coverage

The WPF, Avalonia, and MAUI examples cover:

- ViewModel-based navigation from shell/menu commands into registered pages.
- View-based navigation surfaces via WPF breadcrumb/navigation-window APIs and Avalonia `NavigationWindow` hosting.
- New controls: `CommandButton`, `BusyOverlay`, `SearchBox`, `FilterBar`, `DataPager`, `DateTimeRangePicker`, `SegmentedControl`, `Stepper`, and `ThemeSwitcher` where each platform package supports them.
- Shared styles/themes through WPF `ControlsDictionary`/`ThemesDictionary`, Avalonia style includes, and MAUI `CrissCrossMauiUi.xaml` resources.
- async reactive commands using `ReactiveCommand.CreateFromTask` with deterministic delays and no network dependency.
- activation/disposal via `WhenNavigatedTo` plus disposable observable activity, and view `WhenActivated` patterns.
- platform-specific behavior notes in each playground view model.

## Build and run from WSL

Use Windows `dotnet` from WSL for repository validation:

```bash
cd /mnt/c/Projects/GitHub/ReactiveMarbles/CrissCross/src
"/mnt/c/Program Files/dotnet/dotnet.exe" restore CrissCross.slnx
"/mnt/c/Program Files/dotnet/dotnet.exe" build CrissCross.slnx -c Release
"/mnt/c/Program Files/dotnet/dotnet.exe" test --solution CrissCross.slnx -c Release
```

Run individual galleries:

```bash
cd /mnt/c/Projects/GitHub/ReactiveMarbles/CrissCross/src
"/mnt/c/Program Files/dotnet/dotnet.exe" run --project CrissCross.WPF.UI.Gallery/CrissCross.WPF.UI.Gallery.csproj -c Release
"/mnt/c/Program Files/dotnet/dotnet.exe" run --project CrissCross.Avalonia.UI.Gallery/CrissCross.Avalonia.UI.Gallery.csproj -c Release
"/mnt/c/Program Files/dotnet/dotnet.exe" build CrissCross.MAUI.Test/CrissCross.MAUI.Example.csproj -c Release -f net10.0-windows10.0.19041.0
```

The MAUI Android target requires installed Android workloads/emulator/device. The Windows target is the deterministic validation path on this development machine.

## Manual QA checklist

1. Open each gallery and navigate to the reactive feature playground.
2. Run the async import command and verify command state/progress and busy overlay changes.
3. Edit search text, submit search, clear filters, and page through deterministic data state.
4. Change segmented selection and stepper state.
5. Change theme selection and inspect platform-specific theme behavior.
6. Navigate away and back to verify activation/disposal logging resets without duplicate timers.
