# CrissCross.Maui.UI

CrissCross.Maui.UI provides MAUI projections of the shared CrissCross control-state models used by the WPF.UI and Avalonia.UI packages.

Implemented parity controls:

- `CommandButton` and `AsyncCommandButton`
- `BusyOverlay`
- `EmptyState`
- `SearchBox` and `FilterBar`
- `ReactiveFormField` and `ValidationSummary`
- `Chip` and `ChipGroup`
- `SegmentedControl`
- `DataPager`
- `Stepper`
- `DateTimeRangePicker`
- `ThemeSwitcher`
- `DataFilterPanel`
- `PropertyGridLite`
- `Card`, `CardAction`, and `CardExpander`
- `InfoBar` and `InfoBadge`
- `PersonPicture`
- `RatingControl`
- `ProcessValueIndicator`

The controls intentionally expose bindable state snapshots and command hooks instead of reflection-heavy discovery or platform-specific renderer hacks. The default resource extension follows the MAUI application requested theme; explicit Light, Dark, and High Contrast selections use concrete semantic-token dictionaries. Add the shared resource dictionary with:

```csharp
Application.Current.Resources.UseCrissCrossMauiUiResources();
Application.Current.Resources.UseCrissCrossMauiUiResources(themeState);
```

## WPF-to-MAUI control parity

The Gallery instantiates all 28 controls below. MAUI variants use bindable snapshots and command boundaries; they do not emulate WPF template-part or routed-event internals.

| WPF.UI family | MAUI status | MAUI control or reason |
| --- | --- | --- |
| Commands, busy, search, filters, paging, chips, segmented selection, steps, date ranges, form validation, property grid, themes, empty state | Implemented | `CommandButton`, `AsyncCommandButton`, `BusyOverlay`, `SearchBox`, `FilterBar`, `DataPager`, `DataFilterPanel`, `Chip`, `ChipGroup`, `SegmentedControl`, `Stepper`, `DateTimeRangePicker`, `ReactiveFormField`, `ValidationSummary`, `PropertyGridLite`, `ThemeSwitcher`, `EmptyState` |
| Cards and action/expansion surfaces | Implemented | `Card`, `CardAction`, `CardExpander` use MAUI `Border`, `Button`, and composed layouts. |
| Feedback and status | Implemented | `InfoBar`, `InfoBadge`, plus the existing `ValidationSummary` and `BusyOverlay`. |
| Identity and ratings | Implemented | `PersonPicture` has image/initials fallback; `RatingControl` is command-driven and accessible. |
| Industrial process readouts | Implemented | `ProcessValueIndicator` projects immutable `ProcessValueState` snapshots into value, status, and range-bar presentation. |
| WPF window, title bar, app bar, taskbar, client-area, shell and DWM interop | Incomplete in shared MAUI.UI | MAUI Windows can host desktop applications, but these features need MAUI window/handler integration owned by the application shell. |
| WPF navigation view, page-service, dialogs, snackbars and message boxes | Partially implemented / incomplete | Snackbar is implemented; broader navigation and dialog parity remains shell/platform-service work for MAUI applications. |
| WPF virtualization, TreeGrid, GridView, DataGrid, custom scroll bars and panel implementations | Incomplete | MAUI provides CollectionView/handler virtualization, but equivalent shared controls are not implemented. |
| WPF media, rich-text, BBCode, GIF, icon-element and color-selector template systems | Not yet portable | These depend on WPF text/media/template APIs; a MAUI-native design needs dedicated requirements before adding an incomplete replica. |

MAUI handler-level customization remains intentionally avoided so the package stays AOT- and trimming-friendly.
