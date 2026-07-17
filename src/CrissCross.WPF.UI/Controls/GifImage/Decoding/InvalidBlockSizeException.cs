// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Decoding;
#else
namespace CrissCross.WPF.UI.Controls.Decoding;
#endif

/// <summary>Represents InvalidBlockSizeException.</summary>
/// <seealso cref="GifDecoderException" />
[Serializable]
public class InvalidBlockSizeException : GifDecoderException
{
    /// <summary>Initializes a new instance of the <see cref="InvalidBlockSizeException"/> class.</summary>
    public InvalidBlockSizeException() { }

    /// <summary>Initializes a new instance of the <see cref="InvalidBlockSizeException"/> class.</summary>
    /// <param name="message">The message value.</param>
    public InvalidBlockSizeException(string message)
        : base(message) { }

    /// <summary>Initializes a new instance of the <see cref="InvalidBlockSizeException"/> class.</summary>
    /// <param name="message">The message value.</param>
    /// <param name="inner">The inner value.</param>
    public InvalidBlockSizeException(string message, Exception inner)
        : base(message, inner) { }
}
