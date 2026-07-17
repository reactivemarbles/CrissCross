// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>Label 0x01.</summary>
internal sealed class GifPlainTextExtension : GifExtension
{
    /// <summary>Provides the ExtensionLabel member.</summary>
    internal const int ExtensionLabel = 0x01;

    /// <summary>The plain text extension block size.</summary>
    private const int PlainTextBlockSize = 12;

    /// <summary>The full plain text extension byte count.</summary>
    private const int PlainTextByteCount = PlainTextBlockSize + 1;

    /// <summary>The left offset.</summary>
    private const int LeftOffset = 1;

    /// <summary>The top offset.</summary>
    private const int TopOffset = 3;

    /// <summary>The width offset.</summary>
    private const int WidthOffset = 5;

    /// <summary>The height offset.</summary>
    private const int HeightOffset = 7;

    /// <summary>The cell width offset.</summary>
    private const int CellWidthOffset = 9;

    /// <summary>The cell height offset.</summary>
    private const int CellHeightOffset = 10;

    /// <summary>The foreground color index offset.</summary>
    private const int ForegroundColorIndexOffset = 11;

    /// <summary>The background color index offset.</summary>
    private const int BackgroundColorIndexOffset = 12;

    /// <summary>Initializes a new instance of the <see cref="GifPlainTextExtension"/> class.</summary>
    private GifPlainTextExtension() { }

    /// <summary>Gets the BlockSize value.</summary>
    public int BlockSize { get; private set; }

    /// <summary>Gets the Left value.</summary>
    public int Left { get; private set; }

    /// <summary>Gets the Top value.</summary>
    public int Top { get; private set; }

    /// <summary>Gets the Width value.</summary>
    public int Width { get; private set; }

    /// <summary>Gets the Height value.</summary>
    public int Height { get; private set; }

    /// <summary>Gets the CellWidth value.</summary>
    public int CellWidth { get; private set; }

    /// <summary>Gets the CellHeight value.</summary>
    public int CellHeight { get; private set; }

    /// <summary>Gets the ForegroundColorIndex value.</summary>
    public int ForegroundColorIndex { get; private set; }

    /// <summary>Gets the BackgroundColorIndex value.</summary>
    public int BackgroundColorIndex { get; private set; }

    /// <summary>Gets the Text value.</summary>
    public string? Text { get; private set; }

    /// <summary>Gets the Extensions value.</summary>
    public IList<GifExtension>? Extensions { get; private set; }

    internal override GifBlockKind Kind => GifBlockKind.GraphicRendering;

    /// <summary>Provides the ReadAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <param name="controlExtensions">The controlExtensions value.</param>
    /// <returns>The result.</returns>
    internal static new async Task<GifPlainTextExtension> ReadAsync(
        Stream stream,
        IEnumerable<GifExtension> controlExtensions)
    {
        var plainText = new GifPlainTextExtension();
        await plainText.ReadInternalAsync(stream, controlExtensions).ConfigureAwait(false);
        return plainText;
    }

    /// <summary>Provides the ReadInternalAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <param name="controlExtensions">The controlExtensions value.</param>
    /// <returns>The result.</returns>
    private async Task ReadInternalAsync(Stream stream, IEnumerable<GifExtension> controlExtensions)
    {
        // Note: at this point, the label (0x01) has already been read
        var bytes = new byte[PlainTextByteCount];
        await stream.ReadAllAsync(bytes, 0, bytes.Length).ConfigureAwait(false);

        BlockSize = bytes[0];
        if (BlockSize != PlainTextBlockSize)
        {
            throw GifHelpers.InvalidBlockSizeException("Plain Text Extension", PlainTextBlockSize, BlockSize);
        }

        Left = BitConverter.ToUInt16(bytes, LeftOffset);
        Top = BitConverter.ToUInt16(bytes, TopOffset);
        Width = BitConverter.ToUInt16(bytes, WidthOffset);
        Height = BitConverter.ToUInt16(bytes, HeightOffset);
        CellWidth = bytes[CellWidthOffset];
        CellHeight = bytes[CellHeightOffset];
        ForegroundColorIndex = bytes[ForegroundColorIndexOffset];
        BackgroundColorIndex = bytes[BackgroundColorIndexOffset];

        var dataBytes = await GifHelpers.ReadDataBlocksAsync(stream).ConfigureAwait(false);
        Text = GifHelpers.GetString(dataBytes);
        Extensions = controlExtensions.ToList().AsReadOnly();
    }
}
