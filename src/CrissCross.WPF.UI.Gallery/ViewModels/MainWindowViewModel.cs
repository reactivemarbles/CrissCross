// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.UI.Controls;
using CrissCross.WPF.UI.Gallery.Views;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Splat;

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>MainWindowViewModel member.</summary>
/// <seealso cref="CrissCross.RxObject" />
public partial class MainWindowViewModel : RxObject
{
    /// <summary>Provides access to persisted window tracking.</summary>
    private readonly Tracker? _tracker;

    /// <summary>Provides the application title.</summary>
    [Reactive]
    private string _applicationTitle = "CrissCross UI Gallery";

    /// <summary>Initializes a new instance of the <see cref="MainWindowViewModel"/> class.</summary>
    public MainWindowViewModel()
    {
        _tracker = new();
        SetupTracker();
        NavigationModels = [];
        NavigationModels.AddRange([
            CreateNavigationModel(null, null, SymbolRegular.LineHorizontal320, isExpander: true),
            CreateNavigationModel(typeof(MainViewModel), "Main", SymbolRegular.Home20, isSelected: true),
            CreateNavigationModel(typeof(AllControlsViewModel), "All Controls", SymbolRegular.ControlButton20),
            CreateNavigationModel(typeof(InputControlsViewModel), "Input", SymbolRegular.Keyboard20),
            CreateNavigationModel(typeof(DateTimeControlsViewModel), "Date / Time", SymbolRegular.CalendarLtr20),
            CreateNavigationModel(typeof(MediaControlsViewModel), "Media", SymbolRegular.Image20),
            CreateNavigationModel(typeof(ColorControlsViewModel), "Color", SymbolRegular.Color24),
            CreateNavigationModel(typeof(IndicatorsViewModel), "Indicators", SymbolRegular.Gauge20),
            CreateNavigationModel(typeof(NavigationControlsViewModel), "Navigation", SymbolRegular.Navigation20),
            CreateNavigationModel(typeof(ContainerControlsViewModel), "Containers", SymbolRegular.AppFolder20),
            CreateNavigationModel(
                typeof(FeaturePlaygroundViewModel),
                "Feature Playground",
                SymbolRegular.ControlButton20),
            CreateNavigationModel(typeof(TreeViewViewModel), "TreeViews", SymbolRegular.DataTreemap20),]);

        RegisterBasicViews();
        RegisterCategoryViews();

        AppLocator.CurrentMutable.SetupComplete();
    }

    /// <summary>Gets the navigation models.</summary>
    /// <value>
    /// The navigation models.
    /// </value>
    public List<NavigationModel> NavigationModels { get; }

    /// <summary>Registers the individual control demo views.</summary>
    private static void RegisterBasicViews()
    {
        Register<MainViewModel, MainView>();
        Register<AllControlsViewModel, AllControlsView>();
        Register<AppBarButtonViewModel, AppBarButtonView>();
        Register<BBCodeBlockViewModel, BBCodeBlockView>();
        Register<ButtonsViewModel, ButtonsView>();
        Register<CheckBoxViewModel, CheckBoxView>();
        Register<ComboBoxViewModel, ComboBoxView>();
        Register<DatePickerViewModel, DatePickerView>();
        Register<ImageViewModel, ImageView>();
        Register<NumericPushButtonViewModel, NumericPushButtonView>();
        Register<PasswordBoxViewModel, PasswordBoxView>();
        Register<RadioButtonViewModel, RadioButtonView>();
        Register<SliderViewModel, SliderView>();
        Register<TextBlockViewModel, TextBlockView>();
        Register<TextBoxViewModel, TextBoxView>();
        Register<ToggleButtonViewModel, ToggleButtonView>();
        Register<ColorPickersViewModel, ColorPickersView>();
    }

    /// <summary>Registers the grouped and feature demo views.</summary>
    private static void RegisterCategoryViews()
    {
        Register<InputControlsViewModel, InputControlsView>();
        Register<DateTimeControlsViewModel, DateTimeControlsView>();
        Register<MediaControlsViewModel, MediaControlsView>();
        Register<IndicatorsViewModel, IndicatorsView>();
        Register<NavigationControlsViewModel, NavigationControlsView>();
        Register<ContainerControlsViewModel, ContainerControlsView>();
        Register<ColorControlsViewModel, ColorControlsView>();
        Register<FeaturePlaygroundViewModel, FeaturePlaygroundView>();
        Register<TreeViewViewModel, TreeViewView>();
    }

    /// <summary>Registers one view model and its corresponding view.</summary>
    /// <typeparam name="TViewModel">The registered view model type.</typeparam>
    /// <typeparam name="TView">The registered view type.</typeparam>
    private static void Register<TViewModel, TView>()
        where TViewModel : class, IRxObject, new()
        where TView : class, IViewFor<TViewModel>, new() =>
        AppLocator
            .CurrentMutable.RegisterLazySingletonAnd(static () => new TViewModel())
            .Register<IViewFor<TViewModel>>(static () => new TView());

    /// <summary>Creates a configured navigation model.</summary>
    /// <param name="viewModelType">The destination view model type.</param>
    /// <param name="name">The navigation label.</param>
    /// <param name="symbol">The navigation icon.</param>
    /// <param name="isSelected">Whether the item is initially selected.</param>
    /// <param name="isExpander">Whether the item represents an expander.</param>
    /// <returns>The configured navigation model.</returns>
    private NavigationModel CreateNavigationModel(
        Type? viewModelType,
        string? name,
        SymbolRegular symbol,
        bool isSelected = false,
        bool isExpander = false) =>
        new(viewModelType, NavigationModels, MainWindow.Navigation)
        {
            Name = name ?? string.Empty,
            Icon = new SymbolIcon(symbol),
            IsSelected = isSelected,
            IsExpander = isExpander,
        };

    /// <summary>Configures persisted main window tracking.</summary>
    private void SetupTracker()
    {
        AppLocator.CurrentMutable.RegisterConstant(_tracker);
        _tracker
            ?.Configure(new TrackingRequest<MainWindow>())
            .Id(
                w => w.Name,
                $"[Width={SystemParameters.VirtualScreenWidth},Height{SystemParameters.VirtualScreenHeight}]")
            .Property(w => w.Height)
            .Property(w => w.Width)
            .Property(w => w.Left)
            .Property(w => w.Top)
            .Property(w => w.WindowState)
            .PersistOn(w => nameof(w.Closing))
            .StopTrackingOn(w => nameof(w.Closing));
    }
}
