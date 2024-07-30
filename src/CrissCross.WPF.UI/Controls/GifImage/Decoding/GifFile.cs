// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;

namespace CrissCross.WPF.UI.Controls.Decoding
{
    internal sealed class GifFile
    {
        private GifFile()
        {
        }

        public GifHeader? Header { get; private set; }

        public GifColor[]? GlobalColorTable { get; set; }

        public IList<GifFrame>? Frames { get; set; }

        public IList<GifExtension>? Extensions { get; set; }

        public ushort RepeatCount { get; set; }

        internal static GifFile ReadGifFile(Stream stream, bool metadataOnly)
        {
            var file = new GifFile();
            file.Read(stream, metadataOnly);
            return file;
        }

        private void Read(Stream stream, bool metadataOnly)
        {
            Header = GifHeader.ReadHeader(stream);

            if (Header.LogicalScreenDescriptor?.HasGlobalColorTable == true)
            {
                GlobalColorTable = GifHelpers.ReadColorTable(stream, Header.LogicalScreenDescriptor.GlobalColorTableSize);
            }

            ReadFrames(stream, metadataOnly);

            var netscapeExtension =
                            Extensions?
                                .OfType<GifApplicationExtension>()
                                .FirstOrDefault(GifHelpers.IsNetscapeExtension);

            if (netscapeExtension != null)
            {
                RepeatCount = GifHelpers.GetRepeatCount(netscapeExtension);
            }
            else
            {
                RepeatCount = 1;
            }
        }

        private void ReadFrames(Stream stream, bool metadataOnly)
        {
            List<GifFrame> frames = [];
            List<GifExtension> controlExtensions = [];
            List<GifExtension> specialExtensions = [];
            while (true)
            {
                var block = GifBlock.ReadBlock(stream, controlExtensions, metadataOnly);

                if (block.Kind == GifBlockKind.GraphicRendering)
                {
                    controlExtensions = [];
                }

                if (block is GifFrame gifFrame)
                {
                    frames.Add(gifFrame);
                }
                else if (block is GifExtension gifExtension)
                {
                    var extension = gifExtension;
                    switch (extension.Kind)
                    {
                        case GifBlockKind.Control:
                            controlExtensions.Add(extension);
                            break;
                        case GifBlockKind.SpecialPurpose:
                            specialExtensions.Add(extension);
                            break;
                    }
                }
                else if (block is GifTrailer)
                {
                    break;
                }
            }

            Frames = frames.AsReadOnly();
            Extensions = specialExtensions.AsReadOnly();
        }
    }
}
