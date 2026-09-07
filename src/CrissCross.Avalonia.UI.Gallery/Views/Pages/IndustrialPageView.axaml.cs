// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Gallery.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;
using Splat;

namespace CrissCross.Avalonia.UI.Gallery.Views.Pages;

/// <summary>Industrial process controls gallery page.</summary>
public partial class IndustrialPageView : ReactiveUserControl<IndustrialPageViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="IndustrialPageView"/> class.</summary>
    public IndustrialPageView()
    {
        InitializeComponent();
        _ = this.WhenActivated(
            (ActivationDisposable _) => ViewModel ??= AppLocator.Current.GetService<IndustrialPageViewModel>());
    }
}
