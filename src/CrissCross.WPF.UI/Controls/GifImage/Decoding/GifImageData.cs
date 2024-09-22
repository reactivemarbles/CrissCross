// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;

namespace CrissCross.WPF.UI.Controls.Decoding;

internal sealed class GifImageData
{
    private GifImageData()
    {
    }

    public byte LzwMinimumCodeSize { get; set; }

    public byte[]? CompressedData { get; set; }

    internal static GifImageData ReadImageData(Stream stream, bool metadataOnly)
    {
        var imgData = new GifImageData();
        imgData.Read(stream, metadataOnly);
        return imgData;
    }

    private void Read(Stream stream, bool metadataOnly)
    {
        LzwMinimumCodeSize = (byte)stream.ReadByte();
        CompressedData = GifHelpers.ReadDataBlocks(stream, metadataOnly);
    }
}
