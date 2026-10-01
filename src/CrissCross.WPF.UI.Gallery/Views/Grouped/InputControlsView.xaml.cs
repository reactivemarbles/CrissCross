// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.UI.Gallery.ViewModels;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Input controls grouped page.</summary>
public partial class InputControlsView : IViewFor<InputControlsViewModel>
{
    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(InputControlsViewModel),
        typeof(InputControlsView),
        new(null));

    /// <summary>Initializes a new instance of the <see cref="InputControlsView"/> class.</summary>
    public InputControlsView() => InitializeComponent();

    /// <summary>Gets the binding root view model.</summary>
    public InputControlsViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public InputControlsViewModel? ViewModel
    {
        get => (InputControlsViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (InputControlsViewModel?)value;
    }
}
