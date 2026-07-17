// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Decoding;
#else
namespace CrissCross.WPF.UI.Controls.Decoding;
#endif

/// <summary>Provides the GifExtension member.</summary>
internal abstract class GifExtension : GifBlock
{
    /// <summary>Provides the ExtensionIntroducer member.</summary>
    internal const int ExtensionIntroducer = 0x21;

    /// <summary>Provides the ReadAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <param name="controlExtensions">The controlExtensions value.</param>
    /// <returns>The result.</returns>
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
            GifGraphicControlExtension.ExtensionLabel => await GifGraphicControlExtension
                .ReadAsync(stream)
                .ConfigureAwait(false),
            GifCommentExtension.ExtensionLabel => await GifCommentExtension.ReadAsync(stream).ConfigureAwait(false),
            GifPlainTextExtension.ExtensionLabel => await GifPlainTextExtension
                .ReadAsync(stream, controlExtensions)
                .ConfigureAwait(false),
            GifApplicationExtension.ExtensionLabel => await GifApplicationExtension
                .ReadAsync(stream)
                .ConfigureAwait(false),
            _ => throw GifHelpers.UnknownExtensionTypeException(label),
        };
    }
}
