// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace CrissCross.MAUI.Test;

/// <summary>Hosts the Main view model using native MAUI bindings.</summary>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MainView : ContentPage, IViewFor<MainViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="MainView"/> class.</summary>
    public MainView()
    {
        InitializeComponent();
        ViewModel = AppLocator.Current.GetService<MainViewModel>();
    }

    /// <summary>Gets or sets the view model and native binding context.</summary>
    public MainViewModel? ViewModel
    {
        get => BindingContext as MainViewModel;
        set => BindingContext = value;
    }

    /// <inheritdoc />
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (MainViewModel?)value;
    }
}
