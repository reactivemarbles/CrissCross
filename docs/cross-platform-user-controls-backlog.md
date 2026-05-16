# Cross-platform UserControls backlog

Research card: popular/common UserControls suitable for `CrissCross.WPF.UI`, `CrissCross.Avalonia.UI`, and a future or expanded `CrissCross.MAUI.UI` surface with a shared visual language.

## Current inventory evidence

Inspected platform UI projects under `src/`.

- `CrissCross.WPF.UI/Controls` contains 97 control folders.
- `CrissCross.Avalonia.UI/Controls` contains 99 control folders and is nearly symmetrical with WPF; only `Alarms` and `MenuItem` are Avalonia-only folders.
- `CrissCross.MAUI` currently contains framework helpers only (`NavigationShell.cs`, `AssemblyInfo.cs`) and no reusable UI control folder/package.
- Existing WPF/Avalonia controls already cover many candidate items: `Snackbar`, `ContentDialog`, `LoadingScreen`, `ProgressRing`, `NavigationView`, `BreadcrumbBar`, `Card`, `CardExpander`, `Expander`, `Badge`, `InfoBadge`, `PersonPicture`, `AutoSuggestBox`, `DatePicker`, `TimePicker`, `DateTimePicker`, `ProgressBar`, `InfoBar`, `Flyout`, and virtualization controls.

Conclusion: the backlog should not duplicate the existing Fluent-style primitives. It should add composed, reactive, application-level controls that consume the existing primitives and expose a consistent contract across WPF, Avalonia, and MAUI.

## Recommendation summary

Prioritize controls that solve repeated application workflows and can share non-visual state models:

1. Command execution surfaces.
2. Reactive form validation.
3. Busy/overlay state.
4. Search/filter and empty/data states.
5. Selection tokens and segmented input.
6. Workflow/data navigation controls.
7. Theme/profile conveniences.
8. Descriptor-driven inspector controls.

Avoid high-cost platform-specific clones where an existing native primitive already fits. For example, use existing `Snackbar` rather than a separate toast unless native OS notifications are explicitly in scope, and use existing `NavigationView` before adding a drawer-specific control.

## Prioritized backlog

### P0-1: `CommandButton` / `AsyncCommandButton`

Rationale: Reactive applications repeatedly need a button that binds directly to `ReactiveCommand`, shows execution progress, disables from `CanExecute`, exposes errors, and optionally confirms destructive commands. Existing `Button`, `SplitButton`, `DropDownButton`, `ProgressRing`, `ContentDialog`, and `Snackbar` can be composed, but there is no first-class command execution surface.

Shared abstractions:

- `CommandButtonState`: `Idle`, `Executing`, `Succeeded`, `Failed`, `Cancelled`.
- `CommandButtonAppearance`: primary, secondary, destructive, subtle.
- `IObservable<bool>`/`ReactiveCommand` inputs for `IsExecuting`, `CanExecute`, and errors.
- Optional confirmation request model using existing dialog services.

Platform feasibility:

- WPF: high; template existing `Button` plus `ProgressRing` and `ContentDialogService`.
- Avalonia: high; same model and template with existing controls.
- MAUI: high; `ContentView`/`TemplatedView` with `Button` and `ActivityIndicator`.

Gallery demo requirements:

- Normal, disabled, executing, success, faulted, destructive-confirm states.
- Bound `ReactiveCommand<Unit, Unit>` with cancellation-aware async execution.
- Error routed to existing snackbar/infobar service.

### P0-2: `ReactiveFormField` and `ValidationSummary`

Rationale: Business apps need field-level validation, async validation, required markers, help text, error summaries, and submit gating. Existing input controls are strong, but the shared form pattern is missing.

Shared abstractions:

- `ValidationMessage` with severity, field key, message, optional remediation command.
- `IValidationSource` or observable `IReadOnlyList<ValidationMessage>`.
- `FormFieldState`: normal, focused, valid, warning, invalid, pending.
- AOT-friendly descriptors: avoid reflection; consumers provide field keys and display names explicitly.

