// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Decoding;
#else
namespace CrissCross.WPF.UI.Controls.Decoding;
#endif

/// <summary>Provides the GifTrailer member.</summary>
internal sealed class GifTrailer : GifBlock
{
    /// <summary>Provides the TrailerByte member.</summary>
    internal const int TrailerByte = 0x3B;

    /// <summary>Initializes a new instance of the <see cref="GifTrailer"/> class.</summary>
    private GifTrailer() { }

    internal override GifBlockKind Kind => GifBlockKind.Other;

    /// <summary>Provides the ReadAsync member.</summary>
    /// <returns>The result.</returns>
    internal static Task<GifTrailer> ReadAsync() => Task.FromResult(new GifTrailer());
}
