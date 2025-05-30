﻿// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>
/// UnknownExtensionTypeException.
/// </summary>
/// <seealso cref="CrissCross.WPF.UI.Controls.Decoding.GifDecoderException" />
[Serializable]
public class UnknownExtensionTypeException : GifDecoderException
{
    internal UnknownExtensionTypeException(string message)
        : base(message)
    {
    }

    internal UnknownExtensionTypeException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
