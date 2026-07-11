// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>Provides the GifHelpers member.</summary>
internal static class GifHelpers
{
    /// <summary>The maximum GIF data sub-block size in bytes.</summary>
    private const int MaxDataSubBlockByteCount = 255;

    /// <summary>The GIF color byte count.</summary>
    private const int GifColorByteCount = 3;

    /// <summary>The green byte offset in a GIF color.</summary>
    private const int GifColorGreenOffset = 1;

    /// <summary>The blue byte offset in a GIF color.</summary>
    private const int GifColorBlueOffset = 2;

    /// <summary>The Netscape repeat count minimum data length.</summary>
    private const int NetscapeRepeatCountDataLength = 3;

    /// <summary>The Netscape repeat count offset.</summary>
    private const int NetscapeRepeatCountOffset = 1;

    /// <summary>Provides the ReadStringAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <param name="length">The length value.</param>
    /// <returns>The result.</returns>
    public static async Task<string> ReadStringAsync(Stream stream, int length)
    {
        var bytes = new byte[length];
        await stream.ReadAllAsync(bytes, 0, length).ConfigureAwait(false);
        return GetString(bytes);
    }

    /// <summary>Provides the ConsumeDataBlocksAsync member.</summary>
    /// <param name="sourceStream">The sourceStream value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>The result.</returns>
    public static async Task ConsumeDataBlocksAsync(Stream sourceStream, CancellationToken cancellationToken = default) => await CopyDataBlocksToStreamAsync(sourceStream, Stream.Null, cancellationToken);

    /// <summary>Provides the ReadDataBlocksAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>The result.</returns>
    public static async Task<byte[]> ReadDataBlocksAsync(Stream stream, CancellationToken cancellationToken = default)
    {
#if NET8_0_OR_GREATER
        await using var ms = new MemoryStream();
#else
        using var ms = new MemoryStream();
#endif
        await CopyDataBlocksToStreamAsync(stream, ms, cancellationToken);
        return ms.ToArray();
    }

    /// <summary>Provides the CopyDataBlocksToStreamAsync member.</summary>
    /// <param name="sourceStream">The sourceStream value.</param>
    /// <param name="targetStream">The targetStream value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>The result.</returns>
    public static async Task CopyDataBlocksToStreamAsync(Stream sourceStream, Stream targetStream, CancellationToken cancellationToken = default)
    {
        int len;

        var buffer = new byte[MaxDataSubBlockByteCount];
        while ((len = await sourceStream.ReadByteAsync(cancellationToken)) > 0)
        {
            await sourceStream.ReadAllAsync(buffer, 0, len, cancellationToken).ConfigureAwait(false);
            await targetStream.WriteBufferAsync(buffer, 0, len, cancellationToken);
        }
    }

    /// <summary>Provides the ReadColorTableAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <param name="size">The size value.</param>
    /// <returns>The result.</returns>
    public static async Task<GifColor[]> ReadColorTableAsync(Stream stream, int size)
    {
        var length = GifColorByteCount * size;
        var bytes = new byte[length];
        await stream.ReadAllAsync(bytes, 0, length).ConfigureAwait(false);
        var colorTable = new GifColor[size];
        for (var i = 0; i < size; i++)
        {
            var colorOffset = GifColorByteCount * i;
            var r = bytes[colorOffset];
            var g = bytes[colorOffset + GifColorGreenOffset];
            var b = bytes[colorOffset + GifColorBlueOffset];
            colorTable[i] = new(r, g, b);
        }

        return colorTable;
    }

    /// <summary>Provides the IsNetscapeExtension member.</summary>
    /// <param name="ext">The ext value.</param>
    /// <returns>The result.</returns>
    public static bool IsNetscapeExtension(GifApplicationExtension ext) =>
        ext.ApplicationIdentifier == "NETSCAPE" && GetString(ext.AuthenticationCode) == "2.0";

    /// <summary>Provides the GetRepeatCount member.</summary>
    /// <param name="ext">The ext value.</param>
    /// <returns>The result.</returns>
    public static ushort GetRepeatCount(GifApplicationExtension ext)
    {
        return ext.Data is { Length: >= NetscapeRepeatCountDataLength } data ? BitConverter.ToUInt16(data, NetscapeRepeatCountOffset) : (ushort)1;
    }

    /// <summary>Provides the UnknownBlockTypeException member.</summary>
    /// <param name="blockId">The blockId value.</param>
    /// <returns>The result.</returns>
    public static Exception UnknownBlockTypeException(int blockId) =>
        new UnknownBlockTypeException("Unknown block type: 0x" + blockId.ToString("x2"));

    /// <summary>Provides the UnknownExtensionTypeException member.</summary>
    /// <param name="extensionLabel">The extensionLabel value.</param>
    /// <returns>The result.</returns>
    public static Exception UnknownExtensionTypeException(int extensionLabel) =>
        new UnknownExtensionTypeException("Unknown extension type: 0x" + extensionLabel.ToString("x2"));

    /// <summary>Provides the InvalidBlockSizeException member.</summary>
    /// <param name="blockName">The blockName value.</param>
    /// <param name="expectedBlockSize">The expectedBlockSize value.</param>
    /// <param name="actualBlockSize">The actualBlockSize value.</param>
    /// <returns>The result.</returns>
    public static Exception InvalidBlockSizeException(string blockName, int expectedBlockSize, int actualBlockSize) =>
        new InvalidBlockSizeException($"Invalid block size for {blockName}. Expected {expectedBlockSize}, but was {actualBlockSize}");

    /// <summary>Provides the InvalidSignatureException member.</summary>
    /// <param name="signature">The signature value.</param>
    /// <returns>The result.</returns>
    public static Exception InvalidSignatureException(string signature) =>
        new InvalidSignatureException("Invalid file signature: " + signature);

    /// <summary>Provides the UnsupportedVersionException member.</summary>
    /// <param name="version">The version value.</param>
    /// <returns>The result.</returns>
    public static Exception UnsupportedVersionException(string version) =>
        new UnsupportedGifVersionException("Unsupported version: " + version);

    /// <summary>Provides the GetString member.</summary>
    /// <param name="bytes">The bytes value.</param>
    /// <returns>The result.</returns>
    public static string GetString(byte[]? bytes) =>
       bytes is null ? string.Empty : GetString(bytes, 0, bytes.Length);

    /// <summary>Provides the GetString member.</summary>
    /// <param name="bytes">The bytes value.</param>
    /// <param name="index">The index value.</param>
    /// <param name="count">The count value.</param>
    /// <returns>The result.</returns>
    public static string GetString(byte[] bytes, int index, int count) =>
        Encoding.UTF8.GetString(bytes, index, count);
}
