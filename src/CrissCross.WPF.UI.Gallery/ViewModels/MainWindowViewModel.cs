// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.UI.Controls;
using CrissCross.WPF.UI.Gallery.Views;
using ReactiveUI;
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
        ]);

        // Register ViewModels and Views
        Locator.CurrentMutable.RegisterLazySingletonAnd(static () => new MainViewModel()).Register<IViewFor<MainViewModel>>(static () => new MainView());
        Locator.CurrentMutable.RegisterLazySingletonAnd(static () => new AllControlsViewModel()).Register<IViewFor<AllControlsViewModel>>(static () => new AllControlsView());
        Locator.CurrentMutable.RegisterLazySingletonAnd(static () => new ButtonsViewModel()).Register<IViewFor<ButtonsViewModel>>(static () => new ButtonsView());
        Locator.CurrentMutable.RegisterLazySingletonAnd(static () => new CheckBoxViewModel()).Register<IViewFor<CheckBoxViewModel>>(static () => new CheckBoxView());
        Locator.CurrentMutable.RegisterLazySingletonAnd(static () => new ComboBoxViewModel()).Register<IViewFor<ComboBoxViewModel>>(static () => new ComboBoxView());
        Locator.CurrentMutable.RegisterLazySingletonAnd(static () => new DatePickerViewModel()).Register<IViewFor<DatePickerViewModel>>(static () => new DatePickerView());
        Locator.CurrentMutable.RegisterLazySingletonAnd(static () => new ImageViewModel()).Register<IViewFor<ImageViewModel>>(static () => new ImageView());
        Locator.CurrentMutable.RegisterLazySingletonAnd(static () => new NumericPushButtonViewModel()).Register<IViewFor<NumericPushButtonViewModel>>(static () => new NumericPushButtonView());
        Locator.CurrentMutable.RegisterLazySingletonAnd(static () => new PasswordBoxViewModel()).Register<IViewFor<PasswordBoxViewModel>>(static () => new PasswordBoxView());
        Locator.CurrentMutable.RegisterLazySingletonAnd(static () => new RadioButtonViewModel()).Register<IViewFor<RadioButtonViewModel>>(static () => new RadioButtonView());
        Locator.CurrentMutable.RegisterLazySingletonAnd(static () => new SliderViewModel()).Register<IViewFor<SliderViewModel>>(static () => new SliderView());
        Locator.CurrentMutable.RegisterLazySingletonAnd(static () => new TextBlockViewModel()).Register<IViewFor<TextBlockViewModel>>(static () => new TextBlockView());
        Locator.CurrentMutable.RegisterLazySingletonAnd(static () => new TextBoxViewModel()).Register<IViewFor<TextBoxViewModel>>(static () => new TextBoxView());
        Locator.CurrentMutable.RegisterLazySingletonAnd(static () => new ToggleButtonViewModel()).Register<IViewFor<ToggleButtonViewModel>>(static () => new ToggleButtonView());
        Locator.CurrentMutable.RegisterLazySingletonAnd(static () => new ColorPickersViewModel()).Register<IViewFor<ColorPickersViewModel>>(static () => new ColorPickersView());

        Locator.CurrentMutable.SetupComplete();
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
        Locator.CurrentMutable.RegisterConstant(_tracker);
        _tracker?.Configure<MainWindow>()
            .Id(w => w.Name, $"[Width={SystemParameters.VirtualScreenWidth},Height{SystemParameters.VirtualScreenHeight}]")
            .Properties(w => new { w.Height, w.Width, w.Left, w.Top, w.WindowState })
            .PersistOn(w => nameof(w.Closing))
            .StopTrackingOn(w => nameof(w.Closing));
    }
}
