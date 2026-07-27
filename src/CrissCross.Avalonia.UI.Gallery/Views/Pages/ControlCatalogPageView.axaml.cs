// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Gallery.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;
using Splat;

namespace CrissCross.Avalonia.UI.Gallery.Views.Pages;

/// <summary>Displays the data-driven gallery coverage catalog.</summary>
public partial class ControlCatalogPageView : ReactiveUserControl<ControlCatalogPageViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="ControlCatalogPageView"/> class.</summary>
    public ControlCatalogPageView()
    {
        InitializeComponent();
        _ = this.WhenActivated(
            (CompositeDisposable _) => ViewModel ??= AppLocator.Current.GetService<ControlCatalogPageViewModel>());
    }
}
