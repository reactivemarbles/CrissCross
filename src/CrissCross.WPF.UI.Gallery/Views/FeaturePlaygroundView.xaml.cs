// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.UI.Gallery.ViewModels;
using Splat;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Interaction logic for the reactive feature playground.</summary>
public partial class FeaturePlaygroundView : IViewFor<FeaturePlaygroundViewModel>
{
    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(FeaturePlaygroundViewModel),
        typeof(FeaturePlaygroundView),
        new(null));

    /// <summary>Initializes a new instance of the <see cref="FeaturePlaygroundView"/> class.</summary>
    public FeaturePlaygroundView()
    {
        InitializeComponent();
        ViewModel = AppLocator.Current.GetService<FeaturePlaygroundViewModel>()!;
        DataContext = ViewModel;
    }

    /// <summary>Gets the binding root view model.</summary>
    public FeaturePlaygroundViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public FeaturePlaygroundViewModel? ViewModel
    {
        get => (FeaturePlaygroundViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (FeaturePlaygroundViewModel?)value;
    }
}
