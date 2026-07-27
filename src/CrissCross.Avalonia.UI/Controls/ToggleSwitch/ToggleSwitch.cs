// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Use ToggleSwitch to present users with two mutually exclusive options (like on/off).</summary>
public class ToggleSwitch : global::Avalonia.Controls.ToggleSwitch
{
    // Inherits all functionality from Avalonia ToggleSwitch including:
    // - OnContent property
    // - OffContent property
    // - IsChecked property
    // - Content property
}
