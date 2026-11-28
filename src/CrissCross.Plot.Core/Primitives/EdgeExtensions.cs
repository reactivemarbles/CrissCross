// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides orientation checks for plot edges.</summary>
public static class EdgeExtensions
{
    /// <summary>Extension members for <see cref="Edge"/>.</summary>
    /// <param name="edge">The edge to inspect.</param>
    extension(Edge edge)
    {
        /// <summary>Determines whether the edge represents a horizontal axis.</summary>
        /// <returns><see langword="true"/> for bottom and top edges.</returns>
        public bool IsHorizontal() => edge switch
        {
            Edge.Left or Edge.Right => false,
            Edge.Bottom or Edge.Top => true,
            _ => throw new ArgumentOutOfRangeException(nameof(edge), edge, "Unknown edge value.")
        };

        /// <summary>Determines whether the edge represents a vertical axis.</summary>
        /// <returns><see langword="true"/> for left and right edges.</returns>
        public bool IsVertical() => edge switch
        {
            Edge.Left or Edge.Right => true,
            Edge.Bottom or Edge.Top => false,
            _ => throw new ArgumentOutOfRangeException(nameof(edge), edge, "Unknown edge value.")
        };
    }
}
