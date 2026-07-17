// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Decoding;
#else
namespace CrissCross.WPF.UI.Controls.Decoding;
#endif

/// <summary>Provides the GifCommentExtension member.</summary>
internal sealed class GifCommentExtension : GifExtension
{
    /// <summary>Provides the ExtensionLabel member.</summary>
    internal const int ExtensionLabel = 0xFE;

    /// <summary>Initializes a new instance of the <see cref="GifCommentExtension"/> class.</summary>
    private GifCommentExtension() { }

    /// <summary>Gets the Text value.</summary>
    public string? Text { get; private set; }

    internal override GifBlockKind Kind
    {
        get => GifBlockKind.SpecialPurpose;
    }

    /// <summary>Provides the ReadAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
    internal static async Task<GifCommentExtension> ReadAsync(Stream stream)
    {
        var comment = new GifCommentExtension();
        await comment.ReadInternalAsync(stream).ConfigureAwait(false);
        return comment;
    }

    /// <summary>Provides the ReadInternalAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
    private async Task ReadInternalAsync(Stream stream)
    {
        // Note: at this point, the label (0xFE) has already been read
        var bytes = await GifHelpers.ReadDataBlocksAsync(stream).ConfigureAwait(false);
        if (bytes is null)
        {
            return;
        }

        Text = GifHelpers.GetString(bytes);
    }
}
