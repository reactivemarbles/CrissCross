
<br>
<a href="https://github.com/reactivemarbles/CrissCross">
  <img width="160" height="160" src="https://raw.githubusercontent.com/reactivemarbles/CrissCross/master/Images/CrissCrossIcon_256.png">
</a>
<br>

# CrissCross

CrissCross is a ReactiveUI-first application toolkit for .NET desktop and cross-platform apps. It provides a shared navigation model, lifecycle helpers, reusable state objects, themed UI controls, shell services, plot integration, and WebView2 hosting across WPF, Avalonia, MAUI, and WinForms.

This is the canonical documentation source for every library project and package in this solution:

| Library or project | Purpose |
| --- | --- |
| `CrissCross` | Core ReactiveUI lifecycle, view-model navigation contracts, bidirectional navigation registry, and shared UI state models. |
| `CrissCross.Reactive` | The complete `CrissCross` API compiled against the Reactive dependency family. |
| `CrissCross.Avalonia` | Avalonia navigation windows, routed view hosts, reactive transition content, and base Avalonia integration. |
| `CrissCross.Avalonia.Reactive` | The complete `CrissCross.Avalonia` API compiled against Reactive Avalonia dependencies. |
| `CrissCross.Avalonia.UI` | Avalonia themed controls, Fluent-style app shell controls, services, resources, themes, and gallery-ready user controls. |
| `CrissCross.Avalonia.UI.Reactive` | The complete `CrissCross.Avalonia.UI` API compiled against the Reactive dependency family. |
| `CrissCross.MAUI` | MAUI Shell navigation host and ReactiveUI integration. |
| `CrissCross.MAUI.Reactive` | The complete `CrissCross.MAUI` API compiled against Reactive MAUI dependencies. |
| `CrissCross.Maui.UI` | Project-reference-only MAUI themed controls and shared feature controls backed by the same core state models. |
| `CrissCross.WinForms` | WinForms navigation forms and routed view hosts. |
| `CrissCross.WinForms.Reactive` | The complete `CrissCross.WinForms` API compiled against Reactive WinForms dependencies. |
| `CrissCross.WPF` | WPF navigation windows, routed view hosts, WebView navigation host, single-instance helper, and WPF integration. |
| `CrissCross.WPF.Reactive` | The complete `CrissCross.WPF` API compiled against Reactive WPF dependencies. |
| `CrissCross.WPF.Plot` | WPF/ScottPlot live charting, reactive plot binding, plot adapters, and plot view models. |
| `CrissCross.WPF.Plot.Reactive` | The complete `CrissCross.WPF.Plot` API compiled against the Reactive dependency family. |
| `CrissCross.WPF.UI` | WPF themed controls, Fluent-style shell services, themes, resource dictionaries, and gallery controls. |
| `CrissCross.WPF.UI.Reactive` | The complete `CrissCross.WPF.UI` API compiled against the Reactive dependency family. |
| `CrissCross.WPF.WebView2` | WPF WebView2 wrapper with overlay window hosting support. |
| `CrissCross.WPF.WebView2.Reactive` | The complete `CrissCross.WPF.WebView2` API compiled against the Reactive dependency family. |
| `AICS.Windows.Controls.ColorPicker` | Independently packable WPF color-picker controls whose public namespace is `CrissCross.WPF.UI`. |


## Target Frameworks

| Library family | Target frameworks |
| --- | --- |
| `CrissCross`, `CrissCross.Reactive` | `net8.0`, `net9.0`, `net10.0`, `net11.0`; Windows builds additionally target `net472`, `net48`, `net481`, and `net8.0` through `net11.0` with `-windows10.0.19041.0`. |
| `CrissCross.Avalonia`, `CrissCross.Avalonia.Reactive` | `net8.0`, `net9.0`, `net10.0`, `net11.0`. |
| `CrissCross.Avalonia.UI`, `CrissCross.Avalonia.UI.Reactive` | `net8.0`, `net9.0`, `net10.0`, `net11.0`. |
| `CrissCross.MAUI`, `CrissCross.MAUI.Reactive` | `net10.0`, `net10.0-android`, `net10.0-ios`, `net10.0-maccatalyst`, `net10.0-macos`, `net10.0-tvos`, and `net10.0-windows10.0.19041.0` on Windows. |
| `CrissCross.Maui.UI` | The same .NET 10 MAUI target family as `CrissCross.MAUI`. This project is marked `IsPackable=false`. |
| `CrissCross.WinForms`, `CrissCross.WinForms.Reactive` | `net472`, `net48`, `net481`, and `net8.0` through `net11.0` with `-windows10.0.19041.0`. |
| `CrissCross.WPF`, `CrissCross.WPF.Reactive` | `net472`, `net48`, `net481`, and `net8.0` through `net11.0` with `-windows10.0.19041.0`. |
| `CrissCross.WPF.Plot`, `CrissCross.WPF.Plot.Reactive` | `net472`, `net48`, `net481`, and `net8.0` through `net11.0` with `-windows10.0.19041.0`. |
| `CrissCross.WPF.UI`, `CrissCross.WPF.UI.Reactive` | `net472`, `net48`, `net481`, and `net8.0` through `net11.0` with `-windows10.0.19041.0`. |
| `CrissCross.WPF.WebView2`, `CrissCross.WPF.WebView2.Reactive` | `net472`, `net48`, `net481`, `net8.0-windows`, `net9.0-windows`, `net10.0-windows`, and `net11.0-windows`. |
| `AICS.Windows.Controls.ColorPicker` | `net48`, `net6.0-windows10.0.17763`, `net7.0-windows10.0.17763`. |

## Gallery Projects

Run the galleries:

```powershell
dotnet run --project src/CrissCross.WPF.UI.Gallery/CrissCross.WPF.UI.Gallery.csproj
dotnet run --project src/CrissCross.Avalonia.UI.Gallery/CrissCross.Avalonia.UI.Gallery.csproj
dotnet run --project src/CrissCross.Maui.UI.Gallery/CrissCross.Maui.UI.Gallery.csproj -f net10.0-windows10.0.19041.0
```

## Package Installation

Choose one dependency family for an application. The standard packages use `ReactiveUI.Primitives`; the `.Reactive` packages compile the same linked CrissCross source against the corresponding `ReactiveUI.*.Reactive` packages. Do not reference both variants of the same CrissCross library in one project.

Standard package family:

```powershell
dotnet add package CrissCross
dotnet add package CrissCross.WPF
dotnet add package CrissCross.WPF.UI
dotnet add package CrissCross.WPF.Plot
dotnet add package CrissCross.WPF.WebView2
dotnet add package CrissCross.Avalonia
dotnet add package CrissCross.Avalonia.UI
dotnet add package CrissCross.MAUI
dotnet add package CrissCross.WinForms
dotnet add package AICS.Windows.Controls.ColorPicker
```

Reactive package family:

```powershell
dotnet add package CrissCross.Reactive
dotnet add package CrissCross.WPF.Reactive
dotnet add package CrissCross.WPF.UI.Reactive
dotnet add package CrissCross.WPF.Plot.Reactive
dotnet add package CrissCross.WPF.WebView2.Reactive
dotnet add package CrissCross.Avalonia.Reactive
dotnet add package CrissCross.Avalonia.UI.Reactive
dotnet add package CrissCross.MAUI.Reactive
dotnet add package CrissCross.WinForms.Reactive
```

The public type/member surface, XAML controls, startup sequence, navigation contracts, and behavior are shared because each `.Reactive` project links the standard project source. Use the namespace and XAML assembly for the selected family:

| Standard namespace | `.Reactive` namespace |
| --- | --- |
| `CrissCross` | `CrissCross.Reactive` |
| `CrissCross.WPF` | `CrissCross.Reactive.WPF` |
| `CrissCross.WPF.UI` | `CrissCross.Reactive.WPF.UI` |
| `CrissCross.WPF.Plot` | `CrissCross.Reactive.WPF.Plot` |
| `CrissCross.Avalonia` | `CrissCross.Reactive.Avalonia` |
| `CrissCross.Avalonia.UI` | `CrissCross.Reactive.Avalonia.UI` |
| `CrissCross.MAUI` | `CrissCross.Reactive.MAUI` |
| `CrissCross.WinForms` | `CrissCross.Reactive.WinForms` |

`CrissCross.WPF.WebView2.Reactive` also exposes its linked control source under `CrissCross.Reactive.WPF`. Substitute these namespaces and the `.Reactive` assembly name in the examples below when using the Reactive package family.

`CrissCross.Maui.UI` is not a NuGet package. Reference the project directly:

```xml
<ProjectReference Include="..\CrissCross.Maui.UI\CrissCross.Maui.UI.csproj" />
```

## ReactiveUI Startup

CrissCross relies on the ReactiveUI app builder and Splat service locator registrations used by the platform packages. Each platform package supplies the platform-specific host; the view models can stay in shared projects.

WPF:

```csharp
using CrissCross.WPF;
using ReactiveUI.Builder;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        RxAppBuilder.CreateReactiveUIBuilder()
            .WithWpf()
            .BuildApp();
    }
}
```

Avalonia:

```csharp
using Avalonia;
using ReactiveUI.Avalonia;

internal sealed class Program
{
    public static void Main(string[] args)
        => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI(_ => { });
}
```

MAUI:

```csharp
using CrissCross.MAUI;
using CrissCross.Maui.UI;
using ReactiveUI;
using ReactiveUI.Builder;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseCrissCrossMauiUi();

        return builder.Build();
    }
}

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        Resources.UseCrissCrossMauiUiResources();
        RxAppBuilder.CreateReactiveUIBuilder().WithMaui().BuildApp();
        MainPage = new AppShell();
    }
}
```

WinForms:

