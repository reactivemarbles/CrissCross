# Avalonia UI parity and gallery coverage

The Avalonia UI project has a source control-folder match with the WPF UI
project for every control family except `Symbols`. `Symbols` is a WPF generated
asset container; Avalonia exposes the equivalent `SymbolRegular` and
`SymbolFilled` icon definitions directly, while additionally exposing
`MenuItem` as its own control family. Avalonia also includes the coordinated
industrial `ProcessValueIndicator` control for process-value readouts.

The themed-control inventory contains 114 Avalonia control theme dictionaries
across 118 public control families. The gallery now covers the applicable
rendered control families directly or through the data-driven **Control & Host
Catalog** page. The catalog also marks API-compatibility wrappers whose current
Avalonia implementation is intentionally thin, including `Frame` and `Page`, so
those entries are not counted as behavior-parity demonstrations until their
navigation behavior is implemented. `NavigationUserControl` is now a concrete
Avalonia UI routed view-model host wrapper with a per-instance generated host
name when consumers do not supply one.
It deliberately avoids invalid nested window, dialog, and virtualisation topology.

| Comparison | Before | After |
| --- | ---: | ---: |
| Avalonia public control families | 117 | 118 |
| Directly instantiated gallery families | 60 | 61 |
| Discoverable catalog, host, or wrapper families | 0 | 52 |
| Applicable families without a gallery/host or wrapper entry | 51 | 0 |
| Theme dictionaries covered by an application style include | 113 | 114 |
| Inapplicable nested-page families | 6 | 6 |

The catalog records these composed or host-backed families:

`Alarms`, `Anchor`, `AppBar`, `Arc`, `BreadcrumbBar`, `ChipGroup`,
`ContentDialog`, `ContextMenu`, `DataFilterPanel`, `DataGrid`,
`DynamicScrollBar`, `DynamicScrollViewer`, `EmptyState`, `Expander`, `Flyout`,
`Frame` (wrapper gap tracked separately), `Gauges`, `GifImage`, `GridView`, `GroupBox`, `IconElement`,
`IconSource`, `Image`, `ItemsControl`, `Label`, `ListBox`, `ListView`,
`LoadingScreen`, `Menu`, `MessageBox`, `MessageBoxAsync`,
`NavigationControls`, `NavigationUserControl` (per-instance routed host), `NavigationView`,
`NumericPushButton`, `Page` (wrapper gap tracked separately), `ProcessValueIndicator`, `PropertyGridLite`,
`ScrollBar`, `ScrollViewer`, `StatusBar`, `TabControl`, `TabView`, `TitleBar`,
`ToolBar`, `ToolTip`, `TreeGrid`, `TreeView`, `ValidationSummary`,
`VirtualizingGridView`, `VirtualizingItemsControl`, and
`VirtualizingWrapPanel`.

`NavigationUserControl` now composes the existing Avalonia
`ViewModelRoutedViewHost`, preserves consumer content inside the frame, and
registers a stable per-instance navigation host. `Frame` and `Page` remain thin
Avalonia `UserControl` wrappers retained for API compatibility and are not
behavior-parity navigation implementations yet.

The six remaining inapplicable families are native window-chrome primitives:
`AccessText`, `ClientAreaBorder`, `FluentNavigationWindow`, `FluentWindow`,
`ModernWindow`, and `Window`. They cannot be nested inside a `UserControl`;
they are instead exercised by the application-level gallery window/topology or
are native text/chrome primitives styled by the loaded theme dictionaries.

Interactive gallery coverage includes custom input, command, picker, feedback,
card, workflow, BBCode, rich text, progress, industrial process-value, and
theme controls. It also includes `ThemeSwitcher` and runtime light/dark
selection, which exercise the shared resources in
`Resources/Theme/Light.axaml` and `Resources/Theme/Dark.axaml`.
