// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls.Decoding;

internal abstract class GifBlock
{
    internal abstract GifBlockKind Kind { get; }

    internal static async Task<GifBlock> ReadAsync(Stream stream, IEnumerable<GifExtension> controlExtensions)
    {
        var blockId = await stream.ReadByteAsync().ConfigureAwait(false);
        if (blockId < 0)
        {
            throw new EndOfStreamException();
        }

        return blockId switch
        {
            GifExtension.ExtensionIntroducer => await GifExtension.ReadAsync(stream, controlExtensions).ConfigureAwait(false),
            GifFrame.ImageSeparator => await GifFrame.ReadAsync(stream, controlExtensions).ConfigureAwait(false),
            GifTrailer.TrailerByte => await GifTrailer.ReadAsync().ConfigureAwait(false),
            _ => throw GifHelpers.UnknownBlockTypeException(blockId),
        };
    }
}
