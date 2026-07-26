# MAUI UI parity

MAUI provides a focused, native-composition subset of the WPF UI surface. The
subset prioritises controls whose value is shared state, reactive commands, and
semantic presentation rather than replacing MAUI's built-in input, layout,
navigation, window, and virtualisation controls.

Every public MAUI control is instantiated in `CrissCross.Maui.UI.Gallery` and
uses the paired Light/Dark resources in `Resources/Styles/Colors.xaml` through
`AppThemeBinding`. The gallery demonstrates 27 controls:

`AlarmBanner`, `AsyncCommandButton`, `BusyOverlay`, `Card`, `CardAction`,
`CardColor`, `CardExpander`, `Chip`, `ChipGroup`, `CommandButton`,
`DataFilterPanel`, `DataPager`, `DateTimeRangePicker`, `EmptyState`,
`FilterBar`, `InfoBadge`, `InfoBar`, `PersonPicture`, `PropertyGridLite`,
`RatingControl`, `ReactiveFormField`, `SearchBox`, `SegmentedControl`,
`Snackbar`, `Stepper`, `ThemeSwitcher`, and `ValidationSummary`.

`AlarmBanner`, `CardColor`, and `Snackbar` are deliberate MAUI additions that
close shared WPF feedback/card parity using native `ContentView` composition
and reactive default commands. The remaining WPF controls fall into one of
these platform-inapplicable groups: native MAUI primitives (text/input, layout,
lists, menus, pickers, scrolling), desktop-only window/title/dialog hosts,
WPF-specific plotting/virtualisation, and WPF generated icon assets.
