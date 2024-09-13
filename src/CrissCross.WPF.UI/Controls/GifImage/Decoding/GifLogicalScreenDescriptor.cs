// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;

namespace CrissCross.WPF.UI.Controls.Decoding;

internal class GifLogicalScreenDescriptor
{
    public int Width { get; private set; }

    public int Height { get; private set; }

    public bool HasGlobalColorTable { get; private set; }

    public int ColorResolution { get; private set; }

    public bool IsGlobalColorTableSorted { get; private set; }

    public int GlobalColorTableSize { get; private set; }

    public int BackgroundColorIndex { get; private set; }

    public double PixelAspectRatio { get; private set; }

    internal static GifLogicalScreenDescriptor ReadLogicalScreenDescriptor(Stream stream)
    {
        var descriptor = new GifLogicalScreenDescriptor();
        descriptor.Read(stream);
        return descriptor;
    }

    private void Read(Stream stream)
    {
        var bytes = new byte[7];
        stream.ReadAll(bytes, 0, bytes.Length);

        Width = BitConverter.ToUInt16(bytes, 0);
        Height = BitConverter.ToUInt16(bytes, 2);
        var packedFields = bytes[4];
        HasGlobalColorTable = (packedFields & 0x80) != 0;
        ColorResolution = ((packedFields & 0x70) >> 4) + 1;
        IsGlobalColorTableSorted = (packedFields & 0x08) != 0;
        GlobalColorTableSize = 1 << ((packedFields & 0x07) + 1);
        BackgroundColorIndex = bytes[5];
        PixelAspectRatio =
            bytes[5] == 0
                ? 0.0
                : (15 + bytes[5]) / 64.0;
    }
}
