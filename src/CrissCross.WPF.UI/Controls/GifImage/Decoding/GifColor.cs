// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Decoding;
#else
namespace CrissCross.WPF.UI.Controls.Decoding;
#endif

/// <summary>Provides the GifColor member.</summary>
/// <param name="R">The R value.</param>
/// <param name="G">The G value.</param>
/// <param name="B">The B value.</param>
public readonly record struct GifColor(byte R, byte G, byte B)
{
    /// <summary>Gets the R value.</summary>
    public byte R { get; } = R;

    /// <summary>Gets the G value.</summary>
    public byte G { get; } = G;

    /// <summary>Gets the B value.</summary>
    public byte B { get; } = B;

    public override string ToString() => $"#{R:x2}{G:x2}{B:x2}";
}
