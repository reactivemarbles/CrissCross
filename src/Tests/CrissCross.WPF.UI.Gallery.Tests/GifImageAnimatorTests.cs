// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Windows.Media.Animation;
using StandardImageAnimator = CrissCross.WPF.UI.Controls.ImageAnimator;
using UnsupportedGifVersionException = CrissCross.WPF.UI.Controls.Decoding.UnsupportedGifVersionException;

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Exercises GIF image animator regression paths.</summary>
public sealed class GifImageAnimatorTests
{
    /// <summary>The first byte in the GIF file signature.</summary>
    private const byte GifSignatureFirstByte = 0x47;

    /// <summary>The second byte in the GIF file signature.</summary>
    private const byte GifSignatureSecondByte = 0x49;

    /// <summary>The third byte in the GIF file signature.</summary>
    private const byte GifSignatureThirdByte = 0x46;

    /// <summary>The first byte in the unsupported GIF version.</summary>
    private const byte UnsupportedVersionFirstByte = 0x30;

    /// <summary>The second byte in the unsupported GIF version.</summary>
    private const byte UnsupportedVersionSecondByte = 0x30;

    /// <summary>The third byte in the unsupported GIF version.</summary>
    private const byte UnsupportedVersionThirdByte = 0x30;

    /// <summary>Verifies the stream factory delegates to decoding and returns the awaited decoder fault.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task StreamFactory_WithUnsupportedGifVersion_ThrowsDecoderException()
    {
        var exception = await CaptureCreateAsyncExceptionAsync();

        await Assert.That(exception).IsTypeOf<UnsupportedGifVersionException>();
    }

    /// <summary>Creates a minimal GIF stream with an unsupported version.</summary>
    /// <returns>The stream used by the factory regression test.</returns>
    private static MemoryStream CreateUnsupportedVersionGifStream() => new(
        [
            GifSignatureFirstByte,
            GifSignatureSecondByte,
            GifSignatureThirdByte,
            UnsupportedVersionFirstByte,
            UnsupportedVersionSecondByte,
            UnsupportedVersionThirdByte,
        ]);

    /// <summary>Captures the exception thrown by the awaited stream factory.</summary>
    /// <returns>The captured exception, or <see langword="null"/> when no exception was thrown.</returns>
    private static async Task<Exception?> CaptureCreateAsyncExceptionAsync()
    {
        await using MemoryStream stream = CreateUnsupportedVersionGifStream();
        try
        {
            using StandardImageAnimator animator = await StandardImageAnimator
                .CreateAsync(stream, RepeatBehavior.Forever, null!)
                .ConfigureAwait(false);
            return null;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }
}
