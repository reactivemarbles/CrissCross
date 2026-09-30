// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.UI.Gallery.ViewModels;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Color controls grouped page.</summary>
public partial class ColorControlsView : IViewFor<ColorControlsViewModel>
{
    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(ColorControlsViewModel),
        typeof(ColorControlsView),
        new(null));

    /// <summary>Initializes a new instance of the <see cref="ColorControlsView"/> class.</summary>
    public ColorControlsView() => InitializeComponent();

    /// <summary>Gets the binding root view model.</summary>
    public ColorControlsViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public ColorControlsViewModel? ViewModel
    {
        get => (ColorControlsViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (ColorControlsViewModel?)value;
    }
}
