// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>
/// GifDecoderException.
/// </summary>
/// <seealso cref="System.Exception" />
[Serializable]
public abstract class GifDecoderException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GifDecoderException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected GifDecoderException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GifDecoderException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner.</param>
    protected GifDecoderException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