```csharp
using CrissCross.WinForms;
using ReactiveUI.Builder;

ApplicationConfiguration.Initialize();

RxAppBuilder.CreateReactiveUIBuilder()
    .WithWinForms()
    .BuildApp();

Application.Run(new MainForm());
```

## Core Package: CrissCross

`CrissCross` is the shared foundation used by every platform package. It contains no concrete WPF, Avalonia, MAUI, or WinForms controls.

### Core Concepts

| Feature | Types | Use |
| --- | --- | --- |
| Reactive base object | `RxObject`, `IRxObject`, `RxObjectMixins` | Base class and helpers for ReactiveUI view models with navigation lifecycle flags. |
| View-model routed hosts | `IViewModelRoutedViewHost`, `ViewModelRoutedViewHostMixins` | Platform-neutral navigation contract used by WPF, Avalonia, MAUI, and WinForms hosts. |
| Navigation lifecycle | `IAmBuilt`, `ISetNavigation`, `INotifiyNavigation`, `IUseNavigation`, `IUseHostedNavigation`, `INotifiyRoutableViewModel` | Optional interfaces for setup, hosted navigation, and navigation notifications. |
| Build-completion signal | `EventSignal`, `RxObjectMixins.BuildComplete`, `BuildCompleteDisposable`, `SetupComplete` | Defers work until application dependency registration has completed. |
| Navigation events | `IViewModelNavigationBaseEventArgs`, `IViewModelNavigationEventArgs`, `IViewModelNavigatingEventArgs`, `ViewModelNavigationBaseEventArgs`, `ViewModelNavigationEventArgs`, `ViewModelNavigatingEventArgs` | Event argument models for navigating, navigated, and lifecycle notifications. |
| Navigation requests | `NavigationRequestOptions`, `NavigationKeyRequest<TKey>`, `ViewModelNavigationRequest<TViewModel,TView>`, `ViewNavigationRequest<TViewModel,TView>` | Carries typed/runtime lookup keys, contracts, parameters, supplied instances, and cancellation. |
| Navigation registration | `INavigationRegistry`, `NavigationRegistry`, `NavigationRegistration<TViewModelKey,TViewModel,TViewKey,TView>`, `NavigationRegistryExtensions` | Registers concrete or interface/base-keyed ViewModel/View factory pairs. |
| Bidirectional navigation | `IBidirectionalNavigator`, `NavigationExtensions`, `NavigationResolution`, `NavigationResolution<TViewModel,TView>`, `NavigationJournal` | Resolves views from view models and view models from views, then maintains journal history. |
| Resolved routed hosts | `IResolvedViewModelRoutedViewHost` | Adds registry-based navigation-key resolution to a platform routed host. |
| Navigation exceptions | `NavigationRegistrationException`, `NavigationResolutionException` | Explicit errors for duplicate, missing, or invalid navigation registrations. |
| Navigation metadata | `NavigationType`, `NavigationSourceKind` | Describes normal, reset, and back navigation plus origin source. |

### RxObject

Use `RxObject` for view models that need ReactiveUI notifications, setup completion, and navigation lifecycle support.

```csharp
using CrissCross;

public sealed class DashboardViewModel : RxObject
{
    private string title = "Dashboard";

    public string Title
    {
        get => title;
        set => this.RaiseAndSetIfChanged(ref title, value);
    }

    public DashboardViewModel()
    {
        this.BuildComplete(() => Title = "Dashboard ready");
    }
}
```

`BuildComplete(Action)` registers work that must wait until dependency registration is finished. `BuildCompleteDisposable(Action)` does the same and returns the subscription. After all registrations are present, call `AppLocator.CurrentMutable.SetupComplete()` once to publish the build-complete signal and run those callbacks.

### View-Model Navigation Helpers

`ViewModelRoutedViewHostMixins` provides navigation and lifecycle extensions over `IUseNavigation` and `IUseHostedNavigation`. `NavigationExtensions` supplies instance- and request-based helpers directly on `IViewModelRoutedViewHost`.

| Method | Use |
| --- | --- |
| `NavigateToView<TViewModel>(NavigationKeyRequest<TViewModel>)` | Resolve and navigate to a registered view-model key, with optional contract, parameter, and cancellation metadata. |
| `NavigateToView(Type, NavigationRequestOptions)` | Navigate to a view model type selected at runtime. |
| `NavigateTo<TNavigationKey>(NavigationKeyRequest<TNavigationKey>)` | Navigate through a registered view-model or view interface/base-class key. |
| `NavigateToViewAndClearHistory<TViewModel>(NavigationKeyRequest<TViewModel>)` | Resolve, navigate, and clear the host history. |
| `NavigateBack([hostName], [parameter])` | Navigate to the previous view model, optionally on a named host and with a parameter. |
| `CanNavigateBack([hostName])` | Returns `IObservable<bool>` for the default or named host's back-navigation state. |
| `ClearHistory([hostName])` | Clear the default or named host navigation journal. |
| `WhenSetup([hostName])` | Returns `IObservable<bool>` that emits setup state for the default or named host. |
| `SetMainNavigationHost(...)` | Registers a platform host as the main navigation surface. |
| `WhenNavigating(...)`, `WhenNavigatedTo(...)`, `WhenNavigatedFrom(...)` | Hooks view or view-model navigation lifecycle callbacks. |

Example:

```csharp
public sealed class ShellViewModel : RxObject, IUseHostedNavigation
{
    public ReactiveCommand<Unit, Unit> OpenSettings { get; }

    public ShellViewModel()
    {
        OpenSettings = ReactiveCommand.Create(() =>
        {
            this.NavigateToView(new NavigationKeyRequest<SettingsViewModel>());
        });
    }
}
```

### Navigation Registry

Use `NavigationRegistry` when you need explicit view-model to view registrations or bidirectional resolution independent of a platform host.

```csharp
using CrissCross;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Primitives;

var services = new ServiceCollection();

services.AddSingleton<HomeViewModel>();
services.AddTransient<HomeView>();

var registry = new NavigationRegistry()
    .Register<HomeViewModel, HomeView>(
        sp => sp.GetRequiredService<HomeViewModel>(),
        sp => sp.GetRequiredService<HomeView>(),
        contract: "home");

services.AddSingleton<INavigationRegistry>(registry);
```

Resolve a request:

```csharp
var provider = services.BuildServiceProvider();
var registry = provider.GetRequiredService<INavigationRegistry>();

var resolution = await registry
    .CreateNavigator(provider)
    .NavigateViewModel(
        new ViewModelNavigationRequest<HomeViewModel, HomeView>
        {
            Options = new NavigationRequestOptions { Contract = "home" }
        })
    .FirstAsync();

var viewModel = resolution.ViewModel;
var view = resolution.View;
```

### Shared State Models

The core package includes state objects used by WPF, Avalonia, and MAUI controls. Prefer these models in shared view models so the same feature can be rendered by each UI stack.

Most state models are immutable snapshots. When a value changes, assign a new state instance to your view-model property and raise property change notification instead of mutating nested properties.

| Feature | Types |
| --- | --- |
| Busy and command state | `BusyOperation`, `CommandButtonState`, `CommandButtonStatus` |
| Search and paging | `SearchQueryState`, `PaginationState`, `PageRequest` |
| Filtering | `DataFilterPanelState`, `FilterDescriptor`, `FilterExpression`, `FilterToken`, `FilterOperator`, `FilterEditorKind` |
| Chips and segmented selection | `ChipModel`, `ChipModelOptions`, `ChipGroupState`, `ChipGroupSelectionMode`, `SegmentItem`, `SegmentedSelectionState` |
| Workflow steps | `StepDescriptor`, `StepDescriptorOptions`, `StepperState`, `StepStatus`, `StepperOrientation` |
| Validation and forms | `ValidationMessage`, `ValidationSummaryState`, `ValidationSeverity`, `FormFieldState` |
| Property editing | `PropertyDescriptorModel`, `PropertyDescriptorOptions`, `PropertyDescriptorGroup`, `PropertyGridState`, `PropertyEditorKind` |
| Date and time ranges | `DateTimeRange`, `DateTimeRangePreset`, `DateTimeRangePresetDefinition` |
| Theme preference | `ThemeChoice`, `ThemePreferenceState` |
| Empty states | `EmptyStateModel`, `EmptyStateVariant` |

### Filtering Model

`FilterOperator` values:

```text
Equals
NotEquals
Contains
StartsWith
EndsWith
GreaterThan
GreaterThanOrEqual
LessThan
LessThanOrEqual
Between
```

`FilterEditorKind` values:

```text
Text
Number
Boolean
Enum
Date
DateTime
DateRange
Custom
```

Example:

```csharp
var descriptors = new[]
{
    new FilterDescriptor(
        key: "area",
        displayName: "Area",
        editorKind: FilterEditorKind.Enum,
        choices: new object?[] { "North", "South" })
};

var expressions = new[]
{
    new FilterExpression("area", FilterOperator.Equals, "North")
};

var state = new DataFilterPanelState(descriptors, expressions);
var query = state.ToSearchQueryState(text: "pump alarm", resultCount: 7);
```

### Property Editing Model

`PropertyEditorKind` values:

```text
Text
Number
Boolean
Enum
Color
Date
DateTime
Command
Custom
```

Example:

```csharp
var descriptors = new[]
{
    new PropertyDescriptorModel(
        key: "name",
        displayName: "Name",
        category: "Machine",
        editorKind: PropertyEditorKind.Text,
        value: "Pump 12",
        originalValue: "Pump 12"),
    new PropertyDescriptorModel(
        key: "enabled",
        displayName: "Enabled",
        category: "Machine",
        editorKind: PropertyEditorKind.Boolean,
        value: true,
        originalValue: true)
};

var inspector = new PropertyGridState(descriptors);
```

## CrissCross.WPF

`CrissCross.WPF` supplies WPF navigation hosts and WPF-specific integration for core view-model routing.

### WPF XML Namespace

