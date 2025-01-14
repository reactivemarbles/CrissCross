// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Decoding;

internal sealed class GifTrailer : GifBlock
{
    internal const int TrailerByte = 0x3B;

    private GifTrailer()
    {
    }

    internal override GifBlockKind Kind => GifBlockKind.Other;

    internal static Task<GifTrailer> ReadAsync() => Task.FromResult(new GifTrailer());
}