Platform feasibility:

- WPF: high; wraps content presenter plus existing `TextBox`, `ComboBox`, `DatePicker`, etc.
- Avalonia: high; equivalent templated container.
- MAUI: medium-high; validation visuals vary by platform but model is portable.

Gallery demo requirements:

- Login form with sync and async validation.
- Required/optional labels, helper text, warning vs error severity.
- Summary click/focus navigation to invalid fields.
- Submit `ReactiveCommand` enabled only when valid and not pending.

### P0-3: `BusyOverlay` / `LoaderOverlay`

Rationale: Existing `LoadingScreen` and progress controls cover full-screen or indicator scenarios, but apps commonly need an overlay over a content region while preserving layout and cancellation affordances.

Shared abstractions:

- `BusyOverlayState`: inactive, indeterminate, determinate, cancellable.
- `BusyOperation` model: title, message, progress, cancellation command.
- Observable binding for active operation and progress.

Platform feasibility:

- WPF: high; adorner/grid overlay over content.
- Avalonia: high; overlay layer/content control.
- MAUI: high; grid overlay with `ActivityIndicator`.

Gallery demo requirements:

- Regional overlay, full-page overlay, determinate progress, cancel button.
- Multiple queued operations collapsed into the highest-priority message.
- Accessibility: announce busy state and preserve keyboard focus after completion.

### P1-1: `SearchBox` and `FilterBar`

Rationale: `AutoSuggestBox` exists, but many apps need a lower-friction search box with debounce, clear button, search command, result count, recent terms, and filter chips. This should compose with data grids/lists without forcing suggestion providers.

Shared abstractions:

- `SearchQueryState`: text, debounced text, submitted text, is searching, result count.
- `FilterToken`: field, operator, value, display text, removable flag.
- Observable query stream and `ReactiveCommand<string, Unit>` submission hook.

Platform feasibility:

- WPF: high; compose `TextBox`, icons, `Chip` backlog item, and optional flyout.
- Avalonia: high; same composition.
- MAUI: high; `SearchBar` or custom `ContentView` wrapper.

Gallery demo requirements:

- Debounced local search over sample collection.
- Async remote-search simulation with cancellation on text changes.
- Active filters displayed as removable chips.

### P1-2: `EmptyState`

Rationale: Every list, grid, search, and dashboard needs a consistent empty/error/no-results state. This is simple, common, and improves product polish across all platforms.

Shared abstractions:

- `EmptyStateModel`: title, message, icon, primary action, secondary action, variant (`NoData`, `NoResults`, `Error`, `Offline`, `PermissionRequired`).
- Optional observable state resolver for collection count/search/error.

Platform feasibility:

- WPF: high.
- Avalonia: high.
- MAUI: high.

Gallery demo requirements:

- No data, no search results, offline, and permission-required states.
- Primary action bound to a `ReactiveCommand`.
- Compact and full-page layouts.

### P1-3: `Chip`, `Tag`, and `ChipGroup`

Rationale: Chips/tags are absent as first-class controls and are required by search filters, selection summaries, labels, and compact categorical data. Current `Badge`/`InfoBadge` are status indicators rather than removable/selectable tokens.

Shared abstractions:

- `ChipModel`: text, icon, color/accent, selected, removable, disabled.
- `ChipGroupSelectionMode`: none, single, multiple.
- Observable add/remove/select events.

Platform feasibility:

- WPF: high; styles over `ToggleButton`/`ItemsControl`.
- Avalonia: high; same concept.
- MAUI: medium-high; custom layout for wrapping chips may require platform tuning.

Gallery demo requirements:

- Static tags, removable filter chips, single-select and multi-select groups.
- Keyboard navigation and accessible remove action.

### P1-4: `SegmentedControl`

Rationale: Common for mode switching, density/theme variants, and mobile-friendly choices. It is not present, while existing buttons/toggle buttons provide the building blocks.