```xml
xmlns:rxNav="https://github.com/reactivemarbles/CrissCross"
```

### App Resources

Load the base WPF resource dictionary when using WPF navigation controls:

```xml
<Application
    x:Class="Example.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:rxNav="https://github.com/reactivemarbles/CrissCross"
    StartupUri="MainWindow.xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <rxNav:CrissCrossWpfDictionary />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### NavigationWindow

Use `NavigationWindow` as a XAML shell that hosts view-model navigation. `NavigationWindow<TViewModel>` is available when the shell type itself should carry a strongly typed view model.

```xml
<rxNav:NavigationWindow
    x:Class="Example.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:rxNav="https://github.com/reactivemarbles/CrissCross"
    Title="Example"
    Width="1000"
    Height="700"
    NavigateBackIsEnabled="True" />
```

```csharp
using CrissCross;
using CrissCross.WPF;

public partial class MainWindow : NavigationWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
```

Key members:

| Member | Use |
| --- | --- |
| `NavigationFrame` | The contained `ViewModelRoutedViewHost`. |
| `NavigateBackIsEnabled` | Shows or enables back navigation integration. |
| `CanNavigateBack` | Indicates whether history contains a previous view model. |
| `Transition` | Controls host transition behavior where supported. |

### ViewModelRoutedViewHost

`ViewModelRoutedViewHost` is the WPF control that displays the view located for the current view model.

```xml
<rxNav:ViewModelRoutedViewHost
    HostName="Main"
    NavigateBackIsEnabled="True" />
```

Key members:

| Member | Use |
| --- | --- |
| `HostName` | Names the host when multiple navigation surfaces exist. |
| `NavigationStack` | Read-only navigation journal. |
| `CanNavigateBack` | Returns whether back navigation is possible. |
| `ViewLocator` | ReactiveUI view locator used to resolve views. |
| `Navigate<TViewModel>(TViewModel)` / `Navigate<TViewModel>(NavigationKeyRequest<TViewModel>)` | Navigate with a supplied view model or resolve one from a typed navigation key. |
| `Navigate(IRxObject)` | Navigate to an existing view model. |
| `NavigateAndReset<TViewModel>(TViewModel)` / `NavigateAndReset<TViewModel>(NavigationKeyRequest<TViewModel>)` | Navigate with an instance or typed key and reset history. |
| `NavigateBack()` | Pop history and show the previous view. |
| `ClearHistory()` | Clear host history. |
| `Refresh()` | Re-resolve current view. |
| `Setup()` | Complete host setup and notify view models. |

### Single Instance Helper

`Make.SingleInstance` prevents multiple copies of an application from running and activates the existing main window when a second instance starts.

```csharp
using System.Windows;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        if (!Make.SingleInstance("ExampleApp"))
        {
            return;
        }

        base.OnStartup(e);
    }
}
```

### NavigationWebView

`NavigationWebView` combines a WebView2 browser surface with a WPF navigation overlay. Use it when a WPF app needs web content plus CrissCross view-model navigation hosted over the browser.

```xml
<rxNav:NavigationWebView
    Source="https://example.com"
    NavigateBackIsEnabled="True" />
```

Important members:

| Member | Use |
| --- | --- |
| `Source` | WebView2 URI source. |
| `ZoomFactor` | Browser zoom factor. |
| `Content` | Overlay WPF content. |
| `NavigationFrame` | CrissCross navigation host. |
| `NavigateBackIsEnabled` | Enables overlay back navigation. |
| `EnsureCoreWebView2Async()` | Initializes WebView2. |
| `ExecuteScriptAsync(string)` | Runs JavaScript in WebView2. |
| `NavigateToString(string)` | Loads HTML content. |
| `GoBack()`, `GoForward()`, `Reload()`, `Stop()` | Browser navigation commands. |

## CrissCross.Avalonia

`CrissCross.Avalonia` supplies Avalonia navigation windows, user controls, routed view hosts, and reactive transition content.

### Avalonia XML Namespace

```xml
xmlns:rxNav="https://github.com/reactivemarbles/CrissCross"
```

### App Styles

```xml
<Application
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    x:Class="Example.App">
    <Application.Styles>
        <FluentTheme />
        <StyleInclude Source="avares://CrissCross.Avalonia/Themes/Index.axaml" />
    </Application.Styles>
</Application>
```

### NavigationWindow

```xml
<rxNav:NavigationWindow
    x:Class="Example.MainWindow"
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:rxNav="https://github.com/reactivemarbles/CrissCross"
    Width="1000"
    Height="700"
    Title="Example" />
```

```csharp
using CrissCross.Avalonia;

public partial class MainWindow : NavigationWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
```

### NavigationUserControl

Use `NavigationUserControl` or `NavigationUserControl<TViewModel>` for nested navigation surfaces:

```xml
<rxNav:NavigationUserControl
    x:Class="Example.Views.WorkspaceView"
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:rxNav="https://github.com/reactivemarbles/CrissCross">
    <rxNav:ViewModelRoutedViewHost HostName="Workspace" />
</rxNav:NavigationUserControl>
```

### ViewModelRoutedViewHost

`ViewModelRoutedViewHost` is the Avalonia equivalent of the WPF host. It exposes the same core navigation surface:

| Member | Use |
| --- | --- |
| `HostName` | Names the host. |
| `NavigationStack` | Navigation journal. |
| `CanNavigateBack` | Indicates whether back navigation is available. |
| `Navigate<TViewModel>(TViewModel)` / `Navigate<TViewModel>(NavigationKeyRequest<TViewModel>)` | Navigate with a supplied view model or resolve one from a typed navigation key. |
| `Navigate(IRxObject)` | Navigate to an existing view model. |
| `NavigateAndReset<TViewModel>(TViewModel)` / `NavigateAndReset<TViewModel>(NavigationKeyRequest<TViewModel>)` | Navigate with an instance or typed key and clear history. |
| `NavigateBack()` | Navigate back. |
| `ClearHistory()` | Clear journal history. |
| `Refresh()` | Re-resolve current view. |
| `Setup()` | Complete setup. |

### ReactiveTransitioningContentControl

`ReactiveTransitioningContentControl` cross-fades between presenters when content changes. Use it for animated route or panel transitions.

```xml
<rxNav:ReactiveTransitioningContentControl Content="{Binding CurrentPage}" />
```

## CrissCross.MAUI

`CrissCross.MAUI` supplies a MAUI Shell implementation that behaves like the same view-model routed host used by the desktop packages.

### Register MAUI

```csharp
RxAppBuilder.CreateReactiveUIBuilder()
    .WithMaui()
    .BuildApp();
```

### NavigationShell

`NavigationShell` derives from `Microsoft.Maui.Controls.Shell` and implements `IViewModelRoutedViewHost`.

```xml
<rxNav:NavigationShell
    x:Class="Example.AppShell"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:rxNav="https://github.com/reactivemarbles/CrissCross">
</rxNav:NavigationShell>
```

```csharp
public partial class AppShell : NavigationShell
{
    public AppShell()
    {
        InitializeComponent();
    }
}
```

Key members:

| Member | Use |
| --- | --- |
| `Name` | Shell name. |
| `HostName` | Host key used by navigation-aware view models. |
| `NavigationStack` | View-model navigation history. |
| `CanNavigateBack` | Indicates whether back navigation is possible. |
| `CanNavigateBackObservable` | Observable back-navigation state. |
| `NavigateBackIsEnabled` | Enables shell back behavior. |
| `RequiresSetup` | Indicates whether setup is still required. |
| `ViewLocator` | ReactiveUI view locator used to resolve views. |
| `Navigate<TViewModel>(TViewModel)` / `Navigate<TViewModel>(NavigationKeyRequest<TViewModel>)` | Navigate with a supplied view model or resolve one from a typed navigation key. |
| `Navigate(IRxObject)` | Navigate to an existing view model. |
| `NavigateAndReset<TViewModel>(TViewModel)` / `NavigateAndReset<TViewModel>(NavigationKeyRequest<TViewModel>)` | Navigate with an instance or typed key and reset the stack. |
| `NavigateBack()` | Pop shell navigation. |
| `ClearHistory()` | Clear view-model history. |
| `Refresh()` | Re-resolve current route. |
| `Setup()` | Complete setup and register hosted navigation. |
| `Dispose()` | Release subscriptions and host state. |

## CrissCross.WinForms

`CrissCross.WinForms` supplies WinForms hosts for the same view-model routing model.

### NavigationForm

Use `NavigationForm` or `NavigationForm<TViewModel>` as the main form:

```csharp
using CrissCross.WinForms;

public sealed class MainForm : NavigationForm<MainViewModel>
{
    public MainForm()
    {
        Text = "Example";
        Width = 1000;
        Height = 700;

        NavigateBackIsEnabled = true;
    }
}
```

Key members:

| Member | Use |
| --- | --- |
| `NavigationFrame` | Contained `ViewModelRoutedViewHost`. |
| `NavigationFrameDock` | WinForms docking mode for the host. |
| `NavigateBackIsEnabled` | Enables hosted back behavior. |
| `CanNavigateBack` | Indicates whether the host can go back. |

### ViewModelRoutedViewHost

The WinForms host exposes the same core navigation methods as WPF and Avalonia. It resolves ReactiveUI views and places them into the host control.

```csharp
var host = new ViewModelRoutedViewHost
{
    Dock = DockStyle.Fill,
    HostName = "Main"
};

