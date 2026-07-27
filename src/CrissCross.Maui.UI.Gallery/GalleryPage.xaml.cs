// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Gallery;

/// <summary>Presents every public CrissCross MAUI UI control in one visual QA surface.</summary>
public partial class GalleryPage : ContentPage
{
    /// <summary>Initializes a new instance of the <see cref="GalleryPage"/> class.</summary>
    public GalleryPage()
    {
        InitializeComponent();
        BindingContext = new GalleryViewModel();
    }
}
