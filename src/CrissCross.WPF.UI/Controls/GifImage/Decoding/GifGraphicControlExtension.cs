// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls.Decoding;

// label 0xF9
internal sealed class GifGraphicControlExtension : GifExtension
{
    internal const int ExtensionLabel = 0xF9;

    private GifGraphicControlExtension()
    {
    }

    public int BlockSize { get; private set; }

    public GifFrameDisposalMethod DisposalMethod { get; private set; }

    public bool UserInput { get; private set; }

    public bool HasTransparency { get; private set; }

    public int Delay { get; private set; }

    public int TransparencyIndex { get; private set; }

    internal override GifBlockKind Kind => GifBlockKind.Control;

    internal static async Task<GifGraphicControlExtension> ReadAsync(Stream stream)
    {
        var ext = new GifGraphicControlExtension();
        await ext.ReadInternalAsync(stream).ConfigureAwait(false);
        return ext;
    }

    private async Task ReadInternalAsync(Stream stream)
    {
        // Note: at this point, the label (0xF9) has already been read
        var bytes = new byte[6];
        await stream.ReadAllAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
        BlockSize = bytes[0]; // should always be 4
        if (BlockSize != 4)
        {
            throw GifHelpers.InvalidBlockSizeException("Graphic Control Extension", 4, BlockSize);
        }

        var packedFields = bytes[1];
        DisposalMethod = (GifFrameDisposalMethod)((packedFields & 0x1C) >> 2);
        UserInput = (packedFields & 0x02) != 0;
        HasTransparency = (packedFields & 0x01) != 0;
        Delay = BitConverter.ToUInt16(bytes, 2) * 10; // milliseconds
        TransparencyIndex = bytes[4];
    }
}
