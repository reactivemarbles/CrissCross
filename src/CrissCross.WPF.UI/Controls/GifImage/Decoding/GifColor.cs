// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Decoding;

internal readonly record struct GifColor(byte R, byte G, byte B)
{
    public byte R { get; } = R;

    public byte G { get; } = G;

    public byte B { get; } = B;

    public override string ToString() => $"#{R:x2}{G:x2}{B:x2}";
}
