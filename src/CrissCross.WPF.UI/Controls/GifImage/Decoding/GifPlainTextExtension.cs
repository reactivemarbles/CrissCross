// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls.Decoding;

// label 0x01
internal sealed class GifPlainTextExtension : GifExtension
{
    internal const int ExtensionLabel = 0x01;

    private GifPlainTextExtension()
    {
    }

    public int BlockSize { get; private set; }

    public int Left { get; private set; }

    public int Top { get; private set; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public int CellWidth { get; private set; }

    public int CellHeight { get; private set; }

    public int ForegroundColorIndex { get; private set; }

    public int BackgroundColorIndex { get; private set; }

    public string? Text { get; private set; }

    public IList<GifExtension>? Extensions { get; private set; }

    internal override GifBlockKind Kind => GifBlockKind.GraphicRendering;

    internal static new async Task<GifPlainTextExtension> ReadAsync(Stream stream, IEnumerable<GifExtension> controlExtensions)
    {
        var plainText = new GifPlainTextExtension();
        await plainText.ReadInternalAsync(stream, controlExtensions).ConfigureAwait(false);
        return plainText;
    }

    private async Task ReadInternalAsync(Stream stream, IEnumerable<GifExtension> controlExtensions)
    {
        // Note: at this point, the label (0x01) has already been read
        var bytes = new byte[13];
        await stream.ReadAllAsync(bytes, 0, bytes.Length).ConfigureAwait(false);

        BlockSize = bytes[0];
        if (BlockSize != 12)
        {
            throw GifHelpers.InvalidBlockSizeException("Plain Text Extension", 12, BlockSize);
        }

        Left = BitConverter.ToUInt16(bytes, 1);
        Top = BitConverter.ToUInt16(bytes, 3);
        Width = BitConverter.ToUInt16(bytes, 5);
        Height = BitConverter.ToUInt16(bytes, 7);
        CellWidth = bytes[9];
        CellHeight = bytes[10];
        ForegroundColorIndex = bytes[11];
        BackgroundColorIndex = bytes[12];

        var dataBytes = await GifHelpers.ReadDataBlocksAsync(stream).ConfigureAwait(false);
        Text = GifHelpers.GetString(dataBytes);
        Extensions = controlExtensions.ToList().AsReadOnly();
    }
}
