// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Decoding;
#else
namespace CrissCross.WPF.UI.Controls.Decoding;
#endif

/// <summary>Represents InvalidBlockSizeException.</summary>
/// <seealso cref="GifDecoderException" />
[Serializable]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
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

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
