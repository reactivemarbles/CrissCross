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

    /// <summary>The graphic control extension block size.</summary>
    private const int GraphicControlBlockSize = 4;

    /// <summary>The full graphic control extension byte count.</summary>
    private const int GraphicControlByteCount = GraphicControlBlockSize + 2;

    /// <summary>The packed fields offset.</summary>
    private const int PackedFieldsOffset = 1;

    /// <summary>The disposal method mask.</summary>
    private const int DisposalMethodMask = 0x1C;

    /// <summary>The disposal method shift.</summary>
    private const int DisposalMethodShift = 2;

    /// <summary>The user input flag mask.</summary>
    private const int UserInputFlagMask = 0x02;

    /// <summary>The transparency flag mask.</summary>
    private const int TransparencyFlagMask = 0x01;

    /// <summary>The delay offset.</summary>
    private const int DelayOffset = 2;

    /// <summary>The GIF delay unit in milliseconds.</summary>
    private const int DelayUnitMilliseconds = 10;

    /// <summary>The transparency index offset.</summary>
    private const int TransparencyIndexOffset = 4;

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
        var bytes = new byte[GraphicControlByteCount];
        await stream.ReadAllAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
        BlockSize = bytes[0]; // should always be 4
        if (BlockSize != GraphicControlBlockSize)
        {
            throw GifHelpers.InvalidBlockSizeException("Graphic Control Extension", GraphicControlBlockSize, BlockSize);
        }

        var packedFields = bytes[PackedFieldsOffset];
        DisposalMethod = (GifFrameDisposalMethod)((packedFields & DisposalMethodMask) >> DisposalMethodShift);
        UserInput = (packedFields & UserInputFlagMask) != 0;
        HasTransparency = (packedFields & TransparencyFlagMask) != 0;
        Delay = BitConverter.ToUInt16(bytes, DelayOffset) * DelayUnitMilliseconds;
        TransparencyIndex = bytes[TransparencyIndexOffset];
    }
}
