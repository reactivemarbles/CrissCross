// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace CrissCross.Reactive.Maui.UI.Resources.Styles;
#else
namespace CrissCross.Maui.UI.Resources.Styles;
#endif

/// <summary>Base Light and Dark semantic palette for CrissCross MAUI UI resources.</summary>
public partial class CrissCrossMauiColors : ResourceDictionary
{
    /// <summary>Initializes a new instance of the <see cref="CrissCrossMauiColors"/> class.</summary>
    public CrissCrossMauiColors() => InitializeComponent();
}
