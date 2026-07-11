// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>Provides the GifImageDescriptor member.</summary>
internal sealed class GifImageDescriptor : IGifRect
{
    /// <summary>Initializes a new instance of the <see cref="GifImageDescriptor"/> class.</summary>
    private GifImageDescriptor()
    {
    }

    /// <summary>Gets the Left value.</summary>
    public int Left { get; private set; }

    /// <summary>Gets the Top value.</summary>
    public int Top { get; private set; }

    /// <summary>Gets the Width value.</summary>
    public int Width { get; private set; }

    /// <summary>Gets the Height value.</summary>
    public int Height { get; private set; }

    /// <summary>Gets the HasLocalColorTable value.</summary>
    public bool HasLocalColorTable { get; private set; }

    /// <summary>Gets the Interlace value.</summary>
    public bool Interlace { get; private set; }

    /// <summary>Gets the IsLocalColorTableSorted value.</summary>
    public bool IsLocalColorTableSorted { get; private set; }

    /// <summary>Gets the LocalColorTableSize value.</summary>
    public int LocalColorTableSize { get; private set; }

    /// <summary>Provides the ReadAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
    internal static async Task<GifImageDescriptor> ReadAsync(Stream stream)
    {
        var descriptor = new GifImageDescriptor();
        await descriptor.ReadInternalAsync(stream).ConfigureAwait(false);
        return descriptor;
    }

    /// <summary>Provides the ReadInternalAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
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
