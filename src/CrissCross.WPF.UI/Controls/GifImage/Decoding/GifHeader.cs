// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;

namespace CrissCross.WPF.UI.Controls.Decoding;

internal sealed class GifHeader : GifBlock
{
    private GifHeader()
    {
    }

    public string? Signature { get; private set; }

    public string? Version { get; private set; }

    public GifLogicalScreenDescriptor? LogicalScreenDescriptor { get; private set; }

    internal override GifBlockKind Kind => GifBlockKind.Other;

    internal static async Task<GifHeader> ReadAsync(Stream stream)
    {
        var header = new GifHeader();
        await header.ReadInternalAsync(stream).ConfigureAwait(false);
        return header;
    }

    private async Task ReadInternalAsync(Stream stream)
    {
        Signature = await GifHelpers.ReadStringAsync(stream, 3).ConfigureAwait(false);
        if (Signature != "GIF")
        {
            throw GifHelpers.InvalidSignatureException(Signature);
        }

        Version = await GifHelpers.ReadStringAsync(stream, 3).ConfigureAwait(false);
        if (Version != "87a" && Version != "89a")
        {
            throw GifHelpers.UnsupportedVersionException(Version);
        }

        LogicalScreenDescriptor = await GifLogicalScreenDescriptor.ReadAsync(stream).ConfigureAwait(false);
    }
}
