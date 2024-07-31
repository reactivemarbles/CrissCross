// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Text;

namespace CrissCross.WPF.UI.Controls.Decoding
{
    // label 0xFF
    internal sealed class GifApplicationExtension : GifExtension
    {
        internal const int ExtensionLabel = 0xFF;

        private GifApplicationExtension()
        {
        }

        public int BlockSize { get; private set; }

        public string? ApplicationIdentifier { get; private set; }

        public byte[]? AuthenticationCode { get; private set; }

        public byte[]? Data { get; private set; }

        internal override GifBlockKind Kind => GifBlockKind.SpecialPurpose;

        internal static GifApplicationExtension ReadApplication(Stream stream)
        {
            var ext = new GifApplicationExtension();
            ext.Read(stream);
            return ext;
        }

        private void Read(Stream stream)
        {
            // Note: at this point, the label (0xFF) has already been read
            var bytes = new byte[12];
            stream.ReadAll(bytes, 0, bytes.Length);
            BlockSize = bytes[0]; // should always be 11
            if (BlockSize != 11)
            {
                throw GifHelpers.InvalidBlockSizeException("Application Extension", 11, BlockSize);
            }

            ApplicationIdentifier = Encoding.ASCII.GetString(bytes, 1, 8);
            var authCode = new byte[3];
            Array.Copy(bytes, 9, authCode, 0, 3);
            AuthenticationCode = authCode;
            Data = GifHelpers.ReadDataBlocks(stream, false);
        }
    }
}
