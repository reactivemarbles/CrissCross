// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Runtime.InteropServices;

namespace CrissCross.Plot.IO;

/// <summary>Holds the values that define the header bytes of a bitmap file.</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct BitmapHeader : IEquatable<BitmapHeader>
{
    /// <summary>Gets the number of bytes in the bitmap header.</summary>
    public static readonly int FileHeaderSize = 54;

    /// <summary>Gets the high byte of the bitmap magic number.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly byte MagicNumberHighByte = (byte)'B';

    /// <summary>Gets the low byte of the bitmap magic number.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly byte MagicNumberLowByte = (byte)'M';

    /// <summary>Gets the total number of bytes in the image.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly int Size;

    /// <summary>Gets the first reserved header value.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly short Reserved1;

    /// <summary>Gets the second reserved header value.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly short Reserved2;

    /// <summary>Gets the byte offset where the image data begins.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly int Offset = FileHeaderSize;

    /// <summary>Gets the size of the bitmap information header.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly int BitmapInfoHeaderSize = 40;

    /// <summary>Gets the image width in pixels.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly int Width;

    /// <summary>Gets the image height in pixels; negative values encode rows top-to-bottom.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly int Height;

    /// <summary>Gets the number of color planes, which is one for standard images.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly short ColorPlanes = 1;

    /// <summary>Gets the bits per pixel, typically 8, 24, or 32.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly short BitsPerPixel = 32;

    /// <summary>Gets the image compression method; zero means no compression.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly int CompressionMethod;

    /// <summary>Gets the size of the compressed image, or zero when compression is disabled.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly int PixelSize;

    /// <summary>Gets the horizontal resolution in pixels per meter, or zero when undefined.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly int HorizontalResolution;

    /// <summary>Gets the vertical resolution in pixels per meter, or zero when undefined.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly int VerticalResolution;

    /// <summary>Gets the number of colors in the palette, or zero for the default.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly int ColorsInPalette;

    /// <summary>Gets the number of important colors, or zero when all colors are important.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Public fields preserve the sequential bitmap-header layout and existing field-access API.")]
    public readonly int ImportantColors;

    /// <summary>Gets the number of bits in one byte.</summary>
    private const int BitsPerByte = 8;

    /// <summary>Initializes a new instance of the <see cref="BitmapHeader"/> struct.</summary>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    public BitmapHeader(int width, int height)
    {
        Size = (width * Math.Abs(height) * BitsPerPixel / BitsPerByte) + FileHeaderSize;
        Width = width;
        Height = -height;
    }

    /// <summary>Determines whether two bitmap headers are equal.</summary>
    /// <param name="left">The first header.</param>
    /// <param name="right">The second header.</param>
    /// <returns><see langword="true"/> when the headers are equal.</returns>
    public static bool operator ==(BitmapHeader left, BitmapHeader right) => left.Equals(right);

    /// <summary>Determines whether two bitmap headers are not equal.</summary>
    /// <param name="left">The first header.</param>
    /// <param name="right">The second header.</param>
    /// <returns><see langword="true"/> when the headers are not equal.</returns>
    public static bool operator !=(BitmapHeader left, BitmapHeader right) => !left.Equals(right);

    /// <summary>Determines whether this header equals another bitmap header.</summary>
    /// <param name="other">The bitmap header to compare.</param>
    /// <returns><see langword="true"/> when every header field matches.</returns>
    public bool Equals(BitmapHeader other) => EqualsFirstHalf(other) && EqualsSecondHalf(other);

    /// <summary>Determines whether this header equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal bitmap header.</returns>
    public override bool Equals(object? obj) => obj is BitmapHeader other && Equals(other);

    /// <summary>Gets a hash code for this bitmap header.</summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        int firstHalf = CrissCross.Plot.HashCodeCombiner.Combine(
            CrissCross.Plot.HashCodeCombiner.Combine(MagicNumberHighByte, MagicNumberLowByte, Size, Reserved1, Reserved2),
            CrissCross.Plot.HashCodeCombiner.Combine(Offset, BitmapInfoHeaderSize, Width));
        int secondHalf = CrissCross.Plot.HashCodeCombiner.Combine(
            CrissCross.Plot.HashCodeCombiner.Combine(Height, ColorPlanes, BitsPerPixel, CompressionMethod, PixelSize),
            CrissCross.Plot.HashCodeCombiner.Combine(HorizontalResolution, VerticalResolution, ColorsInPalette));
        return CrissCross.Plot.HashCodeCombiner.Combine(firstHalf, secondHalf, ImportantColors);
    }

    /// <summary>Compares the first group of bitmap-header fields.</summary>
    /// <param name="other">The bitmap header to compare.</param>
    /// <returns><see langword="true"/> when the first group matches.</returns>
    private bool EqualsFirstHalf(in BitmapHeader other) =>
        MagicNumberHighByte == other.MagicNumberHighByte
        && MagicNumberLowByte == other.MagicNumberLowByte
        && Size == other.Size
        && Reserved1 == other.Reserved1
        && Reserved2 == other.Reserved2
        && Offset == other.Offset
        && BitmapInfoHeaderSize == other.BitmapInfoHeaderSize
        && Width == other.Width;

    /// <summary>Compares the remaining bitmap-header fields.</summary>
    /// <param name="other">The bitmap header to compare.</param>
    /// <returns><see langword="true"/> when the remaining fields match.</returns>
    private bool EqualsSecondHalf(in BitmapHeader other) =>
        Height == other.Height
        && ColorPlanes == other.ColorPlanes
        && BitsPerPixel == other.BitsPerPixel
        && CompressionMethod == other.CompressionMethod
        && PixelSize == other.PixelSize
        && HorizontalResolution == other.HorizontalResolution
        && VerticalResolution == other.VerticalResolution
        && ColorsInPalette == other.ColorsInPalette
        && ImportantColors == other.ImportantColors;
}
