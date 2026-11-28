// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides orientation and sizing operations for panels.</summary>
public static class IPanelExtensions
{
    /// <summary>Provides orientation and size operations for a panel.</summary>
    /// <param name="panel">The panel used by these operations.</param>
    extension(IPanel panel)
    {
        /// <summary>Determines whether the panel lies on a horizontal edge.</summary>
        /// <returns><see langword="true"/> for a top or bottom panel.</returns>
        public bool IsHorizontal() => panel.Edge == Edge.Bottom || panel.Edge == Edge.Top;

        /// <summary>Determines whether the panel lies on a vertical edge.</summary>
        /// <returns><see langword="true"/> for a left or right panel.</returns>
        public bool IsVertical() => !panel.IsHorizontal();

        /// <summary>Sets the minimum and maximum panel size to the specified value.</summary>
        /// <param name="size">The fixed panel size in pixels.</param>
        public void LockSize(float size)
        {
            panel.MinimumSize = size;
            panel.MaximumSize = size;
        }

        /// <summary>Removes all pixel size constraints from the panel.</summary>
        public void ResetSize()
        {
            panel.MinimumSize = 0;
            panel.MaximumSize = float.MaxValue;
        }
    }
}
