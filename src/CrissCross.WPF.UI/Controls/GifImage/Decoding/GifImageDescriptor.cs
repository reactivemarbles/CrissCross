// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls.Decoding;

internal sealed class GifImageDescriptor : IGifRect
{
    private GifImageDescriptor()
    {
    }

    public int Left { get; private set; }

    public int Top { get; private set; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public bool HasLocalColorTable { get; private set; }

    public bool Interlace { get; private set; }

    public bool IsLocalColorTableSorted { get; private set; }

    public int LocalColorTableSize { get; private set; }

    internal static async Task<GifImageDescriptor> ReadAsync(Stream stream)
    {
        var descriptor = new GifImageDescriptor();
        await descriptor.ReadInternalAsync(stream).ConfigureAwait(false);
        return descriptor;
    }

    private async Task ReadInternalAsync(Stream stream)
    {
        var bytes = new byte[9];
        await stream.ReadAllAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
        Left = BitConverter.ToUInt16(bytes, 0);
        Top = BitConverter.ToUInt16(bytes, 2);
        Width = BitConverter.ToUInt16(bytes, 4);
        Height = BitConverter.ToUInt16(bytes, 6);
        var packedFields = bytes[8];
        HasLocalColorTable = (packedFields & 0x80) != 0;
        Interlace = (packedFields & 0x40) != 0;
        IsLocalColorTableSorted = (packedFields & 0x20) != 0;
        LocalColorTableSize = 1 << ((packedFields & 0x07) + 1);
    }
}
