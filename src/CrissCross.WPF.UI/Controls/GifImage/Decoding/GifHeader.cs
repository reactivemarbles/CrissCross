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

    internal static GifHeader ReadHeader(Stream stream)
    {
        var header = new GifHeader();
        header.Read(stream);
        return header;
    }

    private void Read(Stream stream)
    {
        Signature = GifHelpers.ReadString(stream, 3);
        if (Signature != "GIF")
        {
            throw GifHelpers.InvalidSignatureException(Signature);
        }

        Version = GifHelpers.ReadString(stream, 3);
        if (Version != "87a" && Version != "89a")
        {
            throw GifHelpers.UnsupportedVersionException(Version);
        }

        LogicalScreenDescriptor = GifLogicalScreenDescriptor.ReadLogicalScreenDescriptor(stream);
    }
}