Controls.Add(host);
host.Setup();
host.Navigate(new HomeViewModel());
```

## CrissCross.WPF.UI

`CrissCross.WPF.UI` is the primary themed UI package. It contains WPF control wrappers, Fluent-style shell controls, feature controls backed by core state models, resource dictionaries, services, and theme managers.

### WPF UI XML Namespace

Use the CrissCross UI namespace for CrissCross themed controls:

```xml
xmlns:ui="https://github.com/reactivemarbles/CrissCross.ui"
```

The WPF UI assembly maps this namespace to CrissCross controls and common WPF namespaces so application XAML can use a single `ui:` prefix for the themed CrissCross surface. The root `Application` and low-level WPF infrastructure still use the normal WPF XML namespace.

### App Resources

Load controls and theme dictionaries once at application startup:

```xml
<Application
    x:Class="Example.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:ui="https://github.com/reactivemarbles/CrissCross.ui"
    StartupUri="MainWindow.xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ui:ControlsDictionary />
                <ui:ThemesDictionary Theme="Dark" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

`ThemesDictionary` accepts `Light`, `Dark`, and `HighContrast`.

### Host Builder Setup

`HostBuilderMixins` wires common WPF UI services and navigation hosts into Microsoft.Extensions.Hosting.

```csharp
using CrissCross.WPF.UI;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureCrissCrossForPageNavigation<MainWindow, HomePage>(
        new PageNavigationRegistration<MainWindow, HomePage>())
    .Build();

await host.StartAsync();
```

For view-model navigation:

```csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureCrissCrossForViewModelNavigation<MainWindow, ShellViewModel>(
        new ViewModelNavigationRegistration<MainWindow, ShellViewModel>())
    .Build();
```

### Services

| Service | Implementation | Use |
| --- | --- | --- |
| `INavigationService` | `NavigationService` | Navigate `NavigationView` pages by type or string key, go back/forward, and navigate with hierarchy. |
| `IPageService` | `PageService` | Resolve pages from dependency injection. |
| `IContentDialogService` | `ContentDialogService` | Show `ContentDialog` instances from view models or services. |
| `ISnackbarService` | `SnackbarService` | Show transient messages through a registered snackbar presenter. |
| `IThemeService` | `ThemeService` | Read and apply app theme, system theme, and accent color. |
| `ITaskBarService` | `TaskBarService` | Set Windows taskbar progress state and value. |
| hosted app startup | `ApplicationHostService` | Starts the configured WPF main window from the generic host. |

Navigation service example:

```csharp
public sealed class ShellViewModel
{
    private readonly INavigationService navigation;

    public ShellViewModel(INavigationService navigation)
    {
        this.navigation = navigation;
    }

    public void OpenSettings()
    {
        navigation.Navigate(typeof(SettingsPage));
    }
}
```

Snackbar example:

```csharp
snackbarService.Show(
    title: "Saved",
    message: "Changes have been written.",
    appearance: ControlAppearance.Success,
    icon: null,
    timeout: TimeSpan.FromSeconds(3));
```

Theme example:

```csharp
themeService.SetTheme(ApplicationTheme.Dark);
themeService.SetSystemAccent();
```

### Shell And Navigation Controls

| Control | Use |
| --- | --- |
| `FluentWindow` | Window base with themed chrome and content integration. |
| `FluentNavigationWindow` | Window with navigation layout and Fluent shell behavior. |
| `ModernWindow` | Modern themed WPF window. |
| `Window` | Themed WPF window wrapper. |
| `TitleBar` | Custom title bar/chrome surface. |
| `NavigationView` | Left/top navigation menu, content host, footer items, pane display modes, selection, and hierarchy. |
| `NavigationViewItem` | Selectable navigation item. |
| `NavigationViewItemHeader` | Navigation section header. |
| `NavigationViewItemSeparator` | Navigation separator. |
| `NavigationViewContentPresenter` | Frame-based presenter for selected navigation content. |
| `INavigationView`, `INavigationViewItem`, `INavigationAware`, `INavigableView<T>` | Navigation shell, item, lifecycle, and typed-view contracts. |
| `NavigationViewPaneDisplayMode`, `NavigationViewBackButtonVisible`, `NavigationCacheMode` | Pane, back-button, and page-cache configuration. |
| `BreadcrumbBar` | Hierarchical breadcrumb path with selected item navigation. |
| `ClientAreaBorder` | Themed border used around custom chrome client content. |
| `Frame` | Themed frame for page content. |
| `Page` | Themed WPF page base. |
| `NavigationUserControl` | User control base for navigation-aware content. |
| `AppBar`, `AppBarButton` | Command bar surface and command button. |
| `TabControl`, `TabView` | Themed tabbed navigation and tabbed document surfaces. |
| `LoadingScreen` | Startup or long-running loading surface. |

NavigationView example:

```xml
<ui:FluentNavigationWindow
    x:Class="Example.MainWindow"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="clr-namespace:Example.Views"
    xmlns:ui="https://github.com/reactivemarbles/CrissCross.ui"
    Title="Example"
    Width="1100"
    Height="750">
    <ui:NavigationView x:Name="RootNavigation">
        <ui:NavigationView.MenuItems>
            <ui:NavigationViewItem Content="Home" TargetPageType="{x:Type local:HomePage}" />
            <ui:NavigationViewItem Content="Settings" TargetPageType="{x:Type local:SettingsPage}" />
        </ui:NavigationView.MenuItems>
    </ui:NavigationView>
</ui:FluentNavigationWindow>
```

### Primitive, Layout, And Text Controls

| Control | Use |
| --- | --- |
| `AccessText` | Themed access-key text. |
| `Anchor` | Link-like navigation or command element. |
| `Border` | Themed border wrapper. |
| `Button` | Themed button. |
| `Grid` | Themed grid wrapper. |
| `GroupBox` | Grouped content container. |
| `HyperlinkButton` | Button styled and behaving like a link. |
| `ItemsControl` | Themed item host. |
| `Label` | Themed label. |
| `ScrollBar`, `ScrollViewer` | Themed scrolling controls. |
| `DynamicScrollBar`, `DynamicScrollViewer` | Scroll controls with dynamic visibility and layout behavior. |
| `Separator` | Themed separator line. |
| `StackPanel` | Themed stack panel wrapper. |
| `StatusBar` | Status area container. |
| `TextBlock` | Themed text display. |
| `BBCodeBlock` | Theme-aware BBCode text with safe external links and `cmd:` command links. |
| `ToolBar` | Themed toolbar container. |
| `ToolTip` | Themed tooltip. |

### Input And Editor Controls

| Control | Use |
| --- | --- |
| `TextBox` | Themed text input with icon and placeholder support where available. |
| `PasswordBox` | Themed password input. |
| `NumberBox` | Numeric input. |
| `NumberPad` | On-screen numeric keypad. |
| `NumericPushButton` | Numeric increment/decrement command button. |
| `ComboBox` | Themed selection drop-down. |
| `AutoSuggestBox` | Text input with suggestion list and query submission. |
| `SearchBox` | Search-specific input bound to `SearchQueryState`. |
| `DatePicker`, `TimePicker`, `DateTimePicker`, `CalendarDatePicker` | Date and time selection controls. |
| `DateTimeRangePicker` | Range and preset selection backed by `DateTimeRange`. |
| `Calendar` | Themed calendar. |
| `Slider`, `SquareSlider`, `HueSlider` | Numeric and color-channel sliders. |
| `CheckBox`, `CheckBoxModern` | Boolean selection controls. |
| `RadioButton` | Exclusive option selection. |
| `ToggleButton`, `ToggleSwitch` | Toggle state controls. |
| `ColorPicker`, `ColorSelector` | Color selection UI. |
| `RatingControl` | Rating value selector. |
| `ThumbRate` | Thumb-based rating or preference control. |

### Command Controls

| Control | Use |
| --- | --- |
| `CommandButton` | Button bound to `CommandButtonState`, including executing and progress states. |
| `AsyncCommandButton` | Async command button surface derived from `CommandButton`. |
| `DropDownButton` | Button with flyout menu/content. |
| `SplitButton` | Primary command plus secondary flyout. |
| `ToggleButton` | Toggle command state. |
| `HyperlinkButton` | Link-styled command. |

### Data, Lists, And Virtualization

| Control | Use |
| --- | --- |
| `DataGrid` | Themed tabular data grid. |
| `DataPager` | Paging command surface backed by `PaginationState`. |
| `DataFilterPanel` | Filter builder and query submission control. |
| `FilterBar` | Active filter chips and clear/remove commands. |
| `GridView` | Grid item presentation. |
| `ListBox`, `ListView` | Themed list selection and display. |
| `TreeView` | Hierarchical item view. |
| `TreeGrid` | Hierarchical tabular data. |
| `VirtualizingItemsControl` | Virtualized items host. |
| `VirtualizingWrapPanel` | Virtualized wrapping panel. |
| `VirtualizingGridView` | Virtualized grid display. |
| `PropertyGridLite` | Property editor generated from `PropertyGridState`. |
| `ReactiveFormField` | Field container driven by validation and form state. |
| `ValidationSummary` | Validation message list backed by `ValidationSummaryState`. |

### Feedback, Surfaces, And Media

| Control | Use |
| --- | --- |
| `AlarmBanner`, `Alarms` | Alarm status and alarm-list presentation controls. |
| `Arc` | Arc shape/control for indicators and visuals. |
| `Badge`, `InfoBadge` | Count/status badges. |
| `BusyOverlay` | Overlay for cancellable or long-running work. |
| `Card`, `CardAction`, `CardColor`, `CardControl`, `CardExpander` | Themed card surfaces and actions. |
| `ContentDialog` | Modal dialog surface. |
| `ContextMenu`, `Menu` | Menu surfaces. |
| `EmptyState` | Empty, loading, error, or success placeholder state. |
| `Expander`, `CardExpander` | Collapsible content. |
| `Flyout` | Lightweight popup/flyout content. |
| `Gauges` | Gauge indicator controls. |
| `GifImage`, `Image`, `PersonPicture` | Media and avatar controls. |
| `InfoBar` | Inline status message with severity. |
| `MessageBox`, `MessageBoxAsync` | Synchronous and asynchronous message dialogs. |
| `ProgressBar`, `ProgressRing` | Progress indicators. |
| `RichTextBox` | Rich text editing/display. |
| `Snackbar` | Transient notification surface. |

