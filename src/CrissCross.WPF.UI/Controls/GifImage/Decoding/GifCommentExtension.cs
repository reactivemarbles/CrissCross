// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Text;

namespace CrissCross.WPF.UI.Controls.Decoding
{
    internal sealed class GifCommentExtension : GifExtension
    {
        internal const int ExtensionLabel = 0xFE;

        private GifCommentExtension()
        {
        }

        public string? Text { get; private set; }

        internal override GifBlockKind Kind => GifBlockKind.SpecialPurpose;

        internal static GifCommentExtension ReadComment(Stream stream)
        {
            var comment = new GifCommentExtension();
            comment.Read(stream);
            return comment;
        }

        private void Read(Stream stream)
        {
            // Note: at this point, the label (0xFE) has already been read
            var bytes = GifHelpers.ReadDataBlocks(stream, false);
            if (bytes != null)
            {
                Text = Encoding.ASCII.GetString(bytes);
            }
        }
    }
}
