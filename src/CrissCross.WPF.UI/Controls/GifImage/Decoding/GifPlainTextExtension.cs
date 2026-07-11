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

    /// <summary>Initializes a new instance of the <see cref="GifPlainTextExtension"/> class.</summary>
    private GifPlainTextExtension()
    {
    }

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
    internal static new async Task<GifPlainTextExtension> ReadAsync(Stream stream, IEnumerable<GifExtension> controlExtensions)
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