### Icons And Appearance

| Type | Use |
| --- | --- |
| `IconElement` | Base type for icon elements. |
| `IconSource` | Source model for icon creation. |
| `FontIcon`, `FontIconSource` | Font glyph icons. |
| `SymbolIcon`, `SymbolIconSource` | Symbol icons. |
| `ImageIcon` | Image-backed icon. |
| `SymbolGlyph`, `SymbolRegular`, `SymbolFilled` | Built-in symbol glyph definitions. |
| `IAppearanceControl` | Contract for controls that expose appearance. |
| `IIconControl` | Contract for controls exposing an icon. |
| `IThemeControl` | Contract for theme-aware controls. |
| `ControlAppearance` | Appearance enum used by many controls. |

### Feature Controls

These controls share their view-model state with Avalonia and MAUI equivalents.

| Control | Primary state | Important members |
| --- | --- | --- |
| `BusyOverlay` | `BusyOperation` | Shows busy text/progress over existing content. |
| `CommandButton` | `CommandButtonState` | `State`, `IsExecuting`, `Progress`, `ExecutingContent`, `ErrorContent`. |
| `SearchBox` | `SearchQueryState` | `Text`, `PlaceholderText`, `QueryState`, `SearchCommand`, `ClearCommand`. `SearchCommand` receives the current text as a string command parameter. |
| `FilterBar` | `SearchQueryState` | `QueryState`, `RemoveFilterCommand`, `ClearAllCommand`. |
| `DataFilterPanel` | `DataFilterPanelState` | `FilterState`, `SearchText`, `ResultCount`, `SubmittedQueryState`, `ApplyFiltersCommand`, `ClearFiltersCommand`, `AddFilterCommand`, `RemoveFilterCommand`. |
| `DataPager` | `PaginationState`, `PageRequest` | `PaginationState`, `CurrentRequest`, `PageRequestCommand`, `SortKey`, `SortDescending`, `QueryState`. |
| `DateTimeRangePicker` | `DateTimeRange` | `Start`, `End`, `CurrentRange`, `SelectedPreset`, `RangeLabel`, `ReferenceTime`, range commands. |
| `ThemeSwitcher` | `ThemePreferenceState` | `SelectedChoice`, `SystemChoice`, `SupportsHighContrast`, `CurrentState`, `ThemeService`, `ThemeChangedCommand`. |
| `Chip`, `ChipGroup` | `ChipModel`, `ChipGroupState` | Selection, close/remove, icon, and grouping state. |
| `SegmentedControl` | `SegmentedSelectionState` | `SelectionState`, `SelectedKey`, `SelectionChangedCommand`. |
| `Stepper` | `StepperState` | `State`, `CurrentKey`, `StepRequestedCommand`, `FinishCommand`, `CancelCommand`. |
| `ValidationSummary` | `ValidationSummaryState` | `SummaryState`, `NavigateToFieldCommand`. |
| `PropertyGridLite` | `PropertyGridState` | `InspectorState`, `SearchText`, `CommitChangesCommand`, `ResetChangesCommand`. |
| `ReactiveFormField` | `FormFieldState` | Field label, hint, validation messages, required state. |
| `EmptyState` | `EmptyStateModel` | Title, description, icon, variant, primary action. |

### BBCodeBlock Rich Text And Links

WPF and Avalonia UI both provide `Controls.BBCodeBlock`. It renders lightweight BBCode including bold, italic, underline, strike-through, color, size, font, paragraph, list, break, code, URL, and email markup. A `[url=cmd:payload]...[/url]` link invokes `Command`; HTTP, HTTPS, and mail links follow `OpenExternalLinks` and raise `ExternalLinkRequested`.

| Member | WPF | Avalonia |
| --- | --- | --- |
| `BBCode` | Dependency property | Styled property |
| `Command`, `CommandParameter` | Supported | Supported |
| `CommandTarget` | Supported for routed commands | Not applicable |
| `OpenExternalLinks` | Supported | Supported |
| `ExternalLinkRequested` | `BBCodeExternalLinkRequestedEventArgs` | `BBCodeLinkRequestedEventArgs` |

```csharp
using System.Diagnostics;
using CrissCross.WPF.UI.Controls;

var command = ReactiveCommand.Create<string>(
    payload => Trace.WriteLine($"BBCode command: {payload}"));

var message = new BBCodeBlock
{
    BBCode = "[b]Status[/b]: [color=#4FC3F7]ready[/color]. "
        + "[url=cmd:refresh]Refresh[/url] or "
        + "[url=https://github.com/reactivemarbles/CrissCross]open documentation[/url].",
    Command = command,
    OpenExternalLinks = false
};

message.ExternalLinkRequested +=
    (_, eventArgs) => Trace.WriteLine(eventArgs);
```

## AICS.Windows.Controls.ColorPicker

This independently packable WPF project is maintained under `CrissCross.WPF.UI/Controls/ColorSelector`. Despite the package ID, its public types use `CrissCross.WPF.UI` (or `CrissCross.Reactive.WPF.UI` when linked into the Reactive WPF UI package).

| API family | Public types and members |
| --- | --- |
| Picker bases | `PickerControlBase` (`ColorState`, `SelectedColor`, `ColorChanged`) and `DualPickerControlBase` (`SecondaryColor`, `HintColor`, `UseHintColor`, `SwapColors()`, `SetMainColorFromHintColor()`). |
| Complete pickers | `StandardColorPicker`, `PortableColorPicker`, `SquarePicker`, `ColorSliders`, `ColorDisplay`, `AlphaSlider`, `HexColorTextBox`. |
| Picker configuration | `PickerType` (`HSV`, `HSL`), `SmallChange`, `ShowAlpha`, `ShowHex`, `ShowSliders`, `ShowColorSwap`, `ShowPickerType`. |
| Color models | `NotifyableColor`, `RgbColorComponents`, `HslColorComponents`, `HsvColorComponents`, `ColorRoutedEventArgs`. |
| Storage contracts | Public `IColorStateStorage` and `IHintColorStateStorage`; secondary-color storage remains an internal implementation detail. |
| Channel sliders | `PreviewColorSlider`, `RgbColorSlider`, `HslColorSlider`, `HsvColorSlider`. |

C# example:

```csharp
using System.Diagnostics;
using System.Windows.Media;
using CrissCross.WPF.UI;

var picker = new StandardColorPicker
{
    SelectedColor = Colors.CornflowerBlue,
    PickerType = PickerType.HSV,
    ShowAlpha = true
};

picker.ColorChanged += (_, _) => Trace.WriteLine(picker.SelectedColor);
```

## CrissCross.Avalonia.UI

`CrissCross.Avalonia.UI` mirrors the WPF UI package for Avalonia. It provides themed controls, feature controls, services, themes, and resources for Avalonia apps.

### Avalonia UI XML Namespace

```xml
xmlns:ui="https://github.com/reactivemarbles/CrissCross.Avalonia.UI"
```

### App Styles

Load Avalonia Fluent theme plus CrissCross UI styles:

```xml
<Application
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    x:Class="Example.App">
    <Application.Styles>
        <FluentTheme />
        <StyleInclude Source="avares://CrissCross.Avalonia/Themes/Index.axaml" />
        <StyleInclude Source="avares://CrissCross.Avalonia.UI/Themes/Index.axaml" />
    </Application.Styles>
</Application>
```

### Services

| Service | Implementation | Use |
| --- | --- | --- |
| `INavigationService` | `NavigationService` | Navigate pages by type or string key, and move back/forward. |
| `IPageService` | `PageService` | Resolve pages from dependency injection. |
| `IContentDialogService` | `ContentDialogService` | Show content dialogs through a registered presenter. |
| `ISnackbarService` | `SnackbarService` | Show transient notifications. |
| `IThemeService` | `ThemeService` | Apply application, system, and accent themes. |
| settings persistence | `IStore`, `JsonFileStore` | Store and retrieve JSON-backed app settings. |

Theme helpers:

| Type | Use |
| --- | --- |
| `ApplicationThemeManager` | Reads and applies app theme and accent color. |
| `SystemThemeManager` | Reads and caches operating-system theme information. |

### Avalonia Control Catalog

`CrissCross.Avalonia.UI` contains Avalonia implementations of the same WPF UI control set, including:

```text
AccessText
AlarmBanner
Alarms
Anchor
AppBar
Arc
AutoSuggestBox
Badge
BBCodeBlock
Border
BreadcrumbBar
BusyOverlay
Button
Calendar
CalendarDatePicker
Card
CardAction
CardColor
CardControl
CardExpander
CheckBox
CheckBoxModern
Chip
ChipGroup
ClientAreaBorder
ColorPicker
ColorSelector
ComboBox
CommandButton
ContentDialog
ContextMenu
DataFilterPanel
DataGrid
DataPager
DatePicker
DateTimePicker
DateTimeRangePicker
DropDownButton
DynamicScrollBar
DynamicScrollViewer
EmptyState
Expander
FilterBar
FluentNavigationWindow
FluentWindow
Flyout
Frame
Gauges
GifImage
Grid
GridView
GroupBox
HueSlider
HyperlinkButton
IconElement
IconSource
Image
InfoBadge
InfoBar
ItemsControl
Label
ListBox
ListView
LoadingScreen
Menu
MenuItem
MessageBox
MessageBoxAsync
ModernWindow
NavigationControls
NavigationUserControl
NavigationView
NumberBox
NumberPad
NumericPushButton
Page
PasswordBox
PersonPicture
ProgressBar
ProgressRing
PropertyGridLite
RadioButton
RatingControl
ReactiveFormField
RichTextBox
ScrollBar
ScrollViewer
SearchBox
SegmentedControl
Separator
Slider
Snackbar
SplitButton
SquareSlider
StackPanel
StatusBar
Stepper
TabControl
TabView
TextBlock
TextBox
ThemeSwitcher
ThumbRate
TimePicker
TitleBar
ToggleButton
ToggleSwitch
ToolBar
ToolTip
TreeGrid
TreeView
ValidationSummary
VirtualizingGridView
VirtualizingItemsControl
VirtualizingWrapPanel
Window
```

