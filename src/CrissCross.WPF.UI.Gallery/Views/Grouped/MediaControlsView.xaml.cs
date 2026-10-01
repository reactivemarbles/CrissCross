// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.UI.Gallery.ViewModels;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Media grouped page.</summary>
public partial class MediaControlsView : IViewFor<MediaControlsViewModel>
{
    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(MediaControlsViewModel),
        typeof(MediaControlsView),
        new(null));

    /// <summary>Initializes a new instance of the <see cref="MediaControlsView"/> class.</summary>
    public MediaControlsView() => InitializeComponent();

    /// <summary>Gets the binding root view model.</summary>
    public MediaControlsViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public MediaControlsViewModel? ViewModel
    {
        get => (MediaControlsViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (MediaControlsViewModel?)value;
    }
}
