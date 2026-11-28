// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides conversions for image formats.</summary>
public static class ImageFormatExtensions
{
    /// <summary>Extension members for <c>ImageFormat</c>.</summary>
    /// <param name="format">The image format.</param>
    extension(ImageFormat format)
    {
        /// <summary>Gets whether this image format is raster-based.</summary>
        /// <returns><see langword="true"/> for raster image formats.</returns>
        public bool IsRasterFormat() => format switch
        {
            ImageFormat.Bmp or ImageFormat.Jpeg or ImageFormat.Png or ImageFormat.Webp => true,
            ImageFormat.Svg => false,
            _ => throw new ArgumentException($"unknown image format: '{format}'")
        };

        /// <summary>Converts this value to a Skia encoded image format.</summary>
        /// <returns>The corresponding Skia encoded image format.</returns>
        /// <exception cref="NotSupportedException">The format is not supported by Skia.</exception>
        public SKEncodedImageFormat ToSKFormat() => format switch
        {
            ImageFormat.Jpeg => SKEncodedImageFormat.Jpeg,
            ImageFormat.Png => SKEncodedImageFormat.Png,
            ImageFormat.Bmp => SKEncodedImageFormat.Bmp,
            ImageFormat.Webp => SKEncodedImageFormat.Webp,
            _ => throw new NotSupportedException($"unsupported format: {format}")
        };
    }
}
