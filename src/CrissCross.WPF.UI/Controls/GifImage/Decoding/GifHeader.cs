// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>Provides the GifHeader member.</summary>
internal sealed class GifHeader : GifBlock
{
    /// <summary>Initializes a new instance of the <see cref="GifHeader"/> class.</summary>
    private GifHeader()
    {
    }

    /// <summary>Gets the Signature value.</summary>
    public string? Signature { get; private set; }

    /// <summary>Gets the Version value.</summary>
    public string? Version { get; private set; }

    /// <summary>Gets the LogicalScreenDescriptor value.</summary>
    public GifLogicalScreenDescriptor? LogicalScreenDescriptor { get; private set; }

    internal override GifBlockKind Kind => GifBlockKind.Other;

    /// <summary>Provides the ReadAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
    internal static async Task<GifHeader> ReadAsync(Stream stream)
    {
        var header = new GifHeader();
        await header.ReadInternalAsync(stream).ConfigureAwait(false);
        return header;
    }

    /// <summary>Provides the ReadInternalAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
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
