// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.UI.Controls;
using CrissCross.WPF.UI.Gallery.Views;
using ReactiveUI;
using ReactiveUI.Builder;
using ReactiveUI.SourceGenerators;
using Splat;

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>
/// MainWindowViewModel.
/// </summary>
/// <seealso cref="CrissCross.RxObject" />
public partial class MainWindowViewModel : RxObject
{
    private readonly Tracker? _tracker;

    [Reactive]
    private string _applicationTitle = "CrissCross UI Gallery";

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        _tracker = new();
        SetupTracker();
        NavigationModels = [];
        NavigationModels.AddRange(
        [
            new NavigationModel(null, NavigationModels, MainWindow.Navigation) { IsExpander = true, Icon = new SymbolIcon(SymbolRegular.LineHorizontal320) },
            new NavigationModel(typeof(MainViewModel), NavigationModels, MainWindow.Navigation) { Name = "Main", Icon = new SymbolIcon(SymbolRegular.Home20), IsSelected = true },
            new NavigationModel(typeof(AllControlsViewModel), NavigationModels, MainWindow.Navigation) { Name = "All Controls", Icon = new SymbolIcon(SymbolRegular.ControlButton20) },
            new NavigationModel(typeof(InputControlsViewModel), NavigationModels, MainWindow.Navigation) { Name = "Input", Icon = new SymbolIcon(SymbolRegular.Keyboard20) },
            new NavigationModel(typeof(DateTimeControlsViewModel), NavigationModels, MainWindow.Navigation) { Name = "Date / Time", Icon = new SymbolIcon(SymbolRegular.CalendarLtr20) },
            new NavigationModel(typeof(MediaControlsViewModel), NavigationModels, MainWindow.Navigation) { Name = "Media", Icon = new SymbolIcon(SymbolRegular.Image20) },
            new NavigationModel(typeof(ColorControlsViewModel), NavigationModels, MainWindow.Navigation) { Name = "Color", Icon = new SymbolIcon(SymbolRegular.Color24) },
            new NavigationModel(typeof(IndicatorsViewModel), NavigationModels, MainWindow.Navigation) { Name = "Indicators", Icon = new SymbolIcon(SymbolRegular.Gauge20) },
            new NavigationModel(typeof(NavigationControlsViewModel), NavigationModels, MainWindow.Navigation) { Name = "Navigation", Icon = new SymbolIcon(SymbolRegular.Navigation20) },
            new NavigationModel(typeof(ContainerControlsViewModel), NavigationModels, MainWindow.Navigation) { Name = "Containers", Icon = new SymbolIcon(SymbolRegular.AppFolder20) },
            new NavigationModel(typeof(TreeViewViewModel), NavigationModels, MainWindow.Navigation) { Name = "TreeViews", Icon = new SymbolIcon(SymbolRegular.DataTreemap20), },
        ]);

        // Register ViewModels and Views (basic demos)
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new MainViewModel()).Register<IViewFor<MainViewModel>>(static () => new MainView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new AllControlsViewModel()).Register<IViewFor<AllControlsViewModel>>(static () => new AllControlsView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new ButtonsViewModel()).Register<IViewFor<ButtonsViewModel>>(static () => new ButtonsView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new CheckBoxViewModel()).Register<IViewFor<CheckBoxViewModel>>(static () => new CheckBoxView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new ComboBoxViewModel()).Register<IViewFor<ComboBoxViewModel>>(static () => new ComboBoxView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new DatePickerViewModel()).Register<IViewFor<DatePickerViewModel>>(static () => new DatePickerView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new ImageViewModel()).Register<IViewFor<ImageViewModel>>(static () => new ImageView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new NumericPushButtonViewModel()).Register<IViewFor<NumericPushButtonViewModel>>(static () => new NumericPushButtonView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new PasswordBoxViewModel()).Register<IViewFor<PasswordBoxViewModel>>(static () => new PasswordBoxView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new RadioButtonViewModel()).Register<IViewFor<RadioButtonViewModel>>(static () => new RadioButtonView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new SliderViewModel()).Register<IViewFor<SliderViewModel>>(static () => new SliderView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new TextBlockViewModel()).Register<IViewFor<TextBlockViewModel>>(static () => new TextBlockView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new TextBoxViewModel()).Register<IViewFor<TextBoxViewModel>>(static () => new TextBoxView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new ToggleButtonViewModel()).Register<IViewFor<ToggleButtonViewModel>>(static () => new ToggleButtonView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new ColorPickersViewModel()).Register<IViewFor<ColorPickersViewModel>>(static () => new ColorPickersView());

        // Group/category views
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new InputControlsViewModel()).Register<IViewFor<InputControlsViewModel>>(static () => new InputControlsView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new DateTimeControlsViewModel()).Register<IViewFor<DateTimeControlsViewModel>>(static () => new DateTimeControlsView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new MediaControlsViewModel()).Register<IViewFor<MediaControlsViewModel>>(static () => new MediaControlsView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new IndicatorsViewModel()).Register<IViewFor<IndicatorsViewModel>>(static () => new IndicatorsView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new NavigationControlsViewModel()).Register<IViewFor<NavigationControlsViewModel>>(static () => new NavigationControlsView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new ContainerControlsViewModel()).Register<IViewFor<ContainerControlsViewModel>>(static () => new ContainerControlsView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new ColorControlsViewModel()).Register<IViewFor<ColorControlsViewModel>>(static () => new ColorControlsView());
        AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new TreeViewViewModel()).Register<IViewFor<TreeViewViewModel>>(static () => new TreeViewView());

        AppLocator.CurrentMutable.SetupComplete();
    }

    /// <summary>
    /// Gets the navigation models.
    /// </summary>
    /// <value>
    /// The navigation models.
    /// </value>
    public List<NavigationModel> NavigationModels { get; }

    private void SetupTracker()
    {
        AppLocator.CurrentMutable.RegisterConstant(_tracker);
        _tracker?.Configure<MainWindow>()
            .Id(w => w.Name, $"[Width={SystemParameters.VirtualScreenWidth},Height{SystemParameters.VirtualScreenHeight}]")
            .Properties(w => new { w.Height, w.Width, w.Left, w.Top, w.WindowState })
            .PersistOn(w => nameof(w.Closing))
            .StopTrackingOn(w => nameof(w.Closing));
    }
}
