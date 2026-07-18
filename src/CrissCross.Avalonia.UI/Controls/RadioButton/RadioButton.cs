// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents the RadioButton type.</summary>
/// <remarks>Radio buttons are typically used in groups, where only one button in the group can be selected at a
/// time. Selecting a different radio button in the same group will automatically clear the selection from the
/// previously selected button. Use radio buttons when you want users to select one option from a set of mutually
/// exclusive choices.</remarks>
public class RadioButton : global::Avalonia.Controls.RadioButton;
