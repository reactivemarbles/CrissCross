// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>Provides the GifBlock member.</summary>
internal abstract class GifBlock
{
    /// <summary>Gets the Kind value.</summary>
    internal abstract GifBlockKind Kind { get; }

    /// <summary>Provides the ReadAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <param name="controlExtensions">The controlExtensions value.</param>
    /// <returns>The result.</returns>
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
