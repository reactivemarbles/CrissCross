// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;

namespace CrissCross.WPF.UI.Controls.Decoding;

internal abstract class GifExtension : GifBlock
{
    internal const int ExtensionIntroducer = 0x21;

    internal static new async Task<GifExtension> ReadAsync(Stream stream, IEnumerable<GifExtension> controlExtensions)
    {
        // Note: at this point, the Extension Introducer (0x21) has already been read
        var label = stream.ReadByte();
        if (label < 0)
        {
            throw new EndOfStreamException();
        }

        return label switch
        {
            GifGraphicControlExtension.ExtensionLabel => await GifGraphicControlExtension.ReadAsync(stream).ConfigureAwait(false),
            GifCommentExtension.ExtensionLabel => await GifCommentExtension.ReadAsync(stream).ConfigureAwait(false),
            GifPlainTextExtension.ExtensionLabel => await GifPlainTextExtension.ReadAsync(stream, controlExtensions).ConfigureAwait(false),
            GifApplicationExtension.ExtensionLabel => await GifApplicationExtension.ReadAsync(stream).ConfigureAwait(false),
            _ => throw GifHelpers.UnknownExtensionTypeException(label),
        };
    }
}
