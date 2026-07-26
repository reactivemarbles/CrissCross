// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Decoding;
#else
namespace CrissCross.WPF.UI.Controls.Decoding;
#endif

/// <summary>Provides the GifImageData member.</summary>
internal sealed class GifImageData
{
    /// <summary>Initializes a new instance of the <see cref="GifImageData"/> class.</summary>
    private GifImageData() { }

    /// <summary>Gets or sets LzwMinimumCodeSize.</summary>
    internal byte LzwMinimumCodeSize { get; set; }

    /// <summary>Gets or sets CompressedDataStartOffset.</summary>
    internal long CompressedDataStartOffset { get; set; }

    /// <summary>Provides the ReadAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
    internal static async Task<GifImageData> ReadAsync(Stream stream)
    {
        var imgData = new GifImageData();
        await imgData.ReadInternalAsync(stream).ConfigureAwait(false);
        return imgData;
    }

    /// <summary>Provides the ReadInternalAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
    private async Task ReadInternalAsync(Stream stream)
    {
        LzwMinimumCodeSize = (byte)stream.ReadByte();
        CompressedDataStartOffset = stream.Position;
        await GifHelpers.ConsumeDataBlocksAsync(stream).ConfigureAwait(false);
    }
}
