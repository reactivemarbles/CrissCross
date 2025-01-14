// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Decoding;

internal class GifCommentExtension : GifExtension
{
    internal const int ExtensionLabel = 0xFE;

    private GifCommentExtension()
    {
    }

    public string? Text { get; private set; }

    internal override GifBlockKind Kind
    {
        get { return GifBlockKind.SpecialPurpose; }
    }

    internal static async Task<GifCommentExtension> ReadAsync(Stream stream)
    {
        var comment = new GifCommentExtension();
        await comment.ReadInternalAsync(stream).ConfigureAwait(false);
        return comment;
    }

    private async Task ReadInternalAsync(Stream stream)
    {
        // Note: at this point, the label (0xFE) has already been read
        var bytes = await GifHelpers.ReadDataBlocksAsync(stream).ConfigureAwait(false);
        if (bytes != null)
        {
            Text = GifHelpers.GetString(bytes);
        }
    }
}
