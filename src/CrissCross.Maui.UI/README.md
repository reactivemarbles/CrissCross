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

The controls intentionally expose bindable state snapshots and command hooks instead of reflection-heavy discovery or platform-specific renderer hacks. Add the shared resource dictionary with:

```csharp
Application.Current.Resources.UseCrissCrossMauiUiResources();
```

Limitations:

- The first slice focuses on testable bindable properties, state projection, and reusable styles.
- Rich visual templates, platform-specific accessibility tuning, and gallery pages remain follow-up work.
- MAUI handler-level customization is intentionally avoided so the package remains AOT/trimming friendly.
