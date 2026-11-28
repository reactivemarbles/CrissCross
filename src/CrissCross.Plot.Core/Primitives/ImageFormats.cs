// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides conversions from file extensions and names to image formats.</summary>
public static class ImageFormats
{
    /// <summary>Determines the image format represented by a file extension.</summary>
    /// <param name="ext">The extension, including its leading period.</param>
    /// <returns>The corresponding image format.</returns>
    /// <exception cref="ArgumentException">The extension is malformed or unsupported.</exception>
    public static ImageFormat FromFileExtension(string ext)
    {
        if (ext.Length == 0 || ext[0] != '.')
        {
            throw new ArgumentException("extension must start with '.'");
        }

        return ext.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => ImageFormat.Jpeg,
            ".png" => ImageFormat.Png,
            ".bmp" => ImageFormat.Bmp,
            ".webp" => ImageFormat.Webp,
            ".svg" => ImageFormat.Svg,
            _ => throw new ArgumentException($"unknown image format: '{ext}'")
        };
    }

    /// <summary>Determines the image format represented by a file name.</summary>
    /// <param name="filename">The file name whose extension is examined.</param>
    /// <returns>The corresponding image format.</returns>
    public static ImageFormat FromFilename(string filename) => FromFileExtension(Path.GetExtension(filename));
}
