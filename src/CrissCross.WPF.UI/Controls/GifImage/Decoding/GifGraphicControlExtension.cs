// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>Label 0xF9.</summary>
internal sealed class GifGraphicControlExtension : GifExtension
{
    /// <summary>Provides the ExtensionLabel member.</summary>
    internal const int ExtensionLabel = 0xF9;

    /// <summary>Initializes a new instance of the <see cref="GifGraphicControlExtension"/> class.</summary>
    private GifGraphicControlExtension()
    {
    }

    /// <summary>Gets the BlockSize value.</summary>
    public int BlockSize { get; private set; }

    /// <summary>Gets the DisposalMethod value.</summary>
    public GifFrameDisposalMethod DisposalMethod { get; private set; }

    /// <summary>Gets the UserInput value.</summary>
    public bool UserInput { get; private set; }

    /// <summary>Gets the HasTransparency value.</summary>
    public bool HasTransparency { get; private set; }

    /// <summary>Gets the Delay value.</summary>
    public int Delay { get; private set; }

    /// <summary>Gets the TransparencyIndex value.</summary>
    public int TransparencyIndex { get; private set; }

    internal override GifBlockKind Kind => GifBlockKind.Control;

    /// <summary>Provides the ReadAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
    internal static async Task<GifGraphicControlExtension> ReadAsync(Stream stream)
    {
        var ext = new GifGraphicControlExtension();
        await ext.ReadInternalAsync(stream).ConfigureAwait(false);
        return ext;
    }

    /// <summary>Provides the ReadInternalAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
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
