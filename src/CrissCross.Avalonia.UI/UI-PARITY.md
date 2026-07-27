# Avalonia UI parity and gallery coverage

The Avalonia UI project has a source control-folder match with the WPF UI
project for every control family except `Symbols`. `Symbols` is a WPF generated
asset container; Avalonia exposes the equivalent `SymbolRegular` and
`SymbolFilled` icon definitions directly, while additionally exposing
`MenuItem` as its own control family.

The themed-control inventory contains 113 Avalonia theme dictionaries across
117 public control families. The gallery now covers every applicable family:
60 families are instantiated directly on its interactive pages and the
remaining 51 are discoverable through the data-driven **Control & Host
Catalog** page. That page identifies the concrete composed page or top-level
host required by each family. It deliberately avoids invalid nested window,
dialog, and virtualisation topology.

| Comparison | Before | After |
| --- | ---: | ---: |
| Avalonia public control families | 117 | 117 |
| Directly instantiated gallery families | 60 | 60 |
| Discoverable catalog or host families | 0 | 51 |
| Applicable families without a gallery/host example | 51 | 0 |
| Theme dictionaries covered by an application style include | 113 | 113 |
| Inapplicable nested-page families | 6 | 6 |

The catalog records these composed or host-backed families:

`Alarms`, `Anchor`, `AppBar`, `Arc`, `BreadcrumbBar`, `ChipGroup`,
`ContentDialog`, `ContextMenu`, `DataFilterPanel`, `DataGrid`,
`DynamicScrollBar`, `DynamicScrollViewer`, `EmptyState`, `Expander`, `Flyout`,
`Frame`, `Gauges`, `GifImage`, `GridView`, `GroupBox`, `IconElement`,
`IconSource`, `Image`, `ItemsControl`, `Label`, `ListBox`, `ListView`,
`LoadingScreen`, `Menu`, `MessageBox`, `MessageBoxAsync`,
`NavigationControls`, `NavigationUserControl`, `NavigationView`,
`NumericPushButton`, `Page`, `PropertyGridLite`, `ScrollBar`, `ScrollViewer`,
`StatusBar`, `TabControl`, `TabView`, `TitleBar`, `ToolBar`, `ToolTip`,
`TreeGrid`, `TreeView`, `ValidationSummary`, `VirtualizingGridView`,
`VirtualizingItemsControl`, and `VirtualizingWrapPanel`.

The six remaining inapplicable families are native window-chrome primitives:
`AccessText`, `ClientAreaBorder`, `FluentNavigationWindow`, `FluentWindow`,
`ModernWindow`, and `Window`. They cannot be nested inside a `UserControl`;
they are instead exercised by the application-level gallery window/topology or
are native text/chrome primitives styled by the loaded theme dictionaries.

Interactive gallery coverage includes custom input, command, picker, feedback,
card, workflow, BBCode, rich text, progress, and theme controls. It also
includes `ThemeSwitcher` and runtime light/dark selection, which exercise the
shared resources in `Resources/Theme/Light.axaml` and
`Resources/Theme/Dark.axaml`.
