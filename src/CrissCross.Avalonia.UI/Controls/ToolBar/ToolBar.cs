// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a control that displays a horizontal bar of commands or tools.
/// </summary>
public class ToolBar : StackPanel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolBar"/> class.
    /// </summary>
    public ToolBar()
    {
        Orientation = Orientation.Horizontal;
        Spacing = 4;
    }
}
