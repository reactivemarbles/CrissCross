// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.UI.Gallery.ViewModels;
using Splat;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Interaction logic for AllControlsView.xaml.</summary>
public partial class AllControlsView : IViewFor<AllControlsViewModel>
{
    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(AllControlsViewModel),
        typeof(AllControlsView),
        new(null));

    /// <summary>Initializes a new instance of the <see cref="AllControlsView"/> class.</summary>
    public AllControlsView()
    {
        InitializeComponent();
        ViewModel = AppLocator.Current.GetService<AllControlsViewModel>()!;
        DataContext = ViewModel;
    }

    /// <summary>Gets the binding root view model.</summary>
    public AllControlsViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public AllControlsViewModel? ViewModel
    {
        get => (AllControlsViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (AllControlsViewModel?)value;
    }
}
