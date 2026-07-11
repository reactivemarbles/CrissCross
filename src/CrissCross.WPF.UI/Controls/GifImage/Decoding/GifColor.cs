// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>Provides the GifColor member.</summary>
/// <param name="R">The R value.</param>
/// <param name="G">The G value.</param>
/// <param name="B">The B value.</param>
internal readonly record struct GifColor(byte R, byte G, byte B)
{
    /// <summary>Gets the R value.</summary>
    public byte R { get; } = R;

    /// <summary>Gets the G value.</summary>
    public byte G { get; } = G;

    /// <summary>Gets the B value.</summary>
    public byte B { get; } = B;

    public override string ToString() => $"#{R:x2}{G:x2}{B:x2}";
}
