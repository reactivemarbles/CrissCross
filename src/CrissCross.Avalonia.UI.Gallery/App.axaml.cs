// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CrissCross.Avalonia.UI.Gallery.ViewModels;
using CrissCross.Avalonia.UI.Gallery.Views;
using CrissCross.Avalonia.UI.Gallery.Views.Pages;
using ReactiveUI;
using Splat;

namespace CrissCross.Avalonia.UI.Gallery;

/// <summary>Gallery application demonstrating all CrissCross.Avalonia.UI controls.</summary>
public class App : Application
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        // Register ViewModels as singletons
        AppLocator.CurrentMutable.RegisterConstant<HomePageViewModel>(new());
        AppLocator.CurrentMutable.RegisterConstant<ButtonsPageViewModel>(new());
        AppLocator.CurrentMutable.RegisterConstant<InputPageViewModel>(new());
        AppLocator.CurrentMutable.RegisterConstant<ProgressPageViewModel>(new());
        AppLocator.CurrentMutable.RegisterConstant<CheckBoxPageViewModel>(new());
        AppLocator.CurrentMutable.RegisterConstant<RadioButtonPageViewModel>(new());
        AppLocator.CurrentMutable.RegisterConstant<ComboBoxPageViewModel>(new());
        AppLocator.CurrentMutable.RegisterConstant<SliderPageViewModel>(new());
        AppLocator.CurrentMutable.RegisterConstant<DatePickerPageViewModel>(new());
        AppLocator.CurrentMutable.RegisterConstant<ColorPickerPageViewModel>(new());
        AppLocator.CurrentMutable.RegisterConstant<BBCodeBlockPageViewModel>(new());
        AppLocator.CurrentMutable.RegisterConstant<FeaturePlaygroundPageViewModel>(new());
        AppLocator.CurrentMutable.RegisterConstant<WorkflowPageViewModel>(new());
        AppLocator.CurrentMutable.RegisterConstant<ControlCatalogPageViewModel>(new());
        AppLocator.CurrentMutable.RegisterConstant<MainViewModel>(new());

        // Register Views for ViewModels - these registrations are used by the default ReactiveUI ViewLocator
        // The ViewLocator will resolve IViewFor<T> to find the appropriate view for a viewmodel
        AppLocator.CurrentMutable.Register<IViewFor<HomePageViewModel>>(static () => new HomePageView());
        AppLocator.CurrentMutable.Register<IViewFor<ButtonsPageViewModel>>(static () => new ButtonsPageView());
        AppLocator.CurrentMutable.Register<IViewFor<InputPageViewModel>>(static () => new InputPageView());
        AppLocator.CurrentMutable.Register<IViewFor<ProgressPageViewModel>>(static () => new ProgressPageView());
        AppLocator.CurrentMutable.Register<IViewFor<CheckBoxPageViewModel>>(static () => new CheckBoxPageView());
        AppLocator.CurrentMutable.Register<IViewFor<RadioButtonPageViewModel>>(static () => new RadioButtonPageView());
        AppLocator.CurrentMutable.Register<IViewFor<ComboBoxPageViewModel>>(static () => new ComboBoxPageView());
        AppLocator.CurrentMutable.Register<IViewFor<SliderPageViewModel>>(static () => new SliderPageView());
        AppLocator.CurrentMutable.Register<IViewFor<DatePickerPageViewModel>>(static () => new DatePickerPageView());
        AppLocator.CurrentMutable.Register<IViewFor<ColorPickerPageViewModel>>(static () => new ColorPickerPageView());
        AppLocator.CurrentMutable.Register<IViewFor<BBCodeBlockPageViewModel>>(static () => new BBCodeBlockPageView());
        AppLocator.CurrentMutable.Register<IViewFor<FeaturePlaygroundPageViewModel>>(static () =>
            new FeaturePlaygroundPageView());
        AppLocator.CurrentMutable.Register<IViewFor<WorkflowPageViewModel>>(static () => new WorkflowPageView());
        AppLocator.CurrentMutable.Register<IViewFor<ControlCatalogPageViewModel>>(static () => new ControlCatalogPageView());

        // NOTE: SetupComplete is called in OnFrameworkInitializationCompleted() to ensure
        // the MainWindow and navigation host are created before BuildComplete callbacks fire
    }

    /// <inheritdoc/>
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        // Signal that IOC setup is complete AFTER the MainWindow is created
        // This ensures navigation hosts are registered before BuildComplete callbacks fire
        AppLocator.CurrentMutable.SetupComplete();

        base.OnFrameworkInitializationCompleted();
    }
}
