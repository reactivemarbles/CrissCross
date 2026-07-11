// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Gallery.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;
using Splat;

namespace CrissCross.Avalonia.UI.Gallery.Views.Pages;

/// <summary>Page demonstrating button controls.</summary>
public partial class ButtonsPageView : ReactiveUserControl<ButtonsPageViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="ButtonsPageView"/> class.</summary>
    public ButtonsPageView()
    {
        InitializeComponent();
        _ = this.WhenActivated((CompositeDisposable _) => ViewModel ??= AppLocator.Current.GetService<ButtonsPageViewModel>());
    }
}
