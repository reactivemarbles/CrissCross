// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.UI.Gallery.ViewModels;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Interaction logic for SliderView.xaml.</summary>
public partial class SliderView : IViewFor<SliderViewModel>
{
    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(SliderViewModel),
        typeof(SliderView),
        new(null));

    /// <summary>Initializes a new instance of the <see cref="SliderView"/> class.</summary>
    public SliderView() => InitializeComponent();

    /// <summary>Gets the binding root view model.</summary>
    public SliderViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public SliderViewModel? ViewModel
    {
        get => (SliderViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (SliderViewModel?)value;
    }
}
