// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Arranges child elements into a single line that can be oriented horizontally or vertically.
/// </summary>
/// <remarks>Use StackPanel to create layouts where child controls are stacked in a single direction. The
/// orientation can be set to either horizontal or vertical, allowing for flexible UI arrangements. StackPanel does not
/// wrap its content; if the combined size of its children exceeds the available space, the content may be clipped. For
/// layouts that require wrapping, consider using a different panel such as WrapPanel.</remarks>
public class StackPanel : global::Avalonia.Controls.StackPanel;
