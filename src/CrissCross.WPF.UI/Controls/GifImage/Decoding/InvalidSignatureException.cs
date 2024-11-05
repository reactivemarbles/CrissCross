// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>
/// InvalidSignatureException.
/// </summary>
/// <seealso cref="CrissCross.WPF.UI.Controls.Decoding.GifDecoderException" />
[Serializable]
public class InvalidSignatureException : GifDecoderException
{
    internal InvalidSignatureException(string message)
        : base(message)
    {
    }

    internal InvalidSignatureException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
