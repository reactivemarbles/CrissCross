// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.UI.Controls.Extensions;
#else
using CrissCross.WPF.UI.Controls.Extensions;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Decoding;
#else
namespace CrissCross.WPF.UI.Controls.Decoding;
#endif

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
            GifExtension.ExtensionIntroducer => await GifExtension
                .ReadAsync(stream, controlExtensions)
                .ConfigureAwait(false),
            GifFrame.ImageSeparator => await GifFrame.ReadAsync(stream, controlExtensions).ConfigureAwait(false),
            GifTrailer.TrailerByte => await GifTrailer.ReadAsync().ConfigureAwait(false),
            _ => throw GifHelpers.UnknownBlockTypeException(blockId),
        };
    }
}