Shared abstractions:

- `SegmentItem`: key, text, icon, enabled.
- Single-selection observable selected key.
- Equal-width and content-width layout modes.

Platform feasibility:

- WPF: high.
- Avalonia: high.
- MAUI: high, and particularly valuable on mobile.

Gallery demo requirements:

- Text-only, icon+text, compact, and disabled segment states.
- Bound selected key drives a sample view switch.

### P1-5: `DataPager` / `Pagination`

Rationale: Data-heavy apps need a standard pager that composes with `DataGrid`, `ListView`, and remote APIs. Existing virtualization controls help large local data but do not express page state or server-side navigation.

Shared abstractions:

- `PaginationState`: page index, page size, total item count, total pages.
- `PageRequest`: page index, page size, sort/filter snapshot.
- `ReactiveCommand<PageRequest, Unit>` or observable request stream.

Platform feasibility:

- WPF: high.
- Avalonia: high.
- MAUI: medium-high; compact/mobile layout needed.

Gallery demo requirements:

- Local collection paging and simulated remote paging.
- Page size picker, first/previous/next/last, numbered pages, loading state.
- Interaction with `FilterBar` query state.

### P2-1: `Stepper` / `WizardProgress`

Rationale: Setup flows, onboarding, import workflows, and multi-step forms need visible progress and guarded navigation. This is application-level and reactive-command-friendly.

Shared abstractions:

- `StepDescriptor`: key, title, status, optional validation state, can enter/can leave.
- `StepperOrientation`: horizontal, vertical, compact.
- Commands for next, previous, jump, cancel, finish.

Platform feasibility:

- WPF: high.
- Avalonia: high.
- MAUI: high for mobile onboarding; responsive layout matters.

Gallery demo requirements:

- Three-step wizard with validation gating.
- Optional/skipped/error/completed states.
- Horizontal desktop and vertical narrow layouts.

### P2-2: `DateTimeRangePicker`

Rationale: Date/time range selection is common for reporting, logs, bookings, and dashboards. Existing date/time picker primitives exist, but there is no range model or shortcut preset surface.

Shared abstractions:

- `DateTimeRange`: start, end, inclusive/exclusive policy, time zone/offset policy.
- `RangePreset`: today, yesterday, last 7 days, this month, custom.
- Validation for empty, reversed, and maximum-length ranges.

Platform feasibility:

- WPF: medium-high; compose existing date/time controls and popup/flyout.
- Avalonia: medium-high; same.
- MAUI: medium; platform date/time pickers differ and need a compact mobile flow.

Gallery demo requirements:

- Presets, custom range, date-only and date-time modes.
- Invalid range validation integrated with `ReactiveFormField`.
- Emits observable range changes for chart/grid filtering.

### P2-3: `ThemeSwitcher`

Rationale: Theme services already exist, but apps need a visible switcher for light/dark/system modes, accent preview, and persisted preference. This should be a small control over `IThemeService` rather than another theme service.

Shared abstractions:

- `ThemeChoice`: system, light, dark, high contrast where supported.
- `ThemePreferenceState`: selected preference, effective theme, system theme.
- Optional accent color model if cross-platform token support is added.

Platform feasibility:

- WPF: high; existing `ThemeService` and theme resources.
- Avalonia: high; existing `IThemeService`.
- MAUI: medium-high; maps to `AppTheme` and platform capabilities.

Gallery demo requirements:

- Toggle mode and dropdown mode.
- System/light/dark effective theme display.
- Persistence simulation through observable settings state.

### P2-4: `DataFilterPanel`

Rationale: Grids and lists often need a more structured filter editor than a search box: fields, operators, values, saved filters, and clear/apply behavior.

Shared abstractions:

- `FilterDescriptor`: field key, display name, supported operators, editor kind.
- `FilterExpression`: field, operator, value, logical grouping.
- No reflection requirement; apps supply descriptors explicitly for trimming/AOT safety.

