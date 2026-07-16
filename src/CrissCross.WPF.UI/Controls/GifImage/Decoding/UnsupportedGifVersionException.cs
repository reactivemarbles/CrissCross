// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>Represents UnsupportedGifVersionException.</summary>
/// <seealso cref="GifDecoderException" />
[Serializable]
public class UnsupportedGifVersionException : GifDecoderException
{
    /// <summary>Initializes a new instance of the <see cref="UnsupportedGifVersionException"/> class.</summary>
    public UnsupportedGifVersionException() { }

    /// <summary>Initializes a new instance of the <see cref="UnsupportedGifVersionException"/> class.</summary>
    /// <param name="message">The message value.</param>
    public UnsupportedGifVersionException(string message)
        : base(message) { }

    /// <summary>Initializes a new instance of the <see cref="UnsupportedGifVersionException"/> class.</summary>
    /// <param name="message">The message value.</param>
    /// <param name="inner">The inner value.</param>
    public UnsupportedGifVersionException(string message, Exception inner)
        : base(message, inner) { }
}
