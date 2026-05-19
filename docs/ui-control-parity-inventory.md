# WPF.UI and Avalonia.UI control parity inventory

Scope: `src/CrissCross.WPF.UI`, `src/CrissCross.WPF.UI.Gallery`, `src/CrissCross.WPF.UI.Test`, `src/CrissCross.Avalonia.UI`, and `src/CrissCross.Avalonia.UI.Gallery`.

This is the discovery artifact for kanban task `t_666935f6`. It records current parity and intentionally does not implement controls.

## Evidence summary

- `CrissCross.WPF.UI` currently contains 408 C# files, 138 XAML files, 68 RESX files, font assets, and image assets.
- `CrissCross.Avalonia.UI` currently contains 206 C# files and 101 AXAML files.
- Top-level `Controls/` folder parity is mostly aligned: WPF has 97 control folders; Avalonia has those same folders plus `Alarms` and `MenuItem`.
- The larger gaps are not top-level folders; they are subcontrols/user controls, resource/theme coverage, attached behaviors/helpers, and gallery/demo coverage.
- WPF gallery/test examples contain 25 view XAML files plus a richer navigation/search surface. Avalonia gallery contains 11 AXAML views.
- Known explicit incomplete navigation behavior found during inventory: `CrissCross.Avalonia.UI/Controls/NavigationView/NavigationView.cs` has `GoForward()` throwing `NotImplementedException`, matching the WPF NavigationView forward path gap.

## Priority legend

- P0: blocks comparable app-shell, theme, navigation, or package consumption.
- P1: visible WPF/gallery parity gap or high-value missing user-facing surface.
- P2: normal control/style/template parity work.
- P3: platform-specific or optional parity; should not force no-op APIs without a consumer.

## Core control parity matrix

