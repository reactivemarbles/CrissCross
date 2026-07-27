// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Maui.UI;

namespace CrissCross.Maui.UI.Gallery;

/// <summary>Hosts the self-contained MAUI control gallery.</summary>
public partial class App : Application
{
    /// <summary>Initializes a new instance of the <see cref="App"/> class.</summary>
    public App()
    {
        InitializeComponent();
        _ = Resources.UseCrissCrossMauiUiResources();
    }

    /// <inheritdoc />
    protected override Window CreateWindow(IActivationState? activationState)
    {
        _ = activationState;
        return new(new GalleryPage());
    }
}
