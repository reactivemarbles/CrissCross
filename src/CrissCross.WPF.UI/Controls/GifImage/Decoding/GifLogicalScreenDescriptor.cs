// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>Provides the GifLogicalScreenDescriptor member.</summary>
internal sealed class GifLogicalScreenDescriptor : IGifRect
{
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
        var bytes = new byte[7];
        await stream.ReadAllAsync(bytes, 0, bytes.Length).ConfigureAwait(false);

        Width = BitConverter.ToUInt16(bytes, 0);
        Height = BitConverter.ToUInt16(bytes, 2);
        var packedFields = bytes[4];
        HasGlobalColorTable = (packedFields & 0x80) != 0;
        ColorResolution = ((packedFields & 0x70) >> 4) + 1;
        IsGlobalColorTableSorted = (packedFields & 0x08) != 0;
        GlobalColorTableSize = 1 << ((packedFields & 0x07) + 1);
        BackgroundColorIndex = bytes[5];
        PixelAspectRatio =
            bytes[6] == 0
                ? 0.0
                : (15 + bytes[6]) / 64.0;
    }
}
