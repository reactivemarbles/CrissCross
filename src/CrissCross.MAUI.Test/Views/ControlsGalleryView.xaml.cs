// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using Splat;

namespace CrissCross.MAUI.Test;

/// <summary>MAUI gallery page for CrissCross.Maui.UI controls.</summary>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ControlsGalleryView
{
    /// <summary>Initializes a new instance of the <see cref="ControlsGalleryView"/> class.</summary>
    public ControlsGalleryView()
    {
        InitializeComponent();
        _ = this.WhenActivated((CompositeDisposable _) =>
        {
            ViewModel ??= AppLocator.Current.GetService<ControlsGalleryViewModel>();
            BindingContext = ViewModel;
        });
    }
}
