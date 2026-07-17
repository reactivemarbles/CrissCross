// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a control that allows the user to select or clear a binary option.</summary>
/// <remarks>A CheckBox displays a box that can be checked or unchecked by the user to indicate a selection. It is
/// typically used to represent a boolean value or to enable or disable a particular feature. The CheckBox can also
/// display content, such as text or images, alongside the box. This class extends the Avalonia implementation of
/// CheckBox and inherits its behavior and properties.</remarks>
public class CheckBox : global::Avalonia.Controls.CheckBox;
