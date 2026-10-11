// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace CrissCross.Reactive.Maui.UI.Resources.Styles;
#else
namespace CrissCross.Maui.UI.Resources.Styles;
#endif

/// <summary>Root resource dictionary for CrissCross MAUI UI colors, typography, and control styles.</summary>
public partial class CrissCrossMauiUi : ResourceDictionary
{
    /// <summary>Initializes a new instance of the <see cref="CrissCrossMauiUi"/> class.</summary>
    public CrissCrossMauiUi()
    {
        InitializeComponent();
    }
}
