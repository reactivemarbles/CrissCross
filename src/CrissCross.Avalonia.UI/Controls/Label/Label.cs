// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a lightweight text control for displaying non-editable text within a user interface.</summary>
/// <remarks>Label is typically used to display static text or captions for other controls, such as input fields.
/// Unlike editable text controls, Label does not support user interaction or text editing. It inherits all text
/// formatting and layout capabilities from TextBlock, allowing customization of font, alignment, and other text
/// properties.</remarks>
public class Label : global::Avalonia.Controls.TextBlock;
