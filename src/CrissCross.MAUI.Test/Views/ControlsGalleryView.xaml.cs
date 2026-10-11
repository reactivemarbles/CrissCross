// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace CrissCross.MAUI.Test;

/// <summary>Hosts the ControlsGallery view model using native MAUI bindings.</summary>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ControlsGalleryView : ContentPage, IViewFor<ControlsGalleryViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="ControlsGalleryView"/> class.</summary>
    public ControlsGalleryView()
    {
        InitializeComponent();
        ViewModel = AppLocator.Current.GetService<ControlsGalleryViewModel>();
    }

    /// <summary>Gets or sets the view model and native binding context.</summary>
    public ControlsGalleryViewModel? ViewModel
    {
        get => BindingContext as ControlsGalleryViewModel;
        set => BindingContext = value;
    }

    /// <inheritdoc />
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (ControlsGalleryViewModel?)value;
    }
}