| Control name / surface | Existing WPF status | Existing Avalonia status | Required common/base-library support | Testability notes | Priority |
|---|---|---|---|---|---|
| `AccessText` | Style resource exists under `Controls/AccessText/AccessText.xaml`; included by `Resources/CrissCross.Ui.xaml`. | AXAML resource exists under `Controls/AccessText/AccessText.axaml`; not part of `Themes/Index.axaml`. | Shared text/accelerator semantics only if callers depend on a cross-platform surface. | Template/resource-load smoke test. | P2 |
| `AlarmBanner` | Implemented with class and style; included by WPF control dictionary. | Implemented with class and theme; included by `Themes/Index.axaml`. | Shared alarm severity/status model if `Alarms` becomes cross-platform. | Template load plus severity visual-state coverage. | P2 |
| `Alarms` | No WPF top-level control folder found. | Avalonia-only control folder/class found. | Decide whether this is Avalonia-only or should map to WPF `AlarmBanner`/future alarm list model. | Add gallery sample before claiming user-facing parity. | P2 |
| `Anchor` | Implemented with class/style and WPF gallery text category coverage. | Implemented with class/theme; no matching Avalonia gallery page. | Shared hyperlink/text command contract. | Template load and keyboard/navigation command tests. | P2 |
| `AppBar` | Implemented as WPF user control/resource. | Implemented with class/theme. | Shared app-bar command/item model if reused by navigation/window shells. | Template load and command binding smoke. | P2 |
| `Arc` | Implemented with class/style. | Implemented with class/theme. | Shared geometry/property coercion only. | Unit-test geometry coercion and template load. | P2 |
| `AutoSuggestBox` | Implemented with class/style; covered through WPF `ComboBox` gallery category. | Implemented with class/theme; no dedicated Avalonia gallery scenario beyond ComboBox page. | Shared query/submission/suggestion event model and cancellation-aware observable query pipeline. | Unit-test query/open/selected-item transitions; gallery smoke with async suggestions. | P1 |
| `Badge` | Implemented with class/style. | Implemented with class/theme. | Shared severity/appearance enum names. | Template-state smoke. | P2 |
| `Border` | Implemented as top-level WPF control folder/class. | Implemented as top-level Avalonia control folder/class. | Mostly platform style parity. | Template/resource smoke. | P2 |
| `BreadcrumbBar` | Implemented with class/style and navigation-related gallery usage. | Implemented with class/theme; no rich Avalonia gallery equivalent. | Shared navigation breadcrumb item model and route projection. | Unit-test item generation and selection; gallery navigation smoke. | P1 |
| `Button` | Implemented with style; WPF gallery `Buttons` page demonstrates button family. | Implemented with theme; Avalonia gallery `ButtonsPageView` exists. | Shared `ControlAppearance`, corner radius, command, hover/pressed/disabled state naming. | Template load and command invocation smoke. | P2 |
| `BezelButton` | Implemented with custom class/style; in WPF button family. | Implemented with class/theme; in Avalonia button family. | Shared custom button base state contract. | Visual-state/template smoke. | P2 |
| `BezelRepeatButton` | Implemented with custom class/style. | Implemented with class/theme. | Shared repeat-button timing/property semantics if exposed. | Unit-test repeat command cadence where possible. | P2 |
| `BezelToggleButton` | Implemented with custom class/style. | Implemented with class/theme. | Shared toggle state and appearance naming. | Unit-test checked/unchecked visual states. | P2 |
| `GelButton` | Implemented with custom class/style. | Implemented with class/theme. | Shared custom button base state contract. | Template visual-state smoke. | P2 |
| `GelRepeatButton` | Implemented with custom class/style. | Implemented with class/theme. | Shared repeat-button timing/property semantics if exposed. | Unit-test repeat command cadence where possible. | P2 |
| `GelToggleButton` | Implemented with custom class/style. | Implemented with class/theme. | Shared toggle state and appearance naming. | Unit-test checked/unchecked visual states. | P2 |
| `Calendar` | Implemented with class/style and WPF date gallery category. | Implemented with class/theme; Avalonia date picker gallery exists. | Shared date selection, culture, min/max, nullable value semantics. | Unit-test date coercion and template part load. | P2 |
| `CalendarDatePicker` | Implemented with class/style. | Implemented with class/theme. | Shared date formatting and popup lifecycle contract. | Template load and nullable date selection tests. | P2 |
| `DatePicker` | WPF style exists and WPF date gallery category exists. | Avalonia class/theme exists and date picker gallery page exists. | Shared date parsing, culture, min/max, nullable value semantics. | Unit-test selected date transitions and headless gallery smoke. | P2 |
| `DateTimePicker` | Implemented with WPF user control/resource. | Avalonia class exists; no `DateTimePicker.axaml` theme entry found, while date/time picker themes include `DatePicker` and `TimePicker`. | Shared date+time value model and format parsing. | Add explicit template/resource coverage before claiming parity. | P1 |
| `TimePicker` | Implemented with WPF class/style. | Implemented with Avalonia class/theme. | Shared time parsing and clock identifier semantics. | Unit-test time coercion and popup lifecycle. | P2 |
| `Card` | Implemented with class/style. | Implemented with class/theme. | Shared card appearance, elevation, corner radius tokens. | Template load and style-key tests. | P2 |
| `CardAction` | Implemented with class/style. | Implemented with class/theme. | Shared action/chevron/icon content contract. | Command and template state tests. | P2 |
| `CardColor` | Implemented with class/style. | Implemented with class/theme. | Shared color/appearance token names. | Template load and property default tests. | P2 |
| `CardControl` | Implemented with class/style and WPF automation peer. | Implemented with class/theme. | Shared header/content/action layout contract; platform-specific automation peers. | Template load plus automation/accessibility smoke. | P2 |
| `CardExpander` | Implemented with class/style. | Implemented with class/theme. | Shared expanded/collapsed model. | Unit-test expanded state and template transitions. | P2 |
| `CheckBox` | Implemented with class/style; WPF gallery page exists. | Implemented with class/theme; Avalonia gallery page exists. | Shared checked/indeterminate command/state naming. | Unit-test tri-state transitions. | P2 |
| `CheckBoxModern` | Implemented with class/style and event args. | Implemented with class/theme and event args. | Shared modern appearance and result event semantics. | Template load and event-state tests. | P2 |
| `ClientAreaBorder` | Implemented as WPF top-level control folder/class. | Implemented as Avalonia top-level control folder/class. | Window/client-area abstraction only if consumed cross-platform. | Platform smoke tests. | P3 |
| `ColorPicker` | Implemented with class/style; WPF color picker gallery page exists. | Implemented with class/theme; Avalonia color picker gallery page exists. | Shared color model/converters, alpha/null semantics, and observable value-change stream. | Deterministic RGB/HSL/HSV conversion tests; gallery smoke. | P1 |
| `ColorSelector` | WPF has a `ColorSelector` folder with several user controls and style resources but no single `ColorSelector` class/theme entry. | Avalonia has `ColorSelector` class and theme. | Decide exact-vs-adapted API shape; extract shared color math and value-change semantics. | Unit-test color conversion and template-part behavior. | P1 |
| `AlphaSlider` | WPF user control under `ColorSelector`; style resource exists. | Avalonia class/theme exists. | Shared alpha channel model. | Slider value coercion and template load tests. | P2 |
| `ColorDisplay` | WPF user control under `ColorSelector`; style resource exists. | Avalonia class/theme exists. | Shared color formatting/contrast calculation. | Template and color-text conversion tests. | P2 |
| `ColorSliders` | WPF-only user control under `ColorSelector`. | Missing as a named Avalonia control/theme. | Decide if Avalonia `ColorSelector` replaces it or port explicit RGB/HSL/HSV sliders. | Conversion and slider synchronization tests. | P1 |
| `HexColorTextBox` | WPF user control under `ColorSelector`. | Avalonia class/theme exists. | Shared hex parse/format and validation behavior. | Parse/validation tests including alpha and invalid input. | P1 |
| `HueSlider` | WPF has hue slider support under `ColorSelector/UIExtensions`. | Avalonia has `HueSlider` class/theme. | Shared hue mapping/conversion math. | Unit-test hue-to-color mapping. | P2 |
| `PortableColorPicker` | WPF-only user control. | Missing as named Avalonia control/theme. | Decide if this is obsolete or should become a cross-platform portable picker. | Gallery and conversion tests if ported. | P1 |
| `SquarePicker` | WPF-only user control. | Missing as named Avalonia control/theme. | Shared 2D color plane math if retained. | Pointer/value mapping tests. | P1 |
| `StandardColorPicker` | WPF-only user control. | Missing as named Avalonia control/theme. | Decide whether standard swatch palette should be shared. | Swatch selection and palette-token tests. | P1 |
| `ComboBox` | Implemented with class/style; WPF combo/gallery category exists. | Implemented with class/theme and Avalonia combo gallery page. | Shared item selection/display member semantics where CrissCross adds value. | Selection and template smoke tests. | P2 |
| `ContentDialog` | Implemented with class/style and service. | Implemented with class/theme and service. | Shared dialog result/lifecycle model and observable request/close streams. | Async result, cancellation, close-button tests. | P1 |
| `ContextMenu` | Implemented as WPF user control/style. | Implemented as Avalonia class/theme. | Shared menu item model only if consumed by CrissCross controls. | Template/load smoke. | P2 |
| `DataGrid` | Implemented with class/style. | Implemented with class/theme. | Shared selection, sorting/header and virtualization contracts where custom. | Headless data/grid selection tests and gallery sample. | P1 |
| `DropDownButton` | Implemented with class/style. | Implemented with class/theme. | Shared flyout/dropdown command lifecycle. | Template and open/close state tests. | P2 |
| `DynamicScrollBar` | Implemented with class/style; WPF XAML contains TODO for custom thumb corner radius. | Implemented with class/theme. | Shared scroll direction/range behavior. | Unit-test range and pointer interaction; visual smoke for thumb states. | P2 |
| `DynamicScrollViewer` | Implemented with class/style; WPF has optimization TODOs. | Implemented with class/theme. | Shared dynamic scrollbar visibility behavior. | Headless scroll/visibility tests. | P2 |
| `Expander` | Implemented with WPF style. | Implemented with Avalonia class/theme. | Shared expanded/collapsed state contract. | State transition and template tests. | P2 |
| `FluentNavigationWindow` | Implemented with WPF class/style and gallery app shell. | Implemented with Avalonia class/theme but Avalonia gallery uses `CrissCross.Avalonia.NavigationWindow`, not this UI-specific shell directly. | Shared hosted navigation/window shell abstraction. | App-shell smoke and route activation tests. | P0 |
| `FluentWindow` | Implemented with class/style. | Implemented with class/theme. | Platform windowing/backdrop/titlebar abstraction; native details stay platform-specific. | Platform smoke/manual visual verification. | P3 |
| `ModernWindow` | Implemented with WPF class/style. | Avalonia class exists; no `ModernWindow.axaml` theme in `Themes/Index.axaml`. | Decide if this is WPF-only or should be styled in Avalonia. | Platform smoke if retained. | P3 |
| `Flyout` | Implemented with class/style. | Implemented with class/theme. | Shared placement/open-close lifecycle model. | Unit-test lifecycle and template load. | P2 |
| `Frame` | Implemented with WPF style. | Implemented with Avalonia class/theme. | Shared route host/cache semantics. | Navigation state and content activation tests. | P0 |
| `Grid` | WPF top-level control folder/class found. | Avalonia top-level control folder/class found. | Mostly style/layout parity; avoid duplicating platform layout engines. | Template/resource smoke only. | P3 |
| `GridView` | WPF has `GridView` class/style and multiple WPF-only subtemplates. | Avalonia class/theme exists. | Shared column/header/row presenter model if feature parity is expected. | Data/header/selection tests and gallery sample. | P1 |
| `GridViewColumn` | WPF-only public class. | Missing as named Avalonia control/class. | Required if Avalonia `GridView` should match WPF column API. | Column sizing/sorting tests. | P1 |
| `GridViewColumnHeader` | WPF-only style resource. | Missing as named Avalonia theme. | Header presenter abstraction if retained. | Template load and sorting visual-state tests. | P1 |
| `GridViewHeaderRowPresenter` | WPF-only public class. | Missing as named Avalonia class. | Required only for exact WPF GridView layout parity. | Layout/unit tests around header row. | P2 |
| `GridViewRowPresenter` | WPF-only public class. | Missing as named Avalonia class. | Required only for exact WPF GridView layout parity. | Row layout tests. | P2 |
| `GroupBox` | Top-level WPF control folder/class found. | Top-level Avalonia control folder/class found. | Style/template parity only. | Template load smoke. | P2 |
| `Gauges` | No WPF top-level `Gauges` control class found, but WPF has `Controls/Gauges/CircularGauge`. | Avalonia has `Gauges` class in addition to `CircularGauge`. | Decide whether `Gauges` is a category wrapper or should exist on WPF. | Gallery sample should clarify usage. | P2 |
| `CircularGauge` | Implemented under WPF `Controls/Gauges`; style included. | Implemented with class/theme. | Shared numeric range, pointer, animation scheduler, and precision contracts. | Range/coercion tests plus visual smoke. | P1 |
| `GifImage` | Implemented with WPF decoder/animator stack and style; WPF image gallery category exists. | Avalonia class/theme exists, but WPF helper classes are not mirrored. | Shared URI/resource loading and animation capability contract; platform rendering stays separate. | Resource loader unit tests and gallery animated GIF smoke. | P1 |
| `HyperlinkButton` | Implemented with class/style. | Implemented with class/theme. | Shared command/navigation behavior. | Command and keyboard accessibility tests. | P2 |
| `IconElement` | WPF classes exist for icon elements/source element and fallback. | Avalonia classes exist for `IconElement`, `FontIcon`, `ImageIcon`, `PathIcon`, `SymbolIcon`; theme coverage is indirect. | Shared icon source model and symbol enum mapping. | Resource/font lookup and template smoke tests. | P1 |
| `FontIconFallback` | WPF-only class/style resource. | Missing as named Avalonia control/theme. | Decide fallback behavior for missing glyphs across platforms. | Font fallback unit/snapshot tests. | P1 |
| `FontIconSource` | WPF-only icon-source class. | Missing as named Avalonia class. | Required for source-based icon parity. | Source-to-element conversion tests. | P1 |
| `SymbolIconSource` | WPF-only icon-source class. | Missing as named Avalonia class. | Required for source-based symbol icon parity. | Symbol source conversion tests. | P1 |
| `Image` | Implemented with class/style; WPF image gallery page exists. | Implemented with class/theme; Avalonia gallery image/media page missing. | Shared URI/resource loading abstraction if custom behavior exists. | Resource-load and gallery smoke tests. | P1 |
| `InfoBadge` | Implemented with class/style. | Implemented with class/theme. | Shared severity and appearance enum names. | Template state tests. | P2 |
| `InfoBar` | Implemented with class/style. | Implemented with class/theme. | Shared severity/action/close lifecycle model. | Unit-test close/action commands and template states. | P2 |
| `ItemsControl` | Implemented with class/style. | Implemented with class/theme. | Shared item container behavior only if CrissCross extends platform defaults. | Items/source smoke tests. | P2 |
| `Label` | Implemented with class/style. | Implemented with class/theme; no Avalonia text gallery parity. | Shared typography/accessibility token names. | Template and access-key tests. | P2 |
| `ListBox` | Implemented with class/style and `ListBoxItem` style. | Implemented with class/theme. | Shared selection/container state names. | Selection and item-container tests. | P2 |
| `ListBoxItem` | WPF-only style resource. | Missing as named Avalonia theme/resource. | Required if Avalonia custom item container style must match WPF. | Container template tests. | P1 |
| `ListView` | Implemented with class/style and WPF `ListViewItem`. | Implemented with class/theme. | Shared selection/container/list view state contract. | Selection and virtualization tests. | P2 |
| `ListViewItem` | WPF public class/style resource. | Missing as named Avalonia class/theme. | Required for exact item-container parity. | Item-state template tests. | P1 |
| `LoadingScreen` | Implemented with class/style. | Implemented with class/theme. | Shared loading state and text/progress model. | Unit-test loading transitions and template load. | P2 |
| `Menu` | Implemented with WPF `Menu.xaml`; WPF `MenuItem.xaml` is under `Controls/Menu`. | Avalonia has `Menu` theme and a separate `MenuItem` folder/class/theme. | Shared menu item command model only if custom. | Template and command routing smoke. | P2 |
| `MenuItem` | WPF has style resource under `Controls/Menu/MenuItem.xaml`, not a top-level folder. | Avalonia has top-level `Controls/MenuItem` class and `Themes/MenuItem.axaml`. | Decide whether WPF should expose equivalent top-level namespace/class or document platform shape. | Menu item command/checked state tests. | P2 |
| `MessageBox` | Implemented with class/style and magic message interfaces. | Implemented with class/theme. | Shared message result/button model and async request stream. | Dialog result and close/cancel tests. | P1 |
| `MessageBoxAsync` | WPF-only user control/resource. | Missing as named Avalonia control/theme. | Prefer shared async dialog service over duplicated async message box control. | Async lifecycle tests before port. | P1 |
| `NavigationControls` | WPF has `NavigationVM` user-control family. | Avalonia has `NavigationControls` class/theme. | Shared navigation item model and activation contract. | Route activation and view-model smoke tests. | P0 |
| `NavigationVM` | WPF-only user control resource. | Missing as named Avalonia control/theme. | Either port or explicitly map to Avalonia `NavigationControls`. | Gallery shell smoke. | P1 |
| `NavigationVMLeft` | WPF-only class used by gallery shell. | Missing as named Avalonia class. | Decide left-pane navigation model parity. | Search/filter/navigation item tests. | P1 |
| `NavigationVMTop` | WPF-only class. | Missing as named Avalonia class. | Decide top navigation parity. | Navigation layout tests. | P2 |
| `NavigationView` | Implemented with broad WPF class/style/subresources; WPF navigation gallery/grouped pages exist. | Avalonia class/theme exists; `GoForward()` currently throws `NotImplementedException`; no rich gallery parity. | Shared route/cache/back-forward/item model and breadcrumb projection. | Unit-test navigation stack, cache, selection, back/forward; gallery route smoke. | P0 |
| `NavigationViewBottom` | WPF-only navigation layout style. | Missing as named Avalonia theme. | Required for bottom pane/layout parity. | Layout visual smoke. | P1 |
| `NavigationViewCompact` | WPF-only navigation layout style. | Missing as named Avalonia theme. | Required for compact pane parity. | Layout visual smoke. | P1 |
| `NavigationViewContentPresenter` | WPF class/style resource. | Missing as named Avalonia class/theme. | Required if Avalonia NavigationView should project the same content model. | Template part tests. | P1 |
| `NavigationViewLeftMinimalCompact` | WPF-only navigation layout style. | Missing as named Avalonia theme. | Required for WPF compact layout parity. | Layout visual smoke. | P2 |
| `NavigationViewTop` | WPF-only navigation layout style. | Missing as named Avalonia theme. | Required for top navigation parity. | Layout visual smoke. | P1 |
| `NumberBox` | Implemented with class/style; WPF numeric gallery category exists. | Implemented with class/theme; no dedicated Avalonia numeric gallery page. | Shared numeric formatter, validation, min/max/spin behavior. | Parser/formatter/coercion tests and gallery sample. | P1 |
| `NumberPad` | Implemented with class/style. | Implemented with class/theme. | Shared keypad entry model. | Button input sequence tests. | P1 |
| `NumericPushButton` | Implemented with class/style; WPF numeric gallery page exists. | Implemented with class/theme; Avalonia numeric gallery page missing. | Shared numeric increment/decrement and formatting behavior. | Value change and command tests. | P1 |
| `Page` | Implemented with class/style; WPF also has `ReactivePage`. | Implemented with class/theme. | Shared activation/navigation lifecycle and view-model binding contract. | Activation and route smoke tests. | P0 |
| `ReactivePage` | WPF-only public class. | Missing as named Avalonia class. | Decide whether Avalonia `Page` already covers reactive activation or needs explicit API. | Activation lifecycle tests. | P1 |
| `PasswordBox` | Implemented with class/style; WPF gallery page exists. | Implemented with class/theme; Avalonia gallery page missing. | Shared password reveal/placeholder/validation semantics. | Template and secure text behavior tests; gallery sample. | P1 |
| `PersonPicture` | Implemented with WPF class/style plus automation/template settings. | Implemented with Avalonia class/theme. | Shared initials/avatar/image fallback model. | Fallback/initials tests and image load smoke. | P2 |
| `ProgressBar` | Implemented with class/style; WPF slider/progress category exists. | Implemented with class/theme and Avalonia progress gallery page exists. | Shared range/coercion and indeterminate animation contract. | Range and template state tests. | P2 |
| `ProgressRing` | Implemented with class/style. | Implemented with class/theme and progress gallery page. | Shared indeterminate/progress animation settings. | Template state smoke. | P2 |
| `RadioButton` | Implemented with class/style; WPF and Avalonia gallery pages exist. | Implemented with class/theme and gallery page. | Shared checked group behavior where custom. | Group selection tests. | P2 |
| `RatingControl` | Implemented with class/style. | Implemented with class/theme. | Shared item count/value/precision contract. | Value coercion and item state tests. | P2 |
| `RichTextBox` | Implemented with class/style and WPF text-box gallery category. | Implemented with class/theme, gallery showcase coverage, rendered-offset document helpers, command/clipboard/drag-drop/runtime policy seams, and default-off remote-image display policy. | Shared text document, selection, clipboard/HTML/Markdown capability contract; remaining advanced editor items stay in the RichTextBox gap plan. | RichTextBox TUnit/MTP coverage now includes document parsing/selection, hit-test/drop positioning fallback, runtime DragEventArgs/IDataTransfer policy, image/text file gates, command keyboard routing, clipboard, context menu, modes, and remote-image policy. | P1 |
| `ScrollBar` | Implemented with class/style. | Implemented with class/theme. | Shared range/orientation behavior only if custom. | Range and template tests. | P2 |
| `ScrollViewer` | Implemented with class/style. | Implemented with class/theme. | Shared scroll command behavior only if custom. | Scroll offset tests. | P2 |
| `Separator` | Implemented with class/style. | Implemented with class/theme. | Style parity only. | Template load. | P2 |
| `Slider` | Implemented with class/style and WPF slider gallery. | Implemented with class/theme and Avalonia slider gallery. | Shared numeric range/coercion, orientation, tick behavior. | Range/tick tests. | P2 |
| `Snackbar` | Implemented with class/style and service; WPF has a TODO around detached queued snackbar processing. | Implemented with class/theme and service. | Shared observable queue/show/close lifecycle. | Queue ordering, cancellation and close tests. | P1 |
| `SplitButton` | Implemented with class/style. | Implemented with class/theme. | Shared split command/dropdown lifecycle. | Command and dropdown state tests. | P2 |
| `SquareSlider` | Implemented with class/style. | Implemented with class/theme. | Shared 2D range/pointer mapping. | Pointer-to-value tests and template smoke. | P2 |
| `StatusBar` | Implemented with class/style. | Implemented with class/theme. | Style/content parity only. | Template load. | P2 |
| `TabControl` | Implemented with class/style and WPF has `TabViewItem` class. | Avalonia has `TabControl` and `TabView` classes/themes. | Shared tab selection/item model if `TabView` is public cross-platform API. | Selection and item close/add tests if enabled. | P2 |
| `TextBlock` | Implemented with class/style; WPF text gallery page exists. | Implemented with class/theme; Avalonia text gallery missing. | Shared typography token names and text color model. | Typography resource key tests and gallery sample. | P1 |
| `TextBox` | Implemented with class/style; WPF text-box gallery page exists. | Implemented with class/theme; Avalonia text-box gallery missing. | Shared placeholder/icon/validation/selection behavior where custom. | Text, validation and template-part tests. | P1 |
| `ThumbRate` | Implemented with class/style. | Implemented with class/theme. | Shared rating/value enum model. | Value/state tests. | P2 |
| `TitleBar` | Implemented with class/style and WPF-only `TitleBarButton`. | Implemented with class/theme. | Platform titlebar abstraction; native drag/window command details stay platform-specific. | Platform smoke/manual test plus command state unit tests. | P3 |
| `TitleBarButton` | WPF-only class. | Missing as named Avalonia class/theme. | Only port if custom Avalonia titlebar uses same button model. | Window command tests. | P3 |
| `ToggleButton` | Implemented with class/style; WPF gallery page exists. | Implemented with class/theme; Avalonia toggle gallery page missing. | Shared checked/unchecked state and command model. | Toggle state tests and gallery sample. | P1 |
| `ToggleSwitch` | Implemented with class/style. | Implemented with class/theme; Avalonia toggle gallery page missing. | Shared checked/unchecked state and content labels. | Toggle and accessibility state tests. | P1 |
| `ToolBar` | Implemented with class/style. | Implemented with class/theme. | Style/item-host parity only. | Template load. | P2 |
| `ToolTip` | Implemented with class/style. | Implemented with class/theme. | Placement/open delay parity if custom. | Template/open-close smoke. | P2 |
| `TreeGrid` | Implemented with class/style and WPF-only header/item classes. | Implemented with class/theme; no Avalonia tree gallery page. | Shared hierarchical item, selection, virtualization, header model. | Tree item expansion/selection tests and gallery sample. | P1 |
| `TreeView` | Implemented with class/style and WPF tree gallery page. | Implemented with class/theme; Avalonia tree gallery page missing. | Shared hierarchical item, selection, expansion and container state model. | Expansion/selection tests and gallery sample. | P1 |
| `ReactiveTreeView` | WPF-only user control/class/model surface. | Missing as named Avalonia control/theme. | Decide whether Avalonia `TreeView` should absorb reactive tree model or port explicit control. | Reactive expansion/selection observable tests. | P1 |
| `TreeViewItem` | WPF public class/style resource. | Missing as named Avalonia class/theme. | Required for exact custom item-container parity. | Container state/template tests. | P1 |
| `VirtualizingGridView` | Implemented with class/style. | Implemented with class/theme. | Shared virtualization, item recycling and scroll contracts. | Headless virtualization tests with large item source. | P1 |
| `VirtualizingItemsControl` | Implemented with class/style. | Implemented with class/theme. | Shared virtualization/recycling contracts. | Large item-source tests. | P1 |
| `VirtualizingWrapPanel` | Implemented with class/style. | Implemented with class/theme. | Shared arrange/measure and virtualization strategy. | Layout measure/arrange tests where possible. | P1 |
| `Window` | Implemented with class/style and WPF backdrop/native interop support. | Implemented with class/theme. | Platform window abstraction; native WPF DWM/taskbar pieces stay isolated. | Platform smoke/manual visual test. | P3 |

