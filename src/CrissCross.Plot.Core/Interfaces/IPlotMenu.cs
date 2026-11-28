// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides context-menu items and display operations for a plot control.</summary>
public interface IPlotMenu
{
    /// <summary>Gets the context-menu items currently registered.</summary>
    List<ContextMenuItem> ContextMenuItems { get; }

    /// <summary>Restores the default context-menu items.</summary>
    void Reset();

    /// <summary>Removes all context-menu items.</summary>
    void Clear();

    /// <summary>Adds a plot action to the context menu.</summary>
    /// <param name="label">The menu label.</param>
    /// <param name="action">The action to invoke.</param>
    void Add(string label, Action<Plot> action);

    /// <summary>Adds a separator to the context menu.</summary>
    void AddSeparator();

    /// <summary>Shows the context menu at the specified pixel position.</summary>
    /// <param name="pixel">The position where the menu should appear.</param>
    void ShowContextMenu(Pixel pixel);
}
