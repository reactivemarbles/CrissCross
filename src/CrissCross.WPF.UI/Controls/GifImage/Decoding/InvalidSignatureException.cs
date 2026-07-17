// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>Represents InvalidSignatureException.</summary>
/// <seealso cref="GifDecoderException" />
[Serializable]
public class InvalidSignatureException : GifDecoderException
{
    /// <summary>Initializes a new instance of the <see cref="InvalidSignatureException"/> class.</summary>
    public InvalidSignatureException() { }

    /// <summary>Initializes a new instance of the <see cref="InvalidSignatureException"/> class.</summary>
    /// <param name="message">The message value.</param>
    public InvalidSignatureException(string message)
        : base(message) { }

    /// <summary>Initializes a new instance of the <see cref="InvalidSignatureException"/> class.</summary>
    /// <param name="message">The message value.</param>
    /// <param name="inner">The inner value.</param>
    public InvalidSignatureException(string message, Exception inner)
        : base(message, inner) { }
}
