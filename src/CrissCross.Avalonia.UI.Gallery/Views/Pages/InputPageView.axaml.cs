// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Gallery.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;
using Splat;

namespace CrissCross.Avalonia.UI.Gallery.Views.Pages;

/// <summary>
/// Page demonstrating input controls.
/// </summary>
public partial class InputPageView : ReactiveUserControl<InputPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputPageView"/> class.
    /// </summary>
    public InputPageView()
    {
        InitializeComponent();
        this.WhenActivated(_ =>
        {
            ViewModel ??= AppLocator.Current.GetService<InputPageViewModel>();
        });
    }
}
