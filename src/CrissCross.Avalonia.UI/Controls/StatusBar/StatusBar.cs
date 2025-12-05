// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a horizontal bar at the bottom of a window that displays status information.
/// </summary>
public class StatusBar : StackPanel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StatusBar"/> class.
    /// </summary>
    public StatusBar()
    {
        Orientation = Orientation.Horizontal;
        HorizontalAlignment = HorizontalAlignment.Stretch;
    }
}