### Avalonia Feature Controls

The Avalonia feature controls use the same core state models as WPF:

| Control | Primary state | Important members |
| --- | --- | --- |
| `BusyOverlay` | `BusyOperation` | Busy text/progress over existing content. |
| `CommandButton` | `CommandButtonState` | `State`, `IsExecuting`, `Progress`, `ExecutingContent`, `ErrorContent`. |
| `SearchBox` | `SearchQueryState` | `Text`, `PlaceholderText`, `QueryState`, `SearchCommand`, `ClearCommand`. `SearchCommand` receives the current text as a string command parameter. |
| `FilterBar` | `SearchQueryState` | `QueryState`, `RemoveFilterCommand`, `ClearAllCommand`. |
| `DataFilterPanel` | `DataFilterPanelState` | `FilterState`, `SearchText`, `ResultCount`, `SubmittedQueryState`, filter commands. |
| `DataPager` | `PaginationState`, `PageRequest` | Page move commands and query/sort request generation. |
| `DateTimeRangePicker` | `DateTimeRange` | Start/end, presets, labels, and range commands. |
| `ThemeSwitcher` | `ThemePreferenceState` | `SelectedChoice`, `SystemChoice`, `SupportsHighContrast`, `CurrentState`, `ThemeService`, `ThemeChangedCommand`. |
| `Chip`, `ChipGroup` | `ChipModel`, `ChipGroupState` | Chip selection and grouping. |
| `SegmentedControl` | `SegmentedSelectionState` | `SelectionState`, `SelectedKey`, `SelectionChangedCommand`. |
| `Stepper` | `StepperState` | `State`, `CurrentKey`, step navigation commands. |
| `ValidationSummary` | `ValidationSummaryState` | Validation message display and field navigation. |
| `PropertyGridLite` | `PropertyGridState` | `InspectorState`, `SearchText`, commit/reset commands. |
| `ReactiveFormField` | `FormFieldState` | Field label, hint, validation, and required state. |
| `EmptyState` | `EmptyStateModel` | Title, description, icon, variant, and action. |

### Avalonia HSV Color Selector

`CrissCross.Avalonia.UI.Controls.ColorSelector` exposes two-way Avalonia styled properties for `SelectedColor`, `Hue` (0–360), `Saturation` (0–1), `Brightness` (0–1), `Alpha` (0–1), and the derived full-saturation `HueColor`.

```csharp
using Avalonia.Media;
using CrissCross.Avalonia.UI.Controls;

var selector = new ColorSelector
{
    Hue = 205,
    Saturation = 0.68,
    Brightness = 0.92,
    Alpha = 1
};

Color selected = selector.SelectedColor;
```

### Avalonia Examples

Auto suggest input:

```xml
<ui:AutoSuggestBox
    ItemsSource="{Binding Suggestions}"
    Text="{Binding SearchText}"
    PlaceholderText="Start typing..." />
```

Split button:

```xml
<ui:SplitButton Content="Run" Command="{Binding RunCommand}">
    <ui:SplitButton.Flyout>
        <MenuFlyout>
            <MenuItem Header="Run all" Command="{Binding RunAllCommand}" />
            <MenuItem Header="Run selected" Command="{Binding RunSelectedCommand}" />
        </MenuFlyout>
    </ui:SplitButton.Flyout>
</ui:SplitButton>
```

Date picker:

```xml
<ui:DatePicker SelectedDate="{Binding DueDate}" />
```

## CrissCross.Maui.UI

`CrissCross.Maui.UI` provides MAUI controls backed by the same core models used by WPF and Avalonia feature controls. This allows shared view models to drive native MAUI UI.

### Register MAUI UI

```csharp
builder
    .UseMauiApp<App>()
    .UseCrissCrossMauiUi();

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        Resources.UseCrissCrossMauiUiResources();
        MainPage = new AppShell();
    }
}
```

You can also merge the XAML resource dictionary manually:

```xml
<Application
    x:Class="Example.App"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:styles="clr-namespace:CrissCross.Maui.UI.Resources.Styles;assembly=CrissCross.Maui.UI">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <styles:CrissCrossMauiUi />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### MAUI UI Controls

Use this XAML namespace for MAUI UI controls:

```xml
xmlns:ui="clr-namespace:CrissCross.Maui.UI.Controls;assembly=CrissCross.Maui.UI"
```

| Control | Primary state | Important members |
| --- | --- | --- |
| `AlarmBanner` | alarm presentation | `IsActive`, `Message`, `Severity`, `AcknowledgeCommand`, `CloseCommand`. |
| `AsyncCommandButton` | command state | `Command`, `CancelCommand`, `CancelText`, async/cancel presentation. |
| `BusyOverlay` | `BusyOperation` | Busy content overlay and progress state. |
| `Card`, `CardAction`, `CardColor`, `CardExpander` | card presentation | Base card content, actions, title/subtitle/color, expandable header/content, and `IsExpanded`. |
| `Chip` | `ChipModel` | Text, selected state, close/remove behavior. |
| `ChipGroup` | `ChipGroupState` | Multiple chips with selection mode. |
| `CommandButton` | `CommandButtonState` | `State`, `IsExecuting`, `Progress`, command presentation. |
| `DataFilterPanel` | `DataFilterPanelState` | `FilterPanelState`, `ApplyFiltersCommand`. |
| `DataPager` | `PaginationState`, `PageRequest` | `PaginationState`, `CurrentRequest`, `PageRequestCommand`, page commands. |
| `DateTimeRangePicker` | `DateTimeRange` | `Range`, `ApplyRangeCommand`. |
| `EmptyState` | `EmptyStateModel` | Empty/loading/error/success placeholder. |
| `FilterBar` | `SearchQueryState` | `SearchState`, `ClearFiltersCommand`. |
| `InfoBadge`, `InfoBadgeSeverity` | status badge | Severity-driven compact status indicator. |
| `InfoBar`, `InfoBarSeverity` | message surface | `Title`, `Message`, `Severity`, `ActionText`, `ActionCommand`. |
| `PersonPicture` | identity presentation | `DisplayName`, `Initials`, `Source`. |
| `PropertyGridLite` | `PropertyGridState` | `PropertyGridState`, `UpdatePropertyCommand`. |
| `RatingControl` | rating value | `Value`, `MaxRating`, `IsReadOnly`, `ValueChangedCommand`. |
| `ReactiveFormField` | `FormFieldState` | `FieldState`. |
| `SearchBox` | `SearchQueryState` | `Text`, `SearchState`, `SubmitCommand`, `SubmitSearch()`. |
| `SegmentedControl` | `SegmentedSelectionState` | `State`, `SelectedKey`, `SelectionCommand`, `SelectSegment`. |
| `Snackbar` | transient message | `IsShown`, `Title`, `Message`, `IsCloseButtonEnabled`, `CloseCommand`. |
| `Stepper` | `StepperState` | `StepperState`, `StepCommand`. |
| `ThemeSwitcher` | `ThemePreferenceState` | `ThemeState`, `ChangeThemeCommand`. |
| `ValidationSummary` | `ValidationSummaryState` | `SummaryState`. |

### MAUI Examples

Search box:

```xml
<ui:SearchBox
    Text="{Binding Search.Text}"
    SearchState="{Binding Search}"
    SubmitCommand="{Binding SubmitSearchCommand}" />
```

Data pager:

```xml
<ui:DataPager
    PaginationState="{Binding Paging}"
    PageRequestCommand="{Binding PageRequestCommand}" />
```

Stepper:

```xml
<ui:Stepper
    StepperState="{Binding WizardSteps}"
    StepCommand="{Binding StepCommand}" />
```

Theme switcher:

```xml
<ui:ThemeSwitcher
    ThemeState="{Binding ThemePreference}"
    ChangeThemeCommand="{Binding ChangeThemeCommand}" />
```

## Cross-Platform Feature Patterns

The feature controls are designed so a shared view model can power WPF, Avalonia, and MAUI UI. Use the core state models as public properties and bind platform controls to those properties.

```csharp
public sealed class OrdersViewModel : RxObject
{
    private SearchQueryState search = new();
    private PaginationState paging = new(pageIndex: 0, pageSize: 25, totalItemCount: 0);

    public SearchQueryState Search
    {
        get => search;
        private set => this.RaiseAndSetIfChanged(ref search, value);
    }

    public PaginationState Paging
    {
        get => paging;
        private set => this.RaiseAndSetIfChanged(ref paging, value);
    }

    public DataFilterPanelState Filters { get; } = new();
    public ValidationSummaryState Validation { get; } = new(Array.Empty<ValidationMessage>());

    public ReactiveCommand<PageRequest, Unit> PageRequested { get; }

