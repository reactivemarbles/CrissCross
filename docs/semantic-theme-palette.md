# Cross-platform semantic theme palette

CrissCross UI controls use semantic resources instead of embedding visual colors in
control templates.  The same roles are available in every supported UI platform,
although the native resource syntax differs.

| Semantic role | WPF and Avalonia resource | MAUI resource |
| --- | --- | --- |
| Application surface | `ApplicationBackgroundColor` | `CrissCrossSurfaceColor` |
| Subtle control surface | `ControlFillColorSecondary` | `CrissCrossSubtleSurfaceColor` |
| Primary and secondary text | `TextFillColorPrimary`, `TextFillColorSecondary` | `CrissCrossTextColor`, `CrissCrossMutedTextColor` |
| Border and focus surface | `ControlStrokeColorDefault`, `FocusStrokeColorOuter` | `CrissCrossBorderColor` |
| Accent and its foreground | `SystemAccentColorPrimary`, `TextOnAccentFillColorPrimary` | `CrissCrossAccentColor`, `CrissCrossAccentTextColor` |
| Attention, success, caution, critical, neutral | `SystemFillColor*` and `SystemFillColor*Background` | `CrissCrossAttention*`, `CrissCrossSuccess*`, `CrissCrossCaution*`, `CrissCrossDanger*`, `CrissCrossNeutral*` |
| Modal/busy overlay | `SmokeFillColorDefault` | `CrissCrossOverlayColor` |

WPF and Avalonia keep matching Light/Dark dictionary keys.  MAUI uses
`AppThemeColor` resources so the operating system theme changes each role without
requiring a template reload.  New controls must consume these resources through
dynamic/theme resource lookup rather than literal colors.

The primary text and accent foreground pairs meet the WCAG 2.1 AA 4.5:1 contrast
threshold in both Light and Dark variants.  Secondary text is intended for
supporting information and maintains at least 3:1 contrast on its application
surface.
