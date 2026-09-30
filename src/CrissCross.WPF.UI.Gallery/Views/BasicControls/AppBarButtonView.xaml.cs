// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.UI.Gallery.ViewModels;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Interaction logic for AppBarButtonView.xaml.</summary>
public partial class AppBarButtonView : IViewFor<AppBarButtonViewModel>
{
    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(AppBarButtonViewModel),
        typeof(AppBarButtonView),
        new(null));

    /// <summary>Initializes a new instance of the <see cref="AppBarButtonView"/> class.</summary>
    public AppBarButtonView() => InitializeComponent();

    /// <summary>Gets the binding root view model.</summary>
    public AppBarButtonViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public AppBarButtonViewModel? ViewModel
    {
        get => (AppBarButtonViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (AppBarButtonViewModel?)value;
    }
}
