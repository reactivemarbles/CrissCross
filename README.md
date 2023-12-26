# CrissCross
A Navigation Framework for ReactiveUI based projects

![Alt](https://repobeats.axiom.co/api/embed/3ffd196858ee7468fa2e28225b0edf635b6c740b.svg "Repobeats analytics image")

![CrissCross](https://github.com/ChrisPulman/CrissCross/blob/master/Images/CrissCross.png)

[![CrissCross CI-Build](https://github.com/ChrisPulman/CrissCross/actions/workflows/BuildOnly.yml/badge.svg)](https://github.com/ChrisPulman/CrissCross/actions/workflows/BuildOnly.yml) 

## What is CrissCross?

CrissCross is a navigation framework for ReactiveUI based projects. It is designed to be used with ReactiveUI, but could be adapted to be used with any MVVM framework.

## Why CrissCross?

CrissCross is designed to be a simple, lightweight, and easy to use navigation framework. It is designed to be used with ReactiveUI.   

## How do I use CrissCross?

### Step 1: Install CrissCross

CrissCross is available on NuGet. You can install it using the NuGet Package Manager:

![Nuget](https://img.shields.io/nuget/v/CrissCross) ![Nuget](https://img.shields.io/nuget/dt/CrissCross)

    Install-Package CrissCross

or ![Nuget](https://img.shields.io/nuget/v/CrissCross.WPF) ![Nuget](https://img.shields.io/nuget/dt/CrissCross.WPF)

    Install-Package CrissCross.WPF

or ![Nuget](https://img.shields.io/nuget/v/CrissCross.XamForms) ![Nuget](https://img.shields.io/nuget/dt/CrissCross.XamForms)

    Install-Package CrissCross.XamForms

or ![Nuget](https://img.shields.io/nuget/v/CrissCross.MAUI) ![Nuget](https://img.shields.io/nuget/dt/CrissCross.MAUI)

    Install-Package CrissCross.MAUI

or ![Nuget](https://img.shields.io/nuget/v/CrissCross.Avalonia) ![Nuget](https://img.shields.io/nuget/dt/CrissCross.Avalonia)


    Install-Package CrissCross.Avalonia

or ![Nuget](https://img.shields.io/nuget/v/CrissCross.WinForms) ![Nuget](https://img.shields.io/nuget/dt/CrissCross.WinForms)

    Install-Package CrissCross.WinForms


### Step 2: Create a ViewModel

Create a ViewModel that inherits from `RxObject`. This is the ViewModel that will be used for the MainWindow.

```c#
    public class MainWindowViewModel : RxObject
    {
        public MainWindowViewModel()
        {
            this.BuildComplete(() =>
            {
                // Do something when the IOC Container is built
            });

            // Setup the IOC Container
            Locator.CurrentMutable.RegisterConstant<MainViewModel>(new());
            Locator.CurrentMutable.Register<IViewFor<MainViewModel>>(() => new MainView());

            Locator.CurrentMutable.RegisterConstant<FirstViewModel>(new());
            Locator.CurrentMutable.Register<IViewFor<FirstViewModel>>(() => new FirstView());
            
            // Notify the application that the IOC Container that it is complete and ready to use.
            Locator.CurrentMutable.SetupComplete();
        }
    }
```

### Step 3: Create a View

Create a View that inherits from `NavigationWindow`. This is the View that will be used for the MainWindow.
add xmlns:rxNav="https://github.com/ChrisPulman/CrissCross" to the Window inherits in XAML.
Change Window to rxNav:NavigationWindow in XAML.
Add x:TypeArguments="local:MainWindowViewModel"

```c#
    public partial class MainWindow : NavigationWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            // Remember to set x:Name in XAML to "mainWindow"
            InitializeComponent();
            
            this.WhenActivated(disposables =>
            {
                // Do something when the View is activated

                // Configure the Navigation for the MainWindow
                NavBack.Command = ReactiveCommand.Create(() => this.NavigateBack(), CanNavigateBack).DisposeWith(d);
                
                // Navigate to the MainViewModel
                this.NavigateToView<MainViewModel>();
            });
        }
    }
```

### Step 4: Create a ViewModel

Create a ViewModel that inherits from `RxObject`. This is the ViewModel that will be used for the MainView.

```c#
    public class MainViewModel : RxObject
    {
        public MainViewModel()
        {
            this.BuildComplete(() =>
            {
                // Do something when the IOC Container is built

                // Configure the Navigation for the MainViewModel using, you will pass the name of the Navigation Host Window that you want to navigate with.
                this.NavigateBack("mainWindow")
                this.CanNavigateBack("mainWindow")
                this.NavigateToView<FirstViewModel>("mainWindow")
            });
        }
    }
```

### Step 5: Create a View

Create a View that inherits from `ReactiveUserControl`. This is the View that will be used for the MainView.

```c#
    public partial class MainView : ReactiveUserControl<MainViewModel>
    {
        public MainView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // Do something when the View is activated
            });
        }
    }
```

### Step 6: For WPF Applications Configure Single Instance Application if required

If you want to prevent multiple instances of the application from running at the same time, you can use the `Make.SingleInstance` method.

```c#
    protected override void OnStartup(StartupEventArgs e)
    {
        // This will prevent multiple instances of the application from running at the same time.
        Make.SingleInstance("MyUniqueAppName ddd81fc8-9107-4e33-b848-cac4c3ec3d2a");
        base.OnStartup(e);
    }
```

### Step 7: Run the application

Run the application and you should see the MainView.

## Avalonia

### Step 1: Install CrissCross


CrissCross is available on NuGet. You can install it using the NuGet Package Manager:

    Install-Package CrissCross.Avalonia

### Step 2: Create a ViewModel

Create a ViewModel that inherits from `RxObject`. This is the ViewModel that will be used for the MainWindow.

```c#
    public class MainWindowViewModel : RxObject
    {
        public MainWindowViewModel()
        {
            this.BuildComplete(() =>
            {
                // Do something when the IOC Container is built
            });

            // Setup the IOC Container
            Locator.CurrentMutable.RegisterConstant<MainViewModel>(new());
            Locator.CurrentMutable.Register<IViewFor<MainViewModel>>(() => new MainView());

            Locator.CurrentMutable.RegisterConstant<FirstViewModel>(new());
            Locator.CurrentMutable.Register<IViewFor<FirstViewModel>>(() => new FirstView());
            
            // Notify the application that the IOC Container that it is complete and ready to use.
            Locator.CurrentMutable.SetupComplete();
        }
    }
```

### Step 3: Create a NavigationView


Create a View that inherits from `NavigationWindow` OR `NavigationUserControl`. This is the View that will be used for the MainWindow.
add xmlns:rxNav="https://github.com/ChrisPulman/CrissCross"
Change Window to rxNav:NavigationWindow in XAML.
OR Change UserControl to rxNav:NavigationUserControl in XAML.

As Avalonia has two modes of operation you will need to select the correct mode for your application.

```c#
    public partial class MainWindow : NavigationWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            // Remember to set x:Name in XAML to "mainWindow"
            InitializeComponent();
            
            this.WhenActivated(disposables =>
            {
                // Do something when the View is activated

                // Configure the Navigation for the MainWindow
                NavBack.Command = ReactiveCommand.Create(() => this.NavigateBack(), CanNavigateBack).DisposeWith(d);
                
                // Navigate to the MainViewModel
                this.NavigateToView<MainViewModel>();
            });
        }
    }
```

### Step 4: Create a ViewModel

Create a ViewModel that inherits from `RxObject`. This is the ViewModel that will be used for the MainView.

```c#
    public class MainViewModel : RxObject
    {
        public MainViewModel()
        {
            this.BuildComplete(() =>
            {
                // Do something when the IOC Container is built

                // Configure the Navigation for the MainViewModel using, you will pass the name of the Navigation Host Window that you want to navigate with.
                this.NavigateBack("mainWindow")
                this.CanNavigateBack("mainWindow")
                this.NavigateToView<FirstViewModel>("mainWindow")
            });
        }
    }
```

### Step 5: Create a View

Create a View that inherits from `ReactiveUserControl`. This is the View that will be used for the MainView.

```c#
    public partial class MainView : ReactiveUserControl<MainViewModel>
    {
        public MainView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // Do something when the View is activated
            });
        }
    }
```
