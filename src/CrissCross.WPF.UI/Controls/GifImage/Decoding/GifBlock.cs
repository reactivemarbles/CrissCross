// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;

namespace CrissCross.WPF.UI.Controls.Decoding;

internal abstract class GifBlock
{
    internal abstract GifBlockKind Kind { get; }

    internal static GifBlock ReadBlock(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
    {
        var blockId = stream.ReadByte();
        if (blockId < 0)
        {
            throw GifHelpers.UnexpectedEndOfStreamException();
        }

        return blockId switch
        {
            GifExtension.ExtensionIntroducer => GifExtension.ReadExtension(stream, controlExtensions, metadataOnly),
            GifFrame.ImageSeparator => GifFrame.ReadFrame(stream, controlExtensions, metadataOnly),
            GifTrailer.TrailerByte => GifTrailer.ReadTrailer(),
            _ => throw GifHelpers.UnknownBlockTypeException(blockId),
        };
    }
}