## Resource, theme, and behavior parity matrix

| Surface | Existing WPF status | Existing Avalonia status | Required common/base-library support | Testability notes | Priority |
|---|---|---|---|---|---|
| Control resource aggregation | `Markup/ControlsDictionary.cs` points to `Resources/CrissCross.Ui.xaml`, which merges WPF control resources. | Avalonia consumers include `Themes/Index.axaml` directly; no typed resource dictionary wrapper equivalent. | Consider an Avalonia include helper only if consumers need a code/XAML wrapper; otherwise document `StyleInclude` usage. | Static parse test can ensure every shipped theme is included by `Index.axaml`. | P0 |
| Theme resource aggregation | `Markup/ThemesDictionary.cs` selects Light/Dark/HighContrast path through `ApplicationThemeManager.ThemesDictionaryPath`. | `Resources/FluentTheme.axaml` only merges Light/Dark dictionaries. | Common theme enum/palette names and high-contrast mapping across platforms. | Resource-load smoke tests for every variant. | P0 |
| High contrast themes | WPF has `Resources/Theme/HC1.xaml`, `HC2.xaml`, `HCBlack.xaml`, and `HCWhite.xaml`. | No Avalonia high-contrast resource dictionaries found. | Shared semantic color tokens before platform dictionaries. | Token coverage tests plus manual accessibility contrast validation. | P1 |
| Fonts and typography | WPF has `Resources/Fonts.xaml`, `Resources/Typography.xaml`, and `Resources/Fonts/` assets. | No font asset/resource folder found in `CrissCross.Avalonia.UI`; typography appears distributed through Fluent/default styles. | Shared typography token names; platform-specific font packaging. | Resource existence/package tests and TextBlock gallery coverage. | P1 |
| Palette, accent, static colors | WPF has `Accent.xaml`, `Palette.xaml`, `StaticColors.xaml`, and `Variables.xaml`. | Avalonia has Light/Dark dictionaries; palette tokens are less obviously centralized. | Shared color-token vocabulary and generated token docs. | Static key-parity test between WPF and Avalonia dictionaries. | P1 |
| Attached behaviors/helpers | WPF includes ColorSelector `TextBoxFocusBehavior`, GIF `AnimationBehavior`, NavigationView attached properties, `DesignerHelper`, `DpiHelper`, and `VisualStateGroupHelper`. | No broad Avalonia attached behavior equivalents found; behavior is mostly embedded in controls. | Document intentional replacements and move platform-neutral state machines into common services where needed. | Unit tests around replacement services; avoid reflection-heavy template probing. | P1 |
| Taskbar/native interop | WPF has `TaskbarProgress`, `TaskBarService`, DWM/User32/Shell interop, hardware/DPI helpers. | No Avalonia taskbar/native interop parity in `CrissCross.Avalonia.UI`. | Platform abstraction only if cross-platform callers need it; keep WPF-native APIs isolated otherwise. | WPF-only integration/manual tests; do not force Avalonia no-op APIs without consumer need. | P3 |
| Settings store | WPF has tracker/configuration/attributes plus JSON store. | Avalonia has `IStore`/`JsonFileStore`; no tracker/configuration attributes found. | Common persistence/tracking abstraction if Avalonia needs window/control state persistence. | Pure unit tests against JSON store and trackable metadata. | P2 |

