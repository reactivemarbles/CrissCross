// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Text;
using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls.Decoding;

internal static class GifHelpers
{
    public static async Task<string> ReadStringAsync(Stream stream, int length)
    {
        var bytes = new byte[length];
        await stream.ReadAllAsync(bytes, 0, length).ConfigureAwait(false);
        return GetString(bytes);
    }

    public static async Task ConsumeDataBlocksAsync(Stream sourceStream, CancellationToken cancellationToken = default) => await CopyDataBlocksToStreamAsync(sourceStream, Stream.Null, cancellationToken);

    public static async Task<byte[]> ReadDataBlocksAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var ms = new MemoryStream();
        await CopyDataBlocksToStreamAsync(stream, ms, cancellationToken);
        return ms.ToArray();
    }

    public static async Task CopyDataBlocksToStreamAsync(Stream sourceStream, Stream targetStream, CancellationToken cancellationToken = default)
    {
        int len;

        // the length is on 1 byte, so each data sub-block can't be more than 255 bytes long
        var buffer = new byte[255];
        while ((len = await sourceStream.ReadByteAsync(cancellationToken)) > 0)
        {
            await sourceStream.ReadAllAsync(buffer, 0, len, cancellationToken).ConfigureAwait(false);
#if LACKS_STREAM_MEMORY_OVERLOADS
            await targetStream.WriteAsync(buffer, 0, len, cancellationToken);
#else
            await targetStream.WriteAsync(buffer.AsMemory(0, len), cancellationToken);
#endif
        }
    }

    public static async Task<GifColor[]> ReadColorTableAsync(Stream stream, int size)
    {
        var length = 3 * size;
        var bytes = new byte[length];
        await stream.ReadAllAsync(bytes, 0, length).ConfigureAwait(false);
        var colorTable = new GifColor[size];
        for (var i = 0; i < size; i++)
        {
            var r = bytes[3 * i];
            var g = bytes[(3 * i) + 1];
            var b = bytes[(3 * i) + 2];
            colorTable[i] = new GifColor(r, g, b);
        }

        return colorTable;
    }

    public static bool IsNetscapeExtension(GifApplicationExtension ext) =>
        ext.ApplicationIdentifier == "NETSCAPE" && GetString(ext.AuthenticationCode) == "2.0";

    public static ushort GetRepeatCount(GifApplicationExtension ext)
    {
        if (ext.Data?.Length >= 3)
        {
            return BitConverter.ToUInt16(ext.Data, 1);
        }

        return 1;
    }

    public static Exception UnknownBlockTypeException(int blockId) =>
        new UnknownBlockTypeException("Unknown block type: 0x" + blockId.ToString("x2"));

    public static Exception UnknownExtensionTypeException(int extensionLabel) =>
        new UnknownExtensionTypeException("Unknown extension type: 0x" + extensionLabel.ToString("x2"));

    public static Exception InvalidBlockSizeException(string blockName, int expectedBlockSize, int actualBlockSize) =>
        new InvalidBlockSizeException($"Invalid block size for {blockName}. Expected {expectedBlockSize}, but was {actualBlockSize}");

    public static Exception InvalidSignatureException(string signature) =>
        new InvalidSignatureException("Invalid file signature: " + signature);

    public static Exception UnsupportedVersionException(string version) =>
        new UnsupportedGifVersionException("Unsupported version: " + version);

    public static string GetString(byte[]? bytes) =>
       bytes == null ? string.Empty : GetString(bytes, 0, bytes.Length);

    public static string GetString(byte[] bytes, int index, int count) =>
        Encoding.UTF8.GetString(bytes, index, count);
}
