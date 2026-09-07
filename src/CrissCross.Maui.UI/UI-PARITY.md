# MAUI UI parity

MAUI provides a focused, native-composition subset of the WPF UI surface. The
subset prioritises controls whose value is shared state, reactive commands, and
semantic presentation rather than replacing MAUI's built-in input, layout,
navigation, window, and virtualisation controls.

Every public MAUI control is instantiated in `CrissCross.Maui.UI.Gallery` and
uses dynamic semantic tokens backed by system-following Light/Dark resources plus
explicit Light, Dark, and High Contrast resource dictionaries. The gallery
demonstrates 28 controls:

`AlarmBanner`, `AsyncCommandButton`, `BusyOverlay`, `Card`, `CardAction`,
`CardColor`, `CardExpander`, `Chip`, `ChipGroup`, `CommandButton`,
`DataFilterPanel`, `DataPager`, `DateTimeRangePicker`, `EmptyState`,
`FilterBar`, `InfoBadge`, `InfoBar`, `PersonPicture`, `PropertyGridLite`,
`ProcessValueIndicator`, `RatingControl`, `ReactiveFormField`, `SearchBox`, `SegmentedControl`,
`Snackbar`, `Stepper`, `ThemeSwitcher`, and `ValidationSummary`.

`AlarmBanner`, `CardColor`, and `Snackbar` are deliberate MAUI additions that
close shared WPF feedback/card parity using native `ContentView` composition
and reactive default commands. The remaining WPF controls are not implemented in the shared MAUI.UI package. Some map to native MAUI primitives (text/input, layout, lists, menus, pickers, scrolling). Others are incomplete for MAUI Windows until there is a MAUI-native design for desktop window chrome, title/dialog hosts, virtualization, plotting, rich text, media, and generated icon assets.
