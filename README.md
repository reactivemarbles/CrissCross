# CrissCross
A Navigation Framework for ReactiveUI based projects

![CrissCross](https://github.com/ChrisPulman/CrissCross/blob/master/Images/CrissCross.png)

## What is CrissCross?

CrissCross is a navigation framework for ReactiveUI based projects. It is designed to be used with ReactiveUI, but could be adapted to be used with any MVVM framework.

## Why CrissCross?

CrissCross is designed to be a simple, lightweight, and easy to use navigation framework. It is designed to be used with ReactiveUI.   

## How do I use CrissCross?

### Step 1: Install CrissCross

CrissCross is available on NuGet. You can install it using the NuGet Package Manager:

    Install-Package CrissCross.WPF

or

    Install-Package CrissCross.XamForms

or

    Install-Package CrissCross.MAUI


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
