// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>
/// UnknownBlockTypeException.
/// </summary>
/// <seealso cref="CrissCross.WPF.UI.Controls.Decoding.GifDecoderException" />
[Serializable]
public class UnknownBlockTypeException : GifDecoderException
{
    internal UnknownBlockTypeException(string message)
        : base(message)
    {
    }

    internal UnknownBlockTypeException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
