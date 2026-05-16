# Zero Pragma Legacy Grandfathering

`agent.md` remains authoritative for new and touched production code: do not add `#pragma warning disable` and remove existing pragmas when a file is materially changed.

For the final validation wave, the following existing production-source pragmas are explicitly grandfathered as legacy debt and are out of scope for unrelated feature cards. They must not be copied into new code. Any card that changes one of these files should either remove the listed pragma by fixing the underlying warning or update this inventory with a narrower, reviewed rationale.

Generated files under `bin/` and `obj/`, example/test projects, galleries, and benchmark projects are excluded from this production-source inventory.

## Grandfathered production-source pragmas

- `src/CrissCross/RxObjectMixins.cs:20` — `#pragma warning disable RCS1175 // Unused 'this' parameter.`
- `src/CrissCross.Avalonia.UI/Controls/RichTextBox/TextPointer.cs:30` — `#pragma warning disable SA1201`
- `src/CrissCross.Avalonia.UI/Controls/RichTextBox/TextSegment.cs:209` — `#pragma warning disable SA1204 // Static members should appear before instance members`
- `src/CrissCross.WPF.UI/Controls/ResourceAccessor.cs:11` — `#pragma warning disable SA1310 // Field names should not contain underscore`
- `src/CrissCross.WPF.UI/Controls/SymbolFilled.cs:11` — `#pragma warning disable SA1602 // Enumeration items should be documented`
- `src/CrissCross.WPF.UI/Controls/SymbolRegular.cs:11` — `#pragma warning disable SA1602 // Enumeration items should be documented`
- `src/CrissCross.WPF.UI/Controls/VisualStateGroupListener.cs:43` — `#pragma warning disable SA1202 // Elements should be ordered by access`
- `src/CrissCross.WPF.UI/Controls/AutoSuggestBox/AutoSuggestBox.cs:154` — `#pragma warning disable SA1401 // Fields should be private`
- `src/CrissCross.WPF.UI/Controls/ContentDialog/ContentDialog.cs:272` — `#pragma warning disable SA1401 // Fields should be private`
- `src/CrissCross.WPF.UI/Controls/GridView/GridViewColumn.cs:43` — `#pragma warning disable SA1202 // Elements should be ordered by access`
- `src/CrissCross.WPF.UI/Controls/IconElement/FontIcon.cs:70` — `#pragma warning disable SA1401 // Fields should be private`
- `src/CrissCross.WPF.UI/Controls/IconElement/ImageIcon.cs:27` — `#pragma warning disable SA1401 // Fields should be private`
- `src/CrissCross.WPF.UI/Controls/NavigationView/NavigationView.Properties.cs:35` — `#pragma warning disable SA1202 // Elements should be ordered by access`
- `src/CrissCross.WPF.UI/Controls/NavigationView/NavigationView.TemplateParts.cs:31` — `#pragma warning disable SA1401 // Fields should be private`
- `src/CrissCross.WPF.UI/Controls/NavigationView/NavigationViewItem.cs:44` — `#pragma warning disable SA1202 // Elements should be ordered by access`
- `src/CrissCross.WPF.UI/Controls/NavigationView/NavigationViewItem.cs:159` — `#pragma warning disable SA1401 // Fields should be private`
- `src/CrissCross.WPF.UI/Controls/PersonPicture/PersonPicture.properties.cs:103` — `#pragma warning disable SA1202 // Elements should be ordered by access`
- `src/CrissCross.WPF.UI/Controls/PersonPicture/PersonPictureTemplateSettings.cs:30` — `#pragma warning disable SA1202 // Elements should be ordered by access`
- `src/CrissCross.WPF.UI/Controls/Snackbar/Snackbar.cs:127` — `#pragma warning disable SA1401 // Fields should be private`
- `src/CrissCross.WPF.UI/Controls/Snackbar/Snackbar.cs:128` — `#pragma warning disable SA1600 // Elements should be documented`
- `src/CrissCross.WPF.UI/Controls/SplitButton/SplitButton.cs:37` — `#pragma warning disable SA1401 // Fields should be private`
- `src/CrissCross.WPF.UI/Controls/VirtualizingWrapPanel/VirtualizingWrapPanel.cs:63` — `#pragma warning disable SA1401 // Fields should be private`
- `src/CrissCross.WPF.UI/Interop/Shell32.cs:110` — `#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.`
- `src/CrissCross.WPF.UI/MagicInterfaces/AppBarMixins.cs:9` — `#pragma warning disable RCS1175 // Unused 'this' parameter`
- `src/CrissCross.WPF.UI/MagicInterfaces/MessageBoxShowMixins.cs:10` — `#pragma warning disable RCS1175 // Unused 'this' parameter`
- `src/CrissCross.WPF.UI/Markup/Design.cs:15` — `#pragma warning disable SA1401 // Fields should be private`
- `src/CrissCross.WPF.UI/Services/ApplicationHostService.cs:10` — `#pragma warning disable CA1812`
- `src/CrissCross.WPF.UI/Services/ApplicationVMHostService.cs:8` — `#pragma warning disable CA1812`
- `src/CrissCross.WPF.UI/Services/PageService.cs:6` — `#pragma warning disable CA1812`
- `src/CrissCross.WPF.WebView2/WebView2Wpf.cs:37` — `#pragma warning disable SA1202 // Elements should be ordered by access`
