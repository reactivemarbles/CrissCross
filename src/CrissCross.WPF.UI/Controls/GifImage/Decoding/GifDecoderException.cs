// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Decoding;
#else
namespace CrissCross.WPF.UI.Controls.Decoding;
#endif

/// <summary>Represents GifDecoderException.</summary>
/// <seealso cref="System.Exception" />
[Serializable]
public class GifDecoderException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="GifDecoderException"/> class.</summary>
    /// <param name="message">The message that describes the error.</param>
    public GifDecoderException(string message)
        : base(message) { }

    /// <summary>Initializes a new instance of the <see cref="GifDecoderException"/> class.</summary>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner.</param>
    public GifDecoderException(string message, Exception inner)
        : base(message, inner) { }

    /// <summary>Initializes a new instance of the <see cref="GifDecoderException"/> class.</summary>
    public GifDecoderException() { }
}
