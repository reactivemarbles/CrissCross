// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using Splat;

namespace CrissCross.MAUI.Test;

/// <summary>
/// MAUI gallery page for CrissCross.Maui.UI controls.
/// </summary>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ControlsGalleryView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ControlsGalleryView"/> class.
    /// </summary>
    public ControlsGalleryView()
    {
        InitializeComponent();
        this.WhenActivated(_ =>
        {
            ViewModel ??= AppLocator.Current.GetService<ControlsGalleryViewModel>();
            BindingContext = ViewModel;
        });
    }
}