    public OrdersViewModel()
    {
        PageRequested = ReactiveCommand.Create<PageRequest>(request =>
        {
            Paging = new PaginationState(request.PageIndex, request.PageSize, Paging.TotalItemCount);
        });

    }
}
```

WPF/Avalonia binding shape:

```xml
<ui:SearchBox QueryState="{Binding Search}" />
<ui:DataFilterPanel FilterState="{Binding Filters}" />
<ui:DataPager PaginationState="{Binding Paging}" PageRequestCommand="{Binding PageRequested}" />
<ui:ValidationSummary SummaryState="{Binding Validation}" />
```

MAUI binding shape:

```xml
<ui:SearchBox SearchState="{Binding Search}" />
<ui:DataFilterPanel FilterPanelState="{Binding Filters}" />
<ui:DataPager PaginationState="{Binding Paging}" PageRequestCommand="{Binding PageRequested}" />
<ui:ValidationSummary SummaryState="{Binding Validation}" />
```

## CrissCross.WPF.Plot

`CrissCross.WPF.Plot` integrates ScottPlot with ReactiveUI and CrissCross WPF UI. It provides live chart controls, plot adapters, reactive data sources, and a right-side properties view.

Use the CrissCross UI XML namespace for plot controls:

```xml
xmlns:plot="https://github.com/reactivemarbles/CrissCross.ui"
```

```xml
<plot:LiveChart />
```

### Main Types

| Type | Use |
| --- | --- |
| `LiveChart` | WPF view that hosts the live plot. |
| `LiveChartViewModel` | Main chart view model with plot setup, commands, axes, markers, and right-side properties. |
| `RightPropertiesView`, `RightPropertiesViewModel` | Plot property editing surface. |
| `ReactivePlotSource` | Factory and source object for reactive plot updates. |
| `ReactivePlotBinder` | Binds one or more `IReactivePlotSource` instances to a chart. |
| `ReactivePlotUpdate` | Describes an append, snapshot, clear, or replace update. |
| `ReactivePlotBindingOptions` | Controls schedulers, batching, visible point limits, overflow, and error mode. |
| `PlotSeriesKey` | Identifies a plotted series. |
| `IReactivePlotAdapter`, `IReactivePlotAdapterFactory` | Adapter abstraction over concrete plot controls. |
| `IReactivePlotConnection` | Disposable binding connection. |
| `IReactivePlotBinder`, `IReactivePlotSource` | Binding and source contracts. |
| `AxisLinesUI`, `Crosshair_UI`, `DataLoggerUI`, `ScatterUI`, `SignalUI`, `SignalXY_UI`, `StreamerUI` | Plot UI models for supported plot item kinds. |
| `IAppearance`, `IPlottableUI` | Plot appearance and plottable contracts. |

### Plot Enums

| Enum | Use |
| --- | --- |
| `PlotType` | Identifies plot type. |
| `UserPlotType` | Identifies user-selectable plot type. |
| `LegendPosition` | Places the plot legend. |
| `PlotXAxisKind` | Selects point or timestamp X axis behavior. |
| `ReactivePlotUpdateKind` | Describes update kind. |
| `ReactivePlotOverflowStrategy` | Controls behavior when max visible points are exceeded. |
| `ReactivePlotErrorMode` | Controls binding error handling. |
| `ReactivePlotConnectionState` | Reports binding connection state. |

### LiveChartViewModel Features

`LiveChartViewModel` supports:

| Feature | Members |
| --- | --- |
| WPF plot creation | Construct `LiveChartViewModel(Grid)` with the host grid; the view model initializes the ScottPlot surface and chart state. |
| Axis configuration | `CreateAxisWithTimeStamp(...)`, `CreateAxisWithPoints(...)`, `AxisStyle(...)`, `YAxesSetup(...)`, `HideAllYAxis()`. |
| Scaling | `ManualScaleY(...)` plus autoscale command helpers. |
| Crosshairs and labels | `AddCrosshairBtn`, `ClearLabels()`, `ClearAxisCrosshairs()`, `ClearAxisLines()`. |
| Marker and lock commands | `EnableMarkerBtn`, `GraphLocked`. |
| Menus and properties | `ExpandMenuBtn`, `LinePropCommand`, right properties view model. |
| Cleanup | `ClearContent()`. |

### Reactive Plot Binding

Create a signal source and bind it to a chart:

```csharp
using CrissCross.WPF.Plot;
using ReactiveUI.Primitives;
using Signal = ReactiveUI.Primitives.Signals.Signal;

var ticks = Signal.Every(TimeSpan.FromMilliseconds(100))
    .Select(i => (
        Name: "temperature",
        Value: (IList<double>)new[] { Math.Sin(i / 10d) },
        DateTime: (IList<double>)new[] { (double)i },
        Axis: 0));

var source = ReactivePlotSource.FromSignalTicks(ticks);

var options = new ReactivePlotBindingOptions
{
    BatchWindow = TimeSpan.FromMilliseconds(50),
    MaxBatchSize = 250,
    MaxVisiblePoints = 10_000,
    OverflowStrategy = ReactivePlotOverflowStrategy.DropOldest,
    ErrorMode = ReactivePlotErrorMode.SurfaceAndStopSeries
};

IReactivePlotConnection connection = new ReactivePlotBinder()
    .Bind(liveChartViewModel, new[] { source }, options);
```

Factory methods available on `ReactivePlotSource`:

| Factory | Use |
| --- | --- |
| `FromUpdates(...)` | Bind a custom observable of `ReactivePlotUpdate`. |
| `FromSignalTicks(...)` | Append signal values by tick. |
| `FromScatterPoints(...)` | Append X/Y scatter points. |
| `FromDataLoggerPoints(...)` | Append data logger points. |
| `FromStreamerPoints(...)` | Append streaming values. |
| `FromSignalXyPoints(...)` | Append SignalXY points. |
| `FromSignalXySnapshot(...)` | Replace SignalXY points with snapshot data. |
| `FromStatic(...)` | Render an immutable `PlotSeriesData` snapshot with optional plot type and style. |
| `FromHistoric(...)` | Reduce a historic series to a target point count before rendering. |
| `FromLive(...)` | Stream numeric `PlotDataPoint` values with optional retention and style settings. |
| `FromDateTimeLive(...)` | Stream timestamped values using the OLE Automation date X-axis representation. |

### Historic Data, Styling, Themes, And Studies

| API | Use |
| --- | --- |
| `PlotSeriesData` | Immutable numeric or `DateTime` X/Y series; use `Numeric(...)`, `DateTime(...)`, `Derive(...)`, and `ToUpdate(...)`. |
| `ReactivePlotSeriesStyle` | Immutable series color, line width, marker size, line/baseline modes, baseline, and legend visibility. |
| `ReactivePlotTheme` | Built-in `Dark` and `Light` plot palettes or an immutable custom palette. Apply with `LiveChartViewModel.ApplyTheme(...)`. |
| `PlotDataReducer.LargestTriangleThreeBuckets(...)` | LTTB downsampling that preserves the visual shape of large historic series. |
| `TechnicalIndicators` | Static SMA, EMA, RSI, MACD, Bollinger Bands, and Ichimoku results over `PlotSeriesData`. |
| `ReactivePlotStudies` | Live SMA, EMA, RSI, MACD, Bollinger Bands, and Ichimoku derived `IReactivePlotSource` streams. |

Continuing from the live `source` created in the previous example:

```csharp
var samples = Enumerable.Range(0, 10_000)
    .Select(static value => Math.Sin(value / 25d))
    .ToArray();

var history = PlotSeriesData.Numeric(
    "temperature",
    axis: 0,
    x: Enumerable.Range(0, samples.Count).Select(static value => (double)value).ToArray(),
    y: samples);

var reduced = PlotDataReducer.LargestTriangleThreeBuckets(history, targetPointCount: 1_000);
var movingAverage = TechnicalIndicators.SimpleMovingAverage(reduced, period: 20);

var style = new ReactivePlotSeriesStyle
{
    Color = "#4FC3F7",
    LineWidth = 2,
    ShowInLegend = true
};

var historicSource = ReactivePlotSource.FromStatic(movingAverage, PlotType.Line, style);
var liveRsiSource = ReactivePlotStudies.RelativeStrengthIndex(source, period: 14);

liveChartViewModel.ApplyTheme(ReactivePlotTheme.Dark);
```

## CrissCross.WPF.WebView2

`CrissCross.WPF.WebView2` provides a WPF `ContentControl` wrapper around Microsoft WebView2 plus an overlay host that can attach WPF windows above the browser HWND.

Use the CrissCross WPF XML namespace for WebView2 controls:

```xml
xmlns:web="https://github.com/reactivemarbles/CrissCross"
```

### WebView2Wpf

```xml
<web:WebView2Wpf
    x:Name="Browser"
    Source="https://example.com"
    ZoomFactor="1.0"
    AutoDispose="True"
    AllowExternalDrop="False" />
```

Key members:

| Member | Use |
| --- | --- |
| `Source` | Current browser URI. |
| `ZoomFactor` | Browser zoom. |
| `CreationProperties` | WebView2 creation settings. |
| `DefaultBackgroundColor` | Browser background color. |
| `DesignModeForegroundColor` | Design-time foreground color. |
| `AllowExternalDrop` | Enables external drag/drop into the browser. |
| `AutoDispose` | Disposes WebView2 when the control is disposed. |
| `CanGoBack`, `CanGoForward` | Browser history state. |
| `CoreWebView2` | Underlying WebView2 instance. |
| `Content` | WPF overlay content. |
| `EnsureCoreWebView2Async(CoreWebView2Environment, CoreWebView2ControllerOptions?)` | Initializes WebView2 with an environment and optional controller options. |
| `ExecuteScriptAsync(string)` | Runs JavaScript. |
| `NavigateToString(string)` | Loads HTML. |
| `GoBack()`, `GoForward()`, `Reload()`, `Stop()` | Browser commands. |
| `Dispose()` | Releases browser resources. |

Events:

```text
ContentLoading
CoreWebView2InitializationCompleted
NavigationStarting
NavigationCompleted
SourceChanged
WebMessageReceived
ZoomFactorChanged
```

### WindowHost

`WindowHost<TWindow>` hosts a transparent WPF `Window` as a child HWND. It is used internally by overlay scenarios and can be used directly when a WebView-hosted surface needs WPF controls above web content.

```csharp
using CrissCross.WPF;

var host = new WindowHost<OverlayWindow>("Overlay");
HostContainer.Children.Add(host);

var overlayWindow = host.Window;
var overlayHandle = host.WindowHandle;

