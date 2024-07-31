// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Text;

namespace CrissCross.WPF.UI.Controls.Decoding
{
    internal static class GifHelpers
    {
        public static string ReadString(Stream stream, int length)
        {
            var bytes = new byte[length];
            stream.ReadAll(bytes, 0, length);
            return Encoding.ASCII.GetString(bytes);
        }

        public static byte[]? ReadDataBlocks(Stream stream, bool discard)
        {
            var ms = discard ? null : new MemoryStream();
            using (ms)
            {
                int len;
                while ((len = stream.ReadByte()) > 0)
                {
                    var bytes = new byte[len];
                    stream.ReadAll(bytes, 0, len);
                    ms?.Write(bytes, 0, len);
                }

                if (ms != null)
                {
                    return ms.ToArray();
                }

                return null;
            }
        }

        public static GifColor[] ReadColorTable(Stream stream, int size)
        {
            var length = 3 * size;
            var bytes = new byte[length];
            stream.ReadAll(bytes, 0, length);
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
            ext.ApplicationIdentifier == "NETSCAPE"
                && Encoding.ASCII.GetString(ext.AuthenticationCode ?? []) == "2.0";

        public static ushort GetRepeatCount(GifApplicationExtension ext)
        {
            if (ext.Data?.Length >= 3)
            {
                return BitConverter.ToUInt16(ext.Data, 1);
            }

            return 1;
        }

        public static Exception UnexpectedEndOfStreamException() => new GifDecoderException("Unexpected end of stream before trailer was encountered");

        public static Exception UnknownBlockTypeException(int blockId) => new GifDecoderException("Unknown block type: 0x" + blockId.ToString("x2"));

        public static Exception UnknownExtensionTypeException(int extensionLabel) => new GifDecoderException("Unknown extension type: 0x" + extensionLabel.ToString("x2"));

        public static Exception InvalidBlockSizeException(string blockName, int expectedBlockSize, int actualBlockSize) => new GifDecoderException(
                string.Format(
                    "Invalid block size for {0}. Expected {1}, but was {2}",
                    blockName,
                    expectedBlockSize,
                    actualBlockSize));

        public static Exception InvalidSignatureException(string signature) => new GifDecoderException("Invalid file signature: " + signature);

        public static Exception UnsupportedVersionException(string version) => new GifDecoderException("Unsupported version: " + version);

        public static void ReadAll(this Stream stream, byte[] buffer, int offset, int count)
        {
            var totalRead = 0;
            while (totalRead < count)
            {
                totalRead += stream.Read(buffer, offset + totalRead, count - totalRead);
            }
        }
    }
}
