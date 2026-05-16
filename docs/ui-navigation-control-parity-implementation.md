# WPF.UI and Avalonia.UI navigation/control parity implementation

Task: `t_4d34bae8`.

## Implemented

- Added a platform-neutral `CrissCross.NavigationJournal` helper for back/forward journal behavior.
- Updated WPF `NavigationView` and Avalonia `NavigationView` to use the shared journal behavior.
- Implemented `GoForward()` in both UI stacks; it now returns `false` when no forward entry exists instead of throwing `NotImplementedException`.
- Added `CanGoForward` to both `INavigationView` contracts.
- Added `GoForward()` to both WPF and Avalonia `INavigationService` contracts and implementations.
- Completed WPF resource aggregation for existing controls that were present but not merged by `Resources/CrissCross.Ui.xaml`:
  - `AppBar`
  - `DateTimePicker`
  - `NumberPad`
  - `SquareSlider`
- Added shared application-control state models in `CrissCross` so WPF and Avalonia do not duplicate non-visual control behavior:
  - `CommandButtonState`
  - `CommandButtonStatus`
  - `BusyOperation`
  - `EmptyStateModel`
  - `EmptyStateVariant`
- Added matching `CommandButton`/`AsyncCommandButton`, `BusyOverlay`, and `EmptyState` controls to WPF.UI and Avalonia.UI.
- Added WPF resource dictionaries and Avalonia theme includes for the new high-value controls.
- Covered the shared command, busy, and empty-state model behavior with TUnit tests.
- Added shared search/filter state models in `CrissCross`:
  - `SearchQueryState`
  - `FilterToken`
  - `FilterOperator`
- Added matching `SearchBox` and `FilterBar` controls to WPF.UI and Avalonia.UI.
- Added WPF resource dictionaries and Avalonia theme includes for the new search/filter controls.
- Covered the shared search/filter state behavior with TUnit tests.
- Added shared reactive form validation models in `CrissCross`:
  - `ValidationSeverity`
  - `ValidationMessage`
  - `ValidationSummaryState`
  - `FormFieldState`
- Added matching `ReactiveFormField` and `ValidationSummary` controls to WPF.UI and Avalonia.UI.
- Added WPF resource dictionary includes and Avalonia theme includes for the new form validation controls.
- Covered the shared validation message/summary behavior with TUnit tests.
- Added shared selection/tagging models in `CrissCross`:
  - `ChipModel`
  - `ChipGroupState`
  - `ChipGroupSelectionMode`
  - `SegmentItem`
  - `SegmentedSelectionState`
- Added matching `Chip`, `ChipGroup`, and `SegmentedControl` controls to WPF.UI and Avalonia.UI.
- Added WPF resource dictionary includes and Avalonia theme includes for the new selection/token controls.
- Covered the shared chip group and segmented-selection behavior with TUnit tests.
- Added shared data paging models in `CrissCross`:
  - `PaginationState`
  - `PageRequest`
- Added matching `DataPager` controls to WPF.UI and Avalonia.UI.
- Added WPF resource dictionary includes and Avalonia theme includes for the new paging control.
- Covered shared pagination range, clamping, and page-request snapshot behavior with TUnit tests.
- Added shared workflow stepper models in `CrissCross`:
  - `StepStatus`
  - `StepperOrientation`
  - `StepDescriptor`
  - `StepperState`
- Added matching `Stepper` controls to WPF.UI and Avalonia.UI.
- Added WPF resource dictionary include and Avalonia theme include for the new stepper/wizard progress control.
- Covered shared step availability, validation blocking, and workflow navigation gating with TUnit tests.
- Added shared date/time range models in `CrissCross`:
  - `DateTimeRange`
  - `DateTimeRangePreset`
  - `DateTimeRangePresetDefinition`
- Added matching `DateTimeRangePicker` controls to WPF.UI and Avalonia.UI.
- Added a dedicated Avalonia `DateTimePicker.axaml` style include and date/time range picker theme include.
- Covered shared date/time range validation and preset resolution behavior with TUnit tests.
- Added shared theme preference models in `CrissCross`:
  - `ThemeChoice`
  - `ThemePreferenceState`
- Added matching `ThemeSwitcher` controls to WPF.UI and Avalonia.UI.
- Added an Avalonia `ThemeService` implementation matching the existing `IThemeService` contract.
- Added WPF resource dictionary include and Avalonia theme include for the new theme switcher surface.
- Covered shared system/light/dark/high-contrast preference resolution behavior with TUnit tests.
- Added shared descriptor-driven data filtering models in `CrissCross`:
  - `FilterEditorKind`
  - `FilterDescriptor`
  - `FilterExpression`
  - `DataFilterPanelState`
- Extended `FilterOperator` with `Between` for date/range filter descriptors.
- Added matching `DataFilterPanel` controls to WPF.UI and Avalonia.UI.
- Added WPF resource dictionary include and Avalonia theme include for the new data filter panel surface.
- Covered shared descriptor/token projection, active-expression filtering, and query-state projection behavior with TUnit tests.
- Added shared descriptor-driven property inspector models in `CrissCross`:
  - `PropertyEditorKind`
  - `PropertyDescriptorModel`
  - `PropertyDescriptorGroup`
  - `PropertyGridState`