// Later
host.Close();
```

| Member | Use |
| --- | --- |
| `Window` | The hosted `TWindow` instance. |
| `WindowHandle` | Safe handle for the hosted child HWND. |
| `Close()` | Closes the hosted window and releases the host handle. |

## Theming

CrissCross themes are intentionally shared in concept across WPF, Avalonia, and MAUI, but each platform applies resources through its native mechanism.

### WPF Theme Flow

1. Merge `ControlsDictionary`.
2. Merge `ThemesDictionary`.
3. Use `IThemeService` or `ApplicationThemeManager` to apply runtime theme changes.
4. Bind `ThemeSwitcher` to `ThemePreferenceState` when the user can select theme preference.

```xml
<ui:ThemeSwitcher CurrentState="{Binding ThemePreference}" />
```

### Avalonia Theme Flow

1. Add Avalonia `FluentTheme`.
2. Add `avares://CrissCross.Avalonia.UI/Themes/Index.axaml`.
3. Use `IThemeService`, `ApplicationThemeManager`, or `ThemeSwitcher` for runtime changes.

```xml
<ui:ThemeSwitcher CurrentState="{Binding ThemePreference}" />
```

### MAUI Theme Flow

1. Call `Resources.UseCrissCrossMauiUiResources()`, or merge `CrissCrossMauiUi`.
2. Bind `ThemeSwitcher` to `ThemePreferenceState`.
3. Apply theme choice through the app-level MAUI theme service or command in your view model.

```xml
<ui:ThemeSwitcher ThemeState="{Binding ThemePreference}" ChangeThemeCommand="{Binding ChangeThemeCommand}" />
```

## Gallery Projects

The gallery applications demonstrate the controls and shared feature patterns:

| Project | Demonstrates |
| --- | --- |
| `CrissCross.WPF.UI.Gallery` | WPF UI controls, themes, navigation, feature controls, view-model navigation, and examples. |
| `CrissCross.Avalonia.UI.Gallery` | Avalonia UI controls, themes, feature controls, and parity examples. |
| `CrissCross.Maui.UI.Gallery` | MAUI feature controls and shared state-model examples. |

Use galleries as executable examples. The README remains the authoritative API and usage documentation.

## Recommended App Structure

Use shared view models and state models in a platform-neutral project:

```text
src/
  Example.Core/
    ViewModels/
      ShellViewModel.cs
      OrdersViewModel.cs
    Navigation/
      NavigationRegistration.cs
  Example.Wpf/
    Views/
      ShellWindow.xaml
      OrdersPage.xaml
  Example.Avalonia/
    Views/
      ShellWindow.axaml
      OrdersPage.axaml
  Example.Maui/
    Views/
      OrdersPage.xaml
```

Register the same view models with the platform-specific view locator, then bind platform controls to the shared state properties.

## Common Recipes

### Navigate From A Command

```csharp
public sealed class HomeViewModel : RxObject, IUseHostedNavigation
{
    public ReactiveCommand<Unit, Unit> OpenDetails { get; }

    public HomeViewModel()
    {
        OpenDetails = ReactiveCommand.Create(() =>
        {
            this.NavigateToView(new NavigationKeyRequest<DetailsViewModel>());
        });
    }
}
```

### Bind Search, Filters, And Paging

```csharp
public sealed class SearchPageViewModel : RxObject
{
    private SearchQueryState search = new();
    private PaginationState paging = new(pageIndex: 0, pageSize: 20, totalItemCount: 0);

    public SearchQueryState Search
    {
        get => search;
        private set => this.RaiseAndSetIfChanged(ref search, value);
    }

    public DataFilterPanelState FilterPanel { get; } = new();

    public PaginationState Paging
    {
        get => paging;
        private set => this.RaiseAndSetIfChanged(ref paging, value);
    }

    public ReactiveCommand<string, Unit> SearchCommand { get; }
    public ReactiveCommand<PageRequest, Unit> PageCommand { get; }

    public SearchPageViewModel()
    {
        SearchCommand = ReactiveCommand.Create<string>(query =>
        {
            Search = new SearchQueryState(query, submittedText: query);
        });

        PageCommand = ReactiveCommand.Create<PageRequest>(request =>
        {
            Paging = new PaginationState(request.PageIndex, request.PageSize, Paging.TotalItemCount);
        });

    }
}
```

### Show Busy Work

```csharp
public sealed class ImportViewModel : RxObject
{
    private BusyOperation? importBusy;

    public BusyOperation? ImportBusy
    {
        get => importBusy;
        private set => this.RaiseAndSetIfChanged(ref importBusy, value);
    }

    public ReactiveCommand<Unit, Unit> ImportCommand { get; }

    public ImportViewModel()
    {
        ImportCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            ImportBusy = new BusyOperation("Importing records");

            try
            {
                await Task.Delay(1000);
            }
            finally
            {
                ImportBusy = null;
            }
        });

    }
}
```

### Validate A Form

```csharp
var summary = new ValidationSummaryState(new[]
{
    new ValidationMessage(
        fieldKey: "email",
        fieldDisplayName: "Email",
        message: "Email address is required.",
        severity: ValidationSeverity.Error)
});
```

### Use PropertyGridLite

```csharp
var descriptors = new[]
{
    new PropertyDescriptorModel(
        key: "host",
        displayName: "Host",
        category: "Connection",
        editorKind: PropertyEditorKind.Text,
        value: "localhost",
        originalValue: "localhost"),
    new PropertyDescriptorModel(
        key: "port",
        displayName: "Port",
        category: "Connection",
        editorKind: PropertyEditorKind.Number,
        value: 5432,
        originalValue: 5432)
};

var state = new PropertyGridState(descriptors);
```

WPF/Avalonia:

```xml
<ui:PropertyGridLite InspectorState="{Binding Inspector}" />
```

MAUI:

```xml
<ui:PropertyGridLite PropertyGridState="{Binding Inspector}" />
```

## Troubleshooting

| Symptom | Fix |
| --- | --- |
| WPF controls render partly dark and partly light | Ensure both `ControlsDictionary` and the selected `ThemesDictionary` are merged once at app startup. Avoid mixing external theme dictionaries after CrissCross resources unless intentional. |
| WPF resource cannot find a CrissCross style | Confirm `xmlns:ui="https://github.com/reactivemarbles/CrissCross.ui"` and `ControlsDictionary` are loaded before views are created. |
| Avalonia control templates are missing | Confirm `StyleInclude Source="avares://CrissCross.Avalonia.UI/Themes/Index.axaml"` is present after the base Avalonia theme. |
| MAUI static resource is missing | Call `Resources.UseCrissCrossMauiUiResources()` or merge `<styles:CrissCrossMauiUi />` in application resources. |
| Reactive command reports wrong parameter type | Bind commands with the parameter type the `ReactiveCommand<TInput,TOutput>` expects. Use `ReactiveCommand<Unit, Unit>` for parameterless buttons, or pass a strongly typed command parameter. |
| WebView2 fails at startup | Install the Microsoft Edge WebView2 runtime. For `NavigationWebView`, call `EnsureCoreWebView2Async()`; for `WebView2Wpf`, create a `CoreWebView2Environment` and pass it to `EnsureCoreWebView2Async(environment)` before script calls. |
| Navigation resolves no view | Register the view with the ReactiveUI view locator or the `NavigationRegistry`, and ensure the view implements the expected `IViewFor<TViewModel>` contract where required. |

## API Surface Checklist

Use this checklist when adding or verifying package parity:

| Package or project | Required feature families |
| --- | --- |
| `CrissCross`, `CrissCross.Reactive` | `RxObject`, `EventSignal`, lifecycle and routed-host interfaces, navigation requests/registrations/resolutions/journal, and shared state models. |
| `CrissCross.WPF`, `CrissCross.WPF.Reactive` | `NavigationWindow`, `NavigationWindow<TViewModel>`, `ViewModelRoutedViewHost`, `NavigationWebView`, `Make.SingleInstance`, `WindowHost<TWindow>`. |
| `CrissCross.Avalonia`, `CrissCross.Avalonia.Reactive` | `NavigationWindow`, `NavigationWindow<TViewModel>`, `NavigationUserControl`, `NavigationUserControl<TViewModel>`, `ViewModelRoutedViewHost`, `ReactiveTransitioningContentControl`. |
| `CrissCross.MAUI`, `CrissCross.MAUI.Reactive` | `NavigationShell`, routed-host setup, typed and runtime navigation. |
| `CrissCross.WinForms`, `CrissCross.WinForms.Reactive` | `NavigationForm`, `NavigationForm<TViewModel>`, `ViewModelRoutedViewHost`. |
| `CrissCross.WPF.UI`, `CrissCross.WPF.UI.Reactive` | WPF controls, themes, services, theme managers, app shell, feature controls, icons, color selection, and resource dictionaries. |
| `CrissCross.Avalonia.UI`, `CrissCross.Avalonia.UI.Reactive` | Avalonia controls, themes, services, theme managers, settings store, app shell, feature controls, and resources. |
| `CrissCross.Maui.UI` (project reference only) | MAUI feature controls, theme resources, and state-model bindings. |
| `CrissCross.WPF.Plot`, `CrissCross.WPF.Plot.Reactive` | Live chart, reactive plot binding, adapters, sources, plot UI models, and right properties view. |
| `CrissCross.WPF.WebView2`, `CrissCross.WPF.WebView2.Reactive` | `WebView2Wpf`, `WindowHost<TWindow>`, WebView2 events, browser commands, and overlay content. |
| `AICS.Windows.Controls.ColorPicker` | Picker bases, complete/portable/square pickers, color-state models, storage contracts, and RGB/HSL/HSV sliders. |


---

## Contributing

Issues and PRs are welcome. Please include platform, .NET version, and a minimal repro where applicable.

---

## License

MIT © ReactiveUI Association Incorporated