## Gallery parity matrix

| WPF/gallery surface | WPF gallery status | Avalonia gallery status | Priority | Notes |
|---|---|---|---|---|
| `AllControls` searchable inventory | WPF `AllControlsViewModel` populates a searchable control list. | Missing. | P1 | Add after route model is stable; useful as a living parity smoke surface. |
| `Buttons` | WPF page exists. | Avalonia `ButtonsPageView` exists. | P2 | Keep scenarios aligned across all custom button styles. |
| `CheckBox` | WPF page exists. | Avalonia `CheckBoxPageView` exists. | P2 | Include standard and modern checkbox variants. |
| `ComboBox` / `AutoSuggestBox` | WPF page exists. | Avalonia `ComboBoxPageView` exists. | P2 | Add async AutoSuggestBox scenario if absent. |
| `DatePicker` / calendar/time | WPF page exists. | Avalonia `DatePickerPageView` exists. | P2 | Add DateTimePicker/TimePicker if missing from sample. |
| `ColorPicker` | WPF page exists. | Avalonia `ColorPickerPageView` exists. | P1 | Use this to decide ColorSelector exact-vs-adapted parity. |
| `Image` / media / icons | WPF page exists. | Missing. | P1 | Should cover Image, GifImage, icon elements, and PersonPicture. |
| `NumericPushButton` / `NumberBox` / `NumberPad` | WPF page exists. | Missing. | P1 | Needed for existing Avalonia controls that lack demos. |
| `PasswordBox` | WPF page exists. | Missing. | P1 | Useful for validation/reveal/placeholder parity. |
| `RadioButton` | WPF page exists. | Avalonia `RadioButtonPageView` exists. | P2 | Keep grouped-selection scenarios aligned. |
| `Slider` / progress/rating | WPF page exists. | Avalonia `SliderPageView` and `ProgressPageView` exist. | P2 | Ensure RatingControl/ThumbRate/CircularGauge are represented. |
| `TextBlock` / typography | WPF page exists. | Missing. | P1 | Needed before theme typography parity can be manually checked. |
| `TextBox` / RichTextBox | WPF page exists. | Missing. | P1 | Needed for validation, icon, multi-line, and rich-text parity. |
| `ToggleButton` / ToggleSwitch | WPF page exists. | Missing. | P1 | Existing Avalonia controls need sample coverage. |
| `TreeView` / TreeGrid | WPF page exists. | Missing. | P1 | Needed for hierarchical item and virtualization parity. |
| Grouped `ColorControls` | WPF grouped page exists. | Missing grouped page. | P1 | Avalonia gallery currently uses direct navigation pages only. |
| Grouped `ContainerControls` | WPF grouped page exists. | Missing grouped page. | P1 | Add once card/expander/content controls have samples. |
| Grouped `DateTimeControls` | WPF grouped page exists. | Missing grouped page. | P1 | Useful for date/time family parity. |
| Grouped `Indicators` | WPF grouped page exists. | Missing grouped page. | P1 | Progress, rating, badge and gauge samples belong here. |
| Grouped `InputControls` | WPF grouped page exists. | Missing grouped page. | P1 | Text, password, numeric, combo and autosuggest samples. |
| Grouped `MediaControls` | WPF grouped page exists. | Missing grouped page. | P1 | Image/GIF/icon/person picture samples. |
| Grouped `NavigationControls` | WPF grouped page exists. | Missing grouped page. | P0 | Important for NavigationView/Frame/Page parity. |

## Recommended follow-up sequence

1. P0: make Avalonia `Themes/Index.axaml` inclusion mechanically testable and resolve the NavigationView forward/navigation surface contract.
2. P1: fill Avalonia gallery coverage for WPF-demonstrated categories before broad control rewrites; this gives immediate smoke coverage for many existing Avalonia controls.
3. P1: explicitly decide whether ColorSelector, NavigationVM/NavigationView layout variants, and ReactiveTreeView should be exact ports or documented adapted Avalonia APIs.
4. P1/P2: add static resource-key parity tests for Light/Dark/high-contrast/palette/typography tokens after token vocabulary is finalized.
5. P2/P3: evaluate platform-only WPF features such as taskbar, DWM/backdrop, Win32 DPI, and hardware acceleration separately; do not introduce placeholder Avalonia APIs unless a consumer requires them.

## Verification notes

- This document is a file-inventory and source-inspection artifact; it adds no production implementation.
- Follow-up implementation cards should use TDD: static resource/index tests first, then template-load/gallery smoke tests, then production code.