Platform feasibility:

- WPF: medium-high.
- Avalonia: medium-high.
- MAUI: medium; mobile should use modal/bottom-sheet style editing.

Gallery demo requirements:

- Filter a sample grid by text, enum, number range, and date range.
- Apply/clear/save filter flows.
- Compact chip summary via `ChipGroup`.

### P3-1: `PropertyGridLite` / `ObjectInspector`

Rationale: A property inspector is useful for admin tools, designers, settings pages, and diagnostics. It is also the highest AOT risk if implemented through reflection; it should be descriptor-driven and optional.

Shared abstractions:

- `PropertyDescriptorModel`: key, display name, category, editor kind, value observable, setter command, validation messages.
- Editor kinds: text, number, boolean, enum, color, date/time, command, custom template key.
- Explicit descriptor providers; no implicit reflection scanner in hot paths.

Platform feasibility:

- WPF: medium.
- Avalonia: medium.
- MAUI: medium-low unless scoped to simple editors first.

Gallery demo requirements:

- Inspect and edit a sample settings object via descriptors.
- Categories, read-only fields, validation, reset command.
- Demonstrate no reflection is required for the primary path.

### P3-2: `NavigationDrawer`

Rationale: A drawer is common on mobile and web-like layouts, but WPF/Avalonia already have `NavigationView`. Add only if the goal is a lighter, responsive, MAUI-friendly drawer abstraction rather than another desktop navigation control.

Shared abstractions:

- `NavigationDrawerItem`: route key, title, icon, badge count, enabled.
- `DrawerDisplayMode`: modal, inline, compact, bottom.
- Route command integration with existing navigation services.

Platform feasibility:

- WPF: medium; can map to `NavigationView`/flyout-like presentation.
- Avalonia: medium; can map to `NavigationView`.
- MAUI: high through `Shell`/flyout concepts, but API mapping must be careful.

Gallery demo requirements:

- Responsive desktop vs narrow/mobile layouts.
- Route selection, nested items, badge count, disabled route.

### P3-3: `DialogHost` / `OverlayHost`

Rationale: `ContentDialogService` already exists in WPF/Avalonia. A host abstraction is still useful if it standardizes overlays, nested dialogs, modal stacking, sheet presentation, and testable interaction requests across platforms.

Shared abstractions:

- `OverlayRequest<TResult>`: content key/model, modality, placement, dismissal policy.
- `IOverlayHostService` with observable active overlays and async result API.
- Modal stack policy and cancellation semantics.

Platform feasibility:

- WPF: medium; must coexist with current `ContentDialogService`.
- Avalonia: medium; same.
- MAUI: medium; maps to modal pages/popups/sheets depending dependencies.

Gallery demo requirements:

- Alert, confirmation, custom form dialog, side sheet, bottom sheet where supported.
- Nested request blocked or queued policy explicitly shown.

## Candidates already covered or lower priority

| Candidate | Current state | Recommendation |
| --- | --- | --- |
| Snackbar/toast | `Snackbar` and `ISnackbarService` exist in WPF/Avalonia; toast-specific native notifications are not present. | Do not add a duplicate toast control. Consider a later native notification service only if OS-level notifications are in scope. |
| Dialog/content dialog | `ContentDialog`, `MessageBox`, async variants, and services exist in WPF/Avalonia. | Prefer `DialogHost` only as a host/service abstraction, not a replacement. |
| Loader/progress | `LoadingScreen`, `ProgressRing`, and `ProgressBar` exist. | Add `BusyOverlay` as a composed workflow control. |
| Navigation drawer | `NavigationView` exists. | Lower priority; focus on MAUI/responsive parity if added. |
| Breadcrumb | `BreadcrumbBar` exists. | No new backlog item unless gallery coverage is missing. |
| Card/expander | `Card`, `CardControl`, `CardExpander`, `Expander` exist. | No new backlog item. |
| Badge/avatar | `Badge`, `InfoBadge`, and `PersonPicture` exist. | Add chips/tags, not another badge/avatar. |
| Search suggestions | `AutoSuggestBox` exists. | Add `SearchBox` for query/debounce/filter composition. |
| Theme services | `IThemeService`/theme resources exist. | Add `ThemeSwitcher` as UX surface only. |

