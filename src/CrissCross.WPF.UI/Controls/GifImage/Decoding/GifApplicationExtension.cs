// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.UI.Controls.Extensions;
#else
using CrissCross.WPF.UI.Controls.Extensions;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Decoding;
#else
namespace CrissCross.WPF.UI.Controls.Decoding;
#endif

/// <summary>Label 0xFF.</summary>
internal sealed class GifApplicationExtension : GifExtension
{
    /// <summary>Provides the ExtensionLabel member.</summary>
    internal const int ExtensionLabel = 0xFF;

    /// <summary>The application extension block size.</summary>
    private const int ApplicationBlockSize = 11;

    /// <summary>The full application extension header byte count.</summary>
    private const int ApplicationHeaderByteCount = ApplicationBlockSize + 1;

    /// <summary>The application identifier offset.</summary>
    private const int ApplicationIdentifierOffset = 1;

    /// <summary>The application identifier byte count.</summary>
    private const int ApplicationIdentifierByteCount = 8;

    /// <summary>The authentication code offset.</summary>
    private const int AuthenticationCodeOffset = 9;

    /// <summary>The authentication code byte count.</summary>
    private const int AuthenticationCodeByteCount = 3;

    /// <summary>Initializes a new instance of the <see cref="GifApplicationExtension"/> class.</summary>
    private GifApplicationExtension() { }

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
        var bytes = new byte[ApplicationHeaderByteCount];
        await stream.ReadAllAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
        BlockSize = bytes[0]; // should always be 11
        if (BlockSize != ApplicationBlockSize)
        {
            throw GifHelpers.InvalidBlockSizeException("Application Extension", ApplicationBlockSize, BlockSize);
        }

        ApplicationIdentifier = GifHelpers.GetString(
            bytes,
            ApplicationIdentifierOffset,
            ApplicationIdentifierByteCount);
        var authCode = new byte[AuthenticationCodeByteCount];
        Array.Copy(bytes, AuthenticationCodeOffset, authCode, 0, AuthenticationCodeByteCount);
        AuthenticationCode = authCode;
        Data = await GifHelpers.ReadDataBlocksAsync(stream).ConfigureAwait(false);
    }
}
