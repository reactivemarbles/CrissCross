// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Decoding;
#else
namespace CrissCross.WPF.UI.Controls.Decoding;
#endif

/// <summary>Represents GifDecoderException.</summary>
/// <seealso cref="System.Exception" />
[Serializable]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
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

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
