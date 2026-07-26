// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Layout;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a control that displays a horizontal bar of commands or tools.</summary>
public class ToolBar : StackPanel
{
    /// <summary>Default spacing between toolbar items.</summary>
    private const double DefaultSpacing = 4D;

    /// <summary>Initializes a new instance of the <see cref="ToolBar"/> class.</summary>
    public ToolBar()
    {
        Orientation = Orientation.Horizontal;
        Spacing = DefaultSpacing;
    }
}
