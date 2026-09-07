# Quality and parity validation

Date: 2026-09-07

## Changes

- Fixed pagination integer overflow and added boundary tests.
- Added immutable process-value state with invalid-configuration, data-quality, and inclusive alarm thresholds. WPF, Avalonia, and MAUI provide a `ProcessValueIndicator` with the same state projections, compact layout, typography, and semantic status colors. Each gallery includes operating, alarm, and unavailable examples.
- Moved Avalonia navigation `ViewModelProperty` ownership to non-generic base classes. The typed `ViewModel` wrappers remain; the public property-handle migration is documented in `navigation-property-migration.md`.
- Added stable logical navigation host names, idempotent setup subscriptions, and disposal of desktop window hosts. Unregistration checks reference identity before removing aliases belonging to a host.
- Fixed plot crosshair subscription replacement and WebView2 pending-window handler disposal. Regressions exercise runtime lifecycle behavior.
- Adopted `ReactiveUI.Binding` observable primitives and generated `ReactiveUI.Primitives.ObservableEvents` in WPF plot views, retaining the existing ReactiveUI view contract for command bindings. Binding dispatch generation emitted no implementations for these WPF call sites, so calling its generated-only fallback methods caused an activation exception despite a clean build; explicit Binding primitives replace those calls.
- Corrected the GIF stream factory's accidental self-recursion.
- Removed production `SuppressMessage` attributes and all tracked `NoWarn` project/build entries. Analyzer fixes change code rather than introduce suppressions. Existing `.editorconfig` diagnostic configuration remains in effect.
- Added explicit Avalonia high contrast resources. Runtime startup requires the custom variant key to be referenced with `x:Static`; a string key compiles but fails when the gallery loads.
- Corrected Avalonia navigation attachment names and transition cancellation. Native gallery navigation now renders the current view; rapid replacement and disposal tests cover both standard and reactive hosts.
- Avalonia wrapper themes reuse native templates explicitly while retaining CrissCross style selection. Rendering tests verify workflow templates, filter removal, chip command fallback, null-state clearing, and derived button styling. Native checks confirm filter labels/actions, workflow step navigation, and navigation after closing another window.
- Added all nine Avalonia button appearances, semantic hover/pressed/disabled brushes, and derived command-button templates. Rendering tests check each appearance in Light, Dark, and HighContrast, including disabling a pressed button.
- Corrected Avalonia disabled text opacity in HighContrast, retained compact wrapping for filter/chip items, and added wrapping to long gallery checkbox labels.
- Corrected Gel button foregrounds and modern checkbox selection glyphs to use the active accent foreground. Their disabled opacity now remains fully visible in HighContrast; attached-window tests cover Light, Dark, and HighContrast.
- Replaced 85 red placeholder color values in each WPF high-contrast palette with palette-consistent resources and distinct success/caution/critical colors. Four native resource-loading tests cover the status regression.
- Fixed the Avalonia gallery import Cancel action to cancel its operation instead of clearing the query. Filter removal and clear actions preserve subsequent query changes.
- Added actual MAUI busy, validation, empty-state, stepper, filter, property editor, date-range, pagination, and form-field compositions. Tests exercise content preservation, command availability, nullable dates and per-endpoint offsets, and theme changes.
- Avalonia tests use a dedicated native dispatcher and TUnit executor; MAUI handler-free tests install a deterministic binding dispatcher.

## Verification recorded so far

These are fresh, bounded results, not a claim of whole-solution coverage or absence of bugs. Later integration runs supersede these results when source changes affect the tested projects.

| Verification | Result |
| --- | --- |
| Full solution Release build with warnings as errors, `build-full12.log` | 0 warnings, 0 errors; completed in 7m 33s on Windows after the final source changes |
| Core/MAUI TUnit suite, `test-core-final16.log` | 305 passed |
| Reactive TUnit suite, `test-reactive-final2.log` | 155 passed |
| Navigation TUnit suite, `test-navigation-final28.log` | 137 passed |
| WPF gallery TUnit suite, `test-wpf-gallery-final10.log` | 32 passed |
| Standard and reactive plot TUnit suites, `test-plot-final6.log` and `test-reactive-plot-final2.log` | 41 passed each |
| WPF.UI coverage in its gallery test report | 52.44% lines; 21.47% branches |
| Avalonia.UI coverage in the navigation test report | 60.65% lines; 41.97% branches |
| Avalonia.UI.Reactive coverage in the navigation test report | 47.47% lines; 14.99% branches |
| Shared core coverage from the core run | 99.94% lines (1792/1793); 94.93% branches (712/750) |
| MAUI.UI coverage from the core run | 92.12% lines (1812/1967); 71.52% branches (545/762) |
| `ProcessValueState` coverage | 100% lines and branches |
| MAUI `ProcessValueIndicator` coverage | 100% lines and branches, including arranged range width and theme changes |
| WPF.UI non-incremental net10 Windows analyzer build, pass 14 | 0 warnings, 0 errors |
| Avalonia gallery net10 analyzer build, pass 8 | 0 warnings, 0 errors |
| Avalonia gallery native launch | Final industrial layout verified in Dark, Light, HighContrast; first window navigates after closing the second; selection labels readable |
| MAUI gallery Windows build, `build-maui-gallery-final3.log` | 0 warnings, 0 errors |
| MAUI gallery native launch | Busy overlay, step navigation, property edit/commit, readable avatar initials, and HighContrast → Light → Dark → HighContrast switching verified after ReactiveUI startup registration |

Coverage was inspected through Mtpunittestmcp using individual fresh Cobertura reports. Old reports from earlier runs must not be merged to represent current coverage. Test execution uses TUnit and Microsoft Testing Platform with builds enabled.

## Integrated validation

- The full solution build passed with warnings as errors across the target frameworks configured by the solution on this Windows host. The final source was unchanged throughout this build.
- All six TUnit projects have passed with fresh per-run coverage (711 tests), including the modern checkbox selection-glyph correction.
- Native Avalonia checks verify Gel button contrast, wrapped checkbox labels, compact filter tokens and removal, step navigation, and revised radio/icon resources. The corrected modern checkbox selection glyph was also verified in HighContrast and Light.
- The original two-window gallery reproduction and real-host automated regression now pass after preserving owner host identity.
- The six passing suites include high contrast resource loading, navigation cleanup, GIF stream factory, plot binding/subscription lifetime, and WebView2 pending-window lifecycle regressions. These bounded checks do not replace native device testing for every supported platform.

## Remaining parity boundaries

WPF/Avalonia wrappers and native MAUI equivalents are not automatically equivalent just because similarly named controls exist. The [Avalonia inventory](../src/CrissCross.Avalonia.UI/UI-PARITY.md), [MAUI inventory](../src/CrissCross.Maui.UI/UI-PARITY.md), and [theme audit](theme-parity-audit.md) describe remaining gaps.

- MAUI exposes 28 composed controls; desktop-capable gaps include rich text, media, virtualization, plotting, and window/dialog hosting. Mobile size constraints do not account for all these gaps.
- Avalonia `Frame` and `Page` remain compatibility wrappers without WPF-equivalent navigation behavior.
- WPF has four named high-contrast palettes; Avalonia and MAUI currently expose one explicit palette. Complete control-by-control visual and interaction parity remains unverified.
- Desktop UI branch coverage remains substantially below 100%, and supported mobile devices have not received full native visual and interaction validation.

This work does not claim complete UI parity, 100% whole-solution coverage, or a proof that no bugs remain.
