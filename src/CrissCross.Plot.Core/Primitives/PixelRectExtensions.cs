// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides conversions from SkiaSharp rectangles to pixel rectangles.</summary>
public static class PixelRectExtensions
{
    /// <summary>Provides conversion from <see cref="SKRect"/> values.</summary>
    /// <param name="rect">The SkiaSharp rectangle whose edges are converted.</param>
    extension(SKRect rect)
    {
        /// <summary>Converts this SkiaSharp rectangle to a <see cref="PixelRect"/>.</summary>
        /// <returns>A pixel rectangle with the same edges.</returns>
        public PixelRect ToPixelRect() => new(rect.Left, rect.Right, rect.Bottom, rect.Top);
    }

    /// <summary>Provides conversion from <see cref="SKRectI"/> values.</summary>
    /// <param name="rect">The SkiaSharp integer rectangle whose edges are converted.</param>
    extension(SKRectI rect)
    {
        /// <summary>Converts this SkiaSharp integer rectangle to a <see cref="PixelRect"/>.</summary>
        /// <returns>A pixel rectangle with the same edges.</returns>
        public PixelRect ToPixelRect() => new(rect.Left, rect.Right, rect.Bottom, rect.Top);
    }
}
