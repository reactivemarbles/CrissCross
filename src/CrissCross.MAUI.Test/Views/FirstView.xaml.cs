// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace CrissCross.MAUI.Test;

/// <summary>Hosts the First view model using native MAUI bindings.</summary>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class FirstView : ContentPage, IViewFor<FirstViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="FirstView"/> class.</summary>
    public FirstView()
    {
        InitializeComponent();
        ViewModel = AppLocator.Current.GetService<FirstViewModel>();
    }

    /// <summary>Gets or sets the view model and native binding context.</summary>
    public FirstViewModel? ViewModel
    {
        get => BindingContext as FirstViewModel;
        set => BindingContext = value;
    }

    /// <inheritdoc />
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (FirstViewModel?)value;
    }
}
