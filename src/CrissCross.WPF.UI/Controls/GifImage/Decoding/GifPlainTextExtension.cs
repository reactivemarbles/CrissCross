// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.UI.Controls.Extensions;
#else
using CrissCross.WPF.UI.Controls.Extensions;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Decoding;
#else
namespace CrissCross.WPF.UI.Controls.Decoding;
#endif

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
    internal int BlockSize { get; private set; }

    /// <summary>Gets the Left value.</summary>
    internal int Left { get; private set; }

    /// <summary>Gets the Top value.</summary>
    internal int Top { get; private set; }

    /// <summary>Gets the Width value.</summary>
    internal int Width { get; private set; }

    /// <summary>Gets the Height value.</summary>
    internal int Height { get; private set; }

    /// <summary>Gets the CellWidth value.</summary>
    internal int CellWidth { get; private set; }

    /// <summary>Gets the CellHeight value.</summary>
    internal int CellHeight { get; private set; }

    /// <summary>Gets the ForegroundColorIndex value.</summary>
    internal int ForegroundColorIndex { get; private set; }

    /// <summary>Gets the BackgroundColorIndex value.</summary>
    internal int BackgroundColorIndex { get; private set; }

    /// <summary>Gets the Text value.</summary>
    internal string? Text { get; private set; }

    /// <summary>Gets the Extensions value.</summary>
    internal IList<GifExtension>? Extensions { get; private set; }

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
        Extensions = new List<GifExtension>(controlExtensions).AsReadOnly();
    }
}
