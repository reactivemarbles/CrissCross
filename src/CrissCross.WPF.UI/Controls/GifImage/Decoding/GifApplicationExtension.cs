// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>Label 0xFF.</summary>
internal sealed class GifApplicationExtension : GifExtension
{
    /// <summary>Provides the ExtensionLabel member.</summary>
    internal const int ExtensionLabel = 0xFF;

    /// <summary>Initializes a new instance of the <see cref="GifApplicationExtension"/> class.</summary>
    private GifApplicationExtension()
    {
    }

    /// <summary>Gets the BlockSize value.</summary>
    public int BlockSize { get; private set; }

    /// <summary>Gets the ApplicationIdentifier value.</summary>
    public string? ApplicationIdentifier { get; private set; }

    /// <summary>Gets the AuthenticationCode value.</summary>
    public byte[]? AuthenticationCode { get; private set; }

    /// <summary>Gets the Data value.</summary>
    public byte[]? Data { get; private set; }

    internal override GifBlockKind Kind => GifBlockKind.SpecialPurpose;

    /// <summary>Provides the ReadAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
    internal static async Task<GifApplicationExtension> ReadAsync(Stream stream)
    {
        var ext = new GifApplicationExtension();
        await ext.ReadInternalAsync(stream).ConfigureAwait(false);
        return ext;
    }

    /// <summary>Provides the ReadInternalAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
    private async Task ReadInternalAsync(Stream stream)
    {
        // Note: at this point, the label (0xFF) has already been read
        var bytes = new byte[12];
        await stream.ReadAllAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
        BlockSize = bytes[0]; // should always be 11
        if (BlockSize != 11)
        {
            throw GifHelpers.InvalidBlockSizeException("Application Extension", 11, BlockSize);
        }

        ApplicationIdentifier = GifHelpers.GetString(bytes, 1, 8);
        var authCode = new byte[3];
        Array.Copy(bytes, 9, authCode, 0, 3);
        AuthenticationCode = authCode;
        Data = await GifHelpers.ReadDataBlocksAsync(stream).ConfigureAwait(false);
    }
}