## Core shared abstractions needed

These can live in a shared UI abstractions layer or be duplicated as matching source-compatible models per platform until a package boundary is chosen.

1. Visual token contract
   - Shared names for spacing, corner radius, elevation/shadow depth, typography roles, icon size, severity brushes, and interaction states.
   - Purpose: WPF/Avalonia/MAUI controls can look related even when templates differ.

2. Reactive command surface model
   - Minimal model around `ReactiveCommand`, `IsExecuting`, `CanExecute`, cancellation, confirmation, and error notification.
   - Purpose: `CommandButton`, wizard commands, empty-state actions, and filter application share execution behavior.

3. Validation model
   - Field-keyed validation messages with severity and async pending state.
   - Purpose: `ReactiveFormField`, `ValidationSummary`, `Stepper`, `DateTimeRangePicker`, and `PropertyGridLite` use the same validation semantics.

4. Overlay/feedback request model
   - Request records for snackbar/infobar/dialog/overlay scenarios, plus observable active state.
   - Purpose: decouple viewmodels from platform-specific presenters without reintroducing global static state.

5. Data interaction state
   - `SearchQueryState`, `FilterExpression`, `PaginationState`, and `DateTimeRange` records.
   - Purpose: search, filtering, paging, and date range controls compose cleanly with data grids/lists and reactive data pipelines.

6. Descriptor-driven editor metadata
   - Explicit property/filter/form descriptors; no default reflection scanner.
   - Purpose: supports AOT/trimming and keeps property-grid/filter/form controls predictable.

7. Gallery demo manifest
   - A small platform-neutral manifest model: control name, category, status, route key, feature flags, and sample scenarios.
   - Purpose: WPF, Avalonia, and MAUI galleries can show equivalent sample coverage without hand-maintained divergence.

## Platform sequencing

1. WPF + Avalonia first for P0/P1 controls because both already have matching control libraries, themes, galleries, dialog/snackbar services, and similar XAML templating models.
2. Define shared records and demo manifest before MAUI implementation, because `CrissCross.MAUI` currently has no UI-control library surface.
3. For MAUI, start with simple composed `ContentView` controls: `CommandButton`, `EmptyState`, `BusyOverlay`, `SegmentedControl`, and `ChipGroup` before heavier controls like `PropertyGridLite`.
4. Keep all shared contracts AOT-friendly: explicit descriptors, no reflection-heavy auto-discovery, observable state instead of global service lookups.

## Suggested gallery categories

- Commanding: `CommandButton`, destructive confirmation, cancellation, error feedback.
- Forms: `ReactiveFormField`, `ValidationSummary`, validation-gated submit.
- Feedback: `BusyOverlay`, existing snackbar/infobar integration.
- Search and data: `SearchBox`, `FilterBar`, `DataFilterPanel`, `Pagination`, `EmptyState`.
- Selection: `ChipGroup`, `SegmentedControl`.
- Workflows: `Stepper`, `DateTimeRangePicker`.
- Personalization: `ThemeSwitcher`.
- Advanced: `PropertyGridLite`, `DialogHost`/`OverlayHost`, responsive `NavigationDrawer`.

## Residual risks

- MAUI currently lacks a dedicated UI library package, so cross-platform API shape needs a package-boundary decision before implementation.
- Controls that look simple in WPF/Avalonia may need platform-specific accessibility and layout work in MAUI.
- `PropertyGridLite` can easily violate AOT goals if descriptor-driven constraints are not enforced.
- Adding both `DialogHost` and existing content-dialog services could fragment modal APIs unless it is explicitly positioned as a host abstraction.
