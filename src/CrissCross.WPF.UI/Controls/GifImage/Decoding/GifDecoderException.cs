// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>
/// Gif Decoder Exception.
/// </summary>
/// <seealso cref="System.Exception" />
[Serializable]
public class GifDecoderException : Exception
{
    internal GifDecoderException()
    {
    }

    internal GifDecoderException(string message)
        : base(message)
    {
    }

    internal GifDecoderException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
