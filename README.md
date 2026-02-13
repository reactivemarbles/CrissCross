# CrissCross
A navigation framework and set of UI components for ReactiveUI-based applications across WPF, Avalonia, MAUI, and WinForms.

![CrissCross](Images/CrissCrossIcon_256.png)

[![CrissCross CI-Build](https://github.com/reactivemarbles/CrissCross/actions/workflows/BuildOnly.yml/badge.svg)](https://github.com/ChrisPulman/CrissCross/actions/workflows/BuildOnly.yml)

## Overview

CrissCross provides ViewModel-first navigation, hostable navigation surfaces, and a comprehensive WPF UI control set with a strong ReactiveUI focus. It promotes:

- ViewModel-first navigation using ReactiveUI’s IViewFor and WhenActivated
- Host-based navigation via named ViewModelRoutedViewHost containers
- Consistent navigation lifecycle notifications (WhenNavigating/WhenNavigatedTo/From)
- Easy DI/IoC integration via Splat and Microsoft.Extensions.Hosting
- A large library of fluent WPF UI controls and services (dialogs, snackbars, themes)

Supported platforms and packages:

- Core: CrissCross (ReactiveUI helpers and navigation abstractions)
- WPF navigation host: CrissCross.WPF
- WPF UI control library: CrissCross.WPF.UI
- Avalonia host: CrissCross.Avalonia
- .NET MAUI helpers: CrissCross.MAUI
- WinForms host: CrissCross.WinForms
- WPF WebView2 overlay host: CrissCross.WPF.WebView2
- WPF Plot control suite: CrissCross.WPF.Plot

NuGet packages:

- CrissCross: ![Nuget](https://img.shields.io/nuget/v/CrissCross) ![Nuget](https://img.shields.io/nuget/dt/CrissCross)
- WPF: ![Nuget](https://img.shields.io/nuget/v/CrissCross.WPF) ![Nuget](https://img.shields.io/nuget/dt/CrissCross.WPF)
- WPF UI: ![Nuget](https://img.shields.io/nuget/v/CrissCross.WPF.UI) ![Nuget](https://img.shields.io/nuget/dt/CrissCross.WPF.UI)
- Avalonia: ![Nuget](https://img.shields.io/nuget/v/CrissCross.Avalonia) ![Nuget](https://img.shields.io/nuget/dt/CrissCross.Avalonia)
- MAUI: ![Nuget](https://img.shields.io/nuget/v/CrissCross.MAUI) ![Nuget](https://img.shields.io/nuget/dt/CrissCross.MAUI)
- WinForms: ![Nuget](https://img.shields.io/nuget/v/CrissCross.WinForms) ![Nuget](https://img.shields.io/nuget/dt/CrissCross.WinForms)
- WPF WebView2: ![Nuget](https://img.shields.io/nuget/v/CrissCross.WPF.WebView2) ![Nuget](https://img.shields.io/nuget/dt/CrissCross.WPF.WebView2)
- WPF Plot: ![Nuget](https://img.shields.io/nuget/v/CrissCross.WPF.Plot) ![Nuget](https://img.shields.io/nuget/dt/CrissCross.WPF.Plot)

Note: Xamarin.Forms support exists in separate projects but for new apps prefer .NET MAUI.

---

## Breaking Changes in Version 3.2.0 +

- Using a new version of ReactiveUI (v23.1+) with some API changes, the main change is the registration of ReactiveUI components which now uses an AppBuilder fluent API instead of Splat’s locator directly. This is a breaking change but allows for better integration with AoT builds and more flexible registration patterns.
- Add `RxAppBuilder.CreateReactiveUIBuilder().With**Platform**().BuildApp();` as early as possible in your app startup (e.g., App.xaml.cs) to register ReactiveUI services. Then register your VMs/Views as usual with Splat or Microsoft.Extensions.DependencyInjection.
- RxApp has been completely removed, so any direct references to RxApp.Current or RxApp.MainThreadScheduler should be updated to use the new builder pattern and new service resolution.

## Core Concepts

CrissCross builds on ReactiveUI to provide ViewModel-first navigation:

- IRxObject: Base ViewModel type used throughout CrissCross
- IViewFor<TViewModel>: ReactiveUI contract mapping VMs to Views
- WhenActivated: ReactiveUI activation lifecycle for Views
- ViewModelRoutedViewHost: Navigation host control that manages a navigation stack and view transitions
- HostName: A host identifier (string) that allows targeting navigation to a specific host
- Navigation lifecycle: WhenNavigating, WhenNavigatedTo, WhenNavigatedFrom via mixins/events

Register your ViewModels and Views with Splat’s Locator or Microsoft.Extensions.DependencyInjection. CrissCross uses the locator to resolve Views for navigation targets.

---

## Quick Start (WPF)

1) Install packages

- CrissCross
- CrissCross.WPF
- CrissCross.WPF.UI (for controls/themes)

2) Register ViewModels and Views

```csharp
public class AppBootstrapper : RxObject
{
    public AppBootstrapper()
    {
        this.BuildComplete(() =>
        {
            // Example VM/View registrations using Splat
             AppLocator.CurrentMutable.RegisterConstant(new MainViewModel());
             AppLocator.CurrentMutable.Register<IViewFor<MainViewModel>>(() => new MainView());

             AppLocator.CurrentMutable.RegisterConstant(new FirstViewModel());
             AppLocator.CurrentMutable.Register<IViewFor<FirstViewModel>>(() => new FirstView());

             AppLocator.CurrentMutable.SetupComplete();
        });
    }
}
```

3) Create a navigation host (NavigationWindow)

```csharp
public partial class MainWindow : NavigationWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent(); // Ensure x:Name is set on the Window (e.g., "mainWindow")

        this.WhenActivated(d =>
        {
            // Bind back command, etc
            NavBack.Command = ReactiveCommand.Create(() => this.NavigateBack(), CanNavigateBack).DisposeWith(d);

            // Navigate to a start VM
            this.NavigateToView<MainViewModel>();
        });
    }
}
```

4) Navigate from a ViewModel

```csharp
public class MainViewModel : RxObject
{
    public MainViewModel()
    {
        this.BuildComplete(() =>
        {
            // Target a specific host by name (Window x:Name)
            this.NavigateToView<FirstViewModel>("mainWindow");
        });
    }
}
```

5) Create a View

```csharp
public partial class MainView : ReactiveUserControl<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();
        this.WhenActivated(_ => { /* bindings, commands */ });
    }
}
```

---

## Hosts and Navigation APIs

- NavigationWindow (WPF): A Window that exposes a ViewModelRoutedViewHost and transition support
  - Properties: Transition, NavigateBackIsEnabled
  - Exposes CanNavigateBack observable and NavigateBack() helper

- FluentNavigationWindow (WPF UI): A fluent-styled NavigationWindow with additional title content areas and Transition

- NavigationUserControl (WPF UI, Avalonia): A hostable control version of the navigation container (for regions/panels)

- ViewModelRoutedViewHost (WPF): Core host implementation
  - Navigate<TViewModel>(contract, parameter)
  - Navigate(IRxObject vm, contract, parameter)
  - NavigateAndReset variants
  - NavigateBack(parameter)
  - CanNavigateBackObservable, NavigationStack
  - Lifecycle events routed via ViewModelRoutedViewHostMixins:
    - WhenNavigating: preview/cancel navigation
    - WhenNavigatedTo/From: after navigation completes

- HostName: Set on NavigationWindow/NavigationUserControl (typically from x:Name) to route cross-host navigation

### Navigation from Views and ViewModels

- From code-behind: this.NavigateToView<TViewModel>(hostName?, parameter?)
- From ViewModel: this.NavigateToView<TViewModel>(hostName, parameter?)
- NavigateBack(hostName?, parameter?) and CanNavigateBack(hostName?) helpers

---

## WPF UI Library (CrissCross.WPF.UI)

A comprehensive set of fluent controls and services designed for ReactiveUI apps. Highlights include:

- NavigationView, BreadcrumbBar and navigation controls/models
- Dialogs: ContentDialog, MessageBox, async variants
- Notifications: Snackbar, InfoBar, InfoBadge
- Input: AutoSuggestBox, NumberBox, PasswordBox, ToggleSwitch, TimePicker, DatePicker
- Lists and virtualization: ListView, VirtualizingGridView, VirtualizingWrapPanel
- Layout/containers: Card, CardExpander, GroupBox, Grid, StackPanel
- Media: GifImage (animation), Image
- PersonPicture, RatingControl, ProgressRing, AppBar, TitleBar, Window enhancements
- Color controls: ColorSelector suite (sliders, dual pickers), HexColorTextBox, and ColorPicker
- Typography and iconography: FontIcon, SymbolIcon, IconSource
- Themes and appearance utilities

Include the control resources by merging the ControlsDictionary:

```xaml
<Application.Resources>
  <ResourceDictionary>
    <ResourceDictionary.MergedDictionaries>
      <ui:ControlsDictionary />
    </ResourceDictionary.MergedDictionaries>
  </ResourceDictionary>
</Application.Resources>
```

### Theming and Appearance

- ApplicationThemeManager and SystemThemeWatcher for light/dark and system theme integration
- Resource dictionaries for typography, colors, focus, default styles
- ThemeService and IThemeService for programmatic control

### Services

- ContentDialogService: Show dialogs and await results
- SnackbarService: Host snackbars with extension helpers
- PageService (WPF UI): Resolve pages by type for embedded/hosted scenarios

---

## WPF Page Navigation (Host Builder)

For page-based apps using WPF UI, use the host builder extensions:

```csharp
private static readonly IHost _host = Host.CreateDefaultBuilder()
    .ConfigureCrissCrossForPageNavigation<MainWindow, DashboardPage>()
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<DashboardPage>().AddSingleton<DashboardViewModel>();
        services.AddSingleton<DataPage>().AddSingleton<DataViewModel>();
        services.AddSingleton<SettingsPage>().AddSingleton<SettingsViewModel>();
        services.AddSingleton<LoginPage>().AddSingleton<LoginViewModel>();
    })
    .Build();
```

Wire up and start in App.xaml.cs, then navigate using IPageService or NavigationView.

---

## NavigationView (WPF UI)

A powerful navigation control that manages a journal and hierarchical navigation stack:

- Navigate(Type pageType, object? dataContext)
- Navigate(string pageIdOrTargetTag, object? dataContext)
- NavigateWithHierarchy(Type pageType, object? dataContext)
- ReplaceContent(Type pageType) / ReplaceContent(UIElement)
- GoBack(), GoForward() (where implemented), ClearJournal()
- Events: Navigating (cancelable), Navigated, BackRequested, SelectionChanged, PaneOpened/Closed

The control maintains a NavigationStack and history so you can build rich shell navigation experiences.

---

## Avalonia

- NavigationUserControl (host)
- ViewModelRoutedViewHost equivalent with CanNavigateBack observable and HostName
- Use ReactiveUI’s WhenActivated and Splat for registration as in WPF

```csharp
public partial class MainUserControl : NavigationUserControl<MainWindowViewModel>
{
    public MainUserControl()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            this.NavigateToView<MainViewModel>();
            _NavBack!.Command = ReactiveCommand.Create(() => this.NavigateBack(), this.CanNavigateBack()).DisposeWith(d);
        });
    }
}
```

---

## .NET MAUI

MAUI helpers integrate with ReactiveUI.Maui. Register your VMs/Views with DI and navigate using the same NavigateToView/Back helpers where applicable. Prefer this approach over Xamarin.Forms for new apps.

Packages:

- CrissCross.MAUI
- ReactiveUI.Maui

---

## WinForms

- ViewModelRoutedViewHost for WinForms
- ReactiveUserControl<TViewModel> usage with WhenActivated
- Navigate using the same helper mixins

---

## WPF WebView2 Overlay Host

CrissCross.WPF.WebView2 provides a NavigationWebView that hosts WebView2 while allowing WPF content overlays:

```xaml
xmlns:webv="https://github.com/reactivemarbles/CrissCross"
<webv:WebView2Wpf x:Name="WebView2Wpf" AutoDispose="True">
    <!-- overlay WPF content here -->
</webv:WebView2Wpf>
```

```csharp
WebView2Wpf.Source = new Uri("https://www.reactiveui.net/");
```

---

## WPF Plot (ScottPlot-based)

CrissCross.WPF.Plot adds Reactive plotting components:

- Bind reactive sequences
- Multiple series, Y-axes, crosshairs
- Zoom/pan, drag zoom selection
- Visibility toggles, auto/manual scale

Install: `Install-Package CrissCross.WPF.Plot`

---

## Settings and Tracking (WPF UI)

Persist and track control/window state:

- Tracker service with attributes (Trackable, PersistOn, StopTrackingOn)
- Example usage in App.xaml.cs wiring window size/position persistence

```csharp
_tracker?.Configure<MainWindow>()
    .Id(w => w.Name, $"[Width={SystemParameters.VirtualScreenWidth},Height{SystemParameters.VirtualScreenHeight}]")
    .Properties(w => new { w.Height, w.Width, w.Left, w.Top, w.WindowState })
    .PersistOn(w => nameof(w.Closing))
    .StopTrackingOn(w => nameof(w.Closing));
```

---

## Color and Media Controls (WPF UI)

- ColorSelector suite (HSV/HSL/RGB sliders, square pickers, hex entry, dual color with hints)
- ColorPicker control: A simple RGBA + Hex picker with a live preview
- GifImage with animation control and performance-oriented decoding/animation components

---

## Samples

- CrissCross.WPF.UI.Test: WPF UI test app with page navigation
- CrissCross.WPF.Test: WPF navigation sample
- CrissCross.Avalonia.Test.*: Avalonia samples (desktop, mobile)
- CrissCross.MAUI.Test: MAUI sample
- CrissCross.WPF.Plot.Test: Plot samples
- CrissCross.WPF.WebView2.Test: WebView2 overlay usage
- CrissCross.WPF.UI.Gallery: Control gallery showcasing WPF UI controls

Browse these projects to see real-world usage patterns, navigation setup, and control bindings.

---

## Single Instance (WPF)

Prevent multiple instances using Make.SingleInstance in App:

```csharp
protected override void OnStartup(StartupEventArgs e)
{
    Make.SingleInstance("MyUniqueAppName ddd81fc8-9107-4e33-b848-cac4c3ec3d2a");
    base.OnStartup(e);
}
```

---

## Contributing

Issues and PRs are welcome. Please include platform, .NET version, and a minimal repro where applicable.

---

## License

MIT © ReactiveUI Association Incorporated