- Added matching `PropertyGridLite` controls to WPF.UI and Avalonia.UI.
- Added WPF resource dictionary include and Avalonia theme include for the new property inspector surface.
- Covered shared descriptor grouping, search filtering, modified-state, and validation-gating behavior with TUnit tests.

## Behavior notes

The shared journal preserves forward entries after a back navigation and truncates them only when a new navigation is recorded from the historical position. This matches browser-style back/forward behavior and keeps WPF and Avalonia consistent.

The new command, busy, and empty-state controls deliberately use shared non-visual models from `CrissCross` and thin platform wrappers/templates. This keeps observable command status, busy operation state, and empty-state action availability consistent while preserving platform-native styling and templating.

The new search/filter controls follow the same pattern: `SearchQueryState` and `FilterToken` capture query text, submitted/debounced text, result counts, active filters, and stable token keys in the base library, while WPF and Avalonia expose thin `SearchBox`/`FilterBar` wrappers for native templates and command binding.

The new reactive form controls keep validation behavior platform-neutral: `ValidationMessage` and `ValidationSummaryState` provide explicit field keys, severities, blocking counts, pending validation state, and remediation command hooks, while `ReactiveFormField` and `ValidationSummary` are thin platform-native presenters.

The new chip and segmented controls keep compact selection behavior platform-neutral: `ChipModel` and `ChipGroupState` cover static tags, removable filter chips, and single/multiple-selection summaries, while `SegmentItem` and `SegmentedSelectionState` cover mode switching without forcing a platform-specific toggle implementation into the base library.

The new paging control keeps server/local pagination behavior platform-neutral: `PaginationState` covers page clamping, display ranges, and navigation availability, while `PageRequest` captures a stable page/search/filter/sort snapshot for reactive data pipelines.

The new stepper control keeps workflow navigation behavior platform-neutral: `StepDescriptor` carries explicit step status, optionality, validation messages, and enter/leave gates, while `StepperState` computes current step, progress text, blocking counts, and previous/next/finish availability for WPF and Avalonia presenters.

The new date/time range control keeps reporting and filtering windows platform-neutral: `DateTimeRange` captures start/end, validity, duration, display text, and inclusive containment, while `DateTimeRangePresetDefinition` resolves deterministic presets such as Today, Yesterday, Last 7 days, and This month against an explicit reference instant.

The new theme switcher keeps personalization state platform-neutral: `ThemePreferenceState` computes the effective concrete theme from the selected preference, host system theme, and high-contrast support, while WPF and Avalonia `ThemeSwitcher` controls call their platform `IThemeService` when consumers provide one.

The new data filter panel keeps structured grid/list filtering platform-neutral: `FilterDescriptor` and `FilterExpression` capture explicit field metadata, editor kind, supported operators, values, and stable token projection without reflection, while WPF and Avalonia `DataFilterPanel` controls expose apply/clear/add/remove command hooks and project active filters into `SearchQueryState` for data pipelines.

The new property inspector keeps settings/admin/designer inspection platform-neutral and AOT-friendly: `PropertyDescriptorModel` requires explicit descriptors instead of reflection scanning, `PropertyGridState` groups/searches descriptors and gates commit on validation, while WPF and Avalonia `PropertyGridLite` controls expose thin native templates and commit/reset command hooks.

## Remaining platform-specific gaps

- WPF retains richer `NavigationVMLeft`/`NavigationVMTop` user-control styles, while Avalonia exposes `NavigationControls`; exact one-to-one API parity still needs a design decision before adding duplicate abstractions.
- High-contrast resources remain WPF-only.
- Gallery parity for Avalonia navigation, text, media, numeric, toggle, password, and tree pages is not covered by this slice.
- Filter-token remove buttons are compile-verified but not yet wired to an Avalonia item-level command binding in the default template; consumers can still bind/remodel token removal through `FilterBar.RemoveFilterCommand` from custom templates.
- Chip, ChipGroup, SegmentedControl, DataPager, Stepper, and DateTimeRangePicker styles are compile-verified but not manually rendered in WPF/Avalonia galleries.
- DataPager emits page requests and exposes navigation command hooks, but page-size picker and numbered-page templates remain future visual polish.
- DateTimeRangePicker emits deterministic range snapshots and presets; richer shortcut menus, timezone labels, and validation integration with `ReactiveFormField` remain future visual/API polish.
- ThemeSwitcher styles are compile-verified but not manually rendered in WPF/Avalonia galleries; persistence remains consumer-owned through settings or application preferences.
- DataFilterPanel styles are compile-verified but not manually rendered in WPF/Avalonia galleries; saved filters, richer per-editor templates, and field-specific validation remain future API/visual polish.
- PropertyGridLite styles are compile-verified but not manually rendered in WPF/Avalonia galleries; typed editor templates, two-way value editing, per-field focus routing, and saved inspector layouts remain future API/visual polish.
