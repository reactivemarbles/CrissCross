// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Maui.UI;
using ReactiveUI;
using ReactiveUI.Builder;
using Splat;

namespace CrissCross.MAUI.Test;

/// <summary>App member.</summary>
/// <seealso cref="Application" />
public partial class App : Application
{
    /// <summary>Initializes a new instance of the <see cref="App"/> class.</summary>
    /// <remarks>
    /// To be added.
    /// </remarks>
    public App()
    {
        InitializeComponent();
        _ = Resources.UseCrissCrossMauiUiResources();
        _ = RxAppBuilder.CreateReactiveUIBuilder().WithMaui().BuildApp();
        AppLocator.CurrentMutable.RegisterConstant<MainViewModel>(new());
        AppLocator.CurrentMutable.Register<IViewFor<MainViewModel>>(() => new MainView());

        AppLocator.CurrentMutable.RegisterConstant<FirstViewModel>(new());
        AppLocator.CurrentMutable.Register<IViewFor<FirstViewModel>>(() => new FirstView());
        AppLocator.CurrentMutable.RegisterConstant<ControlsGalleryViewModel>(new());
        AppLocator.CurrentMutable.Register<IViewFor<ControlsGalleryViewModel>>(() => new ControlsGalleryView());
    }

    /// <inheritdoc/>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        _ = activationState;
        return new Window(new AppShell());
    }
}
