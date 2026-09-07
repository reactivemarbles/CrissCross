# Theme parity audit

This audit distinguishes resource parity from rendered control parity. Equal resource keys or similarly named classes alone do not establish the same appearance or behavior.

## Implemented corrections

- Avalonia provides Light, Dark, and explicit HighContrast dictionaries with the same 276 semantic resource keys. The custom high-contrast variant inherits Dark and is registered with an `x:Static` key so resource loading succeeds at runtime.
- Avalonia theme selection round-trips HighContrast through `ApplicationThemeManager`.
- WPF theme detection recognizes its existing HC1, HC2, HCBlack, and HCWhite resource dictionaries.
- MAUI provides dynamic semantic resources for Light, Dark, and HighContrast. Default and System selection follow application theme changes; explicit choices remain fixed. Theme dictionary replacement avoids accumulating obsolete overrides.
- Avalonia thin wrappers reuse native control themes through explicit theme setters. Their own style keys remain available for CrissCross typography, appearance and behavior selectors; inherited native style keys previously bypassed those selectors. Workflow controls retain their dedicated item templates. Fresh rendering tests verify this integration.
- Avalonia CheckBox and RadioButton string content inherits the semantic foreground, including the disabled state.
- The new ProcessValueIndicator uses shared state, typography, spacing, a range bar, and semantic condition colors across WPF, Avalonia, and MAUI. Each gallery demonstrates nominal, alarm, and bad-quality readings.

## Evidence and validation boundaries

The Avalonia gallery was launched natively in Dark, Light, and HighContrast. This exposed and led to fixes for an invalid theme dictionary key and missing Expander headers. The compact process readout and all nine button appearances were verified in Dark, Light, and HighContrast. RadioButton checked indicators have been verified in Light and HighContrast; button icons are readable across the three themes. Rendering tests verify button hover/pressed/disabled states. Native HighContrast checks also verify readable disabled radio labels, Gel button text, wrapped checkbox labels, compact filter tokens and removal, and workflow step navigation. Modern checkbox selection glyphs now use the semantic accent foreground, with native HighContrast and Light checks and automated coverage across all three themes.

The final full solution Release build passed with warnings treated as errors: zero warnings and zero errors. All six TUnit/MTP suites passed (711 tests). Project NoWarn entries were removed; see `quality-audit.md` for exact build, test, and coverage results.

Tests cover semantic key completeness, contrast calculations, theme variant ownership, state projections, and resource switching. A source-resource assertion is not a substitute for rendered visual verification.

## Remaining boundaries

- WPF has four named high-contrast palettes; Avalonia and MAUI expose one explicit high-contrast palette.
- Native platform controls can differ in rendering, sizing, focus, accessibility, and interaction even when their semantic colors match.
- MAUI currently exposes 28 composed controls. Desktop-capable gaps include rich text, media, virtualization, plotting, and window/dialog hosting. These gaps cannot all be justified by mobile screen constraints.
- Complete gallery control-by-control visual and functional parity, including all supported mobile devices, has not yet been established.
