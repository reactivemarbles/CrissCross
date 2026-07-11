// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>Provides the GifLogicalScreenDescriptor member.</summary>
internal sealed class GifLogicalScreenDescriptor : IGifRect
{
    /// <summary>The logical screen descriptor byte count.</summary>
    private const int DescriptorByteCount = 7;

    /// <summary>The height offset.</summary>
    private const int HeightOffset = 2;

    /// <summary>The packed fields offset.</summary>
    private const int PackedFieldsOffset = 4;

    /// <summary>The background color index offset.</summary>
    private const int BackgroundColorIndexOffset = 5;

    /// <summary>The pixel aspect ratio offset.</summary>
    private const int PixelAspectRatioOffset = 6;

    /// <summary>The global color table flag mask.</summary>
    private const int GlobalColorTableFlagMask = 0x80;

    /// <summary>The color resolution mask.</summary>
    private const int ColorResolutionMask = 0x70;

    /// <summary>The color resolution shift.</summary>
    private const int ColorResolutionShift = 4;

    /// <summary>The global color table sorted flag mask.</summary>
    private const int GlobalColorTableSortedFlagMask = 0x08;

    /// <summary>The global color table size mask.</summary>
    private const int GlobalColorTableSizeMask = 0x07;

    /// <summary>The color table size exponent offset.</summary>
    private const int ColorTableSizeExponentOffset = 1;

    /// <summary>The GIF pixel aspect ratio offset.</summary>
    private const int PixelAspectRatioValueOffset = 15;

    /// <summary>The GIF pixel aspect ratio divisor.</summary>
    private const double PixelAspectRatioDivisor = 64.0;

    /// <summary>Gets the Width value.</summary>
    public int Width { get; private set; }

    /// <summary>Gets the Height value.</summary>
    public int Height { get; private set; }

    /// <summary>Gets the HasGlobalColorTable value.</summary>
    public bool HasGlobalColorTable { get; private set; }

    /// <summary>Gets the ColorResolution value.</summary>
    public int ColorResolution { get; private set; }

    /// <summary>Gets the IsGlobalColorTableSorted value.</summary>
    public bool IsGlobalColorTableSorted { get; private set; }

    /// <summary>Gets the GlobalColorTableSize value.</summary>
    public int GlobalColorTableSize { get; private set; }

    /// <summary>Gets the BackgroundColorIndex value.</summary>
    public int BackgroundColorIndex { get; private set; }

    /// <summary>Gets the PixelAspectRatio value.</summary>
    public double PixelAspectRatio { get; private set; }

    int IGifRect.Left => 0;

    int IGifRect.Top => 0;

    /// <summary>Provides the ReadAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
    internal static async Task<GifLogicalScreenDescriptor> ReadAsync(Stream stream)
    {
        var descriptor = new GifLogicalScreenDescriptor();
        await descriptor.ReadInternalAsync(stream).ConfigureAwait(false);
        return descriptor;
    }

    /// <summary>Provides the ReadInternalAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
    private async Task ReadInternalAsync(Stream stream)
    {
        var bytes = new byte[DescriptorByteCount];
        await stream.ReadAllAsync(bytes, 0, bytes.Length).ConfigureAwait(false);

        Width = BitConverter.ToUInt16(bytes, 0);
        Height = BitConverter.ToUInt16(bytes, HeightOffset);
        var packedFields = bytes[PackedFieldsOffset];
        HasGlobalColorTable = (packedFields & GlobalColorTableFlagMask) != 0;
        ColorResolution = ((packedFields & ColorResolutionMask) >> ColorResolutionShift) + ColorTableSizeExponentOffset;
        IsGlobalColorTableSorted = (packedFields & GlobalColorTableSortedFlagMask) != 0;
        GlobalColorTableSize = 1 << ((packedFields & GlobalColorTableSizeMask) + ColorTableSizeExponentOffset);
        BackgroundColorIndex = bytes[BackgroundColorIndexOffset];
        PixelAspectRatio =
            bytes[PixelAspectRatioOffset] == 0
                ? 0.0
                : (PixelAspectRatioValueOffset + bytes[PixelAspectRatioOffset]) / PixelAspectRatioDivisor;
    }
}
