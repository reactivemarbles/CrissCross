// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides conversions for horizontal alignment values.</summary>
public static class HorizontalAlignmentExtensions
{
    /// <summary>Extension members for <c>HorizontalAlignment</c>.</summary>
    /// <param name="ha">The horizontal alignment value.</param>
    extension(HorizontalAlignment ha)
    {
        /// <summary>Converts this alignment to a Skia text alignment.</summary>
        /// <returns>The corresponding Skia text alignment.</returns>
        public SKTextAlign ToSKTextAlign() => ha switch
        {
            HorizontalAlignment.Left => SKTextAlign.Left,
            HorizontalAlignment.Center => SKTextAlign.Center,
            HorizontalAlignment.Right => SKTextAlign.Right,
            _ => throw new NotSupportedException(),
        };
    }
}
